# MEM-E12-PLAT-02: Menu na mapie: dodaj wspomnienie w tym miejscu

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E12](epiki/E12-ui-mapy.md) UI mapy |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, Web, MEM-E12-PLAT-02 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | P1 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | MEM-E2-PLAT-02 (S04) |
| Blokuje | - |
| Kod roboczy | MEM-E12-PLAT-02 |
| Źródło | prośba PO z 2026-11-06 (pokaz aplikacji w przeglądarce) |

## Cel

Wspomnienie dodaje się tam, gdzie się kliknęło na mapie, zamiast wpisywać współrzędne.

## Zakres

- Prawy przycisk myszy na mapie (w przeglądarce) i długie przytrzymanie (na urządzeniu) otwierają proste menu z pozycją „Dodaj wspomnienie tutaj”
- Wybór otwiera S04 z miejscem własnym: współrzędne z kliknięcia wpisane, nazwę osoba podaje sama (np. „Warszawa”); wyszukiwanie miejsca po nazwie zostaje jak było

## Poza zakresem

- nazywanie miejsca z kliknięcia (geokodowanie odwrotne)

## Kryteria akceptacji

- [ ] Prawy przycisk na mapie otwiera menu w miejscu kliknięcia, bez menu przeglądarki. *Sprawdzenie: test komponentu i ręcznie w przeglądarce*
- [ ] „Dodaj wspomnienie tutaj” otwiera S04 ze współrzędnymi klikniętego punktu, a zapis daje miejsce własne w tym punkcie. *Sprawdzenie: test komponentu*
- [ ] Współrzędne spoza zakresu albo z niepoprawnego adresu są pomijane: S04 otwiera się pusty. *Sprawdzenie: test komponentu*

## Ryzyka i pytania

- brak
