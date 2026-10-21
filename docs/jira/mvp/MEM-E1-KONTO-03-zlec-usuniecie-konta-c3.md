# MEM-E1-KONTO-03: Zleć usunięcie konta (C3)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E1](epiki/E1-konto-rejestracja-16-usuniecie-z-oknem-30-dni.md) Konto: rejestracja 16+, usunięcie z oknem 30 dni |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, konto, MEM-E1-KONTO-03 |
| Moduł | konto |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-KONTO-01](MEM-E0-KONTO-01-szkielet-modulu-konto-struktura-publiczny-kontrakt.md); [MEM-E1-KONTO-06](MEM-E1-KONTO-06-zapytanie-skutki-usuniecia-konta-ile-zdjec-z-ilu-a.md); [MEM-E1-ARCHIWUM-02](MEM-E1-ARCHIWUM-02-read-port-archiwum-dla-konto-ile-tresci-wypadnie-p.md); [MEM-E1-ALBUM-01](MEM-E1-ALBUM-01-read-port-album-dla-konto-z-ilu-albumow-czy-wlasci.md) |
| Blokuje | [MEM-E1-PLAT-02](MEM-E1-PLAT-02-ekran-s24-konto-i-prywatnosc.md) |
| Kod roboczy | MEM-E1-KONTO-03 |
| Źródło | ES:C3 — S2:B3 KOMENDA B; S3:L19 (w aplikacji i przez www) |

## Cel

Zleć usunięcie konta (C3) — część epiku E1 „Konto: rejestracja 16+, usunięcie z oknem 30 dni”.

## Kontekst

- Moduł **konto** — kluczowe pytanie: Czy ta osoba jest aktywnym użytkownikiem i czy jej konto ma zostać?
- Czego moduł nie wie (nie wolno tego wymagać): co dokładnie wypadnie przy usunięciu (pyta inne konteksty)
- Projekt modułu: model procesów modułu konto
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zleć usunięcie konta (C3) (źródło: ES:C3 — S2:B3 KOMENDA B; S3:L19 (w aplikacji i przez www))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E1-PLAT-02)

## Kryteria akceptacji

- [ ] Given aktor widzi: Skutki usunięcia konta (ile zdjęć, z ilu albumów, czyich), when Użytkownik (autor wspomnień) wykonuje „Zleć usunięcie konta”, then powstaje „Usunięcie konta zlecone” (E4). *Sprawdzenie: test jednostkowy handlera*
- [ ] Given warunek odrzucenia, when „Zleć usunięcie konta”, then „Usunięcie konta wstrzymane (album bez następcy)” (E5) i brak zapisu. *Sprawdzenie: test jednostkowy*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- HS-003: Wstrzymanie usunięcia konta właściciela albumu bez następcy (B3) a wymóg sklepów i RODO, by usunięcie konta było dostępne w aplikacji (L19). Decyzja 35 mówi „wybór, nigdy blokada” — ile trwa wybór i co, gdy użytkownik go nie dokona? — Prawnik + PO
