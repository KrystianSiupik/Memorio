import 'dart:math' show Point;

import 'package:flutter/foundation.dart';
import 'package:flutter/services.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:maplibre_gl/maplibre_gl.dart';

import '../domain/map_point.dart';
import 'map_providers.dart';

/// Where the base map (`basemap.pmtiles`, fonts) is served from, set at build time:
/// `--dart-define=MAP_BASE_URL=https://...`. Empty means `map/` next to the web app.
const _mapBaseUrl = String.fromEnvironment('MAP_BASE_URL');

const _pointsSource = 'points';
const _clusterLayer = 'point-clusters';
const _clusterCountLayer = 'point-cluster-counts';
const _singlePointLayer = 'single-points';

/// Layers drawing the points, for tests of what is on the screen.
const mapPointLayers = [_clusterLayer, _singlePointLayer];

/// The points as GeoJSON for a clustered source; clustering happens in the client (D-032).
Map<String, Object> pointsToGeoJson(List<MapPoint> points) => {
  'type': 'FeatureCollection',
  'features': [
    for (final point in points)
      {
        'type': 'Feature',
        'id': point.id,
        // GL JS drops string feature ids; the source promotes this property back to the id.
        'properties': <String, Object>{'id': point.id},
        'geometry': {
          'type': 'Point',
          'coordinates': [point.longitude, point.latitude],
        },
      },
  ],
};

/// MapLibre with the self-hosted base map and clustered points: a globe on the web, flat on
/// Android and iOS, where MapLibre Native renders only the mercator projection (D-191).
class MapLibreMapView extends ConsumerStatefulWidget {
  const MapLibreMapView({super.key, this.onPointsAdded, this.onIdle});

  /// Called when the point layers are on the map (they may not be drawn yet).
  final void Function(MapLibreMapController controller)? onPointsAdded;

  /// Called every time the map finishes drawing.
  final VoidCallback? onIdle;

  @override
  ConsumerState<MapLibreMapView> createState() => _MapLibreMapViewState();
}

class _MapLibreMapViewState extends ConsumerState<MapLibreMapView> {
  late final Future<String> _style = _loadStyle();
  MapLibreMapController? _controller;
  var _layersReady = false;

  Future<String> _loadStyle() async {
    final base = _mapBaseUrl.isEmpty ? Uri.base.resolve('map').toString() : _mapBaseUrl;
    final style = await rootBundle.loadString('assets/map/style.json');
    return style.replaceAll('{{base}}', base);
  }

  Future<void> _addPoints() async {
    final controller = _controller!;
    final points = ref.read(mapPointsProvider);
    if (kIsWeb) {
      await controller.setProjection('globe');
    }
    await controller.addSource(
      _pointsSource,
      GeojsonSourceProperties(
        data: pointsToGeoJson(points),
        promoteId: 'id',
        cluster: true,
        clusterRadius: 50,
        clusterMaxZoom: 14,
      ),
    );
    await controller.addCircleLayer(
      _pointsSource,
      _clusterLayer,
      const CircleLayerProperties(
        circleColor: '#00796b',
        circleOpacity: 0.85,
        circleRadius: [
          'step',
          ['get', 'point_count'],
          14,
          100,
          20,
          1000,
          26,
        ],
      ),
      filter: ['has', 'point_count'],
    );
    await controller.addSymbolLayer(
      _pointsSource,
      _clusterCountLayer,
      const SymbolLayerProperties(
        textField: ['get', 'point_count_abbreviated'],
        textFont: ['Noto Sans Regular'],
        textSize: 12,
        textColor: '#ffffff',
        textAllowOverlap: true,
      ),
      filter: ['has', 'point_count'],
    );
    await controller.addCircleLayer(
      _pointsSource,
      _singlePointLayer,
      const CircleLayerProperties(circleColor: '#00796b', circleRadius: 6),
      filter: [
        '!',
        ['has', 'point_count'],
      ],
    );
    _layersReady = true;
    controller.onFeatureTapped.add(_featureTapped);
    final latest = ref.read(mapPointsProvider);
    if (!identical(latest, points)) {
      await controller.setGeoJsonSource(_pointsSource, pointsToGeoJson(latest));
    }
    widget.onPointsAdded?.call(controller);
  }

  void _featureTapped(Point<double> point, LatLng coordinates, String id, String layerId, Annotation? annotation) {
    if (layerId == _singlePointLayer) {
      ref.read(tappedPointProvider.notifier).tap(id);
    } else if (layerId == _clusterLayer || layerId == _clusterCountLayer) {
      final controller = _controller!;
      controller.animateCamera(CameraUpdate.newLatLngZoom(coordinates, (controller.cameraPosition?.zoom ?? 1) + 2));
    }
  }

  @override
  void dispose() {
    _controller?.onFeatureTapped.remove(_featureTapped);
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    ref.listen(mapFocusProvider, (previous, point) {
      if (point != null) {
        _controller?.animateCamera(
          CameraUpdate.newLatLngZoom(LatLng(point.latitude, point.longitude), 9),
          duration: const Duration(seconds: 2),
        );
      }
    });
    ref.listen(mapPointsProvider, (previous, points) {
      if (_layersReady) {
        _controller?.setGeoJsonSource(_pointsSource, pointsToGeoJson(points));
      }
    });
    return FutureBuilder<String>(
      future: _style,
      builder: (context, style) {
        final json = style.data;
        if (json == null) {
          return const SizedBox.expand();
        }
        return MapLibreMap(
          styleString: json,
          trackCameraPosition: true,
          initialCameraPosition: const CameraPosition(target: LatLng(30, 15), zoom: 1.2),
          onMapCreated: (controller) => _controller = controller,
          onStyleLoadedCallback: _addPoints,
          onMapIdle: widget.onIdle,
        );
      },
    );
  }
}

/// Serves MapLibre GL JS from the app's own origin instead of the plugin's default CDN (D-032).
void useSelfHostedMapLibre() {
  MapLibreMap.webLibrarySource = MapLibreJsSource.urls(
    scriptUrl: Uri.base.resolve('vendor/maplibre/memorio-map.mjs').toString(),
    styleUrl: Uri.base.resolve('vendor/maplibre/maplibre-gl.css').toString(),
  );
}
