/// Outcome of an operation that can fail in an expected way: a value, or a failure of type [F].
/// Features define their failures as sealed classes, so a `switch` over them is exhaustive.
sealed class Result<T, F> {
  const Result();

  R when<R>({required R Function(T value) ok, required R Function(F failure) err}) => switch (this) {
    Ok(:final value) => ok(value),
    Err(:final failure) => err(failure),
  };
}

final class Ok<T, F> extends Result<T, F> {
  const Ok(this.value);

  final T value;
}

final class Err<T, F> extends Result<T, F> {
  const Err(this.failure);

  final F failure;
}
