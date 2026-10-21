# MEM-PROP-01: Ewaluacja jakości podsumowań AI (czy AI nie zmyśla)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E6](../epiki/E6-podsumuj-podsumowania-ai-bez-zmyslania-z-kaskada.md) Podsumuj: podsumowania AI bez zmyślania, z kaskadą |
| Wersja | **propozycja** (poza planem MVP, do decyzji: D-081) |
| Typ | Task |
| Etykiety | Backend, narracja-ai, AI, MEM-PROP-01 |
| Moduł | narracja-ai |
| Warstwa | infrastruktura (testy) + CI |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | do ustalenia |
| Estymacja | 6–10 h (do podziału na 3 tickety po 2–4 h, D-072) |
| Pewność | średnia |
| Status | propozycja |
| Zależy od | [MEM-E0-PLAT-06](../MEM-E0-PLAT-06-adapter-ai-pseudonimizacja-znacznik-ai-wersja-prom.md); [MEM-E6-NARRACJA-AI-03](../MEM-E6-NARRACJA-AI-03-zamow-podsumowanie-okresu-c82.md) |
| Źródło | propozycja użytkownika 2026-10-21; cel G3 „AI nie zmyśla”; QS8 |

## Cel

Obietnica epiku E6 („bez zmyślania”) jest dziś sprawdzana tylko strukturalnie: lista źródeł, znacznik AI,
brak nazw w ładunku. Nikt nie sprawdza, **czy treść podsumowania wynika ze źródeł**. Ewaluacja daje mierzalną
odpowiedź przy każdej zmianie promptu albo modelu.

## Zakres

1. **Zestaw wzorcowy (golden set):** 20–30 syntetycznych okresów (wspomnienia, opisy, miejsca, daty, uczestnicy),
   w tym przypadki trudne: sprzeczne opisy, brak materiałów, materiały pominiętych uczestników, bardzo dużo zdjęć.
   Wyłącznie dane syntetyczne, nigdy dane użytkowników.
2. **Uprząż ewaluacji:** uruchamia prawdziwy adapter AI (z pseudonimizacją, PLAT-06) na zestawie i zapisuje wyniki
   razem z wersją promptu i modelu.
3. **Miary:**
   - *wierność źródłom* (faithfulness): każde twierdzenie podsumowania ma pokrycie w materiałach; ocenia model-sędzia
     z instrukcją „wskaż źródło albo oznacz jako niepoparte”, uzupełniony o sprawdzenia deterministyczne
     (daty, miejsca i liczby z tekstu muszą występować w źródłach);
   - *pominięci uczestnicy*: treść osób bez zgody nie pojawia się w tekście (deterministycznie);
   - *odmowa*: przy braku materiałów model nie dopisuje treści;
   - *zgodność listy źródeł*: źródła podane przez model istnieją w materiałach wejściowych.
4. **Bramka:** wynik poniżej progu (propozycja: wierność ≥ 95%, 0 naruszeń pominiętych uczestników) blokuje
   zmianę wersji promptu/modelu.
5. **Kiedy uruchamiać:** przy zmianie promptu albo modelu oraz raz w nocy, nie przy każdym PR (koszt wywołań).

## Poza zakresem

- ewaluacja na danych produkcyjnych (wymagałaby osobnej podstawy prawnej i DPIA);
- ocena stylu i „ładności” tekstu.

## Kryteria akceptacji

- [ ] Zestaw wzorcowy w repo, wyłącznie dane syntetyczne, z opisem każdego przypadku. *Sprawdzenie: przegląd*
- [ ] Raport z miarami per wersja promptu i modelu. *Sprawdzenie: uruchomienie uprzęży*
- [ ] Zmiana promptu z wynikiem poniżej progu nie przechodzi bramki. *Sprawdzenie: test na celowo zepsutym prompcie*
- [ ] Model-sędzia u dostawcy w UE, ten sam reżim co adapter (D-033). *Sprawdzenie: konfiguracja*

## Ryzyka i pytania

- Model-sędzia też się myli: dlatego miary deterministyczne mają pierwszeństwo, a sędzia ocenia tylko to, czego
  nie da się sprawdzić regułą. Raz na zmianę promptu warto przejrzeć ręcznie próbkę ocen.
- Koszt: około (liczba przypadków × 2 wywołania) na przebieg; próg i częstotliwość do ustalenia.
