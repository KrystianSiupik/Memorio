import 'package:flutter/material.dart';

import '../../../core/l10n/l10n.dart';

/// S16 Shared with me. Placeholder until its screen ticket.
class SharedWithMePage extends StatelessWidget {
  const SharedWithMePage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(context.l10n.sharedTitle)),
      body: Center(child: Text(context.l10n.sharedTitle)),
    );
  }
}
