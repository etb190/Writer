using Avalonia.Controls;
using Writer.Shared.AppServices;
using Writer.Shared.Shell.Avalonia;
using Writer.App.Presentation;
using Writer.App.Presentation.Shell;

namespace Writer.App.Avalonia;

public sealed partial class MainWindow
{
    private async Task OpenExternalHelpLinkAsync(string url, string title)
    {
        var result = await OpenExternalUriAsync(url);
        if (WriterSupportCommandFeedbackPlanner.PlanExternalUriLaunch(result, title, url) is not { } feedback)
            return;

        await WriterInfoDialog.ShowAsync(
            this,
            feedback.Message,
            feedback.Title);
        _editor.Focus();
    }

    private async Task CopyDiagnosticsAsync()
    {
        var diagnosticsDirectory = AppStoragePathPlanner.GetDiagnosticsDirectory(
            PlatformAppDiagnosticsPathProvider.Instance);
        // r169 follow-up: report the file this window ACTUALLY loads and saves. The old path-planner
        // label named %LOCALAPPDATA%\Writer\options.json -- wrong twice over, since Writer keeps its
        // options in settings.json under %APPDATA% (the planner resolves FreeX's file name). Support
        // reports were pointing at a path no Writer install has ever had.
        var optionsPath = _optionsStore.StorePath;
        var diagnosticsText = WriterProductInfo.CreateDiagnosticsText(
            typeof(MainWindow).Assembly,
            diagnosticsDirectory,
            optionsPath);
        var write = await _platformClipboard.WriteAsync(
            new PlatformClipboardContent(Text: diagnosticsText));
        var feedback = WriterSupportCommandFeedbackPlanner.PlanDiagnosticsCopy(write);
        await ShowHelpMessageAsync(feedback.Message, feedback.Title);
    }

    private Task TestCrashReportingAsync() =>
        ShowHelpMessageAsync(
            AppCrashAnalyticsRuntime.UserMessage(AppCrashAnalyticsRuntime.SendTestReport()),
            WriterUiTextCatalog.TestCrashReportingTitle);

    private async Task ShowHelpMessageAsync(string message, string title)
    {
        await WriterInfoDialog.ShowAsync(this, message, title);
        _editor.Focus();
    }

    private static Task<ExternalUriLaunchResult> OpenExternalUriAsync(string target) =>
        Task.FromResult(DesktopExternalUriLauncher.Open(target));
}
