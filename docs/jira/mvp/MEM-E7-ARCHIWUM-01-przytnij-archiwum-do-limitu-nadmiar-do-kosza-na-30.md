# MEM-E7-ARCHIWUM-01: Przytnij archiwum do limitu (nadmiar do kosza na 30 dni) (C24)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E7-ARCHIWUM-01 |
| Moduł | archiwum |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md) |
| Blokuje | [MEM-E7-ARCHIWUM-03](MEM-E7-ARCHIWUM-03-polityka-p33-zawsze-gdy-przyciecie-zlecone-wtedy-p.md); [MEM-E7-ARCHIWUM-04](MEM-E7-ARCHIWUM-04-zdarzenie-integracyjne-archiwum-przyciete-nadmiar-.md) |
| Kod roboczy | MEM-E7-ARCHIWUM-01 |
| Źródło | ES:C24 — przejście 2: S5 A7: przycięcie wykonuje Archiwum na polecenie Planów |

## Cel

Przytnij archiwum do limitu (nadmiar do kosza na 30 dni) (C24) — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Przytnij archiwum do limitu (nadmiar do kosza na 30 dni) (C24) (źródło: ES:C24 — przejście 2: S5 A7: przycięcie wykonuje Archiwum na polecenie Planów)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E7-ARCHIWUM-03, MEM-E7-ARCHIWUM-04)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Przytnij archiwum do limitu (nadmiar do kosza na 30 dni)”, then powstaje „Archiwum przycięte (nadmiar w koszu 30 dni)” (E25). *Sprawdzenie: test jednostkowy handlera*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
