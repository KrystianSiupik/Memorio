# MEM-E11-PLAT-06: Retencja aktów zgonu: osobny prefiks magazynu, czas do zamknięcia sprawy + 12 mies., dostęp tylko obsługa (D-055)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E11](epiki/E11-zgodnosc-przed-startem-dpia-umowy-procedury.md) Zgodność przed startem (DPIA, umowy, procedury) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Zgodność, -, MEM-E11-PLAT-06 |
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
| Kod roboczy | MEM-E11-PLAT-06 |
| Źródło | legal review R11 |

## Cel

Retencja aktów zgonu: osobny prefiks magazynu, czas do zamknięcia sprawy + 12 mies., dostęp tylko obsługa (D-055) — część epiku E11 „Zgodność przed startem (DPIA, umowy, procedury)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- polityka cyklu życia obiektów (źródło: legal review R11)
- dziennik wglądów (źródło: legal review R11)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] polityka cyklu życia obiektów. *Sprawdzenie: przegląd dokumentu przez PO/prawnika*
- [ ] dziennik wglądów. *Sprawdzenie: przegląd dokumentu przez PO/prawnika*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
