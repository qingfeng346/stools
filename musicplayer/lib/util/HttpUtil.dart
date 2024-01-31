import 'package:dio/dio.dart';

class HttpUtil {
  static Future<Response<T>> httpGet<T>(String url) {
    final dio = new Dio();
    return dio.get<T>(url);
  }
  static Future<Response<T>> httpPost<T>(String url, Object? data) {
    final dio = new Dio();
    return dio.post(url, data: data);
  }
}