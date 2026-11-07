# MEM-E13-PLAT-01: Język polski i angielski, ekran Ustawienia

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E13](epiki/E13-wyglad-i-ustawienia.md) Wygląd i ustawienia |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, Web, MEM-E13-PLAT-01 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | P1 |
| Estymacja | 4–6 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | - |
| Blokuje | - |
| Kod roboczy | MEM-E13-PLAT-01 |
| Źródło | prośba PO z 2026-11-07: aplikacja ma być bardzo przyjazna |

## Cel

Aplikacja działa po polsku i po angielsku; osoba wybiera język w nowym ekranie Ustawienia.

## Zakres

- Ekran Ustawienia (z zakładki Konto); ustawienia zapisane na urządzeniu
- Język: jak w systemie (domyślnie), polski albo angielski; wszystkie teksty aplikacji po angielsku
- Daty w formacie wybranego języka tam, gdzie pokazuje je system (kalendarz)

## Poza zakresem

- wiadomości e-mail i strony serwera (zostają po polsku)

## Kryteria akceptacji

- [ ] Wybór języka od razu zmienia teksty aplikacji i zostaje po ponownym uruchomieniu. *Sprawdzenie: test widżetu*
- [ ] Każdy tekst ma tłumaczenie angielskie (brak luk w plikach tłumaczeń). *Sprawdzenie: test widżetu*
