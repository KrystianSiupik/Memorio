# Zgłoszenie zgonu właściciela albumu

Procedura obsługi (zadanie Z1, S2:B3, D-295). Polecenia na serwerze jako root w `/opt/memorio/deploy`. Każdy odczyt aktu i każda decyzja trafiają do `/var/lib/memorio/operations.log`.

**Stan: procedura techniczna.** Kryteria uznania dokumentu za wiarygodny, termin odpowiedzi i to, kto może decydować, musi ustalić PO z prawnikiem przed startem.

## Kroki

1. Sprawdź, co czeka: `succession/cases.sh list`.
2. Pobierz akt do tymczasowego pliku: `succession/cases.sh certificate <id sprawy> /root/akt`.
3. Sprawdź dokument: czy czytelny, czy dotyczy tej osoby. Aplikacja zna właściciela tylko po adresie e-mail, więc zgodność nazwiska z kontem trzeba potwierdzić osobno.
4. **Usuń plik:** `rm /root/akt`. Akt nie może zostać poza magazynem.
5. Sprawdź skrzynkę adresu nadawcy powiadomień: właściciel dostał wiadomość o zgłoszeniu i mógł odpowiedzieć, że żyje (D-304). Odpowiedź właściciela to odrzucenie zgłoszenia.
6. Zdecyduj:
   - `succession/cases.sh confirm <id sprawy> --reason "..."`: zgon potwierdzony,
   - `succession/cases.sh reject <id sprawy> --reason "..."`: zgłoszenie odrzucone, zgłaszający dostanie wiadomość.
7. Sprawdź wynik po minucie: `succession/cases.sh result <id sprawy>`. Pole `outcome` powinno mieć wartość `Applied`.

## Co dzieje się po potwierdzeniu

- Wszystkie albumy tej osoby idą do decyzji ich następców, 30 dni na odpowiedź, przypomnienia po 7 i 21 dniach.
- Album bez następcy od razu zostaje bez właściciela. Plan działa do końca opłaconego okresu, a uczestnik może album przejąć.
- Kontakt spadkowy, jeśli był wskazany, dostaje odczyt prywatnych wspomnień.

**Potwierdzenia nie da się cofnąć poleceniem.** Potwierdzenie pomyłkowe trzeba naprawiać ręcznie, razem z następcą.

Akt jest usuwany z magazynu automatycznie 12 miesięcy po zamknięciu sprawy (D-055).
