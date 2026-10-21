# MEM-E9-SUKCESJA-04: Przekaż album następcy (C122)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, sukcesja, MEM-E9-SUKCESJA-04 |
| Moduł | sukcesja |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-SUKCESJA-01](MEM-E0-SUKCESJA-01-szkielet-modulu-sukcesja-struktura-publiczny-kontr.md); [MEM-E9-SUKCESJA-03](MEM-E9-SUKCESJA-03-zweryfikuj-akt-zgonu-c121.md); [MEM-E9-SUKCESJA-08](MEM-E9-SUKCESJA-08-zapytanie-sprawa-przejecia-albumu-album-termin-los.md); [MEM-E9-ALBUM-06](MEM-E9-ALBUM-06-write-port-album-dla-sukcesja-zmien-wlasciciela-zw.md) |
| Blokuje | [MEM-E9-SUKCESJA-09](MEM-E9-SUKCESJA-09-polityka-p21-zawsze-gdy-zgon-potwierdzony-wtedy-pr.md); [MEM-E9-PLAT-01](MEM-E9-PLAT-01-ekran-s23-przejecie-albumu-nastepca.md) |
| Kod roboczy | MEM-E9-SUKCESJA-04 |
| Źródło | ES:C122 — S2:B3 „Album, jego pula i rola właściciela przechodzą na następcę” |

## Cel

Przekaż album następcy (C122) — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Moduł **sukcesja** — kluczowe pytanie: Kto przejmuje album lub odczyt, gdy właściciela już nie ma?
- Czego moduł nie wie (nie wolno tego wymagać): budowa albumu; mechanika dostępu
- Projekt modułu: model procesów modułu sukcesja
- Agregat AG7 „Sprawa przejęcia”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Przekaż album następcy (C122) (źródło: ES:C122 — S2:B3 „Album, jego pula i rola właściciela przechodzą na następcę”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E9-SUKCESJA-09, MEM-E9-PLAT-01)

## Kryteria akceptacji

- [ ] Given aktor widzi: Sprawa przejęcia albumu (album, termin, losy treści fundatora), when Następca albumu / kontakt spadkowy wykonuje „Przekaż album następcy”, then powstaje „Album przekazany następcy” (E123). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Niezmiennik AG7 „Sprawa przejęcia”: Jedna decyzja na etap sprawy; druga dostaje „już rozstrzygnięte” (D-008, D-021). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG7 „Sprawa przejęcia”: Przejęcie lub odmowa tylko po weryfikacji aktu zgonu (S2:B3). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG7 „Sprawa przejęcia”: Przejąć może tylko następca z migawki zrobionej przy weryfikacji (D-020). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG7 „Sprawa przejęcia”: Następca ma 30 dni od potwierdzenia zgonu; po terminie skutek jak odmowa (HS-021). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG7 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: optymistyczna wersja sprawy. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
