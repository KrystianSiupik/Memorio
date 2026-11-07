// ignore: unused_import
import 'package:intl/intl.dart' as intl;

import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for Polish (`pl`).
class AppLocalizationsPl extends AppLocalizations {
  AppLocalizationsPl([String locale = 'pl']) : super(locale);

  @override
  String get tabMap => 'Mapa';

  @override
  String get tabAlbums => 'Albumy';

  @override
  String get tabShared => 'Udostępnione';

  @override
  String get tabAccount => 'Konto';

  @override
  String get albumsTitle => 'Albumy';

  @override
  String get sharedTitle => 'Udostępnione mi';

  @override
  String get offlineSaved => 'Jesteś bez połączenia. Nowe wspomnienia i zdjęcia zapiszą się i wyślą, gdy wróci sieć.';

  @override
  String get noConnection => 'Brak połączenia';

  @override
  String get retry => 'Ponów';

  @override
  String get updateTitle => 'Zaktualizuj Memorio';

  @override
  String get updateBody => 'Ta wersja aplikacji nie jest już obsługiwana.';

  @override
  String updateBodyVersion(Object version) {
    return 'Ta wersja aplikacji nie jest już obsługiwana. Zainstaluj co najmniej wersję $version.';
  }

  @override
  String get signInTitle => 'Logowanie';

  @override
  String get registerTitle => 'Rejestracja';

  @override
  String get email => 'E-mail';

  @override
  String get password => 'Hasło';

  @override
  String get ageDeclaration => 'Mam ukończone 16 lat';

  @override
  String get acceptTerms => 'Akceptuję regulamin';

  @override
  String get signIn => 'Zaloguj się';

  @override
  String get createAccount => 'Załóż konto';

  @override
  String get noAccount => 'Nie mam konta';

  @override
  String get hasAccount => 'Mam już konto';

  @override
  String get authOffline => 'Brak połączenia. Sprawdź internet i spróbuj ponownie.';

  @override
  String get invalidCredentials => 'Nieprawidłowy e-mail lub hasło.';

  @override
  String get ageRequired => 'Konto może założyć osoba, która ma ukończone 16 lat.';

  @override
  String get emailTaken => 'Konto z tym adresem już istnieje. Zaloguj się.';

  @override
  String get invalidPassword => 'Hasło musi mieć co najmniej 10 znaków.';

  @override
  String get invalidEmail => 'To nie wygląda na poprawny adres e-mail.';

  @override
  String get invalidProviderToken => 'Logowanie nie powiodło się. Spróbuj ponownie.';

  @override
  String get providerUnavailable => 'Ten sposób logowania jest teraz niedostępny.';

  @override
  String get serverError => 'Błąd serwera. Spróbuj ponownie za chwilę.';

  @override
  String get accountTitle => 'Konto i prywatność';

  @override
  String get signOut => 'Wyloguj';

  @override
  String get deletionEffects => 'Skutki usunięcia';

  @override
  String effectMemories(Object count) {
    return 'Wspomnienia: $count';
  }

  @override
  String effectPhotos(Object count) {
    return 'Zdjęcia: $count';
  }

  @override
  String effectOwnAlbums(Object count, Object withoutSuccessor) {
    return 'Twoje albumy: $count, w tym bez następcy: $withoutSuccessor';
  }

  @override
  String effectOtherAlbums(Object count) {
    return 'Cudze albumy, w których uczestniczysz: $count';
  }

  @override
  String get legacyContact => 'Kontakt spadkowy';

  @override
  String get legacyContactNone => 'Nikogo nie wskazano.';

  @override
  String get legacyContactChoose => 'Wskaż kontakt spadkowy';

  @override
  String deletionDate(Object date) {
    return 'Konto zostanie usunięte $date.';
  }

  @override
  String get cancelDeletion => 'Cofnij usunięcie';

  @override
  String get understandDeletion => 'Rozumiem skutki usunięcia konta';

  @override
  String get albumsWithoutSuccessor =>
      'Masz albumy bez następcy. Wskaż następcę w albumie albo zgódź się na ich wygaśnięcie.';

  @override
  String get consentAlbumExpiry => 'Zgadzam się na wygaśnięcie albumów bez następcy';

  @override
  String get deletionWindowInfo => 'Konto zniknie po 30 dniach. Do tego czasu możesz się wycofać.';

  @override
  String get deleteAccount => 'Usuń konto';

  @override
  String get noAccess => 'Brak dostępu. Zaloguj się ponownie.';

  @override
  String get alreadyDecidedAccount => 'To już rozstrzygnięte: okno usunięcia konta się zakończyło.';

  @override
  String get albumWithoutSuccessorError =>
      'Masz albumy bez następcy. Wskaż następcę albo zgódź się na ich wygaśnięcie.';

  @override
  String get mapTitle => 'Mapa';

  @override
  String get mapLoadingPoints => 'Ładowanie punktów…';

  @override
  String get mapEmpty => 'Dodaj pierwsze wspomnienie';

  @override
  String get mapAddMemory => 'Dodaj wspomnienie';

  @override
  String get mapOpenTrash => 'Kosz';

  @override
  String get mapAttribution => '© OpenStreetMap';

  @override
  String get mapFilterDates => 'Daty';

  @override
  String mapFilterDatesRange(Object from, Object to) {
    return '$from – $to';
  }

  @override
  String get mapFilterPeople => 'Osoby';

  @override
  String get mapFilterAlbums => 'Albumy';

  @override
  String mapPendingUpload(Object count) {
    return 'Czeka na wysłanie: $count';
  }

  @override
  String get albumLink => 'Link do albumu';

  @override
  String get albumLinkNone => 'Album nie ma działającego linku';

  @override
  String get albumShareByLink => 'Udostępnij linkiem';

  @override
  String get albumRevokeLink => 'Unieważnij link (nikt nowy nie wejdzie)';

  @override
  String get albumCopyLink => 'Kopiuj link';

  @override
  String get albumLinkCopied => 'Skopiowano link';

  @override
  String get albumViewers => 'Oglądający przez link';

  @override
  String get albumViewersEmpty => 'Nikt jeszcze nie wszedł przez link';

  @override
  String get albumRevokeAccess => 'Odbierz dostęp';

  @override
  String get albumBlock => 'Zablokuj użytkownika';

  @override
  String get albumBlockNobody => 'Nie ma tu nikogo, kogo możesz zablokować.';

  @override
  String get albumBlockConfirmTitle => 'Zablokować tę osobę?';

  @override
  String albumBlockConfirmBody(Object person) {
    return '$person od razu straci dostęp i nie wejdzie już ani linkiem, ani przez zaproszenie. Odblokować może później właściciel albumu.';
  }

  @override
  String get albumBlockConfirm => 'Zablokuj';

  @override
  String get cancel => 'Anuluj';

  @override
  String get albumBlockedPeople => 'Zablokowani';

  @override
  String get albumUnblock => 'Odblokuj';

  @override
  String get albumBlocked => 'Nie masz dostępu do tego albumu.';

  @override
  String get albumLinkGone => 'Ten link już nie działa.';

  @override
  String get albumOnlyOwnerBlocks => 'Osobę w albumie może zablokować tylko właściciel.';

  @override
  String get albumEntryTitle => 'Wejście do albumu przez link';

  @override
  String albumEntryAlbum(Object name) {
    return 'Album: $name';
  }

  @override
  String get albumEntryOpen => 'Otwórz album';

  @override
  String get albumEntryNamed => 'Twój adres e-mail pojawi się na liście oglądających ten album.';

  @override
  String get albumReadOnlyViewer => 'Oglądasz ten album przez link.';

  @override
  String get sharedEmpty => 'Nikt jeszcze nie udostępnił Ci albumu';

  @override
  String get albumMine => 'Moje albumy';

  @override
  String get albumShared => 'Udostępnione mi';

  @override
  String get albumInvitations => 'Zaproszenia';

  @override
  String get albumNewName => 'Nazwa nowego albumu';

  @override
  String get albumCreate => 'Utwórz album';

  @override
  String get albumsEmpty => 'Nie masz jeszcze albumów';

  @override
  String get albumSharedEmpty => 'Nikt nie udostępnił Ci albumu';

  @override
  String albumMembersCount(Object members, Object memories) {
    return 'Osoby: $members, wspomnienia: $memories';
  }

  @override
  String get albumNameRequired => 'Podaj nazwę albumu.';

  @override
  String get albumNoAccess => 'Nie masz już dostępu do tego albumu.';

  @override
  String get albumNoFreeSeats => 'W planie tego albumu nie ma wolnego miejsca.';

  @override
  String get albumInvitationLimit => 'Dzisiejszy limit 10 zaproszeń został wykorzystany. Spróbuj jutro.';

  @override
  String get albumAddressBlocked => 'Ten adres nie przyjmuje zaproszeń.';

  @override
  String get albumInvitationGone => 'Zaproszenie wygasło albo zostało wycofane.';

  @override
  String get albumPoolFull => 'Pula albumu jest pełna. Właściciel dostał powiadomienie.';

  @override
  String get albumOwnerCannotLeave => 'Właściciel nie może opuścić albumu.';

  @override
  String get albumMemories => 'Wspomnienia';

  @override
  String get albumEmpty => 'Album jest pusty';

  @override
  String get albumMembers => 'Skład';

  @override
  String get albumOwnerRole => 'właściciel';

  @override
  String get albumParticipantRole => 'uczestnik';

  @override
  String albumMyContributions(Object memories, Object photos) {
    return 'Moje wkłady: wspomnienia $memories, zdjęcia $photos';
  }

  @override
  String albumPoolLeft(Object left, Object total) {
    return 'Zostało w puli: $left z $total';
  }

  @override
  String get albumNoOwnerState => 'Ten album ma właściciela.';

  @override
  String get albumAddMemory => 'Dodaj moje wspomnienie';

  @override
  String get albumNoMemoriesToAdd => 'Nie masz jeszcze wspomnień do dodania.';

  @override
  String get albumAddPhotos => 'Dodaj zdjęcia';

  @override
  String get albumTag => 'Oznacz uczestnika';

  @override
  String get albumRemoveMyTag => 'Usuń moje oznaczenie';

  @override
  String get albumLeave => 'Opuść album';

  @override
  String get albumLeaveKeep => 'Zostaw moje wkłady w albumie';

  @override
  String get albumLeaveTake => 'Zabierz moje wkłady';

  @override
  String get albumLeaveConfirm => 'Potwierdzam wyjście z albumu';

  @override
  String get albumManage => 'Zarządzanie albumem';

  @override
  String get albumConsents => 'Moje zgody i powierzenia';

  @override
  String albumSeats(Object free, Object invited, Object taken) {
    return 'Miejsca: zajęte $taken, zaproszone $invited, wolne $free';
  }

  @override
  String get albumInviteEmail => 'E-mail zapraszanego';

  @override
  String get albumInvite => 'Zaproś';

  @override
  String get albumInviteInvalid => 'To nie wygląda na adres e-mail.';

  @override
  String get albumInvited => 'Zaproszeni';

  @override
  String get albumWithdrawInvitation => 'Wycofaj';

  @override
  String get albumRemoveParticipant => 'Usuń uczestnika';

  @override
  String get albumSuccessor => 'Następca';

  @override
  String mapPreviewPhotos(Object count) {
    return 'Zdjęcia: $count';
  }

  @override
  String get mapAddMemoryHere => 'Dodaj wspomnienie tutaj';

  @override
  String get ownPlaceFromMap =>
      'Współrzędne z mapy są już wpisane: nazwij to miejsce (np. Warszawa) i dodaj je. Możesz też wyszukać miejsce po nazwie powyżej.';

  @override
  String get mapPreviewUnavailable => 'Nie udało się wczytać podglądu.';

  @override
  String get reportContent => 'Zgłoś treść';

  @override
  String get reportMemoryTitle => 'Zgłoś wspomnienie';

  @override
  String get reportAlbumTitle => 'Zgłoś album';

  @override
  String get reportExplained =>
      'Zgłoszenie trafi do obsługi Memorio. Autor nie dowie się, kto zgłosił. Treści niezgodne z prawem ukrywamy od razu i rozpatrujemy w ciągu 24 godzin, pozostałe zostają widoczne do decyzji, którą podejmujemy w ciągu 7 dni.';

  @override
  String get reportCategory => 'Co jest nie tak?';

  @override
  String get reasonChildAbuse => 'Wykorzystywanie dzieci';

  @override
  String get reasonTerrorism => 'Treści terrorystyczne';

  @override
  String get reasonThreat => 'Groźby';

  @override
  String get reasonIncitement => 'Nawoływanie do przemocy lub nienawiści';

  @override
  String get reasonOtherIllegal => 'Inna treść niezgodna z prawem';

  @override
  String get reasonSomeonesImage => 'Mój wizerunek bez mojej zgody';

  @override
  String get reasonPersonalData => 'Moje dane osobowe';

  @override
  String get reasonDefamation => 'Zniesławienie';

  @override
  String get reasonCopyright => 'Naruszenie praw autorskich';

  @override
  String get reasonOther => 'Inny powód';

  @override
  String get reportIllegalHidden => 'Ta treść zostanie ukryta od razu po wysłaniu zgłoszenia, do decyzji obsługi.';

  @override
  String get reportExplanation => 'Uzasadnienie';

  @override
  String get reportSend => 'Zgłoś';

  @override
  String get reportSent => 'Dziękujemy. Decyzję z uzasadnieniem zobaczysz na ekranie „Moje zgłoszenia i decyzje”.';

  @override
  String get reportSentIllegal =>
      'Dziękujemy. Treść jest już ukryta. Decyzję z uzasadnieniem zobaczysz na ekranie „Moje zgłoszenia i decyzje”.';

  @override
  String get reportBack => 'Wróć';

  @override
  String get reportNoAccess => 'Tej treści już nie widzisz.';

  @override
  String get reportOwnContent => 'Własną treść możesz zmienić albo usunąć samodzielnie.';

  @override
  String get myModerationTitle => 'Moje zgłoszenia i decyzje';

  @override
  String get myModerationEmpty => 'Nie masz zgłoszeń ani decyzji';

  @override
  String get myModerationAboutMine => 'Decyzje o moich treściach';

  @override
  String get myModerationMine => 'Moje zgłoszenia';

  @override
  String myModerationReported(Object date, Object reason) {
    return '$reason, zgłoszone $date';
  }

  @override
  String get myModerationHidden => 'Teraz widzisz to tylko Ty.';

  @override
  String myModerationStatement(Object statement) {
    return 'Uzasadnienie: $statement';
  }

  @override
  String get myModerationAppealPending => 'Odwołanie czeka na decyzję.';

  @override
  String get myModerationAppealReversed => 'Odwołanie uwzględnione: treść jest znów widoczna.';

  @override
  String get myModerationAppealUpheld => 'Odwołanie nieuwzględnione.';

  @override
  String get myModerationReporterAppealReversed => 'Odwołanie uwzględnione: treść została ukryta.';

  @override
  String get reportSuspended =>
      'Nie przyjmujemy teraz Twoich zgłoszeń: trzy w ciągu 90 dni uznaliśmy za oczywiście bezzasadne.';

  @override
  String get myModerationAppealReason => 'Dlaczego decyzja jest błędna';

  @override
  String myModerationAppealUntil(Object date) {
    return 'Możesz się odwołać do $date.';
  }

  @override
  String get myModerationAppeal => 'Odwołaj się';

  @override
  String get verdictRemoved => 'Decyzja: treść ukryta';

  @override
  String get verdictKept => 'Decyzja: treść zostaje';

  @override
  String get verdictPending => 'Czeka na decyzję';

  @override
  String get albumHandover => 'Przekazanie albumu';

  @override
  String get albumHandoverExplain =>
      'Możesz już teraz przekazać album komuś z uczestników. Ta osoba zostanie właścicielem, a Ty zostaniesz w albumie jako uczestnik.';

  @override
  String get albumHandOver => 'Przekaż album';

  @override
  String albumHandoverTo(Object person) {
    return 'Przekazać album: $person?';
  }

  @override
  String get albumHandoverQuestion =>
      'Co z Twoimi własnymi wspomnieniami w tym albumie? Zdecyduj teraz: zostaną w albumie albo wyjmiemy je z niego, gdy ta osoba przyjmie album. U Ciebie zostaną w obu przypadkach.';

  @override
  String get albumHandoverKeep => 'Zostają w albumie';

  @override
  String get albumHandoverTake => 'Zabieram je';

  @override
  String albumHandoverWaiting(Object date, Object person) {
    return 'Czeka na odpowiedź: $person, do $date.';
  }

  @override
  String get albumHandoverWithdraw => 'Wycofaj propozycję';

  @override
  String albumHandoverForMe(Object date) {
    return 'Właściciel chce Ci przekazać ten album. Możesz odpowiedzieć do $date.';
  }

  @override
  String get albumHandoverOwnerKeeps => 'Jego wspomnienia zostaną w albumie.';

  @override
  String get albumHandoverOwnerTakes => 'Jego wspomnienia zostaną z albumu wyjęte.';

  @override
  String get albumHandoverAccept => 'Przejmuję album';

  @override
  String get albumHandoverRefuse => 'Odmawiam';

  @override
  String get albumNoSuccessor => 'Nie wskazano następcy';

  @override
  String get albumNameSuccessor => 'Wskaż następcę';

  @override
  String get albumCurator => 'Kurator';

  @override
  String get albumNoCurator => 'Nie powierzono treści kuratorowi';

  @override
  String get albumEntrust => 'Powierz kuratorowi';

  @override
  String get albumRevokeEntrustment => 'Odwołaj powierzenie';

  @override
  String get albumKept => 'Moje wkłady pozostawione po wyjściu';

  @override
  String get albumKeptEmpty => 'Nie masz wkładów pozostawionych po wyjściu';

  @override
  String albumKeptItem(Object album, Object memories) {
    return '$album: wspomnienia $memories';
  }

  @override
  String get albumKeptNotChosen => 'Usunięto Cię z albumu. Wybierz, czy treści mają zostać.';

  @override
  String get albumKeptConfirm => 'Wiem, że treści znikną z albumu';

  @override
  String get albumRevokeKept => 'Odwołaj zgodę „zostaw”';

  @override
  String get albumEntrusted => 'Powierzone treści';

  @override
  String get albumEntrustedEmpty => 'Nikt nie powierzył Ci treści';

  @override
  String get albumArrange => 'Uporządkuj';

  @override
  String get albumMoveUp => 'Wyżej';

  @override
  String get albumMoveDown => 'Niżej';

  @override
  String get albumInvitationTitle => 'Zaproszenie do albumu';

  @override
  String albumInvitedBy(Object person) {
    return 'Kto zaprasza: $person';
  }

  @override
  String albumInvitationAlbum(Object name) {
    return 'Album: $name';
  }

  @override
  String get albumAccept => 'Przyjmij';

  @override
  String get albumDecline => 'Odrzuć';

  @override
  String get albumBlockInvitations => 'Nie chcę zaproszeń z GEO';

  @override
  String get mapEmptyFiltered => 'Brak wspomnień w tych datach';

  @override
  String mapRejected(Object count) {
    return 'Nie udało się wysłać: $count';
  }

  @override
  String get mapRejectedDismiss => 'Rozumiem';

  @override
  String get newMemoryTitle => 'Nowe wspomnienie';

  @override
  String get newMemoryStepPlace => 'Miejsce';

  @override
  String get newMemoryStepContent => 'Historia';

  @override
  String get next => 'Dalej';

  @override
  String get back => 'Wstecz';

  @override
  String get place => 'Miejsce';

  @override
  String placeChosen(Object name) {
    return 'Wybrano: $name';
  }

  @override
  String get placeChange => 'Zmień miejsce';

  @override
  String get placeRecent => 'Ostatnio użyte';

  @override
  String get ownPlaceName => 'Nazwa własna miejsca';

  @override
  String get ownPlaceLatitude => 'Szerokość geograficzna';

  @override
  String get ownPlaceLongitude => 'Długość geograficzna';

  @override
  String get ownPlaceAdd => 'Dodaj miejsce własne';

  @override
  String get ownPlaceInvalid => 'Podaj nazwę oraz szerokość od -90 do 90 i długość od -180 do 180.';

  @override
  String get datePrecision => 'Data';

  @override
  String get precisionDay => 'Dzień';

  @override
  String get precisionMonth => 'Miesiąc';

  @override
  String get precisionYear => 'Rok';

  @override
  String get precisionPeriod => 'Okres';

  @override
  String get dateChoose => 'Wybierz datę';

  @override
  String get dateChooseEnd => 'Wybierz koniec okresu';

  @override
  String dateChosen(Object date) {
    return 'Od: $date';
  }

  @override
  String dateChosenEnd(Object date) {
    return 'Do: $date';
  }

  @override
  String get timeChoose => 'Dodaj godzinę (nieobowiązkowo)';

  @override
  String timeChosen(Object time) {
    return 'Godzina: $time';
  }

  @override
  String get timeClear => 'Bez godziny';

  @override
  String get dateInFuture => 'Wspomnienie nie może być z przyszłości.';

  @override
  String get photos => 'Zdjęcia';

  @override
  String get photosAdd => 'Dodaj zdjęcia';

  @override
  String photosPicked(Object count) {
    return 'Wybrane zdjęcia: $count';
  }

  @override
  String get description => 'Opis';

  @override
  String get aiDraft => 'Szkic AI';

  @override
  String get aiConsent => 'Zgadzam się na wysłanie opisu do dostawcy AI, bez nazwy miejsca';

  @override
  String get aiRequest => 'Poproś AI o szkic';

  @override
  String get aiMarker => 'Tekst AI';

  @override
  String get aiUnavailableOffline => 'Szkic AI nie działa bez połączenia.';

  @override
  String get aiUseDraft => 'Użyj szkicu';

  @override
  String get approveDescription => 'Zatwierdź opis';

  @override
  String get descriptionApproved => 'Opis zatwierdzony';

  @override
  String get saveMemory => 'Zapisz wspomnienie';

  @override
  String get savedOffline => 'Zapisano. Wyślemy, gdy wróci sieć.';

  @override
  String get offlineNotOnWeb => 'W przeglądarce nie można zapisywać bez połączenia.';

  @override
  String get termsRequired => 'Przed pierwszym wspomnieniem zaakceptuj regulamin.';

  @override
  String get memoryNoAccess => 'Nie masz dostępu do tego wspomnienia.';

  @override
  String get alreadyDecided => 'To już rozstrzygnięte. Odśwież i sprawdź aktualny stan.';

  @override
  String get placeMissing => 'Tego miejsca już nie ma. Wybierz inne.';

  @override
  String get spaceFull => 'Pula miejsca jest pełna. To zdjęcie się nie zmieści.';

  @override
  String get aiLimitReached => 'Limit szkiców AI w tym miesiącu jest wyczerpany.';

  @override
  String get aiProviderDown => 'Dostawca AI jest teraz niedostępny. Spróbuj później.';

  @override
  String get invalidInput => 'Sprawdź wpisane dane.';

  @override
  String get memoryTitle => 'Wspomnienie';

  @override
  String memoryWhen(Object date) {
    return 'Data: $date';
  }

  @override
  String memoryPlace(Object name) {
    return 'Miejsce: $name';
  }

  @override
  String get memoryContents => 'Treści';

  @override
  String get memoryAuthorMe => 'Autor: Ty';

  @override
  String get memoryPeople => 'Osoby';

  @override
  String get memoryPeopleNone => 'Nikt nie jest oznaczony';

  @override
  String get memoryNoDescription => 'Bez opisu';

  @override
  String get memoryGone => 'Nie masz już dostępu do tego wspomnienia';

  @override
  String get memoryBack => 'Wróć';

  @override
  String get deleteMine => 'Usuń moją treść';

  @override
  String get deleteForGoodConfirm => 'Rozumiem, że tego nie da się cofnąć';

  @override
  String get deleteForGood => 'Usuń trwale (formalne żądanie)';

  @override
  String get trashTitle => 'Kosz';

  @override
  String get trashEmpty => 'Kosz jest pusty';

  @override
  String trashItem(Object date, Object place) {
    return '$place, $date';
  }

  @override
  String trashDeletedAt(Object date) {
    return 'Usunięto: $date';
  }

  @override
  String trashDaysLeft(Object count) {
    return 'Zostało dni: $count';
  }

  @override
  String trashPhotos(Object count) {
    return 'Zdjęcia: $count';
  }

  @override
  String get restore => 'Przywróć';

  @override
  String get replayTitle => 'Prześledź';

  @override
  String replayPeriod(String period) {
    return 'Okres: $period';
  }

  @override
  String get replayOtherRange => 'Inny okres';

  @override
  String get replayEmpty => 'W tym okresie nie ma wspomnień';

  @override
  String replayStep(int current, int total) {
    return '$current z $total';
  }

  @override
  String get replayPrevious => 'Poprzednie';

  @override
  String get replayNext => 'Następne';

  @override
  String get replayPlay => 'Odtwarzaj';

  @override
  String get replayPause => 'Zatrzymaj';

  @override
  String replayTempo(int speed) {
    return 'Tempo $speed×';
  }

  @override
  String get replayEnd => 'To już wszystko z tego okresu';

  @override
  String replayStats(int memories, int places) {
    return 'Wspomnienia: $memories, miejsca: $places';
  }

  @override
  String get replayAgain => 'Od początku';

  @override
  String get replayCreateAlbum => 'Utwórz album z okresu';

  @override
  String get replayCreateAlbumExplained =>
      'Do albumu trafią Twoje wspomnienia z tego okresu. Cudze zostają w swoich albumach.';

  @override
  String replayAlbumName(String period) {
    return 'Prześledź $period';
  }

  @override
  String replayAlbumCreated(int count) {
    return 'Album gotowy, dodano wspomnień: $count.';
  }

  @override
  String replayAlbumNotFitting(int count) {
    return 'Nie zmieściło się w puli albumu: $count.';
  }

  @override
  String get replayOpenAlbum => 'Otwórz album';

  @override
  String get replayTooLong => 'W tym okresie jest za dużo wspomnień na jeden album. Wybierz krótszy.';

  @override
  String get takeoverTitle => 'Przejęcie albumu';

  @override
  String takeoverDeadline(String date) {
    return 'Na decyzję masz czas do: $date';
  }

  @override
  String takeoverFounder(int count) {
    return 'Wspomnienia fundatora w albumie: $count. Zostają w nim.';
  }

  @override
  String get takeoverExplained =>
      'Jeśli przejmiesz album, zostaniesz jego właścicielem, razem z jego pulą. Jeśli odmówisz albo nie odpowiesz w terminie, album zostanie bez właściciela, a przejąć go będzie mógł inny uczestnik.';

  @override
  String get takeoverStoreAdvice =>
      'Rodzina zmarłej osoby powinna anulować jej subskrypcję w sklepie: aplikacja nie może tego zrobić. Album działa do końca opłaconego okresu.';

  @override
  String get takeoverAccept => 'Przejmuję album';

  @override
  String get takeoverRefuse => 'Odmawiam';

  @override
  String get takeoverNotSuccessor => 'Nie jesteś wskazanym następcą.';

  @override
  String get takeoverDecided => 'Sprawa już rozstrzygnięta.';

  @override
  String takeoverOfferTile(String album) {
    return 'Wskazano Cię jako następcę: $album';
  }

  @override
  String get takeoverNoOwner =>
      'Ten album nie ma właściciela. Nic z niego nie znika, a każdy uczestnik może go przejąć, opłacając plan.';

  @override
  String get takeoverReserve => 'Przejmuję';

  @override
  String takeoverReservedByMe(String time) {
    return 'Album czeka na Ciebie do $time. Opłać plan albumu, aby go przejąć.';
  }

  @override
  String get takeoverReservedByOther => 'Ktoś właśnie przejmuje ten album.';

  @override
  String get albumReportDeath => 'Zgłoś zgon właściciela';

  @override
  String get deathReportTitle => 'Zgłoś zgon właściciela';

  @override
  String get deathReportExplained =>
      'Dołącz zdjęcie aktu zgonu. Sprawdzi je obsługa GEO. Dokument widzi tylko obsługa, a po zamknięciu sprawy usuwamy go po 12 miesiącach.';

  @override
  String get deathReportPick => 'Wybierz zdjęcie aktu zgonu';

  @override
  String get deathReportSend => 'Wyślij zgłoszenie';

  @override
  String get deathReportSent =>
      'Zgłoszenie wysłane. Obsługa sprawdzi dokument i da znać, jeśli nie będzie mogła go potwierdzić.';

  @override
  String get deathReportBack => 'Wróć do albumu';

  @override
  String get deathReportTaken => 'Zgłoszenie dotyczące tego albumu jest już sprawdzane.';

  @override
  String get legacyContactTitle => 'Kontakt spadkowy';

  @override
  String get legacyContactExplained =>
      'Ta osoba po Twojej śmierci będzie mogła oglądać Twoje prywatne wspomnienia, tylko do odczytu. Nie dostanie reszty konta ani albumów. Bez wskazania nikt ich nie zobaczy, także spadkobiercy.';

  @override
  String legacyContactCurrent(String email) {
    return 'Wskazano: $email';
  }

  @override
  String get legacyContactEmail => 'Adres e-mail tej osoby';

  @override
  String get legacyContactSave => 'Zapisz';

  @override
  String get legacyContactRemove => 'Odwołaj wskazanie';

  @override
  String get legacyContactInvalid => 'Podaj poprawny adres innej osoby.';

  @override
  String get legacyArchiveTitle => 'Prywatne wspomnienia';

  @override
  String get legacyArchiveSubtitle => 'Wskazano Cię jako kontakt spadkowy';

  @override
  String get legacyArchiveUnknown => 'Konto bez adresu';

  @override
  String get legacyArchiveEmpty => 'Nie ma tu żadnych wspomnień.';

  @override
  String get planTitle => 'Plan i subskrypcja';

  @override
  String get planAlbumTitle => 'Plan albumu';

  @override
  String get planModeFree => 'Plan darmowy';

  @override
  String get planModeActive => 'Plan Premium';

  @override
  String get planModeReadOnly => 'Tylko do odczytu';

  @override
  String get planModeOverLimit => 'Nic nowego ponad darmowy limit';

  @override
  String get planModeTrimmed => 'Plan darmowy po przycięciu';

  @override
  String planUsage(String used, String limit) {
    return 'Zajęte miejsce: $used z $limit';
  }

  @override
  String planAiUsage(int used, int limit) {
    return 'AI w tym miesiącu: $used z $limit';
  }

  @override
  String planPaidUntil(String date) {
    return 'Opłacony do: $date';
  }

  @override
  String planReadOnlySince(String date) {
    return 'Plan wygasł: $date';
  }

  @override
  String planTrimAt(String date, String limit) {
    return '$date: to, co przekracza $limit, trafi do kosza na 30 dni.';
  }

  @override
  String planAlbumTrimAt(String date, String limit) {
    return '$date: to, co przekracza $limit, zostanie wyjęte z albumu. Autorzy zachowają swoje wspomnienia na własnych kontach.';
  }

  @override
  String get planHeld =>
      'Przycięcie wstrzymane: ostrzeżenie do kogoś nie dotarło. Nic nie zostanie przycięte, dopóki nie dotrze.';

  @override
  String get planNothingLost =>
      'Nic nie zniknie do tej daty. Możesz wszystko oglądać, a po opłaceniu planu wszystko wróci do pełnych funkcji bez żadnego działania.';

  @override
  String get planChoose => 'Wybierz, co zostanie';

  @override
  String get planRefresh => 'Przywróć zakupy / odśwież';

  @override
  String get planNoAccess => 'Ten plan widzi tylko osoba, która za niego płaci.';

  @override
  String get planChoiceClosed => 'Wybór materiału nie jest teraz otwarty.';

  @override
  String get planMaterialTitle => 'Wybierz, co zostanie po przycięciu';

  @override
  String planMaterialLimit(String used, String limit) {
    return 'Wybrano $used z darmowego limitu $limit';
  }

  @override
  String planMaterialDeadline(String date) {
    return 'Wybór możesz zmieniać do: $date. Bez wyboru zostaje to, co najstarsze.';
  }

  @override
  String get planMaterialTooMuch => 'To za dużo: odznacz coś, aby zmieścić się w limicie.';

  @override
  String get planMaterialKeep => 'Zachowaj wybrane';

  @override
  String get planMaterialSaved => 'Wybór zapisany.';

  @override
  String get albumReadOnly =>
      'Plan albumu wygasł: album jest tylko do odczytu. Wszyscy dalej go oglądają, ale nic nowego nie można dodać.';

  @override
  String get albumReadOnlyRefused =>
      'Plan tego albumu wygasł: nic nowego nie można dodać, dopóki nie zostanie opłacony.';

  @override
  String get settingsTitle => 'Ustawienia';

  @override
  String get settingsLanguage => 'Język';

  @override
  String get settingsLanguageSystem => 'Jak w urządzeniu';

  @override
  String get settingsTheme => 'Wygląd';

  @override
  String get settingsThemeSystem => 'Jak w urządzeniu';

  @override
  String get settingsThemeLight => 'Jasny';

  @override
  String get settingsThemeDark => 'Ciemny';

  @override
  String get calendarClear => 'Wyczyść';

  @override
  String get calendarApply => 'Zastosuj';

  @override
  String get settingsPoints => 'Punkty na mapie';

  @override
  String get settingsPhotoPoints => 'Zdjęcia zamiast kropek';

  @override
  String get settingsPhotoPointsExplained =>
      'Wspomnienie ze zdjęciem pokaże się na mapie jako mały okrąg z pierwszym zdjęciem. Bez zdjęcia zostaje kropką.';

  @override
  String get settingsPointSize => 'Wielkość punktów';

  @override
  String get placeNeeded => 'Wybierz miejsce z listy albo wpisz nazwę i współrzędne miejsca własnego.';

  @override
  String get dateNeeded => 'Wybierz datę.';

  @override
  String get placeSearchHint => 'Szukaj, np. Biskupia Kopa';

  @override
  String get ownPlaceSection => 'Albo własne miejsce';

  @override
  String get descriptionSection => 'Twoja historia';

  @override
  String get photosEmpty => 'Dodaj zdjęcia. Pierwsze pokaże się na mapie.';

  @override
  String get photoPrevious => 'Poprzednie zdjęcie';

  @override
  String get photoNext => 'Następne zdjęcie';

  @override
  String get photoRemove => 'Usuń zdjęcie';

  @override
  String get newMemoryStepDate => 'Data';

  @override
  String get newMemoryStepPhotos => 'Zdjęcia';

  @override
  String get placeQuestion => 'Gdzie to było?';

  @override
  String get placeQuestionHint => 'Wyszukaj miejsce albo nazwij własne.';

  @override
  String get dateQuestion => 'Kiedy to było?';

  @override
  String get dateQuestionHint => 'Nie pamiętasz dnia? Wystarczy miesiąc, rok albo okres.';

  @override
  String get photosQuestion => 'Pokaż, jak tam było';

  @override
  String get photosQuestionHint => 'Pierwsze zdjęcie pokaże się na mapie. Ten krok możesz pominąć.';

  @override
  String get storyQuestion => 'Opowiedz, co się wydarzyło';

  @override
  String get storyQuestionHint => 'Napisz kilka zdań albo poproś AI o szkic.';
}
