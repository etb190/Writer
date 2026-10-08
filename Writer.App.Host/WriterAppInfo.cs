using Writer.App.Presentation;

namespace Writer.App.Host;

public static class WriterAppInfo
{
    public static AboutDialogPresentation AboutPresentation { get; } =
        WriterAboutDialogPresentation.Create(typeof(WriterAppInfo).Assembly);

    public static string AboutText => AboutPresentation.AboutText;

    public static string FeedbackUrl => WriterProductInfo.CreateFeedbackUrl(typeof(WriterAppInfo).Assembly);

    public static string CreateDiagnosticsText(string diagnosticsDirectory, string optionsPath) =>
        WriterProductInfo.CreateDiagnosticsText(
            typeof(WriterAppInfo).Assembly,
            diagnosticsDirectory,
            optionsPath);
}
