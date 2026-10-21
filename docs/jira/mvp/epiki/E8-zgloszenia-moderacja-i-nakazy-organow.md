# E8: Zgłoszenia, moderacja i nakazy organów

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony |
| Cel planu | G4 |
| Priorytet | P0 |
| Moduły | moderacja, dostep, - |
| Zależy od epików | E0 |
| Estymacja | 28–61 h |
| Źródło | S2:C2, C8; legal L10, L11, L18, L19 |

## Kryteria ukończenia epiku

- [ ] Wszystkie tickety P0 epiku zamknięte, a testy przejść jego ekranów zielone. *Sprawdzenie: CI + test na urządzeniu*

## Tickety

| Kod | Tytuł | Moduł / warstwa | h | Status |
|---|---|---|---|---|
| [MEM-E8-MODERACJA-01](../MEM-E8-MODERACJA-01-zglos-tresc-c110.md) | Zgłoś treść (C110) | moderacja / aplikacja | 1–3 | gotowy |
| [MEM-E8-MODERACJA-02](../MEM-E8-MODERACJA-02-rozstrzygnij-zgloszenie-c111.md) | Rozstrzygnij zgłoszenie (C111) | moderacja / domena + aplikacja | 3–4 | gotowy |
| [MEM-E8-MODERACJA-03](../MEM-E8-MODERACJA-03-odwolaj-sie-od-decyzji-c112.md) | Odwołaj się od decyzji (C112) | moderacja / aplikacja | 1–3 | gotowy |
| [MEM-E8-MODERACJA-04](../MEM-E8-MODERACJA-04-ukryj-tresc-natychmiast-c113.md) | Ukryj treść natychmiast (C113) | moderacja / aplikacja | 2–4 | gotowy |
| [MEM-E8-MODERACJA-05](../MEM-E8-MODERACJA-05-zabezpiecz-dane-konta-c114.md) | Zabezpiecz dane konta (C114) | moderacja / aplikacja | 1–3 | gotowy |
| [MEM-E8-MODERACJA-06](../MEM-E8-MODERACJA-06-wydaj-dane-organowi-c115.md) | Wydaj dane organowi (C115) | moderacja / aplikacja | 2–4 | gotowy |
| [MEM-E8-DOSTEP-01](../MEM-E8-DOSTEP-01-ukryj-zakres-c67.md) | Ukryj zakres (C67) | dostep / aplikacja | 1–3 | gotowy |
| [MEM-E8-MODERACJA-07](../MEM-E8-MODERACJA-07-zapytanie-kolejka-zgloszen-r9.md) | Zapytanie: Kolejka zgłoszeń (R9) | moderacja / infrastruktura (query handler) | 1–3 | gotowy |
| [MEM-E8-MODERACJA-08](../MEM-E8-MODERACJA-08-zapytanie-moje-zgloszenia-i-decyzje-moderacji-z-uz.md) | Zapytanie: Moje zgłoszenia i decyzje moderacji (z uzasadnieniem) (R18) | moderacja / infrastruktura (query handler) | 1–3 | gotowy |
| [MEM-E8-DOSTEP-02](../MEM-E8-DOSTEP-02-write-port-dostep-dla-moderacja-ukryj-zakres-natyc.md) | Write port dostep → dla moderacja: ukryj zakres natychmiast i potwierdź (nakaz 1 h) | dostep / kontrakt | 1–2 | gotowy |
| [MEM-E8-MODERACJA-09](../MEM-E8-MODERACJA-09-polityka-p19-zawsze-gdy-przyjdzie-nakaz-usuniecia-.md) | Polityka P19: Zawsze gdy przyjdzie nakaz usunięcia, wtedy natychmiast ukryj treść | moderacja / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E8-MODERACJA-10](../MEM-E8-MODERACJA-10-polityka-p20-zawsze-gdy-przyjdzie-nakaz-wydania-lu.md) | Polityka P20: Zawsze gdy przyjdzie nakaz wydania lub zabezpieczenia danych, wtedy zabezpiecz dane konta | moderacja / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E8-MODERACJA-11](../MEM-E8-MODERACJA-11-zdarzenie-integracyjne-tresc-ukryta-e113-w-kontrak.md) | Zdarzenie integracyjne „Treść ukryta” (E113) w kontrakcie modułu moderacja | moderacja / kontrakt | 1–2 | gotowy |
| [MEM-E8-DOSTEP-03](../MEM-E8-DOSTEP-03-polityka-p32-zawsze-gdy-tresc-ukryta-decyzja-moder.md) | Polityka P32: Zawsze gdy treść ukryta decyzją moderacji, wtedy ukryj zakres w Dostępie | dostep / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E8-PLAT-01](../MEM-E8-PLAT-01-ekran-s21-zglos-tresc.md) | Ekran S21 „Zgłoś treść” | - / UI | 2–4 | gotowy |
| [MEM-E8-PLAT-02](../MEM-E8-PLAT-02-ekran-s22-moje-zgloszenia-i-decyzje.md) | Ekran S22 „Moje zgłoszenia i decyzje” | - / UI | 2–4 | gotowy |
| [MEM-E8-PLAT-03](../MEM-E8-PLAT-03-ekran-s26-panel-obslugi-zgloszenia-nakazy-sukcesja.md) | Ekran S26 „Panel obsługi: zgłoszenia, nakazy, sukcesja” | - / UI | 4–6 | gotowy |
| [MEM-E8-PLAT-04](../MEM-E8-PLAT-04-ekran-s27-zglos-tresc-z-aplikacji.md) | Ekran S27 „Zgłoś treść (z aplikacji)” | - / UI | 2–4 | gotowy |
