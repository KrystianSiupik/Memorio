# MEM-E0-PLAT-06: Adapter AI: pseudonimizacja, znacznik AI, wersja promptu, test ładunku

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Platforma, -, MEM-E0-PLAT-06 |
| Moduł | - |
| Warstwa | infrastruktura |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md) |
| Blokuje | - |
| Kod roboczy | MEM-E0-PLAT-06 |
| Źródło | projekt systemu T6; SD9, QS8 |

## Cel

Adapter AI: pseudonimizacja, znacznik AI, wersja promptu, test ładunku — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Adapter AI: pseudonimizacja, znacznik AI, wersja promptu, test ładunku (źródło: projekt systemu T6; SD9, QS8)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Adapter AI: pseudonimizacja, znacznik AI, wersja promptu, test ładunku. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*
- [ ] Fitness function SD9: ładunek do dostawcy nie zawiera nazw z listy osób/miejsc. *Sprawdzenie: CI — test jednostkowy adaptera na danych syntetycznych*
- [ ] Fitness function SD9: każdy zapisany tekst AI ma znacznik i wersję promptu. *Sprawdzenie: CI — test aggregate/handlera*
- [ ] Scenariusz QS8: 0 nazw z listy osób/miejsc w ładunku (test automatyczny); 100% tekstów AI ze znacznikiem maszynowym; dostawca w UE. *Sprawdzenie: test wydajności / monitoring*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
