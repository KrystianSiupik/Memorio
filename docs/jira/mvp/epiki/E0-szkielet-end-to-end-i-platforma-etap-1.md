# E0: Szkielet end-to-end i platforma (etap 1)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony |
| Cel planu | G1, G4 |
| Priorytet | P0 |
| Moduły | -, archiwum, album, dostep, pochodzenie, miejsca, limity, plany, narracja-ai, moderacja, konto, sukcesja, powiadomienia |
| Zależy od epików | - |
| Estymacja | 49–96 h |
| Źródło | projekt systemu, enablery T1–T14; plan infrastruktury „teraz” |

## Kryteria ukończenia epiku

- [ ] Wszystkie tickety P0 epiku zamknięte, a testy przejść jego ekranów zielone. *Sprawdzenie: CI + test na urządzeniu*

## Tickety

| Kod | Tytuł | Moduł / warstwa | h | Status |
|---|---|---|---|---|
| [MEM-E0-PLAT-01](../MEM-E0-PLAT-01-szkielet-modularnego-monolitu-net-14-modulow-kontr.md) | Szkielet modularnego monolitu .NET: 14 modułów, kontrakty, testy architektury w CI | - / szkielet | 2–4 | gotowy |
| [MEM-E0-PLAT-02](../MEM-E0-PLAT-02-outbox-inbox-w-wolverine-postgresql-bez-brokera-w-.md) | Outbox/Inbox w Wolverine (PostgreSQL, bez brokera) w osobnym workerze + metryka wieku kolejki | - / infrastruktura | 2–4 | gotowy |
| [MEM-E0-PLAT-03](../MEM-E0-PLAT-03-postgresql-postgis-schemat-i-rola-bazy-na-modul-im.md) | PostgreSQL + PostGIS, schemat i rola bazy na moduł, import tabeli OSM | - / infrastruktura | 2–4 | gotowy |
| [MEM-E0-PLAT-04](../MEM-E0-PLAT-04-magazyn-obiektow-w-ue-bucket-prywatny-podpisy-5-mi.md) | Magazyn obiektów w UE: bucket prywatny, podpisy 5 min partiami, strefa tymczasowa uploadu | - / infrastruktura | 2–4 | gotowy |
| [MEM-E0-PLAT-05](../MEM-E0-PLAT-05-spike-mapy-maplibre-pmtiles-w-flutterze-globus-10-.md) | Spike mapy: MapLibre + PMTiles w Flutterze (globus, 10 tys. punktów, web) | - / infrastruktura | 2–4 | gotowy |
| [MEM-E0-PLAT-06](../MEM-E0-PLAT-06-adapter-ai-pseudonimizacja-znacznik-ai-wersja-prom.md) | Adapter AI: pseudonimizacja, znacznik AI, wersja promptu, test ładunku | - / infrastruktura | 2–4 | gotowy |
| [MEM-E0-PLAT-07](../MEM-E0-PLAT-07-modul-konto-asp-net-core-identity-apple-google-dek.md) | Moduł Konto: ASP.NET Core Identity, Apple/Google, deklaracja 16+, strona www usuwania konta | - / infrastruktura | 2–4 | gotowy |
| [MEM-E0-PLAT-08](../MEM-E0-PLAT-08-worker-quartz-net-z-magazynem-w-postgresql-zadania.md) | Worker: Quartz.NET z magazynem w PostgreSQL; zadania idempotentne, gotowe na tryb klastra | - / infrastruktura | 2–4 | gotowy |
| [MEM-E0-PLAT-09](../MEM-E0-PLAT-09-etap-1-vps-compose-nginx-wal-g-comiesieczny-test-o.md) | Etap 1: VPS, Compose, nginx, WAL-G, comiesięczny test odtworzenia | - / infrastruktura | 2–4 | gotowy |
| [MEM-E0-PLAT-10](../MEM-E0-PLAT-10-obserwowalnosc-opentelemetry-prometheus-loki-grafa.md) | Obserwowalność: OpenTelemetry, Prometheus/Loki/Grafana, monitor zewnętrzny, alert | - / infrastruktura | 2–4 | gotowy |
| [MEM-E0-PLAT-11](../MEM-E0-PLAT-11-offline-kolejka-drift-z-uuid-klienta.md) | Offline: kolejka Drift z UUID klienta | - / infrastruktura | 2–4 | gotowy |
| [MEM-E0-PLAT-12](../MEM-E0-PLAT-12-api-v1-openapi-klient-dart-kontrola-zgodnosci-w-ci.md) | API v1: OpenAPI → klient Dart, kontrola zgodności w CI, minimalna wersja aplikacji | - / infrastruktura | 2–4 | gotowy |
| [MEM-E0-PLAT-13](../MEM-E0-PLAT-13-pipeline-wdrozenia-jedna-komenda-lint-migracji-exp.md) | Pipeline wdrożenia jedną komendą + lint migracji expand/contract | - / infrastruktura | 2–4 | gotowy |
| [MEM-E0-PLAT-14](../MEM-E0-PLAT-14-testy-macierzy-dostepu-uzytkownik-zasob-i-wyszukiw.md) | Testy macierzy dostępu (użytkownik × zasób) i wyszukiwania po odebraniu dostępu | - / infrastruktura | 2–4 | gotowy |
| [MEM-E0-PLAT-15](../MEM-E0-PLAT-15-ksiega-uprawnien-w-planach-pierwszy-adapter-sklepy.md) | Księga uprawnień w Planach: pierwszy adapter (sklepy albo serwis subskrypcji), cykl planu czyta tylko księgę | - / infrastruktura | 2–4 | gotowy |
| [MEM-E0-PLAT-16](../MEM-E0-PLAT-16-szablon-warstw-onion-w-module-backendu-i-funkcji-f.md) | Szablon warstw Onion w module backendu i funkcji Fluttera + testy architektury kierunku zależności | - / infrastruktura | 2–4 | gotowy |
| [MEM-E0-PLAT-17](../MEM-E0-PLAT-17-konfiguracja-z-env-z-walidacja-przy-starcie-health.md) | Konfiguracja z ENV z walidacją przy starcie, health i readiness endpoint, kontener (RD1, RD2, RD4) | - / infrastruktura | 2–3 | gotowy |
| [MEM-E0-PLAT-18](../MEM-E0-PLAT-18-sekrety-etapu-1-env-poza-repo-sops-age-do-odtworze.md) | Sekrety etapu 1: .env poza repo + sops/age do odtworzenia (RD10) | - / infrastruktura | 1–2 | gotowy |
| [MEM-E0-ARCHIWUM-01](../MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md) | Szkielet modułu archiwum: struktura, publiczny kontrakt, testy architektury | archiwum / szkielet modułu | 1–2 | gotowy |
| [MEM-E0-ALBUM-01](../MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md) | Szkielet modułu album: struktura, publiczny kontrakt, testy architektury | album / szkielet modułu | 1–2 | gotowy |
| [MEM-E0-DOSTEP-01](../MEM-E0-DOSTEP-01-szkielet-modulu-dostep-struktura-publiczny-kontrak.md) | Szkielet modułu dostep: struktura, publiczny kontrakt, testy architektury | dostep / szkielet modułu | 1–2 | gotowy |
| [MEM-E0-POCHODZENIE-01](../MEM-E0-POCHODZENIE-01-szkielet-modulu-pochodzenie-struktura-publiczny-ko.md) | Szkielet modułu pochodzenie: struktura, publiczny kontrakt, testy architektury | pochodzenie / szkielet modułu | 1–2 | gotowy |
| [MEM-E0-MIEJSCA-01](../MEM-E0-MIEJSCA-01-szkielet-modulu-miejsca-struktura-publiczny-kontra.md) | Szkielet modułu miejsca: struktura, publiczny kontrakt, testy architektury | miejsca / szkielet modułu | 1–2 | gotowy |
| [MEM-E0-LIMITY-01](../MEM-E0-LIMITY-01-szkielet-modulu-limity-struktura-publiczny-kontrak.md) | Szkielet modułu limity: struktura, publiczny kontrakt, testy architektury | limity / szkielet modułu | 1–2 | gotowy |
| [MEM-E0-PLANY-01](../MEM-E0-PLANY-01-szkielet-modulu-plany-struktura-publiczny-kontrakt.md) | Szkielet modułu plany: struktura, publiczny kontrakt, testy architektury | plany / szkielet modułu | 1–2 | gotowy |
| [MEM-E0-NARRACJA-AI-01](../MEM-E0-NARRACJA-AI-01-szkielet-modulu-narracja-ai-struktura-publiczny-ko.md) | Szkielet modułu narracja-ai: struktura, publiczny kontrakt, testy architektury | narracja-ai / szkielet modułu | 1–2 | gotowy |
| [MEM-E0-MODERACJA-01](../MEM-E0-MODERACJA-01-szkielet-modulu-moderacja-struktura-publiczny-kont.md) | Szkielet modułu moderacja: struktura, publiczny kontrakt, testy architektury | moderacja / szkielet modułu | 1–2 | gotowy |
| [MEM-E0-KONTO-01](../MEM-E0-KONTO-01-szkielet-modulu-konto-struktura-publiczny-kontrakt.md) | Szkielet modułu konto: struktura, publiczny kontrakt, testy architektury | konto / szkielet modułu | 1–2 | gotowy |
| [MEM-E0-SUKCESJA-01](../MEM-E0-SUKCESJA-01-szkielet-modulu-sukcesja-struktura-publiczny-kontr.md) | Szkielet modułu sukcesja: struktura, publiczny kontrakt, testy architektury | sukcesja / szkielet modułu | 1–2 | gotowy |
| [MEM-E0-POWIADOMIENIA-01](../MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md) | Szkielet modułu powiadomienia: struktura, publiczny kontrakt, testy architektury | powiadomienia / szkielet modułu | 1–2 | gotowy |
| [MEM-E0-PLAT-19](../MEM-E0-PLAT-19-nawigacja-aplikacji-zakladki-mapa-albumy-udostepni.md) | Nawigacja aplikacji: zakładki Mapa · Albumy · Udostępnione mi · Konto, typy tras, przekierowanie niezalogowanych (S02) | - / UI | 2–3 | gotowy |
