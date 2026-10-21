# E7: Plany, limity i retencja 24 miesiące

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony |
| Cel planu | G1 |
| Priorytet | P0 |
| Moduły | plany, powiadomienia, limity, archiwum, - |
| Zależy od epików | E0 |
| Estymacja | 70–129 h |
| Źródło | S2:B1, C3, C7 |

## Kryteria ukończenia epiku

- [ ] Wszystkie tickety P0 epiku zamknięte, a testy przejść jego ekranów zielone. *Sprawdzenie: CI + test na urządzeniu*

## Tickety

| Kod | Tytuł | Moduł / warstwa | h | Status |
|---|---|---|---|---|
| [MEM-E7-PLANY-01](../MEM-E7-PLANY-01-nadaj-uprawnienia-planu-c90.md) | Nadaj uprawnienia planu (C90) | plany / domena + aplikacja | 3–4 | gotowy |
| [MEM-E7-PLANY-02](../MEM-E7-PLANY-02-przelacz-w-tryb-tylko-do-odczytu-c91.md) | Przełącz w tryb tylko do odczytu (C91) | plany / domena + aplikacja | 3–4 | gotowy |
| [MEM-E7-PLANY-03](../MEM-E7-PLANY-03-odswiez-stan-subskrypcji-u-posrednika-c96.md) | Odśwież stan subskrypcji u pośrednika (C96) | plany / aplikacja | 2–4 | gotowy |
| [MEM-E7-POWIADOMIENIA-01](../MEM-E7-POWIADOMIENIA-01-wyslij-ostrzezenie-przed-przycieciem-c92.md) | Wyślij ostrzeżenie przed przycięciem (C92) | powiadomienia / aplikacja | 2–4 | gotowy |
| [MEM-E7-PLANY-04](../MEM-E7-PLANY-04-wstrzymaj-przyciecie-archiwum-c93.md) | Wstrzymaj przycięcie archiwum (C93) | plany / domena + aplikacja | 3–4 | gotowy |
| [MEM-E7-PLANY-05](../MEM-E7-PLANY-05-wskaz-material-do-zachowania-c94.md) | Wskaż materiał do zachowania (C94) | plany / domena + aplikacja | 3–4 | gotowy |
| [MEM-E7-PLANY-06](../MEM-E7-PLANY-06-zlec-przyciecie-archiwum-do-darmowego-limitu-c95.md) | Zleć przycięcie archiwum do darmowego limitu (C95) | plany / domena + aplikacja | 3–4 | gotowy |
| [MEM-E7-PLANY-07](../MEM-E7-PLANY-07-zapros-do-planu-rodzinnego-c97.md) | Zaproś do planu rodzinnego (C97) | plany / domena + aplikacja | 3–4 | zablokowany |
| [MEM-E7-PLANY-08](../MEM-E7-PLANY-08-dolacz-do-planu-rodzinnego-c98.md) | Dołącz do planu rodzinnego (C98) | plany / domena + aplikacja | 3–4 | zablokowany |
| [MEM-E7-PLANY-09](../MEM-E7-PLANY-09-usun-osobe-z-planu-rodzinnego-c99.md) | Usuń osobę z planu rodzinnego (C99) | plany / domena + aplikacja | 3–4 | gotowy |
| [MEM-E7-PLANY-10](../MEM-E7-PLANY-10-przelacz-konto-w-tryb-ponad-limitem-c100.md) | Przełącz konto w tryb ponad limitem (C100) | plany / domena + aplikacja | 3–4 | gotowy |
| [MEM-E7-LIMITY-01](../MEM-E7-LIMITY-01-ustaw-limit-puli-c152.md) | Ustaw limit puli (C152) | limity / domena + aplikacja | 3–4 | gotowy |
| [MEM-E7-ARCHIWUM-01](../MEM-E7-ARCHIWUM-01-przytnij-archiwum-do-limitu-nadmiar-do-kosza-na-30.md) | Przytnij archiwum do limitu (nadmiar do kosza na 30 dni) (C24) | archiwum / aplikacja | 2–4 | gotowy |
| [MEM-E7-PLANY-11](../MEM-E7-PLANY-11-aktywuj-plan-albumu-po-przejeciu-c165.md) | Aktywuj plan albumu po przejęciu (C165) | plany / domena + aplikacja | 3–4 | gotowy |
| [MEM-E7-ARCHIWUM-02](../MEM-E7-ARCHIWUM-02-przywroc-przyciety-material-z-kosza-c168.md) | Przywróć przycięty materiał z kosza (C168) | archiwum / domena + aplikacja | 3–4 | gotowy |
| [MEM-E7-PLANY-12](../MEM-E7-PLANY-12-zapytanie-stan-planu-tryb-terminy-zuzycie-wybor-do.md) | Zapytanie: Stan planu (tryb, terminy, zużycie, wybór do zachowania) (R16) | plany / infrastruktura (query handler) | 1–3 | gotowy |
| [MEM-E7-PLANY-13](../MEM-E7-PLANY-13-polityka-p13-zawsze-gdy-subskrypcja-aktywowana-wte.md) | Polityka P13: Zawsze gdy subskrypcja aktywowana, wtedy nadaj uprawnienia planu | plany / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E7-PLANY-14](../MEM-E7-PLANY-14-polityka-p14-zawsze-gdy-subskrypcja-wygasla-wtedy-.md) | Polityka P14: Zawsze gdy subskrypcja wygasła, wtedy przełącz album lub konto w tryb tylko do odczytu | plany / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E7-PLANY-15](../MEM-E7-PLANY-15-zdarzenie-integracyjne-nadszedl-termin-ostrzezenia.md) | Zdarzenie integracyjne „Nadszedł termin ostrzeżenia (90/30/7/1 dni przed przycięciem)” (E97) w kontrakcie modułu plany | plany / kontrakt | 1–2 | gotowy |
| [MEM-E7-POWIADOMIENIA-02](../MEM-E7-POWIADOMIENIA-02-polityka-p15-zawsze-gdy-nadejdzie-termin-ostrzezen.md) | Polityka P15: Zawsze gdy nadejdzie termin ostrzeżenia, wtedy ostrzeż właściciela i uczestników | powiadomienia / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E7-POWIADOMIENIA-03](../MEM-E7-POWIADOMIENIA-03-zdarzenie-integracyjne-ostrzezenie-niedoreczone-e9.md) | Zdarzenie integracyjne „Ostrzeżenie niedoręczone” (E99) w kontrakcie modułu powiadomienia | powiadomienia / kontrakt | 1–2 | gotowy |
| [MEM-E7-PLANY-16](../MEM-E7-PLANY-16-polityka-p16-zawsze-gdy-ostrzezenie-niedoreczone-w.md) | Polityka P16: Zawsze gdy ostrzeżenie niedoręczone, wtedy wstrzymaj przycięcie | plany / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E7-PLANY-17](../MEM-E7-PLANY-17-zdarzenie-integracyjne-przyciecie-wstrzymane-e100-.md) | Zdarzenie integracyjne „Przycięcie wstrzymane” (E100) w kontrakcie modułu plany | plany / kontrakt | 1–2 | gotowy |
| [MEM-E7-POWIADOMIENIA-04](../MEM-E7-POWIADOMIENIA-04-polityka-p26-zawsze-gdy-przyciecie-wstrzymane-wted.md) | Polityka P26: Zawsze gdy przycięcie wstrzymane, wtedy ponawiaj ostrzeżenie co miesiąc aż do skutecznego doręczenia | powiadomienia / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E7-PLANY-18](../MEM-E7-PLANY-18-polityka-p17-zawsze-gdy-mina-24-miesiace-tylko-do-.md) | Polityka P17: Zawsze gdy miną 24 miesiące tylko do odczytu, wtedy przytnij archiwum do darmowego limitu | plany / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E7-PLANY-19](../MEM-E7-PLANY-19-polityka-p18-zawsze-gdy-osoba-wypadnie-z-planu-rod.md) | Polityka P18: Zawsze gdy osoba wypadnie z planu rodzinnego, wtedy przełącz jej konto w tryb „nic nowego ponad limit” | plany / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E7-PLANY-20](../MEM-E7-PLANY-20-zdarzenie-integracyjne-plan-aktywny-e94-w-kontrakc.md) | Zdarzenie integracyjne „Plan aktywny” (E94) w kontrakcie modułu plany | plany / kontrakt | 1–2 | gotowy |
| [MEM-E7-PLANY-21](../MEM-E7-PLANY-21-zdarzenie-integracyjne-album-lub-konto-w-trybie-ty.md) | Zdarzenie integracyjne „Album lub konto w trybie tylko do odczytu” (E95) w kontrakcie modułu plany | plany / kontrakt | 1–2 | gotowy |
| [MEM-E7-LIMITY-02](../MEM-E7-LIMITY-02-polityka-p27-zawsze-gdy-plan-aktywny-albo-przelacz.md) | Polityka P27: Zawsze gdy plan aktywny albo przełączony w tryb tylko do odczytu, wtedy ustaw limit puli | limity / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E7-PLANY-22](../MEM-E7-PLANY-22-zdarzenie-integracyjne-przyciecie-archiwum-zlecone.md) | Zdarzenie integracyjne „Przycięcie archiwum zlecone” (E103) w kontrakcie modułu plany | plany / kontrakt | 1–2 | gotowy |
| [MEM-E7-ARCHIWUM-03](../MEM-E7-ARCHIWUM-03-polityka-p33-zawsze-gdy-przyciecie-zlecone-wtedy-p.md) | Polityka P33: Zawsze gdy przycięcie zlecone, wtedy przytnij archiwum według wyboru właściciela albo od najstarszego | archiwum / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E7-ARCHIWUM-04](../MEM-E7-ARCHIWUM-04-zdarzenie-integracyjne-archiwum-przyciete-nadmiar-.md) | Zdarzenie integracyjne „Archiwum przycięte (nadmiar w koszu 30 dni)” (E25) w kontrakcie modułu archiwum | archiwum / kontrakt | 1–2 | gotowy |
| [MEM-E7-PLANY-23](../MEM-E7-PLANY-23-polityka-p36-zawsze-gdy-album-bez-wlasciciela-wted.md) | Polityka P36: Zawsze gdy album bez właściciela, wtedy przełącz plan albumu w tryb tylko do odczytu po końcu opłaconego okresu | plany / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E7-PLANY-24](../MEM-E7-PLANY-24-zdarzenie-integracyjne-plan-albumu-bez-wlasciciela.md) | Zdarzenie integracyjne „Plan albumu bez właściciela opłacony przez uczestnika” (E171) w kontrakcie modułu plany | plany / kontrakt | 1–2 | gotowy |
| [MEM-E7-PLANY-25](../MEM-E7-PLANY-25-polityka-p41-zawsze-gdy-album-przejety-przez-uczes.md) | Polityka P41: Zawsze gdy album przejęty przez uczestnika, wtedy aktywuj plan albumu | plany / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E7-ARCHIWUM-05](../MEM-E7-ARCHIWUM-05-polityka-p44-zawsze-gdy-plan-znowu-aktywny-w-ciagu.md) | Polityka P44: Zawsze gdy plan znowu aktywny w ciągu 30 dni od przycięcia, wtedy przywróć przycięty materiał z kosza | archiwum / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E7-PLAT-01](../MEM-E7-PLAT-01-ekran-s18-plan-i-subskrypcja.md) | Ekran S18 „Plan i subskrypcja” | - / UI | 2–4 | gotowy |
| [MEM-E7-PLAT-02](../MEM-E7-PLAT-02-ekran-s18-plan-i-subskrypcja-akcje-dolacz-do-planu.md) | Ekran S18 „Plan i subskrypcja”: akcje „Dołącz do planu rodzinnego” | - / UI | 1–2 | zablokowany |
| [MEM-E7-PLAT-03](../MEM-E7-PLAT-03-ekran-s19-wybierz-co-zostanie-po-przycieciu.md) | Ekran S19 „Wybierz, co zostanie po przycięciu” | - / UI | 2–4 | gotowy |
| [MEM-E7-PLAT-04](../MEM-E7-PLAT-04-ekran-s20-plan-rodzinny.md) | Ekran S20 „Plan rodzinny” | - / UI | 2–4 | gotowy |
