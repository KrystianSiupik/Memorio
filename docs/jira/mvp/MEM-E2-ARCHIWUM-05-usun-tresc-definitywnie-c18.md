# MEM-E2-ARCHIWUM-05: Usuń treść definitywnie (C18)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E2-ARCHIWUM-05 |
| Moduł | archiwum |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md) |
| Blokuje | [MEM-E2-ARCHIWUM-06](MEM-E2-ARCHIWUM-06-przywroc-tresc-z-kosza-c19.md); [MEM-E7-ARCHIWUM-02](MEM-E7-ARCHIWUM-02-przywroc-przyciety-material-z-kosza-c168.md); [MEM-E2-ARCHIWUM-11](MEM-E2-ARCHIWUM-11-polityka-p3-zawsze-gdy-okno-kosza-minie-wtedy-usun.md); [MEM-E2-ARCHIWUM-12](MEM-E2-ARCHIWUM-12-zdarzenie-integracyjne-tresc-usunieta-definitywnie.md) |
| Kod roboczy | MEM-E2-ARCHIWUM-05 |
| Źródło | ES:C18 — S2:B3 |

## Cel

Usuń treść definitywnie (C18) — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Agregat AG10 „Treść w koszu”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Usuń treść definitywnie (C18) (źródło: ES:C18 — S2:B3)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E2-ARCHIWUM-06, MEM-E7-ARCHIWUM-02, MEM-E2-ARCHIWUM-11)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Usuń treść definitywnie”, then powstaje „Treść usunięta definitywnie” (E22), „Wspomnienie przestało istnieć (ostatnia treść usunięta)” (E23). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Niezmiennik AG10 „Treść w koszu”: Jeden wynik okna 30 dni: usunięta albo przywrócona — warunkowy zapis (D-024). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG10 „Treść w koszu”: Formalne żądanie usunięcia pomija kosz (S2:B3). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG10 „Treść w koszu”: Przycięty materiał wraca z kosza tylko w oknie 30 dni i tylko po opłaceniu planu (HS-022). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG10 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: warunkowy UPDATE stanu. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
