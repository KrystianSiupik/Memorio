import 'dart:typed_data';
import 'dart:ui' as ui;

import 'package:flutter/painting.dart';

/// MEM-E13-PLAT-13: the width and height of [photoMarker] in pixels.
const photoMarkerSize = Size(96, 114);

/// MEM-E13-PLAT-05, MEM-E13-PLAT-13: a photo as a small instant print on the map: the picture cut square in a white
/// frame with a wider bottom edge, and a soft shadow, as a PNG of [photoMarkerSize]. The map shows it smaller (icon
/// size), so it stays sharp on dense screens.
Future<Uint8List> photoMarker(Uint8List photo) async {
  final codec = await ui.instantiateImageCodec(photo, targetWidth: 200);
  final image = (await codec.getNextFrame()).image;
  final recorder = ui.PictureRecorder();
  final canvas = Canvas(recorder);

  const card = Rect.fromLTRB(5, 3, 91, 108);
  const picture = Rect.fromLTRB(12, 10, 84, 82);
  canvas
    ..drawRRect(
      RRect.fromRectAndRadius(card.shift(const Offset(0, 2.5)), const Radius.circular(3)),
      Paint()
        ..color = const Color(0x59000000)
        ..maskFilter = const MaskFilter.blur(BlurStyle.normal, 3),
    )
    ..drawRRect(RRect.fromRectAndRadius(card, const Radius.circular(3)), Paint()..color = const Color(0xFFFFFFFF))
    ..save()
    ..clipRect(picture);
  paintImage(canvas: canvas, rect: picture, image: image, fit: BoxFit.cover);
  canvas
    ..restore()
    ..drawRect(
      picture,
      Paint()
        ..color = const Color(0x1F000000)
        ..style = PaintingStyle.stroke,
    );

  final drawn = await recorder.endRecording().toImage(photoMarkerSize.width.toInt(), photoMarkerSize.height.toInt());
  final bytes = await drawn.toByteData(format: ui.ImageByteFormat.png);
  image.dispose();
  drawn.dispose();
  return bytes!.buffer.asUint8List();
}

/// A slight tilt of each print, from -6 to 6 degrees, always the same for the same memory, as prints thrown on a table.
int photoTiltOf(String memoryId) {
  var hash = 0;
  for (final unit in memoryId.codeUnits) {
    hash = (hash * 31 + unit) & 0x7fffffff;
  }
  return hash % 13 - 6;
}
