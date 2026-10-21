# MEM-E3-POWIADOMIENIA-01: Wyślij przypomnienie o zaproszeniu (C43)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, powiadomienia, MEM-E3-POWIADOMIENIA-01 |
| Moduł | powiadomienia |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-POWIADOMIENIA-01](MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md) |
| Blokuje | [MEM-E3-POWIADOMIENIA-07](MEM-E3-POWIADOMIENIA-07-polityka-p6-zawsze-gdy-minie-7-dni-od-nieprzyjeteg.md) |
| Kod roboczy | MEM-E3-POWIADOMIENIA-01 |
| Źródło | ES:C43 — S2:C10 |

## Cel

Wyślij przypomnienie o zaproszeniu (C43) — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **powiadomienia** — kluczowe pytanie: Czy wiadomość dotarła i do kogo nie wolno pisać?
- Czego moduł nie wie (nie wolno tego wymagać): sens biznesowy wiadomości poza kategorią
- Projekt modułu: model procesów modułu powiadomienia
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Wyślij przypomnienie o zaproszeniu (C43) (źródło: ES:C43 — S2:C10)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-POWIADOMIENIA-07)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Wyślij przypomnienie o zaproszeniu”, then powstaje „Przypomnienie o zaproszeniu wysłane” (E46). *Sprawdzenie: test jednostkowy handlera*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
