# MEM-E8-PLAT-02: Ekran S22 „Moje zgłoszenia i decyzje”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E8](epiki/E8-zgloszenia-moderacja-i-nakazy-organow.md) Zgłoszenia, moderacja i nakazy organów |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E8-PLAT-02 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E8-MODERACJA-03](MEM-E8-MODERACJA-03-odwolaj-sie-od-decyzji-c112.md); [MEM-E8-MODERACJA-08](MEM-E8-MODERACJA-08-zapytanie-moje-zgloszenia-i-decyzje-moderacji-z-uz.md) |
| Blokuje | - |
| Kod roboczy | MEM-E8-PLAT-02 |
| Źródło | specyfikacja ekranu S22; testy przejść S22 |

## Cel

Ekran S22 „Moje zgłoszenia i decyzje” — część epiku E8 „Zgłoszenia, moderacja i nakazy organów”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S22 „Moje zgłoszenia i decyzje” (źródło: specyfikacja ekranu S22; testy przejść S22)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Decyzje, Uzasadnienie odwołania. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Odwołaj się” (dostępna gdy: decyzja dotyczy mnie i termin odwołania trwa). *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — „Nie masz zgłoszeń ani decyzji”; forbidden — n/d: każdy zalogowany ma dostęp do własnych danych. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S22 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
