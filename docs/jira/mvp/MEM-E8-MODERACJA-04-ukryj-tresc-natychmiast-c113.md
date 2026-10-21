# MEM-E8-MODERACJA-04: Ukryj treść natychmiast (C113)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E8](epiki/E8-zgloszenia-moderacja-i-nakazy-organow.md) Zgłoszenia, moderacja i nakazy organów |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, moderacja, MEM-E8-MODERACJA-04 |
| Moduł | moderacja |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-MODERACJA-01](MEM-E0-MODERACJA-01-szkielet-modulu-moderacja-struktura-publiczny-kont.md); [MEM-E8-DOSTEP-02](MEM-E8-DOSTEP-02-write-port-dostep-dla-moderacja-ukryj-zakres-natyc.md) |
| Blokuje | [MEM-E8-MODERACJA-09](MEM-E8-MODERACJA-09-polityka-p19-zawsze-gdy-przyjdzie-nakaz-usuniecia-.md); [MEM-E8-PLAT-03](MEM-E8-PLAT-03-ekran-s26-panel-obslugi-zgloszenia-nakazy-sukcesja.md) |
| Kod roboczy | MEM-E8-MODERACJA-04 |
| Źródło | ES:C113 — S4:K-L11 „narzędzie do natychmiastowego ukrycia treści” |

## Cel

Ukryj treść natychmiast (C113) — część epiku E8 „Zgłoszenia, moderacja i nakazy organów”.

## Kontekst

- Moduł **moderacja** — kluczowe pytanie: Co z tą treścią robimy po zgłoszeniu lub nakazie, i w jakim terminie?
- Czego moduł nie wie (nie wolno tego wymagać): prywatna treść poza zgłoszonym zakresem; relacje w albumach
- Projekt modułu: model procesów modułu moderacja
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ukryj treść natychmiast (C113) (źródło: ES:C113 — S4:K-L11 „narzędzie do natychmiastowego ukrycia treści”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E8-MODERACJA-09, MEM-E8-PLAT-03)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Obsługa GEO wykonuje „Ukryj treść natychmiast”, then powstaje „Treść ukryta” (E113). *Sprawdzenie: test jednostkowy handlera*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- HS-006: Nakaz usunięcia w 1 h całą dobę przy 3 osobach w zespole — kto ma dyżur i jak dociera alert (L11)? — Zespół
