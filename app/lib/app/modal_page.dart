import 'package:flutter/material.dart';

/// MEM-E13-PLAT-07: a route shown as a window over the screen below it, with its own address. A click on the
/// darkened background or Escape does not close it; the window closes itself (its X or Cancel).
class ModalPage<T> extends Page<T> {
  const ModalPage({required this.child, super.key, super.name});

  final Widget child;

  @override
  Route<T> createRoute(BuildContext context) => _ModalPageRoute<T>(this);
}

class _ModalPageRoute<T> extends PopupRoute<T> {
  _ModalPageRoute(ModalPage<T> page) : super(settings: page);

  ModalPage<T> get _page => settings as ModalPage<T>;

  // Not dismissible: neither the background nor Escape (DismissIntent follows this flag) closes the window.
  @override
  bool get barrierDismissible => false;

  @override
  Color get barrierColor => Colors.black54;

  @override
  String? get barrierLabel => null;

  @override
  Duration get transitionDuration => const Duration(milliseconds: 180);

  @override
  Widget buildPage(BuildContext context, Animation<double> animation, Animation<double> secondaryAnimation) =>
      PopScope(canPop: false, child: _page.child);

  @override
  Widget buildTransitions(
    BuildContext context,
    Animation<double> animation,
    Animation<double> secondaryAnimation,
    Widget child,
  ) {
    final curved = CurvedAnimation(parent: animation, curve: Curves.easeOut);
    return FadeTransition(
      opacity: curved,
      child: ScaleTransition(scale: Tween<double>(begin: 0.96, end: 1).animate(curved), child: child),
    );
  }
}
