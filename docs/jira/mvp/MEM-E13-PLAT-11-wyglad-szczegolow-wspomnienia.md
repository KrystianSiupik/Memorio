# MEM-E13-PLAT-11: Nowy wygląd szczegółów wspomnienia (S05)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E13](epiki/E13-wyglad-i-ustawienia.md) Wygląd i ustawienia |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, Web, MEM-E13-PLAT-11 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | P1 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | zrobiony |
| Zależy od | MEM-E2-PLAT-03 |
| Blokuje | - |
| Kod roboczy | MEM-E13-PLAT-11 |
| Źródło | prośba PO z 2026-11-08 |

## Cel

Szczegóły wspomnienia wyglądają jak pamiątka, nie jak formularz: zdjęcia na górze, historia czytelna, usuwanie odsunięte.

## Zakres

- Na górze duża galeria zdjęć (strzałki, licznik), na niej biała nazwa miejsca i data na przyciemnieniu; bez zdjęć kolorowy pasek z nazwą
- Kliknięcie zdjęcia otwiera je na pełnym ekranie z przybliżaniem
- Pod spodem znaczniki (autor, liczba zdjęć, AI), karta „Historia” z opisem większą czcionką, karta „Osoby”
- Usuwanie w osobnej czerwonej sekcji na dole; „Zgłoś” jako przycisk z ikoną
- Treść wyśrodkowana, maks. 760 px

## Kryteria akceptacji

- [ ] Miejsce i data są białe na zdjęciu; strzałki przewijają, licznik się zmienia; zdjęcie otwiera się na pełnym ekranie i zamyka. *Sprawdzenie: test widżetu*
- [ ] Bez zdjęć jest kolorowy pasek; usuwanie leży pod treścią w osobnej sekcji. *Sprawdzenie: test widżetu*
- [ ] Dotychczasowe testy S05 przechodzą. *Sprawdzenie: flutter test*
