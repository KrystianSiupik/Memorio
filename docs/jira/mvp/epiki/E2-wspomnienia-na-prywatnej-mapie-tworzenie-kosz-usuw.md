# E2: Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony |
| Cel planu | G1 |
| Priorytet | P0 |
| Moduły | miejsca, archiwum, narracja-ai, limity, - |
| Zależy od epików | E0, E7 |
| Estymacja | 39–82 h |
| Źródło | S1 §15 pkt 2–4, 9; S2:B3, C1, C4, C6 |

## Kryteria ukończenia epiku

- [ ] Wszystkie tickety P0 epiku zamknięte, a testy przejść jego ekranów zielone. *Sprawdzenie: CI + test na urządzeniu*

## Tickety

| Kod | Tytuł | Moduł / warstwa | h | Status |
|---|---|---|---|---|
| [MEM-E2-MIEJSCA-01](../MEM-E2-MIEJSCA-01-wybierz-miejsce-z-podpowiedzi-c10.md) | Wybierz miejsce z podpowiedzi (C10) | miejsca / aplikacja | 1–3 | gotowy |
| [MEM-E2-MIEJSCA-02](../MEM-E2-MIEJSCA-02-dodaj-miejsce-wlasne-c11.md) | Dodaj miejsce własne (C11) | miejsca / aplikacja | 1–3 | gotowy |
| [MEM-E2-ARCHIWUM-01](../MEM-E2-ARCHIWUM-01-utworz-wspomnienie-c12.md) | Utwórz wspomnienie (C12) | archiwum / aplikacja | 1–3 | gotowy |
| [MEM-E2-ARCHIWUM-02](../MEM-E2-ARCHIWUM-02-dodaj-zdjecia-do-wspomnienia-c13.md) | Dodaj zdjęcia do wspomnienia (C13) | archiwum / aplikacja | 2–4 | gotowy |
| [MEM-E2-NARRACJA-AI-01](../MEM-E2-NARRACJA-AI-01-popros-ai-o-szkic-opisu-c15.md) | Poproś AI o szkic opisu (C15) | narracja-ai / aplikacja | 2–4 | gotowy |
| [MEM-E2-ARCHIWUM-03](../MEM-E2-ARCHIWUM-03-zatwierdz-opis-c16.md) | Zatwierdź opis (C16) | archiwum / aplikacja | 1–3 | gotowy |
| [MEM-E2-ARCHIWUM-04](../MEM-E2-ARCHIWUM-04-usun-swoja-tresc-c17.md) | Usuń swoją treść (C17) | archiwum / aplikacja | 1–3 | gotowy |
| [MEM-E2-ARCHIWUM-05](../MEM-E2-ARCHIWUM-05-usun-tresc-definitywnie-c18.md) | Usuń treść definitywnie (C18) | archiwum / domena + aplikacja | 3–4 | gotowy |
| [MEM-E2-ARCHIWUM-06](../MEM-E2-ARCHIWUM-06-przywroc-tresc-z-kosza-c19.md) | Przywróć treść z kosza (C19) | archiwum / domena + aplikacja | 3–4 | gotowy |
| [MEM-E2-ARCHIWUM-07](../MEM-E2-ARCHIWUM-07-zloz-zadanie-usuniecia-danych-c20.md) | Złóż żądanie usunięcia danych (C20) | archiwum / aplikacja | 1–3 | gotowy |
| [MEM-E2-LIMITY-01](../MEM-E2-LIMITY-01-zwroc-jednostki-do-puli-c151.md) | Zwróć jednostki do puli (C151) | limity / domena + aplikacja | 3–4 | gotowy |
| [MEM-E2-MIEJSCA-03](../MEM-E2-MIEJSCA-03-zapytanie-podpowiedzi-miejsc-openstreetmap-miejsca.md) | Zapytanie: Podpowiedzi miejsc (OpenStreetMap + miejsca własne) (R3) | miejsca / infrastruktura (query handler) | 1–3 | gotowy |
| [MEM-E2-ARCHIWUM-08](../MEM-E2-ARCHIWUM-08-zapytanie-mapa-wspomnien-punkty-wlasne-z-albumow-d.md) | Zapytanie: Mapa wspomnień (punkty: własne + z albumów, do których mam dostęp; filtry dat, osób, albumów) (R11) | archiwum / infrastruktura (query handler) | 1–3 | gotowy |
| [MEM-E2-ARCHIWUM-09](../MEM-E2-ARCHIWUM-09-zapytanie-szczegoly-wspomnienia-miejsce-data-z-pre.md) | Zapytanie: Szczegóły wspomnienia (miejsce, data z precyzją, treści per autor, osoby: tekst autora albo oznaczeni uczestnicy albumu) (R12) | archiwum / infrastruktura (query handler) | 1–3 | gotowy |
| [MEM-E2-ARCHIWUM-10](../MEM-E2-ARCHIWUM-10-zapytanie-kosz-moje-tresci-usuniete-w-ostatnich-30.md) | Zapytanie: Kosz (moje treści usunięte w ostatnich 30 dniach) (R15) | archiwum / infrastruktura (query handler) | 1–3 | gotowy |
| [MEM-E2-MIEJSCA-04](../MEM-E2-MIEJSCA-04-read-port-miejsca-dla-archiwum-odwolanie-do-miejsc.md) | Read port miejsca → dla archiwum: odwołanie do miejsca (wpis OSM albo miejsce własne) i jego istnienie | miejsca / kontrakt | 1–2 | gotowy |
| [MEM-E2-LIMITY-02](../MEM-E2-LIMITY-02-write-port-limity-dla-archiwum-zarezerwuj-zwroc-pr.md) | Write port limity → dla archiwum: zarezerwuj / zwróć przestrzeń w puli konta | limity / kontrakt | 1–2 | gotowy |
| [MEM-E2-LIMITY-03](../MEM-E2-LIMITY-03-write-port-limity-dla-narracja-ai-zuzyj-limit-ai.md) | Write port limity → dla narracja-ai: zużyj limit AI | limity / kontrakt | 1–2 | gotowy |
| [MEM-E2-ARCHIWUM-11](../MEM-E2-ARCHIWUM-11-polityka-p3-zawsze-gdy-okno-kosza-minie-wtedy-usun.md) | Polityka P3: Zawsze gdy okno kosza minie, wtedy usuń treść definitywnie | archiwum / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E2-ARCHIWUM-12](../MEM-E2-ARCHIWUM-12-zdarzenie-integracyjne-tresc-usunieta-definitywnie.md) | Zdarzenie integracyjne „Treść usunięta definitywnie” (E22) w kontrakcie modułu archiwum | archiwum / kontrakt | 1–2 | gotowy |
| [MEM-E2-ARCHIWUM-13](../MEM-E2-ARCHIWUM-13-polityka-p5-zawsze-gdy-zdjecia-maja-przypisane-mie.md) | Polityka P5: Zawsze gdy zdjęcia mają przypisane miejsce, wtedy utwórz z nich wspomnienie | archiwum / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E2-PLAT-01](../MEM-E2-PLAT-01-ekran-s03-mapa-i-globus.md) | Ekran S03 „Mapa i globus” | - / UI | 2–4 | gotowy |
| [MEM-E2-PLAT-02](../MEM-E2-PLAT-02-ekran-s04-nowe-wspomnienie-kreator-miejsce-tresc.md) | Ekran S04 „Nowe wspomnienie (kreator: miejsce → treść)” | - / UI | 4–6 | gotowy |
| [MEM-E2-PLAT-03](../MEM-E2-PLAT-03-ekran-s05-szczegoly-wspomnienia.md) | Ekran S05 „Szczegóły wspomnienia” | - / UI | 2–4 | gotowy |
| [MEM-E2-PLAT-04](../MEM-E2-PLAT-04-ekran-s06-kosz.md) | Ekran S06 „Kosz” | - / UI | 2–4 | gotowy |
