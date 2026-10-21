# MEM-E2-PLAT-02: Ekran S04 „Nowe wspomnienie (kreator: miejsce → treść)”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E2-PLAT-02 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 4–6 h (wyjątek: ekran złożony (6 akcji); dzielenie rozbiłoby jeden stan ekranu na dwa tickety bez osobnego rezultatu (D-072)) |
| Pewność | niska |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E2-MIEJSCA-01](MEM-E2-MIEJSCA-01-wybierz-miejsce-z-podpowiedzi-c10.md); [MEM-E2-MIEJSCA-02](MEM-E2-MIEJSCA-02-dodaj-miejsce-wlasne-c11.md); [MEM-E2-ARCHIWUM-01](MEM-E2-ARCHIWUM-01-utworz-wspomnienie-c12.md); [MEM-E2-ARCHIWUM-02](MEM-E2-ARCHIWUM-02-dodaj-zdjecia-do-wspomnienia-c13.md); [MEM-E2-NARRACJA-AI-01](MEM-E2-NARRACJA-AI-01-popros-ai-o-szkic-opisu-c15.md); [MEM-E2-ARCHIWUM-03](MEM-E2-ARCHIWUM-03-zatwierdz-opis-c16.md); [MEM-E2-MIEJSCA-03](MEM-E2-MIEJSCA-03-zapytanie-podpowiedzi-miejsc-openstreetmap-miejsca.md) |
| Blokuje | - |
| Kod roboczy | MEM-E2-PLAT-02 |
| Źródło | specyfikacja ekranu S04; testy przejść S04 |

## Cel

Ekran S04 „Nowe wspomnienie (kreator: miejsce → treść)” — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S04 „Nowe wspomnienie (kreator: miejsce → treść)” (źródło: specyfikacja ekranu S04; testy przejść S04)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Miejsce, Nazwa własna miejsca, Data (dzień / miesiąc / rok / okres), Zdjęcia, Opis, Szkic AI (oznaczony jako AI). *Sprawdzenie: test komponentu*
- [ ] Akcje: „Wybierz miejsce”; „Dodaj miejsce własne”; „Zapisz wspomnienie” (dostępna gdy: miejsce i data podane; data nie w przyszłości); „Dodaj zdjęcia” (dostępna gdy: pula przestrzeni nie jest pełna); „Poproś AI o szkic” (dostępna gdy: zgoda na AI udzielona; limit AI dostępny); „Zatwierdź opis”. *Sprawdzenie: test komponentu*
- [ ] Stany: loading — n/d: formularz (podpowiedzi miejsc ładowane przy wpisywaniu); error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — zapis do kolejki offline (UUID z klienta, D-039); podpowiedzi miejsc tylko z ostatnio użytych; szkic AI niedostępny; empty — n/d: formularz; forbidden — n/d: własne dane. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S04 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
