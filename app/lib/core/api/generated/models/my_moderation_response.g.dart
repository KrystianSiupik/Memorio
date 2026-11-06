// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'my_moderation_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MyModerationResponse _$MyModerationResponseFromJson(Map<String, dynamic> json) => MyModerationResponse(
  reports: (json['reports'] as List<dynamic>).map((e) => MyReport.fromJson(e as Map<String, dynamic>)).toList(),
  aboutMine: (json['aboutMine'] as List<dynamic>)
      .map((e) => ReportAboutMine.fromJson(e as Map<String, dynamic>))
      .toList(),
);

Map<String, dynamic> _$MyModerationResponseToJson(MyModerationResponse instance) => <String, dynamic>{
  'reports': instance.reports,
  'aboutMine': instance.aboutMine,
};
