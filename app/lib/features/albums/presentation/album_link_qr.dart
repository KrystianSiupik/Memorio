import 'package:flutter/material.dart';
import 'package:qr_flutter/qr_flutter.dart';

import '../../../core/l10n/l10n.dart';

/// MEM-E13-PLAT-12: the album link as a QR code, to scan from another phone. It carries the same link as the text, so
/// revoking the link makes the code dead too.
Future<void> showAlbumLinkQr(BuildContext context, {required String albumName, required String address}) {
  final l10n = context.l10n;
  return showDialog<void>(
    context: context,
    builder: (context) => AlertDialog(
      key: const Key('s11-qr-dialog'),
      title: Text(albumName),
      content: SizedBox(
        width: 280,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.circular(12)),
              child: QrImageView(
                key: const Key('s11-qr-code'),
                data: address,
                size: 240,
                backgroundColor: Colors.white,
                semanticsLabel: l10n.albumQrLabel,
              ),
            ),
            const SizedBox(height: 8),
            SelectableText(
              address,
              key: const Key('s11-qr-address'),
              textAlign: TextAlign.center,
              style: Theme.of(context).textTheme.labelSmall,
            ),
            const SizedBox(height: 12),
            Text(l10n.albumQrHint, textAlign: TextAlign.center, style: Theme.of(context).textTheme.bodySmall),
          ],
        ),
      ),
      actions: [TextButton(onPressed: () => Navigator.of(context).pop(), child: Text(l10n.close))],
    ),
  );
}
