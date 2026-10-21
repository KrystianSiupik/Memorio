# MEM-E9-PLAT-01: Ekran S23 „Przejęcie albumu (następca)”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E9-PLAT-01 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E9-SUKCESJA-04](MEM-E9-SUKCESJA-04-przekaz-album-nastepcy-c122.md); [MEM-E9-SUKCESJA-05](MEM-E9-SUKCESJA-05-odmow-przejecia-albumu-c123.md); [MEM-E9-SUKCESJA-08](MEM-E9-SUKCESJA-08-zapytanie-sprawa-przejecia-albumu-album-termin-los.md) |
| Blokuje | - |
| Kod roboczy | MEM-E9-PLAT-01 |
| Źródło | specyfikacja ekranu S23; testy przejść S23 |

## Cel

Ekran S23 „Przejęcie albumu (następca)” — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S23 „Przejęcie albumu (następca)” (źródło: specyfikacja ekranu S23; testy przejść S23)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Album, Termin, Losy treści fundatora. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Przejmuję album” (dostępna gdy: sprawa zweryfikowana; jestem następcą z migawki); „Odmawiam” (dostępna gdy: sprawa zweryfikowana). *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — n/d: ekran otwierany z powiadomienia o sprawie; forbidden — „Sprawa już rozstrzygnięta” albo „Nie jesteś wskazanym następcą”. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S23 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*
- [ ] Odrzucenie wynikające z rywalizacji (np. „już rozstrzygnięte”, „pula pełna”) pokazane jako komunikat, bez utraty wpisanych danych. *Sprawdzenie: test komponentu z odpowiedzią 409*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
