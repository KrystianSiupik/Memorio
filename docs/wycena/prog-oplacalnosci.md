# Próg opłacalności planów Memorio

> Produkt projektowy. To szacunek do rozmowy o cenach, nie wycena ani prognoza finansowa. Ceny dostawców sprawdzone 2026-10-19; część tylko w źródłach wtórnych (oznaczone). Limity planów to propozycja (D-080), bo źródła ich nie podają (S1 §13: „ustalić po zbadaniu kosztów”).

Liczby policzone w arkuszu kalkulacyjnym na podstawie cen dostawców.

## Założenia

| Parametr | Wartość | Źródło |
|---|---|---|
| Kurs EUR / USD | 4,3702 / 3,8974 zł | NBP, tabela A 203/A/NBP/2026, 2026-10-19 |
| VAT w cenie sklepu | 23% (PL) | cena w sklepie zawiera VAT; prowizja liczona od kwoty netto |
| Prowizja sklepu | 15% | Apple: program małych firm albo subskrypcja po 1. roku (bez programu 1. rok 30%); Google Play: subskrypcje 15% (źródła wtórne) |
| Magazyn zdjęć | 8,70 EUR za TB-miesiąc × 2 kopie × 1.15 (miniatury) = 0,0874 zł za GB-miesiąc | Hetzner Object Storage (źródło wtórne, 2026-07; strona Hetznera nie pokazała kwot) |
| Ruch wychodzący | pomijalny (1 TB w cenie bazowej, potem 1 EUR/TB) | jw. |
| AI: podsumowanie | 40 tys. tokenów wejścia, 2 tys. wyjścia, ×1.5 na ponowienia = 0,134 zł | Mistral Large 3: 0,5 / 1,5 USD za 1 mln tokenów (mistral.ai/pricing, 2026-10-19) |
| AI: szkic opisu | 3 tys. / 300 tokenów ×1.5 = 0,0114 zł | jw. |
| Dane byłych płatników | trzymane 24 mies. w trybie tylko do odczytu (B1) → mnożnik magazynu 1 + odpływ × 24 | przegląd wymagań B1 |
| Infrastruktura etapu 1 | 90–160 EUR/mies. (VPS produkcja, VPS test i staging z monitoringiem, VPS monitoringu produkcji, baza magazynu, kopie, IPv4, e-mail, domena) + Apple Developer 99 USD/rok | ceny VPS po podwyżkach Hetznera z 2026 tylko ze źródeł wtórnych; **do sprawdzenia w konsoli dostawcy** |
| Wytworzenie MVP | 150 000–400 000 zł | szacunek z rozmowy 2026-10-19 |
| Utrzymanie | 17.5% kosztu wytworzenia rocznie | założenie |
| Zwrot wytworzenia | w 36 mies. | założenie |

| Scenariusz | Odpływ płacących / mies. | Konwersja z darmowego | Średnie GB darmowego | Wykorzystanie limitów |
|---|---|---|---|---|
| ostrozny | 6% | 3% | 1,0 | wyższe |
| bazowy | 4% | 5% | 0,7 | bazowe |

## Plany (propozycja limitów D-080)

| Plan | Limit | Średnio zajęte | Podsumowania AI / mies. | Szkice / mies. | Cena testowa brutto |
|---|---|---|---|---|---|
| Darmowy | 2 GB, bez AI | 0,7–1 GB | 0 | 0 | 0 |
| Premium osobiste | 50 GB | 12 GB | 4 | 20 | 14,99 zł/mies. |
| Premium albumu (pula, do 10 osób) | 150 GB | 35 GB | 10 | 60 | 29,99 zł/mies. |
| Plan rodzinny (P2, do 6 kont Premium osobistego) | 300 GB | 43 GB | 12 | 60 | 34,99 zł/mies. |

## 1. Koszt zmienny i cena minimalna (na jednego płacącego, miesięcznie)

Cena minimalna = koszt zmienny przeliczony na cenę brutto w sklepie (÷ 0,85 prowizji, × 1,23 VAT). Poniżej niej każdy płacący dokłada do interesu, zanim policzymy serwery i zespół.

| Plan | Scenariusz | Magazyn | AI | Dopłata do darmowych | Razem | Cena minimalna brutto | Najgorszy przypadek* |
|---|---|---|---|---|---|---|---|
| Premium osobiste | ostrozny | 4,27 | 1,28 | 2,83 | **8,37 zł** | **12,11 zł** | 17,65 zł |
| Premium osobiste | bazowy | 2,06 | 0,77 | 1,16 | **3,99 zł** | **5,77 zł** | 17,65 zł |
| Premium albumu (pula, do 10 osób) | ostrozny | 12,45 | 3,38 | 2,83 | **18,66 zł** | **27,00 zł** | 52,19 zł |
| Premium albumu (pula, do 10 osób) | bazowy | 6,00 | 2,03 | 1,16 | **9,19 zł** | **13,30 zł** | 52,19 zł |
| Plan rodzinny (P2, do 6 kont Premium osobistego) | ostrozny | 15,36 | 3,83 | 2,83 | **22,02 zł** | **31,86 zł** | 99,28 zł |
| Plan rodzinny (P2, do 6 kont Premium osobistego) | bazowy | 7,40 | 2,30 | 1,16 | **10,86 zł** | **15,72 zł** | 99,28 zł |

\* Płacący zapełnia cały limit, zużywa 2× średnią AI i po odejściu jego dane leżą 24 miesiące (odpływ 6%).

## 2. Ilu płacących potrzeba (miesięcznie)

Marża z płacącego = cena netto po VAT i prowizji − koszt zmienny. Liczba płacących = koszty stałe ÷ marża. Mieszanka: 70% Premium osobiste, 30% Premium albumu (plan rodzinny to P2, poza MVP).

| Koszty stałe / mies. | Kwota (zł) |
|---|---|
| A. Tylko infrastruktura | 426–732 |
| B. A + utrzymanie (poprawki, aktualizacje, sklepy) | 2613–6565 |
| C. B + zwrot wytworzenia w 36 mies. | 6780–17 676 |

| Scenariusz | Marża z płacącego | A: płacący | B: płacący | C: płacący | C: konta łącznie (z darmowymi) |
|---|---|---|---|---|---|
| ostrozny | 2,01 zł | 212–364 | 1299–3263 | 3370–8785 | 112 309–292 809 |
| bazowy | 7,92 zł | 54–93 | 330–829 | 856–2232 | 17 117–44 628 |

Dla porównania: architektura zakłada do 5 tys. aktywnych użytkowników w MVP. Przy konwersji 3–5% to 150–250 płacących.

## 3. Wrażliwość: co najbardziej zmienia wynik

| Zmiana (scenariusz bazowy) | Marża z płacącego | Zmiana |
|---|---|---|
| bez zmian | 7,92 zł | – |
| Konwersja 2% zamiast 5% | 6,09 zł | -23% |
| Odpływ 10%/mies. zamiast 4% | 5,54 zł | -30% |
| Wykorzystanie limitów jak w scenariuszu ostrożnym | 5,00 zł | -37% |
| Prowizja 30% (Apple 1. rok bez programu małych firm) | 5,54 zł | -30% |
| Droższy model AI (Mistral Medium 3.5: 1,5 / 7,5 USD, źródło wtórne) | 4,72 zł | -40% |

## Wnioski

1. **Premium osobiste za 14,99 zł ma zapas w obu scenariuszach** (cena minimalna 5,77–12,11 zł). **Premium albumu za 29,99 zł ledwo pokrywa koszt zmienny w scenariuszu ostrożnym** (27,00 zł), a w najgorszym przypadku (pełne 150 GB) dokłada. Album wymaga ceny ok. 35–40 zł albo mniejszej puli (np. 100 GB).
2. **O opłacalności decydują koszty stałe, nie serwery.** Infrastrukturę pokrywa 54–364 płacących; z utrzymaniem 330–3 263; ze zwrotem wytworzenia w 3 lata 856–8 785. MVP z 5 tys. aktywnych (150–250 płacących) pokrywa infrastrukturę, ale nie utrzymanie.
3. **Obietnica 24 miesięcy (B1) mniej więcej podwaja koszt magazynu** (dane byłych płacących przy odpływie 4–6%). To wyróżnik produktu, więc zostaje, ale z metryką kosztu z QS11 od pierwszego dnia.
4. **Największe dźwignie marży:** wybór modelu AI (−40% przy droższym), rzeczywiste zapełnienie limitów (−37%), odpływ i prowizja 30% (po −30%). Program małych firm Apple od pierwszego dnia; tańszy model do podsumowań, droższy tylko po teście jakości.
5. **To dół widełek, nie cennik.** Ceny i rabat roczny ustala się w testach z użytkownikami (S1 §13).

## Źródła

- Kurs: https://api.nbp.pl/api/exchangerates/tables/a/ (2026-10-19)
- Mistral: https://mistral.ai/pricing (2026-10-19); Medium 3.5 ze źródła wtórnego: https://anotherwrapper.com/llm-pricing/mistral-medium-3-5
- Hetzner Object Storage (wtórne): https://sliplane.io/blog/hetzner-object-storage-alternatives
- Hetzner Cloud po podwyżkach 2026 (wtórne): https://findstack.com/resources/hetzner-price-increase-2026, https://wz-it.com/en/blog/hetzner-price-increase-june-2026-cpx-ccx-alternatives/
- Prowizje sklepów (wtórne): https://www.revenuecat.com/blog/engineering/small-business-program, https://dev.to/mr_manushukla/app-subscription-billing-in-2026-3-platform-changes-that-decide-what-your-app-keeps-54l8
