# MEM-E6-PLAT-01: Ekran S17 „Podsumowanie okresu (AI)”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E6](epiki/E6-podsumuj-podsumowania-ai-bez-zmyslania-z-kaskada.md) Podsumuj: podsumowania AI bez zmyślania, z kaskadą |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E6-PLAT-01 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E6-NARRACJA-AI-03](MEM-E6-NARRACJA-AI-03-zamow-podsumowanie-okresu-c82.md); [MEM-E6-NARRACJA-AI-04](MEM-E6-NARRACJA-AI-04-zatwierdz-i-zapisz-podsumowanie-c83.md); [MEM-E6-NARRACJA-AI-05](MEM-E6-NARRACJA-AI-05-dodaj-podsumowanie-do-albumu-c84.md); [MEM-E6-NARRACJA-AI-06](MEM-E6-NARRACJA-AI-06-zapytanie-materialy-dozwolone-do-podsumowania-zgod.md) |
| Blokuje | - |
| Kod roboczy | MEM-E6-PLAT-01 |
| Źródło | specyfikacja ekranu S17; testy przejść S17 |

## Cel

Ekran S17 „Podsumowanie okresu (AI)” — część epiku E6 „Podsumuj: podsumowania AI bez zmyślania, z kaskadą”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S17 „Podsumowanie okresu (AI)” (źródło: specyfikacja ekranu S17; testy przejść S17)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Okres, Materiały dozwolone, Pominięto N uczestników (brak zgody), Tekst (oznaczony jako AI). *Sprawdzenie: test komponentu*
- [ ] Akcje: „Zamów podsumowanie” (dostępna gdy: limit AI dostępny); „Zatwierdź i zapisz”; „Dodaj do albumu (widzą uczestnicy)”. *Sprawdzenie: test komponentu*
- [ ] Stany: loading — „Przygotowujemy podsumowanie — damy znać powiadomieniem” (asynchronicznie, SD9); error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — zamówienie wyłączone; zapisane podsumowania do odczytu; empty — „Brak materiałów, które wolno użyć”; forbidden — n/d: tylko materiały, do których mam dostęp (R8). *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S17 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
