# E9: Sukcesja i przejęcie albumu (następca albo inny uczestnik)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony |
| Cel planu | G1, G2 |
| Priorytet | P0 |
| Moduły | sukcesja, album, powiadomienia, - |
| Zależy od epików | E3 |
| Estymacja | 49–97 h |
| Źródło | S2:B3, C8 |

## Kryteria ukończenia epiku

- [ ] Wszystkie tickety P0 epiku zamknięte, a testy przejść jego ekranów zielone. *Sprawdzenie: CI + test na urządzeniu*

## Tickety

| Kod | Tytuł | Moduł / warstwa | h | Status |
|---|---|---|---|---|
| [MEM-E9-SUKCESJA-01](../MEM-E9-SUKCESJA-01-wskaz-kontakt-spadkowy-c124.md) | Wskaż kontakt spadkowy (C124) | sukcesja / aplikacja | 1–3 | gotowy |
| [MEM-E9-SUKCESJA-02](../MEM-E9-SUKCESJA-02-zglos-zgon-wlasciciela-c120.md) | Zgłoś zgon właściciela (C120) | sukcesja / aplikacja | 1–3 | gotowy |
| [MEM-E9-SUKCESJA-03](../MEM-E9-SUKCESJA-03-zweryfikuj-akt-zgonu-c121.md) | Zweryfikuj akt zgonu (C121) | sukcesja / domena + aplikacja | 3–4 | gotowy |
| [MEM-E9-SUKCESJA-04](../MEM-E9-SUKCESJA-04-przekaz-album-nastepcy-c122.md) | Przekaż album następcy (C122) | sukcesja / domena + aplikacja | 3–4 | gotowy |
| [MEM-E9-SUKCESJA-05](../MEM-E9-SUKCESJA-05-odmow-przejecia-albumu-c123.md) | Odmów przejęcia albumu (C123) | sukcesja / domena + aplikacja | 3–4 | gotowy |
| [MEM-E9-SUKCESJA-06](../MEM-E9-SUKCESJA-06-udostepnij-prywatny-album-kontaktowi-spadkowemu-c1.md) | Udostępnij prywatny album kontaktowi spadkowemu (C125) | sukcesja / aplikacja | 1–3 | gotowy |
| [MEM-E9-ALBUM-01](../MEM-E9-ALBUM-01-zwolnij-role-wlasciciela-albumu-c153.md) | Zwolnij rolę właściciela albumu (C153) | album / domena + aplikacja | 2–4 | gotowy |
| [MEM-E9-SUKCESJA-07](../MEM-E9-SUKCESJA-07-uznaj-brak-odpowiedzi-nastepcy-c126.md) | Uznaj brak odpowiedzi następcy (C126) | sukcesja / domena + aplikacja | 3–4 | gotowy |
| [MEM-E9-ALBUM-02](../MEM-E9-ALBUM-02-zarezerwuj-przejecie-albumu-bez-wlasciciela-c155.md) | Zarezerwuj przejęcie albumu bez właściciela (C155) | album / domena + aplikacja | 3–4 | gotowy |
| [MEM-E9-ALBUM-03](../MEM-E9-ALBUM-03-przejmij-album-bez-wlasciciela-c156.md) | Przejmij album bez właściciela (C156) | album / domena + aplikacja | 3–4 | gotowy |
| [MEM-E9-ALBUM-04](../MEM-E9-ALBUM-04-wygas-rezerwacje-przejecia-c157.md) | Wygaś rezerwację przejęcia (C157) | album / domena + aplikacja | 3–4 | gotowy |
| [MEM-E9-POWIADOMIENIA-01](../MEM-E9-POWIADOMIENIA-01-powiadom-uczestnikow-o-nowym-wlascicielu-c166.md) | Powiadom uczestników o nowym właścicielu (C166) | powiadomienia / aplikacja | 2–4 | gotowy |
| [MEM-E9-POWIADOMIENIA-02](../MEM-E9-POWIADOMIENIA-02-przypomnij-nastepcy-o-decyzji-7-i-21-dzien-c167.md) | Przypomnij następcy o decyzji (7. i 21. dzień) (C167) | powiadomienia / aplikacja | 2–4 | gotowy |
| [MEM-E9-SUKCESJA-08](../MEM-E9-SUKCESJA-08-zapytanie-sprawa-przejecia-albumu-album-termin-los.md) | Zapytanie: Sprawa przejęcia albumu (album, termin, losy treści fundatora) (R17) | sukcesja / infrastruktura (query handler) | 1–3 | gotowy |
| [MEM-E9-ALBUM-05](../MEM-E9-ALBUM-05-zapytanie-album-bez-wlasciciela-stan-przejecia-czy.md) | Zapytanie: Album bez właściciela: stan przejęcia (czy można przejąć, trwająca rezerwacja i do kiedy) (R20) | album / infrastruktura (query handler) | 1–3 | gotowy |
| [MEM-E9-ALBUM-06](../MEM-E9-ALBUM-06-write-port-album-dla-sukcesja-zmien-wlasciciela-zw.md) | Write port album → dla sukcesja: zmień właściciela / zwolnij rolę właściciela (C153) | album / kontrakt | 1–2 | gotowy |
| [MEM-E9-SUKCESJA-09](../MEM-E9-SUKCESJA-09-polityka-p21-zawsze-gdy-zgon-potwierdzony-wtedy-pr.md) | Polityka P21: Zawsze gdy zgon potwierdzony, wtedy przekaż albumy współdzielone następcom | sukcesja / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E9-SUKCESJA-10](../MEM-E9-SUKCESJA-10-polityka-p23-zawsze-gdy-zgon-potwierdzony-wtedy-ud.md) | Polityka P23: Zawsze gdy zgon potwierdzony, wtedy udostępnij albumy prywatne kontaktowi spadkowemu (tylko odczyt) | sukcesja / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E9-SUKCESJA-11](../MEM-E9-SUKCESJA-11-zdarzenie-integracyjne-album-prywatny-udostepniony.md) | Zdarzenie integracyjne „Album prywatny udostępniony kontaktowi spadkowemu” (E126) w kontrakcie modułu sukcesja | sukcesja / kontrakt | 1–2 | gotowy |
| [MEM-E9-ALBUM-07](../MEM-E9-ALBUM-07-zdarzenie-integracyjne-album-bez-wlasciciela-e162-.md) | Zdarzenie integracyjne „Album bez właściciela” (E162) w kontrakcie modułu album | album / kontrakt | 1–2 | gotowy |
| [MEM-E9-ALBUM-08](../MEM-E9-ALBUM-08-polityka-p37-zawsze-gdy-konto-wlasciciela-bez-nast.md) | Polityka P37: Zawsze gdy konto właściciela bez następcy usunięte (ze zgodą na wygaśnięcie albumu), wtedy zwolnij rolę właściciela | album / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E9-SUKCESJA-12](../MEM-E9-SUKCESJA-12-polityka-p38-zawsze-gdy-termin-przejecia-minal-wte.md) | Polityka P38: Zawsze gdy termin przejęcia minął, wtedy uznaj brak odpowiedzi następcy | sukcesja / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E9-ALBUM-09](../MEM-E9-ALBUM-09-polityka-p39-zawsze-gdy-minie-30-minut-rezerwacji-.md) | Polityka P39: Zawsze gdy minie 30 minut rezerwacji, wtedy wygaś rezerwację przejęcia | album / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E9-ALBUM-10](../MEM-E9-ALBUM-10-polityka-p40-zawsze-gdy-plan-albumu-bez-wlasciciel.md) | Polityka P40: Zawsze gdy plan albumu bez właściciela opłacony przez uczestnika, wtedy przejmij album (jeśli to jego aktywna rezerwacja) | album / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E9-ALBUM-11](../MEM-E9-ALBUM-11-zdarzenie-integracyjne-album-przejety-przez-uczest.md) | Zdarzenie integracyjne „Album przejęty przez uczestnika” (E167) w kontrakcie modułu album | album / kontrakt | 1–2 | gotowy |
| [MEM-E9-POWIADOMIENIA-03](../MEM-E9-POWIADOMIENIA-03-polityka-p42-zawsze-gdy-album-przejety-przez-uczes.md) | Polityka P42: Zawsze gdy album przejęty przez uczestnika, wtedy powiadom pozostałych (mogą wyjść z wyborem zostaw/zabierz) | powiadomienia / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E9-SUKCESJA-13](../MEM-E9-SUKCESJA-13-zdarzenie-integracyjne-zgon-potwierdzony-e121-w-ko.md) | Zdarzenie integracyjne „Zgon potwierdzony” (E121) w kontrakcie modułu sukcesja | sukcesja / kontrakt | 1–2 | gotowy |
| [MEM-E9-POWIADOMIENIA-04](../MEM-E9-POWIADOMIENIA-04-polityka-p43-zawsze-gdy-zgon-potwierdzony-wtedy-za.md) | Polityka P43: Zawsze gdy zgon potwierdzony, wtedy zaplanuj przypomnienia dla następcy (7. i 21. dzień) | powiadomienia / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E9-PLAT-01](../MEM-E9-PLAT-01-ekran-s23-przejecie-albumu-nastepca.md) | Ekran S23 „Przejęcie albumu (następca)” | - / UI | 2–4 | gotowy |
| [MEM-E9-PLAT-02](../MEM-E9-PLAT-02-ekran-s25-zglos-zgon-wlasciciela.md) | Ekran S25 „Zgłoś zgon właściciela” | - / UI | 2–4 | gotowy |
