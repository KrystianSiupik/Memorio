# MEM-E3-PLAT-03: Ekran S11 „Zarządzanie albumem (właściciel)”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E3-PLAT-03 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 4–6 h (wyjątek: ekran złożony (6 akcji); dzielenie rozbiłoby jeden stan ekranu na dwa tickety bez osobnego rezultatu (D-072)) |
| Pewność | niska |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E3-SUKCESJA-01](MEM-E3-SUKCESJA-01-wskaz-nastepce-albumu-c41.md); [MEM-E3-ALBUM-02](MEM-E3-ALBUM-02-zapros-osobe-do-albumu-c42.md); [MEM-E3-ALBUM-10](MEM-E3-ALBUM-10-usun-uczestnika-z-albumu-c53.md); [MEM-E4-DOSTEP-01](MEM-E4-DOSTEP-01-udostepnij-album-linkiem-c60.md); [MEM-E4-DOSTEP-03](MEM-E4-DOSTEP-03-uniewaznij-link-c62.md); [MEM-E4-DOSTEP-04](MEM-E4-DOSTEP-04-odbierz-dostep-osobie-c63.md); [MEM-E3-ALBUM-16](MEM-E3-ALBUM-16-zapytanie-miejsca-w-planie-zajete-zaproszone-wolne.md); [MEM-E4-DOSTEP-07](MEM-E4-DOSTEP-07-zapytanie-imienna-lista-ogladajacych-albumu-r10.md) |
| Blokuje | - |
| Kod roboczy | MEM-E3-PLAT-03 |
| Źródło | specyfikacja ekranu S11; testy przejść S11 |

## Cel

Ekran S11 „Zarządzanie albumem (właściciel)” — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S11 „Zarządzanie albumem (właściciel)” (źródło: specyfikacja ekranu S11; testy przejść S11)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Miejsca: zajęte / zaproszone / wolne, Oglądający przez link (imiennie), Następca, E-mail zapraszanego. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Zaproś” (dostępna gdy: wolne miejsce w planie); „Usuń uczestnika”; „Wskaż następcę” (dostępna gdy: kandydat jest uczestnikiem); „Udostępnij linkiem”; „Unieważnij link (nikt nowy nie wejdzie)”; „Odbierz dostęp”. *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — n/d: album ma co najmniej właściciela; forbidden — dostępne tylko dla właściciela; inni nie widzą wejścia. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S11 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*
- [ ] Odrzucenie wynikające z rywalizacji (np. „już rozstrzygnięte”, „pula pełna”) pokazane jako komunikat, bez utraty wpisanych danych. *Sprawdzenie: test komponentu z odpowiedzią 409*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
