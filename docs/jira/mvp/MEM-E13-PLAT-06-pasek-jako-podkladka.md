# MEM-E13-PLAT-06: Pasek nawigacji jako podkładka z wystającymi ikonami

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E13](epiki/E13-wyglad-i-ustawienia.md) Wygląd i ustawienia |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, Web, MEM-E13-PLAT-06 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | P1 |
| Estymacja | 2–3 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | MEM-E13-PLAT-03 |
| Blokuje | - |
| Kod roboczy | MEM-E13-PLAT-06 |
| Źródło | prośba PO z 2026-11-07 po pokazie aplikacji |

## Cel

Ikony z podpisami są większe, a sam pasek niższy: wygląda jak podkładka, z której ikony wystają.

## Zakres

- Podkładka niższa niż ikony, zaokrąglona, z obramówką i cieniem (biała / ciemnoszara jak w MEM-E13-PLAT-03)
- Każda ikona w okrągłym znaczku, który wystaje ponad górną krawędź podkładki; podpis pod nim, na podkładce
- Ikony i podpisy większe niż dotąd; wybrana zakładka ma znaczek w kolorze akcentu
- Najechanie jak dotąd: kolor akcentu, lekkie powiększenie i wysunięcie w górę

## Poza zakresem

- zmiana zakładek i ich kolejności

## Kryteria akceptacji

- [ ] Znaczki ikon wystają ponad podkładkę, a podpisy leżą na niej. *Sprawdzenie: test widżetu (położenie)*
- [ ] Ikony i podpisy są większe niż w MEM-E13-PLAT-03 (ikona 28 px). *Sprawdzenie: test widżetu*
- [ ] Wybór, najechanie i tryb ciemny działają jak dotąd. *Sprawdzenie: testy widżetu*
