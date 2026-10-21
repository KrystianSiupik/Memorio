# MEM-E7-POWIADOMIENIA-01: Wyślij ostrzeżenie przed przycięciem (C92)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, powiadomienia, MEM-E7-POWIADOMIENIA-01 |
| Moduł | powiadomienia |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-POWIADOMIENIA-01](MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md) |
| Blokuje | [MEM-E7-POWIADOMIENIA-02](MEM-E7-POWIADOMIENIA-02-polityka-p15-zawsze-gdy-nadejdzie-termin-ostrzezen.md); [MEM-E7-POWIADOMIENIA-03](MEM-E7-POWIADOMIENIA-03-zdarzenie-integracyjne-ostrzezenie-niedoreczone-e9.md); [MEM-E7-POWIADOMIENIA-04](MEM-E7-POWIADOMIENIA-04-polityka-p26-zawsze-gdy-przyciecie-wstrzymane-wted.md) |
| Kod roboczy | MEM-E7-POWIADOMIENIA-01 |
| Źródło | ES:C92 — S2:B1, C10 |

## Cel

Wyślij ostrzeżenie przed przycięciem (C92) — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **powiadomienia** — kluczowe pytanie: Czy wiadomość dotarła i do kogo nie wolno pisać?
- Czego moduł nie wie (nie wolno tego wymagać): sens biznesowy wiadomości poza kategorią
- Projekt modułu: model procesów modułu powiadomienia
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Wyślij ostrzeżenie przed przycięciem (C92) (źródło: ES:C92 — S2:B1, C10)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E7-POWIADOMIENIA-02, MEM-E7-POWIADOMIENIA-03, MEM-E7-POWIADOMIENIA-04)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Wyślij ostrzeżenie przed przycięciem”, then powstaje „Ostrzeżenie doręczone” (E98). *Sprawdzenie: test jednostkowy handlera*
- [ ] Given warunek odrzucenia, when „Wyślij ostrzeżenie przed przycięciem”, then „Ostrzeżenie niedoręczone” (E99) i brak zapisu. *Sprawdzenie: test jednostkowy*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
