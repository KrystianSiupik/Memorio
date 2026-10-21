# MEM-E2-PLAT-01: Ekran S03 „Mapa i globus”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E2-PLAT-01 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E2-ARCHIWUM-08](MEM-E2-ARCHIWUM-08-zapytanie-mapa-wspomnien-punkty-wlasne-z-albumow-d.md) |
| Blokuje | - |
| Kod roboczy | MEM-E2-PLAT-01 |
| Źródło | specyfikacja ekranu S03; testy przejść S03 |

## Cel

Ekran S03 „Mapa i globus” — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S03 „Mapa i globus” (źródło: specyfikacja ekranu S03; testy przejść S03)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Punkty (klastry), Filtr: daty, Filtr: osoby, Filtr: albumy, Atrybucja OSM. *Sprawdzenie: test komponentu*
- [ ] Stany: loading — podkład mapy + licznik ładowania punktów (cel p95 < 2 s, QS3); error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — mapa z ostatnio pobranych punktów i kafelków; wspomnienia zapisane offline oznaczone „czeka na wysłanie”; empty — pusta mapa z zachętą „Dodaj pierwsze wspomnienie”; forbidden — n/d: własne dane. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S03 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
