using System.Security.Claims;
using Memorio.Modularity;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Succession.Application.Commands.Cases;
using Memorio.Modules.Succession.Application.Commands.NameSuccessor;
using Memorio.Modules.Succession.Application.Queries;
using Memorio.Modules.Succession.Contracts;
using Memorio.Modules.Succession.Domain.Cases;
using Memorio.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Memorio.Modules.Succession.Api;

internal static class SuccessionEndpoints
{
    public static void MapSuccessionEndpoints(this IEndpointRouteBuilder v1)
    {
        v1.MapPut("/albums/{id:guid}/successor", NameAsync)
            .WithTags("Albums")
            .WithName("NameSuccessor")
            .WithSummary("The owner names someone in the album as its successor.");
        v1.MapPost("/albums/{id:guid}/death-reports", ReportAsync)
            .WithTags("Succession")
            .WithName("ReportOwnerDeath")
            .WithSummary("Someone in the album reports the death of its owner, with the certificate; the service checks it.");

        var succession = v1.MapGroup("/succession").WithTags("Succession");
        succession.MapPost("/uploads", StartUploadAsync).WithName("StartCertificateUpload").WithSummary("An address to send one certificate to, valid for 5 minutes.");
        succession.MapGet("/offers", OffersAsync).WithName("ListTakeoverOffers").WithSummary("Albums waiting for the person's answer as their successor.");
        succession.MapGet("/offers/{id:guid}", OfferAsync).WithName("GetTakeoverOffer").WithSummary("For the successor named: the album, the time to answer and what the founder brought.");
        succession.MapPost("/offers/{id:guid}/acceptance", AcceptAsync).WithName("AcceptTakeover").WithSummary("The successor takes the album over.");
        succession.MapPost("/offers/{id:guid}/refusal", RefuseAsync).WithName("RefuseTakeover").WithSummary("The successor refuses; the album is left without an owner.");

        var contact = v1.MapGroup("/legacy-contact").WithTags("Succession");
        contact.MapGet("/", GetContactAsync).WithName("GetLegacyContact").WithSummary("Who may read the person's private memories after their death.");
        contact.MapPut("/", NameContactAsync).WithName("NameLegacyContact").WithSummary("Names that person, or replaces them.");
        contact.MapDelete("/", RemoveContactAsync).WithName("RemoveLegacyContact").WithSummary("Nobody will get it; asking again changes nothing.");
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> NameAsync(Guid id, NameSuccessorRequest request, ClaimsPrincipal principal, NameSuccessorHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new NameSuccessor(id, CurrentAccount.IdOf(principal), request.AccountId), cancellationToken));

    private static async Task<Results<NoContent, ProblemHttpResult>> ReportAsync(Guid id, ReportDeathRequest request, ClaimsPrincipal principal, ReportDeathHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new ReportDeath(new TakeoverCaseId(request.Id), id, CurrentAccount.IdOf(principal), request.UploadKey), cancellationToken));

    private static async Task<Results<Ok<CertificateUploadResponse>, ProblemHttpResult>> StartUploadAsync(StartCertificateUploadRequest request, StartCertificateUploadHandler handler, CancellationToken cancellationToken)
    {
        var slot = await handler.HandleAsync(request.ContentType, cancellationToken);
        return slot.IsFailure
            ? Problems.From(slot.Error)
            : TypedResults.Ok(new CertificateUploadResponse(slot.Value.Key.Value, slot.Value.Upload.Url, slot.Value.ContentType));
    }

    private static async Task<Ok<IReadOnlyList<TakeoverOfferResponse>>> OffersAsync(ClaimsPrincipal principal, ITakeoverQueries queries, IAlbumNames names, IAlbumSuccession albums, TimeProvider clock, CancellationToken cancellationToken)
    {
        var rows = await queries.OpenOffersOfAsync(CurrentAccount.IdOf(principal), clock.GetUtcNow(), cancellationToken);
        var known = await names.NamesOfAsync([.. rows.Select(row => row.AlbumId)], cancellationToken);
        var offers = new List<TakeoverOfferResponse>();
        foreach (var row in rows)
        {
            offers.Add(await ResponseOfAsync(row, known, albums, cancellationToken));
        }

        return TypedResults.Ok<IReadOnlyList<TakeoverOfferResponse>>(offers);
    }

    private static async Task<Results<Ok<TakeoverOfferResponse>, ProblemHttpResult>> OfferAsync(Guid id, ClaimsPrincipal principal, ITakeoverQueries queries, IAlbumNames names, IAlbumSuccession albums, TimeProvider clock, CancellationToken cancellationToken)
    {
        var row = await queries.FindAsync(id, cancellationToken);
        if (row is null || row.SuccessorAccountId != CurrentAccount.IdOf(principal))
        {
            return Problems.From(CaseErrors.NotSuccessor);
        }

        if (row.State != nameof(CaseState.Offered) || row.AnswerBy <= clock.GetUtcNow())
        {
            return Problems.From(CaseErrors.AlreadyDecided);
        }

        return TypedResults.Ok(await ResponseOfAsync(row, await names.NamesOfAsync([row.AlbumId], cancellationToken), albums, cancellationToken));
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> AcceptAsync(Guid id, ClaimsPrincipal principal, AnswerOfferHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new AnswerOffer(new TakeoverCaseId(id), CurrentAccount.IdOf(principal), true), cancellationToken));

    private static async Task<Results<NoContent, ProblemHttpResult>> RefuseAsync(Guid id, ClaimsPrincipal principal, AnswerOfferHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(new AnswerOffer(new TakeoverCaseId(id), CurrentAccount.IdOf(principal), false), cancellationToken));

    private static async Task<Ok<LegacyContactResponse>> GetContactAsync(ClaimsPrincipal principal, ILegacyContacts contacts, CancellationToken cancellationToken) =>
        TypedResults.Ok(new LegacyContactResponse(await contacts.ContactOfAsync(CurrentAccount.IdOf(principal), cancellationToken)));

    private static async Task<Results<NoContent, ProblemHttpResult>> NameContactAsync(NameLegacyContactRequest request, ClaimsPrincipal principal, NameLegacyContactHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.HandleAsync(CurrentAccount.IdOf(principal), request.Email, cancellationToken));

    private static async Task<Results<NoContent, ProblemHttpResult>> RemoveContactAsync(ClaimsPrincipal principal, NameLegacyContactHandler handler, CancellationToken cancellationToken) =>
        Answer(await handler.RemoveAsync(CurrentAccount.IdOf(principal), cancellationToken));

    private static async Task<TakeoverOfferResponse> ResponseOfAsync(TakeoverOfferRow row, IReadOnlyDictionary<Guid, string> names, IAlbumSuccession albums, CancellationToken cancellationToken) =>
        new(row.Id, row.AlbumId, names.GetValueOrDefault(row.AlbumId, ""), row.AnswerBy!.Value, await albums.MemoriesOfAsync(row.AlbumId, row.OwnerAccountId, cancellationToken));

    private static Results<NoContent, ProblemHttpResult> Answer(Result result) =>
        result.IsSuccess ? TypedResults.NoContent() : Problems.From(result.Error);
}

internal sealed record NameSuccessorRequest(Guid AccountId);

internal sealed record ReportDeathRequest(Guid Id, string? UploadKey);

internal sealed record StartCertificateUploadRequest(string? ContentType);

internal sealed record CertificateUploadResponse(string Key, Uri Url, string ContentType);

/// <param name="FounderMemories">What the founder brought: it stays in the album (S2:B3).</param>
internal sealed record TakeoverOfferResponse(Guid Id, Guid AlbumId, string AlbumName, DateTimeOffset AnswerBy, int FounderMemories);

internal sealed record LegacyContactResponse(string? Email);

internal sealed record NameLegacyContactRequest(string? Email);
