import 'package:image_picker/image_picker.dart';

import '../domain/memories_repository.dart';
import '../domain/memory.dart';

/// [PhotoPicker] on the system's gallery.
class ImagePhotoPicker implements PhotoPicker {
  const ImagePhotoPicker();

  @override
  Future<List<PickedPhoto>> pick() async {
    final files = await ImagePicker().pickMultiImage();
    return [
      for (final file in files)
        PickedPhoto(name: file.name, contentType: contentTypeOf(file.name), read: file.readAsBytes, path: file.path),
    ];
  }
}

/// The type the backend accepts, by the name of the file; JPEG when in doubt, as most photos are.
String contentTypeOf(String name) {
  final lower = name.toLowerCase();
  if (lower.endsWith('.png')) {
    return 'image/png';
  }
  if (lower.endsWith('.webp')) {
    return 'image/webp';
  }
  return lower.endsWith('.heic') ? 'image/heic' : 'image/jpeg';
}

/// A photo waiting in the offline queue, read again from its path when it is sent.
PickedPhoto photoAt(String path, String name, String contentType) =>
    PickedPhoto(name: name, contentType: contentType, read: XFile(path).readAsBytes, path: path);
