using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.AiNarration.Domain.Pseudonymization;

/// <summary>
/// Replaces names of people and places with tokens ([PERSON_1], [PLACE_2]) before text goes to the
/// AI provider, and puts the names back into the answer (QS8: 0 names from the list in the payload).
/// Tokens are numbered in order of first appearance.
/// <para>
/// Polish names are inflected ("Kraków" → "w Krakowie", "Anna" → "z Anną"), so a word matches a name
/// when, without diacritics and case, it starts with the name's stem: the word without its last two
/// letters, at least 3 letters (2 for each word of a multi-word name such as "Morskie Oko", where the
/// neighbouring word keeps false matches rare). Matching more than needed only hides an extra word,
/// which is the safe side. Known limit: inflected forms of one-word names shorter than 4 letters
/// ("Ola" → "Oli") are not caught.
/// </para>
/// </summary>
internal static partial class Pseudonymizer
{
    private const char Marker = '\u0001';

    public static PseudonymizedText Pseudonymize(string text, IEnumerable<SensitiveName> names)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(names);

        // First pass: each name found becomes a temporary marker; longest names first, so
        // "Anna Maria" is replaced before "Anna".
        var found = new List<SensitiveName>();
        var result = text.Replace(Marker.ToString(), string.Empty, StringComparison.Ordinal);
        foreach (var name in names.DistinctBy(n => (n.Kind, Fold(n.Value))).OrderByDescending(n => n.Value.Length))
        {
            var pattern = NamePattern(name.Value);
            if (pattern.IsMatch(Fold(result)))
            {
                result = ReplaceFolded(result, pattern, $"{Marker}{found.Count}{Marker}");
                found.Add(name);
            }
        }

        // Second pass: number the tokens per kind in order of first appearance.
        var tokens = new Dictionary<string, SensitiveName>(StringComparer.Ordinal);
        var tokenOf = new Dictionary<int, string>();
        var counters = new Dictionary<SensitiveNameKind, int>();
        result = TemporaryMarker().Replace(result, match =>
        {
            var index = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            if (!tokenOf.TryGetValue(index, out var token))
            {
                var name = found[index];
                counters[name.Kind] = counters.GetValueOrDefault(name.Kind) + 1;
                token = $"[{(name.Kind == SensitiveNameKind.Person ? "PERSON" : "PLACE")}_{counters[name.Kind]}]";
                tokenOf[index] = token;
                tokens[token] = name;
            }

            return token;
        });

        return new PseudonymizedText(result, tokens);
    }

    /// <summary>Puts the names back; fails when the answer contains a token that was never issued.</summary>
    public static Result<string> Restore(string text, PseudonymizedText source)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(source);

        foreach (Match match in TokenPattern().Matches(text))
        {
            if (!source.Tokens.ContainsKey(match.Value))
            {
                return Error.Failure("AiNarration.UnknownToken", "The AI answer contains a name token that was not in the request.");
            }
        }

        return TokenPattern().Replace(text, match => source.Tokens[match.Value].Value);
    }

    /// <summary>Whole words starting with the folded stem of each word of the name.</summary>
    private static Regex NamePattern(string name)
    {
        var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var minimumStem = words.Length > 1 ? 2 : 3;
        var parts = words.Select(word =>
        {
            var folded = Fold(word);
            var stemLength = folded.Length <= minimumStem ? folded.Length : Math.Max(minimumStem, folded.Length - 2);
            return Regex.Escape(folded[..stemLength]) + @"\w*";
        });
        return new Regex(@"(?<!\w)" + string.Join(@"\s+", parts) + @"(?!\w)", RegexOptions.CultureInvariant);
    }

    /// <summary>Replaces matches found in the folded text at the same positions in the original text.</summary>
    private static string ReplaceFolded(string original, Regex pattern, string replacement)
    {
        var folded = Fold(original);
        var builder = new StringBuilder();
        var position = 0;
        foreach (Match match in pattern.Matches(folded))
        {
            builder.Append(original, position, match.Index - position).Append(replacement);
            position = match.Index + match.Length;
        }

        return builder.Append(original, position, original.Length - position).ToString();
    }

    /// <summary>Lower case without diacritics, same length as the input ("Łódź" → "lodz").</summary>
    private static string Fold(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            var lower = char.ToLowerInvariant(character);
            builder.Append(lower == 'ł' ? 'l' : RemoveDiacritic(lower));
        }

        return builder.ToString();
    }

    private static char RemoveDiacritic(char character)
    {
        var decomposed = character.ToString().Normalize(NormalizationForm.FormD);
        return decomposed.Length > 0 && CharUnicodeInfo.GetUnicodeCategory(decomposed[0]) != UnicodeCategory.NonSpacingMark
            ? decomposed[0]
            : character;
    }

    [GeneratedRegex(@"\[(PERSON|PLACE)_\d+\]", RegexOptions.CultureInvariant)]
    private static partial Regex TokenPattern();

    [GeneratedRegex("\u0001(\\d+)\u0001", RegexOptions.CultureInvariant)]
    private static partial Regex TemporaryMarker();
}

/// <summary>Text without the sensitive names, and the tokens that replaced them.</summary>
internal sealed record PseudonymizedText(string Text, IReadOnlyDictionary<string, SensitiveName> Tokens);
