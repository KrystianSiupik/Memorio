// ignore: unused_import
import 'package:intl/intl.dart' as intl;

import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for English (`en`).
class AppLocalizationsEn extends AppLocalizations {
  AppLocalizationsEn([String locale = 'en']) : super(locale);

  @override
  String get tabMap => 'Map';

  @override
  String get tabAlbums => 'Albums';

  @override
  String get tabShared => 'Shared';

  @override
  String get tabAccount => 'Account';

  @override
  String get albumsTitle => 'Albums';

  @override
  String get sharedTitle => 'Shared with me';

  @override
  String get offlineSaved =>
      'You are offline. New memories and photos will be saved and sent when the connection is back.';

  @override
  String get noConnection => 'No connection';

  @override
  String get retry => 'Retry';

  @override
  String get updateTitle => 'Update Memorio';

  @override
  String get updateBody => 'This version of the app is no longer supported.';

  @override
  String updateBodyVersion(Object version) {
    return 'This version of the app is no longer supported. Install version $version or later.';
  }

  @override
  String get signInTitle => 'Sign in';

  @override
  String get registerTitle => 'Sign up';

  @override
  String get email => 'Email';

  @override
  String get password => 'Password';

  @override
  String get ageDeclaration => 'I am at least 16 years old';

  @override
  String get acceptTerms => 'I accept the terms';

  @override
  String get signIn => 'Sign in';

  @override
  String get createAccount => 'Create account';

  @override
  String get noAccount => 'I don\'t have an account';

  @override
  String get hasAccount => 'I already have an account';

  @override
  String get authOffline => 'No connection. Check your internet and try again.';

  @override
  String get invalidCredentials => 'Wrong email or password.';

  @override
  String get ageRequired => 'You must be at least 16 to create an account.';

  @override
  String get emailTaken => 'An account with this address already exists. Sign in.';

  @override
  String get invalidPassword => 'The password must have at least 10 characters.';

  @override
  String get invalidEmail => 'This does not look like a valid email address.';

  @override
  String get invalidProviderToken => 'Sign-in failed. Try again.';

  @override
  String get providerUnavailable => 'This way of signing in is unavailable right now.';

  @override
  String get serverError => 'Server error. Try again in a moment.';

  @override
  String get accountTitle => 'Account and privacy';

  @override
  String get signOut => 'Sign out';

  @override
  String get deletionEffects => 'What deleting removes';

  @override
  String effectMemories(Object count) {
    return 'Memories: $count';
  }

  @override
  String effectPhotos(Object count) {
    return 'Photos: $count';
  }

  @override
  String effectOwnAlbums(Object count, Object withoutSuccessor) {
    return 'Your albums: $count, without a successor: $withoutSuccessor';
  }

  @override
  String effectOtherAlbums(Object count) {
    return 'Other people\'s albums you take part in: $count';
  }

  @override
  String get legacyContact => 'Legacy contact';

  @override
  String get legacyContactNone => 'Nobody named.';

  @override
  String get legacyContactChoose => 'Name a legacy contact';

  @override
  String deletionDate(Object date) {
    return 'The account will be deleted on $date.';
  }

  @override
  String get cancelDeletion => 'Cancel deletion';

  @override
  String get understandDeletion => 'I understand what deleting the account does';

  @override
  String get albumsWithoutSuccessor =>
      'You have albums without a successor. Name a successor in the album or agree that they expire.';

  @override
  String get consentAlbumExpiry => 'I agree that albums without a successor expire';

  @override
  String get deletionWindowInfo => 'The account goes after 30 days. Until then you can change your mind.';

  @override
  String get deleteAccount => 'Delete account';

  @override
  String get noAccess => 'No access. Sign in again.';

  @override
  String get alreadyDecidedAccount => 'Already decided: the deletion window of the account has ended.';

  @override
  String get albumWithoutSuccessorError =>
      'You have albums without a successor. Name a successor or agree that they expire.';

  @override
  String get mapTitle => 'Map';

  @override
  String get mapLoadingPoints => 'Loading points…';

  @override
  String get mapEmpty => 'Add your first memory';

  @override
  String get mapAddMemory => 'Add a memory';

  @override
  String get mapOpenTrash => 'Trash';

  @override
  String get mapAttribution => '© OpenStreetMap';

  @override
  String get mapFilterDates => 'Dates';

  @override
  String mapFilterDatesRange(Object from, Object to) {
    return '$from – $to';
  }

  @override
  String get mapFilterPeople => 'People';

  @override
  String get mapFilterAlbums => 'Albums';

  @override
  String mapPendingUpload(Object count) {
    return 'Waiting to be sent: $count';
  }

  @override
  String get albumLink => 'Album link';

  @override
  String get albumLinkNone => 'The album has no working link';

  @override
  String get albumShareByLink => 'Share with a link';

  @override
  String get albumRevokeLink => 'Revoke the link (nobody new gets in)';

  @override
  String get albumCopyLink => 'Copy link';

  @override
  String get albumLinkCopied => 'Link copied';

  @override
  String get albumViewers => 'Viewing through the link';

  @override
  String get albumViewersEmpty => 'Nobody has come in through the link yet';

  @override
  String get albumRevokeAccess => 'Take access away';

  @override
  String get albumBlock => 'Block user';

  @override
  String get albumBlockNobody => 'There is nobody here you can block.';

  @override
  String get albumBlockConfirmTitle => 'Block this person?';

  @override
  String albumBlockConfirmBody(Object person) {
    return '$person loses access at once and can no longer come in, neither with a link nor with an invitation. The album owner can unblock them later.';
  }

  @override
  String get albumBlockConfirm => 'Block';

  @override
  String get cancel => 'Cancel';

  @override
  String get albumBlockedPeople => 'Blocked';

  @override
  String get albumUnblock => 'Unblock';

  @override
  String get albumBlocked => 'You have no access to this album.';

  @override
  String get albumLinkGone => 'This link no longer works.';

  @override
  String get albumOnlyOwnerBlocks => 'Only the owner can block someone in the album.';

  @override
  String get albumEntryTitle => 'Entering an album through a link';

  @override
  String albumEntryAlbum(Object name) {
    return 'Album: $name';
  }

  @override
  String get albumEntryOpen => 'Open album';

  @override
  String get albumEntryNamed => 'Your email address will appear on the list of people viewing this album.';

  @override
  String get albumReadOnlyViewer => 'You are viewing this album through a link.';

  @override
  String get sharedEmpty => 'Nobody has shared an album with you yet';

  @override
  String get albumMine => 'My albums';

  @override
  String get albumShared => 'Shared with me';

  @override
  String get albumInvitations => 'Invitations';

  @override
  String get albumNewName => 'Name of the new album';

  @override
  String get albumCreate => 'Create album';

  @override
  String get albumsEmpty => 'You have no albums yet';

  @override
  String get albumSharedEmpty => 'Nobody has shared an album with you';

  @override
  String albumMembersCount(Object members, Object memories) {
    return 'People: $members, memories: $memories';
  }

  @override
  String get albumNameRequired => 'Give the album a name.';

  @override
  String get albumNoAccess => 'You no longer have access to this album.';

  @override
  String get albumNoFreeSeats => 'The plan of this album has no free seat.';

  @override
  String get albumInvitationLimit => 'Today\'s limit of 10 invitations is used up. Try tomorrow.';

  @override
  String get albumAddressBlocked => 'This address does not accept invitations.';

  @override
  String get albumInvitationGone => 'The invitation expired or was withdrawn.';

  @override
  String get albumPoolFull => 'The album\'s space is full. The owner has been told.';

  @override
  String get albumOwnerCannotLeave => 'The owner cannot leave the album.';

  @override
  String get albumMemories => 'Memories';

  @override
  String get albumEmpty => 'The album is empty';

  @override
  String get albumMembers => 'Members';

  @override
  String get albumOwnerRole => 'owner';

  @override
  String get albumParticipantRole => 'participant';

  @override
  String albumMyContributions(Object memories, Object photos) {
    return 'My contributions: memories $memories, photos $photos';
  }

  @override
  String albumPoolLeft(Object left, Object total) {
    return 'Space left: $left of $total';
  }

  @override
  String get albumNoOwnerState => 'This album has an owner.';

  @override
  String get albumAddMemory => 'Add my memory';

  @override
  String get albumNoMemoriesToAdd => 'You have no memories to add yet.';

  @override
  String get albumAddPhotos => 'Add photos';

  @override
  String get albumTag => 'Tag a participant';

  @override
  String get albumRemoveMyTag => 'Remove my tag';

  @override
  String get albumLeave => 'Leave album';

  @override
  String get albumLeaveKeep => 'Leave my contributions in the album';

  @override
  String get albumLeaveTake => 'Take my contributions';

  @override
  String get albumLeaveConfirm => 'I confirm leaving the album';

  @override
  String get albumManage => 'Manage album';

  @override
  String get albumConsents => 'My consents and entrustments';

  @override
  String albumSeats(Object free, Object invited, Object taken) {
    return 'Seats: taken $taken, invited $invited, free $free';
  }

  @override
  String get albumInviteEmail => 'Email of the person to invite';

  @override
  String get albumInvite => 'Invite';

  @override
  String get albumInviteInvalid => 'This does not look like an email address.';

  @override
  String get albumInvited => 'Invited';

  @override
  String get albumWithdrawInvitation => 'Withdraw';

  @override
  String get albumRemoveParticipant => 'Remove participant';

  @override
  String get albumSuccessor => 'Successor';

  @override
  String mapPreviewPhotos(Object count) {
    return 'Photos: $count';
  }

  @override
  String get mapAddMemoryHere => 'Add a memory here';

  @override
  String get ownPlaceFromMap =>
      'The coordinates from the map are filled in: name this place (e.g. Warsaw) and add it. You can also search for a place by name above.';

  @override
  String get mapPreviewUnavailable => 'The preview could not be loaded.';

  @override
  String get reportContent => 'Report content';

  @override
  String get reportMemoryTitle => 'Report memory';

  @override
  String get reportAlbumTitle => 'Report album';

  @override
  String get reportExplained =>
      'The report goes to the Memorio team. The author will not learn who reported it. Illegal content is hidden at once and reviewed within 24 hours; other content stays visible until the decision, made within 7 days.';

  @override
  String get reportCategory => 'What is wrong?';

  @override
  String get reasonChildAbuse => 'Child abuse';

  @override
  String get reasonTerrorism => 'Terrorist content';

  @override
  String get reasonThreat => 'Threats';

  @override
  String get reasonIncitement => 'Incitement to violence or hatred';

  @override
  String get reasonOtherIllegal => 'Other illegal content';

  @override
  String get reasonSomeonesImage => 'My image without my consent';

  @override
  String get reasonPersonalData => 'My personal data';

  @override
  String get reasonDefamation => 'Defamation';

  @override
  String get reasonCopyright => 'Copyright infringement';

  @override
  String get reasonOther => 'Another reason';

  @override
  String get reportIllegalHidden =>
      'This content will be hidden as soon as you send the report, until the team decides.';

  @override
  String get reportExplanation => 'Explanation';

  @override
  String get reportSend => 'Report';

  @override
  String get reportSent =>
      'Thank you. You will see the decision and its reasons on the \"My reports and decisions\" screen.';

  @override
  String get reportSentIllegal =>
      'Thank you. The content is already hidden. You will see the decision and its reasons on the \"My reports and decisions\" screen.';

  @override
  String get reportBack => 'Back';

  @override
  String get reportNoAccess => 'You no longer see this content.';

  @override
  String get reportOwnContent => 'You can change or delete your own content yourself.';

  @override
  String get myModerationTitle => 'My reports and decisions';

  @override
  String get myModerationEmpty => 'You have no reports or decisions';

  @override
  String get myModerationAboutMine => 'Decisions about my content';

  @override
  String get myModerationMine => 'My reports';

  @override
  String myModerationReported(Object date, Object reason) {
    return '$reason, reported $date';
  }

  @override
  String get myModerationHidden => 'Right now only you see it.';

  @override
  String myModerationStatement(Object statement) {
    return 'Reasons: $statement';
  }

  @override
  String get myModerationAppealPending => 'The appeal is waiting for a decision.';

  @override
  String get myModerationAppealReversed => 'Appeal accepted: the content is visible again.';

  @override
  String get myModerationAppealUpheld => 'Appeal not accepted.';

  @override
  String get myModerationReporterAppealReversed => 'Appeal accepted: the content has been hidden.';

  @override
  String get reportSuspended =>
      'We are not accepting your reports right now: three of them within 90 days were manifestly unfounded.';

  @override
  String get myModerationAppealReason => 'Why the decision is wrong';

  @override
  String myModerationAppealUntil(Object date) {
    return 'You can appeal until $date.';
  }

  @override
  String get myModerationAppeal => 'Appeal';

  @override
  String get verdictRemoved => 'Decision: content hidden';

  @override
  String get verdictKept => 'Decision: content stays';

  @override
  String get verdictPending => 'Waiting for a decision';

  @override
  String get albumHandover => 'Handing the album over';

  @override
  String get albumHandoverExplain =>
      'You can hand the album over to a participant right now. They become the owner, and you stay in the album as a participant.';

  @override
  String get albumHandOver => 'Hand over album';

  @override
  String albumHandoverTo(Object person) {
    return 'Hand the album over to $person?';
  }

  @override
  String get albumHandoverQuestion =>
      'What about your own memories in this album? Decide now: they stay in the album, or we take them out when this person accepts. You keep them either way.';

  @override
  String get albumHandoverKeep => 'They stay in the album';

  @override
  String get albumHandoverTake => 'I take them';

  @override
  String albumHandoverWaiting(Object date, Object person) {
    return 'Waiting for $person to answer, until $date.';
  }

  @override
  String get albumHandoverWithdraw => 'Withdraw the offer';

  @override
  String albumHandoverForMe(Object date) {
    return 'The owner wants to hand this album over to you. You can answer until $date.';
  }

  @override
  String get albumHandoverOwnerKeeps => 'Their memories stay in the album.';

  @override
  String get albumHandoverOwnerTakes => 'Their memories will be taken out of the album.';

  @override
  String get albumHandoverAccept => 'I take the album over';

  @override
  String get albumHandoverRefuse => 'I decline';

  @override
  String get albumNoSuccessor => 'No successor named';

  @override
  String get albumNameSuccessor => 'Name as successor';

  @override
  String get albumCurator => 'Curator';

  @override
  String get albumNoCurator => 'Nothing entrusted to a curator';

  @override
  String get albumEntrust => 'Entrust to curator';

  @override
  String get albumRevokeEntrustment => 'Revoke the entrustment';

  @override
  String get albumKept => 'My contributions left after leaving';

  @override
  String get albumKeptEmpty => 'You have no contributions left after leaving';

  @override
  String albumKeptItem(Object album, Object memories) {
    return '$album: memories $memories';
  }

  @override
  String get albumKeptNotChosen => 'You were removed from the album. Choose whether your content stays.';

  @override
  String get albumKeptConfirm => 'I know the content will disappear from the album';

  @override
  String get albumRevokeKept => 'Revoke the \"leave it\" consent';

  @override
  String get albumEntrusted => 'Entrusted content';

  @override
  String get albumEntrustedEmpty => 'Nobody has entrusted content to you';

  @override
  String get albumArrange => 'Arrange';

  @override
  String get albumMoveUp => 'Up';

  @override
  String get albumMoveDown => 'Down';

  @override
  String get albumInvitationTitle => 'Album invitation';

  @override
  String albumInvitedBy(Object person) {
    return 'Invited by: $person';
  }

  @override
  String albumInvitationAlbum(Object name) {
    return 'Album: $name';
  }

  @override
  String get albumAccept => 'Accept';

  @override
  String get albumDecline => 'Decline';

  @override
  String get albumBlockInvitations => 'I don\'t want invitations from GEO';

  @override
  String get mapEmptyFiltered => 'No memories in these dates';

  @override
  String mapRejected(Object count) {
    return 'Could not be sent: $count';
  }

  @override
  String get mapRejectedDismiss => 'Got it';

  @override
  String get newMemoryTitle => 'New memory';

  @override
  String get newMemoryStepPlace => 'Place and date';

  @override
  String get newMemoryStepContent => 'Content';

  @override
  String get next => 'Next';

  @override
  String get back => 'Back';

  @override
  String get place => 'Place';

  @override
  String placeChosen(Object name) {
    return 'Chosen: $name';
  }

  @override
  String get placeChange => 'Change place';

  @override
  String get placeRecent => 'Recently used';

  @override
  String get ownPlaceName => 'Own name of the place';

  @override
  String get ownPlaceLatitude => 'Latitude';

  @override
  String get ownPlaceLongitude => 'Longitude';

  @override
  String get ownPlaceAdd => 'Add own place';

  @override
  String get ownPlaceInvalid => 'Give a name, a latitude from -90 to 90 and a longitude from -180 to 180.';

  @override
  String get datePrecision => 'Date';

  @override
  String get precisionDay => 'Day';

  @override
  String get precisionMonth => 'Month';

  @override
  String get precisionYear => 'Year';

  @override
  String get precisionPeriod => 'Period';

  @override
  String get dateChoose => 'Choose a date';

  @override
  String get dateChooseEnd => 'Choose the end of the period';

  @override
  String dateChosen(Object date) {
    return 'From: $date';
  }

  @override
  String dateChosenEnd(Object date) {
    return 'To: $date';
  }

  @override
  String get timeChoose => 'Add a time (optional)';

  @override
  String timeChosen(Object time) {
    return 'Time: $time';
  }

  @override
  String get timeClear => 'No time';

  @override
  String get dateInFuture => 'A memory cannot be in the future.';

  @override
  String get photos => 'Photos';

  @override
  String get photosAdd => 'Add photos';

  @override
  String photosPicked(Object count) {
    return 'Photos chosen: $count';
  }

  @override
  String get description => 'Description';

  @override
  String get aiDraft => 'AI draft';

  @override
  String get aiConsent => 'I agree to send the description to the AI provider, without the place name';

  @override
  String get aiRequest => 'Ask AI for a draft';

  @override
  String get aiMarker => 'AI text';

  @override
  String get aiUnavailableOffline => 'The AI draft does not work offline.';

  @override
  String get aiUseDraft => 'Use the draft';

  @override
  String get approveDescription => 'Approve description';

  @override
  String get descriptionApproved => 'Description approved';

  @override
  String get saveMemory => 'Save memory';

  @override
  String get savedOffline => 'Saved. We will send it when the connection is back.';

  @override
  String get offlineNotOnWeb => 'In the browser you cannot save without a connection.';

  @override
  String get termsRequired => 'Accept the terms before your first memory.';

  @override
  String get memoryNoAccess => 'You have no access to this memory.';

  @override
  String get alreadyDecided => 'Already decided. Refresh and check the current state.';

  @override
  String get placeMissing => 'This place no longer exists. Choose another one.';

  @override
  String get spaceFull => 'Your space is full. This photo will not fit.';

  @override
  String get aiLimitReached => 'This month\'s AI draft limit is used up.';

  @override
  String get aiProviderDown => 'The AI provider is unavailable right now. Try later.';

  @override
  String get invalidInput => 'Check what you entered.';

  @override
  String get memoryTitle => 'Memory';

  @override
  String memoryWhen(Object date) {
    return 'Date: $date';
  }

  @override
  String memoryPlace(Object name) {
    return 'Place: $name';
  }

  @override
  String get memoryContents => 'Contents';

  @override
  String get memoryAuthorMe => 'Author: you';

  @override
  String get memoryPeople => 'People';

  @override
  String get memoryPeopleNone => 'Nobody is tagged';

  @override
  String get memoryNoDescription => 'No description';

  @override
  String get memoryGone => 'You no longer have access to this memory';

  @override
  String get memoryBack => 'Back';

  @override
  String get deleteMine => 'Delete my content';

  @override
  String get deleteForGoodConfirm => 'I understand this cannot be undone';

  @override
  String get deleteForGood => 'Delete for good (formal request)';

  @override
  String get trashTitle => 'Trash';

  @override
  String get trashEmpty => 'The trash is empty';

  @override
  String trashItem(Object date, Object place) {
    return '$place, $date';
  }

  @override
  String trashDeletedAt(Object date) {
    return 'Deleted: $date';
  }

  @override
  String trashDaysLeft(Object count) {
    return 'Days left: $count';
  }

  @override
  String trashPhotos(Object count) {
    return 'Photos: $count';
  }

  @override
  String get restore => 'Restore';

  @override
  String get replayTitle => 'Replay';

  @override
  String replayPeriod(String period) {
    return 'Period: $period';
  }

  @override
  String get replayOtherRange => 'Another period';

  @override
  String get replayEmpty => 'There are no memories in this period';

  @override
  String replayStep(int current, int total) {
    return '$current of $total';
  }

  @override
  String get replayPrevious => 'Previous';

  @override
  String get replayNext => 'Next';

  @override
  String get replayPlay => 'Play';

  @override
  String get replayPause => 'Pause';

  @override
  String replayTempo(int speed) {
    return 'Speed $speed×';
  }

  @override
  String get replayEnd => 'That\'s everything from this period';

  @override
  String replayStats(int memories, int places) {
    return 'Memories: $memories, places: $places';
  }

  @override
  String get replayAgain => 'From the start';

  @override
  String get replayCreateAlbum => 'Create an album from the period';

  @override
  String get replayCreateAlbumExplained =>
      'Your memories from this period go into the album. Other people\'s stay in their albums.';

  @override
  String replayAlbumName(String period) {
    return 'Replay $period';
  }

  @override
  String replayAlbumCreated(int count) {
    return 'The album is ready, memories added: $count.';
  }

  @override
  String replayAlbumNotFitting(int count) {
    return 'Did not fit into the album\'s space: $count.';
  }

  @override
  String get replayOpenAlbum => 'Open album';

  @override
  String get replayTooLong => 'This period has too many memories for one album. Choose a shorter one.';

  @override
  String get takeoverTitle => 'Taking an album over';

  @override
  String takeoverDeadline(String date) {
    return 'You have until $date to decide';
  }

  @override
  String takeoverFounder(int count) {
    return 'The founder\'s memories in the album: $count. They stay in it.';
  }

  @override
  String get takeoverExplained =>
      'If you take the album over, you become its owner, together with its space. If you decline or do not answer in time, the album is left without an owner, and another participant can take it over.';

  @override
  String get takeoverStoreAdvice =>
      'The family of the person who died should cancel their subscription in the store: the app cannot do that. The album works until the end of the paid period.';

  @override
  String get takeoverAccept => 'I take the album over';

  @override
  String get takeoverRefuse => 'I decline';

  @override
  String get takeoverNotSuccessor => 'You are not the successor named.';

  @override
  String get takeoverDecided => 'This case is already decided.';

  @override
  String takeoverOfferTile(String album) {
    return 'You were named as successor: $album';
  }

  @override
  String get takeoverNoOwner =>
      'This album has no owner. Nothing disappears from it, and any participant can take it over by paying for the plan.';

  @override
  String get takeoverReserve => 'Take over';

  @override
  String takeoverReservedByMe(String time) {
    return 'The album waits for you until $time. Pay for the album plan to take it over.';
  }

  @override
  String get takeoverReservedByOther => 'Someone is taking this album over right now.';

  @override
  String get albumReportDeath => 'Report the owner\'s death';

  @override
  String get deathReportTitle => 'Report the owner\'s death';

  @override
  String get deathReportExplained =>
      'Attach a photo of the death certificate. The GEO team checks it. Only the team sees the document, and we delete it 12 months after the case is closed.';

  @override
  String get deathReportPick => 'Choose a photo of the death certificate';

  @override
  String get deathReportSend => 'Send report';

  @override
  String get deathReportSent =>
      'Report sent. The team will check the document and let you know if they cannot confirm it.';

  @override
  String get deathReportBack => 'Back to the album';

  @override
  String get deathReportTaken => 'A report about this album is already being checked.';

  @override
  String get legacyContactTitle => 'Legacy contact';

  @override
  String get legacyContactExplained =>
      'After your death this person will be able to view your private memories, read only. They do not get the rest of the account or your albums. Without one, nobody will see them, not even your heirs.';

  @override
  String legacyContactCurrent(String email) {
    return 'Named: $email';
  }

  @override
  String get legacyContactEmail => 'Email address of this person';

  @override
  String get legacyContactSave => 'Save';

  @override
  String get legacyContactRemove => 'Remove the contact';

  @override
  String get legacyContactInvalid => 'Give a valid address of another person.';

  @override
  String get legacyArchiveTitle => 'Private memories';

  @override
  String get legacyArchiveSubtitle => 'You were named as a legacy contact';

  @override
  String get legacyArchiveUnknown => 'Account without an address';

  @override
  String get legacyArchiveEmpty => 'There are no memories here.';

  @override
  String get planTitle => 'Plan and subscription';

  @override
  String get planAlbumTitle => 'Album plan';

  @override
  String get planModeFree => 'Free plan';

  @override
  String get planModeActive => 'Premium plan';

  @override
  String get planModeReadOnly => 'Read only';

  @override
  String get planModeOverLimit => 'Nothing new above the free limit';

  @override
  String get planModeTrimmed => 'Free plan after trimming';

  @override
  String planUsage(String used, String limit) {
    return 'Space used: $used of $limit';
  }

  @override
  String planAiUsage(int used, int limit) {
    return 'AI this month: $used of $limit';
  }

  @override
  String planPaidUntil(String date) {
    return 'Paid until: $date';
  }

  @override
  String planReadOnlySince(String date) {
    return 'The plan ended: $date';
  }

  @override
  String planTrimAt(String date, String limit) {
    return '$date: what goes over $limit moves to the trash for 30 days.';
  }

  @override
  String planAlbumTrimAt(String date, String limit) {
    return '$date: what goes over $limit is taken out of the album. Authors keep their memories in their own accounts.';
  }

  @override
  String get planHeld => 'Trimming is on hold: a warning did not reach someone. Nothing is trimmed until it does.';

  @override
  String get planNothingLost =>
      'Nothing disappears before this date. You can view everything, and once the plan is paid everything works fully again without you doing anything.';

  @override
  String get planChoose => 'Choose what stays';

  @override
  String get planRefresh => 'Restore purchases / refresh';

  @override
  String get planNoAccess => 'Only the person who pays for this plan can see it.';

  @override
  String get planChoiceClosed => 'Choosing what stays is not open right now.';

  @override
  String get planMaterialTitle => 'Choose what stays after trimming';

  @override
  String planMaterialLimit(String used, String limit) {
    return 'Chosen $used of the free limit $limit';
  }

  @override
  String planMaterialDeadline(String date) {
    return 'You can change the choice until $date. Without a choice the oldest stays.';
  }

  @override
  String get planMaterialTooMuch => 'That\'s too much: unselect something to fit in the limit.';

  @override
  String get planMaterialKeep => 'Keep the chosen';

  @override
  String get planMaterialSaved => 'Choice saved.';

  @override
  String get albumReadOnly =>
      'The album plan ended: the album is read only. Everyone can still view it, but nothing new can be added.';

  @override
  String get albumReadOnlyRefused => 'The plan of this album ended: nothing new can be added until it is paid again.';

  @override
  String get settingsTitle => 'Settings';

  @override
  String get settingsLanguage => 'Language';

  @override
  String get settingsLanguageSystem => 'As on the device';
}
