# MEM-E8-PLAT-04: Ekran S27 „Zgłoś treść (z aplikacji)”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E8](epiki/E8-zgloszenia-moderacja-i-nakazy-organow.md) Zgłoszenia, moderacja i nakazy organów |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E8-PLAT-04 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E8-MODERACJA-01](MEM-E8-MODERACJA-01-zglos-tresc-c110.md) |
| Blokuje | - |
| Kod roboczy | MEM-E8-PLAT-04 |
| Źródło | specyfikacja ekranu S27; testy przejść S27 |

## Cel

Ekran S27 „Zgłoś treść (z aplikacji)” — część epiku E8 „Zgłoszenia, moderacja i nakazy organów”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S27 „Zgłoś treść (z aplikacji)” (źródło: specyfikacja ekranu S27; testy przejść S27)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Kategoria. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Zgłoś”. *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — n/d: formularz; forbidden — n/d. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S27 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
