# MEM-E11-PLAT-02: Umowy powierzenia z dostawcami + rejestr czynności + lista podwykonawców (D-046)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E11](epiki/E11-zgodnosc-przed-startem-dpia-umowy-procedury.md) Zgodność przed startem (DPIA, umowy, procedury) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Zgodność, -, MEM-E11-PLAT-02 |
| Moduł | - |
| Warstwa | proces / dokument |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | niska |
| Status | gotowy |
| Zależy od | - |
| Blokuje | - |
| Kod roboczy | MEM-E11-PLAT-02 |
| Źródło | legal review R02, R13 |

## Cel

Umowy powierzenia z dostawcami + rejestr czynności + lista podwykonawców (D-046) — część epiku E11 „Zgodność przed startem (DPIA, umowy, procedury)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- lista podmiotów z C4 (źródło: legal review R02, R13)
- umowy art. 28 z VPS, magazynem, e-mail/push, AI (źródło: legal review R02, R13)
- rejestr czynności (art. 30) (źródło: legal review R02, R13)
- warunki dla dostawcy AI w umowie i w ustawieniach konta: brak trenowania modeli na naszych danych (zapytania i odpowiedzi), retencja zapytań i odpowiedzi zero albo możliwie krótka (bez domyślnego przechowywania do monitorowania nadużyć, jeśli dostawca pozwala to wyłączyć), przetwarzanie w UE (źródło: rozmowa 2026-10-25, PLAT-06: pseudonimizacja nie jest anonimizacją, dane w ładunku nadal są osobowe)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] lista podmiotów z C4. *Sprawdzenie: przegląd dokumentu przez PO/prawnika*
- [ ] umowy art. 28 z VPS, magazynem, e-mail/push, AI. *Sprawdzenie: przegląd dokumentu przez PO/prawnika*
- [ ] rejestr czynności (art. 30). *Sprawdzenie: przegląd dokumentu przez PO/prawnika*
- [ ] Umowa z dostawcą AI wyklucza trenowanie na naszych danych i określa retencję zapytań i odpowiedzi (zero albo konkretna liczba dni); ustawienia konta u dostawcy są z tym zgodne (zrzut ekranu lub eksport ustawień w dokumentacji). *Sprawdzenie: przegląd dokumentu przez PO/prawnika*
- [ ] Ryzyko szczątkowe pseudonimizacji (identyfikacja z kontekstu) i retencja u dostawcy opisane w DPIA (MEM-E11-PLAT-01). *Sprawdzenie: przegląd dokumentu przez PO/prawnika*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
