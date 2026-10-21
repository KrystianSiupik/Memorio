# MEM-E3-PLAT-05: Ekran S13 „Powierzone treści (kurator)”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E3-PLAT-05 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E3-ALBUM-08](MEM-E3-ALBUM-08-uporzadkuj-powierzone-tresci-c51.md); [MEM-E3-ALBUM-17](MEM-E3-ALBUM-17-zapytanie-album-wspomnienia-sklad-i-moje-wklady-r1.md) |
| Blokuje | - |
| Kod roboczy | MEM-E3-PLAT-05 |
| Źródło | specyfikacja ekranu S13; testy przejść S13 |

## Cel

Ekran S13 „Powierzone treści (kurator)” — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S13 „Powierzone treści (kurator)” (źródło: specyfikacja ekranu S13; testy przejść S13)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Powierzone treści. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Uporządkuj” (dostępna gdy: treść powierzona mi). *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — „Nikt nie powierzył Ci treści”; forbidden — tylko kurator z aktywnym powierzeniem. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S13 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
