# MEM-E13-PLAT-05: Wygląd punktów na mapie: okręgi ze zdjęciem i wielkość

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E13](epiki/E13-wyglad-i-ustawienia.md) Wygląd i ustawienia |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, Web, MEM-E13-PLAT-05 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | P1 |
| Estymacja | 4–6 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | - |
| Blokuje | - |
| Kod roboczy | MEM-E13-PLAT-05 |
| Źródło | prośba PO z 2026-11-07: aplikacja ma być bardzo przyjazna |

## Cel

Osoba może zamiast kropek widzieć na mapie okręgi z pierwszym zdjęciem wspomnienia i powiększyć punkty.

## Zakres

- W Ustawieniach przełącznik z opisem i obrazkiem „kropka → okrąg ze zdjęciem”, domyślnie wyłączony
- Suwak wielkości punktów z podglądem na żywo
- Okrąg trochę większy od kropki, ze zdjęciem w środku; wspomnienie bez zdjęcia zostaje kropką; skupiska bez zmian

## Poza zakresem

- zdjęcia w skupiskach

## Kryteria akceptacji

- [ ] Ustawienia pokazują podgląd kropki i okręgu, a przełącznik i suwak zostają po ponownym uruchomieniu. *Sprawdzenie: test widżetu*
- [ ] Domyślnie mapa pokazuje kropki w obecnej wielkości. *Sprawdzenie: test widżetu*
