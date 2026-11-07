import 'package:flutter/material.dart';

/// MEM-E13-PLAT-02: the two looks of the app. Blue is the accent of the light one, gold of the dark one; screens take
/// their colours from the scheme only, so both work everywhere.
abstract final class AppThemes {
  static const lightAccent = Color(0xFF1F6FEB);
  static const darkAccent = Color(0xFFE0B23C);

  static final light = ThemeData(
    useMaterial3: true,
    colorScheme: ColorScheme.fromSeed(seedColor: lightAccent, primary: lightAccent),
  );

  static final dark = ThemeData(
    useMaterial3: true,
    colorScheme: ColorScheme.fromSeed(
      seedColor: darkAccent,
      brightness: Brightness.dark,
      primary: darkAccent,
      surface: const Color(0xFF1C1D21),
    ),
  );
}
