namespace Writer.App.Presentation.Shell;

public sealed record WriterFrameActionText(string Label, string HelpText);

public sealed record WriterSemanticIdentity(string AutomationId, string AutomationName);

/// <summary>
/// Canonical application-frame text shared by the native Writer renderers.
/// </summary>
public static class WriterApplicationFrameTextCatalog
{
    public const string HelpOnlineCommandName = "Help Online";
    public const string FeedbackCommandName = "Feedback";
    public const string CheckForUpdatesCommandName = "Check for Updates";
    public const string WebLayoutLabel = "Web Layout";
    public const string PageEditLabel = "Page Edit";
    public const string PreviousPagePairLabel = "Previous pair";
    public const string NextPagePairLabel = "Next pair";
    public const string CopyDiagnosticsTitle = "Copy Diagnostics";
    public const string ClipboardUnavailableMessage = "Writer could not access the clipboard.";
    public const string DiagnosticsCopiedMessage = "Writer diagnostics were copied to the clipboard.";

    public static WriterSemanticIdentity PreviousPagePairSemantic { get; } = new(
        "Writer.SideToSide.Previouspair",
        "Previous Side-to-Side page pair");

    public static WriterSemanticIdentity NextPagePairSemantic { get; } = new(
        "Writer.SideToSide.Nextpair",
        "Next Side-to-Side page pair");

    public const string PagePairStatusAutomationId = "Writer.SideToSidePagePairStatus";

    public static string FormatExternalLinkFailure(string title, string url) =>
        $"Writer could not open {title}. The link is:\n\n{url}";

    public static string FormatClipboardFailure(string errorMessage) =>
        $"Writer could not access the clipboard: {errorMessage}";

    public static WriterFrameActionText ReadMode { get; } = new(
        "Read Mode",
        "Toggle distraction-free Read Mode");

    public static WriterFrameActionText PrintLayout { get; } = new(
        "Print Layout",
        "Print Layout page view");

    public static WriterFrameActionText Draft { get; } = new(
        "Draft",
        "Draft: simplified continuous view for fast editing");
}
