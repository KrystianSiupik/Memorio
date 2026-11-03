import 'package:flutter/widgets.dart';

import 'generated/app_localizations.dart';

export 'generated/app_localizations.dart';

/// The app's texts: `context.l10n.retry`. Polish for now; another language is another .arb file (D-233).
extension L10n on BuildContext {
  AppLocalizations get l10n => AppLocalizations.of(this);
}
