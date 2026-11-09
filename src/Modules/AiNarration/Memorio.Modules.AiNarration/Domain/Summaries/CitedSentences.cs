using System.Text.RegularExpressions;

namespace Memorio.Modules.AiNarration.Domain.Summaries;

/// <summary>
/// S2:A2 as a mechanism: the AI ends every sentence with the facts it tells, "W lipcu byliście w górach. [F1, F3]".
/// A sentence pointing to no fact, or to a fact it was not given, is dropped: what cannot be traced is not told.
/// </summary>
internal static partial class CitedSentences
{
    /// <param name="facts">The memory of each fact label, "F1" → its memory.</param>
    public static (IReadOnlyList<SummarySentence> Kept, int Dropped) Read(string text, IReadOnlyDictionary<string, Guid> facts)
    {
        var kept = new List<SummarySentence>();
        var dropped = 0;
        var lastEnd = 0;
        foreach (Match match in Cited().Matches(text))
        {
            dropped += Uncited(text[lastEnd..match.Index]);
            lastEnd = match.Index + match.Length;

            // The facts belong to the last sentence before them; the ones before it have none.
            var pieces = SentenceEnd().Split(match.Groups["text"].Value);
            dropped += pieces[..^1].Count(piece => piece.Any(char.IsLetter));
            var sentence = Normalized(pieces[^1]);
            var labels = match.Groups["ids"].Value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (sentence.Length == 0 || labels.Any(label => !facts.ContainsKey(label)))
            {
                dropped++;
                continue;
            }

            kept.Add(new SummarySentence(sentence, [.. labels.Select(label => facts[label]).Distinct()]));
        }

        dropped += Uncited(text[lastEnd..]);
        return (kept, dropped);
    }

    /// <summary>Sentences left without any facts: counted, never kept.</summary>
    private static int Uncited(string rest) =>
        SentenceEnd().Split(rest).Count(piece => piece.Any(char.IsLetter));

    private static string Normalized(string sentence) => Spaces().Replace(sentence, " ").Trim();

    [GeneratedRegex(@"(?<text>[^\[\]]+?)\s*\[(?<ids>F\d+(?:\s*,\s*F\d+)*)\]", RegexOptions.CultureInvariant)]
    private static partial Regex Cited();

    [GeneratedRegex(@"(?<=[.!?])\s+", RegexOptions.CultureInvariant)]
    private static partial Regex SentenceEnd();

    [GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
    private static partial Regex Spaces();
}
