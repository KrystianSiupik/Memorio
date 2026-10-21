# MEM-E1-PLAT-02: Ekran S24 „Konto i prywatność”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E1](epiki/E1-konto-rejestracja-16-usuniecie-z-oknem-30-dni.md) Konto: rejestracja 16+, usunięcie z oknem 30 dni |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E1-PLAT-02 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E1-KONTO-03](MEM-E1-KONTO-03-zlec-usuniecie-konta-c3.md); [MEM-E1-KONTO-05](MEM-E1-KONTO-05-cofnij-usuniecie-konta-c5.md); [MEM-E9-SUKCESJA-01](MEM-E9-SUKCESJA-01-wskaz-kontakt-spadkowy-c124.md); [MEM-E1-KONTO-06](MEM-E1-KONTO-06-zapytanie-skutki-usuniecia-konta-ile-zdjec-z-ilu-a.md) |
| Blokuje | - |
| Kod roboczy | MEM-E1-PLAT-02 |
| Źródło | specyfikacja ekranu S24; testy przejść S24 |

## Cel

Ekran S24 „Konto i prywatność” — część epiku E1 „Konto: rejestracja 16+, usunięcie z oknem 30 dni”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S24 „Konto i prywatność” (źródło: specyfikacja ekranu S24; testy przejść S24)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Skutki usunięcia: ile zdjęć, z ilu albumów, czyich, Kontakt spadkowy. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Usuń konto” (dostępna gdy: potwierdzenie; jeśli jestem właścicielem bez następcy — wybór: wskaż następcę albo zgoda na wygaśnięcie (S6:L19)); „Cofnij usunięcie” (dostępna gdy: okno 30 dni trwa); „Wskaż kontakt spadkowy”. *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — baner „Brak połączenia”; dane z pamięci podręcznej tylko do odczytu, akcje wyłączone; empty — n/d: konto zawsze ma dane; forbidden — n/d: własne konto. *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S24 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*
- [ ] Odrzucenie wynikające z rywalizacji (np. „już rozstrzygnięte”, „pula pełna”) pokazane jako komunikat, bez utraty wpisanych danych. *Sprawdzenie: test komponentu z odpowiedzią 409*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
