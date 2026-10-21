# MEM-E2-PLAT-04: Ekran S06 „Kosz”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E2-PLAT-04 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E2-ARCHIWUM-06](MEM-E2-ARCHIWUM-06-przywroc-tresc-z-kosza-c19.md); [MEM-E2-ARCHIWUM-10](MEM-E2-ARCHIWUM-10-zapytanie-kosz-moje-tresci-usuniete-w-ostatnich-30.md) |
| Blokuje | - |
| Kod roboczy | MEM-E2-PLAT-04 |
| Źródło | specyfikacja ekranu S06; testy przejść S06 |

## Cel

Ekran S06 „Kosz” — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S06 „Kosz” (źródło: specyfikacja ekranu S06; testy przejść S06)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Treść, Usunięto, Zostało dni. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Przywróć” (dostępna gdy: treść w koszu i okno 30 dni trwa). *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — „Kosz jest pusty”; forbidden — n/d: tylko własne treści. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S06 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*
- [ ] Odrzucenie wynikające z rywalizacji (np. „już rozstrzygnięte”, „pula pełna”) pokazane jako komunikat, bez utraty wpisanych danych. *Sprawdzenie: test komponentu z odpowiedzią 409*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
