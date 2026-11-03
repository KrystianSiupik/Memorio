import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
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
    final l10n = context.l10n;

    return Scaffold(
      appBar: AppBar(title: Text(_hasAccount ? l10n.signInTitle : l10n.registerTitle)),
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
                  decoration: InputDecoration(labelText: l10n.email),
                ),
                const SizedBox(height: 12),
                TextField(
                  controller: _password,
                  enabled: !_busy,
                  obscureText: true,
                  decoration: InputDecoration(labelText: l10n.password),
                ),
                const SizedBox(height: 12),
                if (!_hasAccount) ...[
                  CheckboxListTile(
                    title: Text(l10n.ageDeclaration),
                    value: _declaresMinimumAge,
                    onChanged: _busy ? null : (value) => setState(() => _declaresMinimumAge = value ?? false),
                  ),
                  CheckboxListTile(
                    title: Text(l10n.acceptTerms),
                    value: _acceptsTerms,
                    onChanged: _busy ? null : (value) => setState(() => _acceptsTerms = value ?? false),
                  ),
                  const SizedBox(height: 12),
                ],
                if (failure != null) ...[
                  Text(
                    _messageOf(l10n, failure),
                    key: const Key('s01-error'),
                    style: TextStyle(color: Theme.of(context).colorScheme.error),
                  ),
                  TextButton(onPressed: canSubmit ? _submit : null, child: Text(l10n.retry)),
                  const SizedBox(height: 12),
                ],
                FilledButton(
                  onPressed: canSubmit ? _submit : null,
                  child: Text(_hasAccount ? l10n.signIn : l10n.createAccount),
                ),
                TextButton(
                  onPressed: _busy
                      ? null
                      : () => setState(() {
                          _hasAccount = !_hasAccount;
                          _failure = null;
                        }),
                  child: Text(_hasAccount ? l10n.noAccount : l10n.hasAccount),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  static String _messageOf(AppLocalizations l10n, AuthFailure failure) => switch (failure) {
    AuthOffline() => l10n.authOffline,
    InvalidCredentials() => l10n.invalidCredentials,
    AgeDeclarationRequired() => l10n.ageRequired,
    EmailTaken() => l10n.emailTaken,
    InvalidPassword() => l10n.invalidPassword,
    InvalidEmail() => l10n.invalidEmail,
    InvalidProviderToken() => l10n.invalidProviderToken,
    ProviderUnavailable() => l10n.providerUnavailable,
    AuthServerError() => l10n.serverError,
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
                child: Text(context.l10n.noConnection, style: TextStyle(color: colors.onSecondaryContainer)),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
