using System.Reflection;
using Writer.Shared.Shell;

namespace Writer.App.Presentation;

/// <summary>Shared Writer About content contract for both desktop hosts.</summary>
public static class WriterAboutDialogPresentation
{
    public const string WindowTitle = "About Writer";
    public const string DialogAutomationId = "AboutWriterDialog";
    public const string TextAutomationId = "AboutWriterText";
    public const string OkAutomationId = "AboutWriterOkButton";
    public const string HelpText = "View version, license, privacy, and source information about Writer.";

    // The WPF authority paints the final content pixel at x=528 in the 560x600
    // harness frame. Avalonia needs the measured one-DIP right-edge reserve.
    public const double AvaloniaRootRightMargin = AboutDialogMetrics.WriterAvaloniaRootRightMargin;
    // WPF's native About TextBox keeps its standard 8-DIP right inset. Keep the shared
    // Avalonia default unchanged for other products and pass Writer's measured inset here.
    public const double AvaloniaTextPaddingRight = AboutDialogMetrics.TextPadding;
    public const double AvaloniaTextFontSize = AboutDialogMetrics.TextFontSize;
    public const double AvaloniaTextPaddingTop = AboutDialogMetrics.TextPadding + 1;
    public const bool AvaloniaDefaultButtonAccent = true;
    // WPF's 12px About TextBox advances its wrapped lines at 16 device pixels.
    // Keeping the Avalonia line box at that measured cadence prevents the centered
    // document from drifting upward at the first paragraph and downward by the last.
    public const double AvaloniaTextLineHeight = 16.0;

    public static AboutDialogPresentation Create(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        return new AboutDialogPresentation(
            WindowTitle,
            WriterProductInfo.CreateAboutText(assembly),
            DialogAutomationId,
            TextAutomationId,
            OkAutomationId,
            HelpText,
            AvaloniaRootRightMargin,
            AvaloniaTextPaddingRight,
            AvaloniaTextFontSize,
            AvaloniaTextPaddingTop,
            AvaloniaDefaultButtonAccent,
            AvaloniaTextLineHeight);
    }
}
