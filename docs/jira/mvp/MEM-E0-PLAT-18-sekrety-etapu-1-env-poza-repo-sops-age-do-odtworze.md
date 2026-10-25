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
- rotacja sekretów bez przestoju, osobno dla każdego rodzaju: klucz magazynu obiektów (nowy klucz obok starego, przełączenie API i workera, usunięcie starego, co unieważnia wszystkie podpisane linki), hasła ról bazy (moduły, `memorio_messaging`, `memorio_scheduler`), klucz Data Protection (źródło: rozmowa 2026-10-25, PLAT-04: pojedynczego podpisanego linku nie da się odwołać)
- runbook „wyciek sekretu” w `docs/runbooks/`: który sekret, kolejność kroków, skutki dla użytkowników, potwierdzenie, że stary sekret nie działa

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] plik .env poza repo z uprawnieniami użytkownika usługi. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*
- [ ] zaszyfrowana kopia w repo (sops+age), procedura odtworzenia. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*
- [ ] Rotacja klucza magazynu na lokalnym serwerze etapu 1: link podpisany starym kluczem po jego usunięciu dostaje 403, nowe linki działają, API i worker gotowe. *Sprawdzenie: skrypt na lokalnym serwerze etapu 1*
- [ ] Rotacja hasła roli bazy bez utraty gotowości API i workera. *Sprawdzenie: skrypt na lokalnym serwerze etapu 1*
- [ ] Runbook „wyciek sekretu” opisuje każdy sekret z `.env`. *Sprawdzenie: przegląd + porównanie z `deploy/env.example`*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
