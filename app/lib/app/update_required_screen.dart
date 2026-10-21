import 'package:flutter/material.dart';

/// Shown instead of the app when the backend no longer serves this version (HTTP 426).
class UpdateRequiredScreen extends StatelessWidget {
  const UpdateRequiredScreen({super.key, required this.minimumVersion});

  final String minimumVersion;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Scaffold(
      body: SafeArea(
        child: Center(
          child: Padding(
            padding: const EdgeInsets.all(24),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text('Please update Memorio', style: theme.textTheme.headlineSmall, textAlign: TextAlign.center),
                const SizedBox(height: 12),
                Text(
                  minimumVersion.isEmpty
                      ? 'This version of the app is no longer supported.'
                      : 'This version of the app is no longer supported. Version $minimumVersion or newer is required.',
                  textAlign: TextAlign.center,
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
