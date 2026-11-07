import 'dart:typed_data';
import 'dart:ui' as ui;

import 'package:flutter/painting.dart';

/// MEM-E13-PLAT-05: a photo as the round picture of a point: cut to a circle, with a white ring and a soft shadow,
/// [size] pixels across as a PNG. The map shows it smaller (icon size), so it stays sharp on dense screens.
Future<Uint8List> photoMarker(Uint8List photo, {int size = 96, Color ring = const Color(0xFFFFFFFF)}) async {
  final codec = await ui.instantiateImageCodec(photo, targetWidth: size, targetHeight: size);
  final image = (await codec.getNextFrame()).image;
  final recorder = ui.PictureRecorder();
  final canvas = Canvas(recorder);
  final center = Offset(size / 2, size / 2);
  final radius = size / 2 - 4;

  canvas.drawCircle(
    center + const Offset(0, 1.5),
    radius,
    Paint()
      ..color = const Color(0x40000000)
      ..maskFilter = const MaskFilter.blur(BlurStyle.normal, 2),
  );
  canvas.drawCircle(center, radius, Paint()..color = ring);
  canvas
    ..save()
    ..clipPath(Path()..addOval(Rect.fromCircle(center: center, radius: radius - 4)));
  paintImage(
    canvas: canvas,
    rect: Rect.fromCircle(center: center, radius: radius - 4),
    image: image,
    fit: BoxFit.cover,
  );
  canvas.restore();

  final drawn = await recorder.endRecording().toImage(size, size);
  final bytes = await drawn.toByteData(format: ui.ImageByteFormat.png);
  image.dispose();
  drawn.dispose();
  return bytes!.buffer.asUint8List();
}
