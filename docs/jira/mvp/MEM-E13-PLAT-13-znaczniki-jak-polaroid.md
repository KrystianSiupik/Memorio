# MEM-E13-PLAT-13: Znaczniki ze zdjęciem jako odbitki z polaroida

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E13](epiki/E13-wyglad-i-ustawienia.md) Wygląd i ustawienia |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, Web, MEM-E13-PLAT-13 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | P1 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | zrobiony |
| Zależy od | MEM-E13-PLAT-05 |
| Blokuje | - |
| Kod roboczy | MEM-E13-PLAT-13 |
| Źródło | prośba PO z 2026-11-08 |

## Cel

Wspomnienie ze zdjęciem wygląda na mapie jak prawdziwa odbitka: zdjęcie w białej ramce z szerszym dołem, lekko przechylone.

## Zakres

- Znacznik: zdjęcie przycięte do kwadratu, biała ramka szersza u dołu, cień; ok. 35×41 px przy 100%
- Odbitka stoi na swoim punkcie (dolna krawędź) i jest przechylona o -6..6°, zawsze tak samo dla tego samego wspomnienia
- Podgląd w Ustawieniach i opis przełącznika pokazują odbitkę zamiast okręgu

## Kryteria akceptacji

- [ ] Znacznik ma kształt odbitki (wyższy niż szerszy, biały dół). *Sprawdzenie: test rysowania*
- [ ] Przechył mieści się w -6..6° i jest stały dla wspomnienia. *Sprawdzenie: test jednostkowy*
