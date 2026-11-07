# MEM-E13-PLAT-09: Dodawanie wspomnienia w czterech krokach, mapa nieruchoma pod oknem

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E13](epiki/E13-wyglad-i-ustawienia.md) Wygląd i ustawienia |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, Web, MEM-E13-PLAT-09 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | P1 |
| Estymacja | 3–5 h |
| Pewność | średnia |
| Status | zrobiony |
| Zależy od | MEM-E13-PLAT-08 |
| Blokuje | - |
| Kod roboczy | MEM-E13-PLAT-09 |
| Źródło | prośba PO z 2026-11-07 po obejrzeniu okna |

## Cel

Dodawanie wspomnienia jest przejrzyste: jedna rzecz na krok. Mapa pod otwartym oknem się nie przesuwa.

## Zakres

- Cztery kroki: Miejsce, Data, Zdjęcia, Historia; każdy z dużym pytaniem („Gdzie to było?”, „Kiedy to było?”, „Pokaż, jak tam było”, „Opowiedz, co się wydarzyło”) i jednym zdaniem wyjaśnienia
- Większe odstępy między polami; bez ramek w ramkach
- Zdjęcia można pominąć („Dalej” zawsze aktywne); „Wstecz” cofa o jeden krok
- Gdy okno jest otwarte, mapa pod nim nie reaguje na kółko, przeciąganie, podwójne kliknięcie, najechanie ani prawy przycisk; po zamknięciu wraca do normy

## Poza zakresem

- zapamiętywanie wpisanej treści po zamknięciu okna

## Kryteria akceptacji

- [ ] Każdy krok pokazuje tylko swoje pola; „Dalej” jest aktywne dopiero, gdy krok jest wypełniony (zdjęcia nieobowiązkowe). *Sprawdzenie: test widżetu*
- [ ] Przy otwartym oknie mapa jest wstrzymana, po zamknięciu nie. *Sprawdzenie: test widżetu (stan) + przeglądarka*
- [ ] Dotychczasowe testy S04 przechodzą po dopasowaniu do kroków. *Sprawdzenie: flutter test*
