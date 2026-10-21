# MEM-E3-POWIADOMIENIA-06: Read port powiadomienia → dla album: czy adres zablokował zaproszenia

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, powiadomienia, MEM-E3-POWIADOMIENIA-06 |
| Moduł | powiadomienia |
| Warstwa | kontrakt |
| Rodzaj | kontrakt |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 1–2 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-POWIADOMIENIA-01](MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md); [MEM-E3-POWIADOMIENIA-02](MEM-E3-POWIADOMIENIA-02-zablokuj-zaproszenia-z-geo-na-swoj-adres-c46.md) |
| Blokuje | [MEM-E3-ALBUM-02](MEM-E3-ALBUM-02-zapros-osobe-do-albumu-c42.md) |
| Kod roboczy | MEM-E3-POWIADOMIENIA-06 |
| Źródło | kontrakty modułów porty: album → powiadomienia (read-port) C42 |

## Cel

Read port powiadomienia → dla album: czy adres zablokował zaproszenia — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **powiadomienia** — kluczowe pytanie: Czy wiadomość dotarła i do kogo nie wolno pisać?
- Czego moduł nie wie (nie wolno tego wymagać): sens biznesowy wiadomości poza kategorią
- Projekt modułu: model procesów modułu powiadomienia
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Read port powiadomienia → dla album: czy adres zablokował zaproszenia (źródło: kontrakty modułów porty: album → powiadomienia (read-port) C42)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-ALBUM-02)

## Kryteria akceptacji

- [ ] Read port powiadomienia → dla album: czy adres zablokował zaproszenia. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
