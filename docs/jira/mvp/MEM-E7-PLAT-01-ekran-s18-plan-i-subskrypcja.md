# MEM-E7-PLAT-01: Ekran S18 „Plan i subskrypcja”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E7-PLAT-01 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E7-PLANY-03](MEM-E7-PLANY-03-odswiez-stan-subskrypcji-u-posrednika-c96.md); [MEM-E7-PLANY-12](MEM-E7-PLANY-12-zapytanie-stan-planu-tryb-terminy-zuzycie-wybor-do.md) |
| Blokuje | [MEM-E7-PLAT-02](MEM-E7-PLAT-02-ekran-s18-plan-i-subskrypcja-akcje-dolacz-do-planu.md) |
| Kod roboczy | MEM-E7-PLAT-01 |
| Źródło | specyfikacja ekranu S18; testy przejść S18 |

## Cel

Ekran S18 „Plan i subskrypcja” — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S18 „Plan i subskrypcja” (źródło: specyfikacja ekranu S18; testy przejść S18)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E7-PLAT-02)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Plan i tryb, Zużycie przestrzeni i AI, Terminy (tylko do odczytu, przycięcie). *Sprawdzenie: test komponentu*
- [ ] Akcje: „Przywróć zakupy / odśwież”; „Dołącz do planu rodzinnego” (dostępna gdy: mam zaproszenie; ekran mówi wprost, że opłacony czas własnego Premium przepada (decyzja 30)). *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — n/d: plan darmowy też ma stan; forbidden — n/d: własny plan. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S18 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*
- [ ] Odrzucenie wynikające z rywalizacji (np. „już rozstrzygnięte”, „pula pełna”) pokazane jako komunikat, bez utraty wpisanych danych. *Sprawdzenie: test komponentu z odpowiedzią 409*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
