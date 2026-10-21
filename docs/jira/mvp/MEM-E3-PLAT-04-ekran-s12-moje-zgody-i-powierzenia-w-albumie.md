# MEM-E3-PLAT-04: Ekran S12 „Moje zgody i powierzenia w albumie”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E3-PLAT-04 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E6-NARRACJA-AI-01](MEM-E6-NARRACJA-AI-01-udziel-zgody-na-ai-dla-albumu-c80.md); [MEM-E6-NARRACJA-AI-02](MEM-E6-NARRACJA-AI-02-odwolaj-zgode-na-ai-c81.md); [MEM-E3-ALBUM-11](MEM-E3-ALBUM-11-odwolaj-zgode-na-pozostawienie-wkladow-c55.md); [MEM-E3-ALBUM-06](MEM-E3-ALBUM-06-powierz-tresci-kuratorowi-c49.md); [MEM-E3-ALBUM-07](MEM-E3-ALBUM-07-odwolaj-powierzenie-c50.md); [MEM-E3-ALBUM-17](MEM-E3-ALBUM-17-zapytanie-album-wspomnienia-sklad-i-moje-wklady-r1.md) |
| Blokuje | - |
| Kod roboczy | MEM-E3-PLAT-04 |
| Źródło | specyfikacja ekranu S12; testy przejść S12 |

## Cel

Ekran S12 „Moje zgody i powierzenia w albumie” — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S12 „Moje zgody i powierzenia w albumie” (źródło: specyfikacja ekranu S12; testy przejść S12)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Zgoda na AI dla albumu, Moje wkłady pozostawione po wyjściu, Kurator. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Zgadzam się na AI”; „Odwołaj zgodę na AI”; „Odwołaj zgodę „zostaw” (wycofa treści i artefakty)” (dostępna gdy: potwierdzenie ze skutkami); „Powierz kuratorowi”; „Odwołaj powierzenie”. *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — n/d: zawsze są zgody do pokazania; forbidden — n/d: własne zgody. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S12 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
