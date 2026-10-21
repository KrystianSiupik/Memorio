# MEM-E9-POWIADOMIENIA-01: Powiadom uczestników o nowym właścicielu (C166)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, powiadomienia, MEM-E9-POWIADOMIENIA-01 |
| Moduł | powiadomienia |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-POWIADOMIENIA-01](MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md) |
| Blokuje | [MEM-E9-POWIADOMIENIA-03](MEM-E9-POWIADOMIENIA-03-polityka-p42-zawsze-gdy-album-przejety-przez-uczes.md) |
| Kod roboczy | MEM-E9-POWIADOMIENIA-01 |
| Źródło | ES:C166 — przegląd decyzji 2026-10-19 (D-004) |

## Cel

Powiadom uczestników o nowym właścicielu (C166) — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Moduł **powiadomienia** — kluczowe pytanie: Czy wiadomość dotarła i do kogo nie wolno pisać?
- Czego moduł nie wie (nie wolno tego wymagać): sens biznesowy wiadomości poza kategorią
- Projekt modułu: model procesów modułu powiadomienia
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Powiadom uczestników o nowym właścicielu (C166) (źródło: ES:C166 — przegląd decyzji 2026-10-19 (D-004))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E9-POWIADOMIENIA-03)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Powiadom uczestników o nowym właścicielu”, then powstaje „Uczestnicy powiadomieni o nowym właścicielu” (E172). *Sprawdzenie: test jednostkowy handlera*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
