using Memorio.Modules.AiNarration.Domain.Pseudonymization;

namespace Memorio.Modules.AiNarration.Tests;

public sealed class PseudonymizerTests
{
    private static SensitiveName Person(string name) => SensitiveName.Create(SensitiveNameKind.Person, name).Value;

    private static SensitiveName Place(string name) => SensitiveName.Create(SensitiveNameKind.Place, name).Value;

    [Fact]
    public void Names_are_replaced_with_tokens_and_restored()
    {
        var hidden = Pseudonymizer.Pseudonymize("Anna i Tomek pojechali do Zakopanego.", [Person("Anna"), Person("Tomek"), Place("Zakopane")]);

        Assert.Equal("[PERSON_1] i [PERSON_2] pojechali do [PLACE_1].", hidden.Text);
        Assert.Equal("Anna i Tomek pojechali do Zakopane.", Pseudonymizer.Restore(hidden.Text, hidden).Value);
    }

    [Theory]
    [InlineData("Byliśmy w Krakowie.", "Kraków")]
    [InlineData("Spacer po Krakowa rynku.", "Kraków")]
    [InlineData("Obiad z Anną.", "Anna")]
    [InlineData("Prezent dla Annie.", "Anna")]
    [InlineData("Wieczór w ŁODZI.", "Łódź")]
    [InlineData("Wycieczka nad Morskie Oko.", "Morskie Oko")]
    [InlineData("Wycieczka nad Morskim Okiem.", "Morskie Oko")]
    public void Inflected_and_differently_written_forms_are_hidden(string text, string name)
    {
        var hidden = Pseudonymizer.Pseudonymize(text, [Place(name)]);

        Assert.Contains("[PLACE_1]", hidden.Text, StringComparison.Ordinal);
        Assert.DoesNotContain(name[..3], hidden.Text, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void The_longer_name_wins_over_its_part()
    {
        var hidden = Pseudonymizer.Pseudonymize("Anna Maria i Anna.", [Person("Anna"), Person("Anna Maria")]);

        Assert.Equal("[PERSON_1] i [PERSON_2].", hidden.Text);
        Assert.Equal("Anna Maria", hidden.Tokens["[PERSON_1]"].Value);
    }

    [Fact]
    public void Names_inside_other_words_are_left_alone()
    {
        var hidden = Pseudonymizer.Pseudonymize("Bananowy deser.", [Person("Anna")]);

        Assert.Equal("Bananowy deser.", hidden.Text);
        Assert.Empty(hidden.Tokens);
    }

    [Fact]
    public void An_answer_with_an_unknown_token_is_rejected()
    {
        var hidden = Pseudonymizer.Pseudonymize("Anna.", [Person("Anna")]);

        var restored = Pseudonymizer.Restore("[PERSON_1] i [PERSON_7].", hidden);

        Assert.Equal("AiNarration.UnknownToken", restored.Error.Code);
    }

    [Fact]
    public void A_name_needs_two_characters()
    {
        Assert.True(SensitiveName.Create(SensitiveNameKind.Person, " A ").IsFailure);
        Assert.True(SensitiveName.Create(SensitiveNameKind.Person, null).IsFailure);
    }
}
