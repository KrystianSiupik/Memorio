# Pytania do prawnika

Pytania, które wyszły podczas budowy aplikacji, jedno miejsce zamiast rozproszonych wpisów w `decisions.md`. Wcześniejsze listy są osobno: 21 pytań ze skanu prawnego (`PRZEGLAD-WYMAGAN-2026-10-15.md`, punkt Z7) i 13 z przeglądu prawnego z 2026-10-17 (`decisions.md`). Stan prawa w tamtych przeglądach jest ważny do 2026-11-15.

To nie jest porada prawna. Kolumna „Jak jest teraz” opisuje działanie aplikacji, nie ocenę legalności.

| # | Pytanie | Jak jest teraz | Źródło |
|---|---|---|---|
| 1 | Czy treści osoby usuniętej z albumu przez właściciela mogą w nim zostać bez jej aktywnej zgody? Jeśli tak, to jak długo, i co powinno się stać, gdy osoba nie odpowie? Nie mamy ustalonej długości terminu „zostaw/zabierz” ani skutku jego upływu | Treści zostają bez terminu, a osoba dostaje prośbę o wybór i może je zabrać w każdej chwili. Decyzja PO na razie (2026-11-04) | D-249, D-262 |
| 2 | Czy po wyjściu z albumu z wyborem „zostaw” zgoda może trwać bezterminowo, skoro osoba może ją odwołać w każdej chwili? | Trwa do odwołania; po odwołaniu treści znikają z albumu | D-249, C55 |
| 3 | Czy adres e-mail może być widoczny dla pozostałych osób w albumie, skoro konto nie ma nazwy? Co trzeba o tym powiedzieć w polityce prywatności i przy zaproszeniu? | Widoczny dla osób w albumie. Decyzja PO (2026-11-04) | D-253, D-263 |
| 4 | Czy wiersz po usuniętym koncie (identyfikator, daty, zgoda, bez e-maila) może zostać na stałe jako ślad, co się stało z oknem usunięcia? | Zostaje na stałe | D-204 |
| 5 | Czy wystarczy sama deklaracja ukończenia 16 lat, bez weryfikacji wieku? Co z kontem osoby, która okaże się młodsza? | Sama deklaracja | HS-008 |
| 6 | Czy można wstrzymać usunięcie konta do czasu wskazania następcy albo zgody na wygaśnięcie własnych albumów, bez limitu czasu? | Wstrzymane tylko do wyboru | HS-003, D-201 |
| 8 | Zaproszenie idzie e-mailem na adres podany przez właściciela albumu, także do osoby bez konta. Czy to dopuszczalne bez jej zgody? Osoba bez konta może wyłączyć zaproszenia dopiero po założeniu konta: czy potrzebny jest sposób bez konta? | Wiadomość z nazwą albumu i adresem e-mail zapraszającego, jedno przypomnienie po 7 dniach, do 10 zaproszeń dziennie z konta, wyłączenie tylko z konta | D-265, D-267, D-268 |
| 7 | Skrót adresu na liście blokady zaproszeń zostaje też po usunięciu konta. Czy to dopuszczalne i czy jest to wciąż dana osobowa? | Skrót SHA-256 adresu, bez samego adresu | MEM-E3-POWIADOMIENIA-02 |
| 9 | Gdy właściciel albumu przestanie płacić, po 24 miesiącach wspomnienia ponad limit wypadają z albumu, ale autorzy je zachowują. Czy treść konta ponad darmowy limit może zostać u autora bezterminowo, i czy regulamin powinien to opisywać? Pozostali uczestnicy tracą wtedy dostęp do tych wspomnień: czy wystarczą ostrzeżenia 90, 30, 7 i 1 dzień przed? | Tak działa; ostrzeżenia idą e-mailem do wszystkich | D-290 |
| 10 | Czy podstawa przechowywania przyciętej treści przez 30 dni w koszu musi być opisana w regulaminie, i jak? | Przycięta treść leży 30 dni w koszu, po opłaceniu planu wraca sama | HS-022 |
| 11 | Płatności obsłuży RevenueCat, firma spoza UE. Trafi tam identyfikator konta (UUID, bez adresu e-mail) i dane zakupu ze sklepu. Czy potrzebne są standardowe klauzule umowne, czy wystarczy ich DPA, i co napisać w polityce prywatności? | Jeszcze niepodłączone; dotyczy dopiero startu sprzedaży | D-291 |
| 12 | Akt zgonu to dane o osobie zmarłej i zgłaszającym. Czy wystarczy dostęp tylko dla obsługi z dziennikiem i usunięcie 12 miesięcy po zamknięciu sprawy? Jakie kryteria weryfikacji są wystarczające, skoro konto nie ma nazwiska? | Tak działa; kryteriów jeszcze nie ma | D-055, D-295 |
| 13 | Kontakt spadkowy po potwierdzonym zgonie czyta prywatne wspomnienia, także z opisami innych osób. Czy potrzebna jest osobna zgoda przy wskazaniu, i co ze spadkobiercami, którzy nie zostali wskazani? | Wskazuje właściciel z góry; bez wskazania nikt nie dostaje dostępu | D-296, S2:C8 |

