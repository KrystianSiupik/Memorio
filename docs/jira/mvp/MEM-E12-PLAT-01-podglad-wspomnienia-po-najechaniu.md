# MEM-E12-PLAT-01: Podgląd wspomnienia po najechaniu na punkt mapy

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E12](epiki/E12-ui-mapy.md) UI mapy |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, Web, MEM-E12-PLAT-01 |
| Moduł | archiwum, - |
| Warstwa | API + UI |
| Rodzaj | zwykły |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | P1 |
| Estymacja | 3–5 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | MEM-E2-ARCHIWUM-08 (punkty mapy) |
| Blokuje | - |
| Kod roboczy | MEM-E12-PLAT-01 |
| Źródło | prośba PO z 2026-11-06 (pokaz aplikacji w przeglądarce) |

## Cel

Po najechaniu myszą na punkt wspomnienia widać, co to za wspomnienie, bez wchodzenia w szczegóły.

## Zakres

- Zapytanie „podgląd wspomnienia”: miejsce, data w dokładności wspomnienia (z godziną), pierwsze dodane zdjęcie (adres podpisany na 5 minut), wycinek opisu do 140 znaków ucięty na granicy słowa
- Ten sam filtr dostępu co szczegóły (ADR 0011): cudze wspomnienie tylko z albumu, w którym osoba jest, albo od kontaktu spadkowego; ukryte przez moderację tylko dla autora
- Na mapie: najechanie na pojedynczy punkt pokazuje kartę obok kursora, zjechanie ją chowa; kliknięcie dalej otwiera S05

## Poza zakresem

- urządzenia dotykowe nie mają najechania: stuknięcie dalej otwiera S05
- miniatury zdjęć (pokazywany jest oryginał)

## Kryteria akceptacji

- [ ] Given wspomnienie, które osoba widzi, when pyta o podgląd, then dostaje miejsce, datę, pierwsze zdjęcie i wycinek opisu. *Sprawdzenie: test integracyjny na prawdziwej bazie*
- [ ] Cudze wspomnienie spoza albumów osoby i ukryte przez moderację dają ten sam brak dostępu co szczegóły. *Sprawdzenie: test integracyjny*
- [ ] Najechanie na punkt pokazuje kartę z tymi danymi, zjechanie ją chowa, a stan ładowania i błędu nie zasłania mapy. *Sprawdzenie: test komponentu*

## Ryzyka i pytania

- zdjęcie w podglądzie to pełny oryginał: przy wolnym łączu karta może czekać na obraz; miniatury to osobna decyzja
