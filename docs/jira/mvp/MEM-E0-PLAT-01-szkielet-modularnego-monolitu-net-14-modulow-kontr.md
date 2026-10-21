# MEM-E0-PLAT-01: Szkielet modularnego monolitu .NET: 14 modułów, kontrakty, testy architektury w CI

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Platforma, -, MEM-E0-PLAT-01 |
| Moduł | - |
| Warstwa | szkielet |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | - |
| Blokuje | [MEM-E0-PLAT-02](MEM-E0-PLAT-02-outbox-inbox-w-wolverine-postgresql-bez-brokera-w-.md); [MEM-E0-PLAT-03](MEM-E0-PLAT-03-postgresql-postgis-schemat-i-rola-bazy-na-modul-im.md); [MEM-E0-PLAT-04](MEM-E0-PLAT-04-magazyn-obiektow-w-ue-bucket-prywatny-podpisy-5-mi.md); [MEM-E0-PLAT-05](MEM-E0-PLAT-05-spike-mapy-maplibre-pmtiles-w-flutterze-globus-10-.md); [MEM-E0-PLAT-06](MEM-E0-PLAT-06-adapter-ai-pseudonimizacja-znacznik-ai-wersja-prom.md); [MEM-E0-PLAT-07](MEM-E0-PLAT-07-modul-konto-asp-net-core-identity-apple-google-dek.md); [MEM-E0-PLAT-08](MEM-E0-PLAT-08-worker-quartz-net-z-magazynem-w-postgresql-zadania.md); [MEM-E0-PLAT-09](MEM-E0-PLAT-09-etap-1-vps-compose-nginx-wal-g-comiesieczny-test-o.md); [MEM-E0-PLAT-10](MEM-E0-PLAT-10-obserwowalnosc-opentelemetry-prometheus-loki-grafa.md); [MEM-E0-PLAT-11](MEM-E0-PLAT-11-offline-kolejka-drift-z-uuid-klienta.md); [MEM-E0-PLAT-12](MEM-E0-PLAT-12-api-v1-openapi-klient-dart-kontrola-zgodnosci-w-ci.md); [MEM-E0-PLAT-13](MEM-E0-PLAT-13-pipeline-wdrozenia-jedna-komenda-lint-migracji-exp.md); [MEM-E0-PLAT-14](MEM-E0-PLAT-14-testy-macierzy-dostepu-uzytkownik-zasob-i-wyszukiw.md); [MEM-E0-PLAT-15](MEM-E0-PLAT-15-ksiega-uprawnien-w-planach-pierwszy-adapter-sklepy.md); [MEM-E0-PLAT-16](MEM-E0-PLAT-16-szablon-warstw-onion-w-module-backendu-i-funkcji-f.md); [MEM-E0-PLAT-17](MEM-E0-PLAT-17-konfiguracja-z-env-z-walidacja-przy-starcie-health.md); [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md); [MEM-E0-ALBUM-01](MEM-E0-ALBUM-01-szkielet-modulu-album-struktura-publiczny-kontrakt.md); [MEM-E0-DOSTEP-01](MEM-E0-DOSTEP-01-szkielet-modulu-dostep-struktura-publiczny-kontrak.md); [MEM-E0-POCHODZENIE-01](MEM-E0-POCHODZENIE-01-szkielet-modulu-pochodzenie-struktura-publiczny-ko.md); [MEM-E0-MIEJSCA-01](MEM-E0-MIEJSCA-01-szkielet-modulu-miejsca-struktura-publiczny-kontra.md); [MEM-E0-LIMITY-01](MEM-E0-LIMITY-01-szkielet-modulu-limity-struktura-publiczny-kontrak.md); [MEM-E0-PLANY-01](MEM-E0-PLANY-01-szkielet-modulu-plany-struktura-publiczny-kontrakt.md); [MEM-E0-NARRACJA-AI-01](MEM-E0-NARRACJA-AI-01-szkielet-modulu-narracja-ai-struktura-publiczny-ko.md); [MEM-E0-MODERACJA-01](MEM-E0-MODERACJA-01-szkielet-modulu-moderacja-struktura-publiczny-kont.md); [MEM-E0-KONTO-01](MEM-E0-KONTO-01-szkielet-modulu-konto-struktura-publiczny-kontrakt.md); [MEM-E0-SUKCESJA-01](MEM-E0-SUKCESJA-01-szkielet-modulu-sukcesja-struktura-publiczny-kontr.md); [MEM-E0-POWIADOMIENIA-01](MEM-E0-POWIADOMIENIA-01-szkielet-modulu-powiadomienia-struktura-publiczny-.md) |
| Kod roboczy | MEM-E0-PLAT-01 |
| Źródło | projekt systemu T1; SD1, SD2 |

## Cel

Szkielet modularnego monolitu .NET: 14 modułów, kontrakty, testy architektury w CI — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Szkielet modularnego monolitu .NET: 14 modułów, kontrakty, testy architektury w CI (źródło: projekt systemu T1; SD1, SD2)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E0-PLAT-02, MEM-E0-PLAT-03, MEM-E0-PLAT-04)

## Kryteria akceptacji

- [ ] Szkielet modularnego monolitu .NET: 14 modułów, kontrakty, testy architektury w CI. *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*
- [ ] Fitness function SD1: moduł nie importuje wnętrza innego modułu (tylko contracts). *Sprawdzenie: CI — test architektury (NetArchTest/ArchUnitNET)*
- [ ] Fitness function SD2: konwencje modułów (domain bez infrastruktury). *Sprawdzenie: CI — test architektury*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
