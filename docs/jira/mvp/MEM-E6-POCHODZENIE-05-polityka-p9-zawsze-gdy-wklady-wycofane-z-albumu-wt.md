# MEM-E6-POCHODZENIE-05: Polityka P9: Zawsze gdy wkłady wycofane z albumu, wtedy unieważnij artefakty pochodne, które je cytują

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E6](epiki/E6-podsumuj-podsumowania-ai-bez-zmyslania-z-kaskada.md) Podsumuj: podsumowania AI bez zmyślania, z kaskadą |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, pochodzenie, MEM-E6-POCHODZENIE-05 |
| Moduł | pochodzenie |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-POCHODZENIE-01](MEM-E0-POCHODZENIE-01-szkielet-modulu-pochodzenie-struktura-publiczny-ko.md); [MEM-E6-POCHODZENIE-01](MEM-E6-POCHODZENIE-01-uniewaznij-artefakty-pochodne-c31.md); [MEM-E3-ALBUM-23](MEM-E3-ALBUM-23-zdarzenie-integracyjne-wklady-wycofane-z-albumu-e5.md) |
| Blokuje | - |
| Kod roboczy | MEM-E6-POCHODZENIE-05 |
| Źródło | ES:P9 — S2:B1 Odwoływalność; S2:B2 Kaskada |

## Cel

Polityka P9: Zawsze gdy wkłady wycofane z albumu, wtedy unieważnij artefakty pochodne, które je cytują — część epiku E6 „Podsumuj: podsumowania AI bez zmyślania, z kaskadą”.

## Kontekst

- Moduł **pochodzenie** — kluczowe pytanie: Z czego powstał ten artefakt i czy nadal wolno mu istnieć?
- Czego moduł nie wie (nie wolno tego wymagać): przyczyna niedostępności źródła; treść artefaktu
- Projekt modułu: model procesów modułu pochodzenie
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P9: Zawsze gdy wkłady wycofane z albumu, wtedy unieważnij artefakty pochodne, które je cytują (źródło: ES:P9 — S2:B1 Odwoływalność; S2:B2 Kaskada)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E58, when handler je przetwarza, then wywołana jest komenda C31. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
