using System.Linq;
using Writer.Shared.AppServices;
using Writer.Core.Model;

namespace Writer.App.Presentation.Options;

public sealed record WriterEditorTypingOptionsPlan(
    bool AutoCorrectEnabled,
    AutoFormatOptions AutoFormat,
    AutoCorrectOptions AutoCorrect);

public sealed record WriterOptionsPersistOutcome(
    WriterEditorTypingOptionsPlan EditorTypingOptions,
    bool Persisted);

/// <summary>
/// Owns the mutable application-options instance consumed by a running Writer shell and projects the
/// editor settings that native hosts apply to their platform-specific work area.
/// </summary>
public sealed class WriterOptionsRuntimeSession
{
    public WriterOptionsRuntimeSession(WriterOptions liveOptions)
    {
        LiveOptions = liveOptions ?? throw new ArgumentNullException(nameof(liveOptions));
        LiveOptions.Normalize();
    }

    public WriterOptions LiveOptions { get; }

    public WriterEditorTypingOptionsPlan EditorTypingOptions => new(
        LiveOptions.AutoCorrectEnabled,
        LiveOptions.AutoFormat,
        LiveOptions.AutoCorrect);

    public WriterEditorTypingOptionsPlan Apply(WriterOptions editedOptions)
    {
        ArgumentNullException.ThrowIfNull(editedOptions);

        LiveOptions.RecentFilesCap = editedOptions.RecentFilesCap;
        LiveOptions.DefaultSaveFormat = editedOptions.DefaultSaveFormat;
        LiveOptions.UiLanguage = editedOptions.UiLanguage;
        LiveOptions.AutoCorrectEnabled = editedOptions.AutoCorrectEnabled;
        LiveOptions.AutoFormat = editedOptions.AutoFormat;
        LiveOptions.AutoCorrect = editedOptions.AutoCorrect;
        LiveOptions.Normalize();

        return EditorTypingOptions;
    }

    /// <summary>
    /// Applies the edited options to <see cref="LiveOptions"/> (as <see cref="Apply"/>) and then persists
    /// them. When <paramref name="reloadFresh"/> is supplied, reloads the freshest on-disk snapshot
    /// immediately before saving and copies across only the fields that actually differ between this
    /// dialog session's open-time snapshot (<see cref="LiveOptions"/> as it stood when this method was
    /// called) and <paramref name="editedOptions"/> -- so a concurrently running Writer window or process
    /// that already persisted a change to a field this session never touched is not silently reverted
    /// (last-writer-wins lost update). Mirrors FreeX's <c>FreeXOptionsRuntimeSession.CommitDialog</c> /
    /// <c>OptionsDialogPlanner.MergeOntoFreshLoad</c>. Omitting <paramref name="reloadFresh"/> preserves the
    /// previous whole-document-overwrite behavior for callers that only need an in-memory apply (e.g.
    /// tests).
    /// </summary>
    public WriterOptionsPersistOutcome ApplyAndPersist(
        WriterOptions editedOptions,
        Func<WriterOptions, bool> persist,
        Func<WriterOptions>? reloadFresh = null)
    {
        ArgumentNullException.ThrowIfNull(editedOptions);
        ArgumentNullException.ThrowIfNull(persist);

        var openTimeSnapshot = reloadFresh is null ? null : LiveOptions.Clone();
        var plan = Apply(editedOptions);

        if (reloadFresh is null || openTimeSnapshot is null)
            return new WriterOptionsPersistOutcome(plan, persist(LiveOptions));

        var fresh = reloadFresh();
        fresh.Normalize();
        MergeOntoFreshLoad(fresh, openTimeSnapshot, editedOptions);

        var persisted = persist(fresh);
        if (persisted)
            CopyInto(LiveOptions, fresh);

        return new WriterOptionsPersistOutcome(plan, persisted);
    }

    private static void MergeOntoFreshLoad(WriterOptions freshFromDisk, WriterOptions openTimeSnapshot, WriterOptions edited)
    {
        BasicApplicationOptionsMerge.MergeOntoFreshLoad(freshFromDisk, openTimeSnapshot, edited);

        if (edited.AutoCorrectEnabled != openTimeSnapshot.AutoCorrectEnabled)
            freshFromDisk.AutoCorrectEnabled = edited.AutoCorrectEnabled;
        if (edited.AutoFormat != openTimeSnapshot.AutoFormat)
            freshFromDisk.AutoFormat = edited.AutoFormat;
        if (!AutoCorrectOptionsEqual(edited.AutoCorrect, openTimeSnapshot.AutoCorrect))
            freshFromDisk.AutoCorrect = edited.AutoCorrect;

        freshFromDisk.Normalize();
    }

    private static bool AutoCorrectOptionsEqual(AutoCorrectOptions? a, AutoCorrectOptions? b)
    {
        if (ReferenceEquals(a, b))
            return true;
        if (a is null || b is null)
            return false;

        return a.CorrectTwoInitialCapitals == b.CorrectTwoInitialCapitals
            && a.CapitalizeDayNames == b.CapitalizeDayNames
            && a.ReplaceText == b.ReplaceText
            && a.Replacements.SequenceEqual(b.Replacements);
    }

    private static void CopyInto(WriterOptions target, WriterOptions source)
    {
        target.RecentFilesCap = source.RecentFilesCap;
        target.DefaultSaveFormat = source.DefaultSaveFormat;
        target.UiLanguage = source.UiLanguage;
        target.AutoCorrectEnabled = source.AutoCorrectEnabled;
        target.AutoFormat = source.AutoFormat;
        target.AutoCorrect = source.AutoCorrect;
    }
}
