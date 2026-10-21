# E6: Podsumuj: podsumowania AI bez zmyślania, z kaskadą

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony |
| Cel planu | G3 |
| Priorytet | P0 |
| Moduły | narracja-ai, pochodzenie, dostep, - |
| Zależy od epików | E3, E4 |
| Estymacja | 23–52 h |
| Źródło | S1 §15 pkt 8; S2:A2, C5 |

## Kryteria ukończenia epiku

- [ ] Wszystkie tickety P0 epiku zamknięte, a testy przejść jego ekranów zielone. *Sprawdzenie: CI + test na urządzeniu*

## Tickety

| Kod | Tytuł | Moduł / warstwa | h | Status |
|---|---|---|---|---|
| [MEM-E6-NARRACJA-AI-01](../MEM-E6-NARRACJA-AI-01-udziel-zgody-na-ai-dla-albumu-c80.md) | Udziel zgody na AI dla albumu (C80) | narracja-ai / aplikacja | 1–3 | gotowy |
| [MEM-E6-NARRACJA-AI-02](../MEM-E6-NARRACJA-AI-02-odwolaj-zgode-na-ai-c81.md) | Odwołaj zgodę na AI (C81) | narracja-ai / aplikacja | 1–3 | gotowy |
| [MEM-E6-NARRACJA-AI-03](../MEM-E6-NARRACJA-AI-03-zamow-podsumowanie-okresu-c82.md) | Zamów podsumowanie okresu (C82) | narracja-ai / aplikacja | 2–4 | gotowy |
| [MEM-E6-NARRACJA-AI-04](../MEM-E6-NARRACJA-AI-04-zatwierdz-i-zapisz-podsumowanie-c83.md) | Zatwierdź i zapisz podsumowanie (C83) | narracja-ai / aplikacja | 1–3 | gotowy |
| [MEM-E6-NARRACJA-AI-05](../MEM-E6-NARRACJA-AI-05-dodaj-podsumowanie-do-albumu-c84.md) | Dodaj podsumowanie do albumu (C84) | narracja-ai / aplikacja | 1–3 | gotowy |
| [MEM-E6-POCHODZENIE-01](../MEM-E6-POCHODZENIE-01-uniewaznij-artefakty-pochodne-c31.md) | Unieważnij artefakty pochodne (C31) | pochodzenie / domena + aplikacja | 3–4 | gotowy |
| [MEM-E6-POCHODZENIE-02](../MEM-E6-POCHODZENIE-02-zarejestruj-artefakt-c154.md) | Zarejestruj artefakt (C154) | pochodzenie / domena + aplikacja | 3–4 | gotowy |
| [MEM-E6-NARRACJA-AI-06](../MEM-E6-NARRACJA-AI-06-zapytanie-materialy-dozwolone-do-podsumowania-zgod.md) | Zapytanie: Materiały dozwolone do podsumowania (zgody, uprawnienia) (R8) | narracja-ai / infrastruktura (query handler) | 1–3 | gotowy |
| [MEM-E6-DOSTEP-01](../MEM-E6-DOSTEP-01-read-port-dostep-dla-narracja-ai-ktore-tresci-osob.md) | Read port dostep → dla narracja-ai: które treści osoba może czytać teraz | dostep / kontrakt | 1–2 | gotowy |
| [MEM-E6-POCHODZENIE-03](../MEM-E6-POCHODZENIE-03-write-port-pochodzenie-dla-narracja-ai-zarejestruj.md) | Write port pochodzenie → dla narracja-ai: zarejestruj artefakt z listą źródeł (C154) | pochodzenie / kontrakt | 1–2 | gotowy |
| [MEM-E6-POCHODZENIE-04](../MEM-E6-POCHODZENIE-04-polityka-p4-zawsze-gdy-tresc-usunieta-definitywnie.md) | Polityka P4: Zawsze gdy treść usunięta definitywnie, wtedy unieważnij artefakty pochodne, które ją cytują | pochodzenie / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E6-POCHODZENIE-05](../MEM-E6-POCHODZENIE-05-polityka-p9-zawsze-gdy-wklady-wycofane-z-albumu-wt.md) | Polityka P9: Zawsze gdy wkłady wycofane z albumu, wtedy unieważnij artefakty pochodne, które je cytują | pochodzenie / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E6-POCHODZENIE-06](../MEM-E6-POCHODZENIE-06-polityka-p10-zawsze-gdy-dostep-odebrany-wtedy-unie.md) | Polityka P10: Zawsze gdy dostęp odebrany, wtedy unieważnij artefakty pochodne powstałe z tego albumu u tej osoby | pochodzenie / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E6-NARRACJA-AI-07](../MEM-E6-NARRACJA-AI-07-zdarzenie-integracyjne-zgoda-na-ai-odwolana-e81-w-.md) | Zdarzenie integracyjne „Zgoda na AI odwołana” (E81) w kontrakcie modułu narracja-ai | narracja-ai / kontrakt | 1–2 | gotowy |
| [MEM-E6-POCHODZENIE-07](../MEM-E6-POCHODZENIE-07-polityka-p12-zawsze-gdy-zgoda-na-ai-odwolana-wtedy.md) | Polityka P12: Zawsze gdy zgoda na AI odwołana, wtedy unieważnij artefakty powstałe z treści tej osoby | pochodzenie / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E6-POCHODZENIE-08](../MEM-E6-POCHODZENIE-08-polityka-p34-zawsze-gdy-archiwum-przyciete-wtedy-u.md) | Polityka P34: Zawsze gdy archiwum przycięte, wtedy unieważnij artefakty cytujące usunięty materiał | pochodzenie / aplikacja (handler zdarzenia) | 1–3 | gotowy |
| [MEM-E6-PLAT-01](../MEM-E6-PLAT-01-ekran-s17-podsumowanie-okresu-ai.md) | Ekran S17 „Podsumowanie okresu (AI)” | - / UI | 2–4 | gotowy |

## Propozycje (poza planem, do decyzji)

| Kod | Tytuł | Decyzja |
|---|---|---|
| [MEM-PROP-01](../propozycje/MEM-PROP-01-ewaluacja-jakosci-podsumowan-ai.md) | Ewaluacja jakości podsumowań AI (czy AI nie zmyśla) | D-081 |
| [MEM-PROP-02](../propozycje/MEM-PROP-02-embeddingi-wyszukiwanie-semantyczne.md) | Embeddingi: dobór materiałów do podsumowań (RAG) i wyszukiwanie semantyczne | D-082 |
