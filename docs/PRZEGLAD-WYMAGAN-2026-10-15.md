# GEO — przegląd wymagań

> **Źródło:** `docs/Description.txt` (dokument koncepcyjny produktu, 18 sekcji)
> **Data przeglądu:** 2026-10-15 · **rundy decyzyjne:** 2026-10-15 (19 rozstrzygnięć), 2026-10-16 (11 rozstrzygnięć: A6, trasa, Część 7; 5 po skanie prawnym)
> **Metoda:** cztery kontrole: problem vs rozwiązanie · obserwowalne zachowanie vs status w bazie · mapa ukrytych decyzji · sonda sztywnych kwantyfikatorów
> **Zakres:** 18 sekcji potraktowanych jako 9 klastrów wymagań
> **Uwaga:** `Description.txt` nie został zmieniony. To osobny dokument roboczy.

## Jak czytać ten dokument

| Oznaczenie | Znaczenie |
|---|---|
| **PRZYJĘTE** | decyzja podjęta 2026-10-15, wpisana do wymagania |
| ⚠ | **zadanie albo ustalenie z podmiotem zewnętrznym** — nie decyzja produktowa; lista w Części 4 |
| ⭐ | wniosek, który pojawił się w trakcie przeglądu i nie wynikał z pierwszego czytania |
| §N | sekcja w `Description.txt` |

Każda ramka z wymaganiem jest napisana tak, żeby dała się przetestować: ma niezmiennik, regułę przy równoczesnym działaniu dwóch osób, zachowanie przy powtórzeniu tej samej komendy i ścieżkę cofnięcia.

---

# Część 0 — Wynik w skrócie

| | |
|---|---|
| Przejrzane klastry wymagań | 9 (18 sekcji) |
| Klastry z uwagami | 8 |
| Blokery przebudowane | 4 |
| Klastry mapy sygnałów zamknięte | 5, dwa częściowo |
| Kwantyfikatory przesondowane | 6 |
| Sprzeczności wewnątrz dokumentu | 3 |
| Kolizje z obowiązkami prawnymi | 2 |
| Decyzje otwarte po przeglądzie | **wszystkie rozstrzygnięte**: 35 decyzji produktowych (19 z 2026-10-15, 16 z 2026-10-16, w tym 5 po skanie prawnym); zostało 7 zadań zewnętrznych (Część 4) |
| Klastry dokończone 2026-10-16 | A6, trasa z C6, plan rodzinny, import, osoby i wyszukiwanie, powiadomienia (C7–C10) |

**Najważniejszy wniosek całego przeglądu.** Pięć niezależnych miejsc w dokumencie wymaga tej samej, nieoczywistej maszynerii: **trwałego rejestru pochodzenia każdej treści** — co z czego powstało i kto to wniósł. Osobno żadne z nich nie uzasadnia osobnego podsystemu. Wszystkie razem — tak. Jeśli ten rejestr nie wejdzie do architektury **przed pierwszym wspólnym albumem**, dorobienie go później oznacza migrację całego archiwum zdjęć. Wymagają go: B1 (zgoda odwoływalna po latach), B2 (cofnięcie dostępu musi dotrzeć do kopii), B3 (autorstwo per treść), A2/A3 (mierzalność zakazu zmyślania i ocena uprawnień przy odczycie), C5 (ślad artefaktu AI).

---

# Część 1 — Krytyka: co było nie tak

## Kontrola 2 — CRUD przebrany za logikę domenową (4 blokery)

Wzorzec wspólny dla wszystkich czterech: dokument bardzo dobrze opisuje, **co użytkownik widzi**, i konsekwentnie pomija, **co się dzieje z danymi po czasie** — po wycofaniu dostępu, po usunięciu, po odejściu uczestnika, po wygaśnięciu planu, po śmierci właściciela.

**B1 — §6, Premium dla albumu.** Cytat: *„wykupuje odpowiedni plan i zaprasza pięciu członków rodziny. Każdy z nich może korzystać ze wspólnych funkcji albumu"*. Jedyny opisany skutek: osoba „otrzymuje uprawnienia". Test „usuń flagę i nic nie rób" nie zmienia niczego w opisie. Tymczasem §13 zdradza trzy współdzielone liczniki („dodatkowa przestrzeń", „funkcje AI", „większa liczba uczestników"), o które rywalizuje sześć osób — i o żadnym z nich dokument nie mówi nic poza nazwą. Z tego opisu powstaje tabela członków plus flaga planu: zadziała, przejdzie testy i rozsypie się na pierwszym rachunku za AI.

**B2 — §5 i §11, cofnięcie dostępu.** Cytaty: *„Powinien mieć możliwość cofnięcia dostępu"* oraz *„móc zmienić ustawienia i cofnąć udostępnienie"*. Jedyny skutek: zgaszenie flagi. A produkt równolegle rozsiewa kopie tej treści, co opisuje w innych sekcjach: §7 (inni zapisują interesujące miejsca), §10 („Zaplanuj podobną podróż" z publicznego albumu), §9 (podsumowanie zapisane jako pamiątka i udostępnione rodzinie), indeks wyszukiwania AI. Do tego pułapka techniczna: przy publicznych adresach plików „cofnięcie dostępu" jest fikcją.

**B3 — §15 pkt 9, usuwanie.** Cytat: *„Podstawowe ustawienia prywatności i możliwość usunięcia wspomnień"* — to **całość** tego, co dokument mówi o usuwaniu. Brak odpowiedzi na: wspomnienie współtworzone, to samo zdjęcie w dwóch albumach, wspomnienie zacytowane w zapisanym podsumowaniu, wspomnienie w kopii publicznej. **O usunięciu konta nie ma ani słowa.** O losach albumu po właścicielu też nie — a §5 stawia album *„Nasza rodzina — 1990–2026"*, a §16 obiecuje *„długoterminowe archiwa rodzinne"*. Trzydzieści sześć lat archiwum bez reguły sukcesji.

**B4 — §12, profil miejsca.** Cytat: *„Firma może zgłosić **istniejący** profil i potwierdzić, że jest jego właścicielem"*. Jedno słowo zdradza problem: **„istniejący"**. Profil już jest, więc ktoś go stworzył — zewnętrzna baza (§3) albo sam użytkownik (§4). Zgłoszenie jest zawsze roszczeniem do **cudzego rekordu**, więc nie wystarczy „potwierdzić": potrzebny jest ktoś, kto rozstrzyga spór, i reguła, co dzieje się z tym, co pod rekordem już wisi. Dokument nie nazywa też konfliktu: użytkownik publikuje niepochlebne wspomnienie z lokalu, a lokal jest właścicielem profilu.

## Kontrola 4 — sonda sztywnych kwantyfikatorów (6 trafień)

**A1 — §2 i §17.1: „Każde wspomnienie jest powiązane z miejscem".** Scenariusze graniczne z tej samej domeny: wspomnienie z lotu lub pociągu · „gdzieś we Włoszech, lato 2003" · **zdjęcie z automatycznego importu z telefonu bez geotagu (§16) — to większość zdjęć** · wspomnienie, którego miejsca użytkownik świadomie nie chce zapisywać (szpital, adres domowy) · wspomnienie o osobie, nie o miejscu. Jeśli choć jeden jest możliwy, miejsce nie może być obowiązkowe w modelu. **Ten kwantyfikator wprost kłóci się z §16 i z MVP.** **Rozstrzygnięte w C6:** miejsce zostaje obowiązkowe, ale zyskuje poziomy precyzji i stan „nieokreślone" — cztery z pięciu przypadków granicznych przestają być brakiem miejsca, a piąty zostaje świadomie wykluczony.

**A2 — §9: „System nie może wymyślać wydarzeń, relacji, dat ani emocji".** Reguła słuszna i **niewykonalna jako zapis** — z modelem językowym nie da się jej zagwarantować ani przetestować w tej formie. **Oba przykłady w dokumencie już ją łamią:** §8 *„Tego dnia spotkałeś się ze znajomymi, **aby wspólnie świętować** początek roku"* (intencja, której nie ma w zapisie) oraz §9 *„**Rok rozpoczęliście** od spotkania sylwestrowego"* pod tytułem *„**Wasza rodzina** w 2026"* (wnioskowana relacja, gdy wpis źródłowy mówi „spotkałem się ze znajomymi"). Wymaganie musi zejść z poziomu zakazu na poziom mechanizmu: narracja wyłącznie z jawnej listy faktów, każde zdanie wskazywalne do źródła, treść interpretowana wizualnie oddzielona, zatwierdzenie przed zapisem.

**A3 — §10 i §11: „wyłącznie treści, do których użytkownik ma uprawniony dostęp".** Brakuje **momentu** oceny uprawnień. Zapisane podsumowanie to artefakt, który zamroził cudze treści. Gdy uczestnik wycofa wspomnienie albo odejdzie — czy podsumowania cytujące je trzeba unieważnić? **Rozstrzygnięte w B2 i C5: tak, kaskadowo.** A indeks wyszukiwania: jeden na cały system z filtrem po fakcie, czy osobny per zakres uprawnień? To przesądza architekturę. **Rozstrzygnięte w C9 (2026-10-16):** odebrany dostęp znika z wyników od razu, więc uprawnienia są sprawdzane przy każdym zapytaniu, a nie zapisane w indeksie.

**A4 — §12: „nie powinny umożliwiać identyfikowania konkretnych użytkowników" przy „odpowiednio dużej liczbie danych".** Brak liczby. Scenariusz: lokal z trzema publicznymi wspomnieniami, właściciel widzi „często kojarzone z urodzinami", a wspomnienie urodzinowe jest dokładnie jedno — publiczne, ze zdjęciami i imionami. **Rozstrzygnięte: próg k = 20 (B4).**

**A5 — §11: „Oryginał pozostaje niezmieniony".** Migawka jednokierunkowa znaczy, że późniejsze usunięcie zdjęcia z oryginału (np. bo osoba ze zdjęcia poprosiła) **nie dociera do kopii publicznej**. Dziura w prawie do usunięcia, wynikająca wprost z wybranego rozwiązania. **Rozstrzygnięte w B1 i B2: kopia publiczna jest artefaktem pochodnym i schodzi w kaskadzie.**

**A6 — §5: „Album prywatny. Dostęp ma wyłącznie właściciel".** Granice: obsługa zgłoszenia nadużycia, żądanie organu, spadkobiercy (§16 — patrz procedura zgonu w B3), drugie urządzenie właściciela. **Rozstrzygnięte w C8 (2026-10-16):** kontakt spadkowy wskazany z góry; obsługa widzi treść wyłącznie na żądanie organu, z wpisem do dziennika.

## Kontrola 1 — problem vs rozwiązanie (najrzadziej, i to dobry znak)

Jedyne realne trafienie: **§11 „Publiczna kopia albumu"** opisuje wybrane rozwiązanie (migawka), a nie regułę biznesową — i dlatego nie widzi własnej konsekwencji (A5). Reguła brzmi: oryginał nietknięty + użytkownik widzi wersję publiczną przed publikacją. Mechanizm kopiowania powinien z niej wynikać, a nie ją zastępować.

Świadomie **nie** zgłoszone jako problem: kod QR w §12 (ustalony kanał dla miejsca fizycznego), globus w §3 (to tożsamość produktu, nie dowolny wybór), PDF-y i filmy jako formaty eksportu w §9.

## Kontrola 3 — mapa sygnałów

Zapaliła się we wszystkich klastrach. Najmocniej **🔒 dane osobowe**: produkt trzyma precyzyjne lokalizacje, daty, relacje, twarze i — wprost wymienione w §11 — informacje medyczne, a nie ma ani retencji, ani usuwania, ani zgody osób trzecich. Rozstrzygnięcia: Części 2 i 3.

## Co było napisane dobrze

Rzeczy, które przeszły bez uwag — warto je zachować przy przepisywaniu dokumentu:

- **§8** — *„Tryb powinien obsługiwać brak dokładnej daty, kilka wspomnień z jednego dnia, wydarzenia trwające kilka dni oraz wiele wspomnień w tej samej lokalizacji"*. Przypadki graniczne wymienione, zanim ktokolwiek zapytał.
- **§6** — rozdzielenie trzech zakresów Premium (album / konto / prywatne wspomnienia uczestnika) i zdanie *„Subskrypcja albumu nie powinna automatycznie dawać jego właścicielowi dostępu do pozostałych danych"*.
- **§11** — *„Właściciel albumu nie powinien automatycznie uzyskiwać prawa do publicznego opublikowania każdej treści dodanej przez zaproszonych uczestników"*.
- **§17.6** — *„Produkt powinien mieć wartość również dla jednej osoby"*. Konkretne ograniczenie projektowe, nie slogan.
- **§13** — *„Wysokość limitów, ceny i dostępność należy ustalić po zbadaniu kosztów"*. Uczciwe „nie wiemy jeszcze" zamiast wymyślonej liczby.
- **§15** — odłożenie publicznych albumów na etap po weryfikacji MVP. Jak się okazało w C2, **to właśnie ta decyzja chroni trzyosobowy zespół przed obowiązkiem moderacji.**

---

# Część 2 — Cztery blokery przebudowane

## B1 — Premium dla albumu

**PRZYJĘTE:** miejsce zajmowane w chwili wysłania zaproszenia · wspólna pula albumu opłacana przez właściciela · przy wyczerpaniu puli sygnał do właściciela · wygaśnięcie planu = tryb tylko do odczytu · po 24 miesiącach przycięcie do darmowego limitu · zgoda na pozostawienie treści odwoływalna w każdej chwili · Premium osobiste i Premium albumu nie kumulują się.

```
┌─ KOMENDA A: Właściciel zaprasza osobę do wspólnego albumu ──────────────┐
│ (wyłącznie właściciel — §6; współtwórca nie zaprasza)                   │
Efekt:          Wolne miejsca w planie −1 w chwili WYSŁANIA zaproszenia,
                przed jego przyjęciem. Właściciel widzi: zajęte /
                zaproszone-niepotwierdzone / wolne. Zaproszony nie widzi
                żadnej treści albumu do chwili przyjęcia.
Współbieżność:  Zaproszenie ponad liczbę wolnych miejsc odrzucone przy wysyłce.
                NIEZMIENNIK: uczestnicy + zaproszenia niepotwierdzone ≤ limit planu.
                Nie istnieje stan, w którym sześć osób przyjmuje pięć miejsc.
Idempotentność: Powtórne zaproszenie tej samej osoby odświeża istniejące i NIE
                zajmuje drugiego miejsca. Zaproszenie uczestnika: brak efektu.
Cofnięcie:      Wycofanie, odrzucenie i wygaśnięcie — każde zwalnia miejsce
                natychmiast.

┌─ KOMENDA B: Uczestnik dodaje media / uruchamia AI na albumie ───────────┐
Efekt:          Zużywa się WSPÓLNA pula albumu (przestrzeń i limit AI),
                opłacona przez właściciela, niezależnie od tego, kto działa.
                Osobiste limity uczestnika pozostają nietknięte — treść
                w albumie nie liczy się do jego własnej przestrzeni.
                Uczestnik przed operacją widzi, ile zostało w puli.
Przy wyczerpaniu: Operacja odrzucona z komunikatem „pula albumu pełna",
                a WŁAŚCICIEL dostaje powiadomienie z prośbą o powiększenie planu.
                Uczestnik nie dopłaca i nie przenosi na własną przestrzeń —
                decyzja o pieniądzach zostaje u płacącego.
Współbieżność:  Dwie równoległe operacje nie mogą razem przekroczyć puli;
                nic nie jest przyjmowane „na minus".
Cofnięcie:      Usunięcie treści zwraca przestrzeń. Zużyty limit AI nie wraca —
                operacja została już opłacona u dostawcy.

┌─ ZDARZENIE: plan wygasa lub zostaje obniżony ───────────────────────────┐
Efekt:          Album → „tylko do odczytu". Wszyscy uczestnicy zachowują dostęp,
                widzą całą zawartość, nikt nie traci miejsca, nic nie jest
                usuwane. Blokowane TYLKO operacje powiększające zużycie:
                nowe media, nowe zaproszenia, funkcje AI.
                ŻADNE WSPOMNIENIE NIE GINIE Z POWODU RACHUNKU.
Retencja:       24 miesiące pełnego odczytu. Ostrzeżenia do właściciela
                i uczestników: 90 / 30 / 7 / 1 dzień przed upływem. Eksport
                całego archiwum bezpłatnie przez cały ten okres.
Po 24 miesiącach: Album ZOSTAJE, przycięty do darmowego limitu — nie jest usuwany.
                PRZYJĘTE: właściciel wskazuje, co zostaje, w okienku 30 dni.
                Bez jego decyzji system zachowuje materiał NAJSTARSZY
                chronologicznie do wysokości limitu — w archiwum rodzinnym
                najstarsze jest nieodtwarzalne. Każdy uczestnik powiadamiany
                osobno i może pobrać swoje wkłady niezależnie od decyzji
                właściciela.
                To jedyna nieodwracalna operacja w tym wymaganiu: wymaga serii
                ostrzeżeń i dowodu ich doręczenia.
Cofnięcie:      Ponowne opłacenie planu w dowolnym momencie przywraca pełną
                funkcjonalność bez żadnego działania uczestników.

┌─ KOMENDA C: Uczestnik odchodzi / właściciel go usuwa ───────────────────┐
Efekt:          Traci dostęp, miejsce w planie zwalnia się, treści domyślnie
                zostają we wspólnym archiwum.
Wybór:          Jawny wybór „zostaw" / „zabierz" przy wyjściu, z pokazaniem,
                czego dotyczy (ile zdjęć, wpisów). Właściciel usuwający uczestnika
                NIE decyduje za niego: treści zostają, a odchodzący dostaje
                powiadomienie z tym samym wyborem i terminem na decyzję.
Odwoływalność:  Wybór „zostaw" jest ZGODĄ, nie przekazaniem — odwoływalną
                w dowolnym momencie, także po latach. Odwołanie wycofuje treść
                z albumu ORAZ z wszystkich artefaktów pochodnych.
Premium:        Premium osobiste i Premium albumu nie kumulują się. Praca
                w albumie zużywa wyłącznie pulę albumu.
Spójne z §11:   Zostawienie treści w albumie NIE jest zgodą na jej publikację.
```

### Co wymusza decyzja „zgoda odwoływalna"

Wariant najuczciwszy wobec użytkownika i najdroższy w budowie. Przestają być opcjonalne:

1. **Trwały ślad pochodzenia każdego zdjęcia i wpisu**, przeżywający odejście autora. Nie wystarczy pole „dodane przez" z kluczem do konta, które może zniknąć.
2. **Deduplikacja plików między albumami staje się ryzykowna** — gdy dwie osoby wrzuciły ten sam plik, usunięcie współdzielonego bytu zabrałoby cudzą treść. Pliki rozliczane per wnoszący.
3. **§9 musi zapisywać listę materiałów źródłowych** każdego podsumowania — inaczej nie da się unieważnić narracji cytującej wycofaną treść. **To ten sam rejestr, którego wymaga A2**, by „system nie może wymyślać" przestało być niemierzalnym zakazem.
4. **§11 trzeba przepisać:** kopia publiczna nie jest niezależną migawką, tylko artefaktem pochodnym z żywym odnośnikiem do źródeł — bo musi dać się wydrążyć.
5. **B2 jest częściowo rozstrzygnięty** — ta sama maszyneria obsługuje oba.

## B2 — Cofnięcie dostępu

**PRZYJĘTE:** dostęp do zdjęć równa się dostępowi do albumu i ustaje razem z nim · kaskadowe wygaszanie artefaktów pochodnych · link wymaga zalogowania · okno ważności podpisu mediów 5 minut · artefakty pochodne nie wracają po ponownym wpuszczeniu.

⭐ **Najważniejsze odkrycie: §5 opisuje dwie różne komendy jako jedną.** Przy wymogu logowania **link przestaje być uprawnieniem i staje się kanałem zaproszenia**, a uprawnienie jest imienne. To rozdziela komendy — i stąd brało się złudzenie, że wystarczy zgasić flagę.

⭐ **Sprzeczność w dokumencie:** §5 obiecuje link *„dla osób posiadających"*, a §11 obiecuje wiedzę o tym, *„kto może zobaczyć"*. Przy anonimowym linku właściciel z definicji nie może tego wiedzieć. Wymóg logowania usuwa sprzeczność.

**Reguła wyprowadzona z wymagania, nie z technologii.** Zdanie *„dobrze jak osoba ma dostęp do zdjęć gdy ma dostęp do albumu, ale jak w nim nie będzie to nie"* jest regułą biznesową i samo odrzuca stałe adresy publiczne (kto raz zapisał adres, widzi plik po odebraniu dostępu) oraz ogranicza ciasteczko sesyjne do długości sesji. Zostaje wydawanie mediów sprawdzane przy każdym żądaniu, z krótkim okienkiem ważności.

```
┌─ KOMENDA A: Właściciel udostępnia album linkiem ────────────────────────┐
Efekt:          Link otwiera album wyłącznie osobie zalogowanej. Pierwsze wejście
                dopisuje ją do IMIENNEJ listy oglądających, widocznej dla
                właściciela — to realizuje §11, czego anonimowy link nie mógł.
                Osoba widzi album w „Udostępnione mi"; media są jej wydawane
                tylko tak długo, jak jest na liście.
Idempotentność: Powtórne wejście nie tworzy drugiego wpisu. Wygenerowanie nowego
                linku NIE odbiera dostępu nikomu, kto już wszedł.

┌─ KOMENDA B: Właściciel unieważnia link ─────────────────────────────────┐
Efekt:          Nikt NOWY nie wejdzie. Osoby już wpuszczone zachowują dostęp.
                Obserwowalnie: lista oglądających bez zmian, link martwy.
                To NIE jest „cofnięcie dostępu" i interfejs musi to nazwać
                inaczej, bo dzisiejsze §5 myli te dwie rzeczy.

┌─ KOMENDA C: Właściciel odbiera dostęp osobie (lub wszystkim) ───────────┐
Efekt:          Osoba znika z listy i traci album z „Udostępnione mi".
                Wydawanie jej mediów ustaje — PRZYJĘTE: nie dłużej niż 5 MINUT
                (okno ważności podpisu; podpisy odświeżane w tle).
Kaskada:        Wygaszane są wszystkie artefakty powstałe z tego albumu:
                · zapisane przez innych miejsca (§7) tracą treść źródłową,
                · plany podróży i podsumowania AI (§9, §10) unieważnione,
                · kopia publiczna albumu (§11) schodzi.
Współbieżność:  Osoba przeglądająca w chwili komendy traci media przy kolejnym
                żądaniu, bez czekania na koniec sesji.
Idempotentność: Powtórne odebranie dostępu nie robi nic. Odebranie dostępu
                komuś, kto go nie ma, nie robi nic.
Cofnięcie:      Ponowne wpuszczenie przywraca album i media, ale PRZYJĘTE:
                NIE wskrzesza wygaszonych artefaktów pochodnych — unieważnione
                zostaje unieważnione, podsumowanie generuje się od nowa.
                Nic zbędnego nie jest przechowywane „na wypadek powrotu".
```

### Czego „cofnięcie dostępu" nie gwarantuje — i co musi o tym mówić interfejs

Bez tej części produkt obiecuje nieprawdę:

- zdjęcia **już pobrane** na dysk, zrzuty ekranu i kopie w cache przeglądarki pozostają u osoby, której odebrano dostęp,
- treści przepisane lub zapamiętane nie wracają,
- **§13 ma dziurę: „eksport wspomnień" jako funkcja Premium.** Jeśli osoba z dostępem do cudzego albumu może go wyeksportować, kaskada jest fikcją. **Eksport musi być ograniczony do treści własnych i archiwum, którego się jest właścicielem lub współtwórcą** — nie do wszystkiego, co się aktualnie widzi.

Ekran udostępniania musi powiedzieć to **przed** udostępnieniem, nie w regulaminie.

**Dlaczego 5 minut, a nie dowolna inna liczba.** To bezpośredni handel kosztem: krócej znaczy prawdziwsze cofnięcie i wyraźnie więcej zapytań o podpisy (galeria z §4 to 17 zdjęć naraz, czyli 17 podpisów), dłużej znaczy taniej i wolniejsze odcięcie. Liczba należy do wymagania, nie do pliku konfiguracyjnego, którego nikt nie przeczyta.

## B3 — Usunięcie wspomnienia, konta i losy archiwum

**PRZYJĘTE:** wspomnienie jest pojemnikiem, każdy usuwa swoje · rola kuratora zasobów · usunięcie konta odwołuje wszystkie zgody · następca albumu wskazany z góry · kosz 30 dni · przejęcie po zgonie na podstawie aktu zgonu weryfikowanego przez obsługę · przekazanie następcy nie odwołuje zgód, a fundator potwierdza losy własnych treści przy przekazaniu.

⭐ **Uzasadnienie decyzji o usunięciu konta, warte zapisania w wymaganiu:** *„użytkownik później po usunięciu konta nie będzie mieć możliwości cofnięcia decyzji"*. Zgoda odwoływalna wymaga **żywego konta**, którym się ją odwołuje — więc nic nie może zostać w stanie „zgoda do odwołania" po stronie konta, które przestało istnieć. Decyzja z B1 pociągnęła decyzję w B3. **Bez tego zdania w dokumencie ktoś za rok „uprości" to na zostawianie treści.**

```
┌─ MODEL: wspomnienie jest POJEMNIKIEM, nie bytem jednego autora ──────────┐
Niezmiennik:    Wspomnienie istnieje tak długo, jak ma co najmniej jedną treść.
                Gdy traci ostatnią — przestaje istnieć, a jego punkt znika z mapy (§3).
Tożsamość:      Pojemnik trzyma miejsce i datę; one zostają, dopóki cokolwiek zostaje.
                Tytuł i opis należą do tego, kto je napisał, i odchodzą z nim —
                wspomnienie może mieć zdjęcia bez opisu i taki stan trzeba narysować.
Do zmiany w §2: „autora i uczestników" zakłada jedno autorstwo całości.
                W tym modelu autorstwo jest per treść, nie per wspomnienie.

┌─ KOMENDA A: Wnoszący usuwa swoją treść ─────────────────────────────────┐
Efekt:          Znikają wyłącznie jego zdjęcia, filmy i jego opis. Cudze zostają.
                Nikt nie traci danych cudzą decyzją — ani właściciel albumu,
                ani kurator nie mogą usunąć treści wnoszącego.
Natychmiastowość: Dla pozostałych uczestników treść przestaje być widoczna od razu.
                PRZYJĘTE: dla wnoszącego pozostaje odwracalna 30 DNI (kosz
                widoczny tylko dla niego).
                Formalne żądanie usunięcia danych POMIJA kosz.
Kaskada:        Ta sama maszyneria co w B2 — gasną zapisane przez innych miejsca
                oparte na tej treści oraz artefakty AI, które ją cytowały.

┌─ ROLA: Kurator zasobów (nowa) ──────────────────────────────────────────┐
Może:           Porządkować, opisywać, grupować i wyjmować z albumu treści
                powierzone mu przez wnoszących.
Nie może:       Usunąć treści wnoszącego z systemu ani opublikować jej (§11).
Odwoływalność:  Wnoszący odwołuje delegację w każdej chwili, bez zgody kuratora,
                i zachowuje prawo usunięcia swoich treści wbrew kuratorowi.
                Delegacja NIE jest przeniesieniem własności — inaczej B1 i B3
                mówiłyby sprzeczne rzeczy.

┌─ KOMENDA B: Użytkownik usuwa konto ─────────────────────────────────────┐
Efekt:          Usunięcie konta JEST odwołaniem wszystkich zgód. Wszystkie treści
                wniesione do cudzych albumów gasną, z pełną kaskadą z B2.
Przed komendą:  Ekran usuwania konta pokazuje LICZBAMI, co wypadnie i skąd:
                ile zdjęć, z ilu albumów, czyich. Plus eksport własnego archiwum
                przed usunięciem (bezpłatny — patrz §13).
Właściciel
albumu:         Jeśli usuwający konto jest właścicielem albumu, komenda jest
                wstrzymana do wskazania następcy albo świadomej zgody na wygaśnięcie
                albumu. Konto nie może zniknąć, zostawiając album bez właściciela.
Nieodwracalność: Po upływie okna 30 dni nie ma powrotu.

┌─ ZDARZENIE: Przekazanie albumu następcy ────────────────────────────────┐
Wskazanie:      Właściciel wyznacza następcę z grona uczestników przy zakładaniu
                albumu Premium; może zmienić w każdej chwili. Przypomnienie, gdy
                następca przestaje być uczestnikiem.
Efekt:          Album, jego pula i rola właściciela przechodzą na następcę.
                Następca potwierdza przejęcie; przy odmowie album wchodzi w stan
                „tylko do odczytu" na 24 miesiące — ten sam co w B1.
PRZYJĘTE —      Przekazanie następcy NIE jest usunięciem konta i NIE odwołuje
rozróżnienie,   zgód fundatora. Ekran przekazania PYTA GO WPROST o losy jego
które ratuje    własnych treści — decyzja jest podejmowana wtedy, gdy jeszcze
archiwum:       może ją podjąć. Bez tego zachodzi przypadek najgorszy:
                właściciel buduje archiwum rodzinne, umiera, spadkobiercy
                zamykają jego konto — i następca dziedziczy album wydrążony
                z treści samego fundatora.
PRZYJĘTE —      Uczestnik zgłasza zgon, obsługa weryfikuje akt zgonu, następca
po zgonie:      przejmuje album. Wyklucza to przejęcie archiwum osoby, która
                po prostu długo nie zaglądała. Wymaga opisanej procedury
                i człowieka po stronie zespołu (zadanie Z1).
```

**Konsekwencja dla §16, do dopisania w dokumencie (zadanie Z2):** przy regule „usunięcie konta odwołuje zgody" archiwum jest trwałe tylko tak długo, jak **każdy** uczestnik utrzymuje konto. Pięcioosobowy album rodzinny traci piątą część historii, gdy jedna osoba zamknie konto. Nie wolno sprzedawać „archiwum na pokolenia" bez tego przypisu.

## B4 — Zgłoszenie i weryfikacja profilu miejsca

**PRZYJĘTE:** weryfikacja kodem na adres lokalu, spory rozstrzyga obsługa · kod ważny 30 dni, 3 próby na kwartał · scalenie z miejscem użytkownika, wspomnienia przechodzą · prywatna nazwa zostaje przy wspomnieniu autora · właściciel profilu może tylko zgłosić naruszenie · próg statystyk k = 20.

```
┌─ KOMENDA A: Firma zgłasza roszczenie do profilu miejsca ────────────────┐
Efekt:          Profil wchodzi w stan „roszczenie w toku". PUBLICZNIE NIC SIĘ
                NIE ZMIENIA — samo zgłoszenie nie daje żadnego uprawnienia:
                ani edycji, ani statystyk, ani widoczności „oficjalny".
                Kod wysyłany na adres lokalu albo na numer z publicznego źródła
                dla tego adresu — nie na dane podane przez zgłaszającego.
Niezmiennik:    Miejsce ma w danej chwili co najwyżej JEDNEGO zweryfikowanego
                właściciela.
Współbieżność:  Roszczenie w toku nie blokuje innych zgłoszeń — profil zdobywa
                ten, kto pierwszy potwierdzi kod. Kolejne roszczenie do miejsca
                już zweryfikowanego otwiera SPÓR: profil zostaje przy dotychczasowym
                właścicielu, a obsługa rozstrzyga na podstawie dokumentów.
                Nikt nie traci profilu automatycznie przez samo zgłoszenie.
Idempotentność: Powtórne zgłoszenie tej samej firmy odświeża kod, nie tworzy
                drugiego roszczenia. Zgłoszenie profilu, który firma już ma: brak efektu.
PRZYJĘTE:       Kod ważny 30 DNI (list zdąży dojść także za granicę),
                limit 3 PRÓBY NA KWARTAŁ — żeby zgłaszanie nie było narzędziem
                nagabywania właścicieli lokali ani serią szans na przechwycenie
                listu przez byłego najemcę.
Cofnięcie:      Firma może zrzec się profilu — wraca on do stanu niezweryfikowanego.
                Zrzeczenie NIE cofa scalenia i nie rusza wspomnień.

┌─ KOMENDA B: Scalenie przy pierwszej weryfikacji ───────────────────────┐
Efekt:          Miejsce utworzone wcześniej przez użytkownika (§4) wtapia się
                w oficjalny profil. Wspomnienia działają dalej i wiszą pod nim.
                Dwa punkty na mapie stają się jednym.
Obserwowalnie
dla autora:     Każdy, kto ma wspomnienie pod scalanym miejscem, dostaje
                powiadomienie: „miejsce, pod którym masz 3 wspomnienia, jest
                teraz oficjalnym profilem X". Bez tego jest to cicha podmiana
                kontekstu pod cudzym wspomnieniem.
PRZYJĘTE:       Prywatna nazwa nadana przez użytkownika ZOSTAJE przy jego
                wspomnieniu i jest widoczna tylko dla niego; publicznie widnieje
                nazwa oficjalna. Inaczej scalenie zamieniłoby „dom babci"
                na „Restauracja Kowalski" w czyimś pamiętniku, a to jest
                dokładnie ta treść, której produkt ma nie niszczyć (§17.1).
Odwracalność:   Scalenie musi być odwracalne przez obsługę, bo błędne scalenie
                wiesza cudze wspomnienia pod obcym lokalem. Wymaga zachowania
                tożsamości sprzed scalenia — nie wystarczy przepisać klucza.

┌─ KOMENDA C: Właściciel profilu zgłasza naruszenie ─────────────────────┐
Niezmiennik:    Właściciel profilu NIE MA wobec cudzych wspomnień żadnego
                uprawnienia, którego nie ma zwykły użytkownik. Nie ukrywa,
                nie odpina, nie edytuje.
Efekt:          Zgłoszenie trafia do kolejki. Treść POZOSTAJE WIDOCZNA do
                rozstrzygnięcia — zgłoszenie samo w sobie nic nie ukrywa.
                Autor dowiaduje się o zgłoszeniu, wynik dostają obie strony
                z uzasadnieniem, autor ma ścieżkę odwołania.
Obowiązek,      Procedura rozpatrywania zgłoszeń z terminem i odwołaniem jest
nie funkcja:    dla platformy hostującej publiczne treści w UE obowiązkiem.
                W dokumencie nie ma o niej ani słowa, a §7 i §12 ją wymuszają.
                Rozwinięcie i terminy: C2.
```

### Statystyki — próg i jego skutek dla sprzedaży

§12 obiecuje właścicielowi *„analizować zagregowane statystyki"*, a zaraz obok stawia warunek bez liczby: *„powinny opierać się na odpowiednio dużej liczbie danych"*.

**PRZYJĘTE: k = 20** publicznych wspomnień dla każdego pokazywanego agregatu, z wygaszaniem każdego przekroju schodzącego poniżej progu.

**Skutek biznesowy z tej samej liczby, do uwzględnienia w §13:** większość małych lokali długo nie przekroczy progu, więc płatna funkcja „statystyki" będzie dla nich pusta. Plan biznesowy musi sprzedawać co innego (profil, wydarzenia, promocja), a statystyki traktować jako dodatek dla miejsc popularnych. Alternatywa — obniżenie progu — oznacza agregaty deanonimizujące klientów i została odrzucona.

**Tożsamość profilu przy zmianie najemcy lokalu** — rozstrzygnięta w C4 („cykl życia"): profil wiązany z działalnością, nie z adresem.

---

# Część 3 — Jedenaście bloków (C1–C6 z 2026-10-15, C7–C10 z 2026-10-16, C11 ze skanu prawnego)

## C1 — Data wspomnienia

**PRZYJĘTE:** data cywilna w miejscu zdarzenia · daty w przyszłości niedozwolone.

```
MODEL:          Data wspomnienia to DATA CYWILNA w miejscu zdarzenia: rok,
                miesiąc, dzień. Bez strefy, bez przeliczania. Każdy oglądający
                na świecie widzi tę samą datę, co jest warunkiem, żeby „14 maja
                2026" w pamiętniku znaczyło to samo zawsze i wszędzie.
PRECYZJA
(wymaga jej     Precyzja jest jawnym polem, nie domysłem: ROK · MIESIĄC+ROK ·
już §8)         DZIEŃ · PRZEDZIAŁ. „Lato 2003" to miesiąc albo przedział, nie
                zgadnięty 1 lipca. Album „1990–2026" (§5) to przedział.
                Interfejs nigdy nie dopisuje precyzji, której użytkownik nie podał.
DWA RÓŻNE POLA: data zdarzenia (cywilna, ustawiana przez użytkownika, swobodnie
                wstecz do 1990 i dalej) ≠ data dodania (chwila, ustawiana przez
                system, w UTC). Nigdy mieszane — §9 liczy statystyki okresu
                po pierwszej, nie po drugiej.
PORZĄDEK
W OBRĘBIE DNIA: Kilka wspomnień z jednego dnia (§8 wprost tego wymaga) porządkuje
                KOLEJNOŚĆ USTALONA PRZEZ UŻYTKOWNIKA, nie godzina — bo godziny
                w tym modelu nie ma. Świadoma konsekwencja wyboru daty cywilnej.
FILTR OKRESU:   Wspomnienie wchodzi do okresu, jeśli jego data lub przedział
                PRZECINA ten okres. Wydarzenie przez kilka dni (§8) pojawia się
                w każdym okresie, który dotyka.
PRZYJĘTE:       Daty PÓŹNIEJSZE NIŻ DZIŚ są dla wspomnień zablokowane —
                pamiętnik zapisuje to, co było. Dzięki temu podsumowania
                i statystyki nie liczą wydarzeń, które się nie odbyły.
                Planowanie podróży to osobna rzecz (§10) i nie jest wspomnieniem.
```

⭐ **To naprawia sprzeczność w §8.** *„Sylwester 2025/2026"* jako Etap 1 odtwarzania **roku 2026** jest poprawny, ale **tylko wtedy, gdy wspomnienie jest zapisane jako PRZEDZIAŁ 31.12.2025–01.01.2026**. Jako pojedyncza data 31 grudnia wypada z roku 2026 i przykład przestaje działać. Model przedziałów ratuje własny przykład dokumentu; trzeba go tylko dopisać.

## C2 — Zgłoszenia i moderacja

**PRZYJĘTE:** zachowanie treści przy zgłoszeniu zależy od kategorii · nielegalne rozpatrywane w 24 h, sporne w 7 dni.

```
┌─ KOMENDA: Zgłoszenie publicznej treści ─────────────────────────────────┐
Kto może:       KAŻDY, kto treść widzi — także osoba BEZ KONTA.
⭐ To zamyka     §11 wymienia „twarze osób, które mogą nie chcieć publikacji",
dziurę z §11:   ale osoba, która znajdzie swoją twarz w cudzym opublikowanym
                albumie, nie jest użytkownikiem GEO i nie ma gdzie kliknąć.
                Każda publiczna strona albumu i wspomnienia musi mieć ścieżkę
                zgłoszenia działającą bez rejestracji — inaczej „kontrola
                prywatności" chroni tylko publikującego, nie sfotografowanego.

KATEGORIA A — nielegalne (materiały z dziećmi, groźby, nawoływanie):
Efekt:          Treść natychmiast niewidoczna publicznie, ale NIE usunięta —
                zabezpieczona jako dowód. Autor informowany o ukryciu i powodzie.
                Ścieżka przekazania dalej poza tym systemem.
PRZYJĘTE:       rozpatrzenie ≤ 24 h.

KATEGORIA B — sporne (cudza twarz, cudze dane osobowe, zniesławienie lokalu,
               prawa autorskie):
Efekt:          Treść POZOSTAJE WIDOCZNA do rozstrzygnięcia. Autor dowiaduje się
                o zgłoszeniu i jego kategorii. Zgłoszenie nie jest przyciskiem
                „ukryj cudze wspomnienie".
PRZYJĘTE:       rozpatrzenie ≤ 7 dni, wynik z uzasadnieniem do obu stron,
                odwołanie dla autora.

Niezmiennik
(z B4):         Właściciel profilu miejsca nie ma tu żadnego uprawnienia, którego
                nie ma osoba z ulicy. Zgłoszenie to jego jedyna droga.
Idempotentność: Powtórne zgłoszenie tej samej treści przez tę samą osobę nie
                mnoży kolejki. Nadużywanie zgłoszeń odbiera możliwość zgłaszania.
```

**Co te terminy znaczą operacyjnie:** 24 h na kategorię A wymaga, żeby ktoś zaglądał do kolejki codziennie, także w weekend. Przy trzyosobowym zespole to realne zobowiązanie, które trzeba rozpisać na dyżury — i dlatego granica zakresu MVP poniżej jest tak ważna.

⭐ **§15 już Was przed tym chroni.** MVP wprost odkłada *„publiczne albumy"* na etap po weryfikacji. Dopóki punkt 6 (*„Udostępnianie albumu oraz kontrola uprawnień"*) znaczy **tylko udostępnianie imienne i linkiem**, a nie publikację, MVP nie uruchamia obowiązku moderacji ani dyżurów. **Warto to w §15 napisać wprost jako granicę zakresu**, bo dziś wynika to tylko z kolejności zdań — i pierwsza osoba, która „na szybko" doda publiczny album, wprowadzi trzyosobowy zespół w obowiązek całodobowy.

> ⚠ **POPRAWIONE 2026-10-16 po skanie prawnym (L10, decyzja 33).** Zdanie „MVP nie uruchamia obowiązku moderacji" było nieścisłe. Odkładanie publikacji chroni przed obowiązkami **platformy internetowej** (z sekcji 3 DSA mikroprzedsiębiorstwa i tak są zwolnione) i przed dyżurami 24 h — ale już samo przechowywanie i udostępnianie albumów to **hosting**, a obowiązki hostingu (DSA art. 11–17) nie mają zwolnienia dla mikrofirm. MVP musi więc mieć: regulamin z zasadami moderacji, przycisk zgłoszenia w albumie, uzasadnienie decyzji dla autora i punkt kontaktowy (dla organów i użytkowników). Do tego nakaz usunięcia treści terrorystycznych trzeba wykonać w 1 godzinę (L11). Polska ustawa wdrażająca DSA (koordynator: Prezes UKE) wchodzi w życie 6.11.2026.

## C3 — Sprzedaż i podatek

**PRZYJĘTE:** pośrednik jest sprzedawcą wobec klienta · fotoksiążki zostają jako późniejszy etap z własnym sprzedawcą i partnerem druku.

```
STRUKTURA:      Pośrednik (Paddle / Lemon Squeezy / sklepy mobilne) jest
                SPRZEDAWCĄ wobec klienta. VAT w 27 krajach, faktury, zwroty
                i próg OSS są po jego stronie. Wasz system dostaje wyłącznie
                zdarzenia o stanie subskrypcji.
NIEZMIENNIK:    Uprawnienia w produkcie wynikają WYŁĄCZNIE ze zdarzeń od
                pośrednika — nigdy z wartości wpisanej ręcznie w bazie.
                Inaczej plany rozjeżdżają się z płatnościami.
Idempotentność: Zdarzenia przychodzą po kilka razy i nie w kolejności. Każde ma
                identyfikator i znacznik wersji; przetworzenie drugi raz nie
                zmienia stanu. Starsze zdarzenie nie nadpisuje nowszego stanu.
Awaria:         Produkt musi umieć DOPYTAĆ pośrednika o aktualny stan, a nie
                tylko czekać na powiadomienie — inaczej ktoś płaci i nie dostaje
                dostępu, a to jest zgłoszenie, które kosztuje najwięcej czasu.
Brak płatności: Nie wymaga nowego mechanizmu — wpada w stan ustalony w B1:
                album tylko do odczytu, 24 miesiące, potem przycięcie.
                Nic nie jest usuwane za nieopłaconą fakturę.
PREMIUM DLA
ALBUMU (§6):    Płaci jedna osoba, korzysta sześć — także z innych krajów.
                Podatek idzie za NABYWCĄ, nie za korzystającymi: to jedna
                sprzedaż konsumencka właścicielowi albumu. Kraje uczestników
                nie mają tu znaczenia i §6 może to spokojnie powiedzieć.
FOTOKSIĄŻKI
(§13) PRZYJĘTE: Zostają w dokumencie jako KIERUNEK na później, nie jako strumień
                przychodu na start. Przy uruchomieniu wymagają osobnego
                sprzedawcy i partnera druku, bo to towar fizyczny,
                personalizowany: inna stawka, wysyłka, zwroty, a prawo
                odstąpienia działa inaczej niż dla subskrypcji. Pośrednicy dla
                dóbr cyfrowych tego nie prowadzą. Przy fotoksiążkach w §13
                trzeba to zapisać, żeby nikt nie policzył ich w przychodach MVP.
```

⚠ **Zadanie Z4:** przed wyborem pośrednika sprawdzić, czy obsługuje sprzedaż **B2B** (numer VAT nabywcy, odwrotne obciążenie w UE). Bez tego plany biznesowe z §12 nie mają jak zostać sprzedane.

## C4 — Miejsce jako Wasz wpis

**PRZYJĘTE:** własna tabela miejsc; dane zewnętrzne tylko podpowiadają i są kopiowane w chwili wyboru · źródło podpowiedzi: OpenStreetMap teraz, komercyjne źródło dołożone później tam, gdzie zabraknie danych.

> ⚠ **ZMIENIONE 2026-10-16 po skanie prawnym (L17, decyzja 34).** Kopiowanie danych OSM do jednej tabeli razem z miejscami użytkowników i scalanie duplikatów tworzy najpewniej **bazę pochodną w rozumieniu licencji ODbL**: przy publicznym użyciu całą tabelę, łącznie z „domem babci", trzeba by udostępnić na tej licencji. Nowy model: **dwie tabele** — dane OSM w osobnej, niezmienionej tabeli (baza kolektywna, wymaga tylko uznania autorstwa) i miejsca własne w osobnej tabeli; łączone dopiero przy zapytaniu. Scalanie z B4 dotyczy wyłącznie miejsc własnych i profili. Wspomnienie wskazuje miejsce własne albo wpis OSM po identyfikatorze, bez kopiowania jego danych. W UI uznanie autorstwa OSM. **Do potwierdzenia przez prawnika** (pytanie L17). Ramka poniżej opisuje model sprzed zmiany — przy przepisywaniu dokumentu zastąpić.

```
MODEL:          Miejsce to wpis w Waszej tabeli, z Waszym numerem. Dane zewnętrzne
                są wyłącznie podpowiedzią przy tworzeniu i są kopiowane w chwili
                wyboru: nazwa, współrzędne, adres, plus data skopiowania i źródło.
NIEZMIENNIK:    Wspomnienie nigdy nie wskazuje bezpośrednio na numer z cudzej bazy.
                Numer dostawcy może leżeć obok jako ślad pochodzenia, ale jego
                usunięcie nie rusza ani wspomnienia, ani punktu na mapie.
MIEJSCE WŁASNE: „Dom babci" (§4) to pełnoprawny wpis bez źródła zewnętrznego,
                funkcjonalnie nieodróżnialny od restauracji.
DUPLIKATY:      Dwa wpisy o tym samym lokalu to NORMALNY STAN, nie błąd.
                Użytkownik może zgłosić duplikat; scalanie idzie tą samą,
                odwracalną procedurą co w B4.
                PRZYJĘTY KOSZT: jakość mapy staje się pracą ciągłą, nie
                jednorazową integracją.
CYKL ŻYCIA:     Lokal zamknięty albo przemianowany → wpis ZOSTAJE i dostaje stan
                „zamknięte" z datą. Wspomnienie pokazuje nazwę z czasu zdarzenia,
                profil pokazuje nazwę aktualną.
⭐ Domyka B4:    Profil jest wiązany z działalnością, nie z adresem. Nowy lokal
                pod tym samym adresem to nowy wpis — rocznice w „Restauracji A"
                nie stają się historią „Restauracji B".
ŹRÓDŁO
PRZYJĘTE:       OpenStreetMap na start (dane wolno przechowywać i rysować na
                własnym globusie, bez opłat, z podaniem źródła), komercyjne
                źródło dokładane później tam, gdzie dane o lokalach są zbyt
                ubogie.
KONSEKWENCJE
TEGO WYBORU
(dwa źródła od
początku):      1. Model miejsca musi OD RAZU dopuszczać różne źródła pochodzenia
                   jednego wpisu — dorobienie drugiego źródła później oznacza
                   migrację całej tabeli miejsc.
                2. Ten sam lokal przyjdzie z dwóch stron, więc DUPLIKATÓW BĘDZIE
                   WIĘCEJ niż przy jednym źródle. Scalanie z B4 przestaje być
                   funkcją poboczną i staje się częścią utrzymania.
                3. Każde dołożone źródło wymaga sprawdzenia, czy jego licencja
                   pozwala przechowywać to, co z niego przepisujecie — to warunek
                   wpisany w wymaganie, nie kwestia cennika.
WYKLUCZONE:     Google Places jako źródło podpowiedzi przy tym modelu — regulamin
                zabrania przechowywania danych o miejscach i wymaga pokazywania
                ich na mapie Google, co zabija własny globus z §3.
PODKŁAD MAPY:   To OSOBNA umowa niż baza miejsc. Można mieć własne miejsca
                i kupiony podkład. Nie można mieć cudzych danych o miejscach
                na własnym globusie, jeśli regulamin tego zabrania. §3 i §4
                dotyczą dwóch różnych rzeczy i dokument je dziś zlewa w jedno.
```

## C5 — Dostawca AI

**PRZYJĘTE:** zgoda osobno dla każdego albumu · analiza obrazu u siebie, do dostawcy modelu wyłącznie tekst · do dostawcy wychodzi rodzaj miejsca i region, bez dokładnych nazw i bez imion.

**Dlaczego analiza obrazu zostaje u siebie:** §11 każe szukać w zdjęciach twarzy, dokumentów i informacji medycznych. Gdyby zdjęcia wychodziły na zewnątrz, to właśnie te najwrażliwsze materiały oddawalibyście podmiotowi trzeciemu — czyli funkcja reklamowana jako ochrona prywatności byłaby największym wyciekiem w produkcie. Wykrywanie twarzy i dokumentów robi mały model na własnym serwerze; narracja z §9 potrzebuje wyłącznie tekstu.

```
GRANICA PRZYJĘTA:
   Wychodzą:    opisy, tytuły, daty, tagi, liczby, RODZAJ MIEJSCA i REGION
                („szczyt górski, Dolny Śląsk"), uczestnicy jako „osoba 1", „osoba 2".
   NIE wychodzą: pliki zdjęć i filmów, metadane EXIF, dokładne współrzędne,
                dokładne nazwy miejsc, imiona uczestników.
   Powrót:      Wasz system wstawia prawdziwe nazwy i imiona do gotowego tekstu
                po jego otrzymaniu. Narracja brzmi naturalnie („wiosną
                wybraliście się w góry"), a przekazany materiał nie wskazuje
                konkretnej rodziny. Działa, bo narracja operuje na strukturze
                wydarzeń, nie na wiedzy o świecie.

ZGODA — osobno dla każdego albumu:
   Album prywatny:      zgoda właściciela.
   Album współdzielony: zgoda właściciela NIE wystarcza dla treści uczestników.
                Przetwarzane są wyłącznie treści tych uczestników, którzy
                zgodzili się dla TEGO albumu.
⭐ Konsekwencja  Podsumowanie powstaje wtedy z CZĘŚCI materiału i musi to pokazać
dla §9:         („pominięto wspomnienia 2 uczestników"). Inaczej §9 kłamie
                liczbami — „zapisaliście 37 miejsc" przy policzonych 23.
   Odwołanie zgody: wchodzi w kaskadę z B2 — unieważnia artefakty powstałe
                z treści tego uczestnika.
   Nowy uczestnik dołączający po wygenerowaniu podsumowania nie zmienia go
                wstecz. Podsumowanie ma datę i skład, z którego powstało.

⭐ KONSEKWENCJA DLA §10:
   „Zaplanuj podobną podróż" czyta cudzy PUBLICZNY album. Publikacja nie jest
   zgodą na bycie materiałem wejściowym dla AI — to inny cel. Przy zgodzie
   per album potrzebny jest osobny przełącznik: „pozwól innym planować podróże
   na podstawie tego albumu". §10 dziś milcząco zakłada, że publiczne znaczy
   dozwolone do przetwarzania.

UMOWA I MIEJSCE PRZETWARZANIA (dokument nie mówi o tym nic):
   Przetwarzanie w UE, umowa powierzenia, zakaz trenowania na treściach.
   ⚠ Zadanie Z5: wynegocjować i mieć na piśmie zerową retencję danych
   wejściowych oraz brak logowania treści — duzi dostawcy to dają, ale
   wyłącznie na wniosek.
   Lista podwykonawców widoczna dla użytkownika.

AWARIA I KOSZT:
   Dostawca niedostępny → funkcja pokazuje stan niedostępności, NIGDY częściowy
   wynik. Sufit kosztów wynika z puli z B1: przekroczenie zachowuje się jak
   wyczerpanie puli, z powiadomieniem do właściciela albumu.

ŚLAD:          Każdy artefakt AI zapisuje: listę materiałów źródłowych, datę,
               model i jego wersję oraz zgody, na których powstał.
               PIĄTE miejsce wymagające tego samego rejestru pochodzenia.
```

## C6 — Miejsce wspomnienia: obowiązkowość, precyzja, ukrycie

**PRZYJĘTE:** miejsce obowiązkowe, ale może mieć stan „nieokreślone" · cztery poziomy precyzji · import bez geotagu trafia do poczekalni · dokładne miejsce ukrywane przy udostępnianiu.

⭐ **Pytanie „czy wspomnienie może istnieć bez miejsca" ukrywało pięć różnych przypadków**, a nie jeden — i cztery z nich dały się rozwiązać bez porzucania tezy z §17.1:

| Przypadek | Czego naprawdę brakowało | Rozwiązanie |
|---|---|---|
| Import zdjęcia bez geotagu (§16) | miejsce istnieje, system go nie zna | stan „nieokreślone" + poczekalnia |
| „Gdzieś we Włoszech, lato 2003" | miejsce znane zgrubnie | precyzja „kraj" |
| Lot, pociąg, rejs | miejsce jest ruchome | trasa, którą §2 już przewiduje |
| Szpital, adres domowy | miejsce znane, nieujawniane | ukrycie przy udostępnianiu |
| „Dzień, w którym urodził się syn" | miejsce nie jest treścią wspomnienia | **świadomie niedopuszczone** |

⭐ **Symetria z datą.** Dla daty ustaliliśmy w C1 jawne poziomy precyzji i nie uznaliśmy tego za rezygnację z dat. Miejsce dostaje dokładnie tę samą konstrukcję, więc dwa pola produktu zachowują się tak samo — łatwiej to zrozumieć i łatwiej zbudować.

```
MODEL:          Każde wspomnienie MA miejsce. Miejsce może mieć precyzję:
                PUNKT · MIEJSCOWOŚĆ · REGION · KRAJ · NIEOKREŚLONE.
                „Nieokreślone" to stan przejściowy do uzupełnienia, nie brak.
NIEDOPUSZCZONE: Wspomnienie bez miejsca jako trwały stan. Teza z §17.1 zostaje:
                produkt nie zamienia się w zwykły pamiętnik z osią czasu.

MAPA (§3):      Znacznik pojawia się na POZIOMIE ZGODNYM Z PRECYZJĄ.
                Wspomnienie z precyzją „kraj" nie dostaje szpilki w przypadkowym
                polu pod Rzymem — dostaje znacznik kraju, rozwijany przy
                przybliżeniu. Mapa nigdy nie pokazuje dokładności, której nie ma.
NIEOKREŚLONE:   Nie pojawia się na mapie. Żyje w „Wspomnienia" (§14)
                i w poczekalni „do przypisania".
⭐ JEDEN EKRAN   Poczekalnia obsługuje dwie rzeczy naraz: zdjęcia z importu bez
NA DWIE RZECZY: geotagu i wspomnienia z miejscem nieokreślonym. To ten sam
                problem użytkownika („trzeba to poukładać") i ten sam ekran.

PRZEŚLEDŹ (§8): Wspomnienie bez ustalonego miejsca nie może animować mapy.
                Pokazywane jest jako panel bez ruchu mapy, w swojej kolejności
                chronologicznej. §8 już obsługuje brak dokładnej daty, więc
                obsługa braku dokładnego miejsca jest z nim spójna.

STATYSTYKI
(§3, §9):       Precyzja decyduje, do czego wspomnienie się liczy:
                · „kraj" liczy się do odwiedzonych PAŃSTW, ale nie do REGIONÓW,
                · „nieokreślone" nie liczy się do niczego.
                Dzięki temu „zapisaliście 37 miejsc" z §9 jest prawdą, a nie
                sumą obejmującą materiał bez miejsca.

IMPORT (§16):   Zdjęcie bez geotagu trafia do poczekalni „do przypisania".
                System NICZEGO NIE ZGADUJE — ani z godziny, ani z geotagów
                zdjęć obok. Użytkownik przypisuje grupami.
                Powód jest ten sam co w A2: domysł przedstawiony jako fakt
                jest dokładnie tym, czego zasada 4 z §17 zakazuje.

UKRYCIE:        Dokładne miejsce widzi TYLKO AUTOR. Przy udostępnianiu
                i publikacji autor wybiera poziom ujawnienia (np. tylko
                miejscowość) albo nieujawnianie miejsca wcale.
                Mapa autora pozostaje prawdziwa — nie musi okłamywać własnego
                archiwum, żeby chronić prywatność na zewnątrz.
⚠ Konsekwencja   Publikacja i udostępnienie MUSZĄ usuwać z plików metadane
obowiązkowa:    lokalizacji. §11 wymienia je dziś jako coś, czego „Sprawdź
                prywatność" szuka — przy ukrywaniu miejsca przestaje to być
                sugestia i staje się warunkiem, bo inaczej ukrycie jest pozorne.
AI:             Dokładne miejsce nie wychodzi do dostawcy modelu nigdy —
                C5 wysyła wyłącznie rodzaj miejsca i region. Dwie decyzje
                składają się tu w jedno i nie wymagają dodatkowej reguły.

RUCH (lot,      §2 już przewiduje „opcjonalne informacje o trasie", więc lot
pociąg):        jest wspomnieniem Z TRASĄ, nie bez miejsca.
PRZYJĘTE        Punktem głównym (szpilką) jest miejsce początkowe albo
(2026-10-16):   docelowe — wybiera użytkownik. Do odwiedzonych państw
                i regionów liczą się WYŁĄCZNIE miejsca, w których trasa się
                zatrzymała (porty rejsu, przesiadki z wyjściem). Przelot nad
                krajem nie jest jego odwiedzeniem — mapa zapisuje miejsca,
                w których się było.
```

## C7 — Plan rodzinny (§16)

**PRZYJĘTE (2026-10-16):** „konto rodzinne" z §16 to plan rodzinny Premium osobistego, a nie nowy byt kont · wypadnięcie z planu działa jak wygaśnięcie planu w B1.

⭐ **§16 i §6 mówiły o dwóch różnych rzeczach tym samym słowem „rodzina".** Premium dla albumu kupuje pulę wspólnej przestrzeni. Plan rodzinny kupuje Premium dla N osobnych kont. Gdyby oba nazywały się „rodzinne", użytkownik nie wiedziałby, za co płaci, a kod scaliłby je w jedną tabelę uprawnień.

```
┌─ KOMENDA A: Płacący dodaje osobę do planu rodzinnego ───────────┐
Efekt:          Wolne miejsca −1 w chwili WYSŁANIA (jak B1).
                Po przyjęciu osoba ma Premium osobiste na WŁASNYM koncie.
                Płacący widzi tylko listę członków planu — żadnych
                treści, liczby wspomnień ani zużycia miejsca.
Niezmiennik:    członkowie + zaproszenia ≤ limit planu.
                Osoba jest w co najwyżej JEDNYM planie rodzinnym naraz.
Idempotentność: Ponowne zaproszenie odświeża istniejące.
Zakres:         Plan rodzinny = Premium OSOBISTE. Nie daje puli żadnego
                albumu — praca w albumie zużywa pulę albumu (B1).
                Nie kumuluje się z własnym Premium osobistym.

┌─ ZDARZENIE: osoba wypada z planu ────────────────────────────┐
(płacący ją usunął / przestał płacić / sama odeszła)
Efekt:          Konto w trybie „nic nowego ponad darmowy limit".
                Wszystko widoczne i do pobrania; NIC nie jest usuwane
                przez 24 miesiące; dalej jak w B1 (ostrzeżenia,
                okienko 30 dni, zostaje najstarsze).
Powiadomienie:  Osoba dowiaduje się od razu i może kupić własne
                Premium bez przerwy w dostępie.
Cofnięcie:      Powrót do planu lub własne Premium przywraca pełny
                dostęp bez działania z jej strony.
```

**PRZYJĘTE (2026-10-16): dołączenie z własną subskrypcją — jak w Spotify.**

```
┌─ KOMENDA B: Osoba z własnym Premium przyjmuje plan rodzinny ────────────┐
Efekt:          Plan rodzinny działa OD RAZU. Niewykorzystany, opłacony czas
                własnego Premium PRZEPADA — bez zwrotu i bez przeliczenia.
Przed komendą:  Ekran przyjęcia mówi to WPROST, z liczbą dni, które
                przepadną, i z radą, że można przyjąć zaproszenie tuż przed
                własną datą płatności. Zaproszenie czeka (miejsce zajęte, B1).
Sklep mobilny:  Aplikacja nie może anulować subskrypcji ze sklepu.
                Ekran prowadzi do anulowania w sklepie; do tego czasu
                osoba PŁACI PODWÓJNIE i ekran ma to powiedzieć.
Płacący:        Przejście płacącego z własnego Premium na plan rodzinny to
                inna sprawa: oba plany w JEDNEJ grupie subskrypcji, sklep
                (lub pośrednik, Z4) liczy proporcję i nic nie przepada.
```

Wzorzec: Netflix (zmiana na droższy plan od razu, na tańszy od następnej płatności), Spotify (przejście na Family przelicza czas, dołączenie do cudzego Family go odbiera), App Store i Google Play (w jednej grupie subskrypcji przejście na wyższy plan od razu, z proporcjonalnym zwrotem).

## C8 — Prywatny album: kto poza właścicielem (A6)

**PRZYJĘTE (2026-10-16):** kontakt spadkowy wskazany z góry · obsługa widzi treść wyłącznie na żądanie organu, z wpisem do dziennika.

```
ZASADA:         „Dostęp ma wyłącznie właściciel" (§5) zostaje, z DWOMA
                nazwanymi wyjątkami. Każdy inny przypadek to błąd.
                Drugie urządzenie właściciela nie jest wyjątkiem — to ten
                sam właściciel.

┌─ WYJĄTEK 1: Kontakt spadkowy ───────────────────────────────────────────┐
Wskazanie:      Właściciel MOŻE z góry wskazać osobę dla każdego albumu
                prywatnego (albo dla wszystkich naraz). Może zmienić lub
                odwołać w każdej chwili.
Efekt po zgonie: Procedura aktu zgonu z B3 (Z1). Kontakt dostaje album
                TYLKO DO ODCZYTU, z możliwością eksportu. Nie dostaje
                reszty konta ani innych albumów.
Bez wskazania:  Nikt nie dostaje albumu — także spadkobiercy z dokumentem.
                Nikt nie widzi prywatnych treści bez wcześniejszej decyzji
                właściciela.
Spójne z B3:    Album współdzielony ma NASTĘPCĘ (przejmuje rolę właściciela),
                album prywatny ma KONTAKT SPADKOWY (tylko odczyt). Dwie różne
                role, bo prywatny album nie ma wspólnoty, którą ktoś prowadzi.

┌─ WYJĄTEK 2: Żądanie organu ─────────────────────────────────────────────┐
Efekt:          Obsługa może zobaczyć treść wyłącznie na formalne żądanie
                organu z podstawą prawną. Każdy wgląd zapisany w dzienniku,
                którego nie da się usunąć: kto, kiedy, co, na jakiej podstawie.
Niezmiennik:    W zwykłym zgłoszeniu do pomocy obsługa NIE widzi treści —
                nawet za zgodą właściciela. Narzędzia obsługi pokazują
                metadane (liczby, rozmiary, stany), nigdy zdjęcia i opisy.
Konsekwencja:   Wyklucza szyfrowanie end-to-end — treść musi być dostępna
                po stronie serwera dla AI (C5) i dla żądań organów.
                ⚠ Procedura przyjmowania żądań organów: zadanie Z6.
```

## C9 — Osoby we wspomnieniu i wyszukiwanie (§2, §10)

**PRZYJĘTE (2026-10-16):** osoba we wspomnieniu pochodzi z prywatnej listy osób autora · odebrany dostęp znika z wyników wyszukiwania od razu.

```
MODEL OSOBY:    Każdy użytkownik ma WŁASNĄ listę osób („Ania z pracy",
                „Ania siostra"). Wspomnienie wskazuje osobę z listy,
                nie samo imię. Dwie Anie to dwa wpisy.
Powiązanie
z kontem:       Osobę z listy MOŻNA powiązać z kontem GEO, i to dopiero
                za zgodą tej osoby. Bez powiązania osoba istnieje tylko
                na liście autora (babcia bez konta też jest osobą).
⭐ Niezmiennik   Oznaczenie osoby we wspomnieniu NIE daje jej dostępu do
(wniosek z §17.2): tego wspomnienia. Dostęp wynika wyłącznie z albumu
                i udostępnienia (B2). Inaczej oznaczenie byłoby
                ukrytym kanałem udostępniania.
Do zmiany w §2: „autora i uczestników" → autorstwo per treść (B3)
                + osoby z prywatnej listy autora.
AI (C5):        Do dostawcy modelu osoby wychodzą jako „osoba 1",
                „osoba 2" — lista osób tego nie zmienia.

WYSZUKIWANIE:   „Które miejsca odwiedziliśmy z Anią?" — gdy na liście
                są dwie Anie, system pyta, o którą chodzi; nie zgaduje.
UPRAWNIENIA:    Sprawdzane PRZY KAŻDYM ZAPYTANIU, nie zapisane w indeksie.
                Odebrany dostęp (B2) znika z wyników OD RAZU — szybciej niż
                media (5 minut), bo wynik wyszukiwania pokazuje opis
                i nazwę miejsca bez pobierania pliku.
Przesądza A3:   Jeden indeks z filtrem uprawnień przy zapytaniu, nie
                indeks per zakres uprawnień.
```

## C10 — Import zdjęć i powiadomienia (§16, §5)

**PRZYJĘTE (2026-10-16):** ponowny import pomija ten sam plik tej samej osoby · zaproszenia e-mailem: jedno przypomnienie i blokada adresu · powiadomienia o danych nie do wyłączenia.

```
┌─ KOMENDA: Import zdjęć z telefonu (powtórny) ───────────────────────────┐
Idempotentność: Identyczny plik TEJ SAMEJ osoby nie jest importowany drugi
                raz (ponowna synchronizacja, zmiana telefonu).
                Plik po edycji (przycięty, z filtrem) jest NOWYM plikiem.
Inna osoba:     Ten sam plik u innej osoby to osobna treść — pliki
                rozliczane per wnoszący (B1), więc deduplikacja nigdy
                nie łączy plików dwóch osób.
Miejsce:        Bez geotagu → poczekalnia „do przypisania" (C6).

┌─ ZAPROSZENIE e-mailem dla osoby bez konta ──────────────────────────────┐
Przypomnienia:  Jedno przypomnienie po 7 dniach, potem cisza.
Blokada:        W każdym mailu link „nie chcę zaproszeń z GEO". Po
                kliknięciu nikt nie zaprosi już tego adresu. Właściciel
                albumu widzi tylko „zaproszenie niedostarczalne" — nie
                dowiaduje się, że adresat odmówił.
Miejsce (B1):   Zaproszenie niedostarczalne zwalnia miejsce w planie.

┌─ POWIADOMIENIA: dwie kategorie ─────────────────────────────────────────┐
„Dotyczące      Ostrzeżenia przed przycięciem archiwum (B1), wybór
Twoich danych": „zostaw/zabierz" (B1-C), zgłoszenie Twojej treści (C2),
                przekazanie albumu (B3), wypadnięcie z planu (C7).
                NIE DA SIĘ ICH WYŁĄCZYĆ. Zawsze e-mail + w aplikacji.
Reszta:         Do wyłączenia przez użytkownika.
Niedoręczenie:  Gdy e-mail jest nieprawidłowy lub odrzucony, NIEODWRACALNA
                operacja (przycięcie archiwum) jest WSTRZYMANA, nie
                wykonywana bez ostrzeżenia. Tak realizuje się „dowód
                doręczenia" z B1.
```

## C11 — Wymogi prawne ze skanu (2026-10-16)

> ⚠ **To nie jest porada prawna.** Źródło: skan prawny z 2026-10-16 (prawo UE i polskie, stan sprawdzony w źródłach urzędowych tego dnia, ważny do 15.11.2026). Każdy punkt potwierdza prawnik — 21 pytań jest w raporcie skanu (`02-lawyer-questions.md`). Oznaczenia L01–L21 odnoszą się do tego raportu.

**PRZYJĘTE (2026-10-16):** DPIA warunkiem startu · MVP od 16 lat · minimum obowiązków hostingu DSA w MVP · dwie tabele miejsc (OSM osobno) · funkcje wymagane przez sklepy w MVP.

```
DECYZJE (blokery skanu):
DPIA (L01):       Ocena skutków dla ochrony danych przed pierwszym użytkownikiem MVP
                  (polski wykaz UODO: dane lokalizacyjne + nowa technologia).
                  Jej wynik może zaostrzyć C5, C6 i C8.
WIEK (L05):       MVP od 16 LAT (deklaracja wieku przy rejestracji, regulamin).
                  W Polsce zgoda bez rodzica dopiero od 16 lat, a płatna
                  subskrypcja dla 13–15 wymaga zgody rodzica. Konta 13–15
                  w osobnym etapie: zgoda rodzica, konto nadzorowane, brak
                  płatności bez rodzica, prywatność domyślna — po opinii
                  prawnika i rozstrzygnięciu projektu EU KIDS Act.
                  Dzieci nadal mogą być na zdjęciach i na listach osób (C9).
HOSTING (L10):    Patrz poprawka w C2. Regulamin z zasadami moderacji, zgłoszenie
                  w albumie, uzasadnienie decyzji, punkt kontaktowy.
MIEJSCA (L17):    Patrz zmiana w C4. Dwie tabele, OSM bez modyfikacji.
SKLEPY (L19):     W zakresie MVP:
                  · usunięcie konta Z POZIOMU APLIKACJI oraz przez stronę www
                    (Google). Wstrzymanie z B3 przy albumie bez następcy
                    pokazuje WYBÓR (następca / wygaśnięcie albumu), nigdy blokadę;
                  · BLOKOWANIE użytkownika w albumie i zgłaszanie treści
                    (NOWE — C2 miało tylko zgłaszanie publicznych treści);
                  · regulamin akceptowany przed dodaniem pierwszej treści,
                    opublikowany kontakt;
                  · ekran zgody na przekazanie danych dostawcy AI (spójny
                    z zgodą per album z C5).

DOPISANE DO WYMAGAŃ (bez osobnej decyzji, wynikają wprost z przepisów):
C5 + §11 (L02):   Wykrywanie twarzy pozostaje SAMYM WYKRYWANIEM: bez zapisywania
                  wzorców twarzy, bez grupowania „ta sama osoba", bez
                  wnioskowania o cechach (wiek, płeć, emocje). Każde z tych
                  rozszerzeń to dane biometryczne (art. 9 RODO) albo
                  kategoryzacja biometryczna (AI Act) — osobna decyzja i nowa
                  ocena prawna.
§9 + C5 (L13):    Każdy tekst wygenerowany przez AI (podsumowania, szkice opisów)
                  oznaczony znacznikiem do odczytu maszynowego od pierwszego dnia
                  (AI Act art. 50 ust. 2; produkt wejdzie po 2.08.2026). Ślad
                  artefaktu AI (C5) zapisuje ten znacznik. Zespół: środki
                  wspierające kompetencje w zakresie AI (art. 4).
B3 (L03):         Kopie zapasowe i odtwarzanie do punktu w czasie mają STAŁY,
                  udokumentowany okres wygasania; przy odtworzeniu kopii dane
                  z listy żądań usunięcia są usuwane ponownie. Polityka
                  prywatności mówi o tym wprost.
C10 (L15):        Zaproszenie e-mailem wyłącznie informacyjne — bez treści
                  promujących GEO (inaczej to informacja handlowa wymagająca
                  zgody adresata). Analityka w aplikacji tylko za zgodą.
C3 (L06–L09):     Ustalić w umowach, kto w każdym kanale jest sprzedawcą wobec
                  konsumenta. Web: funkcja „odstąp od umowy tutaj" z
                  potwierdzeniem e-mailem (dyrektywa stosowana w UE od
                  19.06.2026, w Polsce jeszcze niewdrożona). Zasady po
                  wygaśnięciu planu (B1, C7) w regulaminie od początku.
C8 / Z6 (L18):    Procedura obejmuje europejskie nakazy wydania i zabezpieczenia
                  dowodów elektronicznych (od 18.08.2026, także od organów
                  innych państw UE) oraz zabezpieczenie danych konta bez ich
                  ujawnienia.
BEZPIECZEŃSTWO    Aplikacja z backendem to prawdopodobnie produkt z elementami
(L16):            cyfrowymi (CRA): od 11.09.2026 zgłaszanie aktywnie
                  wykorzystywanych podatności (wczesne ostrzeżenie w 24 h),
                  całość od 11.12.2027 — kontakt security, runbook, wykaz
                  komponentów.
DOSTĘPNOŚĆ (L14): Mikroprzedsiębiorstwo jest wyłączone, ale UI od startu wg WCAG,
                  żeby przekroczenie progu nie oznaczało przebudowy.
SANKCJE (L21,     Do opinii prawnika: brak dystrybucji i sprzedaży w Rosji,
NIEZWERYFIKOWANE): na Białorusi, Krymie i okupowanych obwodach Ukrainy, w Iranie
                  i KRLD (sklepy, pośrednik); warunki korzystania tylko dla
                  konsumentów; brak kont firmowych z Rosji i Białorusi (zakaz
                  usług IT i AI dla podmiotów, art. 5n rozp. 833/2014);
                  sprawdzanie list sankcyjnych przy płatnościach bezpośrednich.
```

---

# Część 4 — Decyzje rozstrzygnięte i co zostało

## 15 decyzji produktowych — rozstrzygnięte 2026-10-15

| # | Decyzja | Rozstrzygnięcie | Gdzie |
|---|---|---|---|
| 1 | Przycinanie archiwum po 24 miesiącach | Właściciel wybiera w okienku 30 dni; bez decyzji zostaje materiał najstarszy chronologicznie | B1 |
| 2 | Okno ważności podpisu mediów | 5 minut | B2 |
| 3 | Artefakty pochodne po ponownym wpuszczeniu | Nie wracają; podsumowanie generuje się od nowa | B2 |
| 4 | Okno kosza na własne usunięcia | 30 dni; żądanie usunięcia danych pomija kosz | B3 |
| 5 | Podstawa przejęcia albumu po zgonie | Akt zgonu weryfikowany przez obsługę | B3 |
| 6 | Przekazanie następcy a zgody fundatora | Nie odwołuje zgód; fundator potwierdza losy własnych treści przy przekazaniu | B3 |
| 7 | Ważność kodu weryfikacyjnego i limit prób | 30 dni, 3 próby na kwartał | B4 |
| 8 | Prywatna nazwa miejsca po scaleniu | Zostaje przy wspomnieniu autora, widoczna tylko dla niego | B4 |
| 9 | Próg anonimowości statystyk | k = 20 na każdy pokazywany agregat | B4 |
| 10 | Daty w przyszłości | Zablokowane dla wspomnień | C1 |
| 11 | Terminy rozpatrywania zgłoszeń | Nielegalne 24 h, sporne 7 dni | C2 |
| 12 | Fotoksiążki | Zostają jako kierunek na później, z osobnym sprzedawcą i partnerem druku | C3 |
| 13 | Źródło podpowiedzi miejsc | OpenStreetMap teraz, komercyjne źródło dołożone później | C4 |
| 14 | Gdzie analizowane są zdjęcia | U siebie; do dostawcy modelu wyłącznie tekst | C5 |
| 15 | Co wychodzi do dostawcy modelu | Rodzaj miejsca i region, bez nazw i bez imion; nazwy wstawia Wasz system | C5 |
| 16 | Czy wspomnienie może istnieć bez miejsca (Z3) | Nie; miejsce obowiązkowe, ale może mieć stan „nieokreślone" do uzupełnienia | C6 |
| 17 | Precyzja miejsca | Cztery poziomy: punkt, miejscowość, region, kraj (plus „nieokreślone") | C6 |
| 18 | Import zdjęć bez geotagu | Poczekalnia „do przypisania"; system niczego nie zgaduje | C6 |
| 19 | Miejsce wrażliwe (szpital, dom) | Zapisane dokładnie, ukrywane przy udostępnianiu; metadane lokalizacji usuwane z plików | C6 |

## 11 decyzji produktowych — rozstrzygnięte 2026-10-16

| # | Decyzja | Rozstrzygnięcie | Gdzie |
|---|---|---|---|
| 20 | Wspomnienie z trasą (lot, rejs) | Punkt główny wybiera użytkownik (start albo cel); do odwiedzonych państw liczą się tylko miejsca zatrzymania | C6 |
| 21 | „Konto rodzinne" z §16 | Plan rodzinny Premium osobistego dla N osobnych kont, bez wglądu w cudze treści | C7 |
| 22 | Wypadnięcie z planu rodzinnego | Jak wygaśnięcie planu w B1: nic nowego ponad limit, nic nie jest usuwane przez 24 miesiące | C7 |
| 23 | Prywatny album po zgonie właściciela | Kontakt spadkowy wskazany z góry dostaje tylko odczyt; bez wskazania nikt | C8 |
| 24 | Dostęp obsługi do prywatnych treści | Wyłącznie na żądanie organu z podstawą prawną, każdy wgląd w dzienniku | C8 |
| 25 | Osoba we wspomnieniu | Wpis z prywatnej listy osób autora; powiązanie z kontem GEO tylko za zgodą tej osoby | C9 |
| 26 | Wyszukiwanie po odebraniu dostępu | Znika z wyników od razu; uprawnienia sprawdzane przy każdym zapytaniu | C9 |
| 27 | Powtórny import tych samych zdjęć | Ten sam plik tej samej osoby jest pomijany; między osobami bez deduplikacji | C10 |
| 28 | Zaproszenia e-mailem do osób bez konta | Jedno przypomnienie po 7 dniach; link „nie chcę zaproszeń" blokuje adres | C10 |
| 29 | Powiadomienia dotyczące danych | Nie do wyłączenia, e-mail i aplikacja; przy niedoręczeniu nieodwracalna operacja wstrzymana | C10 |
| 30 | Dołączenie do planu rodzinnego z własnym Premium | Jak w Spotify: plan rodzinny od razu, opłacony czas własnego Premium przepada; ekran mówi to wprost | C7 |

## 5 decyzji po skanie prawnym — 2026-10-16

| # | Decyzja | Rozstrzygnięcie | Gdzie |
|---|---|---|---|
| 31 | Ocena skutków dla ochrony danych | DPIA jest warunkiem startu MVP | C11 (L01) |
| 32 | Minimalny wiek użytkownika | MVP od 16 lat; konta 13–15 osobnym etapem | C11 (L05) |
| 33 | Moderacja w MVP | Minimum obowiązków hostingu DSA już w MVP; dyżury 24 h dopiero z publikacją | C2, C11 (L10) |
| 34 | Model danych miejsc | Dwie tabele: niezmienione dane OSM osobno, miejsca własne osobno | C4, C11 (L17) |
| 35 | Funkcje wymagane przez sklepy | Usunięcie konta w aplikacji i www, zgłaszanie i blokowanie, zgoda na AI — w MVP | C11 (L19) |

## Zadania — nie decyzje produktowe

| # | Zadanie | Gdzie |
|---|---|---|
| **Z1** | Opisać procedurę zgłoszenia i weryfikacji zgonu właściciela: kto zgłasza, jaki dokument, kto sprawdza, w jakim terminie. Wymaga konsultacji prawnej — to furtka do przejęcia cudzego archiwum. | B3 |
| **Z2** | Dopisać do §16 przypis o trwałości archiwum zależnej od utrzymania kont przez uczestników. | B3 |
| **Z4** | Przed wyborem pośrednika płatności sprawdzić obsługę sprzedaży B2B (numer VAT, odwrotne obciążenie). | C3 |
| **Z5** | Wynegocjować z dostawcą modelu zerową retencję danych wejściowych i brak logowania treści, na piśmie. | C5 |
| **Z6** | Opisać procedurę przyjmowania żądań organów: kto sprawdza podstawę prawną, kto ma techniczny dostęp, jak wygląda wpis w dzienniku i czy użytkownik jest informowany. Obejmuje europejskie nakazy dowodów elektronicznych i nakazy usunięcia treści terrorystycznych (1 h). Wymaga konsultacji prawnej. | C8, C11 |
| **Z7** | Wysłać prawnikowi 21 pytań ze skanu prawnego; najpierw L17 (ODbL), L21 (sankcje), L10 (DSA), L13 (AI Act), L01 (DPIA), L19 (usunięcie konta). Stan prawa ważny do 15.11.2026 — później powtórzyć weryfikację. | C11 |
| **Z8** | Przeprowadzić DPIA przed startem MVP (decyzja 31). | C11 |

Z1 obejmuje od 2026-10-16 także kontakt spadkowy dla albumów prywatnych (C8): ta sama weryfikacja aktu zgonu, inny skutek (tylko odczyt zamiast przejęcia).

*Z3 (wspomnienie bez miejsca) został rozstrzygnięty 2026-10-15 — patrz C6 oraz pozycje 16–19 powyżej. Numeracja pozostałych zadań zostaje bez zmian, żeby odnośniki w tekście dalej działały.*

---

# Część 5 — Sprzeczności i kolizje prawne

## Sprzeczności wewnątrz dokumentu

1. **§8 kontra sam §8.** *„Sylwester 2025/2026"* jako Etap 1 odtwarzania roku 2026. **Rozwiązane:** C1, model przedziałów — przykład jest poprawny, jeśli wspomnienie zapisane jako przedział.
2. **§2 kontra §16 i MVP.** *„Każde wspomnienie jest powiązane z miejscem"* kontra *„automatyczny import zdjęć z telefonu"* — większość zdjęć nie ma geotagu. **Rozwiązane:** C6 — zdjęcie bez geotagu nie jest wyjątkiem od reguły, tylko miejscem jeszcze nieustalonym, czekającym w poczekalni „do przypisania".
3. **§5 kontra §11.** Anonimowy link *„dla osób posiadających"* kontra *„Użytkownik powinien wiedzieć, kto może zobaczyć"*. **Rozwiązane:** B2, wymóg logowania.

## Kolizje z obowiązkami prawnymi

1. **§13 stawia „eksport wspomnień" w Premium.** Dostęp do własnych danych i ich przeniesienie to uprawnienie darmowe (RODO art. 15 i 20). Płatna może być tylko wygodna forma — fotoksiążka, film — nie sam eksport. Dodatkowo eksport musi być zawężony do treści własnych, bo inaczej omija cofnięcie dostępu z B2.
2. **Brak ścieżki dla osoby ze zdjęcia.** §11 obiecuje wykrywanie *„twarzy osób, które mogą nie chcieć publikacji"*, ale osoba niebędąca użytkownikiem nie ma jak o nic poprosić. **Rozwiązane:** C2, zgłoszenie bez konta.

**Dodatkowo do przemyślenia, nie rozstrzygnięte w tym przeglądzie:** §11 sam wymienia *„informacje medyczne"* wśród wykrywanych treści, a z samych miejsc i dat wnioskuje się zdrowie, wyznanie i orientację. To dane szczególnej kategorii i produkt zbiera je świadomie. Nazwa funkcji *„Sprawdź prywatność"* przenosi przy tym odpowiedzialność na system, który — jak §11 sam przyznaje — *„nie gwarantuje wykrycia wszystkich danych wrażliwych"*. Decyzja z C5 (analiza u siebie) ogranicza ryzyko, ale nie zmienia tego, że dane szczególnej kategorii są w systemie.

---

# Część 6 — Co zmienić w Description.txt

| Sekcja | Zmiana |
|---|---|
| **§2** | „autora i uczestników" → autorstwo per treść (B3) i osoby z prywatnej listy autora; oznaczenie osoby nie daje jej dostępu (C9). Wspomnienie przestaje mieć jednego właściciela. Przepisać zdanie o miejscu: *„Każde wspomnienie ma miejsce — punkt, miejscowość, region, kraj albo stan nieokreślone, który użytkownik może uzupełnić później"* (C6). Trasa: punkt główny wybiera użytkownik, liczą się miejsca zatrzymania (C6). |
| **§3** | Punkt na mapie znika, gdy pojemnik traci ostatnią treść (B3). Rozdzielić podkład mapy od bazy miejsc (C4). Znacznik pojawia się na poziomie zgodnym z precyzją miejsca, a „odwiedzone państwa i regiony" liczą tylko to, co ma odpowiednią precyzję (C6). |
| **§4** | Miejsce własne jako pełnoprawny wpis w Waszej tabeli; dane z OSM w osobnej, niezmienionej tabeli, z uznaniem autorstwa (C4 po zmianie, L17). |
| **§5** | Rozdzielić unieważnienie linku od odebrania dostępu — dziś jedno zdanie opisuje dwie komendy (B2). „Osobom posiadającym link" → „osobom, które właściciel wpuścił", z wymogiem logowania. Dodać czwartą rolę: kurator zasobów (B3). Dopisać dwa wyjątki do „dostęp ma wyłącznie właściciel": kontakt spadkowy i żądanie organu (C8). Zaproszenia e-mailem: jedno przypomnienie, blokada adresu (C10). |
| **§6** | Dopisać mechanikę miejsc i wspólnej puli, brak kumulacji planów, podatek za nabywcą, następcę albumu (B1, B3, C3). |
| **§7** | „Zapisywanie interesujących miejsc" musi trzymać pochodzenie i mieć **stan po wygaszeniu źródła** — ekran, którego nikt jeszcze nie zaprojektował, a wypadnie w „Odkrywaj" (§14). |
| **§8** | Zapisać model daty cywilnej, precyzji i przedziałów; poprawić przykład sylwestrowy albo zaznaczyć, że to przedział (C1). |
| **§9** | Zamienić zakaz zmyślania na mechanizm: lista faktów źródłowych, wskazywalność każdego zdania, oddzielenie interpretacji, zatwierdzenie (A2). Dopisać ujawnianie niepełnego materiału przy częściowych zgodach (C5). |
| **§10** | Przełącznik „pozwól innym planować na podstawie tego albumu" (C5). Uprawnienia w wyszukiwaniu sprawdzane przy każdym zapytaniu, odebrany dostęp znika od razu (A3, C9). Przy dwóch osobach o tym samym imieniu system dopytuje (C9). |
| **§11** | Kopia publiczna jako artefakt pochodny z odnośnikiem do źródeł, nie niezależna migawka (A5, B1). Ścieżka zgłoszenia bez konta (C2). Zapisać, że analiza obrazu odbywa się u Was (C5). Publikacja i udostępnienie muszą usuwać z plików metadane lokalizacji — inaczej ukrycie miejsca z C6 jest pozorne. Ostrożniej z nazwą funkcji i z obietnicą wykrywania. |
| **§12** | Procedura weryfikacji kodem (30 dni, 3 próby), reguła sporu, scalanie z zachowaniem prywatnej nazwy, zakaz przywilejów właściciela wobec cudzych wspomnień, próg k = 20 i jego skutek dla oferty (B4). |
| **§13** | Eksport poza Premium i zawężony do treści własnych. Fotoksiążki jako kierunek na później z osobnym sprzedawcą. Statystyki nie mogą być głównym argumentem planu biznesowego (B4, C3). |
| **§15** | **Napisać wprost, że MVP nie obejmuje publikacji** — tylko udostępnianie imienne i linkiem (C2). Przyjąć świadomie, że punkty 5, 6 i 9 razem wymagają rejestru pochodzenia i kaskady od pierwszego dnia. Dopisać do MVP (C11): minimalny wiek 16 lat, regulamin z zasadami moderacji, zgłaszanie i blokowanie w albumach, usunięcie konta w aplikacji, zgoda na AI, znakowanie tekstów AI, DPIA przed startem. |
| **§16** | Przypis o trwałości archiwum (zadanie Z2). Import zdjęć bez geotagu trafia do poczekalni „do przypisania" i nic nie jest zgadywane (C6); powtórny import pomija te same pliki (C10). „Konta rodzinne" → „plan rodzinny Premium osobistego", wyraźnie odróżniony od Premium dla albumu (C7). |
| **§17** | Zasada 1 („miejsce jest głównym punktem odniesienia") **zostaje**, z dopisaniem, że miejsce może być znane zgrubnie albo jeszcze nieustalone (C6). Zasada 4 („AI nie zmyśla") — po A2; C6 dokłada do niej zakaz zgadywania miejsca przy imporcie. |

---

# Część 7 — Czego ten przegląd nie dotknął

| Klaster | Stan |
|---|---|
| 📧 Powiadomienia | **zamknięte 2026-10-16 (C10)** — rezygnacja, nieprawidłowy adres i zaproszenia rozstrzygnięte. Drobiazg bez decyzji: czy właściciel widzi, że zaproszenie przyjęto (domyślnie tak, z listy uczestników B1) |
| 🔍 Wyszukiwanie | **prawie zamknięte (C9)** — uprawnienia, świeżość po odebraniu dostępu i rozpoznanie osoby rozstrzygnięte. Zostaje: stronicowanie przy „tysiącach wspomnień" (§3), czy wyniki obejmują kosz (30 dni, B3; rozsądnie: tylko dla autora) i koszt jednego zapytania w języku naturalnym (wejdzie w pulę AI z B1) |
| Równoczesna edycja | **nie dotknięte** — model pojemnika z B3 zmienia granicę jednostki spójności; osobny projekt agregatów |
| Import zdjęć (§16) | **zamknięte 2026-10-16** — poczekalnia (C6), powtórny import i duplikaty (C10) |
| Konta rodzinne (§16) | **zamknięte 2026-10-16 (C7)** — plan rodzinny Premium osobistego; dołączenie z własnym Premium jak w Spotify (decyzja 30) |

---

*Przegląd przeprowadzony 2026-10-15 (19 decyzji) i dokończony 2026-10-16 (11 decyzji: A6, trasa, plan rodzinny, import, osoby, wyszukiwanie, powiadomienia — C7–C10). Uzupełniony 2026-10-16 o wynik skanu prawnego (5 decyzji, C11). Pozostało 7 zadań zewnętrznych (Z1, Z2, Z4–Z8), trzy drobiazgi wyszukiwania z Części 7 i równoczesna edycja do projektu agregatów.*
