/// MEM-E12-PLAT-02: the right click on the map in a browser; nothing elsewhere, where a long press does it.
typedef MapContextMenuHandler = void Function(double latitude, double longitude, double x, double y);

/// Starts listening; the returned function stops it.
void Function() listenToMapContextMenu(MapContextMenuHandler handler) => () {};
