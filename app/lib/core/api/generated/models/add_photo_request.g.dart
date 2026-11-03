// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'add_photo_request.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AddPhotoRequest _$AddPhotoRequestFromJson(Map<String, dynamic> json) =>
    AddPhotoRequest(photoId: json['photoId'] as String, uploadKey: json['uploadKey'] as String?);

Map<String, dynamic> _$AddPhotoRequestToJson(AddPhotoRequest instance) => <String, dynamic>{
  'photoId': instance.photoId,
  'uploadKey': instance.uploadKey,
};
