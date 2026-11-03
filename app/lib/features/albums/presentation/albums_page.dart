import 'package:flutter/material.dart';

import '../../../core/l10n/l10n.dart';

/// S09 Albums: list and new album. Placeholder until its screen ticket.
class AlbumsPage extends StatelessWidget {
  const AlbumsPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(context.l10n.albumsTitle)),
      body: Center(child: Text(context.l10n.albumsTitle)),
    );
  }
}
