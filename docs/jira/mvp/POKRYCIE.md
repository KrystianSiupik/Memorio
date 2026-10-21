# Pokrycie źródła i architektury

## Źródło S1 §15 (MVP)
| Punkt | Epik |
|---|---|
| 1. Rejestracja i konto | E1 (+ E11 ekran zgody) |
| 2. Mapa z wyszukiwaniem i oznaczaniem lokalizacji | E2 (Miejsca C10, C11; ekran S03, S04; enabler T5 spike mapy) |
| 3. Wspomnienia ze zdjęciami, datą i opisem | E2 (C12, C13, C15, C16) |
| 4. Prywatna mapa | E2 (R11, S03) |
| 5. Albumy ze wspólnym dostępem i rolami | E3 |
| 6. Udostępnianie i kontrola uprawnień | E4 |
| 7. Prześledź | E5 |
| 8. Podsumuj | E6 (podsumowanie jako wpis w albumie, HS-009) |
| 9. Prywatność i usuwanie wspomnień | E2 (kosz, C17–C20), E1 (usunięcie konta), E11 |

## Źródło S2 (przegląd wymagań)
| Blok | Epik / status |
|---|---|
| B1 Premium albumu, tryb tylko do odczytu, przycięcie | E3, E7 (przycięcie do kosza na 30 dni, HS-022) |
| B2 Cofnięcie dostępu, kaskada | E4, E6 (Pochodzenie) |
| B3 Usunięcie, kosz, następca | E1, E2, E9 (P0; przejęcie przez innego uczestnika D-004, termin 30 dni HS-021) |
| B4 Profil miejsca | **poza zakresem** (PR12, D-070) |
| C1 Data wspomnienia | E2 (C12, E13) |
| C2 Zgłoszenia i moderacja | E8 |
| C3 Sprzedaż i podatek | E7 (sklepy); www poza MVP (D-051) |
| C4 Miejsce jako wpis | E2 (Miejsca, dwie tabele) |
| C5 Dostawca AI | E6 + E0 (T6) |
| C6 Miejsce: precyzja, ukrycie | E2 |
| C7 Plan rodzinny | E7, **P2** (HS-012) |
| C8 Prywatny album, kontakt spadkowy | E9 (C124, C125), E8 (dziennik wglądów) |
| C9 Osoby i wyszukiwanie | E2 (R12: tekst autora albo oznaczony uczestnik, D-079), E3 (oznaczenia C169/C170), E0 (T14 test uprawnień wyszukiwania) |
| C10 Import i powiadomienia | E10 (**P2**), powiadomienia w E3/E7 |
| C11 Wymogi prawne | E11 + kryteria w ticketach (znacznik AI, usuwanie, zgłoszenia) |

**Pokrycie:** 100% punktów S1 §15 i bloków S2 ma epik albo jawne „poza zakresem” z powodem.

## Zmiany po przeglądzie decyzji (2026-10-19)
- Nowe tickety: przejęcie albumu przez uczestnika (C155–C157, C165, P39–P42), przypomnienia dla następcy (C167, P43), przywrócenie przyciętego materiału (C168, P44), oznaczenia (C169–C171, P45), ekrany S10/S05/S14→S01 (rejestracja z zaproszenia), ekran zgód (E11).
- Odblokowane: sukcesja (HS-020, HS-021), przycięcie (HS-022), podsumowanie w albumie (HS-009). Zablokowane zostają tylko dyżur (HS-006) i plan rodzinny (HS-012, P2).
- Nowe połączenia mapy z kontraktami: Plany → Album (E171), Sukcesja → Powiadomienia (E121).

## Architektura
- **Szkielety modułów:** 12 z 12 modułów MVP (MEM-E0-<MODUŁ>-01), z warstwami Onion i testem kierunku zależności (ADR 0019). Profil miejsca i Publikacja poza MVP.
- **Kontrakty:** każde przejście z kontrakty modułów używane przez ticket MVP ma ticket kontraktu u dostawcy:
  - porty synchroniczne: 13 ticketów „Read port / Write port”;
  - zdarzenia: jeden ticket „Zdarzenie integracyjne …” na każde zdarzenie, które przekracza granicę modułu;
  - read modele czytane przez inny moduł: ticket „Read port: …”.
- `plan-graph.mjs` potwierdza, że żadna zależność nie wychodzi poza mapę i żaden konsument nie omija ticketu kontraktu.
- **Enablery architektury T1–T16:** 16 ticketów w E0 (w tym T15 warstwy Onion, T16 księga uprawnień), z fitness functions jako kryteriami. **Gotowość infra RD1–RD10:** RD1/RD2/RD4 i RD10 jako osobne tickety, RD5–RD9 pokryte przez T9, T10, T13.

## Bramka jakości
- Odrzucone: 0 po poprawkach generatora.
- Dwie poprawki w trakcie: 4 ekrany P0 zależały od komend P1/P2, więc akcje P1/P2 wydzielono do ticketów uzupełniających ekran.
- 4 ekrany złożone (S04, S10, S11, S26) mają estymację powyżej 4 h, z uzasadnieniem i pewnością „niska” (D-072).
- Uwaga jakościowa: tickety komend RC mają do 7 kryteriów (niezmienniki agregatu + współbieżność). To więcej niż zalecane 6, ale każdy niezmiennik musi mieć osobny test.
