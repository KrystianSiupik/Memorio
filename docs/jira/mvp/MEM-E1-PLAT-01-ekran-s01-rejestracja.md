# MEM-E1-PLAT-01: Ekran S01 „Rejestracja”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E1](epiki/E1-konto-rejestracja-16-usuniecie-z-oknem-30-dni.md) Konto: rejestracja 16+, usunięcie z oknem 30 dni |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E1-PLAT-01 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E1-KONTO-01](MEM-E1-KONTO-01-zarejestruj-konto-c1.md); [MEM-E1-KONTO-02](MEM-E1-KONTO-02-zaakceptuj-regulamin-c2.md) |
| Blokuje | - |
| Kod roboczy | MEM-E1-PLAT-01 |
| Źródło | specyfikacja ekranu S01; testy przejść S01 |

## Cel

Ekran S01 „Rejestracja” — część epiku E1 „Konto: rejestracja 16+, usunięcie z oknem 30 dni”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S01 „Rejestracja” (źródło: specyfikacja ekranu S01; testy przejść S01)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: E-mail, Hasło, Mam ukończone 16 lat, Akceptuję regulamin. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Załóż konto” (dostępna gdy: deklaracja 16+ zaznaczona); „Akceptuję regulamin”. *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner offline, przycisk „Załóż konto” wyłączony; empty — n/d: formularz; forbidden — n/d: ekran dla niezalogowanych. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S01 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
