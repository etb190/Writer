using Writer.App.Presentation.DocumentView;

namespace Writer.App.Presentation.Shell;

public enum WriterViewDepthMode
{
    LiveEditor,
    SplitPreview,
    MultiplePagesPreview,
    SideToSidePreview
}

public enum WriterViewDepthCommand
{
    RestoreLiveEditor,
    ToggleSplit,
    ToggleMultiplePages,
    ToggleSideToSide
}

public enum WriterViewDepthPagePairNavigationCommand
{
    PreviousPair,
    NextPair
}

public enum WriterViewDepthSurfaceKind
{
    LiveEditor,
    SplitEditors,
    ReadOnlyPagePreview,
    EditablePageView
}

public sealed record WriterViewDepthCapabilities(
    bool SupportsSplitPreview,
    bool SupportsMultiplePagesPreview,
    bool SupportsSideToSidePreview,
    bool SupportsEditableSideToSide,
    bool SupportsPagePairNavigation)
{
    public static WriterViewDepthCapabilities FullDesktop { get; } = new(
        SupportsSplitPreview: true,
        SupportsMultiplePagesPreview: true,
        SupportsSideToSidePreview: true,
        SupportsEditableSideToSide: true,
        SupportsPagePairNavigation: true);

    public bool Supports(WriterViewDepthMode mode) => mode switch
    {
        WriterViewDepthMode.LiveEditor => true,
        WriterViewDepthMode.SplitPreview => SupportsSplitPreview,
        WriterViewDepthMode.MultiplePagesPreview => SupportsMultiplePagesPreview,
        WriterViewDepthMode.SideToSidePreview => SupportsSideToSidePreview,
        _ => false,
    };
}

public sealed record WriterViewDepthState(WriterViewDepthMode Mode)
{
    public bool IsSplitActive => Mode == WriterViewDepthMode.SplitPreview;
    public bool IsMultiplePagesActive => Mode == WriterViewDepthMode.MultiplePagesPreview;
    public bool IsSideToSideActive => Mode == WriterViewDepthMode.SideToSidePreview;
}

public sealed record WriterViewDepthPlan(
    WriterViewDepthMode Mode,
    WriterViewDepthSurfaceKind SurfaceKind,
    bool IsSplitActive,
    bool IsMultiplePagesActive,
    bool IsSideToSideActive,
    bool UsesReadOnlySnapshot,
    int PagesAcross,
    DocumentViewDepthLayoutPlan Layout,
    string StatusText,
    string? Limitation);

public sealed record WriterViewDepthPagePairNavigationState(
    WriterViewDepthMode Mode,
    int FirstVisiblePageNumber,
    int LastVisiblePageNumber,
    int TotalPages,
    int PagesPerPair,
    bool CanGoToPreviousPair,
    bool CanGoToNextPair,
    string StatusText)
{
    public bool IsSideToSideNavigationActive => Mode == WriterViewDepthMode.SideToSidePreview;
}

public static class WriterViewDepthPlanner
{
    public static WriterViewDepthPlan Plan(
        WriterViewDepthState current,
        WriterViewDepthCommand command,
        WriterViewDepthCapabilities? capabilities = null)
    {
        capabilities ??= WriterViewDepthCapabilities.FullDesktop;
        var target = command switch
        {
            WriterViewDepthCommand.RestoreLiveEditor => WriterViewDepthMode.LiveEditor,
            WriterViewDepthCommand.ToggleSplit => current.IsSplitActive
                ? WriterViewDepthMode.LiveEditor
                : WriterViewDepthMode.SplitPreview,
            WriterViewDepthCommand.ToggleMultiplePages => current.IsMultiplePagesActive
                ? WriterViewDepthMode.LiveEditor
                : WriterViewDepthMode.MultiplePagesPreview,
            WriterViewDepthCommand.ToggleSideToSide => current.IsSideToSideActive
                ? WriterViewDepthMode.LiveEditor
                : WriterViewDepthMode.SideToSidePreview,
            _ => WriterViewDepthMode.LiveEditor
        };

        return Build(capabilities.Supports(target) ? target : current.Mode, capabilities);
    }

    public static WriterViewDepthPlan Build(
        WriterViewDepthMode mode,
        WriterViewDepthCapabilities? capabilities = null)
    {
        capabilities ??= WriterViewDepthCapabilities.FullDesktop;
        if (!capabilities.Supports(mode))
            mode = WriterViewDepthMode.LiveEditor;

        return mode switch
        {
        WriterViewDepthMode.SplitPreview => new WriterViewDepthPlan(
            mode,
            WriterViewDepthSurfaceKind.SplitEditors,
            IsSplitActive: true,
            IsMultiplePagesActive: false,
            IsSideToSideActive: false,
            UsesReadOnlySnapshot: false,
            PagesAcross: 1,
            Layout: DocumentViewDepthLayoutPlanner.Build(mode),
            StatusText: "Split view active: synchronized live editors above and below.",
            Limitation: null),
        WriterViewDepthMode.MultiplePagesPreview => new WriterViewDepthPlan(
            mode,
            WriterViewDepthSurfaceKind.EditablePageView,
            IsSplitActive: false,
            IsMultiplePagesActive: true,
            IsSideToSideActive: false,
            UsesReadOnlySnapshot: false,
            PagesAcross: 2,
            Layout: DocumentViewDepthLayoutPlanner.Build(mode),
            StatusText: "Multiple Pages view active: editable 2-by-2 page grid.",
            Limitation: null),
        WriterViewDepthMode.SideToSidePreview => new WriterViewDepthPlan(
            mode,
            capabilities.SupportsEditableSideToSide
                ? WriterViewDepthSurfaceKind.EditablePageView
                : WriterViewDepthSurfaceKind.ReadOnlyPagePreview,
            IsSplitActive: false,
            IsMultiplePagesActive: false,
            IsSideToSideActive: true,
            UsesReadOnlySnapshot: !capabilities.SupportsEditableSideToSide,
            PagesAcross: 2,
            Layout: DocumentViewDepthLayoutPlanner.Build(mode),
            StatusText: capabilities.SupportsEditableSideToSide
                ? "Side to Side view active: editable two-page horizontal-flow view with pair navigation."
                : "Side to Side view active: read-only two-page horizontal-flow preview with pair navigation.",
            Limitation: capabilities.SupportsEditableSideToSide
                ? null
                : "Editing is disabled because this host does not provide an editable side-to-side surface."),
        _ => new WriterViewDepthPlan(
            WriterViewDepthMode.LiveEditor,
            WriterViewDepthSurfaceKind.LiveEditor,
            IsSplitActive: false,
            IsMultiplePagesActive: false,
            IsSideToSideActive: false,
            UsesReadOnlySnapshot: false,
            PagesAcross: 1,
            Layout: DocumentViewDepthLayoutPlanner.Build(WriterViewDepthMode.LiveEditor),
            StatusText: "Live editor active.",
            Limitation: null)
        };
    }

    public static double BuildPreviewScale(
        WriterViewDepthMode mode,
        double viewportWidthDip,
        double viewportHeightDip,
        double pageWidthDip,
        double pageHeightDip)
    {
        if (!double.IsFinite(viewportWidthDip) || viewportWidthDip <= 0 ||
            !double.IsFinite(viewportHeightDip) || viewportHeightDip <= 0 ||
            !double.IsFinite(pageWidthDip) || pageWidthDip <= 0 ||
            !double.IsFinite(pageHeightDip) || pageHeightDip <= 0)
        {
            return 1.0;
        }

        return DocumentViewDepthLayoutPlanner.BuildPreviewScale(
            Build(mode).Layout,
            viewportWidthDip,
            viewportHeightDip,
            pageWidthDip,
            pageHeightDip);
    }

    public static WriterViewDepthPagePairNavigationState BuildPagePairNavigation(
        WriterViewDepthPlan plan,
        int requestedFirstVisiblePageNumber,
        int totalPages)
    {
        ArgumentNullException.ThrowIfNull(plan);

        var safeTotalPages = Math.Max(1, totalPages);
        var pagesPerPair = Math.Max(1, plan.Layout.PagesAcross);
        if (!plan.IsSideToSideActive)
        {
            return new WriterViewDepthPagePairNavigationState(
                plan.Mode,
                FirstVisiblePageNumber: 1,
                LastVisiblePageNumber: 1,
                TotalPages: safeTotalPages,
                PagesPerPair: pagesPerPair,
                CanGoToPreviousPair: false,
                CanGoToNextPair: false,
                StatusText: plan.StatusText);
        }

        var first = NormalizePairStart(requestedFirstVisiblePageNumber, safeTotalPages, pagesPerPair);
        var last = Math.Min(safeTotalPages, first + pagesPerPair - 1);
        var maxStart = NormalizePairStart(safeTotalPages, safeTotalPages, pagesPerPair);

        return new WriterViewDepthPagePairNavigationState(
            plan.Mode,
            first,
            last,
            safeTotalPages,
            pagesPerPair,
            CanGoToPreviousPair: first > 1,
            CanGoToNextPair: first < maxStart,
            FormatSideToSidePagePairStatus(first, last, safeTotalPages));
    }

    public static WriterViewDepthPagePairNavigationState NavigatePagePair(
        WriterViewDepthPlan plan,
        WriterViewDepthPagePairNavigationState current,
        WriterViewDepthPagePairNavigationCommand command)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(current);

        var step = Math.Max(1, current.PagesPerPair);
        var requested = command switch
        {
            WriterViewDepthPagePairNavigationCommand.PreviousPair => current.FirstVisiblePageNumber - step,
            WriterViewDepthPagePairNavigationCommand.NextPair => current.FirstVisiblePageNumber + step,
            _ => current.FirstVisiblePageNumber
        };

        return BuildPagePairNavigation(plan, requested, current.TotalPages);
    }

    private static int NormalizePairStart(int requestedFirstVisiblePageNumber, int totalPages, int pagesPerPair)
    {
        var safeTotalPages = Math.Max(1, totalPages);
        var safePagesPerPair = Math.Max(1, pagesPerPair);
        var clamped = Math.Clamp(requestedFirstVisiblePageNumber, 1, safeTotalPages);
        return ((clamped - 1) / safePagesPerPair) * safePagesPerPair + 1;
    }

    private static string FormatSideToSidePagePairStatus(int first, int last, int totalPages) =>
        first == last
            ? $"Side to Side page {first} of {totalPages}."
            : $"Side to Side pages {first}-{last} of {totalPages}.";
}
