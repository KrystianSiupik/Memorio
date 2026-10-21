# MEM-E7-PLANY-01: Nadaj uprawnienia planu (C90)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, plany, MEM-E7-PLANY-01 |
| Moduł | plany |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLANY-01](MEM-E0-PLANY-01-szkielet-modulu-plany-struktura-publiczny-kontrakt.md) |
| Blokuje | [MEM-E7-PLANY-02](MEM-E7-PLANY-02-przelacz-w-tryb-tylko-do-odczytu-c91.md); [MEM-E7-PLANY-04](MEM-E7-PLANY-04-wstrzymaj-przyciecie-archiwum-c93.md); [MEM-E7-PLANY-05](MEM-E7-PLANY-05-wskaz-material-do-zachowania-c94.md); [MEM-E7-PLANY-06](MEM-E7-PLANY-06-zlec-przyciecie-archiwum-do-darmowego-limitu-c95.md); [MEM-E7-PLANY-10](MEM-E7-PLANY-10-przelacz-konto-w-tryb-ponad-limitem-c100.md); [MEM-E7-PLANY-11](MEM-E7-PLANY-11-aktywuj-plan-albumu-po-przejeciu-c165.md); [MEM-E3-PLANY-01](MEM-E3-PLANY-01-read-port-plany-dla-album-limit-miejsc-dla-albumu.md); [MEM-E7-PLANY-13](MEM-E7-PLANY-13-polityka-p13-zawsze-gdy-subskrypcja-aktywowana-wte.md); [MEM-E7-PLANY-20](MEM-E7-PLANY-20-zdarzenie-integracyjne-plan-aktywny-e94-w-kontrakc.md); [MEM-E7-PLANY-24](MEM-E7-PLANY-24-zdarzenie-integracyjne-plan-albumu-bez-wlasciciela.md) |
| Kod roboczy | MEM-E7-PLANY-01 |
| Źródło | ES:C90 — S2:C3 |

## Cel

Nadaj uprawnienia planu (C90) — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **plany** — kluczowe pytanie: W jakim stanie jest teraz plan tego konta lub albumu i co z tego wynika?
- Czego moduł nie wie (nie wolno tego wymagać): co jest w archiwum; kto co wniósł; zgony, następcy, odmowy przejęcia (zna tylko „album bez właściciela”)
- Projekt modułu: model procesów modułu plany
- Agregat AG8 „Cykl planu”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Nadaj uprawnienia planu (C90) (źródło: ES:C90 — S2:C3)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E7-PLANY-02, MEM-E7-PLANY-04, MEM-E7-PLANY-05)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Nadaj uprawnienia planu”, then powstaje „Plan aktywny” (E94), „Plan albumu bez właściciela opłacony przez uczestnika” (E171). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Niezmiennik AG8 „Cykl planu”: Przycięcie tylko gdy tryb tylko do odczytu trwa ≥ 24 mies., okno wyboru minęło, brak wstrzymania i brak opłaty (S2:B1, S2:C10); przycięty materiał trafia na 30 dni do kosza (HS-022). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG8 „Cykl planu”: Zdarzenie pośrednika starsze niż ostatnio zastosowane jest ignorowane (S2:C3). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG8 „Cykl planu”: Album bez właściciela przechodzi w tryb tylko do odczytu dopiero po końcu opłaconego okresu (HS-020); wraca do aktywnego tylko po przejęciu przez uczestnika i opłaceniu (D-004). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG8 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: optymistyczna wersja (version). *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
