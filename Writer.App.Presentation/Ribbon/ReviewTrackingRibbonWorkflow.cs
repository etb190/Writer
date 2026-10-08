using Writer.Shared.Ribbon;
using Writer.App.Presentation.DocumentView;

namespace Writer.App.Presentation.Ribbon;

public sealed record ReviewTrackingCommandBindings(
    Action PrepareExecution,
    Func<bool> IsTrackChangesEnabled,
    Func<bool> HasSelection,
    Action ToggleTrackChanges,
    Action MarkSelectionAsInsertion,
    Func<bool> IsTrackFormattingEnabled,
    Action ToggleTrackFormatting,
    Func<ReviewDisplayMode> GetDisplayForReview,
    Action<ReviewDisplayMode> ApplyDisplayForReview,
    Func<bool> ShowMarkupInsertionsAndDeletions,
    Action<bool> ApplyShowMarkupInsertionsAndDeletions,
    Func<bool> ShowMarkupComments,
    Action<bool> ApplyShowMarkupComments,
    Func<bool> ShowMarkupFormatting,
    Action<bool> ApplyShowMarkupFormatting,
    Action AcceptAllRevisions,
    Action RejectAllRevisions,
    Func<bool>? IsTrackChangesLockedByProtection = null);

public sealed record ReviewTrackingRibbonCommands(
    IRibbonStatefulCommand TrackChanges,
    IRibbonStatefulCommand TrackFormatting,
    IRibbonStatefulCommand DisplayAllMarkup,
    IRibbonStatefulCommand DisplaySimpleMarkup,
    IRibbonStatefulCommand DisplayNoMarkup,
    IRibbonStatefulCommand DisplayOriginal,
    IRibbonStatefulCommand ShowInsertionsAndDeletions,
    IRibbonStatefulCommand ShowComments,
    IRibbonStatefulCommand ShowFormatting);

/// <summary>
/// Registers the renderer-neutral Review tracking workflow over host-supplied editor operations.
/// </summary>
public static class ReviewTrackingRibbonWorkflow
{
    public static ReviewTrackingRibbonCommands Register(
        IRibbonCommandRegistry registry,
        ReviewTrackingCommandBindings bindings)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(bindings);

        var trackChanges = new TrackChangesCommand(bindings);
        var trackFormatting = new WriterStatefulToggleCommand(
            bindings.ToggleTrackFormatting,
            bindings.IsTrackFormattingEnabled,
            bindings.PrepareExecution);
        var displayAllMarkup = new DisplayForReviewCommand(bindings, ReviewDisplayMode.AllMarkup);
        var displaySimpleMarkup = new DisplayForReviewCommand(bindings, ReviewDisplayMode.SimpleMarkup);
        var displayNoMarkup = new DisplayForReviewCommand(bindings, ReviewDisplayMode.NoMarkup);
        var displayOriginal = new DisplayForReviewCommand(bindings, ReviewDisplayMode.Original);
        var showInsertionsAndDeletions = new WriterStatefulToggleCommand(
            () => bindings.ApplyShowMarkupInsertionsAndDeletions(!bindings.ShowMarkupInsertionsAndDeletions()),
            bindings.ShowMarkupInsertionsAndDeletions,
            bindings.PrepareExecution);
        var showComments = new WriterStatefulToggleCommand(
            () => bindings.ApplyShowMarkupComments(!bindings.ShowMarkupComments()),
            bindings.ShowMarkupComments,
            bindings.PrepareExecution);
        var showFormatting = new WriterStatefulToggleCommand(
            () => bindings.ApplyShowMarkupFormatting(!bindings.ShowMarkupFormatting()),
            bindings.ShowMarkupFormatting,
            bindings.PrepareExecution);

        registry.Register("writer.track-changes", trackChanges);
        registry.Register("writer.track-formatting", trackFormatting);
        registry.Register("writer.display-for-review", displayAllMarkup);
        registry.Register("writer.display-for-review-all-markup", displayAllMarkup);
        registry.Register("writer.display-for-review-simple-markup", displaySimpleMarkup);
        registry.Register("writer.display-for-review-no-markup", displayNoMarkup);
        registry.Register("writer.display-for-review-original", displayOriginal);
        registry.Register("writer.show-markup", EmptyRibbonCommand.Instance);
        registry.Register("writer.show-markup-insertions-deletions", showInsertionsAndDeletions);
        registry.Register("writer.show-markup-comments", showComments);
        registry.Register("writer.show-markup-formatting", showFormatting);
        registry.Register("writer.accept-all", Action(bindings, bindings.AcceptAllRevisions));
        registry.Register("writer.reject-all", Action(bindings, bindings.RejectAllRevisions));

        return new ReviewTrackingRibbonCommands(
            trackChanges,
            trackFormatting,
            displayAllMarkup,
            displaySimpleMarkup,
            displayNoMarkup,
            displayOriginal,
            showInsertionsAndDeletions,
            showComments,
            showFormatting);
    }

    private static IRibbonCommand Action(ReviewTrackingCommandBindings bindings, Action execute) =>
        new ActionRibbonCommand(() =>
        {
            bindings.PrepareExecution();
            execute();
        });

    private sealed class TrackChangesCommand(ReviewTrackingCommandBindings bindings) : IRibbonStatefulCommand
    {
        public void Execute(RibbonCommandContext context)
        {
            // Restrict Editing > "Tracked changes" (ProtectionMode.TrackChangesOnly) forces every
            // future edit to be a tracked revision; Word greys this toggle out for the duration so a
            // reviewer can't quietly disable tracking and slip in untracked edits. Re-check the lock
            // in Execute too, not just GetState, so nothing that bypasses ribbon enablement can toggle it.
            if (IsLockedByProtection())
                return;

            bindings.PrepareExecution();
            var plan = TrackChangesTogglePlanner.Build(
                bindings.IsTrackChangesEnabled(),
                bindings.HasSelection());
            bindings.ToggleTrackChanges();
            if (plan.MarkSelectionAsInsertion)
                bindings.MarkSelectionAsInsertion();
        }

        public RibbonCommandState GetState() =>
            new(IsEnabled: !IsLockedByProtection(), IsChecked: bindings.IsTrackChangesEnabled());

        private bool IsLockedByProtection() =>
            bindings.IsTrackChangesLockedByProtection?.Invoke() ?? false;
    }

    private sealed class DisplayForReviewCommand(
        ReviewTrackingCommandBindings bindings,
        ReviewDisplayMode mode) : IRibbonStatefulCommand
    {
        public void Execute(RibbonCommandContext context)
        {
            bindings.PrepareExecution();
            bindings.ApplyDisplayForReview(mode);
        }

        public RibbonCommandState GetState() =>
            new(IsEnabled: true, IsChecked: bindings.GetDisplayForReview() == mode);
    }
}
