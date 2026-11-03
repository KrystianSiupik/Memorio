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
}
