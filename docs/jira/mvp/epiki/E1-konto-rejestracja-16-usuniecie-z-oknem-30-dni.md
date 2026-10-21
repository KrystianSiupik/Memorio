# E1: Konto: rejestracja 16+, usunięcie z oknem 30 dni

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony |
| Cel planu | G2, G4 |
| Priorytet | P0 |
| Moduły | konto, archiwum, album, - |
| Zależy od epików | E0 |
| Estymacja | 22–45 h |
| Źródło | S1 §15 pkt 1, 9; S2:B3 |

## Kryteria ukończenia epiku

- [ ] Wszystkie tickety P0 epiku zamknięte, a testy przejść jego ekranów zielone. *Sprawdzenie: CI + test na urządzeniu*

## Tickety

| Kod | Tytuł | Moduł / warstwa | h | Status |
|---|---|---|---|---|
| [MEM-E1-KONTO-01](../MEM-E1-KONTO-01-zarejestruj-konto-c1.md) | Zarejestruj konto (C1) | konto / aplikacja | 1–3 | gotowy |
| [MEM-E1-KONTO-02](../MEM-E1-KONTO-02-zaakceptuj-regulamin-c2.md) | Zaakceptuj regulamin (C2) | konto / aplikacja | 1–3 | gotowy |
| [MEM-E1-KONTO-03](../MEM-E1-KONTO-03-zlec-usuniecie-konta-c3.md) | Zleć usunięcie konta (C3) | konto / aplikacja | 2–4 | gotowy |
| [MEM-E1-KONTO-04](../MEM-E1-KONTO-04-usun-konto-definitywnie-c4.md) | Usuń konto definitywnie (C4) | konto / domena + aplikacja | 3–4 | gotowy |
| [MEM-E1-KONTO-05](../MEM-E1-KONTO-05-cofnij-usuniecie-konta-c5.md) | Cofnij usunięcie konta (C5) | konto / domena + aplikacja | 3–4 | gotowy |
| [MEM-E1-ARCHIWUM-01](../MEM-E1-ARCHIWUM-01-usun-tresci-konta-c25.md) | Usuń treści konta (C25) | archiwum / aplikacja | 2–4 | gotowy |
| [MEM-E1-KONTO-06](../MEM-E1-KONTO-06-zapytanie-skutki-usuniecia-konta-ile-zdjec-z-ilu-a.md) | Zapytanie: Skutki usunięcia konta (ile zdjęć, z ilu albumów, czyich) (R2) | konto / infrastruktura (query handler) | 1–3 | gotowy |
| [MEM-E1-ARCHIWUM-02](../MEM-E1-ARCHIWUM-02-read-port-archiwum-dla-konto-ile-tresci-wypadnie-p.md) | Read port archiwum → dla konto: ile treści wypadnie przy usunięciu konta | archiwum / kontrakt | 1–2 | gotowy |
| [MEM-E1-ALBUM-01](../MEM-E1-ALBUM-01-read-port-album-dla-konto-z-ilu-albumow-czy-wlasci.md) | Read port album → dla konto: z ilu albumów, czy właściciel bez następcy | album / kontrakt | 1–2 | gotowy |
| [MEM-E1-KONTO-07](../MEM-E1-KONTO-07-polityka-p1-zawsze-gdy-okno-usuniecia-konta-minie-.md) | Polityka P1: Zawsze gdy okno usunięcia konta minie, wtedy usuń konto definitywnie | konto / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E1-KONTO-08](../MEM-E1-KONTO-08-zdarzenie-integracyjne-konto-usuniete-e7-w-kontrak.md) | Zdarzenie integracyjne „Konto usunięte” (E7) w kontrakcie modułu konto | konto / kontrakt | 1–2 | gotowy |
| [MEM-E1-ARCHIWUM-03](../MEM-E1-ARCHIWUM-03-polityka-p35-zawsze-gdy-konto-usuniete-wtedy-usun-.md) | Polityka P35: Zawsze gdy konto usunięte, wtedy usuń treści tej osoby | archiwum / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E1-PLAT-01](../MEM-E1-PLAT-01-ekran-s01-rejestracja.md) | Ekran S01 „Rejestracja” | - / UI | 2–4 | gotowy |
| [MEM-E1-PLAT-02](../MEM-E1-PLAT-02-ekran-s24-konto-i-prywatnosc.md) | Ekran S24 „Konto i prywatność” | - / UI | 2–4 | gotowy |
