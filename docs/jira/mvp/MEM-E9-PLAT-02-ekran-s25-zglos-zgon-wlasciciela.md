# MEM-E9-PLAT-02: Ekran S25 „Zgłoś zgon właściciela”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E9-PLAT-02 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E9-SUKCESJA-02](MEM-E9-SUKCESJA-02-zglos-zgon-wlasciciela-c120.md) |
| Blokuje | - |
| Kod roboczy | MEM-E9-PLAT-02 |
| Źródło | specyfikacja ekranu S25; testy przejść S25 |

## Cel

Ekran S25 „Zgłoś zgon właściciela” — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S25 „Zgłoś zgon właściciela” (źródło: specyfikacja ekranu S25; testy przejść S25)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Album, Akt zgonu (plik). *Sprawdzenie: test komponentu*
- [ ] Akcje: „Wyślij zgłoszenie”. *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — n/d: formularz; forbidden — tylko uczestnik albumu. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S25 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
