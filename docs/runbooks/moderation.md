# Zgłoszenia treści i moderacja

Procedura obsługi (S2:C2, DSA art. 16, 17 i 20, D-309). Polecenia na serwerze jako root w `/opt/memorio/deploy`. Każdy wgląd w treść i każda decyzja trafiają do `/var/lib/memorio/operations.log`.

**Stan: procedura techniczna.** Kto ma dyżur, jak dociera alarm (HS-006) i kto rozpatruje odwołanie od własnej decyzji (HS-007), musi ustalić PO z prawnikiem przed startem.

## Terminy

- **Kategoria A** (niezgodne z prawem: wykorzystywanie dzieci, terroryzm, groźby, nawoływanie, inne): treść jest ukryta od razu po zgłoszeniu. Decyzja **w ciągu 24 godzin**.
- **Kategoria B** (sporne: wizerunek, dane osobowe, zniesławienie, prawa autorskie, inne): treść zostaje widoczna. Decyzja **w ciągu 7 dni**.
- Odwołanie autora: od decyzji o ukryciu, przez 6 miesięcy.

## Kroki

1. Sprawdź, co czeka, najpilniejsze pierwsze: `moderation/moderation.sh list`.
2. Obejrzyj zgłoszenie i treść: `moderation/moderation.sh show <id zgłoszenia> /root/zgloszenie`. Zdjęcia trafiają do katalogu; wgląd zapisuje się w dzienniku.
   - Zgłoszenie bez konta nie wskazuje treści po identyfikatorze: znajdź ją po opisie miejsca.
3. **Usuń pobrane pliki:** `rm -r /root/zgloszenie`.
4. Zdecyduj, z uzasadnieniem, które dostaną obie strony (bez danych zgłaszającego):
   - `moderation/moderation.sh decide <id> removed --reason "…"`: treść ukryta, widzi ją tylko autor; nic nie jest usuwane,
   - `moderation/moderation.sh decide <id> kept --reason "…"`: treść zostaje albo wraca.
5. Odwołanie (stan `Appealed` na liście): `moderation/moderation.sh appeal <id> reversed|upheld --reason "…"`.
6. Sprawdź wynik po minucie: `moderation/moderation.sh result <id>`. Pole `result` powinno mieć wartość `Applied`.

## Treści z kategorii A

Materiały z wykorzystywaniem dzieci i treści terrorystyczne zgłoś dalej odpowiednim organom, poza tym systemem. Treść zostaje ukryta, nie usunięta: to dowód.

## Nakazy organów (Z6)

**Stan: procedura techniczna.** Kto sprawdza podstawę prawną nakazu i czy informujemy osobę, której dotyczy, ustala PO z prawnikiem (Z6). Wpisuj tylko nakaz, którego podstawę już sprawdzono.

- **Usunięcie treści** (w tym treści terrorystyczne: **1 godzina**, L11): `moderation/moderation.sh order remove memory|album <id> --authority "…" --reference "…"`. Worker ukrywa treść w ciągu minuty; autor dostaje wiadomość, chyba że dodasz `--confidential`. Treść nie jest usuwana.
- **Zabezpieczenie danych** (L18): `moderation/moderation.sh order preserve <id konta> --authority "…" --reference "…"`. Od tej chwili nic z konta nie znika na stałe: ani wspomnienia z kosza, ani na żądanie usunięcia, ani samo konto. Czeka niewidoczne do zamknięcia nakazu.
- **Wydanie danych**: `moderation/moderation.sh order produce <id konta> …`, potem `moderation/moderation.sh disclose <id nakazu> /root/wydanie --what "co wydajesz"`. Dane wychodzą dopiero po zabezpieczeniu i po wpisie do dziennika. **Usuń katalog po przekazaniu.**
- **Koniec**: `moderation/moderation.sh close <id nakazu>`. Blokada znika, a to, co czekało na usunięcie, usuwa dzienne zadanie Archiwum.
- Stan: `moderation/moderation.sh orders`; dziennik wglądów i wydań: `moderation/moderation.sh log [<id>]`.
