import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/offline/offline.dart';
import '../../../core/result.dart';
import '../domain/auth_repository.dart';
import '../domain/session.dart';
import 'session_providers.dart';

/// S01 Registration and sign-in, where signed-out users are sent. After a success the session changes
/// and the router leaves this screen.
class SignInPage extends ConsumerStatefulWidget {
  const SignInPage({super.key});

  @override
  ConsumerState<SignInPage> createState() => _SignInPageState();
}

class _SignInPageState extends ConsumerState<SignInPage> {
  final _email = TextEditingController();
  final _password = TextEditingController();
  var _hasAccount = false;
  var _declaresMinimumAge = false;
  var _acceptsTerms = false;
  var _busy = false;
  AuthFailure? _failure;

  @override
  void dispose() {
    _email.dispose();
    _password.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    setState(() {
      _busy = true;
      _failure = null;
    });
    final auth = ref.read(authRepositoryProvider);
    final Result<Session, AuthFailure> result = _hasAccount
        ? await auth.signIn(email: _email.text, password: _password.text)
        : await auth.register(email: _email.text, password: _password.text, declaresMinimumAge: _declaresMinimumAge);
    if (!mounted) {
      return;
    }
    switch (result) {
      case Ok():
        // A failure here costs nothing: the terms are asked for again before the first memory.
        if (!_hasAccount && _acceptsTerms) {
          await ref.read(accountRepositoryProvider).acceptCurrentTerms();
        }
        if (mounted) {
          ref.invalidate(sessionProvider);
        }
      case Err(:final failure):
        setState(() {
          _busy = false;
          _failure = failure;
        });
    }
  }

  @override
  Widget build(BuildContext context) {
    final offline = ref.watch(isOnlineProvider).value == false;
    final canSubmit = !_busy && !offline && (_hasAccount || _declaresMinimumAge);
    final failure = _failure;

    return Scaffold(
      appBar: AppBar(title: Text(_hasAccount ? 'Logowanie' : 'Rejestracja')),
      body: Column(
        children: [
          if (offline) const _NoConnectionBanner(),
          if (_busy) const LinearProgressIndicator(key: Key('s01-loading')),
          Expanded(
            child: ListView(
              padding: const EdgeInsets.all(16),
              children: [
                TextField(
                  controller: _email,
                  enabled: !_busy,
                  keyboardType: TextInputType.emailAddress,
                  autofillHints: const [AutofillHints.email],
                  decoration: const InputDecoration(labelText: 'E-mail'),
                ),
                const SizedBox(height: 12),
                TextField(
                  controller: _password,
                  enabled: !_busy,
                  obscureText: true,
                  decoration: const InputDecoration(labelText: 'Hasło'),
                ),
                const SizedBox(height: 12),
                if (!_hasAccount) ...[
                  CheckboxListTile(
                    title: const Text('Mam ukończone 16 lat'),
                    value: _declaresMinimumAge,
                    onChanged: _busy ? null : (value) => setState(() => _declaresMinimumAge = value ?? false),
                  ),
                  CheckboxListTile(
                    title: const Text('Akceptuję regulamin'),
                    value: _acceptsTerms,
                    onChanged: _busy ? null : (value) => setState(() => _acceptsTerms = value ?? false),
                  ),
                  const SizedBox(height: 12),
                ],
                if (failure != null) ...[
                  Text(
                    _messageOf(failure),
                    key: const Key('s01-error'),
                    style: TextStyle(color: Theme.of(context).colorScheme.error),
                  ),
                  TextButton(onPressed: canSubmit ? _submit : null, child: const Text('Ponów')),
                  const SizedBox(height: 12),
                ],
                FilledButton(
                  onPressed: canSubmit ? _submit : null,
                  child: Text(_hasAccount ? 'Zaloguj się' : 'Załóż konto'),
                ),
                TextButton(
                  onPressed: _busy
                      ? null
                      : () => setState(() {
                          _hasAccount = !_hasAccount;
                          _failure = null;
                        }),
                  child: Text(_hasAccount ? 'Nie mam konta' : 'Mam już konto'),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  static String _messageOf(AuthFailure failure) => switch (failure) {
    AuthOffline() => 'Brak połączenia. Sprawdź internet i spróbuj ponownie.',
    InvalidCredentials() => 'Nieprawidłowy e-mail lub hasło.',
    AgeDeclarationRequired() => 'Konto może założyć osoba, która ma ukończone 16 lat.',
    EmailTaken() => 'Konto z tym adresem już istnieje. Zaloguj się.',
    InvalidPassword() => 'Hasło musi mieć co najmniej 10 znaków.',
    InvalidEmail() => 'To nie wygląda na poprawny adres e-mail.',
    InvalidProviderToken() => 'Logowanie nie powiodło się. Spróbuj ponownie.',
    ProviderUnavailable() => 'Ten sposób logowania jest teraz niedostępny.',
    AuthServerError() => 'Błąd serwera. Spróbuj ponownie za chwilę.',
  };
}

class _NoConnectionBanner extends StatelessWidget {
  const _NoConnectionBanner();

  @override
  Widget build(BuildContext context) {
    final colors = Theme.of(context).colorScheme;
    return Material(
      color: colors.secondaryContainer,
      child: SafeArea(
        bottom: false,
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Row(
            children: [
              Icon(Icons.cloud_off, color: colors.onSecondaryContainer),
              const SizedBox(width: 12),
              Expanded(
                child: Text('Brak połączenia', style: TextStyle(color: colors.onSecondaryContainer)),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
