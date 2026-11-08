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

## Dapper po stronie odczytu (2026-10-22)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-126 | **Zapis: EF Core** (repozytoria agregatów, migracje, jednostka pracy). **Odczyt: Dapper** w handlerach zapytań (`Infrastructure`), SQL tylko z parametrami, tylko na schemacie własnego modułu (rola bazy modułu, ADR 0004), wynik mapowany wprost do DTO; zapytania o cudze treści z filtrem Dostępu w tym samym SQL (ADR 0011). Pakiet `Dapper` (wersja przypięta dokładnie, dziś 2.1.89) dochodzi z pierwszym handlerem zapytania; PostGIS przez to samo `NpgsqlDataSource` z NetTopologySuite | lżejsze i szybsze odczyty (mapa, listy, wyszukiwanie), pełna kontrola nad SQL i PostGIS; zapis zostaje przy EF Core dla agregatów i migracji |
## MEM-E0-PLAT-17 · konfiguracja z ENV, health, kontener (2026-10-22)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-109 | Konfiguracja środowiska tylko ze zmiennych `Sekcja__Klucz`; opcje z `ValidateOnStart`, komunikaty budowane z nazwy zmiennej (`ConfigurationErrors.Variable`), nigdy z wartości; przy błędzie log krytyczny i brak startu. `appsettings.json` trzyma tylko ustawienia niezależne od środowiska (poziomy logów) | RD1 (12-factor); wartości mogą być sekretami |
| D-110 | `/health` = proces żyje (bez zależności), `/ready` = kontrole z tagiem `ready` (baza, magazyn dochodzą w swoich ticketach), poza `/v1` i poza kontraktem | RD4; awaria bazy nie powinna restartować kontenera, tylko zdejmować go z ruchu |
| D-111 | Jeden obraz `Dockerfile` w katalogu głównym: sdk 10.0.302 → aspnet 10.0.10, użytkownik bez roota (`APP_UID`), port 8080; brak `HEALTHCHECK` w obrazie (obraz aspnet nie ma curl), sprawdzanie zdrowia w Compose (PLAT-09) | RD2: ten sam obraz w każdym środowisku |
| D-112 | Testy hosta przez `MemorioApiFactory` z wymaganymi ustawieniami środowiska testowego | jedno miejsce z konfiguracją testów; każdy nowy wymagany klucz dopisuje się tylko tam |
| D-127 | Przy złej konfiguracji host loguje błąd (critical, bez wartości) i rzuca wyjątek dalej, więc proces kończy się kodem ≠ 0 (w kontenerze 139), a nie czystym `exit 1` | sprawdzone w Dockerze 2026-10-22; `WebApplicationFactory` w .NET 10 uruchamia `Program` i potrzebuje wyjątku, a `return 1` psuje testy konfiguracji; orkiestrator patrzy tylko na kod ≠ 0 |

## MEM-E0-PLAT-03 · PostgreSQL + PostGIS, schemat i rola na moduł, import OSM (2026-10-23)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-128 | Moduł `Name` → schemat `snake_case(Name)` i rola `memorio_<schemat>` (np. `ai_narration`, `memorio_ai_narration`). Rola jest właścicielem swojego schematu, `NOINHERIT`, bez praw do innych schematów; `PUBLIC` nie ma praw do schematów modułów ani `CREATE` w `public`; `public` trzyma tylko PostGIS, dostępny dla wszystkich. `search_path` roli = jej schemat, `public` | SD4 wymuszane uprawnieniami bazy, nie konwencją; jedna nazwa wyliczana z katalogu modułów |
| D-129 | Schematy i role tworzy idempotentny `db/bootstrap.sql` (psql jako właściciel bazy), nie migracje modułów; hasła ról ustawia każde środowisko ze swoich sekretów (PLAT-18), w skrypcie ich nie ma | migracje modułu działają już jako jego rola i nie mogą tworzyć ról; skrypt można puścić ponownie po dodaniu modułu |
| D-130 | Fitness function SD4 w `tests/Memorio.Database.Tests`: Testcontainers z obrazem `postgis/postgis:18-3.6` (tym z `compose.yaml`), bootstrap przez psql, test każdej roli przeciw każdemu cudzemu schematowi (SELECT, INSERT, CREATE → 42501) i lista schematów zgodna z `ModuleCatalog`. Testy wymagają Dockera (CI go ma) | test na prawdziwych uprawnieniach łapie przypadkowy grant (sprawdzone wstrzykniętym `GRANT`); `dotnet test` bez osobnej konfiguracji |
| D-131 | Import OSM: osm2pgsql (flex, `tools/osm-import/places.lua`) w osobnym obrazie uruchamianym przez `docker compose --profile tools`, nigdy z API; tabela `places.osm_places` (nazwa, rodzaj `klucz=wartość`, tagi `jsonb`, punkt 4326, obszary zamknięte jako centroid), tabele robocze osm2pgsql też w `places`; po imporcie właścicielem jest `memorio_places`. Import zastępuje poprzedni (`--create`); uruchamia go superużytkownik bazy. CI sprawdza import na syntetycznym `sample.osm` | osobna tabela tylko z danymi OSM (D-053, ODbL); narzędzie importu nie trafia do obrazu API |
| D-132 | `compose.yaml` w katalogu głównym tylko do pracy lokalnej i CI (PostgreSQL + narzędzie OSM); port przez `POSTGRES_PORT` (Windows rezerwuje czasem 5432). Środowiska wdrożeniowe dostaje PLAT-09 | jedno polecenie do lokalnej bazy; wdrożenie to osobny ticket |

## MEM-E0-PLAT-02 · Outbox/Inbox w Wolverine, osobny worker, metryka wieku kolejki (2026-10-23)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-133 | Wolverine 6.51.3 z PostgreSQL jako magazyn **i** transport (bez brokera): koperty w schemacie `wolverine`, kolejki w `wolverine_queues`, oba należą do roli `memorio_messaging` (bootstrap), Wolverine zakłada tabele przy starcie hosta. Zdarzenia `IIntegrationEvent` (SharedKernel) idą do kolejki `integration_events`. API tylko wysyła (bez handlerów), słucha wyłącznie **worker** (`src/Host/Memorio.Worker`, osobny obraz: cel `worker` w tym samym Dockerfile) przez trwały inbox; każdy handler modułu ma własną trwałą kolejkę lokalną (`MultipleHandlerBehavior.Separated`). Wspólny kod hostów (katalog modułów, konfiguracja, health, komunikacja) w `src/Host/Memorio.Hosting` | D-030, D-036; izolacja modułów: błąd jednego nie blokuje i nie powtarza innych (sprawdzone testem i mutacją bez `Separated`) |
| D-134 | Rola modułu dostaje w schemacie `wolverine` tylko `INSERT` (domyślne uprawnienia od `memorio_messaging`), bez `SELECT/UPDATE/DELETE` i bez dostępu do `wolverine_queues` | outbox zapisuje się połączeniem modułu w jego transakcji (sprawdzone na prototypie z rolą `memorio_album`); wysyłką i sprzątaniem zajmuje się `memorio_messaging`. Wyjątek od SD4 opisany i testowany w `Memorio.Database.Tests` |
| D-135 | Handlery Wolverine muszą być publiczne, a ich zależności widoczne dla generowanego kodu. Dlatego reakcja modułu to `IIntegrationEventPolicy<TEvent>` (internal, Application; nazwa „Policy” jak polityki P z modelu procesów, CA1711 zabrania sufiksu `EventHandler`), a publiczna statyczna klasa `<Event>Handler` w `Infrastructure/Messaging` tylko pobiera politykę z zakresu wiadomości (`IServiceProvider`) i ją woła; `ServiceLocationPolicy.AlwaysAllowed`. Reguła architektury: poza `IModule` publiczne w module są tylko takie klasy | zachowuje D-083 (wnętrze modułu internal) poza jednym, wąskim i testowanym wyjątkiem |
| D-136 | Metryka wieku: worker co 15 s pobiera identyfikatory oczekujących wiadomości (outbox, kolejka, inbox) i liczy wiek od pierwszej próbki, która je zobaczyła; do tego liczba oczekujących i liczba dead letters. Miernik `Memorio.Messaging`; eksport, alert > 10 min i alert na dead letter w MEM-E0-PLAT-10 | tabele Wolverine (outbox, inbox) nie mają czasu utworzenia; próbkowanie nie zależy od wewnętrznego formatu Wolverine. Wada: dokładność do 15 s, po restarcie workera wiek liczy się od zera |
| D-137 | Polityka błędów handlerów: ponowienia po 1 s, 10 s, 1 min, potem dead letters (`wolverine.wolverine_dead_letters`), każdy dead letter widoczny w metryce | kaskady nie mogą ginąć (G2), a zablokowana wiadomość musi być wykryta (QS12) |
| D-138 | Kod handlerów kompilowany w czasie działania (`WolverineFx.RuntimeCompilation`, Wolverine 6 nie ma go w rdzeniu); kolejka bez ruchu odpytywana co 5 s (domyślnie), bez LISTEN/NOTIFY | najprostsze na start; opóźnienie doręczenia mieści się w QS7 (≤ 1 min, test). Wyzwalacz zmiany: czas startu lub pierwszej wiadomości → kod generowany z góry (`TypeLoadMode.Static`) |
| D-139 | SD6: każdy handler modułu ma klasę `<Handler>IdempotencyTests` w `tests/Modules/Memorio.Modules.<Module>.Tests` (to samo zdarzenie doręczone dwa razy); test architektury szuka jej w źródłach testów tego modułu | konwencja sprawdzalna w CI bez wiedzy o treści testu; test innego modułu jej nie spełnia |
| D-140 | API i worker potrzebują bazy przy starcie (Wolverine zakłada tabele); `/ready` = baza komunikacji odpowiada. Testy API uruchamiają Wolverine bez magazynu i transportu, a cały przepływ (outbox → kolejka → inbox → handlery, wycofana transakcja, podwójne doręczenie, izolacja, metryka) sprawdza `Memorio.Messaging.Tests` na Testcontainers | testy API bez Dockera; prawdziwe zachowanie na prawdziwej bazie |
| D-141 | Połączenia komunikacji z `GssEncryptionMode=Disable` | bez Kerberosa; inaczej Npgsql szuka `libgssapi` w obrazie i zaśmieca log |

Poza zakresem PLAT-02 (z kryteriów ticketu): część QS7 „0 żywych artefaktów z niedostępnym źródłem po kontroli dziennej” należy do Pochodzenia (kontrola dzienna, D-023); alerty i eksport metryk do Prometheusa należą do PLAT-10.

## MEM-E0-PLAT-04 · Magazyn obiektów w UE: bucket prywatny, podpisy 5 min partiami, strefa uploadu (2026-10-23)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-142 | Port `IObjectStorage` w SharedKernel (`Storage/`: `StorageKey`, `SignedUrl`, `UploadSlot`, `StoredObject`), adapter `S3ObjectStorage` (AWSSDK.S3 4.0.104.6, adresowanie ścieżkowe, sumy kontrolne tylko gdy wymagane) w `Memorio.Hosting`, rejestrowany w API i workerze. Konfiguracja `Storage__Endpoint`, opcjonalnie `Storage__PublicEndpoint` (adres w podpisach, gdy hosty widzą magazyn pod innym adresem niż aplikacja), `Storage__Region`, `Storage__Bucket`, `Storage__AccessKey`, `Storage__SecretKey`; `/ready` sprawdza bucket | jeden adapter dla Hetznera/OVH (etap 1), S3 (etap 2, D-058) i lokalnego serwera; moduły zależą tylko od portu (D-066: magazyn to adapter, nie moduł) |
| D-143 | Ważność podpisu 5 min jako stała `ObjectStorageRules.SignatureLifetime`, nie konfiguracja; podpisy odczytu partiami (galeria naraz, jeden czas wygaśnięcia, podpis liczony lokalnie bez zapytań). Upload: aplikacja wysyła podpisanym PUT do `uploads/<uuid v7>` (podpisany typ treści), moduł po sprawdzeniu przenosi plik `PromoteUploadAsync` (kopiowanie + usunięcie) do klucza docelowego; rozmiar zwraca `StoredObject`, bo podpisany PUT nie ogranicza rozmiaru | wymaganie (PRZEGLAD §B2: „liczba należy do wymagania”); SD5 sprawdzane testem; strefa tymczasowa oddziela niezweryfikowane pliki od treści |
| D-144 | Bucket przygotowuje idempotentny `tools/storage/bootstrap.sh` (obraz `amazon/aws-cli:2.37.11`): tworzy bucket, gdy go brak (nigdy publicznej polityki ani ACL), reguła „`uploads/` wygasa po 1 dniu”, na koniec `check-private.sh` (SD5: anonimowy odczyt i listowanie muszą dać 401/403). CI uruchamia go dwa razy na lokalnym serwerze; codzienne uruchamianie na środowiskach — PLAT-10 | ten sam skrypt dla każdego dostawcy zgodnego z S3; kontrola polityki niezależna od kodu aplikacji |
| D-145 | Lokalnie i w testach: **Garage v2.4.1** (jeden węzeł, `--single-node --default-bucket`, konfiguracja `tools/storage/garage.toml`, stałe jawne dane dostępowe tylko do pracy lokalnej) | publiczne obrazy MinIO nie są już dostępne na Docker Hub (sprawdzone 2026-10-23); Garage jest zgodny z S3 (podpisy, kopiowanie, cykl życia), lekki i prywatny domyślnie |
| D-146 | Dostawca etapu 1 musi mieć centrum danych w UE i umowę powierzenia (D-046); region w `Storage__Region` (np. Hetzner `fsn1`/`nbg1`/`hel1`). Kod nie sprawdza położenia | położenie wynika z wyboru dostawcy i umowy, nie z konfiguracji |

Poza zakresem PLAT-04: część QS4 „wyszukiwanie od razu (następne zapytanie)” należy do filtra Dostępu (ADR 0011, PLAT-14).
**Stan testów (2026-10-23):** testy jednostkowe (klucze, podpisy ≤ 300 s, strefa uploadu, konfiguracja) przechodzą; testy na Garage (`PrivateBucketTests`), skrypty bucketu i nowy job CI nie były jeszcze uruchomione (Docker niedostępny po zapełnieniu dysku) — do sprawdzenia razem z PLAT-02.

## MEM-E0-PLAT-07 · Moduł Konto: ASP.NET Core Identity, Apple/Google, deklaracja 16+, strona usuwania konta (2026-10-23)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-147 | ASP.NET Core Identity (`AddIdentityCore`, użytkownik `AccountUser`, tabele w schemacie `account`, rola `memorio_account`) z tokenami bearer Identity: dostępowy 1 h, odświeżający 14 dni, nieprzezroczyste, chronione kluczami Data Protection trzymanymi w schemacie `account` (wspólne dla instancji, przeżywają restart). Własne endpointy `/v1/auth/register`, `/login`, `/refresh`, `/external/{provider}` i `GET /v1/account/me` zamiast `MapIdentityApi` (deklaracja 16+, nasze kody błędów, tylko potrzebne ścieżki). Logowanie: jedna odpowiedź 401 `Account.InvalidCredentials` na złe hasło, brak konta i blokadę; blokada po 5 próbach na 15 min | D-034 (bez podwykonawcy tożsamości); bez wyliczania kont |
| D-148 | Hasło: min. 10 znaków, bez wymogów klas znaków | długość chroni lepiej niż wymuszone klasy (wytyczne NIST SP 800-63B) |
| D-149 | Apple/Google: aplikacja przysyła token ID z natywnego logowania, backend sprawdza podpis kluczami z metadanych OpenID dostawcy (Microsoft.IdentityModel 8.23.0), wydawcę, odbiorcę (`Account__Google__ClientIds`, `Account__Apple__ClientIds`) i ważność; bez identyfikatorów dostawca jest wyłączony (404 `Account.ProviderUnavailable`). Nowe konto z dostawcy też wymaga deklaracji 16+. Logowanie dostawcy **nigdy nie dołącza się** do istniejącego konta po adresie e-mail (409 `Account.EmailTaken`); e-mail od dostawcy zapisywany tylko zweryfikowany | dołączanie po e-mailu oddałoby cudze logowanie temu, kto pierwszy zarejestrował adres (konta hasłowe nie mają jeszcze potwierdzenia e-mail) |
| D-150 | SD10: polityka domyślna wymaga zalogowania dla każdego endpointu (także nieznanych ścieżek: 401, nie 404); anonimowe tylko jawnie (`AllowAnonymous`) i tylko z listy w `AnonymousEndpointsTests` (health, OpenAPI, polityka wersji, `/v1/auth/*`, `/account/delete`) | nowy endpoint jest chroniony bez pamiętania o atrybucie; wyjątek wymaga zmiany listy w przeglądzie |
| D-151 | Aplikacja: tokeny w `flutter_secure_storage` 11.2.0 (Keychain, Android Keystore; w przeglądarce magazyn przeglądarki szyfrowany WebCrypto — słabszy, kod uruchomiony na stronie może go odczytać; przyjęte na MVP, do przeglądu bezpieczeństwa wersji web) razem z identyfikatorem konta, więc sesja jest znana offline. `AuthInterceptor` (kolejkowany) dołącza token, przy 401 odświeża raz dla wszystkich równoległych żądań; odrzucony token odświeżający czyści sesję i wraca do logowania; brak sieci sesji nie kończy. `TokenSessionRepository` zastępuje `NoSessionRepository` (D-106) | sesja odporna na offline (ADR 0016); jedno odświeżenie zamiast wyścigu |
| D-152 | Migracje EF Core w module (`Infrastructure/Persistence/Migrations`, `dotnet ef` 10.0.13 jako narzędzie lokalne w `dotnet-tools.json`), stosowane przy starcie hosta przez usługę modułu; `Database__MigrateOnStartup=false` wyłącza (np. gdy wdrożenie stosuje je samo, PLAT-13). Katalogi `Migrations` oznaczone w `.editorconfig` jako kod generowany; ich publiczne klasy (tak generuje `dotnet ef`) są wyjątkiem w regule widoczności modułu | etap 1: jedna instancja na host, EF Core blokuje równoległe migracje; rola modułu jest właścicielem schematu, więc sama zakłada tabele |
| D-153 | Krawędź HTTP wspólna dla modułów w `Memorio.Modularity`: `Problems.From(Error)` (Validation 400, NotFound 404, Conflict 409, Forbidden 403, Failure 500, rozszerzenie `code`, D-102), `IModule.MapPages` dla stron poza `/v1` i kontraktem, `HealthTags.Ready` dla kontroli gotowości modułów | jedno mapowanie błędów; moduł nie zna hosta |
| D-154 | `GET /account/delete`: publiczna strona (po polsku) z instrukcją usunięcia konta w aplikacji (także w przeglądarce) i 30-dniowym oknem; samo polecenie usunięcia w MEM-E1-KONTO-03 | wymóg sklepów i S3:L19 (usunięcie w aplikacji i przez www) |

Pytania otwarte (PLAT-07): (1) kanał dla osoby, która nie może się zalogować, a chce usunąć konto (adres e-mail/formularz — decyzja PO, wymóg Google Play); (2) potwierdzanie adresu e-mail i reset hasła potrzebują wysyłki e-mail (moduł Powiadomienia) — do zaplanowania.
**Stan testów (2026-10-23):** testy jednostkowe modułu Konto (deklaracja 16+, weryfikacja tokenów ID), testy API (SD10, strona usuwania, konfiguracja) i aplikacji (interceptor, repozytoria) przechodzą; `SignInTests` na PostgreSQL (rejestracja, logowanie, odświeżanie, dostawca, migracje jako `memorio_account`) jeszcze nieuruchomione (Docker niedostępny).
**Znany problem (2026-10-23):** testy API dwa razy (przy PLAT-04 i PLAT-07) miały 5–6 porażek w pierwszym przebiegu zaraz po pełnym buildzie Release, przy braku pamięci na maszynie; ponad 10 kolejnych przebiegów, także po przebudowie, przeszło bez błędów. Do zbadania przy wspólnym testowaniu (podejrzenie: limit czasu startu hosta testowego pod obciążeniem).

## MEM-E0-PLAT-08 · Worker: Quartz.NET z magazynem w PostgreSQL, zadania idempotentne, gotowe na klaster (2026-10-23)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-155 | Quartz.NET **4.6.0** (jeden pakiet `Quartz`: DI, hosting, health check) tylko w workerze. Magazyn ADO w PostgreSQL: schemat `scheduler` roli `memorio_scheduler` (`db/bootstrap.sql`, `search_path` roli = schemat), tabele zakłada Quartz przy starcie (`ProvisionSchema`, tylko tworzy, nic nie zmienia); role modułów nie mają do schematu żadnych praw (SD4, test). Konfiguracja `Scheduling__ConnectionString` (wymagana w workerze); `/ready` z health checkiem Quartz | D-036: harmonogram niezależny od Wolverine; osobna rola jak przy Wolverine (D-134); 4.x to aktualna linia (3.x bez rozwoju), ma wbudowane polityki ponowień i tworzenie schematu |
| D-156 | Tryb klastra **od razu** (`UseClustering`, blokady w bazie, `RequestRecovery`), nie dopiero na etapie 2 (zmiana D-060: „przy migracji”) | kosztuje tylko kilka zapytań; drugi worker przy wdrożeniu bez przestoju (etap 2) nie uruchomi zadań podwójnie; zadanie przerwane padnięciem węzła dokończy inny |
| D-157 | Moduły nie znają Quartz: zadanie to `IScheduledJob` (SharedKernel), rejestracja `AddScheduledJob<TJob>("Moduł.Zadanie", cron)` (Modularity, cron w UTC), terminy przez port `IDeadlines` (`ScheduleAsync` zastępuje termin o tym samym kluczu, `CancelAsync` idempotentne). W magazynie tylko nazwy zadań i dane jako tekst (`StoreJobDataAsStrings`), jeden typ zadania Quartz (`ScheduledJobRunner`, `DisallowConcurrentExecution`). `IDeadlines` tylko w workerze: termin wynikający ze zmiany w API ustawia handler zdarzenia integracyjnego zapisanego w outboksie w tej samej transakcji | moduł zmienia swoje wnętrze bez psucia zapisanych terminów; harmonogram Quartz ma własne połączenie, więc ustawienie terminu wprost z API nie byłoby atomowe ze zmianą agregatu — outbox to gwarantuje |
| D-158 | Nieudane uruchomienie ponawiane po 1 min, 10 min, 1 h (`ScheduledJobRules.RetryDelays`, domyślna polityka Quartz), potem porzucone (log + metryka `given_up`); terminy przegapione, gdy żaden worker nie działał, odpalają się od razu, zadania cykliczne raz i dalej według harmonogramu; nieznana nazwa zadania (moduł je usunął) — log, bez ponowień; zadania cykliczne, których nikt już nie rejestruje, usuwane przy starcie. Metryka `memorio.scheduling.runs` (`scheduled_job`, `outcome`; zmienione z `job` w PLAT-10) w mierniku `Memorio.Scheduling` | przejściowe awarie zależności mijają w minuty–godziny; termin nie może przepaść po restarcie |
| D-159 | SD12: każde `IScheduledJob` modułu ma w testach modułu klasę `<Job>IdempotencyTests` (test architektury, jak SD6); platformowy test `Memorio.Scheduling.Tests` na dwóch węzłach klastra: termin uruchomiony dwa razy zostawia stan jednego uruchomienia, każdy termin wykonany dokładnie raz w klastrze, ponowienie z numerem próby, anulowanie, zastąpienie terminu, zadanie cykliczne raz na wystąpienie | ponowienia i odzyskiwanie oznaczają „co najmniej raz”, tak jak w komunikacji (SD6) |

Uwaga: `DisallowConcurrentExecution` dotyczy jednego zadania Quartz, więc terminy jednego zadania modułu wykonują się po kolei (na całym klastrze). Przy dużej liczbie terminów naraz (np. masowe okna 30-dniowe) — do zmierzenia; ewentualnie zdjęcie tej blokady, bo zadania i tak są idempotentne.
**Stan testów (2026-10-23):** wszystkie testy rozwiązania przechodzą (253), w tym na Dockerze: `Memorio.Scheduling.Tests` (11), `Memorio.Database.Tests`, `Memorio.Messaging.Tests`, `PrivateBucketTests`, `SignInTests` — więc także zaległe testy PLAT-02, PLAT-04 i PLAT-07.

## MEM-E0-PLAT-10 · Obserwowalność: OpenTelemetry, Prometheus/Loki/Grafana, monitor zewnętrzny, alerty (2026-10-23)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-160 | Hosty wysyłają metryki i logi przez OpenTelemetry .NET (1.19.1 / instrumentacje 1.19.0) po OTLP/HTTP do **OpenTelemetry Collector** (0.162.0) na serwerze aplikacji (`Telemetry__OtlpEndpoint`; bez niej nic nie jest wysyłane). Collector wystawia metryki do pobrania (:8889) i wypycha logi do Loki (OTLP). Serwer monitoringu (produkcja, D-038): Prometheus 3.15.0 (pobiera z collectora i node exportera; retencja 30 dni), Loki 3.7.8 (retencja 30 dni), Grafana 13.2.3 (źródła i dashboard „Memorio” jako kod), Alertmanager 0.34.1, blackbox exporter 0.29.0. Środowiska testowe — wszystko na jednym serwerze (profil Compose `monitoring`). Cele Prometheusa z plików na środowisko (`/etc/prometheus/targets`); połączenie i zabezpieczenie ruchu między serwerami (TLS, sieć prywatna) — PLAT-09/18 | collector oddziela aplikację od backendów monitoringu (zmiana backendu = konfiguracja collectora); pobieranie metryk przez serwer monitoringu, więc awaria aplikacji nie zabiera monitoringu |
| D-161 | Etap 1 bez śladów (traces) i bez Tempo: tylko metryki i logi; identyfikatory śladu i tak są w rekordach logów | dwa sygnały wystarczają do QS12 i dostępności; śladowanie to dodanie potoku w collectorze i Tempo, gdy pojawi się problem wydajności do zbadania |
| D-162 | Logi bez treści (D-038): szablony komunikatów z parametrami, bez zakresów (`IncludeScopes = false`), bez treści żądań; zapytania SQL EF Core dopiero od poziomu Warning (bez wartości parametrów — `EnableSensitiveDataLogging` nigdy); w komunikatach tylko identyfikatory, nigdy e-mail, tekst czy plik użytkownika | monitoring nie staje się drugim miejscem przetwarzania danych osobowych |
| D-163 | Alerty jako kod (`monitoring/prometheus/rules`) z testami `promtool` w CI: QS12 (najstarsza wiadomość > 10 min — od razu; dead letter; brak metryki zaległości 10 min), host nie odpowiada 2 min / niegotowy 5 min (sonda zewnętrzna `/health` i `/ready` z serwera monitoringu = monitor zewnętrzny), brak metryk 5 min, błędy 5xx > 5% przez 5 min, dysk < 10% wolnego przez 10 min, zadanie porzucone/nieznane, **SD5 w sposób ciągły**: sonda anonimowego odczytu i listowania bucketu musi dostać 401/403 (zastępuje codzienne uruchamianie `check-private.sh`, D-144). Opóźnienie wykrycia QS12 ≤ ok. 2,5 min (próbka 15 s, eksport 60 s, pobranie 30 s, ocena 30 s); sprawdzone lokalnie: zatrzymanie API → alert w Alertmanagerze po 200 s. Odbiorca: webhook, adres jako sekret w pliku (PLAT-18) | reguły i progi przeglądane jak kod; test reguły chroni przed literówką w nazwie metryki |
| D-164 | Mierniki Memorio nazywane `Memorio.<Obszar>` (stała `MeterName`), eksportowane wzorcem `Memorio.*` — test architektury pilnuje prefiksu. SD14: miernik `Memorio.Account`, `memorio.accounts.count` (liczba kont co 5 min, tylko liczba); koszt na użytkownika = rachunek infrastruktury / liczba kont. Tag zadania w `memorio.scheduling.runs` to `scheduled_job` (nie `job`, którego Prometheus używa dla usługi) | nowy miernik jest widoczny bez pamiętania o liście; SD14 bez danych osobowych |

Pytania otwarte (PLAT-10): (1) kanał alertów i kto je odbiera (e-mail/SMS/komunikator, dyżur — HS-006, PO); (2) dostawca i adres serwera monitoringu produkcji (PLAT-09); (3) hasło administratora Grafany i dostęp do niej (sekret, PLAT-18).
**Stan testów (2026-10-23):** testy reguł `promtool` (w tym test, który celowo się wywraca), walidacja wszystkich konfiguracji (`tools/monitoring/validate.sh`), kontrola end-to-end na lokalnym stosie (`tools/monitoring/check-pipeline.sh`: metryki obu hostów, sondy, SD5, reguły, logi w Loki) i alert `MemorioDown` po zatrzymaniu API — przeszły; testy .NET (konfiguracja telemetrii, prefiks mierników) przechodzą.

## MEM-E0-PLAT-09 · Etap 1: VPS, Compose, nginx, WAL-G, comiesięczny test odtworzenia (2026-10-23)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-165 | Serwer aplikacji etapu 1 = jeden plik `deploy/compose.yaml`: nginx (jedyny publiczny, 80/443), API i worker (obrazy wydania z rejestru, `MEMORIO_VERSION`), PostgreSQL z WAL-G, OpenTelemetry Collector i node exporter związane z adresem sieci prywatnej (`PRIVATE_IP`; Docker omija ufw, więc chroni to wiązanie, nie zapora). Na serwerze repozytorium w tagu wydania (`/opt/memorio`), sekrety w `.env`, `postgres.env`, `app.env` (0600, PLAT-18). `deploy/provision.sh` (Ubuntu 24.04, idempotentny): Docker z repozytorium Dockera, ufw (SSH, 80, 443), automatyczne aktualizacje bezpieczeństwa, pierwszy certyfikat, stos, timery systemd, pierwsza kopia | D-037; jeden opis serwera zamiast ręcznej konfiguracji; ten sam plik sprawdzany lokalnie i w CI |
| D-166 | nginx 1.30.5 (linia stabilna): TLS z Let's Encrypt (certbot v5.8.0, webroot; pierwszy certyfikat w trybie standalone), HSTS, limit 10 żądań/min na adres (zryw 20) dla `/v1/auth/`, treść żądania ≤ 1 MB (media idą prosto do bucketu, D-143), gotowość workera (`/internal/worker/ready`) tylko dla serwera monitoringu. Nazwy usług rozwiązywane przy każdym żądaniu (`resolver 127.0.0.11 valid=1s`) | nginx rozwiązujący nazwę raz przy starcie wysyłał po ponownym utworzeniu kontenera API ruch pod stary adres (w teście trafiał do workera) — każde wdrożenie by to wywołało |
| D-167 | WAL-G v3.0.9 we własnym obrazie `memorio-postgres` (suma kontrolna binarki), ciągła archiwizacja WAL (`archive_timeout` 5 min), kopia bazowa codziennie 02:30 UTC, 14 przechowywanych (odtworzenie do dowolnej chwili z 14 dni), kompresja zstd i **szyfrowanie po stronie serwera** (libsodium, klucz jako sekret). Bucket kopii w UE, inny niż bucket mediów, z własnymi danymi dostępu. Pusta baza przy pierwszym starcie: `bootstrap.sql`, hasła ról z `PG_PASSWORD_<ROLA>` (brak hasła = przerwany start), schemat `operations` (tylko właściciel bazy) na znaczniki testu odtworzenia | QS2 (RPO ≤ 15 min): przy stałym ruchu zapisu (np. meldunki klastra Quartz) segment trafia do bucketu co ≤ 5 min; dostawca magazynu nie widzi danych |
| D-168 | Test odtworzenia PITR (SD13) raz w miesiącu (1. dnia 04:00 UTC, `memorio-restore-test.timer`): znacznik przed celem, cel, znacznik po celu, przełączenie WAL, czekanie na archiwizację; odtworzenie ostatniej kopii + WAL do celu w osobnym kontenerze (nigdy w żywej bazie); kopia musi mieć pierwszy znacznik, nie mieć drugiego i mieć wszystkie schematy Memorio. Mierzy RPO (znacznik → WAL w buckecie, limit 900 s) i RTO (pobranie + odtwarzanie + sprawdzenie, limit 8 h), raport Markdown w `/var/lib/memorio/restore-reports`, metryki dla monitoringu. Lokalnie: RPO 3 s, RTO 23 s (mała baza) | drugi znacznik dowodzi, że odtworzenie zatrzymuje się dokładnie w celu; PostgreSQL wymaga transakcji po celu, żeby uznać cel za osiągnięty |
| D-169 | Odtworzenie awaryjne `deploy/backup/restore-server.sh [chwila]`: tylko do pustego wolumenu bazy (odmawia, gdy jest w nim baza), całość albo do wskazanej chwili, potem start stosu; po odtworzeniu do chwili — nowa kopia bazowa. Sprawdzane w CI: usunięcie wolumenu, odtworzenie z bucketu, logowanie konta sprzed kopii | RTO ≤ 8 h wymaga procedury przećwiczonej, nie opisanej |
| D-170 | Metryki operacji (kopia, archiwizacja WAL, test odtworzenia) zapisują skrypty w katalogu tekstowym node exportera; alerty: `WalArchiveStale` (> 15 min = RPO), `WalArchiveFailing`, `BackupOverdue` (> 26 h), `RestoreTestOverdue` (> 35 dni), z testami `promtool` | brak kopii ma być alarmem, nie odkryciem w dniu awarii |

Pytania otwarte (PLAT-09): (1) dostawca i rozmiar VPS w UE, sieć prywatna do serwera monitoringu (decyzja PO/budżet; konto i dostęp potrzebne do pierwszej instalacji); (2) bucket kopii — ta sama lokalizacja co serwer czy inna (zalecana inna, odporna na awarię jednego centrum); (3) gdzie przechowywać poza serwerem klucz szyfrowania kopii (bez niego kopie są bezużyteczne) — PLAT-18; (4) rejestr obrazów wydania — PLAT-13.
**Stan testów (2026-10-23):** `tools/deploy/check-stage1.sh` przechodzi lokalnie od czystego stanu: nginx (TLS, przekierowanie, logowanie wymagane, zamknięta gotowość workera, nginx po ponownym utworzeniu API), rejestracja konta, kopia bazowa, test PITR z raportem i metrykami, status archiwizacji, odtworzenie po utracie wolumenu i logowanie konta. Shellcheck czysty, testy reguł `promtool` przechodzą. `provision.sh` nie był uruchomiony na prawdziwym VPS (brak serwera).

## MEM-E0-PLAT-13 · Wdrożenie jedną komendą + lint migracji expand/contract (2026-10-23)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-171 | Wydanie = tag `v<wersja>`. Job CI `release` (tylko na tagu i dopiero gdy wszystkie inne joby przeszły) buduje i publikuje `memorio-api`, `memorio-worker`, `memorio-postgres` `:<wersja>` w GitHub Container Registry (prywatnie; kod już jest na GitHubie) z SBOM i provenance (D-052). Serwer pobiera je raz zalogowany tokenem tylko do odczytu (PLAT-18) | ten sam obraz na każde środowisko (RD2); żadnego obrazu bez zielonego CI |
| D-172 | SD18: test architektury `MigrationLintTests` czyta operacje `Up` każdej migracji EF Core modułów i odrzuca: usunięcie tabeli, kolumny, schematu, sekwencji, klucza głównego; zmianę nazwy tabeli i kolumny; kolumnę wymaganą bez wartości domyślnej; zawężenie kolumny (wymagana z opcjonalnej, inny typ, krótsza); surowy SQL. Wyjątek tylko z `[ContractMigration("powód")]` (`Memorio.Modularity`) — migracja kontrakcyjna w wydaniu po tym, w którym kod przestał używać usuwanego elementu. Każda reguła udowodniona na migracjach wzorcowych | w trakcie wdrożenia i po wycofaniu poprzednia wersja działa na zmigrowanej bazie (D-041); bazy się nie cofa |
| D-173 | Jedna komenda: `tools/deploy/deploy.sh <wersja>` (albo workflow `deploy`, ręczny, ze środowiskiem GitHub i sekretami SSH; produkcja może wymagać zatwierdzenia) → SSH → `deploy/release.sh <wersja>` na serwerze: repozytorium w tagu, nazwany punkt odtworzenia `before-<wersja>` (czas w `/var/lib/memorio/deployments.log`), pobranie obrazów, `compose up` (ok. 30 s bez API), sprawdzenie (gotowość API i workera w sieci, API przez nginx i TLS); porażka → automatyczny start poprzedniej wersji (ok. 4 min łącznie, limit startu 120 s), wpis w dzienniku, metryka `memorio_deployment_info`. Wersja sprawdzana wzorcem przed SSH (bez wstrzyknięcia w powłokę serwera) | D-041: krótki restart mieści się w 99,5%; wycofanie bez człowieka; przy wzroście przestojów — wyzwalacz etapu 2 (D-059) |
| D-174 | `release.sh` uruchamia się z kopii (checkout tagu może podmienić sam skrypt); obraz `memorio-postgres` ma wersję wydania, więc wdrożenie restartuje też bazę (kilka sekund) | prostota: jedna wersja na wszystkie obrazy; przy wolnym starcie bazy — osobna wersja obrazu bazy |

**Stan testów (2026-10-23):** `MigrationLintTests` (5) przechodzą, w tym na migracji Konta; `tools/deploy/check-release.sh` lokalnie: wdrożenie wersji „next” (31 s), wydanie z niestartującym API odrzucone i powrót do „next” (217 s), dziennik i metryka; shellcheck i actionlint czyste. Job `release` i workflow `deploy` nie były uruchomione (brak tagu i serwera).

## MEM-E0-PLAT-18 · Sekrety etapu 1: pliki poza repo, kopia sops/age, rotacja bez przestoju (2026-10-25)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-175 | Trzy pliki env serwera (`.env`, `postgres.env`, `app.env`) zostają w `/opt/memorio/deploy`, ignorowane przez git, właściciel root i tryb 0600 (`provision.sh`); „użytkownik usługi” to root, bo Compose, timery systemd i `release.sh` (przez `sudo`) działają jako root. CI (`tools/secrets/check.sh`) pilnuje, że żaden jawny plik env poza `deploy/local/` (publiczne wartości testowe) nie trafi do repo | bez nowego użytkownika i bez zmiany ścieżek w Compose i skryptach PLAT-09/13 |
| D-176 | Kopia w repo: `deploy/secrets/<środowisko>/{compose,postgres,app}.sops.env`, sops 3.13.3 + age 1.3.2 (wersje i SHA-256 przypięte w CI i `provision.sh`), format dotenv: nazwy zmiennych jawne (przegląd zmian), każda wartość zaszyfrowana. Odbiorcy w `recipients.txt` (klucze publiczne age osób, które mogą odtworzyć środowisko). Klucz prywatny **nigdy na serwerze**: odszyfrowanie na komputerze osoby, `scp` plików; serwer szyfruje (do tego wystarczą klucze publiczne) po rotacji. `secrets.sh check` porównuje kopię z plikami `*.example` | RD10 bez nowej usługi (D-061); wyciek repo nie ujawnia sekretów; kopia się nie rozjedzie z przykładami |
| D-177 | Rotacja `deploy/secrets/rotate.sh`: nowa wartość w pliku env → odtworzenie API i workera (kilka sekund, jak wdrożenie, D-041) → czekanie na gotowość obu → przy porażce powrót starej wartości → sprawdzenie, że stara wartość jest odrzucana. Klucz magazynu: nowy klucz sprawdzany na buckecie **przed** przełączeniem (klient S3 jako usługa `storage-cli` w profilu `operations`), stary zapisany do `storage-revoked`, który wymaga 403 po usunięciu go u dostawcy. Hasło roli: losowe (`openssl rand -hex 24`), w `postgres.env` i w connection stringu roli; hosty restartowane tylko, gdy rola jest w `app.env` | jedna procedura na każdy sekret, sprawdzalna skryptem; PostgreSQL ma jedno hasło na rolę, więc otwarte połączenia trwają, a nowe idą już z nowym hasłem po odtworzeniu hostów |
| D-178 | Wyłącznik awaryjny sesji: `rotate.sh data-protection` usuwa klucze Data Protection (`account."DataProtectionKeys"`), hosty tworzą nowe; tokeny dostępu i odświeżania (chronione tymi kluczami) przestają działać, wszyscy logują się ponownie | jedyny sposób odwołania wszystkich tokenów naraz; bez osobnej listy odwołań |
| D-179 | Hasła ról ustawia jeden skrypt `deploy/postgres/30-role-passwords.sh` (pierwszy start bazy, rotacja, `apply-passwords`; także hasło właściciela z `POSTGRES_PASSWORD`), wartości przez środowisko `docker compose exec -e`, nigdy w linii poleceń. `restore-server.sh` po zakończeniu odtwarzania ustawia hasła z `postgres.env` | odtworzenie do chwili sprzed rotacji przywraca stare hashe haseł; bez tego hosty nie wejdą do bazy |

Pytania otwarte (PLAT-18): (1) kto trzyma klucze age dla produkcji (rekomendacja: dwie osoby, każda w swoim menedżerze haseł; bez tego `deploy/secrets/production/` nie powstanie); (2) sekrety serwera monitoringu (hasło Grafany, webhook alertów) — runbook je wymienia, kopia sops powstanie razem z instalacją tego serwera.
**Stan testów (2026-10-25):** `tools/secrets/check.sh` przechodzi w kontenerze Linux (pliki env poza repo, runbook obejmuje każdą zmienną z `*.example`, kopia sops/age w obie strony, tryb 0600, obcy klucz nie odszyfrowuje; `secrets.sh check` wyłapuje brak zmiennej i wartość jawnym tekstem). `tools/deploy/check-secrets.sh` lokalnie po `check-stage1.sh`: klucze Data Protection (token sprzed rotacji → 401, ponowne logowanie działa), hasło `memorio_account` (hosty gotowe, stare hasło odrzucane przez sieć — gniazdo i 127.0.0.1 obraz przepuszcza bez hasła), klucz magazynu A → B (link podpisany A działa do usunięcia A, `storage-revoked` nie przechodzi, póki A istnieje, potem link A → 403, link B → 200, gotowość API), powrót haseł przez `apply-passwords`. Shellcheck i actionlint czyste. Kopia produkcyjna nie istnieje (pytanie 1).

## Szkielety modułów E0 (`MEM-E0-*-01`) · zamknięte przez PLAT-01, PLAT-03 i PLAT-16 (2026-10-25)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-180 | Dwanaście ticketów szkieletów (`ARCHIWUM`, `ALBUM`, `DOSTEP`, `POCHODZENIE`, `MIEJSCA`, `LIMITY`, `PLANY`, `NARRACJA-AI`, `MODERACJA`, `KONTO`, `SUKCESJA`, `POWIADOMIENIA` `-01`) uznane za zrobione bez osobnego kodu: projekt modułu + `Contracts` + `IModule` dla każdego z 14 modułów (PLAT-01), schemat i rola na moduł oraz test izolacji ról na wszystkich schematach (`ModuleSchemaIsolationTests`, 42501; PLAT-03), reguły architektury SD1/SD2 obejmujące każdy moduł z fixture'ami naruszeń (`ModuleRulesTests`; PLAT-01), szablon warstw Onion i moduł wzorcowy (PLAT-16). Foldery warstw i typy kontraktu powstają z pierwszym ticketem z kodem modułu | puste foldery nie istnieją w git, a pusty kontrakt niczego nie sprawdza; każde kryterium akceptacji szkieletu ma już test w CI |

## MEM-E0-PLAT-21 · Pule połączeń, max_connections, alerty (2026-10-25)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-181 | **Jedna pula na rolę i proces.** Messaging: jedno `NpgsqlDataSource` („messaging”) dla Wolverine (`PersistMessagesWithPostgresql(dataSource)` + `EnableMessageTransport`), metryk zaległości i gotowości — wcześniej Wolverine budował własne źródło obok `MessagingDatabase`, więc rola miała dwie pule w procesie (sprawdzone w kodzie Wolverine 6.51.3). Moduły: EF Core współdzieli źródło danych connection stringu; pula nazwana schematem (`ConfigureDataSource`) dla etykiet metryk. Wzorzec Bulkhead w katalogu | sufit puli z connection stringu jest wtedy prawdziwym sufitem roli w procesie, a suma sufitów daje się policzyć |
| D-182 | **Wymiarowanie:** suma (`Maximum Pool Size` × 2 procesy, bo API i worker czytają ten sam `app.env`) ≤ `max_connections` − 20 (3 sloty superużytkownika dla skryptów właściciela, kopie, administrator, zapas). Teraz: messaging 20, account 10, scheduling 15 (10 wątków Quartz + wątki klastra) → 90 z 180; nowy moduł domyślnie 5. `Timeout=5` (s): szybki błąd zamiast 15 s czekania. PostgreSQL: `max_connections=200`, `shared_buffers=1GB`, `effective_cache_size=3GB`, `work_mem=4MB`, `maintenance_work_mem=256MB` jako zmienne `.env` dla serwera 4 GB (lokalnie mniej pamięci). CI (`check-connection-budget.sh`) wymaga jawnego `Maximum Pool Size` i `Timeout` w każdym connection stringu (env serwera, lokalny etap 1, `compose.yaml`) i pilnuje sumy. Gdy suma przestanie się mieścić: większe `max_connections` przy większym RAM albo PgBouncer (połączenia Wolverine w trybie session — advisory locki) | serwer nigdy nie odmawia połączenia (53300), dopóki pula ma miejsce; przekroczenie budżetu wychodzi w CI, a nie na produkcji |
| D-183 | **Widoczność:** `connections-status.sh` co minutę (timer systemd, właściciel przez gniazdo — działa także przy pełnym serwerze dzięki slotom superużytkownika) → `memorio_postgres_connections{role}` i `memorio_postgres_max_connections` → alert `DatabaseConnectionsHigh` (> 80% przez 2 min). Pule hostów: miernik `Npgsql` w OpenTelemetry (`db.client.connection.count/max`, `…npgsql.pending_requests`, `…npgsql.timeouts`) → alert `ConnectionPoolExhausted` (timeout puli w 5 min). Bez postgres_exportera | bez nowej usługi i nowej roli z uprawnieniami monitoringu; wzór jak status archiwum WAL |

Poza zakresem (bez zmian): PgBouncer; limit współbieżności żądań w API — po pierwszym prawdziwym teście obciążenia.

**Stan testów (2026-10-25):** `ConnectionPoolTests` (prawdziwy PostgreSQL z bootstrapem): pula modułu album wyczerpana → kolejne żądanie po 1 s `connection pool has been exhausted` i jeden timeout w metryce tej puli, a pula places w tym czasie daje połączenie od razu; testy bazy (75), messagingu (8, Wolverine na wspólnym źródle danych), modułu Konto (19), API (46) i architektury (49) przechodzą; `promtool test rules` z nowymi testami przechodzi; `check-connection-budget.sh` przechodzi i wyłapuje brak ustawień oraz przekroczenie. `connections-status.sh` sprawdza CI (`check-stage1.sh`); lokalnie pełny etap 1 nie był uruchomiony (brak pamięci). Test obciążenia na serwerze etapu 1 z ticketu zastąpiony testem pul na poziomie bazy — sprawdza tę samą własność (przegroda między modułami) bez stawiania całego serwera. Nazwa puli Quartz to connection string bez hasła (Quartz tworzy połączenia sam).

## MEM-E0-PLAT-20 · Dead letters: skrypt, runbook, test ponowienia (2026-10-26)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-184 | `deploy/messaging/dead-letters.sh` (`list`, `show`, `replay <id>\|--all`, `discard <id> --reason`) uruchamia pliki SQL z `deploy/messaging/` przez psql właściciela, a każdy plik zaczyna się od `SET ROLE memorio_messaging` (zmiany w schemacie `wolverine` tylko tą rolą, D-134). Ponowienie = `replayable = true`; wiersze przenosi do inboxu agent trwałości workera (Wolverine 6.51.3, `MoveReplayableErrorMessagesToIncomingOperation`, co kilka sekund), więc skrypt czeka, aż znikną, a potem 90 s (czas prób handlera) i zgłasza sukces tylko, gdy żaden nie wrócił. `discard` zapisuje kto, co i dlaczego w `/var/lib/memorio/operations.log`. Test `DeadLetterTests` wykonuje **te same pliki SQL** (`:'id'` → parametr), więc sprawdza dokładnie polecenia skryptu; host testowy ma skrócone ponowienia (parametr `retryDelays` jak w Quartz). Alert `MessageDeadLettered` ma `runbook_url` do `docs/runbooks/dead-letters.md` | decyzja człowieka zamiast automatycznego ponawiania (poza zakresem); bez ręcznego SQL na produkcji; skrypt i test nie mogą się rozjechać |
| D-185 | **Agent trwałości Wolverine tylko w workerze** (`Durability.DurabilityAgentEnabled = false` w API). W klastrze agent bazy (odzyskiwanie inboxu, przenoszenie ponowionych dead letters, nieodesłane wiersze outboxu) trafiał do dowolnego węzła, także API; API nie słucha kolejki `integration_events`, więc wiersze inboxu bez właściciela (ponowione dead letters, wiadomości po padnięciu workera) zostawały w nim na zawsze, z ostrzeżeniem w logu. Wykryte testem `DeadLetterTests` (Wolverine 6.51.3: martwa wiadomość ma `received_at = postgresql://integration_events/`, a po ponowieniu `Incoming/0` i agent na węźle API). Nieodesłany outbox API odsyła worker przez tę samą kolejkę PostgreSQL | bez tego `replay` nie działa, a odzyskiwanie po awarii workera zależy od przypadku przydziału agenta |

**Stan testów (2026-10-26):** `DeadLetterTests` (na plikach SQL skryptu): handler pada → dead letter → `list.sql`/`show.sql` pokazują zdarzenie, handler i wyjątek → naprawa → `replay.sql` → handler obsługuje zdarzenie raz, dead letters tego handlera 0; `discard.sql` usuwa bez obsłużenia. `promtool check/test rules` z `runbook_url`, shellcheck czysty. Na lokalnym serwerze etapu 1 skrypt nie był uruchomiony (brak pamięci; w aplikacji nie ma jeszcze handlera, który mógłby paść).

## MEM-E0-PLAT-14 · Macierz dostępu: uprząż testowa i port odczytu Dostępu (2026-10-27)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-186 | **Wariant A (wybór użytkownika 2026-10-27):** PLAT-14 daje uprząż, a nie reguły. `AccessMatrix` (`Memorio.Testing`): aktorzy × zasoby, każda para rozstrzygnięta `Sees`/`DoesNotSee` (`Rows()` pada na parze nierozstrzygniętej, rozstrzygniętej dwa razy albo z nieznaną nazwą), każdy wiersz to osobny przypadek teorii. `AssertRowAsync` sprawdza odczyt pojedynczy **i** listę/wyszukiwanie; `AssertRevokedAsync` sprawdza, że po odebraniu dostępu już następne zapytanie go nie pokazuje (C9, bez czekania i bez cache). Moduł opisuje swój scenariusz jako `IAccessWorld` na prawdziwym PostgreSQL. Dowód w `Memorio.Modules.Access.Tests`: sondy (granty w schemacie `access` rolą `memorio_access`, albumy w `album` rolą `memorio_album`, filtr `= ANY` w SQL wyszukiwania) i testy, że uprząż pada na wyszukiwaniu bez filtra Dostępu, na odczycie bez pytania Dostępu, na buforowanej odpowiedzi i na nierozstrzygniętej parze. Prawdziwe wiersze (właściciel, uczestnik, link, odebrany, zablokowany, ukryty zakres) dopisują tickety E3/E4/E8 | reguł nie zgadujemy bez ADR 0011 (nie ma go w repo); tickety E3/E4 dostają gotowe narzędzie, a uprząż jest sprawdzona na błędach, które ma łapać |
| D-187 | **Port `IAccessReader`** w `Access.Contracts`: `CanSeeAlbumAsync(personId, albumId)` (pojedynczy odczyt, podpis adresu mediów) i `VisibleAlbumsAsync(personId)` (filtr do własnego SQL listy/wyszukiwania, pobierany przy każdym zapytaniu; tak D-031 godzi „filtr w tym samym zapytaniu” z rolą modułu, która nie czyta cudzego schematu, ADR 0004). Do czasu E4 implementacja odmawia wszystkiego (brak reguły = brak dostępu). Identyfikatory agregatów innych modułów przechodzą przez kontrakty jako `Guid`: silnie typowane id zostają wewnątrz modułu-właściciela, a kontrakt Dostępu nie zależy od kontraktów Albumu i Konta. Granulacja: album; ukryty zakres (E8) i treści osoby (E6) rozszerzą port w swoich ticketach | jedno źródło odpowiedzi (D-035) bez RLS i bez uprawnień między schematami |

Poza zakresem: reguła architektury „każde zapytanie o cudze treści idzie przez Dostęp”. Statycznie nie da się odróżnić zapytania o cudze treści od zapytania o własne; pilnuje tego macierz, wymagana dla każdego takiego zapytania (README modułów).

**Stan testów (2026-10-27):** `Memorio.Modules.Access.Tests` 13/13 (6 wierszy macierzy, odebranie dostępu, 4 celowe błędy wykryte, domyślna odmowa portu), testy architektury 49/49 (SD1/SD2 obejmują nowy kontrakt), `dotnet format` czysty.

## MEM-E0-PLAT-15 · Księga uprawnień w Planach, pierwszy adapter: źródło ręczne (2026-10-27)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-188 | **Wariant A (wybór użytkownika 2026-10-27).** Księga `plans.entitlement_entries` (EF Core, migracja `InitialEntitlementLedger`, kolumny snake_case, bo czytają ją też skrypty): wiersz = zasób (konto albo album) ma uprawnienie do `valid_until`, jak źródło powiedziało w `occurred_at`. Tylko dopisywanie; klucz (`source`, `source_reference`) = powiadomienie źródła, więc to samo powiadomienie dwa razy daje jeden wiersz (`RecordEntitlement` zwraca wtedy `false`). Reguła (`EntitlementRules`): z każdego źródła liczy się wiersz najpóźniejszy u źródła (spóźnione powiadomienie nie cofa nowszego, S2:C3; zwrot lub anulowanie kończy to, co źródło dało), zasób ma uprawnienie do najpóźniejszego z nich. `source` to strumień, którego ostatnie słowo się liczy: adapter, którego źródło ma kilka subskrypcji jednego zasobu, nazywa źródło z identyfikatorem subskrypcji. Cykl planu (AG8, E7) czyta wyłącznie `PlanEntitlements` (księga); adaptery leżą w `Infrastructure`, więc reguła SD2 (Domain/Application nie znają Infrastructure) pilnuje, że cykl nie zna źródeł. **Pierwszy adapter: źródło ręczne** `deploy/plans/entitlements.sh` (`list`, `grant --until`, `end`, zawsze z powodem; wpis w `/var/lib/memorio/operations.log`), SQL w `deploy/plans/*.sql` rolą `memorio_plans`; powód wymagany także przez `CHECK` w bazie. Adapter sklepów (bezpośrednio albo przez pośrednika) w E7, po sprawdzeniu warunków (D-078) | wybór dostawcy płatności nie jest zgadywany; źródło ręczne przyda się od razu (testerzy, reklamacje, zwroty) i dowodzi, że nowe źródło nie zmienia ani księgi, ani cyklu |
| D-189 | `Plans__ConnectionString` wymagany w API i workerze (pula 5, `Timeout=5`), zdrowie `plans-database` w gotowości, migracje przy starcie. Lokalny PostgreSQL z `compose.yaml` dostaje `max_connections=200` jak serwer (z pulą Planów suma 85 przekroczyła domyślne 100 − 20); `check-connection-budget.sh` czyta tę wartość z `compose.yaml`. Na serwerze trzeba dopisać zmienną do `app.env` przed wydaniem (deploy/README, „Manual plan entitlements”) | bez tego hosty odmawiają startu (RD1), co jest właściwe, ale musi być zapisane |

**Stan testów (2026-10-27):** `Memorio.Modules.Plans.Tests` 10/10: reguły księgi bez bazy (brak wiersza, spóźnione powiadomienie, zwrot, dwa źródła, walidacja) i na prawdziwym PostgreSQL z migracją rolą modułu: `grant.sql` skryptu daje to, co czyta cykl planu, `end` kończy plan, `list.sql` pokazuje wiersze, wiersz ręczny bez powodu odrzucony przez bazę (23514), to samo powiadomienie dwa razy = jeden wiersz, spóźnione powiadomienie nie cofa nowszego. Shellcheck czysty, budżet połączeń mieści się (100 z 180 na serwerze, 85 z 180 lokalnie).

## MEM-E0-PLAT-05 · Spike mapy: MapLibre + PMTiles we Flutterze (2026-11-02)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-190 | Biblioteka **`maplibre_gl` 0.27.1** (przypięta, D-096), nie `maplibre` 0.3.7+1. Obie renderują MapLibre GL JS na webie i MapLibre Native na urządzeniach. `MapPage` dostaje widok przez `mapViewProvider` (domyślnie wyjątek, nadpisanie w `app/composition.dart`, w testach pusty widget), punkty przez `mapPointsProvider` (pusto do MEM-E2-ARCHIWUM-08). Sam ekran S03 zostaje w MEM-E2-PLAT-01 | `maplibre_gl`: PMTiles na trzech platformach, klastry z `getClusterExpansionZoom`, własne źródło JS. `maplibre`: 0.3.7+1 to ostatnie planowane wydanie linii 0.3, warstwa natywna ma zostać wymieniona. Widok mapy nie renderuje się w testach widgetów, stąd provider |
| D-191 | **Globus tylko na webie.** Android i iOS pokazują płaską mapę (Mercator), bo MapLibre Native nie renderuje globusa: `setProjection` rzuca tam `UnsupportedError`, a globus w silniku to otwarty PR (maplibre-native #4533, stan 2026-11-02). Kod ustawia projekcję tylko pod `kIsWeb`; gdy silnik ją dostanie, wystarczy zdjąć warunek i podbić wersję | wynik spike'a; **PO 2026-11-02: płaska mapa na telefonach przyjęta** do czasu globusa w MapLibre Native, mimo nazwy ekranu S03 „Mapa i globus” |
| D-192 | **Pomiar:** `app/benchmark/map_benchmark_main.dart` (prawdziwa aplikacja na zakładce mapy, 10 tys. punktów przy 40 miejscach, stałe ziarno) + `tools/map-benchmark/run.mjs` (headless Chrome przez protokół DevTools, bez zależności; serwer z gzip i zapytaniami zakresowymi jak nginx). **QS3/SD8 „pierwszy obraz”** = od otwarcia ekranu mapy do chwili, gdy na ekranie są jednocześnie podkład i punkty, zasoby w pamięci podręcznej; czas do `idle` i zimny start raportowane obok. **Próg 2 s obowiązuje tylko z `--gpu`.** Job CI `map` nie ma GPU (renderowanie programowe, ok. 5 FPS), więc pilnuje regresji progiem 15 s, a poza tym: SD3 (pierwsza wizyta ≤ 3,5 MB gzip, biblioteki mapy ≤ 0,35 MB gzip; zmierzone 3,14 i 0,30 plus margines, bo ticket nie podaje liczb), punkty narysowane, podkład przez odpowiedzi 206, brak obcych hostów, globus, brak błędów strony | czasu z renderowania programowego nie wolno porównywać z wymaganiem; rozmiar, prywatność i działanie da się sprawdzać w każdym CI |
| D-193 | **Wszystko z własnego hosta (D-032):** MapLibre GL JS 6.4.1 i pmtiles 4.5.0 w `app/web/vendor/maplibre` (`memorio-map.mjs` dokłada protokół `pmtiles://`; ładowane dopiero przy otwarciu mapy), budowanie web zawsze z `--no-web-resources-cdn`. Domyślnie wtyczka bierze JS z unpkg, a Flutter silnik z gstatic, więc te hosty dowiadywałyby się o otwarciu aplikacji. Podkład testowy: zoom 0–3 z prawdziwej kompilacji świata (2,4 MB, `make-fixture.mjs`), w repo, żeby CI nie zależało od sieci. Serwer aplikacji web musi podawać `.mjs` jako JavaScript i obsługiwać `Range` dla `.pmtiles` | prywatność sprawdzana testem, nie obietnicą |

Rozstrzygnięte przez PO (2026-11-02): płaska mapa na telefonach (D-191); **QS3 na webie, p95 2,11 s przy celu 2 s, przechodzi do strojenia w MEM-E2-PLAT-01**, cel bez zmian (około połowy czasu mija przed dodaniem punktów, na import biblioteki i styl, więc `preWarm` albo wcześniejsze ładowanie JS to pierwszy kandydat); **sprawdzenie Androida i iOS przesunięte na później**, spike zmergowany bez niego. Pytania otwarte (PLAT-05): (1) **prawdziwy podkład:** źródło, maksymalny zoom, rozmiar i hosting w UE (D-057), test obejmuje tylko zoom 0–3; (2) **Android i iOS:** patrz stan testów; do tego czasu kryterium E0 pozostaje nieodhaczone.

**Stan testów (2026-11-02):** Pomiar lokalny, laptop z 8 GB RAM (ok. 1–2 GB wolne), bez opóźnień sieci, 20 przebiegów, GPU: **pierwszy obraz p50 1,66 s, p95 2,11 s: cel QS3 (2 s) niespełniony o ok. 5%**; do `idle` p50 1,76 s, p95 2,61 s; zimny start do punktów p50 2,58 s, maks. 2,87 s; ruch kamery 60 FPS, najgorsza klatka 50 ms. Tryb CI (`--runs=5 --check`, renderowanie programowe): wszystkie kontrole przechodzą lokalnie, pierwszy obraz p95 4,1 s, 5 FPS; **job CI nie był jeszcze uruchomiony na GitHubie**. **Android i iOS niesprawdzone: aplikacji z mapą nie zbudowano ani nie uruchomiono** (brak SDK i urządzenia), więc PMTiles, klastry 10 tys. punktów i QS3 na urządzeniach to na razie tylko deklaracje dokumentacji wtyczki; instrukcja w `app/benchmark/README.md`. Web w Safari i Firefoksie niesprawdzony.

## MEM-E1-KONTO-01 · Zarejestruj konto (C1) (2026-11-02)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-194 | Rejestracja hasłem to komenda `RegisterAccount` z handlerem; Identity za portem `IAccountRegistry`, endpoint tylko tłumaczy wejście i loguje. Kolejność: deklaracja 16+ (E2), potem walidacja wejścia, zapis na końcu. Sam kontrakt HTTP bez zmian | wzorzec Command + Handler, reguły testowalne bez bazy |
| D-195 | „Konto założone” (E1) to zdarzenie domenowe zwracane przez handler, **bez outboxu**: w ticketach E1 nikt na nie nie reaguje. Gdy pojawi się odbiorca, dochodzi zdarzenie integracyjne w tej samej transakcji | wzorzec nie powstaje na zapas |
| D-196 | Równoległe rejestracje tego samego adresu: decyduje unikalny indeks, odpowiedź 409 `Account.EmailTaken`, nie 500. Adres ponad 256 znaków odrzucany walidacją (limit kolumny) | podwójne kliknięcie i ponowienie nie mogą kończyć się błędem serwera; bez obsługi test wyścigu pada |

Poza zakresem, zostaje jak w PLAT-07: logowanie przez Apple/Google zakłada konto własną ścieżką (ta sama reguła `AgeDeclaration`), bez E1. Teksty dla użytkownika należą do ekranu (MEM-E1-PLAT-01), bo `Error.Description` nie jest pokazywany użytkownikowi; aplikacja wybiera komunikat po kodzie.
Pytania otwarte (KONTO-01): **HS-008, PO + prawnik:** kod zakłada samą deklarację 16+, bez weryfikacji wieku; nieustalone, co z kontem osoby, która okaże się młodsza.
**Stan testów (2026-11-02):** `Memorio.Modules.Account.Tests` 33/33 na PostgreSQL w Dockerze, w tym po raz pierwszy lokalnie `SignInTests`; testy architektury 49/49; testy API z kontraktem OpenAPI 46/46. Test wyścigu sprawdzony negatywnie: bez obsługi unikalnego indeksu pada. Zmiana niezmergowana, czeka na akceptację.

## MEM-E1-KONTO-02 · Zaakceptuj regulamin (C2) (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-197 | `POST /v1/account/terms-acceptance` z wersją; przyjmowana tylko wersja aktualna (`TermsOfService.CurrentVersion` = `2026-11`, stała w kodzie), inna daje 400 `Account.TermsVersionNotCurrent`. Wersja i czas w `AspNetUsers` (dwie kolumny dopuszczające null, migracja tylko dodaje), ponowna akceptacja tej samej wersji zachowuje pierwszy czas. `GET /v1/account/me` zwraca `acceptedTermsVersion` i `currentTermsVersion` | zapisujemy, którą treść osoba przyjęła; zmiana wersji wymaga nowej akceptacji |

Założenie do sprawdzenia: **regulaminu jeszcze nie ma**, a `2026-11` to etykieta robocza. Wymuszenie akceptacji przed pierwszą treścią należy do tworzenia wspomnienia (MEM-E2-ARCHIWUM-01). **Stan testów:** handler i test na PostgreSQL przechodzą, testy aplikacji po regeneracji klienta też.

## MEM-E1-ARCHIWUM-02 i MEM-E1-ALBUM-01 · Porty odczytu dla Konta (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-198 | Moduły Archive i Album dostają już teraz **prawdziwe tabele w minimalnym kształcie** (`archive.memories`, `archive.photos`, `album.albums`, `album.album_participants`) z połączeniami `Archive__ConnectionString` i `Album__ConnectionString` (po 5 w puli). Porty czytają je Dapperem (D-126, pakiet Dapper 2.1.66). E2 i E3 dokładają kolumny migracjami rozszerzającymi | E1 wymaga skasowania treści i policzenia albumów, a E2 i E3 są później. Port zwracający na sztywno zera byłby atrapą, której testy niczego nie dowodzą. **Ryzyko do sprawdzenia:** kształt tabel wyprzedza tickety E2 i E3; dotąd brak komend tworzących te dane, więc w działającym systemie liczniki są zerowe |
| D-199 | `IAccountContentReader` zwraca liczby wspomnień i zdjęć autora, `IAccountAlbumsReader`: albumy własne, własne bez następcy i cudze, w których osoba uczestniczy. Bycie następcą albumu nie liczy się jako uczestnictwo, właściciel na liście uczestników liczy się raz. Agregat nazywa się `SharedAlbum`, bo typ `Album` zderzałby się z przestrzenią nazw modułu | porty niosą tylko skutek, bez przyczyny (ADR 0001) |

**Stan testów:** porty sprawdzone na PostgreSQL rolami modułów (przez `AccountContentReaderTests` i `AccountAlbumsReaderTests`), budżet połączeń przechodzi: 120 przy limicie 180.

## MEM-E1-KONTO-06 · Skutki usunięcia konta, zapytanie R2 (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-200 | `GET /v1/account/deletion-impact` składa odpowiedź z portów Archive i Album przy każdym odczycie, bez zapisanej projekcji, więc nie ma czego odbudowywać. Zapytanie nie przyjmuje identyfikatora konta, tylko sesję: osoba nie zapyta o cudze dane. „Czyich” oznacza podział na albumy własne i cudze, **bez nazw właścicieli i bez rozbicia zdjęć na albumy**, bo wkłady dojdą dopiero w E3 | jedno źródło prawdy; kryterium o przebudowie projekcji spełnione przez brak projekcji |

**Stan testów:** macierz osoba × zasób na PostgreSQL przechodzi (`AccountDeletionImpactTests`).

## MEM-E1-KONTO-03 · Zleć usunięcie konta (C3) (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-201 | **HS-003 wg rekomendacji, do potwierdzenia przez PO i prawnika:** albumy własne bez następcy wstrzymują zlecenie (E5, 409 `Account.AlbumWithoutSuccessor`) tylko do wyboru: wskazanie następcy albo zgoda na wygaśnięcie albumów (`consentsToAlbumExpiry`, zapisywana przy oknie). Wybór nie ma limitu czasu, a zgoda jest dostępna od ręki, więc usunięcia nikt nie blokuje | „wybór, nigdy blokada” (decyzja 35), wymóg sklepów i RODO, S3:L19 |
| D-202 | Agregat `AccountDeletion` (AG11), jeden wiersz na konto w `account.account_deletions`. Stan i `sequence` to tokeny współbieżności EF, czyli warunkowy `UPDATE` z D-024. Po cofnięciu kolejne zlecenie otwiera nowe okno z wyższym `sequence`. Ponowne albo równoległe zlecenie w trakcie okna zwraca ten sam termin, bez drugiego zdarzenia | jeden wynik okna; idempotencja bez osobnego klucza |
| D-203 | `POST /v1/account/deletion`, `GET /v1/account/deletion` (termin albo null). E4 trafia do outboxu w transakcji zmiany (`AccountDeletionRequested`), bo termin ustawia dopiero worker (D-157). `AccountDbContext` rejestrowany przez `AddDbContextWithWolverineIntegration`; moduł Konto zależy od `WolverineFx.EntityFrameworkCore` tylko w `Infrastructure` | ADR 0006 |

Pytanie otwarte: czy zlecenie usunięcia ma wymagać ponownego podania hasła. Ticket mówi tylko o potwierdzeniu, więc na razie wystarcza sesja i potwierdzenie na ekranie. **Stan testów:** handler bez bazy oraz na PostgreSQL z outboxem i workerem: pojedyncze, podwójne i równoległe zlecenie daje jedno zdarzenie.

## MEM-E1-KONTO-04 · Usuń konto definitywnie (C4) (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-204 | `AccountDeletion.Complete(sequence, at)` działa tylko dla bieżącego okna, w stanie `Pending` i nie przed jego końcem. Dane logowania (`AspNetUsers`, kaskadą loginy i tokeny) znikają w tej samej transakcji co zmiana stanu. Wiersz `account_deletions` zostaje na stałe: identyfikator, daty i zgoda, bez e-maila | jeden wynik okna; ślad, co się stało z oknem. **Do oceny prawnika:** przechowywanie tego śladu po usunięciu konta |

**Stan testów:** reguły bez bazy oraz na PostgreSQL: równoległe usuwanie ma jednego zwycięzcę, po usunięciu token zwraca 401.

## MEM-E1-KONTO-05 · Cofnij usunięcie konta (C5) (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-205 | `DELETE /v1/account/deletion` (204). Cofnąć można tylko przed końcem okna: w chwili końca odpowiedź to 409 `Account.DeletionAlreadyDecided`, także gdy termin jeszcze się nie wykonał. Ponowne cofnięcie nic nie zmienia i kończy się sukcesem; brak zlecenia to 404 `Account.NoDeletionRequested`. E8 idzie przez outbox | granica okna jest jedna i nie zależy od opóźnienia workera |

**Stan testów:** reguły bez bazy, 10 rund równoległego cofania i usuwania na PostgreSQL: zawsze jeden zwycięzca, a stan konta zgadza się z wynikiem.

## MEM-E1-KONTO-08 · Zdarzenie integracyjne „Konto usunięte” (E7) (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-206 | `AccountDeleted(EventId, OccurredAt, AccountId, Sequence)` w `Account.Contracts`, zapisywane przez outbox w transakcji C4. Bez przyczyny i bez informacji o zgodzie na wygaśnięcie albumów: Album sam wie, które albumy nie mają następcy (MEM-E9-ALBUM-08) | ładunek niesie tylko skutek (ADR 0001) |

**Stan testów:** test kontraktu; zdarzenie dociera do workera na PostgreSQL; nieudany warunkowy zapis nie zostawia ani zmiany, ani zdarzenia. Samo padnięcie procesu po zapisie sprawdzają ogólne testy `Memorio.Messaging.Tests`, nie test tego zdarzenia.

## MEM-E1-KONTO-07 · Polityka P1: po oknie usuń konto (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-207 | E6 to odpalenie terminu `Account.DeletionWindow` (klucz: konto, dane: `sequence`), a nie osobna wiadomość w kolejce. Termin ustawia i zdejmuje handler E4 i E8 w workerze. Oba sprawdzają aktualny stan agregatu, więc spóźnione zdarzenie o wcześniejszym oknie niczego nie zmienia (kolejność z D-013 bez osobnej tabeli pozycji). Zadanie decyduje czasem `ScheduledFor`. Polityki z `IDeadlines` rejestrowane fabryką, bo w API tego portu nie ma | wzorzec Process Manager z katalogu; termin nie zginie po zatwierdzeniu zmiany (D-157); `ValidateOnBuild` w trybie Development nie wywraca API |

**Ryzyko do decyzji:** zadanie po nieudanych ponowieniach (1 min, 10 min, 1 h, D-158) jest porzucane z metryką `given_up`. Konto mogłoby wtedy zostać po 30 dniach mimo obietnicy usunięcia, dopóki ktoś nie zareaguje na metrykę. Rozważyć dzienne sprzątanie zaległych okien; nie dodałem go, bo ticket go nie obejmuje. **Stan testów:** `AccountDeletionRequestedHandlerIdempotencyTests`, `AccountDeletionCancelledHandlerIdempotencyTests`, `AccountDeletionWindowJobIdempotencyTests` na PostgreSQL, z workerem Wolverine i atrapą harmonogramu. Prawdziwy Quartz dla tego zadania nie był uruchamiany; mechanizm terminów sprawdza `Memorio.Scheduling.Tests`.

## MEM-E1-ARCHIWUM-01 · Usuń treści konta (C25) (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-208 | C25 kasuje od razu, z pominięciem 30 dni kosza: najpierw pliki, potem wiersze w jednej transakcji. Awaria magazynu rzuca wyjątek i ponowienie znajduje te same wspomnienia; drugi przebieg nie ma nic do zrobienia. E22 to na razie zdarzenie domenowe zwracane przez handler, bez outboxu: kontrakt integracyjny i jego publikacja należą do MEM-E2-ARCHIWUM-12, a treści i tak jeszcze nie powstają | okno 30 dni dotyczy konta i już minęło; nie wyprzedzam kontraktu z E2 |

**Do pilnowania w E2:** MEM-E2-ARCHIWUM-12 musi objąć też tę ścieżkę, inaczej Pochodzenie (MEM-E6-POCHODZENIE-04) nie dowie się o treściach skasowanych razem z kontem. Treści wniesione do cudzych albumów obsłuży P2 (MEM-E3-ALBUM-19). **Stan testów:** na PostgreSQL z atrapą magazynu: kasowanie, cudze treści nietknięte, ponowienie, awaria magazynu. Prawdziwy magazyn S3 nie był tu użyty; `DeleteAsync` sprawdza `Memorio.Storage.Tests`.

## MEM-E1-ARCHIWUM-03 · Polityka P35: po usunięciu konta usuń treści (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-209 | `AccountDeletedHandler` w workerze woła `DeleteContentWhenAccountDeleted`. Ostatnio zastosowany numer zdarzenia dla konta w `archive.applied_account_events`; starsze jest pomijane (D-013). Archive zależy od `Account.Contracts`, a `AccountDeletionFlowTests` sprawdza całą ścieżkę między modułami | konto usuwa się raz, więc kolejność jest tu czysto zabezpieczająca, ale wymaga jej ticket |

**Stan testów:** `AccountDeletedHandlerIdempotencyTests` i `AccountDeletionFlowTests` na PostgreSQL, z outboxem i kolejkami workera, ale z atrapą harmonogramu i magazynu.

## MEM-E1-PLAT-01 · Ekran S01 „Rejestracja” (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-210 | Na S01 jest też przełącznik „Mam już konto” z logowaniem e-mailem, choć ticket wymienia tylko rejestrację: router wysyła tu każdego niezalogowanego, więc bez logowania nie dałoby się wrócić do konta | ekran z D-106 jest wspólny dla rejestracji i logowania |
| D-211 | Zgodnie z kryterium „Załóż konto” zależy tylko od deklaracji 16+. Regulamin zaznaczony na ekranie zapisywany jest zaraz po założeniu konta; niepowodzenie tego zapisu nie zatrzymuje wejścia do aplikacji, bo regulamin ma być wymuszony przed pierwszą treścią (MEM-E2-ARCHIWUM-01) | S2:C11: akceptacja przed dodaniem pierwszej treści, nie przed rejestracją |
| D-212 | Nowe ekrany mają teksty po polsku wprost w kodzie, a reszta aplikacji jest jeszcze po angielsku (zakładki, S03). Liczby w OpenAPI ASP.NET opisuje jako `integer` lub `string`, więc wygenerowany model Dart ma je jako `dynamic`; repozytorium rzutuje je na `int`. Nie zmieniam globalnych opcji JSON API | kryteria podają polskie etykiety, a mechanizmu tłumaczeń jeszcze nie ma; globalna zmiana objęłaby cały kontrakt. **Do decyzji:** język interfejsu i tłumaczenia |

Poza zakresem: przyciski Apple i Google, bo wymagają natywnych SDK i identyfikatorów aplikacji (backend jest gotowy od PLAT-07), oraz „zapomniałem hasła” (pytanie z PLAT-07). **Stan testów:** testy komponentu pól, wszystkich stanów i przejścia S01 → S03 na atrapach repozytoriów przechodzą. **Ekranu nie uruchamiałem w przeglądarce ani na urządzeniu.**

## MEM-E1-PLAT-02 · Ekran S24 „Konto i prywatność” (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-213 | „Wskaż kontakt spadkowy” jest widoczne, ale wyłączone, a kontakt pokazuje „Nie wskazano”: ticket zależy od MEM-E9-SUKCESJA-01, którego jeszcze nie ma. Wskazanie następcy też jeszcze nie istnieje (E3, E9), więc właściciel bez następcy ma na ekranie działającą tylko zgodę na wygaśnięcie i informację o następcy | E1 idzie przed E9; atrapa przycisku udawałaby działanie. **Kryterium akcji jest przez to spełnione częściowo.** Dokończyć w E9 |
| D-214 | Potwierdzenie i zgoda to pola na ekranie, nie okno dialogowe. Odpowiedź 409 pokazuje komunikat i zostawia zaznaczenia. Dane offline to ostatnia odpowiedź trzymana w pamięci providera przez czas działania aplikacji: **po zimnym starcie bez sieci ekran pokaże błąd z „Ponów”, bo nie ma trwałej pamięci podręcznej** | kryterium „bez utraty danych”; trwała kopia w Drift byłaby przesadą dla jednego ekranu, a baza lokalna nie działa jeszcze na webie (D-115) |

Braki bez ticketu, do decyzji: **przycisk „Wyloguj”**, którego nie wymienia żaden ticket, a `signOut` istnieje w repozytorium. **Stan testów:** testy komponentu pól, akcji, stanów, odpowiedzi 409 i przejść na atrapie repozytorium oraz testy `HttpAccountRepository` na atrapie HTTP przechodzą. **Ekranu nie uruchamiałem w przeglądarce ani na urządzeniu.**

## E1 · Do sprawdzenia przez PO (2026-11-03)

Wszystkie tickety E1 zmergowano do `version/0.1` lokalnie, na polecenie PO: „rób całe E1 i daj w decyzjach, co zrobiłeś, a ja potem sprawdzę”. Nic nie wypchnięto. Decyzje są w sekcjach E1 powyżej (D-194–D-214). Założenia przyjęte bez potwierdzenia:

1. **HS-003** (D-201): albumy bez następcy wstrzymują zlecenie tylko do wyboru, bez limitu czasu. Do potwierdzenia przez PO i prawnika.
2. **HS-008** (pytanie przy KONTO-01): sama deklaracja 16+, bez weryfikacji wieku. Do potwierdzenia przez PO i prawnika.
3. **Tabele Archive i Album** (D-198) wyprzedzają E2 i E3 i będą rozszerzane. Dziś nic ich nie zapełnia, więc w działającym systemie liczniki skutków są zerowe, a kasowanie treści nie ma czego kasować.
4. **Regulamin** (D-197): wersja `2026-11` to etykieta robocza, treści regulaminu nie ma.
5. **Ślad po usunięciu** (D-204): wiersz okna zostaje na stałe. Do oceny prawnika.

Otwarte: ryzyko porzuconego terminu (D-207), brak „Wyloguj” (D-214), język interfejsu (D-212), kanał usuwania dla osoby bez dostępu do konta (pytanie z PLAT-07), ponowne podanie hasła przy usuwaniu (przy KONTO-03). E22 musi objąć ścieżkę usuwania konta w MEM-E2-ARCHIWUM-12 (D-208).

Kryterium ukończenia epiku pozostaje nieodhaczone: wymaga testu na urządzeniu i CI, a żadne z nich jeszcze nie przeszło.

**Stan testów całości (2026-11-03): pełny przebieg nie został ukończony.** System przerwał go z powodu braku pamięci (ok. 0,5–0,9 GB wolnego). Wynik częściowy:

- Przeszły: `SharedKernel`, `Storage`, `AiNarration`, `ArchitectureTests`, `Database`, `Scheduling`, `Modules.Album`, `Modules.Archive`.
- `Api.Tests`: 1 porażka, **błąd z E1**: `/ready` zwracał 503, bo filtr health checków w `MemorioApiFactory` nie znał `archive-database` i `album-database`. Filtr poprawiony, **poprawka nieuruchomiona** z powodu braku pamięci.
- `Modules.Account.Tests`: 1 porażka, `AccountDeletionImpactTests`, limit czasu połączenia Npgsql podczas pełnego obciążenia. Wcześniej, uruchomiony bez reszty rozwiązania, test przechodził. Podejrzenie: brak pamięci; **nie potwierdzone**.
- `Messaging.Tests`: proces zakończony błędem bez nazwanego testu, prawdopodobnie przez przerwanie; **wyniku nie ma**.
- `Modules.Access` i `Modules.Plans`: brak wyniku w logu.

Testy aplikacji: `flutter analyze` czysty, `flutter test` 95/95 przed ostatnim scaleniem. Host API i worker nie były uruchomione jako prawdziwe procesy, więc prawdziwy Quartz z zadaniem `Account.DeletionWindow` i ekrany z prawdziwym backendem pozostają niesprawdzone.

**Stan testów po dokończeniu (2026-11-03):** testy brakujące w przerwanym przebiegu uruchomione po kolei, Release: `Modules.Account` 76/76, `Messaging` 10/10, `Modules.Access` 13/13, `Modules.Plans` 10/10, `Api` 46/46 (z poprawionym `/ready`). Limit czasu w `AccountDeletionImpactTests` się nie powtórzył, ale jego przyczyna nie została potwierdzona. **Znany problem wrócił:** pierwszy przebieg `Api.Tests` zaraz po pełnym buildzie miał porażki, bo hosty nie zbudowały się w limicie 5 minut (m.in. `The_host_starts_with_every_module_registered`); drugi przebieg, bez przebudowy, przeszedł w 27 s. To ten sam objaw co przy PLAT-07 (ok. 1,8 GB wolnej pamięci przed przebiegiem). Przyczyny nie znam, a skoro host czeka aż 5 minut, nie wykluczam zakleszczenia przy starcie zamiast samej powolności. Do zbadania, jeśli wystąpi też w CI.

## Sprzątanie zaległych okien usunięcia konta (MEM-E1-KONTO-07, bez osobnego ticketu, na polecenie PO, 2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-215 | Zadanie cykliczne `Account.DeletionSweep`, codziennie o 03:00 UTC: okna wciąż `Pending` po swoim końcu idą tą samą drogą co termin (P1, C4, `sequence` z wiersza), każde konto w osobnym zakresie. Porażka jednego konta nie zatrzymuje pozostałych, a na końcu oblewa odpalenie, żeby zadziałały ponowienia i metryka `given_up`. Każde znalezione okno to ostrzeżenie w logu z samym identyfikatorem. Częściowy indeks na `window_ends_at` dla `state = 'Pending'` | zamyka ryzyko z D-207: porzucony termin albo zdarzenie w dead letters nie zostawia konta po 30 dniach. Opóźnienie w najgorszym razie to ok. doba |

Alertu na ten log nie dodałem. **Stan testów:** `AccountDeletionSweepJobIdempotencyTests` na PostgreSQL, `AccountDeletionSweepJobTests` bez bazy. Prawdziwy Quartz z tym harmonogramem nie był uruchamiany.

## Wylogowanie na S24 (MEM-E1-PLAT-02, bez osobnego ticketu, na polecenie PO, 2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-216 | Ikona „Wyloguj” na pasku S24, dostępna także bez sieci i gdy dane się nie wczytały: czyści tokeny na urządzeniu i wraca do S01. `accountOverviewProvider` śledzi konto w sesji, więc kolejna osoba nie zobaczy liczb poprzedniej | zamyka brak z D-214; dane w providerze przeżywały zmianę osoby |

**Znane ograniczenia:** wylogowanie jest tylko lokalne: backend nie unieważnia tokenów, więc skopiowany token odświeżający działa do wygaśnięcia (14 dni). **Do pilnowania w E2:** kolejka offline nie jest przypisana do konta; gdy pojawią się w niej operacje, wylogowanie musi je obsłużyć. **Stan testów:** testy komponentu na atrapach. Na urządzeniu i w przeglądarce nieuruchomione.

## Limity: MEM-E3-LIMITY-01 (wciągnięty z E3), MEM-E2-LIMITY-01, -02, -03 (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-217 | E3-LIMITY-01 zrobiony już teraz, bo blokuje tickety E2. `limits.account_pools` (zużycie, limit, numer limitu, limit AI, zużycie i miesiąc AI), księgi `pool_operations` i `ai_operations` z kluczem unikalnym = id treści. Rezerwacja, zwrot i zużycie AI to warunkowe `UPDATE` z księgą w jednej transakcji (D-015, D-016). Pula powstaje przy pierwszym użyciu z limitem planu darmowego: 2 GB, 0 szkiców AI (propozycja z D-078) | E2 potrzebuje puli przed E7. **Skutek do sprawdzenia:** dopóki E7 nie nada planów, „Poproś AI o szkic” zawsze kończy się „limit wyczerpany” |
| D-218 | Reguły AG2 sprawdzane testami na PostgreSQL, a nie testem klasy agregatu: `AccountPool` nie ma metod, bo cała logika to atomowy SQL (D-016). `SetLimitAsync` (tylko nowszy numer) istnieje bez wywołującego do E7-LIMITY-01, bo wymaga go kryterium AG2 | test agregatu w pamięci sprawdzałby kopię reguły, a nie to, co działa |

## Miejsca: MEM-E2-MIEJSCA-01 do -04 (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-219 | Odwołanie do miejsca to tekst: `osm:N:123` albo `own:<uuid>`. Identyfikatory OSM nie wychodzą poza Places inaczej niż w tej postaci. C10 tylko sprawdza miejsce i je zwraca, niczego nie zapisuje; wspomnienie zapamiętuje swoje miejsce. C11 przyjmuje identyfikator z aplikacji (klucz idempotencji) | wzorzec ACL; offline wspomnienie może wskazać miejsce własne, którego serwer jeszcze nie zna |
| D-220 | Podpowiedzi: dopasowanie początku nazwy bez rozróżniania wielkości liter, od 2 znaków, najwyżej 20, własne przed OSM. Pustą `places.osm_places` zakłada `db/bootstrap.sql`, jeśli importu jeszcze nie było (lint migracji zabrania surowego SQL, SD18); import ją zastępuje (D-131). **Każde środowisko musi ponownie uruchomić bootstrap** | podpowiedzi działają bez importu. **Do zrobienia przed danymi produkcyjnymi:** indeks na `lower(name)` musi powstawać przy imporcie, bo `--create` kasuje tabelę; bez niego zapytanie skanuje całą tabelę. Nie sprawdzałem tego na realnym wycinku OSM |

**Stan testów:** `Modules.Places.Tests` na PostgreSQL z PostGIS, w tym macierz osoba × miejsce własne. Same endpointy sprawdza dopiero test na działającym hoście, którego jeszcze nie było.

## MEM-E2-ARCHIWUM-01 · Utwórz wspomnienie (C12) (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-221 | **Wspomnienie ma na razie jednego autora i jest jedną treścią** (stan kosza i E22 dotyczą całego wspomnienia, E23 powstaje razem z E22). Wkłady wielu autorów rozdzielą to w E3, przy rozstrzyganiu HS-019 | HS-019 jest nierozstrzygnięte, a w E2 nie ma drugiego autora. **Ryzyko do sprawdzenia:** E3 będzie musiało rozdzielić wspomnienie na pojemnik i treści migracją rozszerzającą |
| D-222 | Wspomnienie zapisuje kopię miejsca: odwołanie, nazwę i współrzędne. Mapa nie pyta wtedy modułu Places o każdy punkt | **Do opinii prawnej (D-053, ODbL):** nazwa i współrzędne z OSM trafiają do tabel Archive |
| D-223 | Data to dokładność (`Day`, `Month`, `Year`, `Period`) i zakres dni, przesyłane tekstem `yyyy-MM-dd`, bo wygenerowany klient Dart wysyła `DateTime`. Odrzucana jest data zaczynająca się później niż jutro według UTC, bo dzień na urządzeniu może wyprzedzać UTC | filtr mapy działa na zakresach; bez tolerancji osoba w Polsce nie zapisałaby wspomnienia tuż po północy |
| D-224 | Tu wymuszam akceptację regulaminu (D-211): port `ITermsReader` z modułu Konto, 409 `Archive.TermsNotAccepted`. Identyfikator z aplikacji to klucz idempotencji (D-118): ten sam identyfikator i autor to to samo wspomnienie, inny autor dostaje 409 | S2:C11, kolejka offline wysyła tę samą operację wielokrotnie |

**Stan testów:** `CreateMemoryTests` na PostgreSQL, sąsiednie moduły jako atrapy.

## MEM-E2-ARCHIWUM-02 i MEM-E2-NARRACJA-AI-01 · Zdjęcia i szkic AI (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-225 | Zdjęcie: `POST /v1/memories/uploads` daje podpisany adres, aplikacja wysyła plik, `POST /v1/memories/{id}/photos` przenosi go do `photos/<id>/original`, rezerwuje miejsce z kluczem = id zdjęcia i zapisuje wiersz. Pełna pula: plik znika, 409 `Archive.SpaceLimitExceeded`. Cudze i nieistniejące wspomnienie dają tę samą odpowiedź 403 `Archive.NoAccess` | D-015, D-143; nie zdradzamy, czy wspomnienie istnieje. **Znana luka:** padnięcie procesu między przeniesieniem pliku a zapisem wiersza zostawia plik i zajęte miejsce, a ponowienie dostaje `UploadMissing`. Rzadkie; bez sprzątania do decyzji |
| D-226 | Szkic AI jest **synchroniczny** (`POST /v1/ai/drafts`, odpowiedź z tekstem), choć `AiTextService` zakłada przetwarzanie asynchroniczne (ADR 0009). Limit AI zużywany przed wywołaniem dostawcy, z kluczem = id szkicu: porażka dostawcy zostawia zużycie, ale ponowienie pod tym samym id nic nie dolicza. Szkice w `ai_narration.memory_drafts`, żeby ponowienie dało ten sam tekst | **Do decyzji:** ADR 0009; długie żądanie blokuje połączenie. Porzucona po awarii próba kosztuje jeden szkic. Treść szkicu to dane osobowe i zostaje na stałe: **do ustalenia retencja i kasowanie razem z kontem**, którego tu nie ma |
| D-227 | Zgoda na AI jest polem żądania (`consentsToAi`), bez zapisu na serwerze; imiona i nazwy do ukrycia podaje aplikacja. Czego aplikacja nie poda, to dostawca zobaczy | trwałe „Moje zgody” to osobny ticket. **Ryzyko prywatności do sprawdzenia:** imię wpisane w opisie, a niewymienione na liście, trafia do dostawcy |

**Stan testów:** `AddPhotoTests` i `RequestDraftTests` na PostgreSQL z atrapami magazynu, puli i dostawcy. Prawdziwy dostawca AI i prawdziwe przesyłanie pliku nie były użyte.

## Archiwum: opis, kosz, kasowanie, mapa (MEM-E2-ARCHIWUM-03 do -12, 2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-228 | Kosz: stan i `sequence` wspomnienia to tokeny współbieżności, jak w AG11 (D-202). Termin okna ustawia worker z E20 i E24 (`Archive.TrashWindow`), a dzienne sprzątanie `Archive.TrashSweep` (03:30 UTC) łapie okna po terminie, jak D-215. Przywrócić można tylko przed końcem okna; materiał przycięty tylko z opłaconym planem (`MoveToTrashByTrimming`, wywoła go E7) | AG10, D-024, HS-022 |
| D-229 | Jedna droga definitywnego kasowania: `MemoryEraser` (najpierw zwrot miejsca i pliki, na końcu wiersz) dla P3, żądania formalnego (C20) i usunięcia konta (C25). E22 idzie przez outbox ze wszystkich trzech ścieżek, co zamyka D-208 | jedno miejsce, którego nie da się pominąć. **Koszt:** zwrot miejsca przed zapisem wiersza: przy awarii pula na chwilę zaniża zużycie, dopóki ponowienie nie dokończy kasowania |
| D-230 | Opis: tekst z szkicu AI zachowuje znacznik, także po edycji (D-226, AI Act art. 50, K9), a pochodzenie sprawdza port `IAiDraftReader`, nie aplikacja. Wspomnienie w koszu nie przyjmuje zdjęć ani opisu | SD9 |
| D-231 | Zapytania R11, R12 i R15 pokazują **na razie tylko własne treści**. Albumy i reguły dostępu dojdą z E3 i E4 (ADR 0011). Filtry osób i albumów na mapie jeszcze nie działają: backend ma tylko filtr dat | kryterium „własne + z albumów” spełnione częściowo; nie ma jeszcze ani albumów, ani treści w nich. **Do dokończenia w E3/E4**, razem z wierszami macierzy dostępu |
| D-232 | MEM-E2-ARCHIWUM-13 (P5) **nie jest zrobiony**: zależy od zdarzenia E33 z MEM-E10-ARCHIWUM-02 (import zdjęć, E10), którego nie ma | handler bez źródła zdarzenia byłby martwym kodem |

**Stan testów:** `Modules.Archive.Tests` na PostgreSQL z outboxem, w tym równoległe przywracanie i kasowanie. Pula, magazyn i harmonogram w tych testach to atrapy. Prawdziwy Quartz nie był uruchamiany.

## Ekrany E2 · Polskie teksty przez tłumaczenia Fluttera (2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-233 | Teksty aplikacji są w `lib/core/l10n/app_pl.arb`, generowane przez `flutter gen-l10n` (`flutter_localizations`), dostęp przez `context.l10n`. Na razie tylko polski, także w zakładkach, S01, S24 i ekranie aktualizacji. Wygenerowany kod jest w repozytorium, jak klient API | decyzja PO z 2026-11-03, zamyka D-212; kolejny język to kolejny plik `.arb` |

## Ekrany S06, S05 i S04 (MEM-E2-PLAT-04, -03, -02, 2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-234 | S04 to kreator z dwoma krokami: miejsce i data, potem treść. Identyfikatory wspomnienia, zdjęć i nieudanego szkicu AI powstają raz na ekran, więc „Ponów” powtarza te same operacje | idempotencja po UUID z klienta (D-039); ponowienie nie dubluje wspomnienia i nie zużywa drugi raz limitu AI |
| D-235 | Miejsce własne podaje się na razie **wpisując współrzędne**, bez wskazywania na mapie | kryterium S04 tego nie rozstrzyga, a wskazywanie zależy od S03. **Do oceny przez PO:** wpisywanie współrzędnych jest niewygodne dla zwykłej osoby |
| D-236 | „Ostatnio użyte” miejsca, z których S04 korzysta bez sieci, są trzymane tylko w pamięci sesji (10 pozycji). Bez sieci zawsze można dodać miejsce własne, które idzie do kolejki przed wspomnieniem | najprostsze spełnienie kryterium. Po restarcie aplikacji bez sieci lista jest pusta |
| D-237 | W przeglądarce zapis bez sieci jest zablokowany z komunikatem, bo kolejka nie ma tam bazy (D-115) | jawna blokada zamiast utraty danych |
| D-238 | Szczegóły S05 i kosz S06 czytają dane przez dostawców trzymanych przez sesję: bez sieci pokazują ostatnią odpowiedź. Nieudany odczyt czeka na „Ponów”, automatyczne ponawianie Riverpoda jest wyłączone, także w S24 | ponawianie w tle zasłaniało stan błędu i brak dostępu |

**Stan testów:** testy komponentów S04, S05 i S06 z atrapami repozytoriów; kolejka S04 na prawdziwej bazie w pamięci. Znalazły błąd: „Zatwierdź opis” nie włączał się podczas pisania. Nie użyto prawdziwego API, galerii zdjęć ani urządzenia. S04 jest osiągalny z aplikacji dopiero przez S03.

## Ekran S03 Mapa (MEM-E2-PLAT-01, 2026-11-03)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-239 | Punkty mapy wynikają z `memoryPinsProvider` i zostają na mapie podczas odświeżania i po nieudanym odświeżeniu. Bez sieci znane punkty są pokazywane bez komunikatu o błędzie, a przy błędzie serwera z komunikatem i „Ponów” | kryterium stanu offline. **Ograniczenie:** punkty są pamiętane tylko w sesji, więc po restarcie bez sieci mapa jest pusta i pokazuje błąd. Tak samo jak w D-238 |
| D-240 | Filtry osób i albumów są widoczne, ale wyłączone, do czasu E3 i E4 (D-231). Działa filtr dat | kryterium wymaga tych pól, a backend ma tylko daty |
| D-241 | Wspomnienia z kolejki są na S03 **liczone** („Czeka na wysłanie: N”), a nie rysowane jako punkty | zapis w kolejce ma odwołanie do miejsca, bez współrzędnych. Kryterium spełnione w słabszej formie; **do oceny przez PO** |
| D-242 | Operacja odrzucona przez backend jest pokazana na S03 z „Rozumiem”; opuszczenie kolejki odświeża punkty | wcześniej nikt nie mówił o odrzuceniu, więc wspomnienie zapisane offline mogło przepaść po cichu |
| D-243 | Stuknięcie punktu idzie przez `tappedPointProvider` do S03 i otwiera S05; stuknięcie klastra przybliża mapę o 2 poziomy. Identyfikator trafia też do `properties` i wraca przez `promoteId`, bo GL JS gubi tekstowe id obiektów | widok mapy jest osobnym widgetem za `mapViewProvider`, którego testy nie rysują |
| D-244 | Filtr dat, ostatnie miejsca, szczegóły (S05) i kosz (S06) przeładowują się po zmianie konta | **naprawiony błąd:** szczegóły i kosz poprzedniej osoby zostawały w pamięci po wylogowaniu i zalogowaniu kolejnej |

**Stan testów:** `map_page_test.dart` 20 testów, cała aplikacja 165/165. Benchmark w prawdziwym Chrome z GPU, 10 przebiegów: pierwszy obraz p95 **1101 ms** (limit 2000), 60 kl./s, 10 klastrów na ekranie, nic nie wychodzi poza własny adres, brak błędów na stronie. Punkty w benchmarku nie idą przez sieć, więc to nie jest pełny pomiar QS3 z API; poprzedni wynik 2,11 s (PLAT-05) mierzył inny układ, różnica nie dowodzi poprawy.

**Nie sprawdzono:** stuknięcia punktu i klastra na prawdziwej mapie (przeczytałem tylko kod wtyczki), urządzeń Android i iOS, kafelków bez sieci.

**Do oceny przez PO:**
- SD3: pierwsza wizyta to 3,49 MB przy limicie 3,5 MB. Kolejny ekran prawdopodobnie przekroczy budżet.
- Kolejka offline nie jest przypisana do konta: operacje jednej osoby, niewysłane przed wylogowaniem, wyśle po powrocie sieci sesja kolejnej osoby na tym urządzeniu. Nie naprawiam, bo wymaga decyzji, co zrobić z niewysłanymi danymi.
- Operacja, która dostała „spróbuj później”, czeka do następnej zmiany stanu sieci lub startu aplikacji: kolejka nie ponawia się sama przy stałym połączeniu.

## Podsumowanie E2 (2026-11-03)

Wszystkie tickety E2 poza MEM-E2-ARCHIWUM-13 (czeka na E10, D-232) są w `version/0.1` lokalnie, na polecenie PO: „rób E2”, „rób dalej”. PO jeszcze tego nie sprawdzał. Kryterium epiku pozostaje niezaznaczone, bo wymaga CI i testu na urządzeniu.

**Pierwsze prawdziwe uruchomienie API i workera** (procesy z buildu Release, PostgreSQL i magazyn plików z Compose, zapytania HTTP takie jak aplikacji): rejestracja, regulamin (bez niego 409), miejsce własne, wspomnienie (drugi raz z tym samym id bez dubla), zdjęcie przez podpisany adres, opis, mapa z filtrem dat, szczegóły, brak dostępu dla drugiej osoby i bez logowania, podpowiedzi, kosz, przywrócenie, kasowanie definitywne. Wszystko przeszło. Quartz zapisał termin `Archive.TrashWindow` na 30 dni oraz oba codzienne sprzątania. Outbox pusty, brak martwych wiadomości. Jedyne błędy w logu workera to odczyt historii migracji przed jej utworzeniem na świeżych schematach (6 razy), bez skutków.

**Nadal niesprawdzone:**
- aplikacja połączona z prawdziwym API (scenariusz wyżej szedł zwykłym HTTP, nie przez ekrany);
- urządzenia Android i iOS, CI na GitHubie, stuknięcie punktu na prawdziwej mapie;
- prawdziwy dostawca AI, zwrot miejsca do puli poza testami, odpalenie terminu po 30 dniach na prawdziwym harmonogramie;
- `Api.Tests` zawiesza się w pierwszym przebiegu po buildzie (przyczyna nieznana), co może dotyczyć CI.

**Do decyzji PO:** domyślny limit AI to 0 (D-217 do D-232), więc „Poproś AI o szkic” odpowiada każdemu „limit wyczerpany” (potwierdzone w uruchomieniu: 409 `AiNarration.LimitReached`), dopóki plany nie dadzą limitu. Pozostałe: D-235, D-241 i lista w sekcji S03.

## E3: założenia przed startem (2026-11-04)

E3 zależy od rzeczy, których jeszcze nie ma: planu albumu (E7), reguł dostępu (E4) i nierozstrzygniętych pytań. PO: „rób E3”. Niżej odwracalne założenia, do sprawdzenia przez PO.

| D | Decyzja | Dlaczego |
|---|---|---|
| D-245 | Każdy album ma limit **10 osób** (`IAlbumSeatLimit`), liczba z planu albumu w D-080, do czasu cyklu planów z E7 | D-080 nie podaje limitu dla albumu bez planu, a bez płatności każdy album jest darmowy. **Do decyzji PO:** ile osób w darmowym albumie |
| D-246 | Pula albumu to ten sam agregat AG2 co pula konta, pod identyfikatorem albumu (`limits.account_pools.account_id`). Domyślnie 2 GB, tak jak konto, do czasu `MEM-E7-LIMITY-01`; D-080 przewiduje 150 GB dla planu albumu | te same warunkowe zapisy i księga operacji, bez nowej tabeli. Nazwa kolumny `account_id` jest teraz niedokładna |

## Album, powiadomienia i dostęp: backend E3 (2026-11-04)

Dotyczy MEM-E3-ALBUM-01 do -26, -SUKCESJA-01, -POWIADOMIENIA-01 do -10, -ARCHIWUM-01 i -DOSTEP-01. Zrobione w jednej gałęzi, a nie po gałęzi na ticket: to jedna spójna zmiana agregatu AG1, a komputer pozwala na rzadkie budowanie. To odstępstwo od zasady projektu.

| D | Decyzja | Dlaczego |
|---|---|---|
| D-247 | Skład albumu chroni token współbieżności `sequence` z ponowieniem decyzji (3 próby), a nie blokada pesymistyczna z ticketu. Przegrany czyta nowy stan, więc odmowa znaczy „nie ma miejsca”, a nie „ktoś był równocześnie” | ten sam mechanizm co w D-202 i D-228. Kryterium „jedna odrzucona albo stan spójny” sprawdza test 6 równoległych zaproszeń o 2 wolne miejsca. **Odstępstwo od strategii w tickecie, do oceny** |
| D-248 | **HS-019 rozstrzygnięte roboczo:** wspomnienie ma jednego autora, album tylko na nie wskazuje, a zdjęcia dodaje tylko autor (C56). Dodanie do albumu rezerwuje rozmiar wspomnienia w puli albumu, a miejsce w puli autora **nie wraca**, więc to samo zdjęcie liczy się dwa razy. Zdjęcia dodane w albumie liczy tylko pula albumu; przy zabraniu wspomnienia wracają do puli autora, także ponad jej limit | najmniejsza zmiana, bez rozdzielania wspomnienia na pojemnik i treści (ryzyko z D-221). **Do decyzji PO:** czy uczestnicy mają dodawać zdjęcia do cudzych wspomnień i czy podwójne liczenie jest akceptowalne |
| D-249 | Osoba usunięta z albumu dostaje prośbę o wybór, ale jej treści **zostają bez terminu, dopóki sama ich nie zabierze** (C55, także po wyjściu z wyborem „zostaw”). Nie ma automatu po terminie | w repozytorium nie ma długości terminu ani skutku jego upływu, a automatyczne kasowanie byłoby nieodwracalne. **Do decyzji PO i prawnika:** czy treści mogą zostać bez aktywnej zgody |
| D-250 | Dostęp pyta wprost moduł Album przez port `IAlbumMembership`, a nie własną tabelę reguł. **Polityki P29 i P30 z E4 (MEM-E4-DOSTEP-09, -10) są przez to niepotrzebne**, a utrata dostępu działa przy następnym odczycie, bez czekania na zdarzenie | ADR 0011 wymaga jednej odpowiedzi „czy może widzieć”, i to `IAccessReader` nadal ją daje. Tabela reguł dojdzie z linkami i blokadami w E4 |
| D-251 | **Żadna wiadomość nie jest jeszcze wysyłana.** Zaproszenia, przypomnienia i powiadomienia przechodzą całą ścieżkę z księgą `notifications.sent_notifications`, ale `LoggingNotificationSender` tylko zapisuje w logu rodzaj wiadomości, bez adresu | nie wybrano dostawcy e-mail (D-046 wymaga umowy). Osoba zaproszona widzi zaproszenie po zalogowaniu tym adresem, ale nikt jej o tym nie powie. **Przed startem konieczny dostawca** |
| D-252 | **Naprawiony błąd:** handlery rozwiązywały politykę przez wspólny interfejs `IIntegrationEventPolicy<T>`. Gdy na zdarzenie reagują dwa moduły (E7: Archiwum i Album; E42: Album i Powiadomienia), działała tylko polityka zarejestrowana ostatnia, a druga po cichu nie. Handlery tych zdarzeń biorą teraz politykę swojego modułu po typie | byłaby to cicha utrata wycofania wkładów lub kasowania treści konta. **Pozostałe handlery wciąż używają interfejsu:** kolejne zdarzenie z drugim odbiorcą trzeba przełączyć tak samo |
| D-253 | Osobę w albumie rozpoznają inni po adresie e-mail, bo konto nie ma nazwy. Zaproszenie odbiera tylko konto o tym samym adresie, a pełna pula powiadamia właściciela raz dziennie na album | ticket wymaga składu i „kto zaprasza”. **Do oceny pod kątem prywatności** |
| D-254 | `GET /v1/shared-with-me` (R14) jest pusty do czasu linków z E4. Rezerwacja przejęcia albumu bez właściciela z niezmiennika AG1 należy do E9 i nie jest zrobiona. HS-004 (zgoda na AI po wyjściu) dotyczy E6 i dalej czeka na PO. Przypomnienie po 7 dniach nie ma zadania zbierającego zaległe terminy, jak D-215 | zakres E3 bez wymyślania reguł innych epików |

**Stan testów:** Album 53/53 (w tym 6 równoległych zaproszeń i równoległe przyjęcie z wycofaniem), Powiadomienia 11/11, Archiwum 89/89, Dostęp 14/14, Limity 20/20, architektura 49/49, baza 75/75, API 46/46. Testy Konta i pozostałych modułów nie były uruchamiane po tej zmianie. `IAccountDirectory` nie ma własnego testu, sprawdziło go tylko prawdziwe uruchomienie.

**Prawdziwe uruchomienie API i workera** na lokalnej bazie, zwykłym HTTP: album, zaproszenie, miejsca, przyjęcie, wspomnienie w albumie widoczne dla drugiej osoby (szczegóły, zdjęcie i mapa), oznaczenie, następca, blokada adresu, wyjście z utratą dostępu przy następnym odczycie, kasowanie wspomnienia usuwające je z albumu. Wszystko przeszło, brak martwych wiadomości, a worker zaplanował `Album.InvitationReminder`. Nie sprawdzono: pełnej puli, odpalenia terminu po 7 dniach na prawdziwym harmonogramie, usuwania konta w tym scenariuszu.

**Spostrzeżenie o zawieszaniu testów:** pierwsze uruchomienie po buildzie zawiesiło się też w `Album.Tests`, czyli nie jest to tylko `Api.Tests`, ale nie dzieje się za każdym razem. Pojawiło się przy pamięci poniżej 1 GB; związek nie jest dowiedziony.

## Ekrany E3: S09 do S14 (MEM-E3-PLAT-01 do -06, 2026-11-04)

Sześć ekranów w jednej gałęzi, bo mają wspólną warstwę danych. To znowu odstępstwo od zasady jednej gałęzi na ticket.

| D | Decyzja | Dlaczego |
|---|---|---|
| D-255 | **Akcji zależnych od innych epików nie ma na ekranach:** S10 „Zablokuj użytkownika” (E4, C64), S11 „Udostępnij linkiem”, „Unieważnij link”, „Odbierz dostęp” i oglądający przez link (E4), S12 zgoda na AI (E6). Pole „Album bez właściciela” na S10 ma na razie stały tekst „Ten album ma właściciela” (E9) | backend tych akcji nie istnieje, a wyłączony przycisk bez działania niczego by nie dał. Kryteria S10, S11 i S12 są przez to spełnione **częściowo** |
| D-256 | Doszedł `GET /v1/memories/mine`, spoza ticketów: „Dodaj moje wspomnienie” potrzebuje listy z nazwami, a mapa zwraca tylko identyfikatory i współrzędne | bez tego wybór wspomnienia na S10 nie jest możliwy |
| D-257 | S12 otwarty z listy pokazuje wkłady pozostawione po wyjściu, bo z albumu, w którym już się nie jest, nie da się na niego wejść. Otwarty z albumu pokazuje też kuratora. Odwołanie „zostaw” wymaga zaznaczenia potwierdzenia na ekranie, jak w D-214 | wkłady po wyjściu nie mają innego wejścia |
| D-258 | „Nie chcę zaproszeń z GEO” na S14 blokuje adres **i** odrzuca to zaproszenie | inaczej miejsce czekałoby na odpowiedź, której nie będzie |
| D-259 | Dane offline to ostatnia odpowiedź w pamięci, jak D-214 i D-238: po restarcie bez sieci ekran pokazuje błąd z „Ponów”. Akcje albumów nie idą przez kolejkę offline, są wyłączone | kryteria S09 do S14 wymagają tylko odczytu bez sieci |
| D-260 | Stuknięcie wspomnienia na S10 otwiera S05 w zakładce mapy, więc powrót prowadzi na mapę, nie do albumu | S05 jest zagnieżdżony pod mapą (E2). **Niewygodne, do poprawy przy przeglądzie nawigacji** |

**Stan testów:** aplikacja z testami S09 do S14 i mapowaniem odpowiedzi API; wynik końcowy w podsumowaniu E3. Test przeszedł też przez błąd, którego analizator nie widzi: klucze ekranów zawierały znak z generatora.

**Nie sprawdzono:** ekranów połączonych z prawdziwym API, urządzeń, wybierania zdjęć z galerii.

## Podsumowanie E3 (2026-11-04)

Wszystkie 49 ticketów E3 ma wynik w `version/0.1` lokalnie, na polecenie PO: „rób E3”. Część tylko częściowo, zob. statusy w epiku. PO jeszcze tego nie sprawdzał. Kryterium epiku pozostaje niezaznaczone, bo wymaga CI i testu na urządzeniu.

**Testy po ostatnim scaleniu:** aplikacja 217/217, Album 53/53, Powiadomienia 11/11, Archiwum 90/90, Dostęp 14/14, Limity 20/20, Konto 79/79, API 46/46, architektura 49/49, baza 75/75. Moduły Plans, Places i AiNarration oraz `Memorio.Messaging.Tests` i `Memorio.Scheduling.Tests` nie były uruchamiane po tej zmianie.

**Zawieszanie testów:** pierwsze uruchomienie po buildzie stanęło też w `Account.Tests` (po 400 s), w tym samym miejscu co w `Album.Tests`: zaraz po skrypcie startowym bazy. Drugie uruchomienie bez przebudowy przeszło w 72 s. Wygląda to na zjawisko lokalne, ale nie ma potwierdzonej przyczyny.

**Do decyzji PO przed startem:** D-245 (osoby w darmowym albumie), D-246 (pula albumu), D-248 (zdjęcia w cudzych wspomnieniach, podwójne liczenie miejsca), D-249 (treści osoby usuniętej bez aktywnej zgody), D-251 (dostawca e-mail), D-253 (adres e-mail widoczny dla osób w albumie).

## Decyzje PO po E3 (2026-11-04)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-261 | Album bez płatnego planu ma **5 osób**, razem z właścicielem i zaproszonymi. Zastępuje założenie 10 osób z D-245. Dopóki E7 nie odróżnia planów, limit dotyczy każdego albumu; plan albumu ma 10 (D-080). Album, który ma już więcej osób, zostaje, ale nie przyjmie nowych zaproszeń | decyzja PO |
| D-262 | Treści osoby usuniętej z albumu **na razie zostają**, jak w D-249. Pytanie jest w `docs/pytania-do-prawnika.md` | decyzja PO, do opinii prawnika |
| D-263 | Osoby w albumie widzą swoje adresy e-mail, jak w D-253 | decyzja PO. Pytanie o sposób poinformowania o tym jest w `docs/pytania-do-prawnika.md` |

## Miejsce wspomnienia w albumie (MEM-E3-ARCHIWUM-01, MEM-E3-ALBUM-05, 2026-11-04)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-264 | **Decyzja PO, zmienia D-248 w części o miejscu:** po dodaniu do albumu wspomnienie liczy się tylko w puli albumu, a miejsce autora wraca. Gdy ostatni album je wypuści, wraca na konto autora, także ponad limit (nic nowego się wtedy nie zmieści, ale nic nie ginie). W dwóch albumach liczy się w każdym z nich. Zdjęcia nadal dodaje tylko autor (D-248 bez zmian) | plan albumu opłaca wspólne miejsce |
| | **Naprawiony błąd:** klucz rezerwacji raz zwolniony nie liczył się już nigdy, więc wspomnienie zabrane i dodane ponownie nie zajmowało miejsca. Zwolniony klucz liczy się teraz od nowa, także w innej puli | wyszło przy projektowaniu przenoszenia |
| | **Znane ograniczenia:** rozmiar wspomnienia w puli albumu jest ustalony przy dodaniu. Zdjęcie dodane później z S05, a nie z albumu, liczy się autorowi | na razie akceptowalne, bo ktoś za nie płaci |

**Stan testów:** Limity 23/23, Archiwum 93/93, Album 53/53, z prawdziwymi rezerwacjami w `Limits.Tests`. W `Archive.Tests` pula to atrapa, więc całość przeniesienia razem nie była uruchomiona na prawdziwej puli.

## Wysyłka wiadomości (MEM-E3-POWIADOMIENIA-01 do -10, 2026-11-04)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-265 | **Decyzja PO:** dostawcą jest na razie Scaleway Transactional Email, a w przyszłości może AWS. Wysyłka idzie zwykłym SMTP (MailKit, `Notifications__Smtp__*`), a nie przez API dostawcy, więc zmiana na innego, także Amazon SES, to zmiana konfiguracji. Bez `Notifications__Smtp__Host` wiadomości tylko trafiają do logu, tak jak dotąd lokalnie | zamyka D-251. Warunki Scaleway ze stron dostawcy: 300 wiadomości miesięcznie za darmo, potem 0,25 € za 1000, usługa w UE |
| D-266 | Wiadomość zawiera tylko rodzaj sprawy i nazwę albumu, bez wspomnień, zdjęć i imion. Adres trwale odrzucony przez dostawcę (5xx na odbiorcy) jest zapisany w logu bez adresu i nie jest ponawiany. Brak odpowiedzi dostawcy oblewa wysyłkę, więc zdarzenie jest ponawiane | D-038, D-162; ponawianie złego adresu kończyłoby się martwymi wiadomościami |

**Stan testów:** Powiadomienia 22/22, z prawdziwą rozmową SMTP z lokalnym serwerem testowym bez TLS i logowania; architektura 49/49.

**Nie sprawdzono:** Scaleway naprawdę (nie ma konta ani domeny), TLS i logowania, dostarczalności do skrzynek, wysyłki z uruchomionego workera, treści umowy powierzenia. Dopisanie Scaleway do „Podwykonawców” (MEM-E11-PLAT-08) i rejestru czynności (MEM-E11-PLAT-02) czeka na E11.

**Ryzyka przed startem:**
- Nie ma limitu zaproszeń na osobę: limit miejsc dotyczy jednego albumu, a albumów można tworzyć dowolnie dużo, z dowolną nazwą trafiającą do wiadomości. Ktoś mógłby tak wysyłać niechciane wiadomości i psuć reputację domeny.
- Osoba bez konta nie wyłączy zaproszeń bez założenia konta, zob. `docs/pytania-do-prawnika.md`, pytanie 8.
- Wiadomość nie mówi, kto zaprasza; widać to dopiero na S14.

## Limit zaproszeń i zapraszający w wiadomości (MEM-E3-ALBUM-02, MEM-E3-POWIADOMIENIA-01, 2026-11-05)

Zrobione w jednej gałęzi, bo obie zmiany dotyczą tej samej wiadomości z zaproszeniem. Odstępstwo od zasady jednej gałęzi na ticket.

| D | Decyzja | Dlaczego |
|---|---|---|
| D-267 | **Decyzja PO:** limit 10 zaproszeń. PO nie powiedział „na dzień”; przyjęte z propozycji: **10 na dobę UTC na konto, we wszystkich albumach razem**. Ponowne zaproszenie tego samego adresu też się liczy, bo wysyła maila. Odmowa (brak miejsca, zablokowany adres) się nie liczy. Po przekroczeniu 409 `Album.InvitationLimitReached`. Licznik to warunkowy zapis w `album.invitation_quotas`, więc równoległe zaproszenia nie przekroczą limitu. **Znane:** zapis w albumie, który przegra po 3 próbach, zużywa jedno zaproszenie mimo braku wysyłki | zamyka ryzyko wysyłania niechcianych wiadomości z D-265 |
| D-268 | **Decyzja PO, zmienia D-266:** zaproszenie i przypomnienie podają, kto zaprasza. Konto nie ma nazwy, więc jest to **adres e-mail zapraszającego**, ujawniany osobie, której adres wpisał, także bez konta. Inne wiadomości nie podają nikogo | taki mail trudniej wziąć za spam. Pytanie do prawnika 8 uzupełnione |

**Stan testów:** Album 58/58 (w tym 15 równoległych zaproszeń), Powiadomienia 23/23, aplikacja 219/219, baza 75/75, architektura 49/49, API 46/46. **Nie sprawdzono:** prawdziwej wysyłki, bo nadal nie ma konta Scaleway.

## E4: link, oglądający i blokady (MEM-E4-DOSTEP-01 do -11, MEM-E4-PLAT-01, -02, 2026-11-05)

Na polecenie PO: „zaczynaj E4”. Backend i ekrany w jednej gałęzi: to jeden agregat AG3. Odstępstwo od zasady jednej gałęzi na ticket.

| D | Decyzja | Dlaczego |
|---|---|---|
| D-269 | AG3 to `AlbumAudience`: link, lista oglądających i blokady jednego albumu, chronione tokenem `sequence` z ponowieniem, a nie blokadą pesymistyczną z ticketu | ten sam mechanizm co D-247. Testy: równoległe wejścia z unieważnieniem, wejście z blokadą |
| D-270 | Blokada obowiązuje w jednym albumie (D-009). Uczestnik blokuje tylko oglądającego przez link, a osobę w albumie **tylko właściciel**; właściciela nie można zablokować. Blokada działa przy następnym odczycie, a osoba w albumie traci potem miejsce przez zdarzenie E67, jej treści zostają jak po usunięciu (D-262). Zatwierdzone przez PO | ticket mówi „uczestnik blokuje”, ale wyrzucanie się nawzajem przez uczestników obchodziłoby właściciela |
| D-271 | Oglądający przez link widzi wspomnienia i zdjęcia, ale **nie widzi osób w albumie ani ich adresów**, bo D-263 dotyczy osób w albumie. Właściciel i uczestnicy widzą adresy oglądających („imienna lista”, R10); S15 mówi o tym przed wejściem. | D-263, R10 |
| D-272 | **P29 i P30 (DOSTEP-09, -10) nie są potrzebne**, jak w D-250. **P31 (DOSTEP-11) nie jest zrobiona:** czeka na zdarzenie E126 z MEM-E9-SUKCESJA-11. Komenda C65 (DOSTEP-06) jest gotowa jako port `IReadGrants`, jeszcze bez wywołującego | handler bez źródła zdarzenia byłby martwym kodem, jak D-232 |
| D-273 | Link zawiera losowy token (32 bajty) trzymany jawnie w bazie, żeby właściciel mógł go skopiować ponownie. Sam token nie wystarcza: wejście wymaga konta i zapisuje na listę. **Adres w aplikacji mobilnej to na razie sama ścieżka**, bo nie ma domeny aplikacji (`APP_URL`), a otwieranie linków na telefonie nie jest skonfigurowane | prostota; **przed startem potrzebna domena i konfiguracja linków na urządzeniach** |
| D-274 | **Naprawiona luka:** niezalogowany z linkiem trafiał na logowanie, a link przepadał. Teraz wraca do niego po zalogowaniu. Inne miejsca nie są pamiętane, żeby kolejna osoba na tym urządzeniu nie trafiła tam, gdzie była poprzednia | kryterium S15, znalezione testem |
| D-275 | Konto usunięte znika z list oglądających i blokad (poza ticketami) | RODO, jak E1 |
| D-276 | **Decyzja PO, poza ticketami:** blokadę można cofnąć. Robi to tylko właściciel na S11, także gdy zablokował uczestnik. Odblokowanie samo nie daje dostępu: oglądający wraca przez działający link, a osoba z albumu potrzebuje nowego zaproszenia | decyzja PO; właściciel może naprawić pomyłkę |
| D-277 | **Decyzja PO:** na prywatnej mapie (R11) są własne wspomnienia i te z albumów, w których osoba uczestniczy. Album oglądany przez link można otworzyć, ale nie trafia na mapę (`AlbumsTakingPartInAsync`) | jedno wejście przez link nie zaśmieca mapy cudzymi wspomnieniami |
| D-278 | **Naprawione, dotyczy też E1 do E3:** domyślnie Wolverine po pierwszym wysłaniu z outboksu **po cichu odrzuca** to, co opublikuje kolejny zapis w tym samym zakresie (`MultiFlushMode.OnlyOnce`). Wszystkie cztery repozytoria zapisują teraz z `AllowMultiples`, a po odrzuconym zapisie wycofują transakcję, bo PostgreSQL odmawiałby wszystkiego po nieudanej instrukcji. **Zostaje świadomie:** po przegranym wyścigu zdarzenie nieudanej próby wychodzi razem z ponowioną, więc może być duplikat. Handlery są idempotentne (SD6), a `SendOnce` nie wyśle dwóch maili z tym samym numerem | znalezione testem E4. Żadna dzisiejsza ścieżka nie robiła dwóch udanych zapisów ze zdarzeniami, więc nic jeszcze nie zginęło; biblioteka nie ma publicznego sposobu na porzucenie samych wiadomości bez ruszania jej stanu |

**Stan testów:** Dostęp 28/28, Album 65/65, Archiwum 94/94, Konto 79/79, baza 75/75, architektura 49/49, API 46/46, aplikacja 245/245. Znalazły dwa błędy, których atrapy nie pokazałyby: lista oglądających wywalała się na mapowaniu daty oraz D-278.

**Prawdziwe uruchomienie API i workera** na lokalnej bazie, zwykłym HTTP: link, podgląd, wejście, oglądający bez osób i adresów, mapa bez albumu z linku, lista oglądających, odebranie dostępu z powrotem przez link, blokada przez uczestnika i właściciela, odblokowanie, odebranie miejsca przez workera, unieważnienie linku. Wszystko przeszło, bez martwych wiadomości. **Nie sprawdzono:** ekranów z prawdziwym API, otwierania linków na urządzeniach, równoległości poza testami modułu.

**Spostrzeżenie o zawieszaniu testów:** pierwszy przebieg po buildzie staje też w testach API, które nie używają Dockera. Sam silnik Dockera stanął osobno, a PO zrestartował go 2026-11-05. Przyczyna zawieszeń jest nieznana; wszystkie zdarzenia wystąpiły przy około 1 GB wolnej pamięci.

## MEM-E7 · Plany, limity i retencja 24 miesiące (2026-11-05)

PO: „rób E7”. Płatności i plan rodzinny wymagają decyzji PO, więc ich nie zgaduję. Reszta to odwracalne założenia, do sprawdzenia przez PO. Żadne z nich nie zadziała wcześniej niż po 24 miesiącach od końca płatnego planu.

| D | Decyzja | Dlaczego |
|---|---|---|
| D-279 | Okno wyboru materiału to **ostatnie 30 dni przed przycięciem**, a nie 30 dni po 24 miesiącach. Przycięcie następuje dokładnie po 24 miesiącach. **Zatwierdzone przez PO 2026-11-05.** Zapis S2:B1 dało się czytać też jako okno po 24 miesiącach | S2:B1 mówi „24 miesiące pełnego odczytu” i „okienko 30 dni”, bez kolejności. Tak ostrzeżenie „30 dni” otwiera wybór, a obietnica 24 miesięcy pozostaje dotrzymana |
| D-280 | Cyklem porusza jedno zadanie godzinowe `Plans.Cycle`, a nie osobne terminy dla każdego planu. Czyta księgę, ustala tryb, wysyła należne ostrzeżenia, zleca przycięcie. Polityki P13, P14, P17 i komendy C90, C91, C95 spotykają się w `AdvanceCycleHandler`, zdarzeń E90, E91 i E102 nie ma jako osobnych wiadomości. **C100 scalone z C91:** tryb tylko do odczytu konta jest właśnie „nic nowego ponad darmowy limit” (S2:C7). Limit darmowy konta to 2 GB, album w tym trybie ma 0 | wygaśnięcie wynika z daty w księdze, bez żadnego powiadomienia, a wpis ręczny (D-188) omija kod: odpytywanie wyłapuje oba. Nie ma czego odwoływać, gdy ktoś zapłaci, więc odpada wyścig opłaty z terminem (D-006). Wzorzec Process Manager zostaje, z cyklicznym zamiast jednorazowego zadaniem Quartz |
| D-281 | **Nie ma żadnego źródła płatności.** `ISubscriptionSource` ma tylko implementację `NoSubscriptionSource`: przycisk „Przywróć zakupy / odśwież” czyta księgę. Nie ma ekranu zakupu ani webhooka. Plan da się dziś nadać tylko ręcznie (`deploy/plans/entitlements.sh`), i to jedyny sposób uruchomienia płatnej ścieżki. **Wybór źródła: D-291** | wymaga kont i umów PO, a wybór adaptera nie zmienia cyklu (D-078) |
| D-282 | Zlecone przycięcie sprowadza też pulę z powrotem na darmowy limit: Limity słuchają E103, poza E94 i E95 z P27. Album po przycięciu znów działa jak darmowy (2 GB, 5 osób); to, co przekracza limit, leży w koszu i liczy się do puli przez 30 dni, więc do tego czasu nic nowego się nie zmieści | S2:B1 „album zostaje, przycięty do darmowego limitu”. P27 nie wymieniała tego przejścia, a bez niego album zostałby na limicie 0 na zawsze |
| D-283 | **Naprawione, dotyczy E3:** album nie może przyjąć identyfikatora istniejącego konta (`Album.IdTaken`) | identyfikator przychodzi od klienta, a pula i plan albumu żyją pod tym identyfikatorem (D-246). Znając cudzy identyfikator konta, można było zapełnić komuś jego miejsce albo, po wygaśnięciu planu, ustawić mu limit 0. Komentarz „UUID się nie spotkają” był fałszywy wobec złośliwego klienta |
| D-284 | Przycięcie konta bierze wspomnienia autora, za które nie płaci żaden album (D-264). Przycięcie albumu bierze jego wspomnienia, także te z innych albumów, bo wypadają tylko z tego (zmienione przez D-290). Zostaje wybór właściciela, potem najstarsze; co się nie mieści, jest pomijane, a mniejsze po nim zostają. Wspomnienie ze zdjęciami przycinane jest w całości | S2:B1: najstarsze są nieodtwarzalne |
| D-285 | **Dowód doręczenia jest pozytywny:** przycięcie wymaga potwierdzonych wszystkich czterech ostrzeżeń (90, 30, 7, 1 dni). Brak potwierdzenia wstrzymuje przycięcie, także gdy wiadomość utknęła w martwych wiadomościach. Odmowa choćby jednego adresu wstrzymuje przycięcie, reszta dostaje wiadomość. Co miesiąc idzie ponowienie, tylko do osób, do których nie dotarło. Po doręczeniu nowy termin to co najmniej 30 dni i ostrzeżenia, które wypadają po tej dacie, idą ponownie. **Zatwierdzone przez PO 2026-11-05:** jeden martwy adres uczestnika wstrzymuje album bezterminowo, co kosztuje miejsce, ale niczego nie traci. **Niedoręczenie po przyjęciu przez dostawcę (bounce) nie jest obsługiwane:** liczy się odmowa na etapie SMTP. Webhook odbić wymaga konta dostawcy | S2:B1, S2:C10: to jedyna nieodwracalna operacja, a błąd w tę stronę jest najgorszym błędem produktu |
| D-286 | Limity z D-080: darmowy 2 GB bez AI, osobisty 50 GB, album 150 GB, 10 osób. **Limit AI to na razie sama liczba szkiców:** 20 i 60. Podsumowania (4 i 10) dojdą w E6, bo pula ma jeden licznik | D-080 czeka na przegląd, liczby są w jednym miejscu (`PlanTerms`) |
| D-287 | **Naprawione, dotyczy E4:** odwołanie linku mogło przegrać trzy próby z wejściami przez ten link i odpowiedzieć „już rozstrzygnięte”. Wykrył to test równoległy, który wcześniej przechodził. Teraz 8 prób z losowym odstępem | przypadkowo zielony test krył realną słabość |
| D-288 | **Zastąpione przez D-290.** Pierwotnie: wspomnienie przycięte z albumu szło do kosza autora, więc znikało też z jego prywatnej mapy, tak jak mówi HS-022. Wątpliwość: właściciel albumu, który nie płaci, decyduje wtedy o treści uczestnika. Uczestnik dostaje ostrzeżenia. Alternatywa: tylko usunąć z albumu i zostawić autorowi. Dopisane jako pytanie 9 do prawnika | literalnie HS-022 i S2:B1; zmiana jest prosta, zanim cokolwiek zostanie przycięte |
| D-289 | Sukcesja (E9) nie istnieje, więc C165 i oznaczenie albumu bez właściciela (P36) są gotowe jako handlery bez wywołującego, a E171 wychodzi bez odbiorcy. E25 też nie ma jeszcze odbiorcy. E174 scalone z E24: przywrócenie to to samo zdarzenie | MEM-E7-PLANY-23 i -25 mają zdarzenia E162 i E167 dopiero w E9 |

**Bez zmian w kodzie, wymaga PO:** plan rodzinny (HS-012, P2 wg D-069). Nie zrobione: MEM-E7-PLANY-07, -08, -09, -19, MEM-E7-PLAT-02, -04. **Decyzja PO 2026-11-05: po MVP**, własne miejsca w planie, bo sklepy nie znają kont GEO.

**Błędy znalezione po drodze:** `LoggingNotificationSender` (brak dostawcy poczty) wyglądał jak wysłanie, więc bez poczty przycięcie przeszłoby bez ostrzeżenia. Nadawca zwraca teraz wynik doręczenia, a brak dostawcy się nie liczy. Host API odmawiał startu, bo polityka Powiadomień wymagała `IDeadlines`, które istnieją tylko w workerze: wyłapał to test API, naprawione przed zamknięciem E7.

**Stan testów:** Plany 62/62, Limity 29/29, Archiwum 112/112, Album 69/69, Powiadomienia 35/35, Konto 79/79, baza 75/75, wiadomości 10/10, harmonogram 11/11, architektura 49/49, API 46/46, aplikacja 276/276. Dostęp 28/28 po poprawce D-287.

**Prawdziwe uruchomienie API, workera i lokalnego serwera SMTP** na lokalnej bazie: opłacenie, limity przez workera, 10 miejsc, koniec planu, odczyt bez dodawania, ostrzeżenie do właściciela i uczestnika z potwierdzeniem, odmowa adresu i wstrzymanie, brak przycięcia po terminie, wybór właściciela, zlecenie, kosz na 30 dni, darmowy limit, brak przywrócenia bez planu, ponowne opłacenie z powrotem treści. Bez martwych wiadomości. Terminy przesuwane w bazie SQL-em i wywoływane przyciskiem odświeżenia. **Nie sprawdzono:** zadania godzinowego i miesięcznego ponowienia w działającym workerze (oba tylko w testach modułu), ekranów z prawdziwym API, urządzeń, prawdziwego dostawcy poczty.

## MEM-E7 · Decyzje PO po przeglądzie (2026-11-05)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-290 | **Decyzja PO, zmienia HS-022 dla albumów:** przycięcie albumu tylko wyjmuje wspomnienia z albumu. Autor je zachowuje, także na mapie, a miejsce wraca na jego konto, także ponad limit (D-264): nic nowego mu się wtedy nie zmieści, ale nic nie ginie. Pula albumu zwalnia się od razu. Ponowne opłacenie w ciągu 30 dni wkłada je z powrotem, także ponad limit, o ile autor nadal jest w albumie, bez oznaczeń i ułożenia. Po 30 dniach zostają u autora i może je dodać sam. **Przycięcie konta bez zmian:** własny nadmiar idzie do kosza na 30 dni, bo to własny plan tej osoby. **Skutek:** przycięcie albumu nie zmniejsza kosztu magazynu, a konto bez płatnego planu może zostać ponad limitem bezterminowo | właściciel, który przestał płacić, nie decyduje o treści innej osoby: „żadne wspomnienie nie ginie z powodu rachunku” (S2:B1). Znika też zastrzeżenie D-284 o innych albumach. Lista wyjmowanych zapisywana jest przed wyjęciem, bo Archiwum i Album to osobne transakcje: ponowienie kończy tę samą listę |
| D-291 | **Decyzja PO:** źródłem płatności będzie **RevenueCat**, na sklepach. Adapter jeszcze nie powstał, bo wymaga konta RevenueCat i produktów w sklepach, a sposób sprzedaży planu albumu czeka na D-292. Ustalenia do budowy, ze strony RevenueCat, 2026-11-05: webhook z własnym nagłówkiem autoryzacji, dostarczany co najmniej raz, z 5 ponowieniami w około 80 minut, anulowania mogą iść do 2 godzin, a po każdym zaleca się odczytać stan przez `GET /v1/subscribers`, co jednak zakłada klienta, jeśli go nie było. Identyfikator w RevenueCat to tylko UUID konta, bez adresu e-mail. **Cena wg źródeł wtórnych, do sprawdzenia na stronie cennika:** bezpłatnie do 2500 USD przychodu miesięcznie, potem 1%, od kwoty przed prowizją sklepu. **Umowa:** RevenueCat publikuje DPA i występuje jako podmiot przetwarzający; nie znalazłem potwierdzenia przechowywania danych w UE (pytanie 11 do prawnika) | odpytywanie stanu i księga z D-078 są już gotowe, a dochodzi tylko adapter |
| D-292 | **Decyzja PO:** jeden płatny album na osobę, z możliwością przeniesienia planu na inny album; do wdrożenia razem z adapterem RevenueCat. Pierwotne pytanie: ile płatnych albumów może mieć jedna osoba. Subskrypcja należy do konta w sklepie, więc to samo konto zwykle nie kupi tego samego produktu drugi raz (informacja z forum RevenueCat, nie z oficjalnej dokumentacji; do sprawdzenia w sandboxie). Rekomendacja na MVP: jeden płatny album na osobę, z możliwością przeniesienia planu na inny album | wynika z modelu sklepów, a nie z naszego kodu |

**Stan testów po D-290:** Album 76/76, Archiwum 112/112, Limity 30/30, Powiadomienia 35/35, architektura 49/49, aplikacja 276/276. Pełny przebieg na API, workerze i lokalnym SMTP przeszedł ze zmienioną ścieżką: wspomnienie wypada z albumu, zostaje autorowi na mapie, jego miejsce wraca na konto, a po opłaceniu wraca do albumu.

## MEM-E9 · Sukcesja i przejęcie albumu (2026-11-05)

PO: „zaczynaj e9”. Odwracalne założenia, do sprawdzenia przez PO.

| D | Decyzja | Dlaczego |
|---|---|---|
| D-293 | Album bez właściciela ma `owner_account_id` równy pustemu identyfikatorowi (`SharedAlbum.NoOwner`), a nie NULL. Port `OwnerOfAsync` zwraca wtedy null, a listy osób go pomijają | żadne konto nie ma takiego identyfikatora, więc każde „czy to właściciel” odpowiada „nie” bez zmian w kilkunastu miejscach; kolumna zostaje wymagana (SD18) |
| D-294 | **Zmienione przez D-306.** Pierwotnie, do sprawdzenia przez PO: opłata bez trwającej rezerwacji, np. po 31 minutach, nie daje albumu (E168). Zostaje wpis w logu, a pieniądze trzeba zwrócić ręcznie przez sklep. Do czasu RevenueCat opłatę można dać tylko ręcznie | D-004 wprost: brak opłaty w 30 minut to koniec rezerwacji. Przy adapterze warto sprawdzić, czy zakup w toku może przedłużyć rezerwację |
| D-295 | **Obsługa decyduje przez `deploy/succession/cases.sh` na serwerze.** Skrypt dopisuje wiersz, a zadanie workera co minutę stosuje go zwykłym handlerem. Nie ma żadnego punktu HTTP ani sekretu dla obsługi, a odczyt aktu i decyzje trafiają do dziennika operacji. Procedura: `docs/runbooks/takeover-case.md`. **Do ustalenia przez PO z prawnikiem:** kryteria uznania aktu (Z1). Konto w aplikacji nie ma nazwiska, więc zgodność aktu z kontem nie wynika z danych | ta sama droga co ręczne plany (D-188). Funkcja dotycząca zgonów nie powinna mieć publicznego wejścia, a E8 dopiero zdecyduje o narzędziach obsługi |
| D-296 | **„Album prywatny” z S2:C8 nie istnieje w produkcie.** Kontakt spadkowy to jedna osoba na konto, a dostaje wspomnienia autora, których nie ma w żadnym albumie, tylko do odczytu. Wskazywany adresem, bo może nie mieć jeszcze konta. Dostęp działa dopiero po zalogowaniu na ten adres, a w Dostępie leży tylko skrót adresu. Bez wskazania nikt nic nie dostaje. **Nie zrobione:** eksport dla kontaktu (S2:C8), bo eksportu nie ma w ogóle. Ten ticket nie miał ekranu wskazywania, więc dodałem prosty, w koncie | S2:C8 dopuszcza „dla wszystkich naraz”; na mapie nie ma drugiego rodzaju albumu |
| D-297 | Potwierdzony zgon jednego albumu dotyczy osoby, więc otwiera sprawy dla wszystkich jej albumów bez ponownej weryfikacji (P21). Oferta ma własne zdarzenie, a z E121 zostaje fakt o osobie, przypomnienia i termin liczą się od oferty. **Konto żyjącej osoby usuwane z następcą** idzie tą samą drogą: następca dostaje ofertę na 30 dni (S2:B3). Bez następcy P37 zwalnia rolę od razu | C122 wykonuje następca (D-043), więc P21 nie może przekazać albumu sam; ten sam skutek bez drugiej ścieżki |
| D-298 | Album, który nigdy nie płacił, po utracie właściciela wchodzi w tryb tylko do odczytu od razu (D-001). Opłacony działa do końca okresu (HS-020). Opłata w opłaconym jeszcze okresie też zgłasza E171, inaczej nie dałoby się przejąć albumu przed końcem okresu | E7 zakładało cykl tylko dla płacących; plan odnawiany dalej przez sklep zmarłej osoby wywoła E171 bez rezerwacji, czyli tylko wpis w logu |
| D-299 | **Rozstrzygnięte 2026-11-06 przez D-304 do D-307.** Pierwotnie, do decyzji PO: (1) potwierdzony zgon nie blokuje logowania na konto zmarłej osoby ani nie zamyka konta; (2) właściciel nie dostaje wiadomości o zgłoszeniu swojego zgonu, a po pomyłkowym potwierdzeniu nie ma polecenia cofnięcia. Polecam wysyłać taką wiadomość, bo koszt jest mały, a chroni przed fałszywym aktem; (3) S2:B3 chce pytać fundatora o los treści przy przekazaniu za życia, ale przekazania za życia nie ma w ticketach: po zgonie treści zostają (D-004); (4) w aplikacji akt to zdjęcie, bo nie ma wyboru pliku PDF, choć API go przyjmuje | te punkty zmieniają zachowanie wobec rodzin zmarłych, więc nie zgaduję |

**Domknięte z E4 i E7:** MEM-E4-DOSTEP-11 (P31), MEM-E7-PLANY-11, -23, -24, -25.

**Stan testów:** Sukcesja 30/30, Album 90/90, Plany 68/68, Dostęp 31/31, Archiwum 115/115, aplikacja 305/305, API 46/46, architektura 49/49.

**Prawdziwe uruchomienie API, workera, magazynu Garage i lokalnego SMTP:** zgłoszenie z prawdziwym plikiem, decyzja przez wiersz i zadanie workera, oferty na dwa albumy, album bez następcy bez właściciela, przejęcie z wkładami fundatora, odmowa, kontakt spadkowy czyta tylko prywatne, rezerwacja, opłata przez prawdziwe odpalenie `Plans.Cycle`, odrzucone zgłoszenie. Bez martwych wiadomości. Pobranie aktu kontenerem `aws-cli` sprawdzone osobnym poleceniem, **sam skrypt `cases.sh` nie był uruchamiany**, bo wymaga serwera. **Nie sprawdzono:** terminów 30 dni i przypomnień w działającym workerze (tylko testy modułu), ekranów z prawdziwym API, urządzeń.

## MEM-E5 · Prześledź (2026-11-06)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-300 | Kolejność w okresie: data początku, potem końca, a w obrębie dnia **kolejność dodania**. Nie ma komendy, którą osoba ustala własną kolejność (S2:C1 „kolejność ustalona przez użytkownika”), bo żaden ticket jej nie ma. **Zastąpione przez D-303** | kolejność dodania to najbliższe przybliżenie, a zmiana nie wymaga przebudowy |
| D-301 | Album z okresu (C71) bierze **tylko własne wspomnienia** osoby (D-248): cudze zostają w swoich albumach. Co nie mieści się w puli nowego albumu, jest liczone i pokazywane, a album i tak powstaje. Limit 1000 wspomnień na raz. Wspomnienia idą zwykłą drogą C47, więc powtórka nic nie zmienia | wspomnienie wnosi tylko jego autor; jeden album na tysiące wspomnień to pomyłka okresu |
| D-302 | S08 wybiera okres tylko jako dany rok albo zakres dat. **Nie ma** wyboru „album, osoba, kolekcja” (S1 §8), uczestników w panelu ani narracji AI (E6). Na czas ekranu mapa pokazuje tylko punkty okresu i jedzie do bieżącego wspomnienia; odtwarzanie rusza dopiero po „Odtwarzaj”, w tempie 6 albo 3 sekundy | tickety E5 obejmują okres; narracja to E6, osoby bez wyszukiwania (D-079) nie mają filtra |

**Stan testów:** Archiwum 119/119, Album 94/94, API 46/46, aplikacja 317/317. **Prawdziwe uruchomienie API:** okres z przedziałem przez Nowy Rok w obu latach, kolejność, wspomnienie przyjaciela z albumu, brak cudzych prywatnych, album z okresu tylko z własnymi, powtórka bez zmian, zły okres odrzucony. **Nie sprawdzono:** animacji mapy na prawdziwej mapie (w testach mapa to atrapa, lokalnie brak kafelków), urządzeń.

## MEM-E5-ARCHIWUM-02 · Godzina wspomnienia (2026-11-06)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-303 | Decyzja PO do D-300: w obrębie dnia kolejność **po godzinie**. Wspomnienie z dokładnością „dzień” może mieć nieobowiązkową godzinę (HH:mm, kolumna `time_of_day`), podawaną przy tworzeniu (C12) i pokazywaną w S05 i S08. Kolejność okresu: data początku, data końca, godzina, a wspomnienia bez godziny na końcu dnia w kolejności dodania. Godzina przy miesiącu, roku lub przedziale jest odrzucana (`Archive.InvalidDate`). Nie ma zmiany godziny po zapisaniu, bo nie ma też zmiany daty; godzina nie jest brana ze zdjęć (EXIF) | PO wybrał kolejność po dacie i godzinie zamiast ręcznego układania; godzina to wiedza osoby, jak data, więc ta sama droga i te same zasady |

## MEM-E9 · Decyzje PO do D-299 (2026-11-06)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-304 | **Decyzja PO do D-299 (2):** właściciel dostaje wiadomość o zgłoszeniu swojego zgonu (nowe zdarzenie E120 `DeathReportedEvent`, rodzaj `DeathReported`), zanim obsługa zdecyduje. Wiadomość nie wymienia zgłaszającego. Odpowiedź idzie na adres nadawcy powiadomień, a procedura każe obsłudze sprawdzić tę skrzynkę przed decyzją; odpowiedź właściciela oznacza odrzucenie. Nie ma okresu oczekiwania ani przycisku sprzeciwu w aplikacji, a cofnięcia potwierdzenia nadal nie ma | i tak weryfikuje człowiek, więc wiadomość wystarcza jako ochrona przed fałszywym aktem; **wymaga monitorowanej skrzynki adresu nadawcy** przy wdrożeniu |
| D-305 | **Decyzja PO do D-299 (1):** konto osoby, której zgon potwierdzono, **na razie zostaje** bez zmian: logowanie działa, konto nie jest zamykane | PO: na razie bez zmian |
| D-306 | **PO zostawił decyzję mnie (D-299, spóźniona opłata).** Rezerwacja nadal trwa 30 minut i tylko przez ten czas blokuje innych. Opłata, która przyjdzie później, ale **do 24 godzin** po końcu rezerwacji, daje album osobie, która rezerwowała, o ile album nadal nie ma właściciela, ona wciąż w nim jest i **nikt inny nie zarezerwował go w międzyczasie**. Nowa rezerwacja innej osoby odbiera to pierwszeństwo. Po 24 godzinach zadanie `Album.TakeoverReservation` zapomina rezerwującego, a późniejsza opłata to E168: wpis w logu i zwrot ręczny przez sklep (D-294). **Przy RevenueCat:** zdarzenie opłaty powinno nieść konto płacącego i wtedy album dostaje tylko ono, zamiast założenia, że zapłacił rezerwujący | zakup w sklepie rozpoczęty w czasie może dojść później: RevenueCat ponawia webhook przez około 80 minut, a potwierdzenia płatności bywają opóźnione; doba z zapasem to obejmuje. Pierwszeństwo nowej rezerwacji chroni osobę, która przyszła po wygaśnięciu, a dzisiejsza księga nie zna płacącego |
| D-307 | **Decyzja PO do D-299 (3): przekazanie albumu za życia jest.** Właściciel w S11 wskazuje uczestnika („Przekaż album”) i od razu odpowiada, co z jego własnymi wspomnieniami: zostają w albumie albo wypadają z niego przy przyjęciu, drogą C30 (S2:B3). U autora zostają w obu przypadkach. Wskazana osoba dostaje wiadomość i odpowiada w S10 w ciągu 30 dni; do tego czasu nic się nie zmienia. Po przyjęciu album, pula, plan i rola przechodzą na nią (`AlbumOwnerChanged`, reszta dostaje wiadomość o nowym właścicielu), a dawny właściciel zostaje uczestnikiem i może wyjść jak każdy. Następca zostaje wyczyszczony, a nowy właściciel wskazuje własnego. Jedna propozycja naraz: nowa zastępuje starą. Propozycja przepada, gdy osoba wyjdzie albo zostanie usunięta, gdy album straci właściciela, i po 30 dniach. Właściciel może ją wycofać. Odmowa nie powiadamia właściciela: widzi w S11, że nic nie czeka. **Plan opłacony przez sklep dawnego właściciela** zostaje przy albumie do końca okresu; przeniesienie subskrypcji to sprawa adaptera RevenueCat (D-292) | S2:B3 wymaga pytania o los treści wtedy, gdy fundator jeszcze może zdecydować; przyjęcie przez drugą stronę chroni przed narzuceniem albumu i kosztu |

**Stan testów po D-304 do D-307:** Album 102/102, Sukcesja 31/31, Powiadomienia 44/44, API 46/46, architektura 49/49, baza 75/75, aplikacja 325/325. **Prawdziwe uruchomienie API i workera z lokalnym SMTP:** zgłoszenie zgonu wysyła wiadomość właścicielowi; przekazanie z zabraniem treści: oferta tylko od właściciela, wiadomość do wskazanej osoby, przyjęcie, nowa właścicielka, dawny właściciel w albumie bez swojego wspomnienia, które zostaje u niego, druga akceptacja odrzucona, brak martwych wiadomości.

## MEM-E8 · Zgłoszenia, moderacja i nakazy (2026-11-06)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-308 | **Ukryty zakres (C67, E161) żyje w Dostępie** jako tabela `access.hidden_scopes`: jeden wiersz na **blokadę** (zgłoszenie albo nakaz), z zasobem (wspomnienie albo album), autorem i numerem kolejnym (D-013). Zasób jest ukryty, dopóki stoi choć jedna blokada. **Autor nadal widzi swoją treść**: wspomnienie autor, album właściciel; nikt inny. Odczyty albumów (`CanSeeAlbumAsync`, `VisibleAlbumsAsync`, `AlbumsTakingPartInAsync`) pomijają ukryte albumy same, a odczyty cudzych wspomnień (szczegóły, mapa, Prześledź, album, archiwum kontaktu spadkowego) i lista „moje albumy” pytają nowym `HiddenFromAsync`. Moderacja ukrywa przez port zapisu `IContentHiding` z odpowiedzią (nakaz w 1 h, L11) albo zdarzeniem E113 (P32); oba idą tą samą komendą, więc drugie nic nie zmienia. **Nic nie jest usuwane**: ukrycie zostawia dowód (S2:C2) | Dostęp jest jedynym miejscem, które odpowiada na „kto co widzi” (ADR 0011), a osobna blokada na zgłoszenie pozwala cofnąć jedną decyzję bez zdejmowania innej |
| D-309 | **Moduł Moderacja (AG4 „Zgłoszenie”).** Kategoria wynika z powodu: A (wykorzystywanie dzieci, terroryzm, groźby, nawoływanie, inne nielegalne) ukrywa treść od razu, termin 24 h; B (wizerunek, dane osobowe, zniesławienie, prawa autorskie, inne) zostawia ją widoczną, termin 7 dni (S2:C2). Zgłaszać można tylko wspomnienie albo album, który osoba **widzi teraz**, i nie własne. To samo zgłoszenie tej samej osoby o tę samą rzecz, póki czeka, to wciąż jedno zgłoszenie (indeks `ux_reports_open`). Jedna decyzja z uzasadnieniem dla obu stron (DSA art. 17); autor nie dowiaduje się, kto zgłosił. Odwołanie: **tylko autor, tylko od ukrycia, przez 6 miesięcy** (DSA art. 20), raz, z osobną decyzją. **Panel obsługi S26 zastępuje skrypt** `deploy/moderation/moderation.sh` (kolejka R9 według terminu, podgląd treści z wpisem do dziennika, decyzja, odwołanie) i minutowe zadanie `Moderation.OperatorActions`, jak w D-295: brak publicznego wejścia dla obsługi. Procedura: `docs/runbooks/moderation.md` | rola obsługi w publicznym API to nowa powierzchnia ataku i osobne logowanie, a trzyosobowy zespół i tak pracuje na serwerze; skrypt i dziennik operacji już są |
| D-310 | **S21 bez konta** to zwykła strona `/report` poza API (jak `/account/delete`), z adresem e-mail, opisem miejsca (do 1000 znaków), powodem, uzasadnieniem i oświadczeniem o dobrej wierze (DSA art. 16 ust. 2). Treść nie jest wskazywana identyfikatorem, bo bez konta nikt go nie zna: obsługa znajduje ją po opisie, a takie zgłoszenie nic nie ukrywa samo. nginx: 5 na minutę z adresu. Zgłaszający bez konta dostaje potwierdzenie i decyzję z uzasadnieniem e-mailem | DSA wymaga zgłoszeń od każdego, a osoba ze zdjęcia nie musi mieć konta (S2:C2) |
| D-311 | **Rozstrzygnięte 2026-11-06 przez D-314 do D-317.** Pierwotnie, nie zrobione w E8, do decyzji PO: (1) odebranie możliwości zgłaszania za nadużycia (S2:C2) — dziś zgłoszenie A od uczestnika albumu ukrywa treść od razu, więc nadużycie jest możliwe aż do decyzji obsługi; (2) odwołanie zgłaszającego od decyzji „zostaje” (DSA art. 20 obejmuje też zgłaszających); (3) HS-006 dyżur i alarm o zgłoszeniach A i nakazach, HS-007 kto rozpatruje odwołanie od własnej decyzji; (4) wiadomość do obsługi o nowym zgłoszeniu — dziś obsługa sama patrzy w `list` | każde z nich zmienia zobowiązania zespołu, a (1) i (2) wymagają reguł, których źródła nie podają |
| D-312 | **Nakazy organów (Z6, L11, L18).** Obsługa wpisuje nakaz skryptem dopiero po sprawdzeniu podstawy (`moderation.sh order …`), a worker stosuje go w ciągu minuty. **Usunięcie** (E115, P19): treść ukryta przez port zapisu Dostępu z potwierdzeniem; bez potwierdzenia dostarczenie się nie udaje i ponawia, więc godzina z L11 zależy od dostępności Dostępu, nie od dyżuru. Autor dostaje wiadomość, chyba że organ prosi o poufność. Nakaz usunięcia nie ma zamknięcia. **Zabezpieczenie i wydanie** (E116, P20, C114): najpierw blokada prawna na konto (E117, port `ILegalHolds`). Dopóki stoi, **nic z konta nie znika na stałe**: `MemoryEraser` zamiast usunąć przenosi wspomnienie w nowy stan `Erasing` (niewidoczne także dla autora; dzienne zadanie Archiwum usuwa je po zdjęciu blokady), a usunięcie konta czeka na kolejne dzienne zadanie. Osoba nie dowiaduje się o blokadzie. **Wydanie** (C115, E118) tylko dla nakazu wydania, tylko po zabezpieczeniu i przed zamknięciem: worker zapisuje wpis do dziennika `moderation.access_log`, dopiero potem skrypt eksportuje dane (wspomnienia w JSON i zdjęcia) do katalogu. Do tego samego dziennika trafia każdy podgląd treści przy zgłoszeniu. Dziennik jest tylko dopisywany: kod nie ma ścieżki zmiany ani usunięcia, ale rola modułu technicznie mogłaby go zmienić (lint migracji nie dopuszcza wyzwalaczy w SQL); kopia idzie do dziennika operacji na serwerze | L18 wymaga zabezpieczenia przed wydaniem i bez ujawnienia; jeden punkt usuwania wspomnień (`MemoryEraser`) obejmuje kosz, żądanie usunięcia i usunięcie konta |
| D-313 | **S27 i S22 w aplikacji.** „Zgłoś treść” jest w S05 przy cudzym wspomnieniu i w S10 dla każdego poza właścicielem (także przy albumie oglądanym przez link). S27 ma powód i **obowiązkowe uzasadnienie** (ticket wymienia tylko kategorię, ale DSA art. 16 wymaga uzasadnienia); przy powodach z kategorii A ekran mówi, że treść zniknie od razu. S22 jest w pasku S24: decyzje o moich treściach z uzasadnieniem i odwołaniem, póki trwa jego termin, oraz moje zgłoszenia. Bez ekranu na odwołanie zgłaszającego (D-311) | wymagania ekranów z modelu, zgłoszenia i blokowania wymagają sklepy (L19) |

**Stan testów E8:** Moderacja 28/28, Dostęp 38/38, Archiwum 129/129, Album 102/102, Konto 80/80, Powiadomienia 48/48, API 46/46, architektura 49/49, baza 75/75, aplikacja 337/337. **Prawdziwe uruchomienie API i workera z lokalnym SMTP:** zgłoszenie A ukrywa od razu dla wszystkich poza autorem (szczegóły, mapa, album), zgłoszenie B zostaje do decyzji, decyzje przez skrypt stosuje worker, odwołanie i jego uwzględnienie, formularz `/report` bez konta z potwierdzeniem e-mailem; nakaz usunięcia ukrył treść po 43 s, nakaz zabezpieczenia zatrzymał formalne usunięcie (stan `Erasing`, poufnie), zamknięcie zdjęło blokadę; bez martwych wiadomości. **Nie sprawdzono:** ekranów na urządzeniu, skryptu `moderation.sh` na serwerze (tylko shellcheck), eksportu przy wydaniu danych.

## MEM-E8 · Decyzje PO do D-311 (2026-11-06)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-314 | **Decyzja PO do D-311 (2): odwołuje się strona, przeciw której zapadła decyzja.** Autor od ukrycia, zgłaszający z kontem od decyzji „treść zostaje”, przez 6 miesięcy, raz. Uwzględnione odwołanie zgłaszającego ukrywa treść, a autor dostaje wiadomość z uzasadnieniem, bez kolejnego odwołania. Zgłaszający bez konta nie ma ekranu odwołania: może odpowiedzieć na wiadomość z decyzją | DSA art. 20 obejmuje obie strony |
| D-315 | **Decyzja PO do D-311 (1), po porównaniu z praktyką dużych serwisów:** obsługa może uznać zgłoszenie za oczywiście bezzasadne (`decide … unfounded`: treść zostaje). Zgłaszający dostaje wtedy **ostrzeżenie** z zasadą. Po **3 takich zgłoszeniach w ciągu 90 dni** jego zgłoszenia (z konta albo z tego samego adresu na stronie `/report`) nie są przyjmowane przez **6 miesięcy** od ostatniego, więc niczego też nie ukryje. Natychmiastowe ukrycie przy kategorii A zostaje (S2:C2) | DSA art. 23: zawieszenie po ostrzeżeniu, na rozsądny czas; ogranicza nadużycie natychmiastowego ukrycia bez opóźniania prawdziwych zgłoszeń |
| D-316 | **Decyzja PO do D-311 (4), HS-007:** odwołanie rozpatruje **inna osoba** niż ta, która wydała pierwszą decyzję. Pilnuje tego domena (`Moderation.SameDecider`), więc skrypt nie przepuści tej samej osoby | inaczej odwołanie jest fikcją |
| D-317 | **Decyzja PO do D-311 (3):** powiadomienia obsługi o nowych zgłoszeniach i nakazach („dzwoneczek”) **po MVP**. Do tego czasu obsługa sprawdza `moderation.sh list` sama; HS-006 (dyżur przy nakazie 1 h) zostaje otwarte | PO |

## MEM-E12 · UI mapy (2026-11-06)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-318 | **Nowy epik E12 z prośby PO po pokazie.** Podgląd po najechaniu (MEM-E12-PLAT-01): osobne lekkie zapytanie `GET /v1/memories/{id}/preview` z tym samym filtrem dostępu co szczegóły (także ukrycie przez moderację), pierwsze dodane zdjęcie (najstarszy identyfikator) jako podpisany oryginał, wycinek opisu do 140 znaków ucięty na słowie. Karta tylko po najechaniu myszą (przeglądarka); stuknięcie dalej otwiera S05, bo urządzenia dotykowe nie mają najechania. Podgląd zostaje w pamięci, póki mapa jest otwarta. **Nie ma miniatur**: duże zdjęcie przy wolnym łączu ładuje się dłużej | szczegóły podpisywałyby wszystkie zdjęcia przy każdym ruchu myszy; miniatury to osobna praca przy przesyłaniu zdjęć |
| D-319 | **Menu mapy (MEM-E12-PLAT-02):** w przeglądarce prawy przycisk (moduł `memorio-map.mjs` podmienia klasę `Map` wtyczki na taką, która przy `contextmenu` blokuje menu przeglądarki i wysyła zdarzenie `memorio-map-contextmenu` ze współrzędnymi na `window`), na urządzeniu długie przytrzymanie. Menu ma jedną pozycję: „Dodaj wspomnienie tutaj”, która otwiera S04 ze współrzędnymi w polach miejsca własnego (`?lat=&lng=`, poza zakresem pomijane); nazwę osoba wpisuje sama albo wybiera miejsce z wyszukiwarki. **Bez geokodowania odwrotnego** (nazwy z klikniętego punktu) | wtyczka nie zgłasza prawego przycisku, a nazwa z punktu wymaga osobnej usługi albo bazy |
| D-320 | **Szczegółowa mapa (MEM-E12-PLAT-03).** Styl z warstwami schematu Protomaps: lasy i tereny, woda i rzeki, zabudowa, budynki od zoomu 14, ścieżki, drogi, granice, **szczyty z wysokością** (od zoomu 10) i nazwy po polsku (`name:pl`, potem oryginalne). Na produkcji skrypt `deploy/map/basemap.sh install <dzień> [obszar] [zoom]` wycina obszar z wybranej wersji mapy Protomaps obrazem `protomaps/go-pmtiles:v1.31.2`, sprawdza plik i podmienia go dopiero w całości; nginx serwuje `/map/` z `/var/lib/memorio/map`. **Domyślnie Europa do zoomu 15: kilkadziesiąt GB na dysku serwera** (do decyzji PO przy wdrożeniu: mniejszy obszar albo zoom). Lokalna próbka: `tools/map-benchmark/make-fixture.mjs` z `--area` dokłada wybrane obszary w wyższym zoomie (pokaz: Tatry i Warszawa do zoomu 14). **Uwaga:** produkcja nie serwuje jeszcze samej aplikacji webowej (nginx przekazuje wszystko do API); aplikacje na urządzeniach biorą mapę z `MAP_BASE_URL` | D-032: mapa z własnego serwera; pobranie gotowego wycinka jest tańsze niż własne generowanie z OSM |

## MEM-E13 · Wygląd i ustawienia (2026-11-07)

| D | Decyzja | Dlaczego |
|---|---|---|
| D-321 | **Nowy epik E13 z prośby PO.** Ustawienia urządzenia (`core/settings`: język, motyw, wygląd punktów) w bezpiecznym magazynie urządzenia, obok sesji, bez serwera; domyślne wartości od razu, zapamiętane po wczytaniu. Ekran Ustawienia z zakładki Konto (ikona w pasku). **Języki: polski i angielski** (`app_en.arb`, test pilnuje kompletu kluczy). Domyślnie język urządzenia; nieobsługiwany daje polski. Nazwy języków w ich własnym brzmieniu. **Poza zakresem:** wiadomości e-mail i strony serwera zostają po polsku | PO: aplikacja ma być bardzo przyjazna; ustawienia wyglądu są sprawą urządzenia, nie konta |
| D-322 | **Tryb ciemny (MEM-E13-PLAT-02):** jak w urządzeniu (domyślnie), jasny, ciemny. Akcent jasnego motywu niebieski, ciemnego złoty (zamiast dawnego morskiego). Ekrany biorą kolory tylko z motywu. **Mapa bazowa zostaje jasna** także w ciemnym trybie (osobny styl to osobna praca) | prośba PO |
| D-323 | **Nowy pasek zakładek (MEM-E13-PLAT-03):** własny widżet zamiast `NavigationBar`: wyśrodkowany, najwyżej 440 px, zaokrąglony, biały (ciemny: #2A2B30), obramówka i cień. Ikona pod myszą przyjmuje akcent motywu (niebieski albo złoty), powiększa się o 20% i wysuwa w górę; wybrana zakładka ma akcent i podświetlenie. Dostępność: każda zakładka to przycisk z nazwą i stanem wybrania | prośba PO; `NavigationBar` nie ma najechania ani pływającego wyglądu |
| D-324 | **Mały kalendarz filtra dat (MEM-E13-PLAT-04):** własny widżet miesiąca (szer. 296 px, tydzień od poniedziałku, nazwy z `MaterialLocalizations`, więc w obu językach) zamiast pełnoekranowego `showDateRangePicker`. Na ekranie od 600 px stoi po lewej pod filtrami, na węższym na środku. Pierwsze stuknięcie to początek, drugie koniec (w dowolnej kolejności), dni po dzisiejszym wyłączone; „Zastosuj”, „Wyczyść”, stuknięcie obok zamyka bez zmian | prośba PO |
| D-325 | **Wygląd punktów (MEM-E13-PLAT-05):** w Ustawieniach podgląd kropka → okrąg ze zdjęciem, przełącznik z opisem (domyślnie wyłączony) i suwak wielkości 100–200%. Z włączonym przełącznikiem mapa prosi o `/v1/memories/map?photos=true` (pierwsze zdjęcie każdego wspomnienia, podpisane) i rysuje okrąg ok. 29 px z białą obwódką i cieniem, zrobiony w aplikacji z pobranego zdjęcia; bez zdjęcia albo gdy zdjęcia nie da się odczytać zostaje kropka; skupiska bez zmian. **Na produkcji magazyn musi pozwalać na pobranie zdjęcia z przeglądarki (CORS)**; w demo idzie przez ten sam adres. Pobierany jest oryginał zdjęcia (brak miniatur, D-318) | prośba PO: przyjazna aplikacja, opcja nie domyślna |
| D-326 | **S04 bez osobnego kroku dla miejsca własnego (MEM-E12-PLAT-05, z pokazu):** gdy pola miejsca własnego mają nazwę i współrzędne, „Dalej” jest aktywne i samo dodaje to miejsce; pod przyciskiem krótko widać, czego jeszcze brakuje (miejsca, daty). „Dodaj miejsce własne” zostaje | PO na pokazie nie wiedział, czemu „Dalej” jest wyszarzone |
| D-327 | **Pasek jako podkładka (MEM-E13-PLAT-06):** niska podkładka 46 px (maks. 400 px szerokości), nad nią okrągłe znaczki 48 px z ikoną 28 px, wystające ok. 30 px ponad krawędź; podpis (labelMedium) leży na podkładce. Wybrana zakładka: znaczek w kolorze akcentu z białą ikoną; najechanie: akcent, powiększenie 1,12 i wysunięcie | prośba PO z pokazu |
| D-328 | **Nowe wspomnienie w oknie (MEM-E13-PLAT-07):** `/memories/new` zostaje adresem, ale otwiera się jako okno na głównym nawigatorze (przykrywa też dolny pasek; mapa zostaje pod spodem): `ModalPage` z `PopupRoute`, `barrierDismissible: false` (tło i Esc nie zamykają) i `PopScope(canPop: false)` (systemowe „wstecz” też nie). Zamykają tylko X i „Anuluj”, wracając na mapę; wpisana treść przepada bez pytania (pytanie poza zakresem). Szeroki ekran (≥720 px): okno 640 px pośrodku; telefon: cały ekran. U góry pasek etapów (1 Miejsce i data, 2 Treść), na dole stała stopka z Anuluj / Wstecz / Dalej / Zapisz | prośba PO z pokazu |
| D-329 | **Układ okna nowego wspomnienia i karuzela (MEM-E13-PLAT-08):** sekcje w ramkach z ikoną (Miejsce, Data, Zdjęcia, Twoja historia); pola z obramówką, szerokość i długość obok siebie; daty i godzina jako przyciski z obramówką w zawijanym wierszu; wybrane miejsce jako karta z „Zmień miejsce”. Okno 680 px. Zdjęcia przed zapisem w karuzeli (duży podgląd, strzałki, licznik, miniatury, usuń); usunięte nie są wysyłane. Podgląd czyta bajty wybranego pliku w aplikacji, bez wysyłki | prośba PO po obejrzeniu okna |
| D-330 | **Cztery kroki i mapa pod oknem (MEM-E13-PLAT-09):** nowe wspomnienie w krokach Miejsce → Data → Zdjęcia (można pominąć) → Historia, każdy z dużym pytaniem i zdaniem wyjaśnienia, bez ramek, odstępy 16 px. Okno na czas otwarcia wstrzymuje mapę: licznik `mapHoldProvider` (okna mogą się nakładać), który `ModalPage` podnosi przez `MapHeld`; widok mapy wyłącza wtedy wszystkie gesty (wtyczka web stosuje je tylko wszystkie pięć razem) oraz najechanie, kliknięcie punktu i menu prawego przycisku | prośba PO: mapa przesuwała się pod oknem, pola były ściśnięte |
| D-331 | **Podgląd na zdjęciu (MEM-E13-PLAT-10):** karta 260 px; zdjęcie wypełnia ją (min. 200 px), u dołu pasek #303236 z przezroczystością 85% i białym tekstem; bez zdjęcia cała karta szara. Kolory stałe, niezależne od jasnego i ciemnego trybu, bo tekst leży na zdjęciu | prośba PO |
| D-332 | **Wygląd S05 (MEM-E13-PLAT-11):** galeria 380 px (telefon 280 px) z białym miejscem i datą na przyciemnieniu; bez zdjęć pasek 160 px w barwach akcentu. Zdjęcie na pełnym ekranie z przybliżaniem. Treść maks. 760 px: znaczniki, „Historia”, „Osoby”, czerwona sekcja „Usuwanie” na końcu. Teksty „Miejsce: …” i „Data: …” zastąpione samą nazwą i datą (etykiety zbędne przy takim układzie) | prośba PO |
| D-333 | **QR linku albumu (MEM-E13-PLAT-12):** kod z tym samym adresem co link (S11), rysowany lokalnie pakietem `qr_flutter` 4.1 (BSD-3, bez sieci). Osobnego tokenu ani śledzenia skanów nie ma: unieważnienie linku wyłącza kod | prośba PO |
| D-334 | **Odbitki zamiast okręgów (MEM-E13-PLAT-13, zastępuje kształt z D-325):** znacznik 96×114 px (na mapie ×0,36, ok. 35×41 px): zdjęcie przycięte do kwadratu, biała ramka 7 px, dół 26 px, cień. `icon-anchor: bottom`, `icon-rotate` z właściwości `tilt` (-6..6°, z haszu id, stały). Skupiska bez zmian | prośba PO: ma przypominać prawdziwe zdjęcia z polaroida |
| D-335 | **HS-004 (zgoda na AI po wyjściu z albumu):** zgoda jest dla pary osoba × album i **wygasa**, gdy osoba opuszcza album albo zostaje usunięta (także z wyborem „zostaw”). Wygaśnięcie to odwołanie (E81): kaskada P12 unieważnia artefakty z jej treści w tym albumie, a pozostawione treści nie idą do nowych podsumowań. Po powrocie zgodę daje się od nowa | wariant polecony PO 2026-11-08 („możesz robić epik z AI”); bezpieczniejszy dla prywatności i zgodny z C5 („zgoda osobno dla każdego albumu”) |
| D-336 | **Projekt E6.** (1) **Zgoda** `AiConsent` w NarracjaAI: osoba × album, udziel (E80) / odwołaj (E81, w kontrakcie). Właściciel też musi wyrazić zgodę dla swoich treści. (2) **Materiały (R8)** = wspomnienia albumu z okresu, których autor ma zgodę dla tego albumu, nieukryte dla pytającego (Dostęp: `CanSeeAlbum` + `HiddenFrom`); wynik mówi też, ilu uczestników i wspomnień pominięto. (3) **Podsumowanie (C82)** zamawia uczestnik albumu z własną zgodą, za 1 operację z miesięcznej puli AI (osobny licznik 4/10 z D-286 dalej czeka; jedna pula na razie). Do dostawcy idą tylko fakty F1..Fn (data, opis po pseudonimizacji, miejsce jako token), bez zdjęć; model zwraca zdania z listą faktów, a zdanie bez poprawnego źródła jest **odrzucane** (A2: każde zdanie wskazywalne). Tekst ma znacznik AI, model i wersję promptu. (4) **Zatwierdź (C83)** sprawdza materiały jeszcze raz (zmienione: trzeba wygenerować od nowa) i rejestruje artefakt w Pochodzeniu (C154) z listą źródeł (wspomnienie, autor, album). (5) **Dodaj do albumu (C84)** pokazuje zapisane podsumowanie uczestnikom albumu. (6) **Pochodzenie**: rejestr artefaktów i źródeł; unieważnienie (C31) jest trwałe, nie wraca po ponownym wpuszczeniu (B2), publikuje zdarzenie, na które NarracjaAI oznacza podsumowanie jako nieważne. Polityki: P4 `ContentDeleted` (wspomnienie), P9 `ContributionsWithdrawn` (autor × album), P10 `AccessRevoked` (artefakty tej osoby z albumu), P12 E81 (autor × album), P34 `ArchiveTrimmed` rozszerzone o listę przyciętych wspomnień | S2:A2, B2, C5; tickety E6 są ogólne, więc szczegóły ustalam tutaj |
| D-337 | **C31 i AG9 (MEM-E6-POCHODZENIE-01):** fakty „źródło niedostępne” (`provenance.unavailable_sources`, tylko dopisywane, id = id zdarzenia) w czterech rodzajach: wspomnienie (P4), wspomnienie w albumie (P34), autor w albumie (P9, P12), czytelnik albumu (P10). Czas faktu = czas zdarzenia źródłowego. C31 dopisuje fakt, potem unieważnia trafione ważne artefakty; rejestracja po zapisie sprawdza fakty od `MaterialsReadAt` (D-023). Każde unieważnienie publikuje `ArtifactInvalidatedEvent` (E61). **Kontrola dzienna z D-023 jeszcze nie istnieje**: zapis-potem-sprawdzenie zamyka wyścig (test 10 rund równoległych), a kontrola dzienna dojdzie z metryką QS7 | ticket wymaga zapisu, potem sprawdzenia; kontrola dzienna to siatka bezpieczeństwa, nie warunek poprawności |
