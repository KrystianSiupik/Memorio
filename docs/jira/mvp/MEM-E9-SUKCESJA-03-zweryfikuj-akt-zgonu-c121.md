# MEM-E9-SUKCESJA-03: Zweryfikuj akt zgonu (C121)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, sukcesja, MEM-E9-SUKCESJA-03 |
| Moduł | sukcesja |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-SUKCESJA-01](MEM-E0-SUKCESJA-01-szkielet-modulu-sukcesja-struktura-publiczny-kontr.md) |
| Blokuje | [MEM-E9-SUKCESJA-04](MEM-E9-SUKCESJA-04-przekaz-album-nastepcy-c122.md); [MEM-E9-SUKCESJA-05](MEM-E9-SUKCESJA-05-odmow-przejecia-albumu-c123.md); [MEM-E9-SUKCESJA-07](MEM-E9-SUKCESJA-07-uznaj-brak-odpowiedzi-nastepcy-c126.md); [MEM-E9-SUKCESJA-09](MEM-E9-SUKCESJA-09-polityka-p21-zawsze-gdy-zgon-potwierdzony-wtedy-pr.md); [MEM-E9-SUKCESJA-10](MEM-E9-SUKCESJA-10-polityka-p23-zawsze-gdy-zgon-potwierdzony-wtedy-ud.md); [MEM-E9-SUKCESJA-13](MEM-E9-SUKCESJA-13-zdarzenie-integracyjne-zgon-potwierdzony-e121-w-ko.md); [MEM-E8-PLAT-03](MEM-E8-PLAT-03-ekran-s26-panel-obslugi-zgloszenia-nakazy-sukcesja.md) |
| Kod roboczy | MEM-E9-SUKCESJA-03 |
| Źródło | ES:C121 — S2:B3 „obsługa weryfikuje akt zgonu” (Z1) |

## Cel

Zweryfikuj akt zgonu (C121) — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Moduł **sukcesja** — kluczowe pytanie: Kto przejmuje album lub odczyt, gdy właściciela już nie ma?
- Czego moduł nie wie (nie wolno tego wymagać): budowa albumu; mechanika dostępu
- Projekt modułu: model procesów modułu sukcesja
- Agregat AG7 „Sprawa przejęcia”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zweryfikuj akt zgonu (C121) (źródło: ES:C121 — S2:B3 „obsługa weryfikuje akt zgonu” (Z1))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E9-SUKCESJA-04, MEM-E9-SUKCESJA-05, MEM-E9-SUKCESJA-07)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Obsługa GEO wykonuje „Zweryfikuj akt zgonu”, then powstaje „Zgon potwierdzony” (E121). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Given warunek odrzucenia, when „Zweryfikuj akt zgonu”, then „Zgłoszenie zgonu odrzucone” (E122) i brak zapisu. *Sprawdzenie: test jednostkowy*
- [ ] Niezmiennik AG7 „Sprawa przejęcia”: Jedna decyzja na etap sprawy; druga dostaje „już rozstrzygnięte” (D-008, D-021). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG7 „Sprawa przejęcia”: Przejęcie lub odmowa tylko po weryfikacji aktu zgonu (S2:B3). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG7 „Sprawa przejęcia”: Przejąć może tylko następca z migawki zrobionej przy weryfikacji (D-020). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG7 „Sprawa przejęcia”: Następca ma 30 dni od potwierdzenia zgonu; po terminie skutek jak odmowa (HS-021). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG7 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: optymistyczna wersja sprawy. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
