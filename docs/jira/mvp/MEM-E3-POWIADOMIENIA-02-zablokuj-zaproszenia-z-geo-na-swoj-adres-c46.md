# MEM-E3-POWIADOMIENIA-02: Zablokuj zaproszenia z GEO na swój adres (C46)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, powiadomienia, MEM-E3-POWIADOMIENIA-02 |
| Moduł | powiadomienia |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-POWIADOMIENIA-01](MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md) |
| Blokuje | [MEM-E3-POWIADOMIENIA-06](MEM-E3-POWIADOMIENIA-06-read-port-powiadomienia-dla-album-czy-adres-zablok.md); [MEM-E3-PLAT-06](MEM-E3-PLAT-06-ekran-s14-zaproszenie-do-albumu.md) |
| Kod roboczy | MEM-E3-POWIADOMIENIA-02 |
| Źródło | ES:C46 — S2:C10 „link »nie chcę zaproszeń z GEO«” |

## Cel

Zablokuj zaproszenia z GEO na swój adres (C46) — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **powiadomienia** — kluczowe pytanie: Czy wiadomość dotarła i do kogo nie wolno pisać?
- Czego moduł nie wie (nie wolno tego wymagać): sens biznesowy wiadomości poza kategorią
- Projekt modułu: model procesów modułu powiadomienia
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zablokuj zaproszenia z GEO na swój adres (C46) (źródło: ES:C46 — S2:C10 „link »nie chcę zaproszeń z GEO«”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-POWIADOMIENIA-06, MEM-E3-PLAT-06)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Osoba zaproszona wykonuje „Zablokuj zaproszenia z GEO na swój adres”, then powstaje „Adres zablokowany dla zaproszeń” (E49). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
