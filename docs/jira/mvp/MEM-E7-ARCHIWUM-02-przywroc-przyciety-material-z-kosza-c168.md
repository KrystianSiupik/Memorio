# MEM-E7-ARCHIWUM-02: Przywróć przycięty materiał z kosza (C168)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E7-ARCHIWUM-02 |
| Moduł | archiwum |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md); [MEM-E2-ARCHIWUM-05](MEM-E2-ARCHIWUM-05-usun-tresc-definitywnie-c18.md) |
| Blokuje | [MEM-E7-ARCHIWUM-05](MEM-E7-ARCHIWUM-05-polityka-p44-zawsze-gdy-plan-znowu-aktywny-w-ciagu.md) |
| Kod roboczy | MEM-E7-ARCHIWUM-02 |
| Źródło | ES:C168 — przegląd decyzji 2026-10-19 (HS-022) |

## Cel

Przywróć przycięty materiał z kosza (C168) — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Agregat AG10 „Treść w koszu”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Przywróć przycięty materiał z kosza (C168) (źródło: ES:C168 — przegląd decyzji 2026-10-19 (HS-022))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E7-ARCHIWUM-05)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Przywróć przycięty materiał z kosza”, then powstaje „Przycięty materiał przywrócony z kosza” (E174). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Niezmiennik AG10 „Treść w koszu”: Jeden wynik okna 30 dni: usunięta albo przywrócona — warunkowy zapis (D-024). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG10 „Treść w koszu”: Formalne żądanie usunięcia pomija kosz (S2:B3). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG10 „Treść w koszu”: Przycięty materiał wraca z kosza tylko w oknie 30 dni i tylko po opłaceniu planu (HS-022). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG10 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: warunkowy UPDATE stanu. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
