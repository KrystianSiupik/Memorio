# MEM-E3-ALBUM-05: Dodaj wspomnienie do albumu (C47)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E3-ALBUM-05 |
| Moduł | album |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md); [MEM-E3-LIMITY-02](MEM-E3-LIMITY-02-read-port-stan-puli-albumu-r6.md) |
| Blokuje | [MEM-E3-ALBUM-21](MEM-E3-ALBUM-21-zdarzenie-integracyjne-pula-albumu-wyczerpana-e51-.md); [MEM-E3-PLAT-02](MEM-E3-PLAT-02-ekran-s10-album.md) |
| Kod roboczy | MEM-E3-ALBUM-05 |
| Źródło | ES:C47 — S2:B1 KOMENDA B „Uczestnik przed operacją widzi, ile zostało w puli” |

## Cel

Dodaj wspomnienie do albumu (C47) — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Dodaj wspomnienie do albumu (C47) (źródło: ES:C47 — S2:B1 KOMENDA B „Uczestnik przed operacją widzi, ile zostało w puli”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E3-ALBUM-21, MEM-E3-PLAT-02)

## Kryteria akceptacji

- [ ] Given aktor widzi: Stan puli albumu, when Uczestnik albumu wykonuje „Dodaj wspomnienie do albumu”, then powstaje „Wspomnienie dodane do albumu” (E50). *Sprawdzenie: test jednostkowy handlera*
- [ ] Given warunek odrzucenia, when „Dodaj wspomnienie do albumu”, then „Pula albumu wyczerpana” (E51) i brak zapisu. *Sprawdzenie: test jednostkowy*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
