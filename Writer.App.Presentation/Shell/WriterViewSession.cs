using Writer.App.Presentation.DocumentView;

namespace Writer.App.Presentation.Shell;

public sealed record WriterViewDepthTransition(
    WriterViewDepthPlan Previous,
    WriterViewDepthPlan Current,
    bool ExitSplitSurface,
    bool ExitPageSurface);

public sealed record WriterDocumentViewChangePlan(
    DocumentViewMode TargetMode,
    bool ExitOutlineMode,
    bool ExitPagedEditMode,
    bool ExitPaginatedView);

public sealed record WriterDocumentViewCheckPlan(
    bool PrintLayout,
    bool WebLayout,
    bool Draft,
    bool PagedEdit);

public sealed record WriterOutlineViewTransition(
    bool IsOutlineMode,
    bool IsPagedEditMode,
    bool ExitPageSurface,
    bool ExitPagedEditSurface,
    bool EnterPagedEditSurface);

/// <summary>
/// Owns the portable view-mode and view-depth state for a Writer work area. Renderers apply the
/// returned plans to native controls and continue to own measurement, focus, and surface creation.
/// </summary>
public sealed class WriterViewSession
{
    private readonly WriterViewDepthCapabilities _capabilities;
    private bool _restorePagedEditAfterOutline;

    public WriterViewSession(WriterViewDepthCapabilities capabilities)
    {
        ArgumentNullException.ThrowIfNull(capabilities);
        _capabilities = capabilities;
        CurrentDepth = WriterViewDepthPlanner.Build(WriterViewDepthMode.LiveEditor, capabilities);
        PagePairNavigation = WriterViewDepthPlanner.BuildPagePairNavigation(
            CurrentDepth,
            requestedFirstVisiblePageNumber: 1,
            totalPages: 1);
    }

    public WriterViewDepthPlan CurrentDepth { get; private set; }

    public WriterViewDepthPagePairNavigationState PagePairNavigation { get; private set; }

    public bool IsPageSurfaceActive =>
        CurrentDepth.SurfaceKind is WriterViewDepthSurfaceKind.ReadOnlyPagePreview or
            WriterViewDepthSurfaceKind.EditablePageView;

    public WriterViewDepthTransition Execute(WriterViewDepthCommand command) =>
        TransitionTo(WriterViewDepthPlanner.Plan(
            new WriterViewDepthState(CurrentDepth.Mode),
            command,
            _capabilities));

    public WriterViewDepthTransition RestoreLiveEditor() =>
        TransitionTo(WriterViewDepthPlanner.Build(WriterViewDepthMode.LiveEditor, _capabilities));

    public WriterViewDepthPagePairNavigationState StartPagePairNavigation(
        int totalPages,
        int requestedFirstVisiblePageNumber = 1)
    {
        PagePairNavigation = WriterViewDepthPlanner.BuildPagePairNavigation(
            CurrentDepth,
            requestedFirstVisiblePageNumber,
            totalPages);
        return PagePairNavigation;
    }

    public WriterViewDepthPagePairNavigationState NavigatePagePair(
        WriterViewDepthPagePairNavigationCommand command)
    {
        if (!_capabilities.SupportsPagePairNavigation || !CurrentDepth.IsSideToSideActive)
            return PagePairNavigation;

        PagePairNavigation = WriterViewDepthPlanner.NavigatePagePair(
            CurrentDepth,
            PagePairNavigation,
            command);
        return PagePairNavigation;
    }

    public void ResetPagePairNavigation()
    {
        var live = WriterViewDepthPlanner.Build(WriterViewDepthMode.LiveEditor, _capabilities);
        PagePairNavigation = WriterViewDepthPlanner.BuildPagePairNavigation(
            live,
            requestedFirstVisiblePageNumber: 1,
            totalPages: 1);
    }

    public WriterDocumentViewChangePlan PlanDocumentViewChange(
        DocumentViewMode currentMode,
        bool isOutlineMode,
        bool isPagedEditMode,
        DocumentViewMode targetMode)
    {
        if (targetMode == DocumentViewMode.PagedEdit)
            throw new ArgumentOutOfRangeException(nameof(targetMode), targetMode, "Paged Edit is an overlay workflow.");

        return new WriterDocumentViewChangePlan(
            targetMode,
            ExitOutlineMode: isOutlineMode,
            ExitPagedEditMode: isPagedEditMode,
            ExitPaginatedView: IsPageSurfaceActive);
    }

    public WriterDocumentViewCheckPlan BuildDocumentViewChecks(
        DocumentViewMode currentMode,
        bool isOutlineMode,
        bool isPagedEditMode) => new(
            PrintLayout: !isOutlineMode && !isPagedEditMode && currentMode == DocumentViewMode.PrintLayout,
            WebLayout: !isOutlineMode && !isPagedEditMode && currentMode == DocumentViewMode.WebLayout,
            Draft: !isOutlineMode && !isPagedEditMode && currentMode == DocumentViewMode.Draft,
            PagedEdit: !isOutlineMode && isPagedEditMode);

    public WriterOutlineViewTransition EnterOutline(bool isPagedEditMode)
    {
        _restorePagedEditAfterOutline = isPagedEditMode;
        return new WriterOutlineViewTransition(
            IsOutlineMode: true,
            IsPagedEditMode: false,
            ExitPageSurface: IsPageSurfaceActive,
            ExitPagedEditSurface: isPagedEditMode,
            EnterPagedEditSurface: false);
    }

    public WriterOutlineViewTransition LeaveOutline(bool restorePriorView = true)
    {
        var restorePagedEdit = restorePriorView && _restorePagedEditAfterOutline;
        _restorePagedEditAfterOutline = false;
        return new WriterOutlineViewTransition(
            IsOutlineMode: false,
            IsPagedEditMode: restorePagedEdit,
            ExitPageSurface: false,
            ExitPagedEditSurface: false,
            EnterPagedEditSurface: restorePagedEdit);
    }

    private WriterViewDepthTransition TransitionTo(WriterViewDepthPlan next)
    {
        var previous = CurrentDepth;
        var nextIsPageSurface = next.SurfaceKind is WriterViewDepthSurfaceKind.ReadOnlyPagePreview or
            WriterViewDepthSurfaceKind.EditablePageView;
        var exitPageSurface = IsPageSurfaceActive &&
            (!nextIsPageSurface || previous.Mode != next.Mode);
        var exitSplitSurface = previous.IsSplitActive && !next.IsSplitActive;

        CurrentDepth = next;
        if (!next.IsSideToSideActive)
            ResetPagePairNavigation();

        return new WriterViewDepthTransition(previous, next, exitSplitSurface, exitPageSurface);
    }
}
