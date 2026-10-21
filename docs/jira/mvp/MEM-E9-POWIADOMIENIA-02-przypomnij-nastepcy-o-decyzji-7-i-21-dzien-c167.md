# MEM-E9-POWIADOMIENIA-02: Przypomnij następcy o decyzji (7. i 21. dzień) (C167)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, powiadomienia, MEM-E9-POWIADOMIENIA-02 |
| Moduł | powiadomienia |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-POWIADOMIENIA-01](MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md) |
| Blokuje | [MEM-E9-POWIADOMIENIA-04](MEM-E9-POWIADOMIENIA-04-polityka-p43-zawsze-gdy-zgon-potwierdzony-wtedy-za.md) |
| Kod roboczy | MEM-E9-POWIADOMIENIA-02 |
| Źródło | ES:C167 — przegląd decyzji 2026-10-19 (HS-021) |

## Cel

Przypomnij następcy o decyzji (7. i 21. dzień) (C167) — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Moduł **powiadomienia** — kluczowe pytanie: Czy wiadomość dotarła i do kogo nie wolno pisać?
- Czego moduł nie wie (nie wolno tego wymagać): sens biznesowy wiadomości poza kategorią
- Projekt modułu: model procesów modułu powiadomienia
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Przypomnij następcy o decyzji (7. i 21. dzień) (C167) (źródło: ES:C167 — przegląd decyzji 2026-10-19 (HS-021))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E9-POWIADOMIENIA-04)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Przypomnij następcy o decyzji (7. i 21. dzień)”, then powstaje „Następcy przypomniano o decyzji” (E173). *Sprawdzenie: test jednostkowy handlera*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
