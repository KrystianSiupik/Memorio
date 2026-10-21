# MEM-E11-PLAT-08: Sekcja „Podwykonawcy” w ekranie Konto i prywatność (S24)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E11](epiki/E11-zgodnosc-przed-startem-dpia-umowy-procedury.md) Zgodność przed startem (DPIA, umowy, procedury) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E11-PLAT-08 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md) |
| Blokuje | - |
| Kod roboczy | MEM-E11-PLAT-08 |
| Źródło | legal review R02; S2:C5; D-046 |

## Cel

Sekcja „Podwykonawcy” w ekranie Konto i prywatność (S24) — część epiku E11 „Zgodność przed startem (DPIA, umowy, procedury)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- lista podwykonawców z krajem przetwarzania (źródło: legal review R02; S2:C5; D-046)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] lista podwykonawców z krajem przetwarzania. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
