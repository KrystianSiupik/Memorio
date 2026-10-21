# MEM-E7-PLAT-02: Ekran S18 „Plan i subskrypcja”: akcje „Dołącz do planu rodzinnego”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E7-PLAT-02 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P2 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | zablokowany: HS-012: plan rodzinny w sklepie czy własny? (PO) |
| Zależy od | [MEM-E7-PLAT-01](MEM-E7-PLAT-01-ekran-s18-plan-i-subskrypcja.md); [MEM-E7-PLANY-08](MEM-E7-PLANY-08-dolacz-do-planu-rodzinnego-c98.md) |
| Blokuje | - |
| Kod roboczy | MEM-E7-PLAT-02 |
| Źródło | specyfikacja ekranu S18 |

## Cel

Ekran S18 „Plan i subskrypcja”: akcje „Dołącz do planu rodzinnego” — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S18 „Plan i subskrypcja”: akcje „Dołącz do planu rodzinnego” (źródło: specyfikacja ekranu S18)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: brak. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Dołącz do planu rodzinnego” (dostępna gdy: mam zaproszenie; ekran mówi wprost, że opłacony czas własnego Premium przepada (decyzja 30)). *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — n/d: plan darmowy też ma stan; forbidden — n/d: własny plan. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S18 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*
- [ ] Odrzucenie wynikające z rywalizacji (np. „już rozstrzygnięte”, „pula pełna”) pokazane jako komunikat, bez utraty wpisanych danych. *Sprawdzenie: test komponentu z odpowiedzią 409*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
