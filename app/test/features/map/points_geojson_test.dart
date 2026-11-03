import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/features/map/map.dart';

void main() {
  test('points become GeoJSON features with longitude first', () {
    final geoJson = pointsToGeoJson(const [MapPoint(id: 'a', latitude: 52.2, longitude: 21.0)]);

    expect(geoJson['type'], 'FeatureCollection');
    final feature = (geoJson['features']! as List<Object?>).single! as Map<String, Object?>;
    expect(feature['id'], 'a');
    expect(feature['properties'], {'id': 'a'});
    expect((feature['geometry']! as Map<String, Object?>)['coordinates'], [21.0, 52.2]);
  });

  test('no points is an empty collection, not an error', () {
    expect(pointsToGeoJson(const [])['features'], isEmpty);
  });
}
