# MEM-E11-PLAT-07: Ekran zgody przy pierwszym uruchomieniu: analityka i powiadomienia spoza „dotyczących danych” (D-050)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E11](epiki/E11-zgodnosc-przed-startem-dpia-umowy-procedury.md) Zgodność przed startem (DPIA, umowy, procedury) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E11-PLAT-07 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 2–3 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md) |
| Blokuje | - |
| Kod roboczy | MEM-E11-PLAT-07 |
| Źródło | legal review R06 (PKE art. 399); D-050 |

## Cel

Ekran zgody przy pierwszym uruchomieniu: analityka i powiadomienia spoza „dotyczących danych” (D-050) — część epiku E11 „Zgodność przed startem (DPIA, umowy, procedury)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- ekran zgody przed jakimkolwiek zapisem analityki (źródło: legal review R06 (PKE art. 399); D-050)
- zmiana zgody w S24 (źródło: legal review R06 (PKE art. 399); D-050)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] ekran zgody przed jakimkolwiek zapisem analityki. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*
- [ ] zmiana zgody w S24. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
