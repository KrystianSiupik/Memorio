using Memorio.Modules.Notifications.Application.Ports;

namespace Memorio.Modules.Notifications.Application;

/// <summary>What a message says. The name of the album and, for an invitation, who invites, for a warning, when: never a memory or a photo (D-038, D-268).</summary>
internal static class NotificationTexts
{
    public static (string Subject, string Body) Of(NotificationKind kind, string? albumName, string? appUrl, string? detail = null)
    {
        var inviter = string.IsNullOrWhiteSpace(detail) ? null : OneLine(detail);
        var album = string.IsNullOrWhiteSpace(albumName) ? "album" : $"album „{OneLine(albumName)}”";
        var (subject, body) = kind switch
        {
            NotificationKind.Invitation => (
                "Zaproszenie do albumu w GEO",
                $"{(inviter is null ? "Zaproszono Cię" : $"{inviter} zaprasza Cię")} do wspólnego albumu: {album}.\n\nZaloguj się albo załóż konto na ten adres e-mail, a zobaczysz zaproszenie i zdecydujesz, czy je przyjąć. Na ekranie zaproszenia możesz też wyłączyć kolejne zaproszenia z GEO."),
            NotificationKind.InvitationReminder => (
                "Przypomnienie o zaproszeniu do albumu w GEO",
                $"{(inviter is null ? "Zaproszenie czeka" : $"Zaproszenie od {inviter} czeka")} na Twoją odpowiedź: {album}.\n\nTo jedyne przypomnienie. Jeśli nie chcesz odpowiadać, nic więcej nie musisz robić."),
            NotificationKind.PoolFull => (
                "Pula albumu w GEO jest pełna",
                $"Coś się nie zmieściło: {album}.\n\nAby uczestnicy mogli dodawać dalej, zwolnij miejsce albo wybierz większy plan."),
            NotificationKind.ChooseKeepOrTake => (
                "Usunięto Cię z albumu w GEO: wybierz, co z Twoimi treściami",
                $"Właściciel usunął Cię z albumu: {album}.\n\nTwoje treści na razie w nim zostają. Możesz je zabrać w każdej chwili na ekranie „Moje zgody i powierzenia”."),
            NotificationKind.Tagged => (
                "Oznaczono Cię we wspomnieniu w GEO",
                $"Oznaczono Cię we wspomnieniu: {album}.\n\nJeśli nie chcesz tego oznaczenia, możesz je usunąć jednym kliknięciem w albumie."),
            NotificationKind.TrimWarningAlbum => (
                $"Album w GEO zostanie przycięty za {OneLine(detail ?? "")}",
                $"Plan albumu wygasł: {album}.\n\nZa {OneLine(detail ?? "")} to, co przekracza darmowy limit, zostanie wyjęte z albumu. Autorzy zachowają swoje wspomnienia na własnych kontach. Właściciel może wybrać, co zostanie w albumie, albo odnowić plan: wtedy album zostanie bez zmian."),
            NotificationKind.TrimWarningAccount => (
                $"Twoje archiwum w GEO zostanie przycięte za {OneLine(detail ?? "")}",
                $"Twój plan wygasł.\n\nZa {OneLine(detail ?? "")} to, co przekracza darmowy limit, trafi do kosza na 30 dni, a potem zniknie. Możesz wybrać, co zostanie, albo odnowić plan: wtedy nic nie zniknie."),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };
        return (subject, string.IsNullOrWhiteSpace(appUrl) ? body : $"{body}\n\nOtwórz GEO: {appUrl}");
    }

    private static string OneLine(string text) => string.Join(' ', text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
}
