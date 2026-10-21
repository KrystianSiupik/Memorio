# MEM-E3-PLAT-06: Ekran S14 „Zaproszenie do albumu”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E3-PLAT-06 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E3-ALBUM-03](MEM-E3-ALBUM-03-przyjmij-zaproszenie-c44.md); [MEM-E3-ALBUM-04](MEM-E3-ALBUM-04-odrzuc-zaproszenie-c45.md); [MEM-E3-POWIADOMIENIA-02](MEM-E3-POWIADOMIENIA-02-zablokuj-zaproszenia-z-geo-na-swoj-adres-c46.md); [MEM-E3-ALBUM-18](MEM-E3-ALBUM-18-zapytanie-zaproszenie-kto-zaprasza-nazwa-albumu-be.md) |
| Blokuje | - |
| Kod roboczy | MEM-E3-PLAT-06 |
| Źródło | specyfikacja ekranu S14; testy przejść S14 |

## Cel

Ekran S14 „Zaproszenie do albumu” — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S14 „Zaproszenie do albumu” (źródło: specyfikacja ekranu S14; testy przejść S14)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Kto zaprasza, Album. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Przyjmij”; „Odrzuć”; „Nie chcę zaproszeń z GEO”. *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — n/d: zaproszenie zawsze ma dane; forbidden — „Zaproszenie wygasło albo zostało wycofane”. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S14 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*
- [ ] Odrzucenie wynikające z rywalizacji (np. „już rozstrzygnięte”, „pula pełna”) pokazane jako komunikat, bez utraty wpisanych danych. *Sprawdzenie: test komponentu z odpowiedzią 409*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
