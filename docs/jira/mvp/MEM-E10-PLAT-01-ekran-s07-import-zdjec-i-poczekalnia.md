# MEM-E10-PLAT-01: Ekran S07 „Import zdjęć i poczekalnia”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E10](epiki/E10-import-zdjec-z-telefonu-po-mvp.md) Import zdjęć z telefonu (po MVP) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E10-PLAT-01 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P2 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E10-ARCHIWUM-01](MEM-E10-ARCHIWUM-01-zaimportuj-zdjecia-z-telefonu-c21.md); [MEM-E10-ARCHIWUM-02](MEM-E10-ARCHIWUM-02-przypisz-miejsce-grupie-zdjec-c22.md); [MEM-E10-ARCHIWUM-03](MEM-E10-ARCHIWUM-03-usun-zdjecie-z-poczekalni-c23.md); [MEM-E10-ARCHIWUM-04](MEM-E10-ARCHIWUM-04-zapytanie-poczekalnia-do-przypisania-r4.md) |
| Blokuje | - |
| Kod roboczy | MEM-E10-PLAT-01 |
| Źródło | specyfikacja ekranu S07; testy przejść S07 |

## Cel

Ekran S07 „Import zdjęć i poczekalnia” — część epiku E10 „Import zdjęć z telefonu (po MVP)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S07 „Import zdjęć i poczekalnia” (źródło: specyfikacja ekranu S07; testy przejść S07)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Grupy zdjęć (miejsce, czas), Postęp importu, Pominięte duplikaty. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Importuj z telefonu” (dostępna gdy: zgoda systemowa na galerię); „Przypisz miejsce grupie”; „Usuń z poczekalni”. *Sprawdzenie: test komponentu*
- [ ] Stany: loading — pasek postępu importu, aplikacja dalej używalna; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — import wstrzymany, wznowi się po powrocie sieci (QS10); empty — „Poczekalnia jest pusta”; forbidden — n/d: każdy zalogowany ma dostęp do własnych danych. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S07 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
