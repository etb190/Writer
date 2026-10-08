using Writer.Core.Model;
using Writer.Shared.Shell;

namespace Writer.App.Presentation.Backstage;

/// <summary>Builds the shared document statistics rows consumed by both Writer shell renderers.</summary>
public static class BackstageInfoStatisticsPlanner
{
    public static IReadOnlyList<BackstageFieldRow> Build(TextDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var stats = WordCount.Of(document);
        return
        [
            new("Words", stats.Words.ToString()),
            new("Characters", stats.CharactersWithSpaces.ToString()),
            new("Paragraphs", stats.Paragraphs.ToString()),
        ];
    }
}
