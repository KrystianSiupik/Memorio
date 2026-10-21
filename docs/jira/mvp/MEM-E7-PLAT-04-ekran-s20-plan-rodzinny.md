# MEM-E7-PLAT-04: Ekran S20 „Plan rodzinny”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E7-PLAT-04 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P2 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E7-PLANY-07](MEM-E7-PLANY-07-zapros-do-planu-rodzinnego-c97.md); [MEM-E7-PLANY-09](MEM-E7-PLANY-09-usun-osobe-z-planu-rodzinnego-c99.md) |
| Blokuje | - |
| Kod roboczy | MEM-E7-PLAT-04 |
| Źródło | specyfikacja ekranu S20; testy przejść S20 |

## Cel

Ekran S20 „Plan rodzinny” — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S20 „Plan rodzinny” (źródło: specyfikacja ekranu S20; testy przejść S20)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Członkowie, E-mail. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Zaproś do planu” (dostępna gdy: wolne miejsce); „Usuń z planu (osoba przejdzie w tryb „ponad limitem”)”. *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — „Nikogo jeszcze nie zaprosiłeś”; forbidden — tylko płacący za plan rodzinny. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S20 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*
- [ ] Odrzucenie wynikające z rywalizacji (np. „już rozstrzygnięte”, „pula pełna”) pokazane jako komunikat, bez utraty wpisanych danych. *Sprawdzenie: test komponentu z odpowiedzią 409*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
