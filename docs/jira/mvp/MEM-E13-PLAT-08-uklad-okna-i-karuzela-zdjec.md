# MEM-E13-PLAT-08: Układ okna nowego wspomnienia i karuzela zdjęć

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E13](epiki/E13-wyglad-i-ustawienia.md) Wygląd i ustawienia |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, Web, MEM-E13-PLAT-08 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | P1 |
| Estymacja | 4–6 h |
| Pewność | średnia |
| Status | zrobiony |
| Zależy od | MEM-E13-PLAT-07 |
| Blokuje | - |
| Kod roboczy | MEM-E13-PLAT-08 |
| Źródło | prośba PO z 2026-11-07 po obejrzeniu okna |

## Cel

Okno nowego wspomnienia jest czytelne: pogrupowane, z polami obok siebie tam, gdzie się mieszczą, z wyraźnymi przyciskami. Wybrane zdjęcia widać i można je przewijać oraz usuwać przed zapisem.

## Zakres

- Sekcje w ramkach z nagłówkiem i ikoną: Miejsce, Data, Zdjęcia, Opis
- Pola z obramówką; szerokość i długość geograficzna obok siebie; przyciski daty i godziny w jednym wierszu, zawijane na wąskim ekranie
- Przyciski drugorzędne z obramówką i ikoną, główny wypełniony
- Wybrane miejsce jako wyraźna karta z przyciskiem „Zmień”
- Karuzela zdjęć: duży podgląd, strzałki, licznik „2 / 5”, miniatury pod spodem (kliknięcie przewija), usunięcie bieżącego zdjęcia; puste pole zachęca do dodania

## Poza zakresem

- obracanie i kadrowanie zdjęć; zmiana kolejności

## Kryteria akceptacji

- [ ] Wybrane zdjęcia widać w karuzeli; strzałki i miniatury przewijają. *Sprawdzenie: test widżetu*
- [ ] Usunięte zdjęcie znika z karuzeli i nie jest wysyłane przy zapisie. *Sprawdzenie: test widżetu*
- [ ] Szerokość i długość leżą w jednym wierszu na szerokim ekranie; na wąskim nic nie wychodzi poza ekran. *Sprawdzenie: test widżetu*
- [ ] Dotychczasowe testy S04 przechodzą. *Sprawdzenie: flutter test*
