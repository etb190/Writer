using Writer.App.Avalonia.Editing;
using Writer.Shared.AppServices.Printing;
using Writer.Shared.Pdf;
using Writer.Shared.Pdf.Skia;
using Writer.App.Presentation.Shell;

namespace Writer.App.Avalonia.Pdf;

/// <summary>Result of an Avalonia Writer PDF export: the page count plus the backend used.</summary>
public sealed record WriterAvaloniaPdfExportResult(
    int PageCount,
    PdfExportBackend Backend,
    IReadOnlyList<string> ImageDiagnostics);

/// <summary>
/// Writer's Avalonia (Linux/macOS) PDF export. It mirrors FreeX's Avalonia routing: build the shared
/// app-agnostic <see cref="PdfContentDocument"/> from the editor layout
/// (<see cref="DocumentView.BuildPdfContent"/>) and prefer the Unicode-capable
/// <see cref="SkiaPdfWriter"/> (auto font embedding); fall back to the dependency-free
/// <see cref="PortablePdfWriter"/> (WinAnsi) when the Skia native asset is missing
/// (headless / no-Skia environments).
/// </summary>
public static class WriterAvaloniaPdfExport
{
    public static WriterAvaloniaPdfExportResult Save(DocumentView view, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanWrite)
            throw new ArgumentException("PDF export requires a writable stream.", nameof(stream));

        var document = view.BuildPdfContent();
        return Write(document, stream);
    }

    public static WriterAvaloniaPdfExportResult Save(
        DocumentView view,
        Stream stream,
        PrintSelection selection)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(selection);
        if (!stream.CanWrite)
            throw new ArgumentException("PDF export requires a writable stream.", nameof(stream));

        return Write(WriterFixedLayoutPdfPlanner.Apply(view.BuildPdfContent(), selection), stream);
    }

    private static WriterAvaloniaPdfExportResult Write(PdfContentDocument document, Stream stream)
    {
        // Populated by the shared writer when an embedded picture's bytes cannot be decoded (corrupt
        // or an unrecognized format): that image is silently omitted from the page unless this sink
        // catches the diagnostic, so callers can surface the loss instead of the export looking clean.
        var imageDiagnostics = new List<string>();

        // Skia shapes (HarfBuzz) and automatically embeds/subsets the fonts it draws, so non-WinAnsi
        // text exports correctly without bundling a font. When the Skia native asset is missing it
        // throws on first use; we then fall back to the dependency-free WinAnsi writer.
        var result = PdfBackendFallbackExecutor.Execute(
            stream,
            target => SkiaPdfWriter.Write(document, target, imageDiagnostics),
            target =>
            {
                imageDiagnostics.Clear();
                var bytes = PortablePdfWriter.WriteToBytes(
                    document,
                    "Writer portable PDF",
                    imageDiagnostics);
                target.Write(bytes);
                return document.Pages.Count;
            });

        return new WriterAvaloniaPdfExportResult(result.Result, result.Backend, imageDiagnostics);
    }
}
