using System.Net;
using Memorio.Modules.Moderation.Application.Commands.Reports;
using Memorio.Modules.Moderation.Domain.Reports;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.Moderation.Api;

/// <summary>
/// S21: reporting without an account (S2:C2, DSA art. 16, L10), for someone who found themselves or their work in what
/// somebody shared. A plain form outside /v1 and the API contract; anonymous (SD10 allowlist), limited per address by
/// nginx. Content is not named by id here: the person says where it is, and the service finds it.
/// </summary>
internal static class PublicReportPage
{
    public const string Path = "/report";

    private static readonly (string Value, string Label)[] Reasons =
    [
        (nameof(ReportReason.ChildAbuse), "Wykorzystywanie dzieci"),
        (nameof(ReportReason.Terrorism), "Treści terrorystyczne"),
        (nameof(ReportReason.Threat), "Groźby"),
        (nameof(ReportReason.Incitement), "Nawoływanie do przemocy lub nienawiści"),
        (nameof(ReportReason.OtherIllegal), "Inna treść niezgodna z prawem"),
        (nameof(ReportReason.SomeonesImage), "Mój wizerunek bez mojej zgody"),
        (nameof(ReportReason.PersonalData), "Moje dane osobowe"),
        (nameof(ReportReason.Defamation), "Zniesławienie"),
        (nameof(ReportReason.Copyright), "Naruszenie praw autorskich"),
        (nameof(ReportReason.Other), "Inny powód"),
    ];

    public static void MapPublicReportPage(this IEndpointRouteBuilder root)
    {
        root.MapGet(Path, () => Results.Content(Form, "text/html; charset=utf-8"))
            .AllowAnonymous()
            .ExcludeFromDescription();
        root.MapPost(Path, SubmitAsync)
            .AllowAnonymous()
            .ExcludeFromDescription();
    }

    private static async Task<IResult> SubmitAsync(HttpRequest request, SubmitPublicReportHandler handler, CancellationToken cancellationToken)
    {
        if (!request.HasFormContentType)
        {
            return Results.Content(Page("Nie udało się wysłać zgłoszenia", "<p>Wyślij zgłoszenie z formularza.</p>"), "text/html; charset=utf-8", statusCode: 400);
        }

        var form = await request.ReadFormAsync(cancellationToken);
        if (form["goodFaith"] != "yes")
        {
            return Invalid();
        }

        var id = Guid.TryParse(form["id"], out var parsed) ? parsed : Guid.CreateVersion7();
        var reported = await handler.HandleAsync(
            new SubmitPublicReport(new ReportId(id), form["email"], form["location"], form["reason"], form["explanation"]),
            cancellationToken);
        return reported.IsFailure
            ? Invalid()
            : Results.Content(
                Page(
                    "Zgłoszenie przyjęte",
                    $"<p>Dziękujemy. Numer zgłoszenia: <code>{WebUtility.HtmlEncode(reported.Value.Value.ToString())}</code>.</p>"
                    + "<p>Potwierdzenie i decyzję z uzasadnieniem wyślemy na podany adres e-mail. Treści niezgodne z prawem rozpatrujemy w ciągu 24 godzin, pozostałe w ciągu 7 dni.</p>"),
                "text/html; charset=utf-8");
    }

    private static IResult Invalid() =>
        Results.Content(
            Page("Nie udało się wysłać zgłoszenia", "<p>Podaj adres e-mail, wskaż, gdzie jest treść, wybierz powód, opisz go i potwierdź oświadczenie. <a href=\"/report\">Wróć do formularza</a>.</p>"),
            "text/html; charset=utf-8",
            statusCode: 400);

    private static string Form =>
        Page(
            "Zgłoś treść w Memorio",
            $$"""
            <p>Zgłoszenie możesz wysłać bez konta. Treści niezgodne z prawem ukrywamy od razu i rozpatrujemy w ciągu 24 godzin,
               pozostałe zostają widoczne do decyzji, którą podejmujemy w ciągu 7 dni. Decyzję z uzasadnieniem wyślemy na Twój adres.</p>
            <form method="post" action="/report">
              <input type="hidden" name="id" value="{{Guid.CreateVersion7()}}">
              <p><label>Adres e-mail<br><input type="email" name="email" required maxlength="320"></label></p>
              <p><label>Gdzie jest ta treść (link albo opis: album, miejsce, data)<br><textarea name="location" required maxlength="{{Report.MaximumLocationLength}}" rows="3"></textarea></label></p>
              <p><label>Powód<br><select name="reason" required>{{string.Concat(Reasons.Select(reason => $"<option value=\"{reason.Value}\">{reason.Label}</option>"))}}</select></label></p>
              <p><label>Uzasadnienie<br><textarea name="explanation" required maxlength="{{Report.MaximumExplanationLength}}" rows="6"></textarea></label></p>
              <p><label><input type="checkbox" name="goodFaith" value="yes" required> Oświadczam w dobrej wierze, że informacje w zgłoszeniu są prawdziwe i kompletne.</label></p>
              <p><button type="submit">Wyślij zgłoszenie</button></p>
            </form>
            """);

    private static string Page(string title, string body) => $$"""
        <!doctype html>
        <html lang="pl">
        <head>
          <meta charset="utf-8">
          <meta name="viewport" content="width=device-width, initial-scale=1">
          <title>{{title}}</title>
          <style>
            body { font-family: system-ui, sans-serif; max-width: 40rem; margin: 2rem auto; padding: 0 1rem; line-height: 1.5; }
            input[type=email], textarea, select { width: 100%; box-sizing: border-box; }
          </style>
        </head>
        <body>
          <h1>{{title}}</h1>
          {{body}}
        </body>
        </html>
        """;
}
