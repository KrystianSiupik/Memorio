# MEM-E0-PLAT-18: Sekrety etapu 1: .env poza repo + sops/age do odtworzenia (RD10)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Platforma, -, MEM-E0-PLAT-18 |
| Moduł | - |
| Warstwa | infrastruktura |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-09](MEM-E0-PLAT-09-etap-1-vps-compose-nginx-wal-g-comiesieczny-test-o.md) |
| Blokuje | - |
| Kod roboczy | MEM-E0-PLAT-18 |
| Źródło | infra RD10; D-061 |

## Cel

Sekrety etapu 1: .env poza repo + sops/age do odtworzenia (RD10) — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- plik .env poza repo z uprawnieniami użytkownika usługi (źródło: infra RD10; D-061)
- zaszyfrowana kopia w repo (sops+age), procedura odtworzenia (źródło: infra RD10; D-061)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] plik .env poza repo z uprawnieniami użytkownika usługi. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*
- [ ] zaszyfrowana kopia w repo (sops+age), procedura odtworzenia. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
