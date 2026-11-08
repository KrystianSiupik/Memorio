# MEM-E13-PLAT-12: Kod QR z linkiem do albumu

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E13](epiki/E13-wyglad-i-ustawienia.md) Wygląd i ustawienia |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, Web, MEM-E13-PLAT-12 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | P1 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | zrobiony |
| Zależy od | MEM-E4-DOSTEP-01 |
| Blokuje | - |
| Kod roboczy | MEM-E13-PLAT-12 |
| Źródło | prośba PO z 2026-11-08 |

## Cel

Link do albumu można pokazać jako kod QR, żeby ktoś obok zeskanował go telefonem.

## Zakres

- Na S11 przy linku przycisk „Pokaż kod QR”; okno z kodem, adresem pod spodem i wyjaśnieniem
- Kod niesie ten sam link: unieważnienie linku wyłącza też kod
- Pakiet `qr_flutter` (BSD-3), kod rysowany w aplikacji, bez usług zewnętrznych

## Kryteria akceptacji

- [ ] Przy aktywnym linku przycisk otwiera okno z kodem i tym samym adresem; „Zamknij” je zamyka. *Sprawdzenie: test widżetu*
