# MEM-E5-ALBUM-01: Utwórz album z odtworzonego okresu (C71)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E5](epiki/E5-przesledz-odtwarzanie-okresu-na-mapie.md) Prześledź: odtwarzanie okresu na mapie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, album, MEM-E5-ALBUM-01 |
| Moduł | album |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md); [MEM-E5-ARCHIWUM-01](MEM-E5-ARCHIWUM-01-read-port-wspomnienia-okresu-przecinajace-okres-w-.md) |
| Blokuje | [MEM-E5-PLAT-01](MEM-E5-PLAT-01-ekran-s08-przesledz-okres.md) |
| Kod roboczy | MEM-E5-ALBUM-01 |
| Źródło | ES:C71 — S1:§8 |

## Cel

Utwórz album z odtworzonego okresu (C71) — część epiku E5 „Prześledź: odtwarzanie okresu na mapie”.

## Kontekst

- Moduł **album** — kluczowe pytanie: Kto współtworzy ten wspólny zbiór i co do niego wniósł?
- Czego moduł nie wie (nie wolno tego wymagać): jak działa wydawanie mediów i link; pieniądze (tylko limit miejsc); treść wspomnień; dlaczego album stracił właściciela
- Projekt modułu: model procesów modułu album
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Utwórz album z odtworzonego okresu (C71) (źródło: ES:C71 — S1:§8)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E5-PLAT-01)

## Kryteria akceptacji

- [ ] Given aktor widzi: Wspomnienia okresu (przecinające okres, w kolejności użytkownika), when Użytkownik (autor wspomnień) wykonuje „Utwórz album z odtworzonego okresu”, then powstaje „Album utworzony” (E40). *Sprawdzenie: test jednostkowy handlera*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
