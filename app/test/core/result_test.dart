import 'package:flutter_test/flutter_test.dart';
import 'package:memorio/core/result.dart';

void main() {
  String describe(Result<int, String> result) => result.when(ok: (v) => 'ok $v', err: (f) => 'err $f');

  test('when runs the branch of the outcome', () {
    expect(describe(const Ok(42)), 'ok 42');
    expect(describe(const Err('offline')), 'err offline');
  });
}
