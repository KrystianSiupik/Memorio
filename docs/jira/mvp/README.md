# Plan MVP — Memorio / GEO (lokalny, bez Jiry)

> Produkt projektowy (ćwiczenie projektowe), nie realny produkt komercyjny — zadania prawne i organizacyjne dokumentują, co byłoby potrzebne w realnym wdrożeniu.

Plan zaktualizowany 2026-10-19 po przeglądzie wszystkich decyzji (D-001…D-079 w `../../decisions.md`). **Tickety nie są założone w Jirze** — to pliki lokalne.

## Źródła

- opis produktu §15–16 · przegląd wymagań (B1–B4, C1–C11) · mapa modułów (+§8–§10) · model procesów i kontrakty modułów · projekt agregatów · architektura (ADR 0001–0021) · model ekranów (28 ekranów, testy przejść) · przegląd prawny R01–R13 · plan infrastruktury · symulacja zmian biznesowych

## Architektura

- Modularny monolit .NET (14 modułów), w każdym module warstwy Onion (Domain ← Application ← Infrastructure ← API), lekki CQRS (ADR 0019).
- Flutter (iOS, Android, web), Clean Architecture per funkcja (domain / data / presentation).
- Osobny worker: Wolverine (outbox/inbox i kolejki w PostgreSQL, bez brokera) + Quartz.NET (terminy).
- Jeden PostgreSQL + PostGIS, schemat na moduł; zdjęcia w prywatnym magazynie obiektów UE, adresy ważne 5 min.

## Cele planu (metryka bez wartości docelowej = pytanie do PO)

| Cel | Miara | Jak i kiedy | Wartość docelowa |
|---|---|---|---|
| G1 Ludzie regularnie gromadzą i przeglądają wspomnienia na mapie | retencja 12-mies.; % płacących po 90 dniach | analityka po zgodzie (D-050), miesięcznie | **pytanie do PO** |
| G2 Kontrola nad treściami działa także po latach | 0 żywych artefaktów z niedostępnym źródłem (QS7); usunięcie konta ≤ 65 dni z kopiami (QS9) | kontrola dzienna; test odtworzenia co miesiąc | z wymagań |
| G3 AI nie zmyśla i nie wysyła nazw | 0 nazw w ładunku do dostawcy, 100% tekstów AI ze znacznikiem (QS8) | test automatyczny w CI | z wymagań |
| G4 Start legalny i tani (w realnym wdrożeniu) | DPIA, umowy powierzenia, etap 1 infrastruktury ($) | przed pierwszym użytkownikiem | z decyzji 31–35 |

## Poza zakresem MVP

- Publikacja i profil miejsca (PR11, PR12), Odkrywaj/rekomendacje, druk, filmy (D-070)
- Import z telefonu i plan rodzinny — P2 (tickety istnieją)
- Sprzedaż przez www (D-051), gość bez konta (D-077: zaproszenie prowadzi przez rejestrację)
- Etap 2 infrastruktury (AWS) — backlog z wyzwalaczem (niżej)

## Epiki

| Kod | Epik | Priorytet | Tickety | h |
|---|---|---|---|---|
| [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) | Szkielet end-to-end i platforma (etap 1) | P0 | 31 | 49–96 |
| [E11](epiki/E11-zgodnosc-przed-startem-dpia-umowy-procedury.md) | Zgodność przed startem (DPIA, umowy, procedury) | P0 | 8 | 15–29 |
| [E1](epiki/E1-konto-rejestracja-16-usuniecie-z-oknem-30-dni.md) | Konto: rejestracja 16+, usunięcie z oknem 30 dni | P0 | 14 | 22–45 |
| [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) | Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie | P0 | 25 | 39–82 |
| [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) | Album współdzielony: skład, wkłady, zgody | P0 | 49 | 78–159 |
| [E4](epiki/E4-udostepnianie-linkiem-i-odbieranie-dostepu.md) | Udostępnianie linkiem i odbieranie dostępu | P0 | 13 | 21–43 |
| [E5](epiki/E5-przesledz-odtwarzanie-okresu-na-mapie.md) | Prześledź: odtwarzanie okresu na mapie | P0 | 3 | 5–11 |
| [E6](epiki/E6-podsumuj-podsumowania-ai-bez-zmyslania-z-kaskada.md) | Podsumuj: podsumowania AI bez zmyślania, z kaskadą | P0 | 17 | 23–52 |
| [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) | Plany, limity i retencja 24 miesiące | P0 | 40 | 70–129 |
| [E8](epiki/E8-zgloszenia-moderacja-i-nakazy-organow.md) | Zgłoszenia, moderacja i nakazy organów | P0 | 18 | 28–61 |
| [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) | Sukcesja i przejęcie albumu (następca albo inny uczestnik) | P0 | 30 | 49–97 |
| [E10](epiki/E10-import-zdjec-z-telefonu-po-mvp.md) | Import zdjęć z telefonu (po MVP) | P2 | 5 | 7–17 |

**Razem:** 253 ticketów, 406–821 h (widełki). P0: 241 ticketów, 385–780 h.

## Tickety

| Kod | Tytuł | Epik | Moduł / warstwa | Klasa | h | Pewność | Status |
|---|---|---|---|---|---|---|---|
| [MEM-E0-PLAT-01](MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md) | Szkielet modularnego monolitu .NET: 14 modułów, kontrakty, testy architektury w CI | E0 | - / szkielet | integration | 2–4 | średnia | gotowy |
| [MEM-E0-PLAT-02](MEM-E0-PLAT-02-outbox-inbox-w-wolverine-postgresql-bez-brokera-w-.md) | Outbox/Inbox w Wolverine (PostgreSQL, bez brokera) w osobnym workerze + metryka wieku kolejki | E0 | - / infrastruktura | integration | 2–4 | średnia | gotowy |
| [MEM-E0-PLAT-03](MEM-E0-PLAT-03-postgresql-postgis-schemat-i-rola-bazy-na-modul-im.md) | PostgreSQL + PostGIS, schemat i rola bazy na moduł, import tabeli OSM | E0 | - / infrastruktura | integration | 2–4 | średnia | gotowy |
| [MEM-E0-PLAT-04](MEM-E0-PLAT-04-magazyn-obiektow-w-ue-bucket-prywatny-podpisy-5-mi.md) | Magazyn obiektów w UE: bucket prywatny, podpisy 5 min partiami, strefa tymczasowa uploadu | E0 | - / infrastruktura | integration | 2–4 | średnia | gotowy |
| [MEM-E0-PLAT-05](MEM-E0-PLAT-05-spike-mapy-maplibre-pmtiles-w-flutterze-globus-10-.md) | Spike mapy: MapLibre + PMTiles w Flutterze (globus, 10 tys. punktów, web) | E0 | - / infrastruktura | integration | 2–4 | średnia | gotowy |
| [MEM-E0-PLAT-06](MEM-E0-PLAT-06-adapter-ai-pseudonimizacja-znacznik-ai-wersja-prom.md) | Adapter AI: pseudonimizacja, znacznik AI, wersja promptu, test ładunku | E0 | - / infrastruktura | integration | 2–4 | średnia | gotowy |
| [MEM-E0-PLAT-07](MEM-E0-PLAT-07-modul-konto-asp-net-core-identity-apple-google-dek.md) | Moduł Konto: ASP.NET Core Identity, Apple/Google, deklaracja 16+, strona www usuwania konta | E0 | - / infrastruktura | integration | 2–4 | średnia | gotowy |
| [MEM-E0-PLAT-08](MEM-E0-PLAT-08-worker-quartz-net-z-magazynem-w-postgresql-zadania.md) | Worker: Quartz.NET z magazynem w PostgreSQL; zadania idempotentne, gotowe na tryb klastra | E0 | - / infrastruktura | integration | 2–4 | średnia | gotowy |
| [MEM-E0-PLAT-09](MEM-E0-PLAT-09-etap-1-vps-compose-nginx-wal-g-comiesieczny-test-o.md) | Etap 1: VPS, Compose, nginx, WAL-G, comiesięczny test odtworzenia | E0 | - / infrastruktura | integration | 2–4 | średnia | gotowy |
| [MEM-E0-PLAT-10](MEM-E0-PLAT-10-obserwowalnosc-opentelemetry-prometheus-loki-grafa.md) | Obserwowalność: OpenTelemetry, Prometheus/Loki/Grafana, monitor zewnętrzny, alert | E0 | - / infrastruktura | integration | 2–4 | średnia | gotowy |
| [MEM-E0-PLAT-11](MEM-E0-PLAT-11-offline-kolejka-drift-z-uuid-klienta.md) | Offline: kolejka Drift z UUID klienta | E0 | - / infrastruktura | integration | 2–4 | średnia | gotowy |
| [MEM-E0-PLAT-12](MEM-E0-PLAT-12-api-v1-openapi-klient-dart-kontrola-zgodnosci-w-ci.md) | API v1: OpenAPI → klient Dart, kontrola zgodności w CI, minimalna wersja aplikacji | E0 | - / infrastruktura | integration | 2–4 | średnia | gotowy |
| [MEM-E0-PLAT-13](MEM-E0-PLAT-13-pipeline-wdrozenia-jedna-komenda-lint-migracji-exp.md) | Pipeline wdrożenia jedną komendą + lint migracji expand/contract | E0 | - / infrastruktura | integration | 2–4 | średnia | gotowy |
| [MEM-E0-PLAT-14](MEM-E0-PLAT-14-testy-macierzy-dostepu-uzytkownik-zasob-i-wyszukiw.md) | Testy macierzy dostępu (użytkownik × zasób) i wyszukiwania po odebraniu dostępu | E0 | - / infrastruktura | integration | 2–4 | średnia | gotowy |
| [MEM-E0-PLAT-15](MEM-E0-PLAT-15-ksiega-uprawnien-w-planach-pierwszy-adapter-sklepy.md) | Księga uprawnień w Planach: pierwszy adapter (sklepy albo serwis subskrypcji), cykl planu czyta tylko księgę | E0 | - / infrastruktura | integration | 2–4 | średnia | gotowy |
| [MEM-E0-PLAT-16](MEM-E0-PLAT-16-szablon-warstw-onion-w-module-backendu-i-funkcji-f.md) | Szablon warstw Onion w module backendu i funkcji Fluttera + testy architektury kierunku zależności | E0 | - / infrastruktura | integration | 2–4 | średnia | gotowy |
| [MEM-E0-PLAT-17](MEM-E0-PLAT-17-konfiguracja-z-env-z-walidacja-przy-starcie-health.md) | Konfiguracja z ENV z walidacją przy starcie, health i readiness endpoint, kontener (RD1, RD2, RD4) | E0 | - / infrastruktura | integration | 2–3 | średnia | gotowy |
| [MEM-E0-PLAT-18](MEM-E0-PLAT-18-sekrety-etapu-1-env-poza-repo-sops-age-do-odtworze.md) | Sekrety etapu 1: .env poza repo + sops/age do odtworzenia (RD10) | E0 | - / infrastruktura | crud | 1–2 | średnia | gotowy |
| [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md) | Szkielet modułu archiwum: struktura, publiczny kontrakt, testy architektury | E0 | archiwum / szkielet modułu | crud | 1–2 | średnia | gotowy |
| [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md) | Szkielet modułu album: struktura, publiczny kontrakt, testy architektury | E0 | album / szkielet modułu | crud | 1–2 | średnia | gotowy |
| [MEM-E0-DOSTEP-01](MEM-E0-DOSTEP-01-szkielet-modulu-dostep-struktura-publiczny-kontrak.md) | Szkielet modułu dostep: struktura, publiczny kontrakt, testy architektury | E0 | dostep / szkielet modułu | crud | 1–2 | średnia | gotowy |
| [MEM-E0-POCHODZENIE-01](MEM-E0-POCHODZENIE-01-szkielet-modulu-pochodzenie-struktura-publiczny-ko.md) | Szkielet modułu pochodzenie: struktura, publiczny kontrakt, testy architektury | E0 | pochodzenie / szkielet modułu | crud | 1–2 | średnia | gotowy |
| [MEM-E0-MIEJSCA-01](MEM-E0-MIEJSCA-01-szkielet-modulu-miejsca-struktura-publiczny-kontra.md) | Szkielet modułu miejsca: struktura, publiczny kontrakt, testy architektury | E0 | miejsca / szkielet modułu | crud | 1–2 | średnia | gotowy |
| [MEM-E0-LIMITY-01](MEM-E0-LIMITY-01-szkielet-modulu-limity-struktura-publiczny-kontrak.md) | Szkielet modułu limity: struktura, publiczny kontrakt, testy architektury | E0 | limity / szkielet modułu | crud | 1–2 | średnia | gotowy |
| [MEM-E0-PLANY-01](MEM-E0-PLANY-01-szkielet-modulu-plany-struktura-publiczny-kontrakt.md) | Szkielet modułu plany: struktura, publiczny kontrakt, testy architektury | E0 | plany / szkielet modułu | crud | 1–2 | średnia | gotowy |
| [MEM-E0-NARRACJA-AI-01](MEM-E0-NARRACJA-AI-01-szkielet-modulu-narracja-ai-struktura-publiczny-ko.md) | Szkielet modułu narracja-ai: struktura, publiczny kontrakt, testy architektury | E0 | narracja-ai / szkielet modułu | crud | 1–2 | średnia | gotowy |
| [MEM-E0-MODERACJA-01](MEM-E0-MODERACJA-01-szkielet-modulu-moderacja-struktura-publiczny-kont.md) | Szkielet modułu moderacja: struktura, publiczny kontrakt, testy architektury | E0 | moderacja / szkielet modułu | crud | 1–2 | średnia | gotowy |
| [MEM-E0-KONTO-01](MEM-E0-KONTO-01-szkielet-modulu-konto-struktura-publiczny-kontrakt.md) | Szkielet modułu konto: struktura, publiczny kontrakt, testy architektury | E0 | konto / szkielet modułu | crud | 1–2 | średnia | gotowy |
| [MEM-E0-SUKCESJA-01](MEM-E0-SUKCESJA-01-szkielet-modulu-sukcesja-struktura-publiczny-kontr.md) | Szkielet modułu sukcesja: struktura, publiczny kontrakt, testy architektury | E0 | sukcesja / szkielet modułu | crud | 1–2 | średnia | gotowy |
| [MEM-E0-POWIADOMIENIA-01](MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md) | Szkielet modułu powiadomienia: struktura, publiczny kontrakt, testy architektury | E0 | powiadomienia / szkielet modułu | crud | 1–2 | średnia | gotowy |
| [MEM-E1-KONTO-01](MEM-E1-KONTO-01-zarejestruj-konto-c1.md) | Zarejestruj konto (C1) | E1 | konto / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E1-KONTO-02](MEM-E1-KONTO-02-zaakceptuj-regulamin-c2.md) | Zaakceptuj regulamin (C2) | E1 | konto / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E1-KONTO-03](MEM-E1-KONTO-03-zlec-usuniecie-konta-c3.md) | Zleć usunięcie konta (C3) | E1 | konto / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E1-KONTO-04](MEM-E1-KONTO-04-usun-konto-definitywnie-c4.md) | Usuń konto definitywnie (C4) | E1 | konto / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E1-KONTO-05](MEM-E1-KONTO-05-cofnij-usuniecie-konta-c5.md) | Cofnij usunięcie konta (C5) | E1 | konto / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E2-MIEJSCA-01](MEM-E2-MIEJSCA-01-wybierz-miejsce-z-podpowiedzi-c10.md) | Wybierz miejsce z podpowiedzi (C10) | E2 | miejsca / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E2-MIEJSCA-02](MEM-E2-MIEJSCA-02-dodaj-miejsce-wlasne-c11.md) | Dodaj miejsce własne (C11) | E2 | miejsca / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E2-ARCHIWUM-01](MEM-E2-ARCHIWUM-01-utworz-wspomnienie-c12.md) | Utwórz wspomnienie (C12) | E2 | archiwum / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E2-ARCHIWUM-02](MEM-E2-ARCHIWUM-02-dodaj-zdjecia-do-wspomnienia-c13.md) | Dodaj zdjęcia do wspomnienia (C13) | E2 | archiwum / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E2-NARRACJA-AI-01](MEM-E2-NARRACJA-AI-01-popros-ai-o-szkic-opisu-c15.md) | Poproś AI o szkic opisu (C15) | E2 | narracja-ai / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E2-ARCHIWUM-03](MEM-E2-ARCHIWUM-03-zatwierdz-opis-c16.md) | Zatwierdź opis (C16) | E2 | archiwum / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E2-ARCHIWUM-04](MEM-E2-ARCHIWUM-04-usun-swoja-tresc-c17.md) | Usuń swoją treść (C17) | E2 | archiwum / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E2-ARCHIWUM-05](MEM-E2-ARCHIWUM-05-usun-tresc-definitywnie-c18.md) | Usuń treść definitywnie (C18) | E2 | archiwum / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E2-ARCHIWUM-06](MEM-E2-ARCHIWUM-06-przywroc-tresc-z-kosza-c19.md) | Przywróć treść z kosza (C19) | E2 | archiwum / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E2-ARCHIWUM-07](MEM-E2-ARCHIWUM-07-zloz-zadanie-usuniecia-danych-c20.md) | Złóż żądanie usunięcia danych (C20) | E2 | archiwum / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E10-ARCHIWUM-01](MEM-E10-ARCHIWUM-01-zaimportuj-zdjecia-z-telefonu-c21.md) | Zaimportuj zdjęcia z telefonu (C21) | E10 | archiwum / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E10-ARCHIWUM-02](MEM-E10-ARCHIWUM-02-przypisz-miejsce-grupie-zdjec-c22.md) | Przypisz miejsce grupie zdjęć (C22) | E10 | archiwum / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E10-ARCHIWUM-03](MEM-E10-ARCHIWUM-03-usun-zdjecie-z-poczekalni-c23.md) | Usuń zdjęcie z poczekalni (C23) | E10 | archiwum / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-01](MEM-E3-ALBUM-01-utworz-album-c40.md) | Utwórz album (C40) | E3 | album / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E3-SUKCESJA-01](MEM-E3-SUKCESJA-01-wskaz-nastepce-albumu-c41.md) | Wskaż następcę albumu (C41) | E3 | sukcesja / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-02](MEM-E3-ALBUM-02-zapros-osobe-do-albumu-c42.md) | Zaproś osobę do albumu (C42) | E3 | album / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E3-POWIADOMIENIA-01](MEM-E3-POWIADOMIENIA-01-wyslij-przypomnienie-o-zaproszeniu-c43.md) | Wyślij przypomnienie o zaproszeniu (C43) | E3 | powiadomienia / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E3-ALBUM-03](MEM-E3-ALBUM-03-przyjmij-zaproszenie-c44.md) | Przyjmij zaproszenie (C44) | E3 | album / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E3-ALBUM-04](MEM-E3-ALBUM-04-odrzuc-zaproszenie-c45.md) | Odrzuć zaproszenie (C45) | E3 | album / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E3-POWIADOMIENIA-02](MEM-E3-POWIADOMIENIA-02-zablokuj-zaproszenia-z-geo-na-swoj-adres-c46.md) | Zablokuj zaproszenia z GEO na swój adres (C46) | E3 | powiadomienia / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-05](MEM-E3-ALBUM-05-dodaj-wspomnienie-do-albumu-c47.md) | Dodaj wspomnienie do albumu (C47) | E3 | album / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E3-POWIADOMIENIA-03](MEM-E3-POWIADOMIENIA-03-powiadom-wlasciciela-o-pelnej-puli-c48.md) | Powiadom właściciela o pełnej puli (C48) | E3 | powiadomienia / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E3-ALBUM-06](MEM-E3-ALBUM-06-powierz-tresci-kuratorowi-c49.md) | Powierz treści kuratorowi (C49) | E3 | album / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-07](MEM-E3-ALBUM-07-odwolaj-powierzenie-c50.md) | Odwołaj powierzenie (C50) | E3 | album / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-08](MEM-E3-ALBUM-08-uporzadkuj-powierzone-tresci-c51.md) | Uporządkuj powierzone treści (C51) | E3 | album / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-09](MEM-E3-ALBUM-09-opusc-album-c52.md) | Opuść album (C52) | E3 | album / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E3-ALBUM-10](MEM-E3-ALBUM-10-usun-uczestnika-z-albumu-c53.md) | Usuń uczestnika z albumu (C53) | E3 | album / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E3-POWIADOMIENIA-04](MEM-E3-POWIADOMIENIA-04-popros-o-wybor-zostaw-zabierz-c54.md) | Poproś o wybór zostaw/zabierz (C54) | E3 | powiadomienia / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E3-ALBUM-11](MEM-E3-ALBUM-11-odwolaj-zgode-na-pozostawienie-wkladow-c55.md) | Odwołaj zgodę na pozostawienie wkładów (C55) | E3 | album / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-12](MEM-E3-ALBUM-12-wycofaj-wklady-osoby-z-albumow-c30.md) | Wycofaj wkłady osoby z albumów (C30) | E3 | album / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E4-DOSTEP-01](MEM-E4-DOSTEP-01-udostepnij-album-linkiem-c60.md) | Udostępnij album linkiem (C60) | E4 | dostep / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E4-DOSTEP-02](MEM-E4-DOSTEP-02-wejdz-do-albumu-przez-link-c61.md) | Wejdź do albumu przez link (C61) | E4 | dostep / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E4-DOSTEP-03](MEM-E4-DOSTEP-03-uniewaznij-link-c62.md) | Unieważnij link (C62) | E4 | dostep / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E4-DOSTEP-04](MEM-E4-DOSTEP-04-odbierz-dostep-osobie-c63.md) | Odbierz dostęp osobie (C63) | E4 | dostep / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E4-DOSTEP-05](MEM-E4-DOSTEP-05-zablokuj-uzytkownika-c64.md) | Zablokuj użytkownika (C64) | E4 | dostep / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E5-ALBUM-01](MEM-E5-ALBUM-01-utworz-album-z-odtworzonego-okresu-c71.md) | Utwórz album z odtworzonego okresu (C71) | E5 | album / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E6-NARRACJA-AI-01](MEM-E6-NARRACJA-AI-01-udziel-zgody-na-ai-dla-albumu-c80.md) | Udziel zgody na AI dla albumu (C80) | E6 | narracja-ai / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E6-NARRACJA-AI-02](MEM-E6-NARRACJA-AI-02-odwolaj-zgode-na-ai-c81.md) | Odwołaj zgodę na AI (C81) | E6 | narracja-ai / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E6-NARRACJA-AI-03](MEM-E6-NARRACJA-AI-03-zamow-podsumowanie-okresu-c82.md) | Zamów podsumowanie okresu (C82) | E6 | narracja-ai / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E6-NARRACJA-AI-04](MEM-E6-NARRACJA-AI-04-zatwierdz-i-zapisz-podsumowanie-c83.md) | Zatwierdź i zapisz podsumowanie (C83) | E6 | narracja-ai / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E6-NARRACJA-AI-05](MEM-E6-NARRACJA-AI-05-dodaj-podsumowanie-do-albumu-c84.md) | Dodaj podsumowanie do albumu (C84) | E6 | narracja-ai / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E6-POCHODZENIE-01](MEM-E6-POCHODZENIE-01-uniewaznij-artefakty-pochodne-c31.md) | Unieważnij artefakty pochodne (C31) | E6 | pochodzenie / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E7-PLANY-01](MEM-E7-PLANY-01-nadaj-uprawnienia-planu-c90.md) | Nadaj uprawnienia planu (C90) | E7 | plany / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E7-PLANY-02](MEM-E7-PLANY-02-przelacz-w-tryb-tylko-do-odczytu-c91.md) | Przełącz w tryb tylko do odczytu (C91) | E7 | plany / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E7-PLANY-03](MEM-E7-PLANY-03-odswiez-stan-subskrypcji-u-posrednika-c96.md) | Odśwież stan subskrypcji u pośrednika (C96) | E7 | plany / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E7-POWIADOMIENIA-01](MEM-E7-POWIADOMIENIA-01-wyslij-ostrzezenie-przed-przycieciem-c92.md) | Wyślij ostrzeżenie przed przycięciem (C92) | E7 | powiadomienia / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E7-PLANY-04](MEM-E7-PLANY-04-wstrzymaj-przyciecie-archiwum-c93.md) | Wstrzymaj przycięcie archiwum (C93) | E7 | plany / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E7-PLANY-05](MEM-E7-PLANY-05-wskaz-material-do-zachowania-c94.md) | Wskaż materiał do zachowania (C94) | E7 | plany / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E7-PLANY-06](MEM-E7-PLANY-06-zlec-przyciecie-archiwum-do-darmowego-limitu-c95.md) | Zleć przycięcie archiwum do darmowego limitu (C95) | E7 | plany / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E7-PLANY-07](MEM-E7-PLANY-07-zapros-do-planu-rodzinnego-c97.md) | Zaproś do planu rodzinnego (C97) | E7 | plany / domena + aplikacja | rc | 3–4 | średnia | zablokowany |
| [MEM-E7-PLANY-08](MEM-E7-PLANY-08-dolacz-do-planu-rodzinnego-c98.md) | Dołącz do planu rodzinnego (C98) | E7 | plany / domena + aplikacja | rc | 3–4 | średnia | zablokowany |
| [MEM-E7-PLANY-09](MEM-E7-PLANY-09-usun-osobe-z-planu-rodzinnego-c99.md) | Usuń osobę z planu rodzinnego (C99) | E7 | plany / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E7-PLANY-10](MEM-E7-PLANY-10-przelacz-konto-w-tryb-ponad-limitem-c100.md) | Przełącz konto w tryb ponad limitem (C100) | E7 | plany / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E8-MODERACJA-01](MEM-E8-MODERACJA-01-zglos-tresc-c110.md) | Zgłoś treść (C110) | E8 | moderacja / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E8-MODERACJA-02](MEM-E8-MODERACJA-02-rozstrzygnij-zgloszenie-c111.md) | Rozstrzygnij zgłoszenie (C111) | E8 | moderacja / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E8-MODERACJA-03](MEM-E8-MODERACJA-03-odwolaj-sie-od-decyzji-c112.md) | Odwołaj się od decyzji (C112) | E8 | moderacja / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E8-MODERACJA-04](MEM-E8-MODERACJA-04-ukryj-tresc-natychmiast-c113.md) | Ukryj treść natychmiast (C113) | E8 | moderacja / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E8-MODERACJA-05](MEM-E8-MODERACJA-05-zabezpiecz-dane-konta-c114.md) | Zabezpiecz dane konta (C114) | E8 | moderacja / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E8-MODERACJA-06](MEM-E8-MODERACJA-06-wydaj-dane-organowi-c115.md) | Wydaj dane organowi (C115) | E8 | moderacja / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E9-SUKCESJA-01](MEM-E9-SUKCESJA-01-wskaz-kontakt-spadkowy-c124.md) | Wskaż kontakt spadkowy (C124) | E9 | sukcesja / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E9-SUKCESJA-02](MEM-E9-SUKCESJA-02-zglos-zgon-wlasciciela-c120.md) | Zgłoś zgon właściciela (C120) | E9 | sukcesja / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E9-SUKCESJA-03](MEM-E9-SUKCESJA-03-zweryfikuj-akt-zgonu-c121.md) | Zweryfikuj akt zgonu (C121) | E9 | sukcesja / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E9-SUKCESJA-04](MEM-E9-SUKCESJA-04-przekaz-album-nastepcy-c122.md) | Przekaż album następcy (C122) | E9 | sukcesja / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E9-SUKCESJA-05](MEM-E9-SUKCESJA-05-odmow-przejecia-albumu-c123.md) | Odmów przejęcia albumu (C123) | E9 | sukcesja / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E9-SUKCESJA-06](MEM-E9-SUKCESJA-06-udostepnij-prywatny-album-kontaktowi-spadkowemu-c1.md) | Udostępnij prywatny album kontaktowi spadkowemu (C125) | E9 | sukcesja / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E3-LIMITY-01](MEM-E3-LIMITY-01-zarezerwuj-jednostki-z-puli-c150.md) | Zarezerwuj jednostki z puli (C150) | E3 | limity / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E2-LIMITY-01](MEM-E2-LIMITY-01-zwroc-jednostki-do-puli-c151.md) | Zwróć jednostki do puli (C151) | E2 | limity / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E7-LIMITY-01](MEM-E7-LIMITY-01-ustaw-limit-puli-c152.md) | Ustaw limit puli (C152) | E7 | limity / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E4-DOSTEP-06](MEM-E4-DOSTEP-06-przyznaj-odczyt-zakresu-c65.md) | Przyznaj odczyt zakresu (C65) | E4 | dostep / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E8-DOSTEP-01](MEM-E8-DOSTEP-01-ukryj-zakres-c67.md) | Ukryj zakres (C67) | E8 | dostep / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E7-ARCHIWUM-01](MEM-E7-ARCHIWUM-01-przytnij-archiwum-do-limitu-nadmiar-do-kosza-na-30.md) | Przytnij archiwum do limitu (nadmiar do kosza na 30 dni) (C24) | E7 | archiwum / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E1-ARCHIWUM-01](MEM-E1-ARCHIWUM-01-usun-tresci-konta-c25.md) | Usuń treści konta (C25) | E1 | archiwum / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E3-ALBUM-13](MEM-E3-ALBUM-13-dodaj-zdjecia-do-wspomnienia-w-albumie-c56.md) | Dodaj zdjęcia do wspomnienia w albumie (C56) | E3 | album / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E9-ALBUM-01](MEM-E9-ALBUM-01-zwolnij-role-wlasciciela-albumu-c153.md) | Zwolnij rolę właściciela albumu (C153) | E9 | album / domena + aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E6-POCHODZENIE-02](MEM-E6-POCHODZENIE-02-zarejestruj-artefakt-c154.md) | Zarejestruj artefakt (C154) | E6 | pochodzenie / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E9-SUKCESJA-07](MEM-E9-SUKCESJA-07-uznaj-brak-odpowiedzi-nastepcy-c126.md) | Uznaj brak odpowiedzi następcy (C126) | E9 | sukcesja / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E9-ALBUM-02](MEM-E9-ALBUM-02-zarezerwuj-przejecie-albumu-bez-wlasciciela-c155.md) | Zarezerwuj przejęcie albumu bez właściciela (C155) | E9 | album / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E9-ALBUM-03](MEM-E9-ALBUM-03-przejmij-album-bez-wlasciciela-c156.md) | Przejmij album bez właściciela (C156) | E9 | album / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E9-ALBUM-04](MEM-E9-ALBUM-04-wygas-rezerwacje-przejecia-c157.md) | Wygaś rezerwację przejęcia (C157) | E9 | album / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E7-PLANY-11](MEM-E7-PLANY-11-aktywuj-plan-albumu-po-przejeciu-c165.md) | Aktywuj plan albumu po przejęciu (C165) | E7 | plany / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E9-POWIADOMIENIA-01](MEM-E9-POWIADOMIENIA-01-powiadom-uczestnikow-o-nowym-wlascicielu-c166.md) | Powiadom uczestników o nowym właścicielu (C166) | E9 | powiadomienia / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E9-POWIADOMIENIA-02](MEM-E9-POWIADOMIENIA-02-przypomnij-nastepcy-o-decyzji-7-i-21-dzien-c167.md) | Przypomnij następcy o decyzji (7. i 21. dzień) (C167) | E9 | powiadomienia / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E7-ARCHIWUM-02](MEM-E7-ARCHIWUM-02-przywroc-przyciety-material-z-kosza-c168.md) | Przywróć przycięty materiał z kosza (C168) | E7 | archiwum / domena + aplikacja | rc | 3–4 | średnia | gotowy |
| [MEM-E3-ALBUM-14](MEM-E3-ALBUM-14-oznacz-uczestnika-albumu-we-wspomnieniu-c169.md) | Oznacz uczestnika albumu we wspomnieniu (C169) | E3 | album / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-15](MEM-E3-ALBUM-15-usun-swoje-oznaczenie-c170.md) | Usuń swoje oznaczenie (C170) | E3 | album / aplikacja | crud | 1–3 | wysoka | gotowy |
| [MEM-E3-POWIADOMIENIA-05](MEM-E3-POWIADOMIENIA-05-powiadom-oznaczonego-c171.md) | Powiadom oznaczonego (C171) | E3 | powiadomienia / aplikacja | integration | 2–4 | wysoka | gotowy |
| [MEM-E1-KONTO-06](MEM-E1-KONTO-06-zapytanie-skutki-usuniecia-konta-ile-zdjec-z-ilu-a.md) | Zapytanie: Skutki usunięcia konta (ile zdjęć, z ilu albumów, czyich) (R2) | E1 | konto / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E2-MIEJSCA-03](MEM-E2-MIEJSCA-03-zapytanie-podpowiedzi-miejsc-openstreetmap-miejsca.md) | Zapytanie: Podpowiedzi miejsc (OpenStreetMap + miejsca własne) (R3) | E2 | miejsca / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E10-ARCHIWUM-04](MEM-E10-ARCHIWUM-04-zapytanie-poczekalnia-do-przypisania-r4.md) | Zapytanie: Poczekalnia „do przypisania” (R4) | E10 | archiwum / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-16](MEM-E3-ALBUM-16-zapytanie-miejsca-w-planie-zajete-zaproszone-wolne.md) | Zapytanie: Miejsca w planie: zajęte / zaproszone / wolne (R5) | E3 | album / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E3-LIMITY-02](MEM-E3-LIMITY-02-read-port-stan-puli-albumu-r6.md) | Read port: Stan puli albumu (R6) | E3 | limity / kontrakt | tp | 1–3 | wysoka | gotowy |
| [MEM-E5-ARCHIWUM-01](MEM-E5-ARCHIWUM-01-read-port-wspomnienia-okresu-przecinajace-okres-w-.md) | Read port: Wspomnienia okresu (przecinające okres, w kolejności użytkownika) (R7) | E5 | archiwum / kontrakt | tp | 1–3 | wysoka | gotowy |
| [MEM-E6-NARRACJA-AI-06](MEM-E6-NARRACJA-AI-06-zapytanie-materialy-dozwolone-do-podsumowania-zgod.md) | Zapytanie: Materiały dozwolone do podsumowania (zgody, uprawnienia) (R8) | E6 | narracja-ai / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E8-MODERACJA-07](MEM-E8-MODERACJA-07-zapytanie-kolejka-zgloszen-r9.md) | Zapytanie: Kolejka zgłoszeń (R9) | E8 | moderacja / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E4-DOSTEP-07](MEM-E4-DOSTEP-07-zapytanie-imienna-lista-ogladajacych-albumu-r10.md) | Zapytanie: Imienna lista oglądających albumu (R10) | E4 | dostep / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E2-ARCHIWUM-08](MEM-E2-ARCHIWUM-08-zapytanie-mapa-wspomnien-punkty-wlasne-z-albumow-d.md) | Zapytanie: Mapa wspomnień (punkty: własne + z albumów, do których mam dostęp; filtry dat, osób, albumów) (R11) | E2 | archiwum / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E2-ARCHIWUM-09](MEM-E2-ARCHIWUM-09-zapytanie-szczegoly-wspomnienia-miejsce-data-z-pre.md) | Zapytanie: Szczegóły wspomnienia (miejsce, data z precyzją, treści per autor, osoby: tekst autora albo oznaczeni uczestnicy albumu) (R12) | E2 | archiwum / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-17](MEM-E3-ALBUM-17-zapytanie-album-wspomnienia-sklad-i-moje-wklady-r1.md) | Zapytanie: Album: wspomnienia, skład i moje wkłady (R13) | E3 | album / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E3-DOSTEP-01](MEM-E3-DOSTEP-01-zapytanie-udostepnione-mi-albumy-z-odczytem-r14.md) | Zapytanie: Udostępnione mi (albumy z odczytem) (R14) | E3 | dostep / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E2-ARCHIWUM-10](MEM-E2-ARCHIWUM-10-zapytanie-kosz-moje-tresci-usuniete-w-ostatnich-30.md) | Zapytanie: Kosz (moje treści usunięte w ostatnich 30 dniach) (R15) | E2 | archiwum / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E7-PLANY-12](MEM-E7-PLANY-12-zapytanie-stan-planu-tryb-terminy-zuzycie-wybor-do.md) | Zapytanie: Stan planu (tryb, terminy, zużycie, wybór do zachowania) (R16) | E7 | plany / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E9-SUKCESJA-08](MEM-E9-SUKCESJA-08-zapytanie-sprawa-przejecia-albumu-album-termin-los.md) | Zapytanie: Sprawa przejęcia albumu (album, termin, losy treści fundatora) (R17) | E9 | sukcesja / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E8-MODERACJA-08](MEM-E8-MODERACJA-08-zapytanie-moje-zgloszenia-i-decyzje-moderacji-z-uz.md) | Zapytanie: Moje zgłoszenia i decyzje moderacji (z uzasadnieniem) (R18) | E8 | moderacja / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-18](MEM-E3-ALBUM-18-zapytanie-zaproszenie-kto-zaprasza-nazwa-albumu-be.md) | Zapytanie: Zaproszenie (kto zaprasza, nazwa albumu — bez treści) (R19) | E3 | album / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E9-ALBUM-05](MEM-E9-ALBUM-05-zapytanie-album-bez-wlasciciela-stan-przejecia-czy.md) | Zapytanie: Album bez właściciela: stan przejęcia (czy można przejąć, trwająca rezerwacja i do kiedy) (R20) | E9 | album / infrastruktura (query handler) | tp | 1–3 | wysoka | gotowy |
| [MEM-E2-MIEJSCA-04](MEM-E2-MIEJSCA-04-read-port-miejsca-dla-archiwum-odwolanie-do-miejsc.md) | Read port miejsca → dla archiwum: odwołanie do miejsca (wpis OSM albo miejsce własne) i jego istnienie | E2 | miejsca / kontrakt | tp | 1–2 | wysoka | gotowy |
| [MEM-E2-LIMITY-02](MEM-E2-LIMITY-02-write-port-limity-dla-archiwum-zarezerwuj-zwroc-pr.md) | Write port limity → dla archiwum: zarezerwuj / zwróć przestrzeń w puli konta | E2 | limity / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E3-LIMITY-03](MEM-E3-LIMITY-03-write-port-limity-dla-album-zarezerwuj-przestrzen-.md) | Write port limity → dla album: zarezerwuj przestrzeń w puli albumu | E3 | limity / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E3-ARCHIWUM-01](MEM-E3-ARCHIWUM-01-write-port-archiwum-dla-album-zapisz-tresc-z-numer.md) | Write port archiwum → dla album: zapisz treść z numerem puli albumu | E3 | archiwum / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E3-PLANY-01](MEM-E3-PLANY-01-read-port-plany-dla-album-limit-miejsc-dla-albumu.md) | Read port plany → dla album: limit miejsc dla albumu | E3 | plany / kontrakt | tp | 1–2 | wysoka | gotowy |
| [MEM-E3-POWIADOMIENIA-06](MEM-E3-POWIADOMIENIA-06-read-port-powiadomienia-dla-album-czy-adres-zablok.md) | Read port powiadomienia → dla album: czy adres zablokował zaproszenia | E3 | powiadomienia / kontrakt | tp | 1–2 | wysoka | gotowy |
| [MEM-E6-DOSTEP-01](MEM-E6-DOSTEP-01-read-port-dostep-dla-narracja-ai-ktore-tresci-osob.md) | Read port dostep → dla narracja-ai: które treści osoba może czytać teraz | E6 | dostep / kontrakt | tp | 1–2 | wysoka | gotowy |
| [MEM-E2-LIMITY-03](MEM-E2-LIMITY-03-write-port-limity-dla-narracja-ai-zuzyj-limit-ai.md) | Write port limity → dla narracja-ai: zużyj limit AI | E2 | limity / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E6-POCHODZENIE-03](MEM-E6-POCHODZENIE-03-write-port-pochodzenie-dla-narracja-ai-zarejestruj.md) | Write port pochodzenie → dla narracja-ai: zarejestruj artefakt z listą źródeł (C154) | E6 | pochodzenie / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E8-DOSTEP-02](MEM-E8-DOSTEP-02-write-port-dostep-dla-moderacja-ukryj-zakres-natyc.md) | Write port dostep → dla moderacja: ukryj zakres natychmiast i potwierdź (nakaz 1 h) | E8 | dostep / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E1-ARCHIWUM-02](MEM-E1-ARCHIWUM-02-read-port-archiwum-dla-konto-ile-tresci-wypadnie-p.md) | Read port archiwum → dla konto: ile treści wypadnie przy usunięciu konta | E1 | archiwum / kontrakt | tp | 1–2 | wysoka | gotowy |
| [MEM-E1-ALBUM-01](MEM-E1-ALBUM-01-read-port-album-dla-konto-z-ilu-albumow-czy-wlasci.md) | Read port album → dla konto: z ilu albumów, czy właściciel bez następcy | E1 | album / kontrakt | tp | 1–2 | wysoka | gotowy |
| [MEM-E9-ALBUM-06](MEM-E9-ALBUM-06-write-port-album-dla-sukcesja-zmien-wlasciciela-zw.md) | Write port album → dla sukcesja: zmień właściciela / zwolnij rolę właściciela (C153) | E9 | album / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E1-KONTO-07](MEM-E1-KONTO-07-polityka-p1-zawsze-gdy-okno-usuniecia-konta-minie-.md) | Polityka P1: Zawsze gdy okno usunięcia konta minie, wtedy usuń konto definitywnie | E1 | konto / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E1-KONTO-08](MEM-E1-KONTO-08-zdarzenie-integracyjne-konto-usuniete-e7-w-kontrak.md) | Zdarzenie integracyjne „Konto usunięte” (E7) w kontrakcie modułu konto | E1 | konto / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E3-ALBUM-19](MEM-E3-ALBUM-19-polityka-p2-zawsze-gdy-konto-usuniete-wtedy-wycofa.md) | Polityka P2: Zawsze gdy konto usunięte, wtedy wycofaj wkłady tej osoby z cudzych albumów | E3 | album / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E2-ARCHIWUM-11](MEM-E2-ARCHIWUM-11-polityka-p3-zawsze-gdy-okno-kosza-minie-wtedy-usun.md) | Polityka P3: Zawsze gdy okno kosza minie, wtedy usuń treść definitywnie | E2 | archiwum / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E2-ARCHIWUM-12](MEM-E2-ARCHIWUM-12-zdarzenie-integracyjne-tresc-usunieta-definitywnie.md) | Zdarzenie integracyjne „Treść usunięta definitywnie” (E22) w kontrakcie modułu archiwum | E2 | archiwum / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E6-POCHODZENIE-04](MEM-E6-POCHODZENIE-04-polityka-p4-zawsze-gdy-tresc-usunieta-definitywnie.md) | Polityka P4: Zawsze gdy treść usunięta definitywnie, wtedy unieważnij artefakty pochodne, które ją cytują | E6 | pochodzenie / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E2-ARCHIWUM-13](MEM-E2-ARCHIWUM-13-polityka-p5-zawsze-gdy-zdjecia-maja-przypisane-mie.md) | Polityka P5: Zawsze gdy zdjęcia mają przypisane miejsce, wtedy utwórz z nich wspomnienie | E2 | archiwum / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-20](MEM-E3-ALBUM-20-zdarzenie-integracyjne-minelo-7-dni-od-zaproszenia.md) | Zdarzenie integracyjne „Minęło 7 dni od zaproszenia” (E45) w kontrakcie modułu album | E3 | album / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E3-POWIADOMIENIA-07](MEM-E3-POWIADOMIENIA-07-polityka-p6-zawsze-gdy-minie-7-dni-od-nieprzyjeteg.md) | Polityka P6: Zawsze gdy minie 7 dni od nieprzyjętego zaproszenia, wtedy wyślij jedno przypomnienie | E3 | powiadomienia / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-21](MEM-E3-ALBUM-21-zdarzenie-integracyjne-pula-albumu-wyczerpana-e51-.md) | Zdarzenie integracyjne „Pula albumu wyczerpana” (E51) w kontrakcie modułu album | E3 | album / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E3-POWIADOMIENIA-08](MEM-E3-POWIADOMIENIA-08-polityka-p7-zawsze-gdy-pula-albumu-wyczerpana-wted.md) | Polityka P7: Zawsze gdy pula albumu wyczerpana, wtedy powiadom właściciela z prośbą o większy plan | E3 | powiadomienia / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-22](MEM-E3-ALBUM-22-zdarzenie-integracyjne-uczestnik-usuniety-z-albumu.md) | Zdarzenie integracyjne „Uczestnik usunięty z albumu” (E59) w kontrakcie modułu album | E3 | album / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E3-POWIADOMIENIA-09](MEM-E3-POWIADOMIENIA-09-polityka-p8-zawsze-gdy-uczestnik-usuniety-z-albumu.md) | Polityka P8: Zawsze gdy uczestnik usunięty z albumu, wtedy poproś go o wybór zostaw/zabierz z terminem | E3 | powiadomienia / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-23](MEM-E3-ALBUM-23-zdarzenie-integracyjne-wklady-wycofane-z-albumu-e5.md) | Zdarzenie integracyjne „Wkłady wycofane z albumu” (E58) w kontrakcie modułu album | E3 | album / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E6-POCHODZENIE-05](MEM-E6-POCHODZENIE-05-polityka-p9-zawsze-gdy-wklady-wycofane-z-albumu-wt.md) | Polityka P9: Zawsze gdy wkłady wycofane z albumu, wtedy unieważnij artefakty pochodne, które je cytują | E6 | pochodzenie / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E4-DOSTEP-08](MEM-E4-DOSTEP-08-zdarzenie-integracyjne-dostep-odebrany-e66-w-kontr.md) | Zdarzenie integracyjne „Dostęp odebrany” (E66) w kontrakcie modułu dostep | E4 | dostep / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E6-POCHODZENIE-06](MEM-E6-POCHODZENIE-06-polityka-p10-zawsze-gdy-dostep-odebrany-wtedy-unie.md) | Polityka P10: Zawsze gdy dostęp odebrany, wtedy unieważnij artefakty pochodne powstałe z tego albumu u tej osoby | E6 | pochodzenie / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E6-NARRACJA-AI-07](MEM-E6-NARRACJA-AI-07-zdarzenie-integracyjne-zgoda-na-ai-odwolana-e81-w-.md) | Zdarzenie integracyjne „Zgoda na AI odwołana” (E81) w kontrakcie modułu narracja-ai | E6 | narracja-ai / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E6-POCHODZENIE-07](MEM-E6-POCHODZENIE-07-polityka-p12-zawsze-gdy-zgoda-na-ai-odwolana-wtedy.md) | Polityka P12: Zawsze gdy zgoda na AI odwołana, wtedy unieważnij artefakty powstałe z treści tej osoby | E6 | pochodzenie / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E7-PLANY-13](MEM-E7-PLANY-13-polityka-p13-zawsze-gdy-subskrypcja-aktywowana-wte.md) | Polityka P13: Zawsze gdy subskrypcja aktywowana, wtedy nadaj uprawnienia planu | E7 | plany / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E7-PLANY-14](MEM-E7-PLANY-14-polityka-p14-zawsze-gdy-subskrypcja-wygasla-wtedy-.md) | Polityka P14: Zawsze gdy subskrypcja wygasła, wtedy przełącz album lub konto w tryb tylko do odczytu | E7 | plany / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E7-PLANY-15](MEM-E7-PLANY-15-zdarzenie-integracyjne-nadszedl-termin-ostrzezenia.md) | Zdarzenie integracyjne „Nadszedł termin ostrzeżenia (90/30/7/1 dni przed przycięciem)” (E97) w kontrakcie modułu plany | E7 | plany / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E7-POWIADOMIENIA-02](MEM-E7-POWIADOMIENIA-02-polityka-p15-zawsze-gdy-nadejdzie-termin-ostrzezen.md) | Polityka P15: Zawsze gdy nadejdzie termin ostrzeżenia, wtedy ostrzeż właściciela i uczestników | E7 | powiadomienia / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E7-POWIADOMIENIA-03](MEM-E7-POWIADOMIENIA-03-zdarzenie-integracyjne-ostrzezenie-niedoreczone-e9.md) | Zdarzenie integracyjne „Ostrzeżenie niedoręczone” (E99) w kontrakcie modułu powiadomienia | E7 | powiadomienia / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E7-PLANY-16](MEM-E7-PLANY-16-polityka-p16-zawsze-gdy-ostrzezenie-niedoreczone-w.md) | Polityka P16: Zawsze gdy ostrzeżenie niedoręczone, wtedy wstrzymaj przycięcie | E7 | plany / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E7-PLANY-17](MEM-E7-PLANY-17-zdarzenie-integracyjne-przyciecie-wstrzymane-e100-.md) | Zdarzenie integracyjne „Przycięcie wstrzymane” (E100) w kontrakcie modułu plany | E7 | plany / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E7-POWIADOMIENIA-04](MEM-E7-POWIADOMIENIA-04-polityka-p26-zawsze-gdy-przyciecie-wstrzymane-wted.md) | Polityka P26: Zawsze gdy przycięcie wstrzymane, wtedy ponawiaj ostrzeżenie co miesiąc aż do skutecznego doręczenia | E7 | powiadomienia / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E7-PLANY-18](MEM-E7-PLANY-18-polityka-p17-zawsze-gdy-mina-24-miesiace-tylko-do-.md) | Polityka P17: Zawsze gdy miną 24 miesiące tylko do odczytu, wtedy przytnij archiwum do darmowego limitu | E7 | plany / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E7-PLANY-19](MEM-E7-PLANY-19-polityka-p18-zawsze-gdy-osoba-wypadnie-z-planu-rod.md) | Polityka P18: Zawsze gdy osoba wypadnie z planu rodzinnego, wtedy przełącz jej konto w tryb „nic nowego ponad limit” | E7 | plany / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E8-MODERACJA-09](MEM-E8-MODERACJA-09-polityka-p19-zawsze-gdy-przyjdzie-nakaz-usuniecia-.md) | Polityka P19: Zawsze gdy przyjdzie nakaz usunięcia, wtedy natychmiast ukryj treść | E8 | moderacja / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E8-MODERACJA-10](MEM-E8-MODERACJA-10-polityka-p20-zawsze-gdy-przyjdzie-nakaz-wydania-lu.md) | Polityka P20: Zawsze gdy przyjdzie nakaz wydania lub zabezpieczenia danych, wtedy zabezpiecz dane konta | E8 | moderacja / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E9-SUKCESJA-09](MEM-E9-SUKCESJA-09-polityka-p21-zawsze-gdy-zgon-potwierdzony-wtedy-pr.md) | Polityka P21: Zawsze gdy zgon potwierdzony, wtedy przekaż albumy współdzielone następcom | E9 | sukcesja / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E9-SUKCESJA-10](MEM-E9-SUKCESJA-10-polityka-p23-zawsze-gdy-zgon-potwierdzony-wtedy-ud.md) | Polityka P23: Zawsze gdy zgon potwierdzony, wtedy udostępnij albumy prywatne kontaktowi spadkowemu (tylko odczyt) | E9 | sukcesja / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E7-PLANY-20](MEM-E7-PLANY-20-zdarzenie-integracyjne-plan-aktywny-e94-w-kontrakc.md) | Zdarzenie integracyjne „Plan aktywny” (E94) w kontrakcie modułu plany | E7 | plany / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E7-PLANY-21](MEM-E7-PLANY-21-zdarzenie-integracyjne-album-lub-konto-w-trybie-ty.md) | Zdarzenie integracyjne „Album lub konto w trybie tylko do odczytu” (E95) w kontrakcie modułu plany | E7 | plany / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E7-LIMITY-02](MEM-E7-LIMITY-02-polityka-p27-zawsze-gdy-plan-aktywny-albo-przelacz.md) | Polityka P27: Zawsze gdy plan aktywny albo przełączony w tryb tylko do odczytu, wtedy ustaw limit puli | E7 | limity / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-24](MEM-E3-ALBUM-24-zdarzenie-integracyjne-zaproszenie-przyjete-e47-w-.md) | Zdarzenie integracyjne „Zaproszenie przyjęte” (E47) w kontrakcie modułu album | E3 | album / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E4-DOSTEP-09](MEM-E4-DOSTEP-09-polityka-p29-zawsze-gdy-zaproszenie-przyjete-wtedy.md) | Polityka P29: Zawsze gdy zaproszenie przyjęte, wtedy przyznaj odczyt albumu | E4 | dostep / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-25](MEM-E3-ALBUM-25-zdarzenie-integracyjne-uczestnik-opuscil-album-mie.md) | Zdarzenie integracyjne „Uczestnik opuścił album (miejsce zwolnione)” (E56) w kontrakcie modułu album | E3 | album / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E4-DOSTEP-10](MEM-E4-DOSTEP-10-polityka-p30-zawsze-gdy-uczestnik-opuscil-album-al.md) | Polityka P30: Zawsze gdy uczestnik opuścił album albo został usunięty, wtedy odbierz mu odczyt | E4 | dostep / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E9-SUKCESJA-11](MEM-E9-SUKCESJA-11-zdarzenie-integracyjne-album-prywatny-udostepniony.md) | Zdarzenie integracyjne „Album prywatny udostępniony kontaktowi spadkowemu” (E126) w kontrakcie modułu sukcesja | E9 | sukcesja / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E4-DOSTEP-11](MEM-E4-DOSTEP-11-polityka-p31-zawsze-gdy-album-prywatny-udostepnion.md) | Polityka P31: Zawsze gdy album prywatny udostępniony kontaktowi spadkowemu, wtedy przyznaj mu odczyt | E4 | dostep / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E8-MODERACJA-11](MEM-E8-MODERACJA-11-zdarzenie-integracyjne-tresc-ukryta-e113-w-kontrak.md) | Zdarzenie integracyjne „Treść ukryta” (E113) w kontrakcie modułu moderacja | E8 | moderacja / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E8-DOSTEP-03](MEM-E8-DOSTEP-03-polityka-p32-zawsze-gdy-tresc-ukryta-decyzja-moder.md) | Polityka P32: Zawsze gdy treść ukryta decyzją moderacji, wtedy ukryj zakres w Dostępie | E8 | dostep / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E7-PLANY-22](MEM-E7-PLANY-22-zdarzenie-integracyjne-przyciecie-archiwum-zlecone.md) | Zdarzenie integracyjne „Przycięcie archiwum zlecone” (E103) w kontrakcie modułu plany | E7 | plany / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E7-ARCHIWUM-03](MEM-E7-ARCHIWUM-03-polityka-p33-zawsze-gdy-przyciecie-zlecone-wtedy-p.md) | Polityka P33: Zawsze gdy przycięcie zlecone, wtedy przytnij archiwum według wyboru właściciela albo od najstarszego | E7 | archiwum / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E7-ARCHIWUM-04](MEM-E7-ARCHIWUM-04-zdarzenie-integracyjne-archiwum-przyciete-nadmiar-.md) | Zdarzenie integracyjne „Archiwum przycięte (nadmiar w koszu 30 dni)” (E25) w kontrakcie modułu archiwum | E7 | archiwum / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E6-POCHODZENIE-08](MEM-E6-POCHODZENIE-08-polityka-p34-zawsze-gdy-archiwum-przyciete-wtedy-u.md) | Polityka P34: Zawsze gdy archiwum przycięte, wtedy unieważnij artefakty cytujące usunięty materiał | E6 | pochodzenie / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E1-ARCHIWUM-03](MEM-E1-ARCHIWUM-03-polityka-p35-zawsze-gdy-konto-usuniete-wtedy-usun-.md) | Polityka P35: Zawsze gdy konto usunięte, wtedy usuń treści tej osoby | E1 | archiwum / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E9-ALBUM-07](MEM-E9-ALBUM-07-zdarzenie-integracyjne-album-bez-wlasciciela-e162-.md) | Zdarzenie integracyjne „Album bez właściciela” (E162) w kontrakcie modułu album | E9 | album / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E7-PLANY-23](MEM-E7-PLANY-23-polityka-p36-zawsze-gdy-album-bez-wlasciciela-wted.md) | Polityka P36: Zawsze gdy album bez właściciela, wtedy przełącz plan albumu w tryb tylko do odczytu po końcu opłaconego okresu | E7 | plany / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E9-ALBUM-08](MEM-E9-ALBUM-08-polityka-p37-zawsze-gdy-konto-wlasciciela-bez-nast.md) | Polityka P37: Zawsze gdy konto właściciela bez następcy usunięte (ze zgodą na wygaśnięcie albumu), wtedy zwolnij rolę właściciela | E9 | album / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E9-SUKCESJA-12](MEM-E9-SUKCESJA-12-polityka-p38-zawsze-gdy-termin-przejecia-minal-wte.md) | Polityka P38: Zawsze gdy termin przejęcia minął, wtedy uznaj brak odpowiedzi następcy | E9 | sukcesja / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E9-ALBUM-09](MEM-E9-ALBUM-09-polityka-p39-zawsze-gdy-minie-30-minut-rezerwacji-.md) | Polityka P39: Zawsze gdy minie 30 minut rezerwacji, wtedy wygaś rezerwację przejęcia | E9 | album / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E7-PLANY-24](MEM-E7-PLANY-24-zdarzenie-integracyjne-plan-albumu-bez-wlasciciela.md) | Zdarzenie integracyjne „Plan albumu bez właściciela opłacony przez uczestnika” (E171) w kontrakcie modułu plany | E7 | plany / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E9-ALBUM-10](MEM-E9-ALBUM-10-polityka-p40-zawsze-gdy-plan-albumu-bez-wlasciciel.md) | Polityka P40: Zawsze gdy plan albumu bez właściciela opłacony przez uczestnika, wtedy przejmij album (jeśli to jego aktywna rezerwacja) | E9 | album / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E9-ALBUM-11](MEM-E9-ALBUM-11-zdarzenie-integracyjne-album-przejety-przez-uczest.md) | Zdarzenie integracyjne „Album przejęty przez uczestnika” (E167) w kontrakcie modułu album | E9 | album / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E7-PLANY-25](MEM-E7-PLANY-25-polityka-p41-zawsze-gdy-album-przejety-przez-uczes.md) | Polityka P41: Zawsze gdy album przejęty przez uczestnika, wtedy aktywuj plan albumu | E7 | plany / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E9-POWIADOMIENIA-03](MEM-E9-POWIADOMIENIA-03-polityka-p42-zawsze-gdy-album-przejety-przez-uczes.md) | Polityka P42: Zawsze gdy album przejęty przez uczestnika, wtedy powiadom pozostałych (mogą wyjść z wyborem zostaw/zabierz) | E9 | powiadomienia / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E9-SUKCESJA-13](MEM-E9-SUKCESJA-13-zdarzenie-integracyjne-zgon-potwierdzony-e121-w-ko.md) | Zdarzenie integracyjne „Zgon potwierdzony” (E121) w kontrakcie modułu sukcesja | E9 | sukcesja / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E9-POWIADOMIENIA-04](MEM-E9-POWIADOMIENIA-04-polityka-p43-zawsze-gdy-zgon-potwierdzony-wtedy-za.md) | Polityka P43: Zawsze gdy zgon potwierdzony, wtedy zaplanuj przypomnienia dla następcy (7. i 21. dzień) | E9 | powiadomienia / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E7-ARCHIWUM-05](MEM-E7-ARCHIWUM-05-polityka-p44-zawsze-gdy-plan-znowu-aktywny-w-ciagu.md) | Polityka P44: Zawsze gdy plan znowu aktywny w ciągu 30 dni od przycięcia, wtedy przywróć przycięty materiał z kosza | E7 | archiwum / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E3-ALBUM-26](MEM-E3-ALBUM-26-zdarzenie-integracyjne-uczestnik-oznaczony-we-wspo.md) | Zdarzenie integracyjne „Uczestnik oznaczony we wspomnieniu” (E175) w kontrakcie modułu album | E3 | album / kontrakt | integration | 1–2 | wysoka | gotowy |
| [MEM-E3-POWIADOMIENIA-10](MEM-E3-POWIADOMIENIA-10-polityka-p45-zawsze-gdy-uczestnik-oznaczony-wtedy-.md) | Polityka P45: Zawsze gdy uczestnik oznaczony, wtedy powiadom go (może usunąć oznaczenie) | E3 | powiadomienia / aplikacja (handler zdarzenia) | integration | 1–3 | wysoka | gotowy |
| [MEM-E0-PLAT-19](MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md) | Nawigacja aplikacji: zakładki Mapa · Albumy · Udostępnione mi · Konto, typy tras, przekierowanie niezalogowanych (S02) | E0 | - / UI | tp | 2–3 | wysoka | gotowy |
| [MEM-E1-PLAT-01](MEM-E1-PLAT-01-ekran-s01-rejestracja.md) | Ekran S01 „Rejestracja” | E1 | - / UI | crud | 2–4 | średnia | gotowy |
| [MEM-E2-PLAT-01](MEM-E2-PLAT-01-ekran-s03-mapa-i-globus.md) | Ekran S03 „Mapa i globus” | E2 | - / UI | tp | 2–4 | średnia | gotowy |
| [MEM-E2-PLAT-02](MEM-E2-PLAT-02-ekran-s04-nowe-wspomnienie-kreator-miejsce-tresc.md) | Ekran S04 „Nowe wspomnienie (kreator: miejsce → treść)” | E2 | - / UI | crud | 4–6 | niska | gotowy |
| [MEM-E2-PLAT-03](MEM-E2-PLAT-03-ekran-s05-szczegoly-wspomnienia.md) | Ekran S05 „Szczegóły wspomnienia” | E2 | - / UI | crud | 2–4 | średnia | gotowy |
| [MEM-E2-PLAT-04](MEM-E2-PLAT-04-ekran-s06-kosz.md) | Ekran S06 „Kosz” | E2 | - / UI | rc | 2–4 | średnia | gotowy |
| [MEM-E10-PLAT-01](MEM-E10-PLAT-01-ekran-s07-import-zdjec-i-poczekalnia.md) | Ekran S07 „Import zdjęć i poczekalnia” | E10 | - / UI | crud | 2–4 | średnia | gotowy |
| [MEM-E5-PLAT-01](MEM-E5-PLAT-01-ekran-s08-przesledz-okres.md) | Ekran S08 „Prześledź okres” | E5 | - / UI | crud | 2–4 | średnia | gotowy |
| [MEM-E3-PLAT-01](MEM-E3-PLAT-01-ekran-s09-albumy-lista-i-nowy-album.md) | Ekran S09 „Albumy (lista) i nowy album” | E3 | - / UI | crud | 2–4 | średnia | gotowy |
| [MEM-E3-PLAT-02](MEM-E3-PLAT-02-ekran-s10-album.md) | Ekran S10 „Album” | E3 | - / UI | rc | 4–6 | niska | gotowy |
| [MEM-E3-PLAT-03](MEM-E3-PLAT-03-ekran-s11-zarzadzanie-albumem-wlasciciel.md) | Ekran S11 „Zarządzanie albumem (właściciel)” | E3 | - / UI | rc | 4–6 | niska | gotowy |
| [MEM-E3-PLAT-04](MEM-E3-PLAT-04-ekran-s12-moje-zgody-i-powierzenia-w-albumie.md) | Ekran S12 „Moje zgody i powierzenia w albumie” | E3 | - / UI | crud | 2–4 | średnia | gotowy |
| [MEM-E3-PLAT-05](MEM-E3-PLAT-05-ekran-s13-powierzone-tresci-kurator.md) | Ekran S13 „Powierzone treści (kurator)” | E3 | - / UI | crud | 2–4 | średnia | gotowy |
| [MEM-E3-PLAT-06](MEM-E3-PLAT-06-ekran-s14-zaproszenie-do-albumu.md) | Ekran S14 „Zaproszenie do albumu” | E3 | - / UI | rc | 2–4 | średnia | gotowy |
| [MEM-E4-PLAT-01](MEM-E4-PLAT-01-ekran-s15-wejscie-do-albumu-przez-link.md) | Ekran S15 „Wejście do albumu przez link” | E4 | - / UI | rc | 2–4 | średnia | gotowy |
| [MEM-E4-PLAT-02](MEM-E4-PLAT-02-ekran-s16-udostepnione-mi.md) | Ekran S16 „Udostępnione mi” | E4 | - / UI | tp | 2–4 | średnia | gotowy |
| [MEM-E6-PLAT-01](MEM-E6-PLAT-01-ekran-s17-podsumowanie-okresu-ai.md) | Ekran S17 „Podsumowanie okresu (AI)” | E6 | - / UI | crud | 2–4 | średnia | gotowy |
| [MEM-E7-PLAT-01](MEM-E7-PLAT-01-ekran-s18-plan-i-subskrypcja.md) | Ekran S18 „Plan i subskrypcja” | E7 | - / UI | rc | 2–4 | średnia | gotowy |
| [MEM-E7-PLAT-02](MEM-E7-PLAT-02-ekran-s18-plan-i-subskrypcja-akcje-dolacz-do-planu.md) | Ekran S18 „Plan i subskrypcja”: akcje „Dołącz do planu rodzinnego” | E7 | - / UI | rc | 1–2 | średnia | zablokowany |
| [MEM-E7-PLAT-03](MEM-E7-PLAT-03-ekran-s19-wybierz-co-zostanie-po-przycieciu.md) | Ekran S19 „Wybierz, co zostanie po przycięciu” | E7 | - / UI | rc | 2–4 | średnia | gotowy |
| [MEM-E7-PLAT-04](MEM-E7-PLAT-04-ekran-s20-plan-rodzinny.md) | Ekran S20 „Plan rodzinny” | E7 | - / UI | rc | 2–4 | średnia | gotowy |
| [MEM-E8-PLAT-01](MEM-E8-PLAT-01-ekran-s21-zglos-tresc.md) | Ekran S21 „Zgłoś treść” | E8 | - / UI | crud | 2–4 | średnia | gotowy |
| [MEM-E8-PLAT-02](MEM-E8-PLAT-02-ekran-s22-moje-zgloszenia-i-decyzje.md) | Ekran S22 „Moje zgłoszenia i decyzje” | E8 | - / UI | crud | 2–4 | średnia | gotowy |
| [MEM-E9-PLAT-01](MEM-E9-PLAT-01-ekran-s23-przejecie-albumu-nastepca.md) | Ekran S23 „Przejęcie albumu (następca)” | E9 | - / UI | rc | 2–4 | średnia | gotowy |
| [MEM-E1-PLAT-02](MEM-E1-PLAT-02-ekran-s24-konto-i-prywatnosc.md) | Ekran S24 „Konto i prywatność” | E1 | - / UI | rc | 2–4 | średnia | gotowy |
| [MEM-E9-PLAT-02](MEM-E9-PLAT-02-ekran-s25-zglos-zgon-wlasciciela.md) | Ekran S25 „Zgłoś zgon właściciela” | E9 | - / UI | crud | 2–4 | średnia | gotowy |
| [MEM-E8-PLAT-03](MEM-E8-PLAT-03-ekran-s26-panel-obslugi-zgloszenia-nakazy-sukcesja.md) | Ekran S26 „Panel obsługi: zgłoszenia, nakazy, sukcesja” | E8 | - / UI | rc | 4–6 | niska | gotowy |
| [MEM-E8-PLAT-04](MEM-E8-PLAT-04-ekran-s27-zglos-tresc-z-aplikacji.md) | Ekran S27 „Zgłoś treść (z aplikacji)” | E8 | - / UI | crud | 2–4 | średnia | gotowy |
| [MEM-E11-PLAT-01](MEM-E11-PLAT-01-dpia-przed-pierwszym-uzytkownikiem-z8.md) | DPIA przed pierwszym użytkownikiem (Z8) | E11 | - / proces / dokument | crud | 2–4 | niska | gotowy |
| [MEM-E11-PLAT-02](MEM-E11-PLAT-02-umowy-powierzenia-z-dostawcami-rejestr-czynnosci-l.md) | Umowy powierzenia z dostawcami + rejestr czynności + lista podwykonawców (D-046) | E11 | - / proces / dokument | crud | 2–4 | niska | gotowy |
| [MEM-E11-PLAT-03](MEM-E11-PLAT-03-procedura-z6-nakazy-organow-terreg-1-h-e-dowody-10.md) | Procedura Z6: nakazy organów (TERREG 1 h, e-dowody 10 dni, zabezpieczenie 60+30 dni) i zgłaszanie podejrzeń przestępstw (DSA art. 18) | E11 | - / proces / dokument | crud | 2–4 | niska | gotowy |
| [MEM-E11-PLAT-04](MEM-E11-PLAT-04-dyzur-1-h-pod-alertem-punktu-kontaktowego.md) | Dyżur 1 h pod alertem punktu kontaktowego | E11 | - / proces / dokument | crud | 2–4 | niska | zablokowany |
| [MEM-E11-PLAT-05](MEM-E11-PLAT-05-polityka-zglaszania-podatnosci-cra-security-txt-ko.md) | Polityka zgłaszania podatności CRA: security.txt, kontakt, runbook 24 h/72 h (D-052) | E11 | - / proces / dokument | crud | 2–4 | niska | gotowy |
| [MEM-E11-PLAT-06](MEM-E11-PLAT-06-retencja-aktow-zgonu-osobny-prefiks-magazynu-czas-.md) | Retencja aktów zgonu: osobny prefiks magazynu, czas do zamknięcia sprawy + 12 mies., dostęp tylko obsługa (D-055) | E11 | - / proces / dokument | crud | 2–4 | niska | gotowy |
| [MEM-E11-PLAT-07](MEM-E11-PLAT-07-ekran-zgody-przy-pierwszym-uruchomieniu-analityka-.md) | Ekran zgody przy pierwszym uruchomieniu: analityka i powiadomienia spoza „dotyczących danych” (D-050) | E11 | - / UI | crud | 2–3 | średnia | gotowy |
| [MEM-E11-PLAT-08](MEM-E11-PLAT-08-sekcja-podwykonawcy-w-ekranie-konto-i-prywatnosc-s.md) | Sekcja „Podwykonawcy” w ekranie Konto i prywatność (S24) | E11 | - / UI | tp | 1–2 | wysoka | gotowy |

## Backlog z wyzwalaczem (nie planować przed wyzwalaczem)

- Etap 2 (AWS UE): MAU > 5 tys. przez 30 dni / dostępność < 99% przez 2 mies. / umowa ≥ 99,5% / p95 API > 2 s przez 7 dni / przestoje wdrożeń > 50% budżetu (plan infrastruktury)
- Broker kolejek (RabbitMQ/SQS) w Wolverine: etap 2, wydzielenie modułu albo zalegająca skrzynka (D-030)
- Kafelki serwerowe mapy: archiwum > 50 tys. punktów albo p95 mapy > 2 s (ADR 0008)
- Klasa rzadkiego dostępu dla zdjęć > 12 mies.: przechowywanie > ~40% kosztów albo przejście na AWS (D-074)
- Inteligentne wyszukiwanie (Elasticsearch/OpenSearch): decyzja o §10 albo skargi na trafność (D-075)
- „Zaznacz tekst i podmień na oznaczenie konta we wszystkich wystąpieniach” (D-079)
- Multi-tenancy: pierwszy klient instytucjonalny (D-073)
