# MEM-E5-PLAT-01: Ekran S08 „Prześledź okres”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E5](epiki/E5-przesledz-odtwarzanie-okresu-na-mapie.md) Prześledź: odtwarzanie okresu na mapie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E5-PLAT-01 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E5-ALBUM-01](MEM-E5-ALBUM-01-utworz-album-z-odtworzonego-okresu-c71.md); [MEM-E5-ARCHIWUM-01](MEM-E5-ARCHIWUM-01-read-port-wspomnienia-okresu-przecinajace-okres-w-.md) |
| Blokuje | - |
| Kod roboczy | MEM-E5-PLAT-01 |
| Źródło | specyfikacja ekranu S08; testy przejść S08 |

## Cel

Ekran S08 „Prześledź okres” — część epiku E5 „Prześledź: odtwarzanie okresu na mapie”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S08 „Prześledź okres” (źródło: specyfikacja ekranu S08; testy przejść S08)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Okres, Wspomnienia w kolejności, Punkt główny. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Utwórz album z okresu”. *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — „W tym okresie nie ma wspomnień”; forbidden — n/d: pokazuje tylko treści, do których mam dostęp (R7 przez read port). *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S08 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
