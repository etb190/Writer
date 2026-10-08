using Writer.Shared.AppServices;

namespace Writer.App.Presentation.Shell;

public enum WriterCommandFeedbackTone
{
    Information,
    Warning,
}

public sealed record WriterCommandFeedbackPlan(
    string Title,
    string Message,
    WriterCommandFeedbackTone Tone);

/// <summary>Owns cross-renderer feedback decisions for the Help and support command family.</summary>
public static class WriterSupportCommandFeedbackPlanner
{
    public static WriterCommandFeedbackPlan? PlanExternalUriLaunch(
        ExternalUriLaunchResult result,
        string title,
        string url) =>
        result == ExternalUriLaunchResult.Launched
            ? null
            : new(
                title,
                WriterApplicationFrameTextCatalog.FormatExternalLinkFailure(title, url),
                WriterCommandFeedbackTone.Warning);

    public static WriterCommandFeedbackPlan PlanDiagnosticsCopy(PlatformClipboardWriteResult result) =>
        result.Status switch
        {
            PlatformClipboardWriteStatus.Success => new(
                WriterApplicationFrameTextCatalog.CopyDiagnosticsTitle,
                WriterApplicationFrameTextCatalog.DiagnosticsCopiedMessage,
                WriterCommandFeedbackTone.Information),
            PlatformClipboardWriteStatus.Unavailable => new(
                WriterApplicationFrameTextCatalog.CopyDiagnosticsTitle,
                WriterApplicationFrameTextCatalog.ClipboardUnavailableMessage,
                WriterCommandFeedbackTone.Warning),
            _ => new(
                WriterApplicationFrameTextCatalog.CopyDiagnosticsTitle,
                WriterApplicationFrameTextCatalog.FormatClipboardFailure(
                    result.ErrorMessage ?? "Clipboard write failed."),
                WriterCommandFeedbackTone.Warning),
        };
}
