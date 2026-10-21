# MEM-E4-PLAT-02: Ekran S16 „Udostępnione mi”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E4](epiki/E4-udostepnianie-linkiem-i-odbieranie-dostepu.md) Udostępnianie linkiem i odbieranie dostępu |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E4-PLAT-02 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E3-DOSTEP-01](MEM-E3-DOSTEP-01-zapytanie-udostepnione-mi-albumy-z-odczytem-r14.md) |
| Blokuje | - |
| Kod roboczy | MEM-E4-PLAT-02 |
| Źródło | specyfikacja ekranu S16; testy przejść S16 |

## Cel

Ekran S16 „Udostępnione mi” — część epiku E4 „Udostępnianie linkiem i odbieranie dostępu”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S16 „Udostępnione mi” (źródło: specyfikacja ekranu S16; testy przejść S16)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Albumy. *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — „Nikt jeszcze nie udostępnił Ci albumu”; forbidden — n/d: lista własna. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S16 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
