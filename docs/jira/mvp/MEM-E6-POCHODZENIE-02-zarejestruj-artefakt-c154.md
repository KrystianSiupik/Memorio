# MEM-E6-POCHODZENIE-02: Zarejestruj artefakt (C154)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E6](epiki/E6-podsumuj-podsumowania-ai-bez-zmyslania-z-kaskada.md) Podsumuj: podsumowania AI bez zmyślania, z kaskadą |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, pochodzenie, MEM-E6-POCHODZENIE-02 |
| Moduł | pochodzenie |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-POCHODZENIE-01](MEM-E0-POCHODZENIE-01-szkielet-modulu-pochodzenie-struktura-publiczny-ko.md); [MEM-E6-POCHODZENIE-01](MEM-E6-POCHODZENIE-01-uniewaznij-artefakty-pochodne-c31.md) |
| Blokuje | [MEM-E6-POCHODZENIE-03](MEM-E6-POCHODZENIE-03-write-port-pochodzenie-dla-narracja-ai-zarejestruj.md) |
| Kod roboczy | MEM-E6-POCHODZENIE-02 |
| Źródło | ES:C154 — projekt agregatów 2026-10-17: port Narracja AI → Pochodzenie (S5) jako komenda; zapis, potem sprawdzenie (D-023) |

## Cel

Zarejestruj artefakt (C154) — część epiku E6 „Podsumuj: podsumowania AI bez zmyślania, z kaskadą”.

## Kontekst

- Moduł **pochodzenie** — kluczowe pytanie: Z czego powstał ten artefakt i czy nadal wolno mu istnieć?
- Czego moduł nie wie (nie wolno tego wymagać): przyczyna niedostępności źródła; treść artefaktu
- Projekt modułu: model procesów modułu pochodzenie
- Agregat AG9 „Artefakt pochodny”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zarejestruj artefakt (C154) (źródło: ES:C154 — projekt agregatów 2026-10-17: port Narracja AI → Pochodzenie (S5) jako komenda; zapis, potem sprawdzenie (D-023))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E6-POCHODZENIE-03)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Zarejestruj artefakt”, then powstaje „Artefakt zarejestrowany” (E163), „Artefakty pochodne unieważnione” (E61). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Niezmiennik AG9 „Artefakt pochodny”: Żywy artefakt nie ma źródła niedostępnego od chwili odczytu materiałów — zapis, potem sprawdzenie (D-023). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG9 „Artefakt pochodny”: Unieważniony artefakt nie wraca (S2:B2). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG9 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: zapis, potem sprawdzenie + kontrola dzienna. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- HS-018: Komendy wywoływane tylko przez write port (C150, C151) nie mają aktora ani polityki w gramatyce modelu — es.mjs zgłasza ostrzeżenie. To ograniczenie notacji, nie modelu: wywołującym jest komenda innego kontekstu (C13, C47, C56, C82). — Architekt
