# MEM-E3-PLAT-02: Ekran S10 „Album”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E3-PLAT-02 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 4–6 h (wyjątek: ekran złożony (6 akcji); dzielenie rozbiłoby jeden stan ekranu na dwa tickety bez osobnego rezultatu (D-072)) |
| Pewność | niska |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E3-ALBUM-05](MEM-E3-ALBUM-05-dodaj-wspomnienie-do-albumu-c47.md); [MEM-E3-ALBUM-13](MEM-E3-ALBUM-13-dodaj-zdjecia-do-wspomnienia-w-albumie-c56.md); [MEM-E3-ALBUM-09](MEM-E3-ALBUM-09-opusc-album-c52.md); [MEM-E4-DOSTEP-05](MEM-E4-DOSTEP-05-zablokuj-uzytkownika-c64.md); [MEM-E3-ALBUM-14](MEM-E3-ALBUM-14-oznacz-uczestnika-albumu-we-wspomnieniu-c169.md); [MEM-E9-ALBUM-02](MEM-E9-ALBUM-02-zarezerwuj-przejecie-albumu-bez-wlasciciela-c155.md); [MEM-E3-ALBUM-17](MEM-E3-ALBUM-17-zapytanie-album-wspomnienia-sklad-i-moje-wklady-r1.md); [MEM-E3-LIMITY-02](MEM-E3-LIMITY-02-read-port-stan-puli-albumu-r6.md); [MEM-E9-ALBUM-05](MEM-E9-ALBUM-05-zapytanie-album-bez-wlasciciela-stan-przejecia-czy.md) |
| Blokuje | - |
| Kod roboczy | MEM-E3-PLAT-02 |
| Źródło | specyfikacja ekranu S10; testy przejść S10 |

## Cel

Ekran S10 „Album” — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S10 „Album” (źródło: specyfikacja ekranu S10; testy przejść S10)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Wspomnienia, Skład (role), Moje wkłady (liczby), Zostało w puli, Album bez właściciela: stan przejęcia. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Dodaj moje wspomnienie” (dostępna gdy: jestem autorem wspomnienia; album nie jest tylko do odczytu); „Dodaj zdjęcia” (dostępna gdy: pula albumu nie jest pełna; album nie jest tylko do odczytu); „Opuść album (zostaw / zabierz)” (dostępna gdy: nie jestem właścicielem); „Zablokuj użytkownika”; „Oznacz uczestnika we wspomnieniu” (dostępna gdy: oznaczany jest uczestnikiem tego albumu (D-079)); „Przejmij album (30 min na opłacenie)” (dostępna gdy: album bez właściciela, trwa 24-mies. tryb tylko do odczytu, brak innej rezerwacji (D-004)). *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — album z pamięci podręcznej (media tylko zbuforowane), akcje wyłączone; empty — „Album jest pusty” + Dodaj wspomnienie; forbidden — „Nie masz już dostępu do tego albumu” (odebrany dostęp, blokada, QS4). *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S10 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*
- [ ] Odrzucenie wynikające z rywalizacji (np. „już rozstrzygnięte”, „pula pełna”) pokazane jako komunikat, bez utraty wpisanych danych. *Sprawdzenie: test komponentu z odpowiedzią 409*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
