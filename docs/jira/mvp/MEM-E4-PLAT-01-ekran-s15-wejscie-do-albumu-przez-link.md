# MEM-E4-PLAT-01: Ekran S15 „Wejście do albumu przez link”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E4](epiki/E4-udostepnianie-linkiem-i-odbieranie-dostepu.md) Udostępnianie linkiem i odbieranie dostępu |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E4-PLAT-01 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E4-DOSTEP-02](MEM-E4-DOSTEP-02-wejdz-do-albumu-przez-link-c61.md) |
| Blokuje | - |
| Kod roboczy | MEM-E4-PLAT-01 |
| Źródło | specyfikacja ekranu S15; testy przejść S15 |

## Cel

Ekran S15 „Wejście do albumu przez link” — część epiku E4 „Udostępnianie linkiem i odbieranie dostępu”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S15 „Wejście do albumu przez link” (źródło: specyfikacja ekranu S15; testy przejść S15)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Album. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Otwórz album” (dostępna gdy: zalogowany). *Sprawdzenie: test komponentu*
- [ ] Stany: loading — sprawdzanie linku; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner offline, akcja wyłączona; empty — n/d: formularz; forbidden — niezalogowany → S01 i powrót; zablokowany → „Nie masz dostępu do tego albumu”. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S15 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*
- [ ] Odrzucenie wynikające z rywalizacji (np. „już rozstrzygnięte”, „pula pełna”) pokazane jako komunikat, bez utraty wpisanych danych. *Sprawdzenie: test komponentu z odpowiedzią 409*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
