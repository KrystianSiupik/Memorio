import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../core/l10n/l10n.dart';
import '../../../core/settings/settings.dart';

/// E13: what the person sets for this device. Every change applies at once, and is remembered.
class SettingsPage extends ConsumerWidget {
  const SettingsPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final l10n = context.l10n;
    final settings = ref.watch(settingsProvider);
    final notifier = ref.read(settingsProvider.notifier);
    return Scaffold(
      appBar: AppBar(title: Text(l10n.settingsTitle)),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          _Section(
            title: l10n.settingsTheme,
            child: Padding(
              padding: const EdgeInsets.fromLTRB(16, 8, 16, 0),
              child: SegmentedButton<AppTheme>(
                key: const Key('settings-theme'),
                segments: [
                  ButtonSegment(
                    value: AppTheme.system,
                    icon: const Icon(Icons.brightness_auto),
                    label: Text(l10n.settingsThemeSystem),
                  ),
                  ButtonSegment(
                    value: AppTheme.light,
                    icon: const Icon(Icons.light_mode_outlined),
                    label: Text(l10n.settingsThemeLight),
                  ),
                  ButtonSegment(
                    value: AppTheme.dark,
                    icon: const Icon(Icons.dark_mode_outlined),
                    label: Text(l10n.settingsThemeDark),
                  ),
                ],
                selected: {settings.theme},
                onSelectionChanged: (chosen) => notifier.update((current) => current.copyWith(theme: chosen.single)),
              ),
            ),
          ),
          _Section(
            title: l10n.settingsLanguage,
            child: RadioGroup<AppLanguage>(
              groupValue: settings.language,
              onChanged: (language) => notifier.update((current) => current.copyWith(language: language)),
              child: Column(
                children: [
                  for (final language in AppLanguage.values)
                    RadioListTile<AppLanguage>(
                      key: Key('settings-language-${language.name}'),
                      value: language,
                      title: Text(switch (language) {
                        AppLanguage.system => l10n.settingsLanguageSystem,
                        // Each language in its own name, so it is found whatever is on the screen now.
                        AppLanguage.polish => 'Polski',
                        AppLanguage.english => 'English',
                      }),
                    ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _Section extends StatelessWidget {
  const _Section({required this.title, required this.child});

  final String title;
  final Widget child;

  @override
  Widget build(BuildContext context) => Card(
    margin: const EdgeInsets.only(bottom: 16),
    child: Padding(
      padding: const EdgeInsets.symmetric(vertical: 12),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16),
            child: Text(title, style: Theme.of(context).textTheme.titleMedium),
          ),
          child,
        ],
      ),
    ),
  );
}
