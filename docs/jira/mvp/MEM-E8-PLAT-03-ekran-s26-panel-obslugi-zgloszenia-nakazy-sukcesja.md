# MEM-E8-PLAT-03: Ekran S26 „Panel obsługi: zgłoszenia, nakazy, sukcesja”

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E8](epiki/E8-zgloszenia-moderacja-i-nakazy-organow.md) Zgłoszenia, moderacja i nakazy organów |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Panel obsługi (web), -, MEM-E8-PLAT-03 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 4–6 h (wyjątek: ekran złożony (5 akcji); dzielenie rozbiłoby jeden stan ekranu na dwa tickety bez osobnego rezultatu (D-072)) |
| Pewność | niska |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md); [MEM-E8-MODERACJA-02](MEM-E8-MODERACJA-02-rozstrzygnij-zgloszenie-c111.md); [MEM-E8-MODERACJA-04](MEM-E8-MODERACJA-04-ukryj-tresc-natychmiast-c113.md); [MEM-E8-MODERACJA-05](MEM-E8-MODERACJA-05-zabezpiecz-dane-konta-c114.md); [MEM-E8-MODERACJA-06](MEM-E8-MODERACJA-06-wydaj-dane-organowi-c115.md); [MEM-E9-SUKCESJA-03](MEM-E9-SUKCESJA-03-zweryfikuj-akt-zgonu-c121.md); [MEM-E8-MODERACJA-07](MEM-E8-MODERACJA-07-zapytanie-kolejka-zgloszen-r9.md) |
| Blokuje | - |
| Kod roboczy | MEM-E8-PLAT-03 |
| Źródło | specyfikacja ekranu S26; testy przejść S26 |

## Cel

Ekran S26 „Panel obsługi: zgłoszenia, nakazy, sukcesja” — część epiku E8 „Zgłoszenia, moderacja i nakazy organów”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ekran S26 „Panel obsługi: zgłoszenia, nakazy, sukcesja” (źródło: specyfikacja ekranu S26; testy przejść S26)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Ekran pokazuje pola: Kolejka zgłoszeń, Dziennik wglądów. *Sprawdzenie: test komponentu*
- [ ] Akcje: „Rozstrzygnij” (dostępna gdy: zgłoszenie otwarte); „Ukryj natychmiast (nakaz)”; „Zabezpiecz dane konta”; „Wydaj dane organowi” (dostępna gdy: nakaz zweryfikowany); „Zweryfikuj akt zgonu”. *Sprawdzenie: test komponentu*
- [ ] Stany: loading — szkielet ekranu, akcje nieaktywne; error — komunikat zależny od rodzaju błędu (brak sieci / brak dostępu / konflikt „już rozstrzygnięte” / błąd serwera) + Ponów; offline — n/d: panel web, wymaga sieci; empty — „Kolejka pusta”; forbidden — tylko rola obsługi; każdy wgląd w treść zapisany w dzienniku (S2:C8). *Sprawdzenie: test komponentu każdego stanu*
- [ ] Każde przejście z S26 z testów przejść ekranów przechodzi. *Sprawdzenie: test nawigacji (model-based)*
- [ ] Odrzucenie wynikające z rywalizacji (np. „już rozstrzygnięte”, „pula pełna”) pokazane jako komunikat, bez utraty wpisanych danych. *Sprawdzenie: test komponentu z odpowiedzią 409*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
