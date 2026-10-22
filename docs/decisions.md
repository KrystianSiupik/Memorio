# Memorio / GEO — rejestr decyzji

Tickety powstają jako pliki w repozytorium, bez Jiry.

Format: **D-nnn** · etap · pytanie · wybór · dlaczego · alternatywy · odwracalność · gdzie zapisane.
Status: `do przeglądu` (domyślny) → `zaakceptowane` / `odrzucone: …`.

Decyzje sprzed 2026-10-17 (1–35 w przeglądzie wymagań, odpowiedzi na hot spoty, decyzje infrastruktury)
są w przeglądzie wymagań i nie są tu powtarzane.

> **Charakter projektu (2026-10-19):** Memorio / GEO to **produkt projektowy (ćwiczenie projektowe), nie realny produkt komercyjny**. Ustalenia prawne, DPIA, umowy, dyżury i koszty są dokumentacją tego, co byłoby potrzebne w realnym wdrożeniu — nie zobowiązaniami do wykonania; „blokery” blokują start realnego produktu, nie dalsze projektowanie.

---

## Sonda granic modułów HS-017 (2026-10-17)

### D-001 · Gdzie żyje „album bez właściciela” (HS-017) — `zaakceptowane 2026-10-18`
- **Wybór:** tryb tylko do odczytu zostaje stanem **Planów** (jedna maszyna stanów z B1); „Album bez właściciela” to fakt **Albumu**. Odmowa następcy → Sukcesja woła Album istniejącym portem („zwolnij rolę właściciela”) → Album emituje zdarzenie → Plany przełączają plan albumu w tryb tylko do odczytu. Nowe połączenie mapy: **Album → Plany (zdarzenie)**.
- **Dlaczego:** test skutku — B3 mówi wprost „ten sam co w B1”; Plany nie muszą znać zgonów ani odmów. Album i tak wie o zmianie właściciela (port Sukcesja → Album).
- **Odrzucone:** (a) zdarzenie Sukcesja → Plany — Plany poznałyby przyczynę i potrzebowałyby osobnego wejścia z Konta dla tego samego skutku; (b) tryb jako stan Albumu — dubluje terminy 24 mies., ostrzeżenia z dowodem doręczenia i przycięcie, które są w Planach.
- **Odwracalność:** wysoka (jedno zdarzenie i jedna polityka, P36).
- **Zapisane:** sonda 05/06, mapa S5 §9, model procesów (C153, E162, P36, HS-017 answered), kontrakty modułów.

### D-002 · Usunięcie konta właściciela ze zgodą na wygaśnięcie idzie tą samą drogą — `zaakceptowane 2026-10-18`
- **Wybór:** polityka P37 w Albumie: „konto usunięte” (właściciel bez następcy, zgoda na wygaśnięcie) → „zwolnij rolę właściciela” → ten sam fakt E162.
- **Dlaczego:** trzecia przyczyna tego samego skutku (S2:B3 KOMENDA B); bez tego Plany potrzebowałyby połączenia z Kontem.
- **Odwracalność:** wysoka.

### D-003 · Album bez właściciela: wybór materiału przy przycięciu — `zaakceptowane 2026-10-18`
- **Wybór:** nie ma kto wybrać w oknie 30 dni → reguła domyślna z B1 (zostaje najstarsze); ostrzeżenia 90/30/7/1 dzień idą do uczestników; każdy może pobrać swoje wkłady.
- **Dlaczego:** B1 już przewiduje regułę „bez jego decyzji”; nie dodaję nowego zachowania.
- **Odwracalność:** wysoka.

### D-004 · Czy inny uczestnik może przejąć album po odmowie następcy? — `zmienione 2026-10-18`
- **Pierwotnie:** w MVP nie, pytanie dla PO.
- **Decyzja:** **tak, już w MVP.** Zasady:
  - przejąć może każdy uczestnik albumu; **wygrywa pierwszy, kto opłaci** — technicznie: „Przejmuję” daje 30-minutową rezerwację (inni widzą „ktoś właśnie przejmuje”), opłacenie w tym czasie = nowy właściciel, brak opłaty = rezerwacja wygasa (bez podwójnych płatności i zwrotów);
  - **bez zgody pozostałych** — tylko powiadomienie; każdy może wyjść z albumu z wyborem zostaw/zabierz;
  - **treści zmarłej właścicielki zostają** (jak przy przejęciu przez następcę, B3);
  - przejęcie możliwe **przez całe 24 miesiące** trybu tylko do odczytu (do przycięcia); opłacenie przywraca pełne funkcje.
- **Skutki do naniesienia (po przeglądzie wszystkich decyzji):** nowa komenda w Albumie „Zarezerwuj przejęcie” + „Przejmij album” (RC, AG1, pierwszy wygrywa), nowe połączenie mapy **Plany → Album (zdarzenie „plan albumu opłacony”)**, zmiana niezmiennika AG8 („album bez właściciela wraca do aktywnego po przejęciu i opłaceniu”), nowy ekran/akcja w S10, tickety w E3/E7, aktualizacja sondy HS-017.

### D-005 · Nowe hot spoty z symulacji — `rozstrzygnięte 2026-10-18`
- **HS-020 (opłaty po zgonie):** po potwierdzeniu zgonu album **działa do końca opłaconego okresu**; rodzina dostaje instrukcję anulowania subskrypcji w sklepie (aplikacja nie może tego zrobić za nią); zwroty obsługuje sklep. Po końcu okresu — przejęcie przez następcę, inny uczestnik (D-004) albo tryb tylko do odczytu.
- **HS-021 (termin następcy):** **30 dni** od potwierdzenia zgonu, przypomnienia po 7 i 21 dniach; po terminie skutek jak odmowa (album bez właściciela), a przejąć może inny uczestnik (D-004).
- **Skutki do naniesienia:** odblokować tickety sukcesji (C122, C123, C126 — D-071), parametr terminu 30 dni w AG7, polityka przypomnień w Powiadomieniach, komunikat dla rodziny w S23/S25; AG8: „album bez właściciela” przechodzi w tryb tylko do odczytu dopiero po końcu opłaconego okresu.

## Klasyfikacja problemów (2026-10-17)
14 komend z CRUD/Integracji → RC.

### D-006 · Cykl planu (C90, C91, C93, C94, C95) to rywalizacja o zasób — `zaakceptowane 2026-10-18`
- **Wybór:** RC. Płatność, wstrzymanie, wybór materiału i przycięcie zmieniają/sprawdzają ten sam stan planu; przycięcie jest nieodwracalne.
- **Dlaczego:** scenariusz „zapłacił i stracił zdjęcia” albo „przycięto mimo wstrzymania” (S2:B1, S2:C10).
- **Alternatywa odrzucona:** zadanie w tle sprawdza stan i przycina bez blokady — tanie, ale otwiera najgorszy błąd produktu.
- **Odwracalność:** wysoka do czasu implementacji.

### D-007 · Okno 30 dni (C18/C19, C4/C5) — RC z warunkowym zapisem, bez agregatu-obiektu — `zaakceptowane 2026-10-18`
- **Wybór:** RC rozwiązane jednym warunkowym zapisem stanu (w koszu → usunięte / przywrócone).
- **Dlaczego:** wyścig rzadki, ale nieodwracalny; dwóch aktorów (użytkownik i zadanie), więc wystarczy warunek w zapisie.

### D-008 · Sprawa przejęcia (C121, C122, C123) — RC w Sukcesji — `zaakceptowane 2026-10-18` (version + unikalność „jedna otwarta sprawa na album”)
- **Wybór:** jedna decyzja na etap sprawy (weryfikacja aktu, przejęcie/odmowa); druga dostaje „już rozstrzygnięte”.
- **Dlaczego:** podwójny wynik daje sprzeczne skutki w Albumie i Planach (E123 + E162).

### D-009 · Blokada (C64) a wejście przez link (C61) — RC, jednostka = album — `zaakceptowane 2026-10-18`
- **Wybór:** C64 do RC; C63 (odbierz dostęp) zostaje CRUD, bo wejście linkiem po odebraniu dostępu jest dozwolone (S2:B2).

### D-010 · Rejestr pochodzenia (C31 + rejestracja artefaktu) — RC — `zaakceptowane 2026-10-18`
- **Wybór:** rejestracja i unieważnienie muszą się widzieć; inaczej artefakt z wycofanym źródłem zostaje żywy.
- **Dlaczego:** S2:B1 „Odwołanie wycofuje treść… z wszystkich artefaktów pochodnych”.

### D-011 · Wskazanie następcy (C41) zostaje CRUD — `zaakceptowane 2026-10-18`
- **Dlaczego:** źródło przewiduje reakcję (przypomnienie, gdy następca odchodzi), nie blokadę.

### D-012 · Zaproszenie a blokada adresu (C42/C46) zostaje CRUD — `zaakceptowane 2026-10-18`
- **Dlaczego:** koszt wyścigu to jeden e-mail; blokadę sprawdzają Powiadomienia przy wysyłce.

### D-013 · Kolejność zdarzeń dostępu (C65/C63) — Integracja, nie RC — `zaakceptowane 2026-10-18`
- **Wybór:** handler w Dostępie ignoruje zdarzenie starsze niż ostatnio zastosowane dla (album, osoba); zdarzenia skutku z Albumu niosą numer kolejny.
- **Dlaczego:** problem kolejności dostarczenia, nie równoległych decyzji.

## Projekt agregatów (2026-10-17)
AG7–AG11 nowe, AG3 rozszerzony.

### D-014 · Profil wolumenu — `zaakceptowane 2026-10-18`
- **Wybór:** do 5 tys. użytkowników; najgorętsza jest pula albumu (kilka operacji/s w szczycie); wszystkie jednostki podzielone na instancje.
- **Dlaczego:** decyzja infrastruktury z 2026-10-16; brak innych danych.

### D-015 · Pula: rezerwacja przy zapisie, bez dwufazowej rezerwacji — `zaakceptowane 2026-10-18`
- **Wybór:** plik najpierw do strefy tymczasowej, rezerwacja przy zapisie w Archiwum z kluczem = id treści; nieudany zapis → zwrot (kompensacja).
- **Odrzucone:** rezerwuj → potwierdź z wygasaniem — wymaga zadania sprzątającego i stanu „zarezerwowane”.
- **Ryzyko:** plik w strefie tymczasowej przy pełnej puli trzeba usunąć (sprzątanie strefy tymczasowej i tak potrzebne).

### D-016 · Pula: warunkowy UPDATE zamiast wersji — `zaakceptowane 2026-10-18`
- **Dlaczego:** gorący wiersz; atomowe polecenie nie wymaga ponowień. Zużycie > limit dozwolone tylko po obniżeniu limitu (tryb tylko do odczytu).

### D-017 · C100 (tryb ponad limitem) w Cyklu planu — `zaakceptowane 2026-10-18`
- **Dlaczego:** konflikt z zakupem własnego Premium w tej samej chwili (C90).

### D-018 · Cykl planu: optymistyczna wersja — pierwszy agregat z `version` — `zaakceptowane 2026-10-18`
- **Dlaczego:** zapisy rzadkie, ponowienia automatyczne (zadania, handlery).

### D-019 · Brak odpowiedzi następcy: komenda C126 z terminem jako parametrem — `zaakceptowane 2026-10-18` (termin 30 dni — HS-021)
- **Wybór:** po terminie skutek jak odmowa (E124 → album bez właściciela); długość terminu i ewentualna lista następców — HS-021 (PO).
- **Odwracalność:** wysoka (parametr + jedna polityka).

### D-020 · Migawka następcy przy weryfikacji aktu zgonu — `zaakceptowane 2026-10-18`
- **Dlaczego:** usuwa konflikt „zmiana następcy w trakcie sprawy” bez włączania C41 do jednostki.

### D-021 · Sprawa przejęcia: optymistyczna wersja, skutek do Albumu po decyzji, idempotentnie — `zaakceptowane 2026-10-18`

### D-022 · Przyznanie odczytu sprawdza blokadę pod blokadą wiersza albumu — `zaakceptowane 2026-10-18`
- **Dlaczego:** inaczej zaproszenie przyjęte w chwili blokady omija blokadę (wymóg sklepów, S2:C11).

### D-023 · Pochodzenie: zapis, potem sprawdzenie (bez blokady) + dzienna kontrola — `zaakceptowane 2026-10-18`
- **Wybór:** fakty „źródło niedostępne” tylko dopisywane, z czasem; rejestracja sprawdza fakty od chwili odczytu materiałów.
- **Odrzucone:** blokada per źródło (setki źródeł na podsumowanie, tysiące artefaktów przy usunięciu konta) i blokada globalna.
- **Koszt:** okno niespójności rzędu milisekund przed pokazaniem artefaktu; kontrola dzienna jako siatka bezpieczeństwa.

### D-024 · Okno 30 dni: warunkowy UPDATE stanu (kosz, usunięcie konta) — `zaakceptowane 2026-10-18`

### HS-022 · Przycięty materiał do kosza na 30 dni — `rozstrzygnięte 2026-10-18: TAK`
- **Decyzja:** przycięty materiał trafia na 30 dni do kosza; opłacenie planu w tym czasie przywraca zdjęcia; po 30 dniach usunięcie ostateczne. **Do naniesienia:** AG8/C95 i Archiwum C24 (przycięcie = przeniesienie do kosza), odblokować ticket C95 (D-071), regulamin (podstawa przechowywania 30 dni — pytanie do prawnika).
- **Rekomendacja:** tak — zamienia jedyną nieodwracalną operację w odwracalną; mechanizm kosza już jest. Nie wprowadzam bez PO, bo to zmiana zakresu.

## Architektura systemu (2026-10-17)
Opcje i kompromisy każdej decyzji są w ADR o tym samym numerze.

| D | ADR | Decyzja | Najważniejszy powód | Odwracalność | Status |
|---|---|---|---|---|---|
| D-025 | 0001 | Modularny monolit, 14 modułów z mapy | transakcja + outbox w jednej bazie, 3 osoby | łatwa | zaakceptowane 2026-10-18 |
| D-026 | 0002 | Backend .NET (LTS) z DDD (outbox/inbox, Result, typowane identyfikatory) | dojrzały outbox/inbox; kaskady to rdzeń produktu | **trudna** — zależy od Z4 (zespół zna .NET?) | zaakceptowane 2026-10-18 (zespół zna .NET — Z4 potwierdzone) |
| D-027 | 0003 | Flutter na iOS, Android i web | jeden kod klienta przy 3 osobach; offline przez lokalną bazę (Drift) | **trudna** | zaakceptowane 2026-10-18 (pierwszy krok: spike mapy T5) |
| D-028 | 0004 | Jeden PostgreSQL + PostGIS, schemat i rola na moduł | jeden PITR (RPO 15 min), outbox w transakcji | średnia | zaakceptowane 2026-10-18 |
| D-029 | 0005 | Media w prywatnym magazynie obiektów UE, podpis 5 min po sprawdzeniu Dostępu | B2 bez proxy; koszt etapu 1 | średnia | zaakceptowane 2026-10-18 |
| D-030 | 0006 | Outbox/Inbox + proces roboczy, idempotentne handlery, numer kolejny zdarzeń | kaskady nie mogą ginąć przy awarii (G2) | średnia | zaakceptowane 2026-10-18: **Wolverine** (MIT), outbox/inbox i kolejki w PostgreSQL bez brokera na start; RabbitMQ/SQS po wyzwalaczu (etap 2, wydzielenie modułu, zalegająca skrzynka); na etapie 2 bez transakcyjnego poolera (PgBouncer/RDS Proxy) dla Wolverine |
| D-031 | 0007 | Wyszukiwanie w PostgreSQL z filtrem Dostępu w tym samym zapytaniu | C9 „od razu” z definicji | łatwa | zaakceptowane 2026-10-18 (MVP; wyszukiwanie za jednym portem — gotowe na D-075) |
| D-032 | 0008 | MapLibre + własny podkład PMTiles w UE, klastrowanie w kliencie | prywatność (nikt nie widzi, gdzie patrzysz), koszt stały; wyzwalacz: > 50 tys. punktów | średnia | zaakceptowane 2026-10-19 |
| D-033 | 0009 | AI asynchronicznie, pseudonimizacja, znacznik, wersja promptu; dostawca w UE po DPIA | C5, L13 testowalnie | łatwa | zaakceptowane 2026-10-19 |
| D-034 | 0010 | Konta w module Konto (ASP.NET Core Identity) + Apple/Google | brak podwykonawcy tożsamości; jedno miejsce usunięcia | średnia | zaakceptowane 2026-10-19 |
| D-035 | 0011 | Autoryzacja w aplikacji przez read port Dostępu, bez RLS (RLS ewentualnie później) | Dostęp jedynym źródłem odpowiedzi | łatwa | zaakceptowane 2026-10-19 |
| D-036 | 0012 | Quartz.NET z magazynem w PostgreSQL; zadania wołają agregaty | terminy prowadzą do nieodwracalnych operacji | łatwa | zaakceptowane 2026-10-19: **osobny worker** (kontener niezależny od API, wydzielalny); terminy w Quartz.NET z magazynem w PostgreSQL, tryb klastra na etapie 2; harmonogram niezależny od Wolverine (Wolverine tylko do wiadomości, D-030) |
| D-037 | 0013 | Etap 1: jeden VPS, Compose, nginx, WAL-G (wymuszone decyzją infra z 2026-10-16) | — | łatwa | zaakceptowane 2026-10-19 |
| D-038 | 0014 | OpenTelemetry → Prometheus/Loki/Grafana na osobnym VPS + monitor zewnętrzny; logi bez treści | brak podwykonawcy z danymi osobowymi | łatwa | zaakceptowane 2026-10-19 z poprawką: osobny serwer monitoringu tylko dla produkcji; środowiska testowe i staging — monitoring na tym samym serwerze co aplikacja |
| — | 0015 | Nakaz 1 h: **otwarte** — technika gotowa, ale zależy od dyżurów (HS-006, PO) | decyzja organizacyjna | — | otwarte |
| D-039 | 0016 | Offline: tylko tworzenie wspomnień i dodawanie zdjęć; UUID klienta = klucz idempotentności | scenariusz „szczyt górski”; **QS15 nie ma w źródłach** — PO może zawęzić | łatwa | zaakceptowane 2026-10-19 (offline tylko dla tworzenia wspomnień i dodawania zdjęć + przeglądanie z pamięci podręcznej) |
| D-040 | 0017 | REST + OpenAPI /v1, klient Dart generowany, kontrola zgodności w CI, minimalna wersja aplikacji | stare wersje ze sklepów | średnia | zaakceptowane 2026-10-19 |
| D-041 | 0018 | Wdrożenie jedną komendą, krótki restart, migracje expand/contract | 99% mieści restart; etap 2 tani | łatwa | zaakceptowane 2026-10-19 |

**Uwaga do przeglądu:** D-026 i D-027 (stos) są najdroższe do cofnięcia i opierają się na założeniu Z4. Jeśli zespół nie zna .NET albo Fluttera — te dwie decyzje trzeba zmienić przed startem; reszta modelu (granice, agregaty, outbox) od nich nie zależy.

## Model ekranów (2026-10-17)
27 ekranów, 10 przepływów, 259 przypadków testowych przejść.

### D-042 · Dopisane read modele R11–R19 do modelu procesów — `zaakceptowane 2026-10-19`
- **Co:** mapa wspomnień, szczegóły wspomnienia, album (skład i wkłady), „Udostępnione mi”, kosz, stan planu, sprawa przejęcia, moje zgłoszenia i decyzje, zaproszenie (bez treści).
- **Dlaczego:** źródła ich wymagają (S1 §3–§5, S2 B1–B3, C2), a event storming ich nie zapisał — bez nich nie ma ekranów głównych. Wszystkie z cytatem ze źródła, nie wymyślone.

### D-043 · C122 „Przekaż album następcy” wykonuje następca (A9), nie właściciel — `zaakceptowane 2026-10-19`
- **Dlaczego:** S2:B3 „Następca potwierdza przejęcie”; w modelu był aktor A2 (pomyłka z przejścia 1). Poprzednia wartość w `byPrev`.

### D-044 · Połączenie 63 kandydatów w 27 ekranów — `zaakceptowane 2026-10-19`
- Kreatory i ekrany łączone po momencie procesu i aktorze, np. „Nowe wspomnienie” = miejsce → treść (6 komend), „Zarządzanie albumem” = zaproszenia, następca, link, odbieranie dostępu (tylko właściciel), „Moje zgody” = AI, „zostaw”, kurator.
- Akcje RC i CRUD na jednym ekranie tylko tam, gdzie aktor i moment są te same; komunikaty odrzuceń („pula pełna”, „już rozstrzygnięte”) jako stany ekranu.
- Zakładki główne: Mapa · Albumy · Udostępnione mi · Konto.
- Panel obsługi (S26) jako osobna aplikacja web — pytanie Q1 (rekomendacja: tak).
- Zgłaszanie treści dostępne z aplikacji (S27) i przez www bez konta (S21) — wymóg sklepów i DSA (S6:L10, L19).

## Przegląd prawny (2026-10-17)
13 ustaleń (2 blokery), 13 pytań do prawnika; przepisy sprawdzone w źródłach urzędowych.
> ⚠ To nie jest porada prawna. Decyzje poniżej to decyzje projektowe do czasu opinii prawnika, nie ocena legalności.

| D | Ustalenie | Decyzja | Status |
|---|---|---|---|
| D-045 | R01 DPIA (bloker) | DPIA przed startem; wejście = architektura + ekrany | zaakceptowane 2026-10-19 |
| D-046 | R02 + R13 podwykonawcy | umowy art. 28 z każdym procesorem (VPS, magazyn, e-mail/push, AI), rejestr czynności, sekcja „Podwykonawcy” w S24, dostawcy tylko z gotową umową | zaakceptowane 2026-10-19 |
| D-047 | R03 link a „platforma” | link zostaje do opinii; procedura art. 18 DSA dopisana do Z6 | zaakceptowane 2026-10-19 |
| D-048 | R04 nakaz 1 h (bloker) | zdolność 1 h = wymaganie; forma dyżuru → PO (HS-006) | zaakceptowane 2026-10-19 |
| D-049 | R05 AI Act art. 50 ust. 2 | znacznik jak w SD9, format wg kodeksu praktyk | zaakceptowane 2026-10-19 |
| D-050 | R06 PKE art. 399 | **nowy ekran zgody przy pierwszym uruchomieniu** (analityka, powiadomienia spoza danych) w MVP | zaakceptowane 2026-10-19 |
| D-051 | R07 web | **MVP: sprzedaż tylko przez sklepy; web bez płatności** do opinii prawnika | zaakceptowane 2026-10-19 |
| D-052 | R08 CRA art. 14 | polityka zgłaszania podatności, runbook 24 h, SBOM w CI | zaakceptowane 2026-10-19 |
| D-053 | R09 ODbL | osobne tabele i atrybucja bez zmian; ID OSM w wspomnieniach do opinii | zaakceptowane 2026-10-19 |
| D-054 | R10 e-dowody | Z6: terminy 10 dni / pilne, zabezpieczenie 60 + 30 dni | zaakceptowane 2026-10-19 |
| D-055 | R11 akt zgonu | retencja: do zamknięcia sprawy + 12 mies. (propozycja), dostęp tylko obsługa z dziennikiem | zaakceptowane 2026-10-19 |
| D-056 | R12 sankcje | minimum ze skanu obowiązuje do opinii | zaakceptowane 2026-10-19 |

Nowe elementy do planu ticketów: ekran zgody (D-050), sekcja „Podwykonawcy” (D-046), proces podatności (D-052), procedura Z6 rozszerzona (D-047, D-054).

## Plan infrastruktury (2026-10-17)

| D | Decyzja | Dlaczego | Status |
|---|---|---|---|
| D-057 | Etap 1 = topologia z ADR: backend + proces roboczy w Compose, PostGIS, PMTiles i akty zgonu w magazynie obiektów, osobny VPS monitoringu; **bez serwera analizy obrazu** | wykrywanie twarzy poza MVP; monitoring nie może umrzeć razem z aplikacją | zaakceptowane 2026-10-19 |
| D-058 | Etap 2 = AWS UE: ECS (backend + proces roboczy osobno), RDS Multi-AZ, S3 + CloudFront z podpisami 5 min, Secrets Manager | bezstanowe usługi z etapu 1 przenoszą się bez zmian | zaakceptowane 2026-10-19 |
| D-059 | Nowy wyzwalacz etapu 2: przestoje wdrożeń > 50% budżetu niedostępności przez 2 mies. | przy 99,5% krótki restart przestaje się mieścić | zaakceptowane 2026-10-19 |
| D-060 | Quartz.NET w trybie klastra przy migracji + test, że zadania nieodwracalne nie uruchamiają się podwójnie | AG8, AG10, AG11 zależą od pojedynczego wykonania | zaakceptowane 2026-10-19 |
| D-061 | Sekrety etapu 1: .env poza repo + sops/age w repo do odtworzenia | gotowość RD10 bez nowej usługi | zaakceptowane 2026-10-19 |

## Symulacja zmian biznesowych (2026-10-17) — D-062…D-067 `zaakceptowane 2026-10-19`

| D | Etap | Decyzja | Dlaczego |
|---|---|---|---|
| D-062 | założenia | Opis biznesu bez nazw modułów; metryka: % płacących po 90 dniach + retencja 12-mies.; mapa = 14 modułów, rozmiar = liczba komend/zdarzeń/polityk/read modeli | brak kodu → dokumenty jako architektura; metryka z modelu przychodu (S1 §16) |
| D-063 | perspektywy | perspektywa dużego klienta zamieniona na użytkownika końcowego | produkt B2C — klient korporacyjny nierealny |
| D-064 | przegląd opisu | opis bez zmian, braki zapisane jako znane luki (napięcie „nic nie znika” ↔ przycięcie = HS-022) | braki to doprecyzowania, nie zmieniają perspektyw |
| D-065 | wymagania | 17 wymagań po deduplikacji: 16 realistycznych, R09 (domy seniora) odrzucone — brak klienta B2B i podstawy zgody za osobę niezdolną; R08 realistyczne z notą (MVP bez sprzedaży web, D-051) | — |
| D-066 | wpływ na moduły | 28 rozbieżnych ocen wpływu rozstrzygniętych (25 komórek + 3 nowe moduły): zasada — „nowa funkcja”, gdy pojęcie nie istnieje w module (gość bez konta, osoba oznaczona, transkrypcja głosu), „już jest”, gdy wystarcza konfiguracja; nowe moduły: przyjęty „jurysdykcja-i-lokalizacja”, odrzucone „analityka-konwersji” (poza domeną) i „magazyn-mediow” (adapter infrastruktury) | spójność między wymaganiami |
| D-067 | wnioski | granice bez zmian; ryzyka „konto 16+ jako jedyny uczestnik”, „osoba oznaczona”, „plan tylko ze sklepu” → notatki w epikach i sondy po MVP; port „źródło uprawnień” w Planach | `31-wnioski.md` |

## Plan epików i ticketów (2026-10-17)
| D | Decyzja | Dlaczego | Status |
|---|---|---|---|
| D-068 | Plan i tickety jako pliki w `docs/jira/mvp/`; **bez Jiry** | tickety jako pliki | zaakceptowane 2026-10-19 |
| D-069 | Priorytety: P0 = MVP z S1 §15 (konto, mapa, wspomnienia, albumy, udostępnianie, Prześledź, Podsumuj, prywatność i usuwanie) + plany/limity (model przychodu i obietnica 24 mies.) + moderacja (wymóg prawny L10/L19); P1 = sukcesja (B3, ale nie w §15); P2 = import z telefonu i plan rodzinny (S1 §16 „dalszy rozwój”) | S1 §15/§16 + skan prawny | zaakceptowane 2026-10-19 ze zmianą: **sukcesja P0** (wymaga jej D-004) |
| D-070 | Poza zakresem MVP: Publikacja (PR11), Profil miejsca (PR12), Odkrywaj/rekomendacje (HS-002), druk/fotoksiążki, generowanie filmów | S1 §15–16 | zaakceptowane 2026-10-19 |
| D-071 | Hot spoty blokują tylko tickety, których wynik od nich zależy: C84 (HS-009), C95 (HS-022), C122/C123/C126 (HS-020/021), plan rodzinny (HS-012); pozostałe otwarte hot spoty jako ryzyka w ticketach | nie blokować ścieżki krytycznej pytaniami, które nie zmieniają kodu ticketu | zaakceptowane 2026-10-19 |
| D-072 | Rozmiar 1–4 h; 4 ekrany złożone (S04, S10, S11, S26) jako wyjątek 4–6 h z uzasadnieniem i pewnością „niska” | kreatory z 5–6 akcjami | zaakceptowane 2026-10-19 |

---

## Co przejrzeć najpierw (podsumowanie 2026-10-17)
1. **D-026, D-027 — stos (.NET + Flutter).** Najdroższe do cofnięcia, oparte na założeniu Z4 (zespół zna te technologie).
2. **D-050, D-051 — nowy ekran zgody w MVP oraz brak sprzedaży przez www.** Zmieniają zakres.
3. **D-006…D-010 — nowe miejsca rywalizacji o zasób** (cykl planu, okno 30 dni, sukcesja, blokada, pochodzenie). Dodają pracy, ale chronią nieodwracalne operacje.
4. **Pytania do PO, które blokują tickety:** HS-006 (dyżur 1 h), HS-009, HS-012, HS-020, HS-021, HS-022.
5. **Pytania do prawnika:** 13 z przeglądu prawnego i 21 ze skanu. Stan prawa jest ważny do 2026-11-15.

## Decyzje dodane w przeglądzie (2026-10-18)

### D-073 · Bez multi-tenancy — `zaakceptowane 2026-10-18`
- **Wybór:** brak podziału na tenantów; izolacja na poziomie osoby i albumu przez moduł Dostęp (filtr w każdym zapytaniu, ADR 0011), test macierzy użytkownik × zasób w CI (T14), media tylko przez adresy ważne 5 min (ADR 0005); RLS w bazie jako możliwa druga linia później.
- **Dlaczego:** klienci to osoby i rodziny, nie organizacje; ta sama osoba jest w wielu albumach różnych grup, więc granica „tenanta” nie istnieje; narzut na każdą tabelę, migrację i kopię przy 3-osobowym zespole bez zysku.
- **Wyzwalacz powrotu:** pierwszy klient instytucjonalny (np. domy seniora, R09 z symulacji zmian) — wtedy raczej logiczny podział (kolumna instytucji + filtr), nie osobne bazy.

### D-074 · Klasy przechowywania zdjęć (archiwizacja w magazynie obiektów) — `zaakceptowane 2026-10-18`
- **Wybór:** MVP — jedna klasa przechowywania; od startu zapisujemy przy zdjęciu datę dodania i ostatniego otwarcia. Po wyzwalaczu reguła cyklu życia „> 12 mies. → klasa rzadkiego dostępu z natychmiastowym odczytem” (np. S3 Standard-IA / Glacier Instant Retrieval).
- **Wyzwalacz:** przechowywanie zdjęć > ~40% kosztów infrastruktury (miara kosztu/użytkownika z SD14) albo przejście na AWS (etap 2).
- **Nigdy:** archiwum z odtwarzaniem w godzinach (Glacier Flexible/Deep Archive) dla zdjęć widocznych dla użytkownika — łamie „24 miesiące pełnego odczytu” i adresy 5 min (D-029).
- **Do sprawdzenia przy wyborze dostawcy etapu 1:** jakie klasy i ceny ma magazyn Hetzner/OVH (bez kwot z pamięci).
- **Źródło:** przegląd decyzji; symulacja zmian R16.

### D-075 · Inteligentne wyszukiwanie: Elasticsearch/OpenSearch w późniejszym etapie — `zaakceptowane 2026-10-18`
- **Wybór:** po MVP wyszukiwarka pełnotekstowa z inteligentnym dopasowaniem (Elasticsearch albo OpenSearch — licencja do sprawdzenia przy wdrożeniu). Indeks zawiera tylko album i autora treści, **nie uprawnienia**; przy każdym zapytaniu lista albumów osoby pobierana na żywo z Dostępu jako filtr + ponowne sprawdzenie uprawnień trafień przed pokazaniem (C9: odebrany dostęp znika od razu).
- **Gotowość w MVP:** wyszukiwanie za jednym portem w kodzie (D-031), podmiana implementacji bez zmian w reszcie.
- **Warunki:** indeks w UE; usługa zewnętrzna = umowa powierzenia i wpis na listę podwykonawców (D-046); raczej etap 2 (pamięć).
- **Wyzwalacz:** decyzja o inteligentnym wyszukiwaniu (S1 §10) albo skargi na trafność.
- **Źródło:** przegląd decyzji.

### D-076 · Clean Architecture (Onion) + lekki CQRS — `zaakceptowane 2026-10-18`
- **Backend:** każdy moduł ma warstwy Domena (bez zależności) ← Aplikacja (przypadki użycia, porty, handlery) ← Infrastruktura (EF Core, Wolverine, magazyn zdjęć, dostawca AI) ← API. Zależności tylko do środka; domena nie zna Wolverine, EF Core ani HTTP.
- **Aplikacja Flutter:** Clean Architecture per funkcja (mapa, album, konto…): domain / data / presentation, zależności do środka, funkcje korzystają ze sobą tylko przez publiczny eksport.
- **Lekki CQRS:** komendy przez domenę i agregaty; zapytania (mapa, listy, wyszukiwanie) jako projekcje do DTO w infrastrukturze, bez logiki biznesowej, nigdy źródło prawdy dla komend.
- **Pilnowanie:** testy architektury w CI (backend: NetArchTest/ArchUnitNET; Flutter: lint importów). Nowy ADR 0019, enabler T15.
- **Do naniesienia:** szkielety modułów i funkcji aplikacji oraz kryteria ticketów w planie (przy zbiorczej aktualizacji).

### D-077 · Zaproszenie do dodania zdjęć prowadzi przez rejestrację — `zaakceptowane 2026-10-19`
- **Wybór:** link „wrzuć swoje zdjęcia” (np. zbiórka po wyjeździe) przekierowuje na rejestrację; po niej użytkownik ląduje od razu w kontekście zaproszenia (album, okres). Rejestracja krótka: Apple/Google jednym dotknięciem, wersja web bez instalacji.
- **Skutek:** autor treści to zawsze konto — model „uczestnik = konto 16+” zostaje; prawo do wycofania działa przez konto; brak „gościa” i lekkiej tożsamości (ryzyko 1 z symulacji zmian zamknięte tym kompromisem).
- **Koszt:** część zaproszonych nie przejdzie rejestracji. Babcia-widz („list z rodziny”) — po MVP, wtedy decyzja, czy wymaga konta.

### D-078 · Księga uprawnień w Planach — `zaakceptowane 2026-10-19`
- **Wybór:** Plany prowadzą księgę „zasób X ma uprawnienie do daty Y, źródło Z”; każde źródło płatności to adapter (MVP: sklepy — bezpośrednio albo serwis subskrypcji typu RevenueCat, wybór przy ticketach płatności po sprawdzeniu warunków). Cykl planu (D-006) czyta wyłącznie księgę.
- **Dlaczego:** nowe źródła (www, faktury, zakupy jednorazowe, zniżki, przejęcie albumu z D-004) nie dotykają logiki przycięcia; historia płatności przy sporach i zwrotach.

### D-079 · Oznaczenia osób — `zaakceptowane 2026-10-19`
- **Oznaczyć można tylko konto**, i to **tylko uczestnika tego samego albumu** (obcego nie da się oznaczyć ani wyszukać). Wspomnienie prywatne (poza albumem) — bez oznaczeń, tylko tekst.
- **Osoba bez konta = zwykły tekst** w opisie (np. „Babcia Ania”): dane autora, bez praw i powiadomień.
- **Oznaczony dostaje powiadomienie** („Kasia oznaczyła Cię”) i **może jednym kliknięciem usunąć swoje oznaczenie**.
- **Gdy osoba z tekstu założy konto:** na razie autor zamienia wpisy na oznaczenia ręcznie. **Backlog:** „zaznacz tekst i podmień na oznaczenie konta we wszystkich wystąpieniach” (u danego autora).
- **Zgłoszenia:** kategoria „to ja na zdjęciu” (dla osób bez konta, obsługa ręczna) zostaje.
- **Zmienia:** C9 („osoba z prywatnej listy autora”) → tekst albo oznaczenie uczestnika; problem 2 z symulacji zmian rozwiązany bez mechanizmu „przejęcia”.

### HS-009 · Podsumowanie AI „udostępnione rodzinie” — `rozstrzygnięte 2026-10-19`
- **Decyzja:** to **wpis w albumie** — odbiorcy to uczestnicy albumu, podlega tym samym zasadom co album (odebranie dostępu → znika, kaskada B2, Pochodzenie). Odblokować ticket C84 (D-071).

### D-080 · Limity planów i ceny testowe do progu opłacalności — `do przeglądu 2026-10-19`
- **Wybór (propozycja, źródła nie podają liczb — S1 §13):** Darmowy 2 GB bez AI; Premium osobiste 50 GB, 4 podsumowania AI i 20 szkiców miesięcznie, 14,99 zł; Premium albumu pula 150 GB (do 10 osób), 10 podsumowań, 60 szkiców, 29,99 zł; plan rodzinny (P2) 6 kont, 34,99 zł.
- **Wynik:** `wycena/prog-oplacalnosci.md` (skrypt, ceny dostawców sprawdzone 2026-10-19). Premium osobiste ma zapas; **Premium albumu za 29,99 zł ledwo pokrywa koszt zmienny w scenariuszu ostrożnym** → do decyzji: cena 35–40 zł albo pula 100 GB.
- **Dlaczego:** limity z kosztu magazynu (Hetzner, 2 kopie), AI (Mistral Large 3, UE) i obietnicy 24 mies. (B1), która podwaja koszt magazynu.
- **Otwarte:** ceny VPS po podwyżkach Hetznera i prowizje sklepów tylko ze źródeł wtórnych; ostateczne ceny po testach z użytkownikami.

---

## Stan przeglądu (2026-10-19)
- Przejrzane wszystkie decyzje D-001…D-072 + nowe D-073…D-079. Zmienione w przeglądzie: D-004, D-005 (HS-020/021), HS-022, D-030 (Wolverine), D-036 (osobny worker, Quartz), D-038 (monitoring testowy na tym samym serwerze), D-069 (sukcesja P0), HS-009, D-076, D-077, D-079.
- Jedyna otwarta blokada: HS-006 (dyżur 1 h) — w projekcie projektowym jako dokumentacja.
- **Zmiany naniesione 2026-10-19:** model (model procesów: +11 komend, +13 zdarzeń, +7 polityk, R20; HS-009/020/021/022 zamknięte), mapa modułów §10 (Plany → Album, Sukcesja → Powiadomienia), agregaty (AG1, AG7, AG8, AG10), architektura (ADR 0001–0021, w tym 0019 Onion, 0020 księga uprawnień, 0021 bez multi-tenancy; Wolverine, worker, monitoring), ekrany (28, ekran zgód, przejęcie, oznaczenia, rejestracja z zaproszenia), plan (253 tickety, walidacja OK).

---

## Propozycje (2026-10-21) — `propozycja, do decyzji`

### D-081 · Ewaluacja jakości podsumowań AI — `propozycja`
- **Propozycja:** zestaw wzorcowy na danych syntetycznych + uprząż ewaluacji adaptera AI; miary: wierność źródłom (model-sędzia + sprawdzenia deterministyczne dat, miejsc, liczb), brak treści pominiętych uczestników, odmowa przy braku materiałów, zgodność listy źródeł; bramka blokująca zmianę promptu/modelu poniżej progu.
- **Dlaczego:** cel G3 „AI nie zmyśla” jest dziś sprawdzany tylko strukturalnie (lista źródeł, znacznik, brak nazw); treść podsumowania nie jest mierzona.
- **Alternatywy:** ręczny przegląd próbek przy każdej zmianie promptu (tani, nieskalowalny); brak ewaluacji (status quo).
- **Odwracalność:** łatwa (testy i CI, bez zmian w domenie).
- **Ticket:** `jira/mvp/propozycje/MEM-PROP-01-ewaluacja-jakosci-podsumowan-ai.md` (E6, poza planem MVP).

### D-082 · Embeddingi: wyszukiwanie semantyczne i dobór materiałów do podsumowań (RAG) — `propozycja`
- **Propozycja:** po MVP spike `pgvector` (w istniejącym PostgreSQL) vs Elasticsearch/OpenSearch (D-075); model embeddingów preferowany lokalny; filtr Dostępu w tym samym zapytaniu (ADR 0011); embedding usuwany razem z treścią; wersja modelu przy wektorze; wybrane materiały trafiają na listę źródeł (Pochodzenie).
- **Dlaczego:** jeden mechanizm daje wyszukiwanie po znaczeniu i dobór materiałów do podsumowań dłuższych okresów.
- **Alternatywy:** tylko pełnotekstowe (D-031/D-075); dobór materiałów regułą (najnowsze, oznaczone).
- **Odwracalność:** średnia (dane pochodne do usunięcia i przeliczenia; port wyszukiwania już jest, D-031).
- **Otwarte:** podstawa prawna i DPIA dla embeddingów opisów; czy potrzebna osobna zgoda (jak C80).
- **Ticket:** `jira/mvp/propozycje/MEM-PROP-02-embeddingi-wyszukiwanie-semantyczne.md` (backlog z wyzwalaczem).

---

## MEM-E0-PLAT-01 · szkielet modularnego monolitu (2026-10-21)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-083 | **2 projekty na moduł:** `Memorio.Modules.<X>` (warstwy jako foldery, typy `internal`) + `Memorio.Modules.<X>.Contracts` (publiczny) | izolacja między modułami pilnowana przez kompilator, warstwy przez testy architektury; ~30 projektów zamiast ~70 |
| D-084 | **Wszystkie 14 modułów z mapy** od startu, także Publikacja i Profil miejsca (poza MVP, puste) | literalnie wg ticketu; moduły po MVP czekają na swoje tickety |
| D-085 | **Własny `Result` / `Error` w SharedKernel** (Validation, NotFound, Conflict, Forbidden, Failure) zamiast biblioteki | mały kod, pełna kontrola, bez zależności |
| D-086 | Testy architektury w **ArchUnitNET** (ADR 0001 dopuszcza NetArchTest/ArchUnitNET); każda reguła ma fixture, który ją łamie, a poprawny fixture wymaga, by każda reguła coś sprawdzała | NetArchTest nie ma dopasowania zależności wyrażeniem regularnym, potrzebnym do wyjątku „tylko Contracts” |
| D-087 | .NET 10 (SDK przypięty w `global.json`), xUnit v3 na **Microsoft.Testing.Platform**, centralne wersje pakietów, ostrzeżenia jako błędy; CI jako GitHub Actions (plik w repo, bez zdalnego repo na razie) | .NET 10 to aktualne LTS (ADR 0002); `dotnet test` w .NET 10 wymaga MTP |
| D-088 | Nazwy modułów w kodzie po angielsku: Archive, Album, Access, Provenance, Places, Limits, Plans, AiNarration, Moderation, Account, Succession, Notifications, PlaceProfiles, Publishing | kod po angielsku; mapowanie 1:1 na karty z mapa modułów |

## MEM-E0-PLAT-16 · struktura aplikacji Flutter (2026-10-21)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-089 | **Riverpod** (flutter_riverpod) na stan i wstrzykiwanie zależności | jeden mechanizm, kontrola kompilatora, podmiana zależności w testach przez overrides |
| D-090 | **go_router** do nawigacji | adresy URL na web, redirect niezalogowanych, zakładki (StatefulShellRoute) w PLAT-19 |
| D-091 | W tym tickecie tylko część Flutterowa PLAT-16; wzorzec warstw backendu osobno | reguły backendu są już w PLAT-01; przykład warstw backendu powstanie z pierwszym modułem |
| D-092 | Aplikacja w `app/`, pakiet `memorio`, org `app.memorio` (identyfikator aplikacji do potwierdzenia przed pierwszym wydaniem w sklepie) | jeden folder klienta dla mobile i web; identyfikator łatwo zmienić przed publikacją |
| D-093 | Funkcje: `lib/features/<f>/{domain,data,presentation}` + plik publiczny `<f>.dart`; `core/` dla kodu 2+ funkcji; `app/` = composition root (overrides providerów) | ADR 0019; prezentacja zależy tylko od interfejsów domeny |
| D-094 | Reguły importów sprawdzane własnym testem (`test/architecture/import_rules.dart`) z fixturami, zamiast pakietu lint | brak utrzymywanego darmowego pakietu wyrażającego warstwy w funkcjach i pliki publiczne; ~150 linii, każda reguła ma fixture |
| D-095 | Błędy oczekiwane jako `Result<T, F>` (sealed, Dart 3) w `core/`, z zamkniętym typem błędów per funkcja | wyczerpujący `switch` bez zależności (odpowiednik `Result` w backendzie, D-085) |
| D-096 | Zależności przypięte dokładnie (flutter_riverpod 3.4.3, go_router 18.1.1, flutter_lints 6.0.0); Flutter 3.47.7 w CI; formatter 120 znaków | powtarzalne buildy, ta sama szerokość linii co backend |

## MEM-E0-PLAT-12 · API v1, OpenAPI, klient Dart, minimalna wersja aplikacji (2026-10-22)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-097 | Klient Dart generowany **swagger_parser** (dio + retrofit + json_serializable) do `app/lib/core/api/generated` | czysty Dart, bez Javy lokalnie i w CI |
| D-098 | Zgodność kontraktu: **snapshot** `api/openapi.v1.json` (test w backendzie) + **oasdiff** (`breaking --fail-on ERR`) względem `version/0.1` w CI | snapshot łapie każdą zmianę, oasdiff blokuje zmiany łamiące zgodność |
| D-099 | Wygenerowany kod klienta **commitowany**; CI generuje ponownie i wymaga braku różnic (`tools/generate-api-client.sh`) | zmiany kontraktu widoczne w review, aplikacja buduje się bez generatora |
| D-100 | Dokument OpenAPI z wbudowanego Microsoft.AspNetCore.OpenApi (OpenAPI 3.1), bez sekcji `servers`; wszystkie ścieżki pod `/v1` (moduły dostają grupę `/v1` w `IModule.MapEndpoints`) | standard .NET 10 bez dodatkowych bibliotek; kontrakt niezależny od hosta |
| D-101 | Minimalna wersja: nagłówek `X-App-Version`, poniżej `AppVersion:Minimum` odpowiedź **426** z kodem `App.UpdateRequired`; brak nagłówka = obsłużone (web, narzędzia), zły format = 400; `GET /v1/app/version-policy` jako sprawdzenie przy starcie aplikacji | ADR 0017; aplikacja dowiaduje się przy pierwszym żądaniu, bez osobnej logiki porównania w kliencie |
| D-102 | Format błędów API: **RFC 7807 problem details** z rozszerzeniem `code` („<Obszar>.<Powód>”) | standard ASP.NET Core; kod maszynowy do mapowania na komunikaty w aplikacji |
| D-103 | Wersja aplikacji z `package_info_plus`, odczytana raz w `main()`; adres backendu z `--dart-define=API_BASE_URL` | jedno źródło wersji (pubspec); adres ustawiany przy buildzie per środowisko |

## MEM-E0-PLAT-19 · nawigacja aplikacji (2026-10-22)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-104 | Typowane trasy jako zamknięta hierarchia klas `AppRoute` (bez generowania kodu); zakładki przez `StatefulShellRoute.indexedStack` (każda zakładka ma własny stos) | go_router_builder 4.5.2 jest niezgodny z build_runner 2.17.1 wymaganym przez generatory klienta API; ręczne typy dają to samo bez cofania wersji |
| D-105 | Teksty interfejsu na razie po angielsku, w kodzie; lokalizacja (PL/EN) osobnym zadaniem | ekrany to jeszcze zaślepki; l10n warto wprowadzić przed pierwszym prawdziwym ekranem |
| D-106 | Sesja w funkcji `account`: interfejs `SessionRepository` w domenie, `sessionProvider` w prezentacji, do PLAT-07 implementacja `NoSessionRepository` (nikt nie jest zalogowany) | przekierowanie działa i jest testowane już teraz; PLAT-07 podmienia tylko implementację w `app/composition.dart` |
| D-107 | Podczas odczytu sesji ekran startowy z `?from=`, po odczycie powrót do żądanego adresu; błąd odczytu = niezalogowany | żadna zakładka nie mignie przed sprawdzeniem sesji; głębokie linki nie giną |
| D-108 | Baner offline z S02 przenoszę do MEM-E0-PLAT-11 (offline) | wymaga wykrywania łączności, które należy do tamtego ticketu |

## MEM-E0-PLAT-11 · kolejka offline (2026-10-22)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-113 | Kolejka offline w `core/offline`: tabela Drift `pending_operations`, id = **UUID v7 nadany w aplikacji** = klucz idempotencji dla backendu; wysyłka w kolejności utworzenia, pierwsza operacja do ponowienia zatrzymuje wysyłkę (zdjęcia nigdy przed wspomnieniem); ponowienia 5 s → ×2 → maks. 15 min; odrzucone biznesowo (np. pula pełna) zostają z kodem do pokazania użytkownikowi | ADR 0016, D-015, D-039; UUID v7 rośnie w czasie, więc dobrze się indeksuje po stronie serwera |
| D-114 | Rodzaje operacji i ich wysyłające (`OperationSender`) rejestrują funkcje w `app/composition.dart` (`operationSendersProvider`); wysyłka startuje po powrocie łączności (connectivity_plus) | kolejka nie zna funkcji; nowa operacja offline = jeden sender w funkcji |
| D-115 | Baza lokalna na iOS/Android przez drift_flutter; **web bez bazy lokalnej** do pierwszej funkcji, która kolejkuje na web (potrzebne `sqlite3.wasm` i `drift_worker.js` w `web/`) | offline dotyczy telefonu („szczyt górski”); nie dokładam zasobów web bez użycia |
| D-116 | build_runner cofnięty do 2.16.2 (pozostałe generatory bez zmian wersji), bo drift_dev 2.35.1 wymaga `build` < 5; skrypt `tools/generate-app-code.sh` generuje cały kod (klient API, Drift, JSON) i formatuje `lib`, CI wymaga braku różnic w `lib` | jeden sposób generowania; klient API generuje się identycznie |
| D-117 | Baner offline S02 (D-108) zrobiony tutaj: na każdej zakładce, gdy brak łączności | wykrywanie łączności powstało w tym tickecie |
| D-118 | Idempotencja po stronie backendu (ten sam id = jeden skutek, test API z powtórzonym kluczem) powstaje z pierwszym endpointem tworzącym (C12 w E2) | dziś nie ma endpointu, który by ją przyjmował |

## MEM-E0-PLAT-06 · adapter AI (2026-10-22)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-119 | Pseudonimizacja w domenie Narracji AI: nazwy osób i miejsc → `[PERSON_n]`/`[PLACE_n]` (numerowane wg pierwszego wystąpienia), dopasowanie bez wielkości liter i polskich znaków po rdzeniu (słowo bez 2 ostatnich liter, min. 3; min. 2 dla nazw wielowyrazowych), odpowiedź z nieznanym tokenem odrzucana | QS8: polskie nazwy się odmieniają („w Krakowie”), dokładne dopasowanie przepuszczałoby odmiany; nadmiarowe ukrycie słowa jest bezpieczną stroną. Znane ograniczenie: odmiany jednowyrazowych nazw krótszych niż 4 litery |
| D-120 | `AiGeneratedText` nie istnieje bez znacznika AI, modelu i wersji promptu (`id@wersja`); prompty wersjonowane w kodzie (`Infrastructure/Prompts`), zmiana treści = wyższa wersja | SD9, AI Act art. 50 ust. 2 (K9) |
| D-121 | Port `ITextGenerator` w Application, pierwszy adapter: **Mistral** (chat completions, wynik JSON) przez `HttpClient` + Microsoft.Extensions.Http.Resilience (60 s na próbę); ostateczny dostawca po DPIA i umowie powierzenia (ADR 0009) | D-080 liczył koszty na Mistral (UE); port pozwala zmienić dostawcę bez zmian domeny |
| D-122 | Konfiguracja AI opcjonalna (`Ai__ApiKey`): bez klucza funkcje AI zwracają `AiNarration.ProviderUnavailable` (E18), reszta aplikacji działa; treść materiałów i odpowiedzi nigdy nie trafia do logów | środowiska bez AI (dev, testy) nie potrzebują klucza |
| D-123 | Testy modułów w osobnych projektach `tests/Modules/Memorio.Modules.<X>.Tests` z `InternalsVisibleTo` | typy modułu są `internal` (D-083); testy domeny bez bazy i HTTP |

## MEM-E0-PLAT-16 (backend) · szablon warstw modułu (2026-10-22)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-124 | Szablon modułu backendu jako opis w `src/Modules/README.md` z modułem Narracja AI jako wzorcem; testy kierunku zależności już działają od PLAT-01 | szablon generatora (`dotnet new`) nie opłaca się przy kilkunastu modułach tworzonych raz; reguły pilnuje CI |

## Katalog wzorców (2026-10-22)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-125 | Katalog wzorców w `docs/architecture/patterns.md`: problem → wzorzec → jak w Memorio → przykład w kodzie, plus lista antywzorców; nowy wzorzec spoza katalogu wymaga wpisu w `decisions.md` | te same rozwiązania tych samych problemów w każdym tickecie; wzorce tylko tam, gdzie rozwiązują konkretny problem |
