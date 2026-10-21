# MEM-E2-PLAT-03: Ekran S05 „Szczegóły wspomnienia”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E2-PLAT-03 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E2-ARCHIWUM-04](MEM-E2-ARCHIWUM-04-usun-swoja-tresc-c17.md); [MEM-E2-ARCHIWUM-07](MEM-E2-ARCHIWUM-07-zloz-zadanie-usuniecia-danych-c20.md); [MEM-E3-ALBUM-15](MEM-E3-ALBUM-15-usun-swoje-oznaczenie-c170.md); [MEM-E2-ARCHIWUM-09](MEM-E2-ARCHIWUM-09-zapytanie-szczegoly-wspomnienia-miejsce-data-z-pre.md) |
| Blokuje | - |
| Kod roboczy | MEM-E2-PLAT-03 |
| Źródło | specyfikacja ekranu S05; testy przejść S05 |

## Cel

Ekran S05 „Szczegóły wspomnienia” — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S05 „Szczegóły wspomnienia” (źródło: specyfikacja ekranu S05; testy przejść S05)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Miejsce, Data, Treści (autor przy każdej), Osoby, Znacznik AI przy tekstach AI. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Usuń moją treść” (dostępna gdy: jestem autorem treści); „Usuń trwale (formalne żądanie)” (dostępna gdy: jestem autorem treści; potwierdzenie); „Usuń moje oznaczenie” (dostępna gdy: jestem oznaczony na tym wspomnieniu (D-079)). *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — n/d: wspomnienie istnieje, dopóki ma treść (S2:B3); forbidden — „Nie masz już dostępu do tego wspomnienia” → powrót (dostęp odebrany, QS4). *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S05 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
