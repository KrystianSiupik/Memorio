# MEM-PROP-02: Embeddingi: wyszukiwanie semantyczne i dobór materiałów do podsumowań

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | po MVP (backlog z wyzwalaczem) |
| Wersja | **propozycja** (poza planem MVP, do decyzji: D-082) |
| Typ | Spike + Task |
| Etykiety | Backend, archiwum, narracja-ai, AI, MEM-PROP-02 |
| Moduł | archiwum (indeks treści), narracja-ai (dobór materiałów), dostep (filtr) |
| Warstwa | infrastruktura (adapter wyszukiwania, adapter embeddingów) |
| Klasa problemu | Transformacja i prezentacja |
| Priorytet | do ustalenia |
| Estymacja | spike 3–4 h; wdrożenie 10–16 h (do podziału po decyzji) |
| Pewność | niska |
| Status | propozycja |
| Zależy od | D-031 (wyszukiwanie za jednym portem); [MEM-E0-PLAT-14](../MEM-E0-PLAT-14-testy-macierzy-dostepu-uzytkownik-zasob-i-wyszukiw.md) |
| Źródło | propozycja użytkownika 2026-10-21; D-075 (inteligentne wyszukiwanie po MVP) |

## Cel

Dwa zastosowania jednego mechanizmu (wektorowa reprezentacja opisów wspomnień):

1. **Wyszukiwanie po znaczeniu:** „lato nad morzem z babcią” znajduje wspomnienia opisane innymi słowami.
2. **Dobór materiałów do podsumowania (RAG):** gdy okres ma więcej materiałów, niż mieści kontekst modelu,
   do podsumowania trafiają najbardziej istotne, a nie pierwsze z brzegu.

## Zakres (propozycja)

- **Spike:** porównanie `pgvector` w istniejącym PostgreSQL (bez nowej usługi i podwykonawcy, zgodne z D-028)
  z Elasticsearch/OpenSearch z D-075 (wyszukiwanie hybrydowe); trafność na zestawie syntetycznym, koszt, pamięć.
- **Model embeddingów:** preferowany model wielojęzyczny uruchamiany u nas (CPU, w workerze), bo embedding
  opisu to przetwarzanie treści użytkownika; dostawca zewnętrzny tylko w UE, z umową powierzenia (D-046).
- **Uprawnienia:** filtr Dostępu w tym samym zapytaniu co wyszukiwanie wektorowe (ADR 0011, D-031); odebrany
  dostęp znika od razu (C9). Test macierzy dostępu (PLAT-14) obejmuje też wyszukiwanie semantyczne.
- **Usuwanie:** embedding jest daną osobową pochodną treści; znika razem z treścią (kosz, usunięcie definitywne,
  usunięcie konta, przycięcie), tą samą kaskadą co treść.
- **Wersjonowanie:** wersja modelu embeddingów przy każdym wektorze; zmiana modelu = przeliczenie w tle.
- **Pochodzenie:** materiały wybrane do podsumowania trafiają na listę źródeł (C154), więc kaskada unieważnień
  działa bez zmian.

## Poza zakresem

- rekomendacje i „Odkrywaj” (D-070);
- embeddingi zdjęć (analiza obrazu poza MVP, D-057).

## Kryteria akceptacji (do doprecyzowania po spike'u)

- [ ] Raport ze spike'u: trafność, koszt i rekomendacja pgvector vs wyszukiwarka. *Sprawdzenie: przegląd*
- [ ] Wyszukiwanie nigdy nie zwraca treści bez uprawnienia; odebrany dostęp znika od razu. *Sprawdzenie: test macierzy dostępu*
- [ ] Usunięcie treści usuwa jej embedding. *Sprawdzenie: test integracyjny*

## Wyzwalacz

Decyzja o inteligentnym wyszukiwaniu (S1 §10), skargi na trafność wyszukiwania (D-075) albo podsumowania, w
których materiały okresu nie mieszczą się w kontekście modelu.

## Ryzyka i pytania

- Podstawa prawna i DPIA dla przetwarzania opisów przez model embeddingów (pytanie do prawnika, jak przy AI).
- Czy wyszukiwanie semantyczne wymaga osobnej zgody, jak podsumowania AI (zgoda na AI per album, C80)?
