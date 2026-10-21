# MEM-E7-PLAT-03: Ekran S19 „Wybierz, co zostanie po przycięciu”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E7-PLAT-03 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E7-PLANY-05](MEM-E7-PLANY-05-wskaz-material-do-zachowania-c94.md); [MEM-E7-PLANY-12](MEM-E7-PLANY-12-zapytanie-stan-planu-tryb-terminy-zuzycie-wybor-do.md) |
| Blokuje | - |
| Kod roboczy | MEM-E7-PLAT-03 |
| Źródło | specyfikacja ekranu S19; testy przejść S19 |

## Cel

Ekran S19 „Wybierz, co zostanie po przycięciu” — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S19 „Wybierz, co zostanie po przycięciu” (źródło: specyfikacja ekranu S19; testy przejść S19)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Limit darmowy, Materiał (najstarsze zaznaczone domyślnie), Termin. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Zachowaj wybrane” (dostępna gdy: okno wyboru trwa). *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — n/d: ekran dostępny tylko w oknie wyboru; forbidden — tylko właściciel planu/albumu. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S19 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*
- [ ] Odrzucenie wynikające z rywalizacji (np. „już rozstrzygnięte”, „pula pełna”) pokazane jako komunikat, bez utraty wpisanych danych. *Sprawdzenie: test komponentu z odpowiedzią 409*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
