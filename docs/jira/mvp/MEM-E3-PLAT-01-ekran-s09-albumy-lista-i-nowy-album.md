# MEM-E3-PLAT-01: Ekran S09 „Albumy (lista) i nowy album”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E3-PLAT-01 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E3-ALBUM-01](MEM-E3-ALBUM-01-utworz-album-c40.md); [MEM-E3-DOSTEP-01](MEM-E3-DOSTEP-01-zapytanie-udostepnione-mi-albumy-z-odczytem-r14.md) |
| Blokuje | - |
| Kod roboczy | MEM-E3-PLAT-01 |
| Źródło | specyfikacja ekranu S09; testy przejść S09 |

## Cel

Ekran S09 „Albumy (lista) i nowy album” — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S09 „Albumy (lista) i nowy album” (źródło: specyfikacja ekranu S09; testy przejść S09)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Moje albumy, Udostępnione mi, Nazwa nowego albumu. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Utwórz album”. *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — „Nie masz jeszcze albumów” + Utwórz album; forbidden — n/d: lista własna. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S09 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
