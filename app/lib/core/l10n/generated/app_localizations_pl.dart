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
  String get legacyContactNone => 'Nie wskazano';

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
  String get newMemoryStepPlace => 'Miejsce i data';

  @override
  String get newMemoryStepContent => 'Treść';

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
}
