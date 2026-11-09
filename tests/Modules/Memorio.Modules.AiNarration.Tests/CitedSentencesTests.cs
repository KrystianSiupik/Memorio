using Memorio.Modules.AiNarration.Domain.Summaries;

namespace Memorio.Modules.AiNarration.Tests;

/// <summary>MEM-E6-NARRACJA-AI-03, S2:A2: a sentence is kept only with facts it was given.</summary>
public sealed class CitedSentencesTests
{
    private static readonly Guid First = Guid.CreateVersion7();
    private static readonly Guid Second = Guid.CreateVersion7();
    private static readonly Dictionary<string, Guid> Facts = new() { ["F1"] = First, ["F2"] = Second };

    [Fact]
    public void Sentences_keep_their_memories_and_the_rest_is_counted_not_kept()
    {
        var (kept, dropped) = CitedSentences.Read(
            "Wstęp bez źródła. W lipcu byliście w górach. [F1, F2]   Potem   nad morzem. [F2] Zmyślone. [F7] A na koniec nic.",
            Facts);

        Assert.Equal(["W lipcu byliście w górach.", "Potem nad morzem."], kept.Select(sentence => sentence.Text));
        Assert.Equal([First, Second], kept[0].MemoryIds);
        Assert.Equal([Second], kept[1].MemoryIds);
        Assert.Equal(3, dropped);
    }

    [Fact]
    public void Text_without_any_facts_keeps_nothing()
    {
        var (kept, dropped) = CitedSentences.Read("Było pięknie. Wszyscy się cieszyli.", Facts);

        Assert.Empty(kept);
        Assert.Equal(2, dropped);
    }
}
