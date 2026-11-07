# MEM-E13-PLAT-04: Mały kalendarz filtra dat

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E13](epiki/E13-wyglad-i-ustawienia.md) Wygląd i ustawienia |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, Web, MEM-E13-PLAT-04 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | P1 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | - |
| Blokuje | - |
| Kod roboczy | MEM-E13-PLAT-04 |
| Źródło | prośba PO z 2026-11-07: aplikacja ma być bardzo przyjazna |

## Cel

Filtr dat na mapie otwiera mały kalendarz zamiast dużego okna na cały ekran.

## Zakres

- Na szerokim ekranie kalendarz zakresu pojawia się mały, po lewej, przy przycisku filtra
- Na wąskim ekranie (telefon) mały kalendarz na środku
- Wybór dnia początku i końca, „Wyczyść”, „Zastosuj”; kliknięcie obok zamyka bez zmian

## Poza zakresem

- zapamiętywanie filtra między uruchomieniami

## Kryteria akceptacji

- [ ] Kalendarz otwiera się mały w opisanym miejscu, a zastosowany zakres filtruje punkty jak dotąd. *Sprawdzenie: test widżetu*
- [ ] Zamknięcie bez „Zastosuj” nie zmienia filtra. *Sprawdzenie: test widżetu*
