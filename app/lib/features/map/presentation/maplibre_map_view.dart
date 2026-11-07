import 'dart:math' show Point;

import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/services.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:maplibre_gl/maplibre_gl.dart';

import '../../../core/settings/settings.dart';
import '../domain/map_point.dart';
import 'map_context_menu.dart' if (dart.library.js_interop) 'map_context_menu_web.dart';
import 'map_providers.dart';
import 'photo_marker.dart';

/// Where the base map (`basemap.pmtiles`, fonts) is served from, set at build time:
/// `--dart-define=MAP_BASE_URL=https://...`. Empty means `map/` next to the web app.
const _mapBaseUrl = String.fromEnvironment('MAP_BASE_URL');

const _pointsSource = 'points';
const _clusterLayer = 'point-clusters';
const _clusterCountLayer = 'point-cluster-counts';
const _singlePointLayer = 'single-points';
const _photoPointLayer = 'photo-points';

/// Layers drawing the points, for tests of what is on the screen.
const mapPointLayers = [_clusterLayer, _singlePointLayer, _photoPointLayer];

/// The name of a point's round photo on the map (MEM-E13-PLAT-05).
String photoImageOf(String memoryId) => 'photo-$memoryId';

/// The points as GeoJSON for a clustered source; clustering happens in the client (D-032). A point whose round photo
/// is on the map ([photos]) is drawn with it, the others as dots.
Map<String, Object> pointsToGeoJson(List<MapPoint> points, {Set<String> photos = const {}}) => {
  'type': 'FeatureCollection',
  'features': [
    for (final point in points)
      {
        'type': 'Feature',
        'id': point.id,
        // GL JS drops string feature ids; the source promotes this property back to the id.
        'properties': <String, Object>{
          'id': point.id,
          if (point.photoUrl != null && photos.contains(point.id)) 'photo': photoImageOf(point.id),
        },
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
  void Function()? _stopContextMenu;
  var _layersReady = false;

  /// The round photos already on the map, by memory id; a failed one stays a dot.
  final _photos = <String>{};
  final _photoDownloads = Dio();

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
      CircleLayerProperties(
        circleColor: '#00796b',
        circleRadius: _dotRadius(ref.read(settingsProvider)),
        circleStrokeColor: '#ffffff',
        circleStrokeWidth: 1.5,
      ),
      filter: [
        'all',
        [
          '!',
          ['has', 'point_count'],
        ],
        [
          '!',
          ['has', 'photo'],
        ],
      ],
    );
    await controller.addSymbolLayer(
      _pointsSource,
      _photoPointLayer,
      SymbolLayerProperties(
        iconImage: ['get', 'photo'],
        iconSize: _photoSize(ref.read(settingsProvider)),
        iconAllowOverlap: true,
      ),
      filter: [
        'all',
        [
          '!',
          ['has', 'point_count'],
        ],
        ['has', 'photo'],
      ],
    );
    _layersReady = true;
    controller.onFeatureTapped.add(_featureTapped);
    controller.onFeatureHover.add(_featureHovered);
    _stopContextMenu ??= listenToMapContextMenu((latitude, longitude, x, y) {
      if (!_held) {
        ref.read(mapMenuProvider.notifier).open(latitude, longitude, Offset(x, y));
      }
    });
    await _showPoints(ref.read(mapPointsProvider));
    widget.onPointsAdded?.call(controller);
  }

  /// MEM-E13-PLAT-09: a window is open over the map; the map neither moves nor answers the mouse.
  bool get _held => ref.read(mapHoldProvider) > 0;

  void _featureTapped(Point<double> point, LatLng coordinates, String id, String layerId, Annotation? annotation) {
    if (_held) {
      return;
    }
    if (layerId == _singlePointLayer || layerId == _photoPointLayer) {
      ref.read(tappedPointProvider.notifier).tap(id);
    } else if (layerId == _clusterLayer || layerId == _clusterCountLayer) {
      final controller = _controller!;
      controller.animateCamera(CameraUpdate.newLatLngZoom(coordinates, (controller.cameraPosition?.zoom ?? 1) + 2));
    }
  }

  /// Only single points have a memory id (a UUID); clusters have numbers.
  void _featureHovered(
    Point<double> point,
    LatLng coordinates,
    String id,
    Annotation? annotation,
    HoverEventType eventType,
  ) {
    if (!id.contains('-') || (_held && eventType != HoverEventType.leave)) {
      return;
    }
    final hovered = ref.read(hoveredPointProvider.notifier);
    if (eventType == HoverEventType.leave) {
      hovered.leave(id);
    } else {
      hovered.hover(id, Offset(point.x, point.y));
    }
  }

  /// MEM-E13-PLAT-05: a dot of 6 pixels at the smallest size, growing with the setting.
  static double _dotRadius(AppSettings settings) => 6 * settings.pointScale;

  /// The photo is drawn 96 pixels wide: 0.3 makes a circle of about 29 pixels, a little bigger than a dot.
  static double _photoSize(AppSettings settings) => 0.3 * settings.pointScale;

  /// Points first as they are; round photos follow as they arrive, each in its turn.
  Future<void> _showPoints(List<MapPoint> points) async {
    final controller = _controller;
    if (controller == null) {
      return;
    }
    await controller.setGeoJsonSource(_pointsSource, pointsToGeoJson(points, photos: _photos));
    var added = false;
    for (final point in points) {
      final url = point.photoUrl;
      if (url == null || _photos.contains(point.id) || !mounted) {
        continue;
      }
      try {
        final response = await _photoDownloads.get<List<int>>(url, options: Options(responseType: ResponseType.bytes));
        await controller.addImage(photoImageOf(point.id), await photoMarker(Uint8List.fromList(response.data!)));
        _photos.add(point.id);
        added = true;
      } on Object {
        // Not readable as a picture, or gone: the point stays a dot.
      }
    }
    if (added && mounted) {
      await controller.setGeoJsonSource(_pointsSource, pointsToGeoJson(ref.read(mapPointsProvider), photos: _photos));
    }
  }

  @override
  void dispose() {
    _controller?.onFeatureTapped.remove(_featureTapped);
    _controller?.onFeatureHover.remove(_featureHovered);
    _stopContextMenu?.call();
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
        _showPoints(points);
      }
    });
    ref.listen(settingsProvider, (previous, settings) {
      final controller = _controller;
      if (!_layersReady || controller == null || previous?.pointScale == settings.pointScale) {
        return;
      }
      controller
        ..setLayerProperties(_singlePointLayer, CircleLayerProperties(circleRadius: _dotRadius(settings)))
        ..setLayerProperties(_photoPointLayer, SymbolLayerProperties(iconSize: _photoSize(settings)));
    });
    return FutureBuilder<String>(
      future: _style,
      builder: (context, style) {
        final json = style.data;
        if (json == null) {
          return const SizedBox.expand();
        }
        final moves = ref.watch(mapHoldProvider) == 0;
        return MapLibreMap(
          // All five together: the web plugin applies gestures only when it gets every one of them.
          rotateGesturesEnabled: moves,
          scrollGesturesEnabled: moves,
          tiltGesturesEnabled: moves,
          zoomGesturesEnabled: moves,
          doubleClickZoomEnabled: moves,
          styleString: json,
          trackCameraPosition: true,
          // On a device the long press opens the menu; in a browser it comes with a double click, which zooms.
          onMapLongClick: kIsWeb || !moves
              ? null
              : (point, coordinates) => ref
                    .read(mapMenuProvider.notifier)
                    .open(coordinates.latitude, coordinates.longitude, Offset(point.x, point.y)),
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
