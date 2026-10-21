# MEM-E1-KONTO-05: Cofnij usunięcie konta (C5)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E1](epiki/E1-konto-rejestracja-16-usuniecie-z-oknem-30-dni.md) Konto: rejestracja 16+, usunięcie z oknem 30 dni |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, konto, MEM-E1-KONTO-05 |
| Moduł | konto |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-KONTO-01](MEM-E0-KONTO-01-szkielet-modulu-konto-struktura-publiczny-kontrakt.md); [MEM-E1-KONTO-04](MEM-E1-KONTO-04-usun-konto-definitywnie-c4.md) |
| Blokuje | [MEM-E1-PLAT-02](MEM-E1-PLAT-02-ekran-s24-konto-i-prywatnosc.md) |
| Kod roboczy | MEM-E1-KONTO-05 |
| Źródło | ES:C5 — okno 30 dni z B3 ma sens tylko, jeśli można się wycofać przed jego końcem |

## Cel

Cofnij usunięcie konta (C5) — część epiku E1 „Konto: rejestracja 16+, usunięcie z oknem 30 dni”.

## Kontekst

- Moduł **konto** — kluczowe pytanie: Czy ta osoba jest aktywnym użytkownikiem i czy jej konto ma zostać?
- Czego moduł nie wie (nie wolno tego wymagać): co dokładnie wypadnie przy usunięciu (pyta inne konteksty)
- Projekt modułu: model procesów modułu konto
- Agregat AG11 „Usunięcie konta”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Cofnij usunięcie konta (C5) (źródło: ES:C5 — okno 30 dni z B3 ma sens tylko, jeśli można się wycofać przed jego końcem)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E1-PLAT-02)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Użytkownik (autor wspomnień) wykonuje „Cofnij usunięcie konta”, then powstaje „Usunięcie konta cofnięte” (E8). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Niezmiennik AG11 „Usunięcie konta”: Jeden wynik okna 30 dni: usunięte albo cofnięte — warunkowy zapis (D-024). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG11 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: warunkowy UPDATE stanu. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
