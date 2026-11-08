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

  test('a point is drawn with its round photo only once the photo is on the map, and only while it has one', () {
    const points = [
      MapPoint(id: 'a', latitude: 52.2, longitude: 21.0, photoUrl: 'https://storage.example.invalid/a'),
      MapPoint(id: 'b', latitude: 52.3, longitude: 21.1, photoUrl: 'https://storage.example.invalid/b'),
      MapPoint(id: 'c', latitude: 52.4, longitude: 21.2),
    ];

    final features = (pointsToGeoJson(points, photos: {'a', 'c'})['features']! as List<Object?>)
        .cast<Map<String, Object?>>();

    expect(features[0]['properties'], {'id': 'a', 'photo': photoImageOf('a'), 'tilt': photoTiltOf('a')});
    expect(features[1]['properties'], {'id': 'b'});
    expect(features[2]['properties'], {'id': 'c'});
  });

  test('no points is an empty collection, not an error', () {
    expect(pointsToGeoJson(const [])['features'], isEmpty);
  });

  test('each print leans a little, always the same way for the same memory', () {
    final tilts = {for (var i = 0; i < 200; i++) photoTiltOf('memory-$i')};

    expect(tilts.every((tilt) => tilt >= -6 && tilt <= 6), isTrue);
    expect(tilts.length, greaterThan(5));
    expect(photoTiltOf('memory-7'), photoTiltOf('memory-7'));
  });
}
