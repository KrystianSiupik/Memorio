using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.Account.Api;

/// <summary>
/// Public web page on deleting an account (S3:L19: in the app and on the web), required by the app
/// stores. It explains the steps; the deletion itself is requested while signed in (MEM-E1-KONTO-03).
/// Outside /v1 and the API contract; anonymous (SD10 allowlist).
/// </summary>
internal static class AccountDeletionPage
{
    public const string Path = "/account/delete";

    public static void MapAccountDeletionPage(this IEndpointRouteBuilder root) =>
        root.MapGet(Path, () => Results.Content(Html, "text/html; charset=utf-8"))
            .AllowAnonymous()
            .ExcludeFromDescription();

    private const string Html = """
        <!doctype html>
        <html lang="pl">
        <head>
          <meta charset="utf-8">
          <meta name="viewport" content="width=device-width, initial-scale=1">
          <title>Memorio: usuwanie konta</title>
          <style>
            body { font-family: system-ui, sans-serif; max-width: 40rem; margin: 2rem auto; padding: 0 1rem; line-height: 1.5; }
          </style>
        </head>
        <body>
          <h1>Usuwanie konta Memorio</h1>
          <p>Konto usuniesz w aplikacji (na telefonie albo w przeglądarce):</p>
          <ol>
            <li>Zaloguj się do Memorio.</li>
            <li>Otwórz <strong>Konto i prywatność</strong>.</li>
            <li>Wybierz <strong>Usuń konto</strong>. Zobaczysz, które treści zostaną usunięte, i potwierdzisz.</li>
          </ol>
          <p>Po potwierdzeniu masz 30 dni na zmianę zdania: w tym czasie możesz cofnąć usunięcie.
             Po 30 dniach konto i Twoje treści zostają usunięte na stałe.</p>
        </body>
        </html>
        """;
}
