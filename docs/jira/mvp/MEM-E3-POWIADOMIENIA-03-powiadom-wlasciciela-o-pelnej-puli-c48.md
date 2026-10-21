# MEM-E3-POWIADOMIENIA-03: Powiadom właściciela o pełnej puli (C48)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, powiadomienia, MEM-E3-POWIADOMIENIA-03 |
| Moduł | powiadomienia |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-POWIADOMIENIA-01](MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md) |
| Blokuje | [MEM-E3-POWIADOMIENIA-08](MEM-E3-POWIADOMIENIA-08-polityka-p7-zawsze-gdy-pula-albumu-wyczerpana-wted.md) |
| Kod roboczy | MEM-E3-POWIADOMIENIA-03 |
| Źródło | ES:C48 — S2:B1 |

## Cel

Powiadom właściciela o pełnej puli (C48) — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **powiadomienia** — kluczowe pytanie: Czy wiadomość dotarła i do kogo nie wolno pisać?
- Czego moduł nie wie (nie wolno tego wymagać): sens biznesowy wiadomości poza kategorią
- Projekt modułu: model procesów modułu powiadomienia
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Powiadom właściciela o pełnej puli (C48) (źródło: ES:C48 — S2:B1)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-POWIADOMIENIA-08)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Powiadom właściciela o pełnej puli”, then powstaje „Właściciel powiadomiony o pełnej puli” (E52). *Sprawdzenie: test jednostkowy handlera*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
