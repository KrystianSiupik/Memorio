import 'package:dio/dio.dart';

/// Sends the app version with every request and reports HTTP 426 ("update required", ADR 0017).
class AppVersionInterceptor extends Interceptor {
  AppVersionInterceptor({required this.appVersion, required this.onUpdateRequired});

  static const header = 'X-App-Version';

  final String appVersion;
  final void Function(String minimumVersion) onUpdateRequired;

  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    options.headers[header] = appVersion;
    handler.next(options);
  }

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    final response = err.response;
    if (response?.statusCode == 426) {
      final data = response?.data;
      final minimumVersion = data is Map<String, dynamic> ? data['minimumVersion'] : null;
      onUpdateRequired(minimumVersion is String ? minimumVersion : '');
    }
    handler.next(err);
  }
}
