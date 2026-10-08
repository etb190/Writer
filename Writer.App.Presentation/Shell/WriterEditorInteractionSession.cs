using Writer.App.Presentation.DocumentView;

namespace Writer.App.Presentation.Shell;

public enum WriterChromeVisibility
{
    Visible,
    Hidden,
    Collapsed,
}

public sealed record WriterEditorChromeVisibility(
    WriterChromeVisibility TitleBar,
    WriterChromeVisibility Ribbon,
    WriterChromeVisibility DataFolder,
    WriterChromeVisibility ViewSwitch,
    WriterChromeVisibility Zoom,
    WriterChromeVisibility NavigationPane,
    WriterChromeVisibility RevealPane,
    WriterChromeVisibility ReviewingPane)
{
    public static WriterEditorChromeVisibility ReadMode { get; } = new(
        TitleBar: WriterChromeVisibility.Collapsed,
        Ribbon: WriterChromeVisibility.Collapsed,
        DataFolder: WriterChromeVisibility.Collapsed,
        ViewSwitch: WriterChromeVisibility.Collapsed,
        Zoom: WriterChromeVisibility.Collapsed,
        NavigationPane: WriterChromeVisibility.Collapsed,
        RevealPane: WriterChromeVisibility.Collapsed,
        ReviewingPane: WriterChromeVisibility.Collapsed);
}

public sealed record WriterReadModeTransition(
    bool IsActive,
    WriterEditorChromeVisibility Chrome,
    double ColumnWidth,
    string PageColorHex);

public sealed record WriterReadModeColumnPlan(
    string Token,
    double ColumnWidth,
    bool ApplyImmediately);

public sealed record WriterReadModePageColorPlan(
    string Token,
    string PageColorHex,
    bool ApplyImmediately);

/// <summary>
/// Owns host-neutral interaction state and decisions for the Writer editor work area.
/// Native controls, rendering, focus, and editor-layout snapshots stay in the platform adapters.
/// </summary>
public sealed class WriterEditorInteractionSession
{
    private WriterEditorChromeVisibility? _chromeBeforeReadMode;

    public bool IsReadModeActive { get; private set; }

    public string ReadModeColumnWidth { get; private set; } = WriterReadModePlanner.DefaultColumn;

    public string ReadModePageColor { get; private set; } = WriterReadModePlanner.NoColor;

    public WriterReadModeTransition ToggleReadMode(WriterEditorChromeVisibility currentChrome)
    {
        ArgumentNullException.ThrowIfNull(currentChrome);

        IsReadModeActive = !IsReadModeActive;
        if (IsReadModeActive)
        {
            _chromeBeforeReadMode = currentChrome;
            return BuildReadModeTransition(WriterEditorChromeVisibility.ReadMode);
        }

        var restoredChrome = _chromeBeforeReadMode ?? currentChrome;
        _chromeBeforeReadMode = null;
        return BuildReadModeTransition(restoredChrome);
    }

    public WriterReadModeColumnPlan UpdateReadModeColumnWidth(string? token)
    {
        ReadModeColumnWidth = WriterReadModePlanner.NormalizeColumnWidth(token);
        return new WriterReadModeColumnPlan(
            ReadModeColumnWidth,
            WriterReadModePlanner.ColumnWidth(ReadModeColumnWidth),
            IsReadModeActive);
    }

    public WriterReadModePageColorPlan UpdateReadModePageColor(string? token)
    {
        ReadModePageColor = WriterReadModePlanner.NormalizePageColor(token);
        return new WriterReadModePageColorPlan(
            ReadModePageColor,
            WriterReadModePlanner.PageColorHex(ReadModePageColor),
            IsReadModeActive);
    }

    public WriterEditorStatusPlan BuildStatus(WriterEditorStatusSnapshot snapshot) =>
        WriterEditorStatusPlanner.Build(snapshot);

    public WriterEditorStatusPlan BuildStatus(WriterEditorStatusContext context) =>
        WriterEditorStatusPlanner.Build(context);

    private WriterReadModeTransition BuildReadModeTransition(WriterEditorChromeVisibility chrome) => new(
        IsReadModeActive,
        chrome,
        WriterReadModePlanner.ColumnWidth(ReadModeColumnWidth),
        WriterReadModePlanner.PageColorHex(ReadModePageColor));
}
