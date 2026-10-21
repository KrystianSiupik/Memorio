# MEM-E0-PLAT-19: Nawigacja aplikacji: zakładki Mapa · Albumy · Udostępnione mi · Konto, typy tras, przekierowanie niezalogowanych (S02)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E0](epiki/E0-szkielet-end-to-end-i-platforma-etap-1.md) Szkielet end-to-end i platforma (etap 1) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Mobile, -, MEM-E0-PLAT-19 |
| Moduł | - |
| Warstwa | UI |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P0 |
| Estymacja | 2–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-PLAT-12](MEM-E0-PLAT-12-api-v1-openapi-klient-dart-kontrola-zgodnosci-w-ci.md) |
| Blokuje | [MEM-E1-PLAT-01](MEM-E1-PLAT-01-ekran-s01-rejestracja.md); [MEM-E2-PLAT-01](MEM-E2-PLAT-01-ekran-s03-mapa-i-globus.md); [MEM-E2-PLAT-02](MEM-E2-PLAT-02-ekran-s04-nowe-wspomnienie-kreator-miejsce-tresc.md); [MEM-E2-PLAT-03](MEM-E2-PLAT-03-ekran-s05-szczegoly-wspomnienia.md); [MEM-E2-PLAT-04](MEM-E2-PLAT-04-ekran-s06-kosz.md); [MEM-E10-PLAT-01](MEM-E10-PLAT-01-ekran-s07-import-zdjec-i-poczekalnia.md); [MEM-E5-PLAT-01](MEM-E5-PLAT-01-ekran-s08-przesledz-okres.md); [MEM-E3-PLAT-01](MEM-E3-PLAT-01-ekran-s09-albumy-lista-i-nowy-album.md); [MEM-E3-PLAT-02](MEM-E3-PLAT-02-ekran-s10-album.md); [MEM-E3-PLAT-03](MEM-E3-PLAT-03-ekran-s11-zarzadzanie-albumem-wlasciciel.md); [MEM-E3-PLAT-04](MEM-E3-PLAT-04-ekran-s12-moje-zgody-i-powierzenia-w-albumie.md); [MEM-E3-PLAT-05](MEM-E3-PLAT-05-ekran-s13-powierzone-tresci-kurator.md); [MEM-E3-PLAT-06](MEM-E3-PLAT-06-ekran-s14-zaproszenie-do-albumu.md); [MEM-E4-PLAT-01](MEM-E4-PLAT-01-ekran-s15-wejscie-do-albumu-przez-link.md); [MEM-E4-PLAT-02](MEM-E4-PLAT-02-ekran-s16-udostepnione-mi.md); [MEM-E6-PLAT-01](MEM-E6-PLAT-01-ekran-s17-podsumowanie-okresu-ai.md); [MEM-E7-PLAT-01](MEM-E7-PLAT-01-ekran-s18-plan-i-subskrypcja.md); [MEM-E7-PLAT-03](MEM-E7-PLAT-03-ekran-s19-wybierz-co-zostanie-po-przycieciu.md); [MEM-E7-PLAT-04](MEM-E7-PLAT-04-ekran-s20-plan-rodzinny.md); [MEM-E8-PLAT-01](MEM-E8-PLAT-01-ekran-s21-zglos-tresc.md); [MEM-E8-PLAT-02](MEM-E8-PLAT-02-ekran-s22-moje-zgloszenia-i-decyzje.md); [MEM-E9-PLAT-01](MEM-E9-PLAT-01-ekran-s23-przejecie-albumu-nastepca.md); [MEM-E1-PLAT-02](MEM-E1-PLAT-02-ekran-s24-konto-i-prywatnosc.md); [MEM-E9-PLAT-02](MEM-E9-PLAT-02-ekran-s25-zglos-zgon-wlasciciela.md); [MEM-E8-PLAT-03](MEM-E8-PLAT-03-ekran-s26-panel-obslugi-zgloszenia-nakazy-sukcesja.md); [MEM-E8-PLAT-04](MEM-E8-PLAT-04-ekran-s27-zglos-tresc-z-aplikacji.md); [MEM-E11-PLAT-07](MEM-E11-PLAT-07-ekran-zgody-przy-pierwszym-uruchomieniu-analityka-.md); [MEM-E11-PLAT-08](MEM-E11-PLAT-08-sekcja-podwykonawcy-w-ekranie-konto-i-prywatnosc-s.md) |
| Kod roboczy | MEM-E0-PLAT-19 |
| Źródło | specyfikacja ekranu S02; mapa nawigacji |

## Cel

Nawigacja aplikacji: zakładki Mapa · Albumy · Udostępnione mi · Konto, typy tras, przekierowanie niezalogowanych (S02) — część epiku E0 „Szkielet end-to-end i platforma (etap 1)”.

## Kontekst

- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Nawigacja aplikacji: zakładki Mapa · Albumy · Udostępnione mi · Konto, typy tras, przekierowanie niezalogowanych (S02) (źródło: specyfikacja ekranu S02; mapa nawigacji)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E1-PLAT-01, MEM-E2-PLAT-01, MEM-E2-PLAT-02)

## Kryteria akceptacji

- [ ] Nawigacja aplikacji: zakładki Mapa · Albumy · Udostępnione mi · Konto, typy tras, przekierowanie niezalogowanych (S02). *Sprawdzenie: test/skrypt w CI lub ręcznie na środowisku*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
