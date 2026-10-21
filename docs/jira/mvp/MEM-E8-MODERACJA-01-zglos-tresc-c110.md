# MEM-E8-MODERACJA-01: Zgłoś treść (C110)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E8](epiki/E8-zgloszenia-moderacja-i-nakazy-organow.md) Zgłoszenia, moderacja i nakazy organów |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, moderacja, MEM-E8-MODERACJA-01 |
| Moduł | moderacja |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-MODERACJA-01](MEM-E0-MODERACJA-01-szkielet-modulu-moderacja-struktura-publiczny-kont.md) |
| Blokuje | [MEM-E8-PLAT-01](MEM-E8-PLAT-01-ekran-s21-zglos-tresc.md); [MEM-E8-PLAT-04](MEM-E8-PLAT-04-ekran-s27-zglos-tresc-z-aplikacji.md) |
| Kod roboczy | MEM-E8-MODERACJA-01 |
| Źródło | ES:C110 — S2:C2 KOMENDA; S3:L10 (zgłoszenie już w MVP) |

## Cel

Zgłoś treść (C110) — część epiku E8 „Zgłoszenia, moderacja i nakazy organów”.

## Kontekst

- Moduł **moderacja** — kluczowe pytanie: Co z tą treścią robimy po zgłoszeniu lub nakazie, i w jakim terminie?
- Czego moduł nie wie (nie wolno tego wymagać): prywatna treść poza zgłoszonym zakresem; relacje w albumach
- Projekt modułu: model procesów modułu moderacja
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zgłoś treść (C110) (źródło: ES:C110 — S2:C2 KOMENDA; S3:L10 (zgłoszenie już w MVP))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E8-PLAT-01, MEM-E8-PLAT-04)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Osoba zgłaszająca (także bez konta) wykonuje „Zgłoś treść”, then powstaje „Treść zgłoszona” (E110). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
