# MEM-E9-SUKCESJA-07: Uznaj brak odpowiedzi następcy (C126)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, sukcesja, MEM-E9-SUKCESJA-07 |
| Moduł | sukcesja |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-SUKCESJA-01](MEM-E0-SUKCESJA-01-szkielet-modulu-sukcesja-struktura-publiczny-kontr.md); [MEM-E9-SUKCESJA-03](MEM-E9-SUKCESJA-03-zweryfikuj-akt-zgonu-c121.md) |
| Blokuje | [MEM-E9-SUKCESJA-12](MEM-E9-SUKCESJA-12-polityka-p38-zawsze-gdy-termin-przejecia-minal-wte.md) |
| Kod roboczy | MEM-E9-SUKCESJA-07 |
| Źródło | ES:C126 — przegląd decyzji 2026-10-19 (HS-021: 30 dni) |

## Cel

Uznaj brak odpowiedzi następcy (C126) — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Moduł **sukcesja** — kluczowe pytanie: Kto przejmuje album lub odczyt, gdy właściciela już nie ma?
- Czego moduł nie wie (nie wolno tego wymagać): budowa albumu; mechanika dostępu
- Projekt modułu: model procesów modułu sukcesja
- Agregat AG7 „Sprawa przejęcia”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Uznaj brak odpowiedzi następcy (C126) (źródło: ES:C126 — przegląd decyzji 2026-10-19 (HS-021: 30 dni))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E9-SUKCESJA-12)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Uznaj brak odpowiedzi następcy”, then powstaje „Przejęcie albumu odrzucone” (E124). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Niezmiennik AG7 „Sprawa przejęcia”: Jedna decyzja na etap sprawy; druga dostaje „już rozstrzygnięte” (D-008, D-021). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG7 „Sprawa przejęcia”: Przejęcie lub odmowa tylko po weryfikacji aktu zgonu (S2:B3). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG7 „Sprawa przejęcia”: Przejąć może tylko następca z migawki zrobionej przy weryfikacji (D-020). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG7 „Sprawa przejęcia”: Następca ma 30 dni od potwierdzenia zgonu; po terminie skutek jak odmowa (HS-021). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG7 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: optymistyczna wersja sprawy. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
