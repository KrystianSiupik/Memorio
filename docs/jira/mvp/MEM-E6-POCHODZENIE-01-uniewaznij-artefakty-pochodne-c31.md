# MEM-E6-POCHODZENIE-01: Unieważnij artefakty pochodne (C31)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E6](epiki/E6-podsumuj-podsumowania-ai-bez-zmyslania-z-kaskada.md) Podsumuj: podsumowania AI bez zmyślania, z kaskadą |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, pochodzenie, MEM-E6-POCHODZENIE-01 |
| Moduł | pochodzenie |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-POCHODZENIE-01](MEM-E0-POCHODZENIE-01-szkielet-modulu-pochodzenie-struktura-publiczny-ko.md) |
| Blokuje | [MEM-E6-POCHODZENIE-02](MEM-E6-POCHODZENIE-02-zarejestruj-artefakt-c154.md); [MEM-E6-POCHODZENIE-04](MEM-E6-POCHODZENIE-04-polityka-p4-zawsze-gdy-tresc-usunieta-definitywnie.md); [MEM-E6-POCHODZENIE-05](MEM-E6-POCHODZENIE-05-polityka-p9-zawsze-gdy-wklady-wycofane-z-albumu-wt.md); [MEM-E6-POCHODZENIE-06](MEM-E6-POCHODZENIE-06-polityka-p10-zawsze-gdy-dostep-odebrany-wtedy-unie.md); [MEM-E6-POCHODZENIE-07](MEM-E6-POCHODZENIE-07-polityka-p12-zawsze-gdy-zgoda-na-ai-odwolana-wtedy.md); [MEM-E6-POCHODZENIE-08](MEM-E6-POCHODZENIE-08-polityka-p34-zawsze-gdy-archiwum-przyciete-wtedy-u.md) |
| Kod roboczy | MEM-E6-POCHODZENIE-01 |
| Źródło | ES:C31 — S2:B1 pkt 3, B2 Kaskada, C5 |

## Cel

Unieważnij artefakty pochodne (C31) — część epiku E6 „Podsumuj: podsumowania AI bez zmyślania, z kaskadą”.

## Kontekst

- Moduł **pochodzenie** — kluczowe pytanie: Z czego powstał ten artefakt i czy nadal wolno mu istnieć?
- Czego moduł nie wie (nie wolno tego wymagać): przyczyna niedostępności źródła; treść artefaktu
- Projekt modułu: model procesów modułu pochodzenie
- Agregat AG9 „Artefakt pochodny”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Unieważnij artefakty pochodne (C31) (źródło: ES:C31 — S2:B1 pkt 3, B2 Kaskada, C5)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E6-POCHODZENIE-02, MEM-E6-POCHODZENIE-04, MEM-E6-POCHODZENIE-05)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Unieważnij artefakty pochodne”, then powstaje „Artefakty pochodne unieważnione” (E61). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Niezmiennik AG9 „Artefakt pochodny”: Żywy artefakt nie ma źródła niedostępnego od chwili odczytu materiałów — zapis, potem sprawdzenie (D-023). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG9 „Artefakt pochodny”: Unieważniony artefakt nie wraca (S2:B2). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG9 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: zapis, potem sprawdzenie + kontrola dzienna. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
