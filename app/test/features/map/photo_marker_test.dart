import 'dart:convert';
import 'dart:ui' as ui;

import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/features/map/presentation/photo_marker.dart';

void main() {
  // A 1 by 1 pixel PNG.
  final photo = base64Decode(
    'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==',
  );

  testWidgets('a photo becomes a print taller than wide, white at its bottom edge', (tester) async {
    final drawn = await tester.runAsync(() async {
      final png = await photoMarker(photo);
      final image = (await (await ui.instantiateImageCodec(png)).getNextFrame()).image;
      final pixels = (await image.toByteData())!;
      // RGBA of a pixel in the wide bottom edge of the frame.
      final at = (95 * image.width + 48) * 4;
      return (image.width, image.height, [for (var i = 0; i < 4; i++) pixels.getUint8(at + i)]);
    });

    final (width, height, bottomEdge) = drawn!;
    expect(width, photoMarkerSize.width);
    expect(height, photoMarkerSize.height);
    expect(bottomEdge, [255, 255, 255, 255]);
  });
}
