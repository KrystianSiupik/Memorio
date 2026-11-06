import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:intl/intl.dart' as intl;

import 'app_localizations_pl.dart';

// ignore_for_file: type=lint

/// Callers can lookup localized strings with an instance of AppLocalizations
/// returned by `AppLocalizations.of(context)`.
///
/// Applications need to include `AppLocalizations.delegate()` in their app's
/// `localizationDelegates` list, and the locales they support in the app's
/// `supportedLocales` list. For example:
///
/// ```dart
/// import 'generated/app_localizations.dart';
///
/// return MaterialApp(
///   localizationsDelegates: AppLocalizations.localizationsDelegates,
///   supportedLocales: AppLocalizations.supportedLocales,
///   home: MyApplicationHome(),
/// );
/// ```
///
/// ## Update pubspec.yaml
///
/// Please make sure to update your pubspec.yaml to include the following
/// packages:
///
/// ```yaml
/// dependencies:
///   # Internationalization support.
///   flutter_localizations:
///     sdk: flutter
///   intl: any # Use the pinned version from flutter_localizations
///
///   # Rest of dependencies
/// ```
///
/// ## iOS Applications
///
/// iOS applications define key application metadata, including supported
/// locales, in an Info.plist file that is built into the application bundle.
/// To configure the locales supported by your app, you’ll need to edit this
/// file.
///
/// First, open your project’s ios/Runner.xcworkspace Xcode workspace file.
/// Then, in the Project Navigator, open the Info.plist file under the Runner
/// project’s Runner folder.
///
/// Next, select the Information Property List item, select Add Item from the
/// Editor menu, then select Localizations from the pop-up menu.
///
/// Select and expand the newly-created Localizations item then, for each
/// locale your application supports, add a new item and select the locale
/// you wish to add from the pop-up menu in the Value field. This list should
/// be consistent with the languages listed in the AppLocalizations.supportedLocales
/// property.
abstract class AppLocalizations {
  AppLocalizations(String locale) : localeName = intl.Intl.canonicalizedLocale(locale.toString());

  final String localeName;

  static AppLocalizations of(BuildContext context) {
    return Localizations.of<AppLocalizations>(context, AppLocalizations)!;
  }

  static const LocalizationsDelegate<AppLocalizations> delegate = _AppLocalizationsDelegate();

  /// A list of this localizations delegate along with the default localizations
  /// delegates.
  ///
  /// Returns a list of localizations delegates containing this delegate along with
  /// GlobalMaterialLocalizations.delegate, GlobalCupertinoLocalizations.delegate,
  /// and GlobalWidgetsLocalizations.delegate.
  ///
  /// Additional delegates can be added by appending to this list in
  /// MaterialApp. This list does not have to be used at all if a custom list
  /// of delegates is preferred or required.
  static const List<LocalizationsDelegate<dynamic>> localizationsDelegates = <LocalizationsDelegate<dynamic>>[
    delegate,
    GlobalMaterialLocalizations.delegate,
    GlobalCupertinoLocalizations.delegate,
    GlobalWidgetsLocalizations.delegate,
  ];

  /// A list of this localizations delegate's supported locales.
  static const List<Locale> supportedLocales = <Locale>[Locale('pl')];

  /// No description provided for @tabMap.
  ///
  /// In pl, this message translates to:
  /// **'Mapa'**
  String get tabMap;

  /// No description provided for @tabAlbums.
  ///
  /// In pl, this message translates to:
  /// **'Albumy'**
  String get tabAlbums;

  /// No description provided for @tabShared.
  ///
  /// In pl, this message translates to:
  /// **'Udostępnione'**
  String get tabShared;

  /// No description provided for @tabAccount.
  ///
  /// In pl, this message translates to:
  /// **'Konto'**
  String get tabAccount;

  /// No description provided for @albumsTitle.
  ///
  /// In pl, this message translates to:
  /// **'Albumy'**
  String get albumsTitle;

  /// No description provided for @sharedTitle.
  ///
  /// In pl, this message translates to:
  /// **'Udostępnione mi'**
  String get sharedTitle;

  /// No description provided for @offlineSaved.
  ///
  /// In pl, this message translates to:
  /// **'Jesteś bez połączenia. Nowe wspomnienia i zdjęcia zapiszą się i wyślą, gdy wróci sieć.'**
  String get offlineSaved;

  /// No description provided for @noConnection.
  ///
  /// In pl, this message translates to:
  /// **'Brak połączenia'**
  String get noConnection;

  /// No description provided for @retry.
  ///
  /// In pl, this message translates to:
  /// **'Ponów'**
  String get retry;

  /// No description provided for @updateTitle.
  ///
  /// In pl, this message translates to:
  /// **'Zaktualizuj Memorio'**
  String get updateTitle;

  /// No description provided for @updateBody.
  ///
  /// In pl, this message translates to:
  /// **'Ta wersja aplikacji nie jest już obsługiwana.'**
  String get updateBody;

  /// No description provided for @updateBodyVersion.
  ///
  /// In pl, this message translates to:
  /// **'Ta wersja aplikacji nie jest już obsługiwana. Zainstaluj co najmniej wersję {version}.'**
  String updateBodyVersion(Object version);

  /// No description provided for @signInTitle.
  ///
  /// In pl, this message translates to:
  /// **'Logowanie'**
  String get signInTitle;

  /// No description provided for @registerTitle.
  ///
  /// In pl, this message translates to:
  /// **'Rejestracja'**
  String get registerTitle;

  /// No description provided for @email.
  ///
  /// In pl, this message translates to:
  /// **'E-mail'**
  String get email;

  /// No description provided for @password.
  ///
  /// In pl, this message translates to:
  /// **'Hasło'**
  String get password;

  /// No description provided for @ageDeclaration.
  ///
  /// In pl, this message translates to:
  /// **'Mam ukończone 16 lat'**
  String get ageDeclaration;

  /// No description provided for @acceptTerms.
  ///
  /// In pl, this message translates to:
  /// **'Akceptuję regulamin'**
  String get acceptTerms;

  /// No description provided for @signIn.
  ///
  /// In pl, this message translates to:
  /// **'Zaloguj się'**
  String get signIn;

  /// No description provided for @createAccount.
  ///
  /// In pl, this message translates to:
  /// **'Załóż konto'**
  String get createAccount;

  /// No description provided for @noAccount.
  ///
  /// In pl, this message translates to:
  /// **'Nie mam konta'**
  String get noAccount;

  /// No description provided for @hasAccount.
  ///
  /// In pl, this message translates to:
  /// **'Mam już konto'**
  String get hasAccount;

  /// No description provided for @authOffline.
  ///
  /// In pl, this message translates to:
  /// **'Brak połączenia. Sprawdź internet i spróbuj ponownie.'**
  String get authOffline;

  /// No description provided for @invalidCredentials.
  ///
  /// In pl, this message translates to:
  /// **'Nieprawidłowy e-mail lub hasło.'**
  String get invalidCredentials;

  /// No description provided for @ageRequired.
  ///
  /// In pl, this message translates to:
  /// **'Konto może założyć osoba, która ma ukończone 16 lat.'**
  String get ageRequired;

  /// No description provided for @emailTaken.
  ///
  /// In pl, this message translates to:
  /// **'Konto z tym adresem już istnieje. Zaloguj się.'**
  String get emailTaken;

  /// No description provided for @invalidPassword.
  ///
  /// In pl, this message translates to:
  /// **'Hasło musi mieć co najmniej 10 znaków.'**
  String get invalidPassword;

  /// No description provided for @invalidEmail.
  ///
  /// In pl, this message translates to:
  /// **'To nie wygląda na poprawny adres e-mail.'**
  String get invalidEmail;

  /// No description provided for @invalidProviderToken.
  ///
  /// In pl, this message translates to:
  /// **'Logowanie nie powiodło się. Spróbuj ponownie.'**
  String get invalidProviderToken;

  /// No description provided for @providerUnavailable.
  ///
  /// In pl, this message translates to:
  /// **'Ten sposób logowania jest teraz niedostępny.'**
  String get providerUnavailable;

  /// No description provided for @serverError.
  ///
  /// In pl, this message translates to:
  /// **'Błąd serwera. Spróbuj ponownie za chwilę.'**
  String get serverError;

  /// No description provided for @accountTitle.
  ///
  /// In pl, this message translates to:
  /// **'Konto i prywatność'**
  String get accountTitle;

  /// No description provided for @signOut.
  ///
  /// In pl, this message translates to:
  /// **'Wyloguj'**
  String get signOut;

  /// No description provided for @deletionEffects.
  ///
  /// In pl, this message translates to:
  /// **'Skutki usunięcia'**
  String get deletionEffects;

  /// No description provided for @effectMemories.
  ///
  /// In pl, this message translates to:
  /// **'Wspomnienia: {count}'**
  String effectMemories(Object count);

  /// No description provided for @effectPhotos.
  ///
  /// In pl, this message translates to:
  /// **'Zdjęcia: {count}'**
  String effectPhotos(Object count);

  /// No description provided for @effectOwnAlbums.
  ///
  /// In pl, this message translates to:
  /// **'Twoje albumy: {count}, w tym bez następcy: {withoutSuccessor}'**
  String effectOwnAlbums(Object count, Object withoutSuccessor);

  /// No description provided for @effectOtherAlbums.
  ///
  /// In pl, this message translates to:
  /// **'Cudze albumy, w których uczestniczysz: {count}'**
  String effectOtherAlbums(Object count);

  /// No description provided for @legacyContact.
  ///
  /// In pl, this message translates to:
  /// **'Kontakt spadkowy'**
  String get legacyContact;

  /// No description provided for @legacyContactNone.
  ///
  /// In pl, this message translates to:
  /// **'Nikogo nie wskazano.'**
  String get legacyContactNone;

  /// No description provided for @legacyContactChoose.
  ///
  /// In pl, this message translates to:
  /// **'Wskaż kontakt spadkowy'**
  String get legacyContactChoose;

  /// No description provided for @deletionDate.
  ///
  /// In pl, this message translates to:
  /// **'Konto zostanie usunięte {date}.'**
  String deletionDate(Object date);

  /// No description provided for @cancelDeletion.
  ///
  /// In pl, this message translates to:
  /// **'Cofnij usunięcie'**
  String get cancelDeletion;

  /// No description provided for @understandDeletion.
  ///
  /// In pl, this message translates to:
  /// **'Rozumiem skutki usunięcia konta'**
  String get understandDeletion;

  /// No description provided for @albumsWithoutSuccessor.
  ///
  /// In pl, this message translates to:
  /// **'Masz albumy bez następcy. Wskaż następcę w albumie albo zgódź się na ich wygaśnięcie.'**
  String get albumsWithoutSuccessor;

  /// No description provided for @consentAlbumExpiry.
  ///
  /// In pl, this message translates to:
  /// **'Zgadzam się na wygaśnięcie albumów bez następcy'**
  String get consentAlbumExpiry;

  /// No description provided for @deletionWindowInfo.
  ///
  /// In pl, this message translates to:
  /// **'Konto zniknie po 30 dniach. Do tego czasu możesz się wycofać.'**
  String get deletionWindowInfo;

  /// No description provided for @deleteAccount.
  ///
  /// In pl, this message translates to:
  /// **'Usuń konto'**
  String get deleteAccount;

  /// No description provided for @noAccess.
  ///
  /// In pl, this message translates to:
  /// **'Brak dostępu. Zaloguj się ponownie.'**
  String get noAccess;

  /// No description provided for @alreadyDecidedAccount.
  ///
  /// In pl, this message translates to:
  /// **'To już rozstrzygnięte: okno usunięcia konta się zakończyło.'**
  String get alreadyDecidedAccount;

  /// No description provided for @albumWithoutSuccessorError.
  ///
  /// In pl, this message translates to:
  /// **'Masz albumy bez następcy. Wskaż następcę albo zgódź się na ich wygaśnięcie.'**
  String get albumWithoutSuccessorError;

  /// No description provided for @mapTitle.
  ///
  /// In pl, this message translates to:
  /// **'Mapa'**
  String get mapTitle;

  /// No description provided for @mapLoadingPoints.
  ///
  /// In pl, this message translates to:
  /// **'Ładowanie punktów…'**
  String get mapLoadingPoints;

  /// No description provided for @mapEmpty.
  ///
  /// In pl, this message translates to:
  /// **'Dodaj pierwsze wspomnienie'**
  String get mapEmpty;

  /// No description provided for @mapAddMemory.
  ///
  /// In pl, this message translates to:
  /// **'Dodaj wspomnienie'**
  String get mapAddMemory;

  /// No description provided for @mapOpenTrash.
  ///
  /// In pl, this message translates to:
  /// **'Kosz'**
  String get mapOpenTrash;

  /// No description provided for @mapAttribution.
  ///
  /// In pl, this message translates to:
  /// **'© OpenStreetMap'**
  String get mapAttribution;

  /// No description provided for @mapFilterDates.
  ///
  /// In pl, this message translates to:
  /// **'Daty'**
  String get mapFilterDates;

  /// No description provided for @mapFilterDatesRange.
  ///
  /// In pl, this message translates to:
  /// **'{from} – {to}'**
  String mapFilterDatesRange(Object from, Object to);

  /// No description provided for @mapFilterPeople.
  ///
  /// In pl, this message translates to:
  /// **'Osoby'**
  String get mapFilterPeople;

  /// No description provided for @mapFilterAlbums.
  ///
  /// In pl, this message translates to:
  /// **'Albumy'**
  String get mapFilterAlbums;

  /// No description provided for @mapPendingUpload.
  ///
  /// In pl, this message translates to:
  /// **'Czeka na wysłanie: {count}'**
  String mapPendingUpload(Object count);

  /// No description provided for @albumLink.
  ///
  /// In pl, this message translates to:
  /// **'Link do albumu'**
  String get albumLink;

  /// No description provided for @albumLinkNone.
  ///
  /// In pl, this message translates to:
  /// **'Album nie ma działającego linku'**
  String get albumLinkNone;

  /// No description provided for @albumShareByLink.
  ///
  /// In pl, this message translates to:
  /// **'Udostępnij linkiem'**
  String get albumShareByLink;

  /// No description provided for @albumRevokeLink.
  ///
  /// In pl, this message translates to:
  /// **'Unieważnij link (nikt nowy nie wejdzie)'**
  String get albumRevokeLink;

  /// No description provided for @albumCopyLink.
  ///
  /// In pl, this message translates to:
  /// **'Kopiuj link'**
  String get albumCopyLink;

  /// No description provided for @albumLinkCopied.
  ///
  /// In pl, this message translates to:
  /// **'Skopiowano link'**
  String get albumLinkCopied;

  /// No description provided for @albumViewers.
  ///
  /// In pl, this message translates to:
  /// **'Oglądający przez link'**
  String get albumViewers;

  /// No description provided for @albumViewersEmpty.
  ///
  /// In pl, this message translates to:
  /// **'Nikt jeszcze nie wszedł przez link'**
  String get albumViewersEmpty;

  /// No description provided for @albumRevokeAccess.
  ///
  /// In pl, this message translates to:
  /// **'Odbierz dostęp'**
  String get albumRevokeAccess;

  /// No description provided for @albumBlock.
  ///
  /// In pl, this message translates to:
  /// **'Zablokuj użytkownika'**
  String get albumBlock;

  /// No description provided for @albumBlockNobody.
  ///
  /// In pl, this message translates to:
  /// **'Nie ma tu nikogo, kogo możesz zablokować.'**
  String get albumBlockNobody;

  /// No description provided for @albumBlockConfirmTitle.
  ///
  /// In pl, this message translates to:
  /// **'Zablokować tę osobę?'**
  String get albumBlockConfirmTitle;

  /// No description provided for @albumBlockConfirmBody.
  ///
  /// In pl, this message translates to:
  /// **'{person} od razu straci dostęp i nie wejdzie już ani linkiem, ani przez zaproszenie. Odblokować może później właściciel albumu.'**
  String albumBlockConfirmBody(Object person);

  /// No description provided for @albumBlockConfirm.
  ///
  /// In pl, this message translates to:
  /// **'Zablokuj'**
  String get albumBlockConfirm;

  /// No description provided for @cancel.
  ///
  /// In pl, this message translates to:
  /// **'Anuluj'**
  String get cancel;

  /// No description provided for @albumBlockedPeople.
  ///
  /// In pl, this message translates to:
  /// **'Zablokowani'**
  String get albumBlockedPeople;

  /// No description provided for @albumUnblock.
  ///
  /// In pl, this message translates to:
  /// **'Odblokuj'**
  String get albumUnblock;

  /// No description provided for @albumBlocked.
  ///
  /// In pl, this message translates to:
  /// **'Nie masz dostępu do tego albumu.'**
  String get albumBlocked;

  /// No description provided for @albumLinkGone.
  ///
  /// In pl, this message translates to:
  /// **'Ten link już nie działa.'**
  String get albumLinkGone;

  /// No description provided for @albumOnlyOwnerBlocks.
  ///
  /// In pl, this message translates to:
  /// **'Osobę w albumie może zablokować tylko właściciel.'**
  String get albumOnlyOwnerBlocks;

  /// No description provided for @albumEntryTitle.
  ///
  /// In pl, this message translates to:
  /// **'Wejście do albumu przez link'**
  String get albumEntryTitle;

  /// No description provided for @albumEntryAlbum.
  ///
  /// In pl, this message translates to:
  /// **'Album: {name}'**
  String albumEntryAlbum(Object name);

  /// No description provided for @albumEntryOpen.
  ///
  /// In pl, this message translates to:
  /// **'Otwórz album'**
  String get albumEntryOpen;

  /// No description provided for @albumEntryNamed.
  ///
  /// In pl, this message translates to:
  /// **'Twój adres e-mail pojawi się na liście oglądających ten album.'**
  String get albumEntryNamed;

  /// No description provided for @albumReadOnlyViewer.
  ///
  /// In pl, this message translates to:
  /// **'Oglądasz ten album przez link.'**
  String get albumReadOnlyViewer;

  /// No description provided for @sharedEmpty.
  ///
  /// In pl, this message translates to:
  /// **'Nikt jeszcze nie udostępnił Ci albumu'**
  String get sharedEmpty;

  /// No description provided for @albumMine.
  ///
  /// In pl, this message translates to:
  /// **'Moje albumy'**
  String get albumMine;

  /// No description provided for @albumShared.
  ///
  /// In pl, this message translates to:
  /// **'Udostępnione mi'**
  String get albumShared;

  /// No description provided for @albumInvitations.
  ///
  /// In pl, this message translates to:
  /// **'Zaproszenia'**
  String get albumInvitations;

  /// No description provided for @albumNewName.
  ///
  /// In pl, this message translates to:
  /// **'Nazwa nowego albumu'**
  String get albumNewName;

  /// No description provided for @albumCreate.
  ///
  /// In pl, this message translates to:
  /// **'Utwórz album'**
  String get albumCreate;

  /// No description provided for @albumsEmpty.
  ///
  /// In pl, this message translates to:
  /// **'Nie masz jeszcze albumów'**
  String get albumsEmpty;

  /// No description provided for @albumSharedEmpty.
  ///
  /// In pl, this message translates to:
  /// **'Nikt nie udostępnił Ci albumu'**
  String get albumSharedEmpty;

  /// No description provided for @albumMembersCount.
  ///
  /// In pl, this message translates to:
  /// **'Osoby: {members}, wspomnienia: {memories}'**
  String albumMembersCount(Object members, Object memories);

  /// No description provided for @albumNameRequired.
  ///
  /// In pl, this message translates to:
  /// **'Podaj nazwę albumu.'**
  String get albumNameRequired;

  /// No description provided for @albumNoAccess.
  ///
  /// In pl, this message translates to:
  /// **'Nie masz już dostępu do tego albumu.'**
  String get albumNoAccess;

  /// No description provided for @albumNoFreeSeats.
  ///
  /// In pl, this message translates to:
  /// **'W planie tego albumu nie ma wolnego miejsca.'**
  String get albumNoFreeSeats;

  /// No description provided for @albumInvitationLimit.
  ///
  /// In pl, this message translates to:
  /// **'Dzisiejszy limit 10 zaproszeń został wykorzystany. Spróbuj jutro.'**
  String get albumInvitationLimit;

  /// No description provided for @albumAddressBlocked.
  ///
  /// In pl, this message translates to:
  /// **'Ten adres nie przyjmuje zaproszeń.'**
  String get albumAddressBlocked;

  /// No description provided for @albumInvitationGone.
  ///
  /// In pl, this message translates to:
  /// **'Zaproszenie wygasło albo zostało wycofane.'**
  String get albumInvitationGone;

  /// No description provided for @albumPoolFull.
  ///
  /// In pl, this message translates to:
  /// **'Pula albumu jest pełna. Właściciel dostał powiadomienie.'**
  String get albumPoolFull;

  /// No description provided for @albumOwnerCannotLeave.
  ///
  /// In pl, this message translates to:
  /// **'Właściciel nie może opuścić albumu.'**
  String get albumOwnerCannotLeave;

  /// No description provided for @albumMemories.
  ///
  /// In pl, this message translates to:
  /// **'Wspomnienia'**
  String get albumMemories;

  /// No description provided for @albumEmpty.
  ///
  /// In pl, this message translates to:
  /// **'Album jest pusty'**
  String get albumEmpty;

  /// No description provided for @albumMembers.
  ///
  /// In pl, this message translates to:
  /// **'Skład'**
  String get albumMembers;

  /// No description provided for @albumOwnerRole.
  ///
  /// In pl, this message translates to:
  /// **'właściciel'**
  String get albumOwnerRole;

  /// No description provided for @albumParticipantRole.
  ///
  /// In pl, this message translates to:
  /// **'uczestnik'**
  String get albumParticipantRole;

  /// No description provided for @albumMyContributions.
  ///
  /// In pl, this message translates to:
  /// **'Moje wkłady: wspomnienia {memories}, zdjęcia {photos}'**
  String albumMyContributions(Object memories, Object photos);

  /// No description provided for @albumPoolLeft.
  ///
  /// In pl, this message translates to:
  /// **'Zostało w puli: {left} z {total}'**
  String albumPoolLeft(Object left, Object total);

  /// No description provided for @albumNoOwnerState.
  ///
  /// In pl, this message translates to:
  /// **'Ten album ma właściciela.'**
  String get albumNoOwnerState;

  /// No description provided for @albumAddMemory.
  ///
  /// In pl, this message translates to:
  /// **'Dodaj moje wspomnienie'**
  String get albumAddMemory;

  /// No description provided for @albumNoMemoriesToAdd.
  ///
  /// In pl, this message translates to:
  /// **'Nie masz jeszcze wspomnień do dodania.'**
  String get albumNoMemoriesToAdd;

  /// No description provided for @albumAddPhotos.
  ///
  /// In pl, this message translates to:
  /// **'Dodaj zdjęcia'**
  String get albumAddPhotos;

  /// No description provided for @albumTag.
  ///
  /// In pl, this message translates to:
  /// **'Oznacz uczestnika'**
  String get albumTag;

  /// No description provided for @albumRemoveMyTag.
  ///
  /// In pl, this message translates to:
  /// **'Usuń moje oznaczenie'**
  String get albumRemoveMyTag;

  /// No description provided for @albumLeave.
  ///
  /// In pl, this message translates to:
  /// **'Opuść album'**
  String get albumLeave;

  /// No description provided for @albumLeaveKeep.
  ///
  /// In pl, this message translates to:
  /// **'Zostaw moje wkłady w albumie'**
  String get albumLeaveKeep;

  /// No description provided for @albumLeaveTake.
  ///
  /// In pl, this message translates to:
  /// **'Zabierz moje wkłady'**
  String get albumLeaveTake;

  /// No description provided for @albumLeaveConfirm.
  ///
  /// In pl, this message translates to:
  /// **'Potwierdzam wyjście z albumu'**
  String get albumLeaveConfirm;

  /// No description provided for @albumManage.
  ///
  /// In pl, this message translates to:
  /// **'Zarządzanie albumem'**
  String get albumManage;

  /// No description provided for @albumConsents.
  ///
  /// In pl, this message translates to:
  /// **'Moje zgody i powierzenia'**
  String get albumConsents;

  /// No description provided for @albumSeats.
  ///
  /// In pl, this message translates to:
  /// **'Miejsca: zajęte {taken}, zaproszone {invited}, wolne {free}'**
  String albumSeats(Object free, Object invited, Object taken);

  /// No description provided for @albumInviteEmail.
  ///
  /// In pl, this message translates to:
  /// **'E-mail zapraszanego'**
  String get albumInviteEmail;

  /// No description provided for @albumInvite.
  ///
  /// In pl, this message translates to:
  /// **'Zaproś'**
  String get albumInvite;

  /// No description provided for @albumInviteInvalid.
  ///
  /// In pl, this message translates to:
  /// **'To nie wygląda na adres e-mail.'**
  String get albumInviteInvalid;

  /// No description provided for @albumInvited.
  ///
  /// In pl, this message translates to:
  /// **'Zaproszeni'**
  String get albumInvited;

  /// No description provided for @albumWithdrawInvitation.
  ///
  /// In pl, this message translates to:
  /// **'Wycofaj'**
  String get albumWithdrawInvitation;

  /// No description provided for @albumRemoveParticipant.
  ///
  /// In pl, this message translates to:
  /// **'Usuń uczestnika'**
  String get albumRemoveParticipant;

  /// No description provided for @albumSuccessor.
  ///
  /// In pl, this message translates to:
  /// **'Następca'**
  String get albumSuccessor;

  /// No description provided for @reportContent.
  ///
  /// In pl, this message translates to:
  /// **'Zgłoś treść'**
  String get reportContent;

  /// No description provided for @reportMemoryTitle.
  ///
  /// In pl, this message translates to:
  /// **'Zgłoś wspomnienie'**
  String get reportMemoryTitle;

  /// No description provided for @reportAlbumTitle.
  ///
  /// In pl, this message translates to:
  /// **'Zgłoś album'**
  String get reportAlbumTitle;

  /// No description provided for @reportExplained.
  ///
  /// In pl, this message translates to:
  /// **'Zgłoszenie trafi do obsługi Memorio. Autor nie dowie się, kto zgłosił. Treści niezgodne z prawem ukrywamy od razu i rozpatrujemy w ciągu 24 godzin, pozostałe zostają widoczne do decyzji, którą podejmujemy w ciągu 7 dni.'**
  String get reportExplained;

  /// No description provided for @reportCategory.
  ///
  /// In pl, this message translates to:
  /// **'Co jest nie tak?'**
  String get reportCategory;

  /// No description provided for @reasonChildAbuse.
  ///
  /// In pl, this message translates to:
  /// **'Wykorzystywanie dzieci'**
  String get reasonChildAbuse;

  /// No description provided for @reasonTerrorism.
  ///
  /// In pl, this message translates to:
  /// **'Treści terrorystyczne'**
  String get reasonTerrorism;

  /// No description provided for @reasonThreat.
  ///
  /// In pl, this message translates to:
  /// **'Groźby'**
  String get reasonThreat;

  /// No description provided for @reasonIncitement.
  ///
  /// In pl, this message translates to:
  /// **'Nawoływanie do przemocy lub nienawiści'**
  String get reasonIncitement;

  /// No description provided for @reasonOtherIllegal.
  ///
  /// In pl, this message translates to:
  /// **'Inna treść niezgodna z prawem'**
  String get reasonOtherIllegal;

  /// No description provided for @reasonSomeonesImage.
  ///
  /// In pl, this message translates to:
  /// **'Mój wizerunek bez mojej zgody'**
  String get reasonSomeonesImage;

  /// No description provided for @reasonPersonalData.
  ///
  /// In pl, this message translates to:
  /// **'Moje dane osobowe'**
  String get reasonPersonalData;

  /// No description provided for @reasonDefamation.
  ///
  /// In pl, this message translates to:
  /// **'Zniesławienie'**
  String get reasonDefamation;

  /// No description provided for @reasonCopyright.
  ///
  /// In pl, this message translates to:
  /// **'Naruszenie praw autorskich'**
  String get reasonCopyright;

  /// No description provided for @reasonOther.
  ///
  /// In pl, this message translates to:
  /// **'Inny powód'**
  String get reasonOther;

  /// No description provided for @reportIllegalHidden.
  ///
  /// In pl, this message translates to:
  /// **'Ta treść zostanie ukryta od razu po wysłaniu zgłoszenia, do decyzji obsługi.'**
  String get reportIllegalHidden;

  /// No description provided for @reportExplanation.
  ///
  /// In pl, this message translates to:
  /// **'Uzasadnienie'**
  String get reportExplanation;

  /// No description provided for @reportSend.
  ///
  /// In pl, this message translates to:
  /// **'Zgłoś'**
  String get reportSend;

  /// No description provided for @reportSent.
  ///
  /// In pl, this message translates to:
  /// **'Dziękujemy. Decyzję z uzasadnieniem zobaczysz na ekranie „Moje zgłoszenia i decyzje”.'**
  String get reportSent;

  /// No description provided for @reportSentIllegal.
  ///
  /// In pl, this message translates to:
  /// **'Dziękujemy. Treść jest już ukryta. Decyzję z uzasadnieniem zobaczysz na ekranie „Moje zgłoszenia i decyzje”.'**
  String get reportSentIllegal;

  /// No description provided for @reportBack.
  ///
  /// In pl, this message translates to:
  /// **'Wróć'**
  String get reportBack;

  /// No description provided for @reportNoAccess.
  ///
  /// In pl, this message translates to:
  /// **'Tej treści już nie widzisz.'**
  String get reportNoAccess;

  /// No description provided for @reportOwnContent.
  ///
  /// In pl, this message translates to:
  /// **'Własną treść możesz zmienić albo usunąć samodzielnie.'**
  String get reportOwnContent;

  /// No description provided for @myModerationTitle.
  ///
  /// In pl, this message translates to:
  /// **'Moje zgłoszenia i decyzje'**
  String get myModerationTitle;

  /// No description provided for @myModerationEmpty.
  ///
  /// In pl, this message translates to:
  /// **'Nie masz zgłoszeń ani decyzji'**
  String get myModerationEmpty;

  /// No description provided for @myModerationAboutMine.
  ///
  /// In pl, this message translates to:
  /// **'Decyzje o moich treściach'**
  String get myModerationAboutMine;

  /// No description provided for @myModerationMine.
  ///
  /// In pl, this message translates to:
  /// **'Moje zgłoszenia'**
  String get myModerationMine;

  /// No description provided for @myModerationReported.
  ///
  /// In pl, this message translates to:
  /// **'{reason}, zgłoszone {date}'**
  String myModerationReported(Object date, Object reason);

  /// No description provided for @myModerationHidden.
  ///
  /// In pl, this message translates to:
  /// **'Teraz widzisz to tylko Ty.'**
  String get myModerationHidden;

  /// No description provided for @myModerationStatement.
  ///
  /// In pl, this message translates to:
  /// **'Uzasadnienie: {statement}'**
  String myModerationStatement(Object statement);

  /// No description provided for @myModerationAppealPending.
  ///
  /// In pl, this message translates to:
  /// **'Odwołanie czeka na decyzję.'**
  String get myModerationAppealPending;

  /// No description provided for @myModerationAppealReversed.
  ///
  /// In pl, this message translates to:
  /// **'Odwołanie uwzględnione: treść jest znów widoczna.'**
  String get myModerationAppealReversed;

  /// No description provided for @myModerationAppealUpheld.
  ///
  /// In pl, this message translates to:
  /// **'Odwołanie nieuwzględnione.'**
  String get myModerationAppealUpheld;

  /// No description provided for @myModerationAppealReason.
  ///
  /// In pl, this message translates to:
  /// **'Dlaczego decyzja jest błędna'**
  String get myModerationAppealReason;

  /// No description provided for @myModerationAppealUntil.
  ///
  /// In pl, this message translates to:
  /// **'Możesz się odwołać do {date}.'**
  String myModerationAppealUntil(Object date);

  /// No description provided for @myModerationAppeal.
  ///
  /// In pl, this message translates to:
  /// **'Odwołaj się'**
  String get myModerationAppeal;

  /// No description provided for @verdictRemoved.
  ///
  /// In pl, this message translates to:
  /// **'Decyzja: treść ukryta'**
  String get verdictRemoved;

  /// No description provided for @verdictKept.
  ///
  /// In pl, this message translates to:
  /// **'Decyzja: treść zostaje'**
  String get verdictKept;

  /// No description provided for @verdictPending.
  ///
  /// In pl, this message translates to:
  /// **'Czeka na decyzję'**
  String get verdictPending;

  /// No description provided for @albumHandover.
  ///
  /// In pl, this message translates to:
  /// **'Przekazanie albumu'**
  String get albumHandover;

  /// No description provided for @albumHandoverExplain.
  ///
  /// In pl, this message translates to:
  /// **'Możesz już teraz przekazać album komuś z uczestników. Ta osoba zostanie właścicielem, a Ty zostaniesz w albumie jako uczestnik.'**
  String get albumHandoverExplain;

  /// No description provided for @albumHandOver.
  ///
  /// In pl, this message translates to:
  /// **'Przekaż album'**
  String get albumHandOver;

  /// No description provided for @albumHandoverTo.
  ///
  /// In pl, this message translates to:
  /// **'Przekazać album: {person}?'**
  String albumHandoverTo(Object person);

  /// No description provided for @albumHandoverQuestion.
  ///
  /// In pl, this message translates to:
  /// **'Co z Twoimi własnymi wspomnieniami w tym albumie? Zdecyduj teraz: zostaną w albumie albo wyjmiemy je z niego, gdy ta osoba przyjmie album. U Ciebie zostaną w obu przypadkach.'**
  String get albumHandoverQuestion;

  /// No description provided for @albumHandoverKeep.
  ///
  /// In pl, this message translates to:
  /// **'Zostają w albumie'**
  String get albumHandoverKeep;

  /// No description provided for @albumHandoverTake.
  ///
  /// In pl, this message translates to:
  /// **'Zabieram je'**
  String get albumHandoverTake;

  /// No description provided for @albumHandoverWaiting.
  ///
  /// In pl, this message translates to:
  /// **'Czeka na odpowiedź: {person}, do {date}.'**
  String albumHandoverWaiting(Object date, Object person);

  /// No description provided for @albumHandoverWithdraw.
  ///
  /// In pl, this message translates to:
  /// **'Wycofaj propozycję'**
  String get albumHandoverWithdraw;

  /// No description provided for @albumHandoverForMe.
  ///
  /// In pl, this message translates to:
  /// **'Właściciel chce Ci przekazać ten album. Możesz odpowiedzieć do {date}.'**
  String albumHandoverForMe(Object date);

  /// No description provided for @albumHandoverOwnerKeeps.
  ///
  /// In pl, this message translates to:
  /// **'Jego wspomnienia zostaną w albumie.'**
  String get albumHandoverOwnerKeeps;

  /// No description provided for @albumHandoverOwnerTakes.
  ///
  /// In pl, this message translates to:
  /// **'Jego wspomnienia zostaną z albumu wyjęte.'**
  String get albumHandoverOwnerTakes;

  /// No description provided for @albumHandoverAccept.
  ///
  /// In pl, this message translates to:
  /// **'Przejmuję album'**
  String get albumHandoverAccept;

  /// No description provided for @albumHandoverRefuse.
  ///
  /// In pl, this message translates to:
  /// **'Odmawiam'**
  String get albumHandoverRefuse;

  /// No description provided for @albumNoSuccessor.
  ///
  /// In pl, this message translates to:
  /// **'Nie wskazano następcy'**
  String get albumNoSuccessor;

  /// No description provided for @albumNameSuccessor.
  ///
  /// In pl, this message translates to:
  /// **'Wskaż następcę'**
  String get albumNameSuccessor;

  /// No description provided for @albumCurator.
  ///
  /// In pl, this message translates to:
  /// **'Kurator'**
  String get albumCurator;

  /// No description provided for @albumNoCurator.
  ///
  /// In pl, this message translates to:
  /// **'Nie powierzono treści kuratorowi'**
  String get albumNoCurator;

  /// No description provided for @albumEntrust.
  ///
  /// In pl, this message translates to:
  /// **'Powierz kuratorowi'**
  String get albumEntrust;

  /// No description provided for @albumRevokeEntrustment.
  ///
  /// In pl, this message translates to:
  /// **'Odwołaj powierzenie'**
  String get albumRevokeEntrustment;

  /// No description provided for @albumKept.
  ///
  /// In pl, this message translates to:
  /// **'Moje wkłady pozostawione po wyjściu'**
  String get albumKept;

  /// No description provided for @albumKeptEmpty.
  ///
  /// In pl, this message translates to:
  /// **'Nie masz wkładów pozostawionych po wyjściu'**
  String get albumKeptEmpty;

  /// No description provided for @albumKeptItem.
  ///
  /// In pl, this message translates to:
  /// **'{album}: wspomnienia {memories}'**
  String albumKeptItem(Object album, Object memories);

  /// No description provided for @albumKeptNotChosen.
  ///
  /// In pl, this message translates to:
  /// **'Usunięto Cię z albumu. Wybierz, czy treści mają zostać.'**
  String get albumKeptNotChosen;

  /// No description provided for @albumKeptConfirm.
  ///
  /// In pl, this message translates to:
  /// **'Wiem, że treści znikną z albumu'**
  String get albumKeptConfirm;

  /// No description provided for @albumRevokeKept.
  ///
  /// In pl, this message translates to:
  /// **'Odwołaj zgodę „zostaw”'**
  String get albumRevokeKept;

  /// No description provided for @albumEntrusted.
  ///
  /// In pl, this message translates to:
  /// **'Powierzone treści'**
  String get albumEntrusted;

  /// No description provided for @albumEntrustedEmpty.
  ///
  /// In pl, this message translates to:
  /// **'Nikt nie powierzył Ci treści'**
  String get albumEntrustedEmpty;

  /// No description provided for @albumArrange.
  ///
  /// In pl, this message translates to:
  /// **'Uporządkuj'**
  String get albumArrange;

  /// No description provided for @albumMoveUp.
  ///
  /// In pl, this message translates to:
  /// **'Wyżej'**
  String get albumMoveUp;

  /// No description provided for @albumMoveDown.
  ///
  /// In pl, this message translates to:
  /// **'Niżej'**
  String get albumMoveDown;

  /// No description provided for @albumInvitationTitle.
  ///
  /// In pl, this message translates to:
  /// **'Zaproszenie do albumu'**
  String get albumInvitationTitle;

  /// No description provided for @albumInvitedBy.
  ///
  /// In pl, this message translates to:
  /// **'Kto zaprasza: {person}'**
  String albumInvitedBy(Object person);

  /// No description provided for @albumInvitationAlbum.
  ///
  /// In pl, this message translates to:
  /// **'Album: {name}'**
  String albumInvitationAlbum(Object name);

  /// No description provided for @albumAccept.
  ///
  /// In pl, this message translates to:
  /// **'Przyjmij'**
  String get albumAccept;

  /// No description provided for @albumDecline.
  ///
  /// In pl, this message translates to:
  /// **'Odrzuć'**
  String get albumDecline;

  /// No description provided for @albumBlockInvitations.
  ///
  /// In pl, this message translates to:
  /// **'Nie chcę zaproszeń z GEO'**
  String get albumBlockInvitations;

  /// No description provided for @mapEmptyFiltered.
  ///
  /// In pl, this message translates to:
  /// **'Brak wspomnień w tych datach'**
  String get mapEmptyFiltered;

  /// No description provided for @mapRejected.
  ///
  /// In pl, this message translates to:
  /// **'Nie udało się wysłać: {count}'**
  String mapRejected(Object count);

  /// No description provided for @mapRejectedDismiss.
  ///
  /// In pl, this message translates to:
  /// **'Rozumiem'**
  String get mapRejectedDismiss;

  /// No description provided for @newMemoryTitle.
  ///
  /// In pl, this message translates to:
  /// **'Nowe wspomnienie'**
  String get newMemoryTitle;

  /// No description provided for @newMemoryStepPlace.
  ///
  /// In pl, this message translates to:
  /// **'Miejsce i data'**
  String get newMemoryStepPlace;

  /// No description provided for @newMemoryStepContent.
  ///
  /// In pl, this message translates to:
  /// **'Treść'**
  String get newMemoryStepContent;

  /// No description provided for @next.
  ///
  /// In pl, this message translates to:
  /// **'Dalej'**
  String get next;

  /// No description provided for @back.
  ///
  /// In pl, this message translates to:
  /// **'Wstecz'**
  String get back;

  /// No description provided for @place.
  ///
  /// In pl, this message translates to:
  /// **'Miejsce'**
  String get place;

  /// No description provided for @placeChosen.
  ///
  /// In pl, this message translates to:
  /// **'Wybrano: {name}'**
  String placeChosen(Object name);

  /// No description provided for @placeChange.
  ///
  /// In pl, this message translates to:
  /// **'Zmień miejsce'**
  String get placeChange;

  /// No description provided for @placeRecent.
  ///
  /// In pl, this message translates to:
  /// **'Ostatnio użyte'**
  String get placeRecent;

  /// No description provided for @ownPlaceName.
  ///
  /// In pl, this message translates to:
  /// **'Nazwa własna miejsca'**
  String get ownPlaceName;

  /// No description provided for @ownPlaceLatitude.
  ///
  /// In pl, this message translates to:
  /// **'Szerokość geograficzna'**
  String get ownPlaceLatitude;

  /// No description provided for @ownPlaceLongitude.
  ///
  /// In pl, this message translates to:
  /// **'Długość geograficzna'**
  String get ownPlaceLongitude;

  /// No description provided for @ownPlaceAdd.
  ///
  /// In pl, this message translates to:
  /// **'Dodaj miejsce własne'**
  String get ownPlaceAdd;

  /// No description provided for @ownPlaceInvalid.
  ///
  /// In pl, this message translates to:
  /// **'Podaj nazwę oraz szerokość od -90 do 90 i długość od -180 do 180.'**
  String get ownPlaceInvalid;

  /// No description provided for @datePrecision.
  ///
  /// In pl, this message translates to:
  /// **'Data'**
  String get datePrecision;

  /// No description provided for @precisionDay.
  ///
  /// In pl, this message translates to:
  /// **'Dzień'**
  String get precisionDay;

  /// No description provided for @precisionMonth.
  ///
  /// In pl, this message translates to:
  /// **'Miesiąc'**
  String get precisionMonth;

  /// No description provided for @precisionYear.
  ///
  /// In pl, this message translates to:
  /// **'Rok'**
  String get precisionYear;

  /// No description provided for @precisionPeriod.
  ///
  /// In pl, this message translates to:
  /// **'Okres'**
  String get precisionPeriod;

  /// No description provided for @dateChoose.
  ///
  /// In pl, this message translates to:
  /// **'Wybierz datę'**
  String get dateChoose;

  /// No description provided for @dateChooseEnd.
  ///
  /// In pl, this message translates to:
  /// **'Wybierz koniec okresu'**
  String get dateChooseEnd;

  /// No description provided for @dateChosen.
  ///
  /// In pl, this message translates to:
  /// **'Od: {date}'**
  String dateChosen(Object date);

  /// No description provided for @dateChosenEnd.
  ///
  /// In pl, this message translates to:
  /// **'Do: {date}'**
  String dateChosenEnd(Object date);

  /// No description provided for @timeChoose.
  ///
  /// In pl, this message translates to:
  /// **'Dodaj godzinę (nieobowiązkowo)'**
  String get timeChoose;

  /// No description provided for @timeChosen.
  ///
  /// In pl, this message translates to:
  /// **'Godzina: {time}'**
  String timeChosen(Object time);

  /// No description provided for @timeClear.
  ///
  /// In pl, this message translates to:
  /// **'Bez godziny'**
  String get timeClear;

  /// No description provided for @dateInFuture.
  ///
  /// In pl, this message translates to:
  /// **'Wspomnienie nie może być z przyszłości.'**
  String get dateInFuture;

  /// No description provided for @photos.
  ///
  /// In pl, this message translates to:
  /// **'Zdjęcia'**
  String get photos;

  /// No description provided for @photosAdd.
  ///
  /// In pl, this message translates to:
  /// **'Dodaj zdjęcia'**
  String get photosAdd;

  /// No description provided for @photosPicked.
  ///
  /// In pl, this message translates to:
  /// **'Wybrane zdjęcia: {count}'**
  String photosPicked(Object count);

  /// No description provided for @description.
  ///
  /// In pl, this message translates to:
  /// **'Opis'**
  String get description;

  /// No description provided for @aiDraft.
  ///
  /// In pl, this message translates to:
  /// **'Szkic AI'**
  String get aiDraft;

  /// No description provided for @aiConsent.
  ///
  /// In pl, this message translates to:
  /// **'Zgadzam się na wysłanie opisu do dostawcy AI, bez nazwy miejsca'**
  String get aiConsent;

  /// No description provided for @aiRequest.
  ///
  /// In pl, this message translates to:
  /// **'Poproś AI o szkic'**
  String get aiRequest;

  /// No description provided for @aiMarker.
  ///
  /// In pl, this message translates to:
  /// **'Tekst AI'**
  String get aiMarker;

  /// No description provided for @aiUnavailableOffline.
  ///
  /// In pl, this message translates to:
  /// **'Szkic AI nie działa bez połączenia.'**
  String get aiUnavailableOffline;

  /// No description provided for @aiUseDraft.
  ///
  /// In pl, this message translates to:
  /// **'Użyj szkicu'**
  String get aiUseDraft;

  /// No description provided for @approveDescription.
  ///
  /// In pl, this message translates to:
  /// **'Zatwierdź opis'**
  String get approveDescription;

  /// No description provided for @descriptionApproved.
  ///
  /// In pl, this message translates to:
  /// **'Opis zatwierdzony'**
  String get descriptionApproved;

  /// No description provided for @saveMemory.
  ///
  /// In pl, this message translates to:
  /// **'Zapisz wspomnienie'**
  String get saveMemory;

  /// No description provided for @savedOffline.
  ///
  /// In pl, this message translates to:
  /// **'Zapisano. Wyślemy, gdy wróci sieć.'**
  String get savedOffline;

  /// No description provided for @offlineNotOnWeb.
  ///
  /// In pl, this message translates to:
  /// **'W przeglądarce nie można zapisywać bez połączenia.'**
  String get offlineNotOnWeb;

  /// No description provided for @termsRequired.
  ///
  /// In pl, this message translates to:
  /// **'Przed pierwszym wspomnieniem zaakceptuj regulamin.'**
  String get termsRequired;

  /// No description provided for @memoryNoAccess.
  ///
  /// In pl, this message translates to:
  /// **'Nie masz dostępu do tego wspomnienia.'**
  String get memoryNoAccess;

  /// No description provided for @alreadyDecided.
  ///
  /// In pl, this message translates to:
  /// **'To już rozstrzygnięte. Odśwież i sprawdź aktualny stan.'**
  String get alreadyDecided;

  /// No description provided for @placeMissing.
  ///
  /// In pl, this message translates to:
  /// **'Tego miejsca już nie ma. Wybierz inne.'**
  String get placeMissing;

  /// No description provided for @spaceFull.
  ///
  /// In pl, this message translates to:
  /// **'Pula miejsca jest pełna. To zdjęcie się nie zmieści.'**
  String get spaceFull;

  /// No description provided for @aiLimitReached.
  ///
  /// In pl, this message translates to:
  /// **'Limit szkiców AI w tym miesiącu jest wyczerpany.'**
  String get aiLimitReached;

  /// No description provided for @aiProviderDown.
  ///
  /// In pl, this message translates to:
  /// **'Dostawca AI jest teraz niedostępny. Spróbuj później.'**
  String get aiProviderDown;

  /// No description provided for @invalidInput.
  ///
  /// In pl, this message translates to:
  /// **'Sprawdź wpisane dane.'**
  String get invalidInput;

  /// No description provided for @memoryTitle.
  ///
  /// In pl, this message translates to:
  /// **'Wspomnienie'**
  String get memoryTitle;

  /// No description provided for @memoryWhen.
  ///
  /// In pl, this message translates to:
  /// **'Data: {date}'**
  String memoryWhen(Object date);

  /// No description provided for @memoryPlace.
  ///
  /// In pl, this message translates to:
  /// **'Miejsce: {name}'**
  String memoryPlace(Object name);

  /// No description provided for @memoryContents.
  ///
  /// In pl, this message translates to:
  /// **'Treści'**
  String get memoryContents;

  /// No description provided for @memoryAuthorMe.
  ///
  /// In pl, this message translates to:
  /// **'Autor: Ty'**
  String get memoryAuthorMe;

  /// No description provided for @memoryPeople.
  ///
  /// In pl, this message translates to:
  /// **'Osoby'**
  String get memoryPeople;

  /// No description provided for @memoryPeopleNone.
  ///
  /// In pl, this message translates to:
  /// **'Nikt nie jest oznaczony'**
  String get memoryPeopleNone;

  /// No description provided for @memoryNoDescription.
  ///
  /// In pl, this message translates to:
  /// **'Bez opisu'**
  String get memoryNoDescription;

  /// No description provided for @memoryGone.
  ///
  /// In pl, this message translates to:
  /// **'Nie masz już dostępu do tego wspomnienia'**
  String get memoryGone;

  /// No description provided for @memoryBack.
  ///
  /// In pl, this message translates to:
  /// **'Wróć'**
  String get memoryBack;

  /// No description provided for @deleteMine.
  ///
  /// In pl, this message translates to:
  /// **'Usuń moją treść'**
  String get deleteMine;

  /// No description provided for @deleteForGoodConfirm.
  ///
  /// In pl, this message translates to:
  /// **'Rozumiem, że tego nie da się cofnąć'**
  String get deleteForGoodConfirm;

  /// No description provided for @deleteForGood.
  ///
  /// In pl, this message translates to:
  /// **'Usuń trwale (formalne żądanie)'**
  String get deleteForGood;

  /// No description provided for @trashTitle.
  ///
  /// In pl, this message translates to:
  /// **'Kosz'**
  String get trashTitle;

  /// No description provided for @trashEmpty.
  ///
  /// In pl, this message translates to:
  /// **'Kosz jest pusty'**
  String get trashEmpty;

  /// No description provided for @trashItem.
  ///
  /// In pl, this message translates to:
  /// **'{place}, {date}'**
  String trashItem(Object date, Object place);

  /// No description provided for @trashDeletedAt.
  ///
  /// In pl, this message translates to:
  /// **'Usunięto: {date}'**
  String trashDeletedAt(Object date);

  /// No description provided for @trashDaysLeft.
  ///
  /// In pl, this message translates to:
  /// **'Zostało dni: {count}'**
  String trashDaysLeft(Object count);

  /// No description provided for @trashPhotos.
  ///
  /// In pl, this message translates to:
  /// **'Zdjęcia: {count}'**
  String trashPhotos(Object count);

  /// No description provided for @restore.
  ///
  /// In pl, this message translates to:
  /// **'Przywróć'**
  String get restore;

  /// S08
  ///
  /// In pl, this message translates to:
  /// **'Prześledź'**
  String get replayTitle;

  /// No description provided for @replayPeriod.
  ///
  /// In pl, this message translates to:
  /// **'Okres: {period}'**
  String replayPeriod(String period);

  /// No description provided for @replayOtherRange.
  ///
  /// In pl, this message translates to:
  /// **'Inny okres'**
  String get replayOtherRange;

  /// No description provided for @replayEmpty.
  ///
  /// In pl, this message translates to:
  /// **'W tym okresie nie ma wspomnień'**
  String get replayEmpty;

  /// No description provided for @replayStep.
  ///
  /// In pl, this message translates to:
  /// **'{current} z {total}'**
  String replayStep(int current, int total);

  /// No description provided for @replayPrevious.
  ///
  /// In pl, this message translates to:
  /// **'Poprzednie'**
  String get replayPrevious;

  /// No description provided for @replayNext.
  ///
  /// In pl, this message translates to:
  /// **'Następne'**
  String get replayNext;

  /// No description provided for @replayPlay.
  ///
  /// In pl, this message translates to:
  /// **'Odtwarzaj'**
  String get replayPlay;

  /// No description provided for @replayPause.
  ///
  /// In pl, this message translates to:
  /// **'Zatrzymaj'**
  String get replayPause;

  /// No description provided for @replayTempo.
  ///
  /// In pl, this message translates to:
  /// **'Tempo {speed}×'**
  String replayTempo(int speed);

  /// No description provided for @replayEnd.
  ///
  /// In pl, this message translates to:
  /// **'To już wszystko z tego okresu'**
  String get replayEnd;

  /// No description provided for @replayStats.
  ///
  /// In pl, this message translates to:
  /// **'Wspomnienia: {memories}, miejsca: {places}'**
  String replayStats(int memories, int places);

  /// No description provided for @replayAgain.
  ///
  /// In pl, this message translates to:
  /// **'Od początku'**
  String get replayAgain;

  /// No description provided for @replayCreateAlbum.
  ///
  /// In pl, this message translates to:
  /// **'Utwórz album z okresu'**
  String get replayCreateAlbum;

  /// No description provided for @replayCreateAlbumExplained.
  ///
  /// In pl, this message translates to:
  /// **'Do albumu trafią Twoje wspomnienia z tego okresu. Cudze zostają w swoich albumach.'**
  String get replayCreateAlbumExplained;

  /// No description provided for @replayAlbumName.
  ///
  /// In pl, this message translates to:
  /// **'Prześledź {period}'**
  String replayAlbumName(String period);

  /// No description provided for @replayAlbumCreated.
  ///
  /// In pl, this message translates to:
  /// **'Album gotowy, dodano wspomnień: {count}.'**
  String replayAlbumCreated(int count);

  /// No description provided for @replayAlbumNotFitting.
  ///
  /// In pl, this message translates to:
  /// **'Nie zmieściło się w puli albumu: {count}.'**
  String replayAlbumNotFitting(int count);

  /// No description provided for @replayOpenAlbum.
  ///
  /// In pl, this message translates to:
  /// **'Otwórz album'**
  String get replayOpenAlbum;

  /// No description provided for @replayTooLong.
  ///
  /// In pl, this message translates to:
  /// **'W tym okresie jest za dużo wspomnień na jeden album. Wybierz krótszy.'**
  String get replayTooLong;

  /// S23
  ///
  /// In pl, this message translates to:
  /// **'Przejęcie albumu'**
  String get takeoverTitle;

  /// No description provided for @takeoverDeadline.
  ///
  /// In pl, this message translates to:
  /// **'Na decyzję masz czas do: {date}'**
  String takeoverDeadline(String date);

  /// No description provided for @takeoverFounder.
  ///
  /// In pl, this message translates to:
  /// **'Wspomnienia fundatora w albumie: {count}. Zostają w nim.'**
  String takeoverFounder(int count);

  /// No description provided for @takeoverExplained.
  ///
  /// In pl, this message translates to:
  /// **'Jeśli przejmiesz album, zostaniesz jego właścicielem, razem z jego pulą. Jeśli odmówisz albo nie odpowiesz w terminie, album zostanie bez właściciela, a przejąć go będzie mógł inny uczestnik.'**
  String get takeoverExplained;

  /// No description provided for @takeoverStoreAdvice.
  ///
  /// In pl, this message translates to:
  /// **'Rodzina zmarłej osoby powinna anulować jej subskrypcję w sklepie: aplikacja nie może tego zrobić. Album działa do końca opłaconego okresu.'**
  String get takeoverStoreAdvice;

  /// No description provided for @takeoverAccept.
  ///
  /// In pl, this message translates to:
  /// **'Przejmuję album'**
  String get takeoverAccept;

  /// No description provided for @takeoverRefuse.
  ///
  /// In pl, this message translates to:
  /// **'Odmawiam'**
  String get takeoverRefuse;

  /// No description provided for @takeoverNotSuccessor.
  ///
  /// In pl, this message translates to:
  /// **'Nie jesteś wskazanym następcą.'**
  String get takeoverNotSuccessor;

  /// No description provided for @takeoverDecided.
  ///
  /// In pl, this message translates to:
  /// **'Sprawa już rozstrzygnięta.'**
  String get takeoverDecided;

  /// No description provided for @takeoverOfferTile.
  ///
  /// In pl, this message translates to:
  /// **'Wskazano Cię jako następcę: {album}'**
  String takeoverOfferTile(String album);

  /// No description provided for @takeoverNoOwner.
  ///
  /// In pl, this message translates to:
  /// **'Ten album nie ma właściciela. Nic z niego nie znika, a każdy uczestnik może go przejąć, opłacając plan.'**
  String get takeoverNoOwner;

  /// No description provided for @takeoverReserve.
  ///
  /// In pl, this message translates to:
  /// **'Przejmuję'**
  String get takeoverReserve;

  /// No description provided for @takeoverReservedByMe.
  ///
  /// In pl, this message translates to:
  /// **'Album czeka na Ciebie do {time}. Opłać plan albumu, aby go przejąć.'**
  String takeoverReservedByMe(String time);

  /// No description provided for @takeoverReservedByOther.
  ///
  /// In pl, this message translates to:
  /// **'Ktoś właśnie przejmuje ten album.'**
  String get takeoverReservedByOther;

  /// No description provided for @albumReportDeath.
  ///
  /// In pl, this message translates to:
  /// **'Zgłoś zgon właściciela'**
  String get albumReportDeath;

  /// S25
  ///
  /// In pl, this message translates to:
  /// **'Zgłoś zgon właściciela'**
  String get deathReportTitle;

  /// No description provided for @deathReportExplained.
  ///
  /// In pl, this message translates to:
  /// **'Dołącz zdjęcie aktu zgonu. Sprawdzi je obsługa GEO. Dokument widzi tylko obsługa, a po zamknięciu sprawy usuwamy go po 12 miesiącach.'**
  String get deathReportExplained;

  /// No description provided for @deathReportPick.
  ///
  /// In pl, this message translates to:
  /// **'Wybierz zdjęcie aktu zgonu'**
  String get deathReportPick;

  /// No description provided for @deathReportSend.
  ///
  /// In pl, this message translates to:
  /// **'Wyślij zgłoszenie'**
  String get deathReportSend;

  /// No description provided for @deathReportSent.
  ///
  /// In pl, this message translates to:
  /// **'Zgłoszenie wysłane. Obsługa sprawdzi dokument i da znać, jeśli nie będzie mogła go potwierdzić.'**
  String get deathReportSent;

  /// No description provided for @deathReportBack.
  ///
  /// In pl, this message translates to:
  /// **'Wróć do albumu'**
  String get deathReportBack;

  /// No description provided for @deathReportTaken.
  ///
  /// In pl, this message translates to:
  /// **'Zgłoszenie dotyczące tego albumu jest już sprawdzane.'**
  String get deathReportTaken;

  /// No description provided for @legacyContactTitle.
  ///
  /// In pl, this message translates to:
  /// **'Kontakt spadkowy'**
  String get legacyContactTitle;

  /// No description provided for @legacyContactExplained.
  ///
  /// In pl, this message translates to:
  /// **'Ta osoba po Twojej śmierci będzie mogła oglądać Twoje prywatne wspomnienia, tylko do odczytu. Nie dostanie reszty konta ani albumów. Bez wskazania nikt ich nie zobaczy, także spadkobiercy.'**
  String get legacyContactExplained;

  /// No description provided for @legacyContactCurrent.
  ///
  /// In pl, this message translates to:
  /// **'Wskazano: {email}'**
  String legacyContactCurrent(String email);

  /// No description provided for @legacyContactEmail.
  ///
  /// In pl, this message translates to:
  /// **'Adres e-mail tej osoby'**
  String get legacyContactEmail;

  /// No description provided for @legacyContactSave.
  ///
  /// In pl, this message translates to:
  /// **'Zapisz'**
  String get legacyContactSave;

  /// No description provided for @legacyContactRemove.
  ///
  /// In pl, this message translates to:
  /// **'Odwołaj wskazanie'**
  String get legacyContactRemove;

  /// No description provided for @legacyContactInvalid.
  ///
  /// In pl, this message translates to:
  /// **'Podaj poprawny adres innej osoby.'**
  String get legacyContactInvalid;

  /// No description provided for @legacyArchiveTitle.
  ///
  /// In pl, this message translates to:
  /// **'Prywatne wspomnienia'**
  String get legacyArchiveTitle;

  /// No description provided for @legacyArchiveSubtitle.
  ///
  /// In pl, this message translates to:
  /// **'Wskazano Cię jako kontakt spadkowy'**
  String get legacyArchiveSubtitle;

  /// No description provided for @legacyArchiveUnknown.
  ///
  /// In pl, this message translates to:
  /// **'Konto bez adresu'**
  String get legacyArchiveUnknown;

  /// No description provided for @legacyArchiveEmpty.
  ///
  /// In pl, this message translates to:
  /// **'Nie ma tu żadnych wspomnień.'**
  String get legacyArchiveEmpty;

  /// S18
  ///
  /// In pl, this message translates to:
  /// **'Plan i subskrypcja'**
  String get planTitle;

  /// No description provided for @planAlbumTitle.
  ///
  /// In pl, this message translates to:
  /// **'Plan albumu'**
  String get planAlbumTitle;

  /// No description provided for @planModeFree.
  ///
  /// In pl, this message translates to:
  /// **'Plan darmowy'**
  String get planModeFree;

  /// No description provided for @planModeActive.
  ///
  /// In pl, this message translates to:
  /// **'Plan Premium'**
  String get planModeActive;

  /// No description provided for @planModeReadOnly.
  ///
  /// In pl, this message translates to:
  /// **'Tylko do odczytu'**
  String get planModeReadOnly;

  /// No description provided for @planModeOverLimit.
  ///
  /// In pl, this message translates to:
  /// **'Nic nowego ponad darmowy limit'**
  String get planModeOverLimit;

  /// No description provided for @planModeTrimmed.
  ///
  /// In pl, this message translates to:
  /// **'Plan darmowy po przycięciu'**
  String get planModeTrimmed;

  /// No description provided for @planUsage.
  ///
  /// In pl, this message translates to:
  /// **'Zajęte miejsce: {used} z {limit}'**
  String planUsage(String used, String limit);

  /// No description provided for @planAiUsage.
  ///
  /// In pl, this message translates to:
  /// **'AI w tym miesiącu: {used} z {limit}'**
  String planAiUsage(int used, int limit);

  /// No description provided for @planPaidUntil.
  ///
  /// In pl, this message translates to:
  /// **'Opłacony do: {date}'**
  String planPaidUntil(String date);

  /// No description provided for @planReadOnlySince.
  ///
  /// In pl, this message translates to:
  /// **'Plan wygasł: {date}'**
  String planReadOnlySince(String date);

  /// No description provided for @planTrimAt.
  ///
  /// In pl, this message translates to:
  /// **'{date}: to, co przekracza {limit}, trafi do kosza na 30 dni.'**
  String planTrimAt(String date, String limit);

  /// No description provided for @planAlbumTrimAt.
  ///
  /// In pl, this message translates to:
  /// **'{date}: to, co przekracza {limit}, zostanie wyjęte z albumu. Autorzy zachowają swoje wspomnienia na własnych kontach.'**
  String planAlbumTrimAt(String date, String limit);

  /// No description provided for @planHeld.
  ///
  /// In pl, this message translates to:
  /// **'Przycięcie wstrzymane: ostrzeżenie do kogoś nie dotarło. Nic nie zostanie przycięte, dopóki nie dotrze.'**
  String get planHeld;

  /// No description provided for @planNothingLost.
  ///
  /// In pl, this message translates to:
  /// **'Nic nie zniknie do tej daty. Możesz wszystko oglądać, a po opłaceniu planu wszystko wróci do pełnych funkcji bez żadnego działania.'**
  String get planNothingLost;

  /// No description provided for @planChoose.
  ///
  /// In pl, this message translates to:
  /// **'Wybierz, co zostanie'**
  String get planChoose;

  /// No description provided for @planRefresh.
  ///
  /// In pl, this message translates to:
  /// **'Przywróć zakupy / odśwież'**
  String get planRefresh;

  /// No description provided for @planNoAccess.
  ///
  /// In pl, this message translates to:
  /// **'Ten plan widzi tylko osoba, która za niego płaci.'**
  String get planNoAccess;

  /// No description provided for @planChoiceClosed.
  ///
  /// In pl, this message translates to:
  /// **'Wybór materiału nie jest teraz otwarty.'**
  String get planChoiceClosed;

  /// S19
  ///
  /// In pl, this message translates to:
  /// **'Wybierz, co zostanie po przycięciu'**
  String get planMaterialTitle;

  /// No description provided for @planMaterialLimit.
  ///
  /// In pl, this message translates to:
  /// **'Wybrano {used} z darmowego limitu {limit}'**
  String planMaterialLimit(String used, String limit);

  /// No description provided for @planMaterialDeadline.
  ///
  /// In pl, this message translates to:
  /// **'Wybór możesz zmieniać do: {date}. Bez wyboru zostaje to, co najstarsze.'**
  String planMaterialDeadline(String date);

  /// No description provided for @planMaterialTooMuch.
  ///
  /// In pl, this message translates to:
  /// **'To za dużo: odznacz coś, aby zmieścić się w limicie.'**
  String get planMaterialTooMuch;

  /// No description provided for @planMaterialKeep.
  ///
  /// In pl, this message translates to:
  /// **'Zachowaj wybrane'**
  String get planMaterialKeep;

  /// No description provided for @planMaterialSaved.
  ///
  /// In pl, this message translates to:
  /// **'Wybór zapisany.'**
  String get planMaterialSaved;

  /// No description provided for @albumReadOnly.
  ///
  /// In pl, this message translates to:
  /// **'Plan albumu wygasł: album jest tylko do odczytu. Wszyscy dalej go oglądają, ale nic nowego nie można dodać.'**
  String get albumReadOnly;

  /// No description provided for @albumReadOnlyRefused.
  ///
  /// In pl, this message translates to:
  /// **'Plan tego albumu wygasł: nic nowego nie można dodać, dopóki nie zostanie opłacony.'**
  String get albumReadOnlyRefused;
}

class _AppLocalizationsDelegate extends LocalizationsDelegate<AppLocalizations> {
  const _AppLocalizationsDelegate();

  @override
  Future<AppLocalizations> load(Locale locale) {
    return SynchronousFuture<AppLocalizations>(lookupAppLocalizations(locale));
  }

  @override
  bool isSupported(Locale locale) => <String>['pl'].contains(locale.languageCode);

  @override
  bool shouldReload(_AppLocalizationsDelegate old) => false;
}

AppLocalizations lookupAppLocalizations(Locale locale) {
  // Lookup logic when only language code is specified.
  switch (locale.languageCode) {
    case 'pl':
      return AppLocalizationsPl();
  }

  throw FlutterError(
    'AppLocalizations.delegate failed to load unsupported locale "$locale". This is likely '
    'an issue with the localizations generation tool. Please file an issue '
    'on GitHub with a reproducible sample app and the gen-l10n configuration '
    'that was used.',
  );
}
