using Writer.Shared.AppServices;
using Writer.Shared.AppServices.Printing;
using Writer.Shared.Shell;
using Writer.App.Presentation.Backstage;
using Writer.App.Presentation.Dialogs;
using Writer.Core.IO;
using Writer.Core.Model;

namespace Writer.App.Presentation.Shell;

public enum WriterExportFormat
{
    Pdf,
    Xps,
}

public sealed record WriterExportRequestPlan(
    WriterExportFormat Format,
    string PickerTitle,
    string CommandName,
    FileDialogPickerTypeDescriptor FileType,
    string SuggestedFileName,
    string DefaultExtensionWithDot,
    string Filter)
{
    public string DefaultExtensionWithoutDot => DefaultExtensionWithDot.TrimStart('.');
}

public sealed record WriterExportArtifact(
    int? PageCount = null,
    string? Backend = null,
    int ImageWarningCount = 0);

public enum WriterExportExecutionOutcome
{
    Succeeded,
    Canceled,
    Failed,
}

public sealed record WriterExportExecutionResult(
    WriterExportExecutionOutcome Outcome,
    WriterExportRequestPlan Plan,
    string? Path,
    string Message,
    WriterExportArtifact? Artifact = null,
    Exception? Exception = null)
{
    public bool Succeeded => Outcome == WriterExportExecutionOutcome.Succeeded;
}

public static class WriterExportWorkflow
{
    public static WriterExportRequestPlan CreatePlan(WriterExportFormat format, string? displayName)
    {
        var baseName = string.IsNullOrWhiteSpace(displayName)
            ? DocumentPersistenceWorkflow.DefaultFallbackDisplayName
            : Path.GetFileNameWithoutExtension(displayName.Trim());
        var isPdf = format == WriterExportFormat.Pdf;
        var extension = isPdf ? ".pdf" : ".xps";
        var typeName = isPdf
            ? WriterFileTextResources.PdfFileTypeName
            : WriterFileTextResources.XpsFileTypeName;
        return new(
            format,
            isPdf ? WriterFileTextResources.ExportPdfPickerTitle : WriterFileTextResources.ExportXpsPickerTitle,
            isPdf ? WriterFileTextResources.PdfExportCommand : WriterFileTextResources.XpsExportCommand,
            new FileDialogPickerTypeDescriptor(
                typeName,
                [$"*{extension}"],
                isPdf
                    ? ["application/pdf"]
                    : ["application/oxps", "application/vnd.ms-xpsdocument"]),
            baseName + extension,
            extension,
            $"{typeName} (*{extension})|*{extension}");
    }

    public static async Task<WriterExportExecutionResult> ExecuteAsync(
        WriterExportRequestPlan plan,
        string path,
        Func<Stream, CancellationToken, ValueTask<WriterExportArtifact>> renderAsync,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(renderAsync);

        // ConfigureAwait(false) is required here: both Writer.App.Host (WPF) call sites invoke
        // this whole chain via `.GetAwaiter().GetResult()` on the UI thread. AtomicExportExecutor
        // opens the destination temp file with real async I/O (FileOptions.Asynchronous); if that
        // write genuinely completes asynchronously, an un-configured await here would try to
        // resume by posting its continuation back to the captured WPF
        // DispatcherSynchronizationContext -- but that dispatcher is the very UI thread blocked
        // in GetResult(), so the app hangs forever. AtomicExportExecutor's own internals already
        // ConfigureAwait(false) throughout; this call site was the missing link.
        var execution = await new AtomicExportExecutor().ExecuteAsync<WriterExportArtifact>(
            path,
            (output, token) => renderAsync(output, token),
            cancellationToken).ConfigureAwait(false);
        if (execution.Succeeded)
        {
            var artifact = execution.Value!;
            return new(
                WriterExportExecutionOutcome.Succeeded,
                plan,
                execution.Path,
                FormatSuccess(plan, execution.Path!, artifact),
                artifact);
        }

        if (execution.Cancelled)
        {
            return new(
                WriterExportExecutionOutcome.Canceled,
                plan,
                execution.Path ?? path,
                $"{plan.CommandName} canceled.",
                Exception: execution.Exception);
        }

        var exception = execution.Exception ?? new IOException(
            execution.Error?.Detail.Message ??
            execution.Validation?.Detail.ToString() ??
            "Export did not complete.");
        return new(
            WriterExportExecutionOutcome.Failed,
            plan,
            execution.Path ?? path,
            SisterAppFileTextPlanner.FormatCommandFailed(
                WriterFileTextResources.Document,
                plan.CommandName,
                exception.Message),
            Exception: exception);
    }

    private static string FormatSuccess(
        WriterExportRequestPlan plan,
        string path,
        WriterExportArtifact artifact)
    {
        if (plan.Format == WriterExportFormat.Pdf &&
            artifact.PageCount is { } pageCount &&
            artifact.Backend is { } backend)
        {
            return WriterFileTextResources.FormatPdfExported(
                pageCount,
                backend,
                Path.GetFileName(path),
                artifact.ImageWarningCount);
        }

        return plan.Format == WriterExportFormat.Xps
            ? WriterFileTextResources.FormatXpsExported(path)
            : $"Exported to PDF: {path}";
    }
}

public sealed record WriterPrintRequestPlan(
    string Description,
    int TotalPages,
    double PageWidthDip,
    double PageHeightDip,
    PrintSelection Selection);

public static class WriterPrintRequestPlanner
{
    public static WriterPrintRequestPlan Create(
        string description,
        PageSettings page,
        int totalPages,
        PrintSelection? selection = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentNullException.ThrowIfNull(page);
        selection ??= new PrintSelection();
        selection.Validate();
        var (pageWidth, pageHeight) = PageLayout.PageSizeDip(page);
        return new(description, Math.Max(1, totalPages), pageWidth, pageHeight, selection);
    }

    public static (int FirstPage, int LastPage) ResolvePageRange(
        PrintPageRange range,
        int totalPages)
    {
        ArgumentNullException.ThrowIfNull(range);
        range.Validate();
        var lastDocumentPage = Math.Max(1, totalPages);
        return range.Kind switch
        {
            PrintPageRangeKind.All => (1, lastDocumentPage),
            PrintPageRangeKind.Single =>
                (Math.Clamp(range.FirstPage!.Value, 1, lastDocumentPage),
                 Math.Clamp(range.FirstPage.Value, 1, lastDocumentPage)),
            _ => ResolveBoundedRange(range, lastDocumentPage),
        };
    }

    public static PrintPageRange FromOneBasedRange(int firstPage, int lastPage, int totalPages)
    {
        var lastDocumentPage = Math.Max(1, totalPages);
        var first = Math.Clamp(firstPage, 1, lastDocumentPage);
        var last = Math.Clamp(lastPage, first, lastDocumentPage);
        return first == last ? PrintPageRange.Single(first) : PrintPageRange.Between(first, last);
    }

    private static (int FirstPage, int LastPage) ResolveBoundedRange(
        PrintPageRange range,
        int lastDocumentPage)
    {
        var first = Math.Clamp(range.FirstPage!.Value, 1, lastDocumentPage);
        return (first, Math.Clamp(range.LastPage!.Value, first, lastDocumentPage));
    }
}

public static class WriterPrintMessagePlanner
{
    public const string Canceled = "Print canceled.";
    private const string Fallback = "Use Print Preview or Create PDF.";

    public static string FormatDiscovery(PrinterDiscoveryResult discovery)
    {
        ArgumentNullException.ThrowIfNull(discovery);
        return discovery.Status switch
        {
            PrinterDiscoveryStatus.NoPrinters =>
                $"No printers are installed or available. {Fallback}",
            PrinterDiscoveryStatus.Unavailable =>
                AppendFallback(discovery.Message, "Direct printing is unavailable on this host."),
            PrinterDiscoveryStatus.Failed =>
                AppendFallback(discovery.Message, "Printer discovery failed."),
            PrinterDiscoveryStatus.Cancelled => Canceled,
            _ => $"Direct printing is unavailable. {Fallback}",
        };
    }

    public static string FormatSubmission(PrintSubmissionResult submission)
    {
        ArgumentNullException.ThrowIfNull(submission);
        return submission.Status switch
        {
            PrintSubmissionStatus.Submitted => $"Sent to printer {submission.PrinterName}.",
            PrintSubmissionStatus.Cancelled => Canceled,
            PrintSubmissionStatus.NoPrinters =>
                $"No printers are installed or available. {Fallback}",
            PrintSubmissionStatus.Unavailable =>
                AppendFallback(submission.Message, "Direct printing is unavailable on this host."),
            _ => string.IsNullOrWhiteSpace(submission.Message)
                ? $"Print submission failed. {Fallback}"
                : submission.Message,
        };
    }

    public static string FormatExecution(PortablePrintExecutionResult execution)
    {
        ArgumentNullException.ThrowIfNull(execution);
        if (execution.Submission is { } submission)
            return FormatSubmission(submission);
        if (execution.Discovery is { IsAvailable: false } discovery)
            return FormatDiscovery(discovery);
        if (execution.Status == OperationStatus.Cancelled)
            return Canceled;

        return SisterAppFileTextPlanner.FormatCommandFailed(
            WriterFileTextResources.Document,
            "Print",
            execution.Operation.Exception?.Message ?? "Print submission failed.");
    }

    public static BackstageDirectPrintCapability PlanCapability(
        bool isPrintServiceSupported,
        PrinterDiscoveryResult? discovery)
    {
        if (discovery?.IsAvailable == true)
        {
            return BackstageDirectPrintCapability.PlatformPrinterAvailable(
                "Platform printer discovery and foreground PDF submission are available on this host; no native system print dialog is used.");
        }

        if (!isPrintServiceSupported)
        {
            return BackstageDirectPrintCapability.Deferred(
                $"This host has no supported native printer service; {Fallback}");
        }

        var reason = discovery?.Status switch
        {
            PrinterDiscoveryStatus.NoPrinters => "No usable printer was discovered on this host.",
            PrinterDiscoveryStatus.Unavailable => "The platform printer backend is unavailable on this host.",
            PrinterDiscoveryStatus.Failed => "Platform printer discovery failed on this host.",
            PrinterDiscoveryStatus.Cancelled => "Printer discovery was canceled.",
            _ => "Printer discovery is still in progress.",
        };
        return BackstageDirectPrintCapability.Deferred($"{reason} {Fallback}");
    }

    private static string AppendFallback(string? message, string fallbackMessage) =>
        $"{(string.IsNullOrWhiteSpace(message) ? fallbackMessage : message.Trim())} {Fallback}";
}

public enum WriterPrintPreviewPrimaryAction
{
    None,
    DirectPrint,
    CreatePdf,
}

public sealed record WriterPrintPreviewActionPlan(
    WriterPrintPreviewPrimaryAction Action,
    string Label,
    string Description,
    bool IsEnabled);

public sealed record WriterPrintPreviewState(
    string Title,
    string DisplayName,
    string Description,
    IReadOnlyList<BackstageFieldRow> Fields,
    int CurrentPage,
    int TotalPages,
    string PageCountText,
    PrintSelection Options,
    WriterPrintPreviewActionPlan PrimaryAction);

/// <summary>
/// Renderer-neutral print preview state. Hosts realize the returned page, option, summary, and primary
/// action plans through their native viewer controls and renderer-specific paginated surfaces.
/// </summary>
public sealed class WriterPrintPreviewSession
{
    private readonly string _displayName;
    private readonly BackstagePrintPanePlan _summary;
    private readonly BackstageDirectPrintCapability _capability;
    private readonly bool _canCreatePdf;
    private readonly bool _canDirectPrint;
    private int _currentPage = 1;
    private int _totalPages = 1;
    private PrintSelection _options = new();

    public WriterPrintPreviewSession(
        string? displayName,
        PageSettings page,
        BackstageDirectPrintCapability capability,
        bool canCreatePdf,
        bool canDirectPrint)
    {
        _displayName = string.IsNullOrWhiteSpace(displayName) ? "Untitled" : displayName.Trim();
        _capability = capability ?? throw new ArgumentNullException(nameof(capability));
        _summary = BackstagePrintPanePlanner.Build(_displayName, page, capability);
        _canCreatePdf = canCreatePdf;
        _canDirectPrint = canDirectPrint;
    }

    public WriterPrintPreviewState State => new(
        $"Print Preview - {_displayName}",
        _displayName,
        $"Preview uses the current paginated layout. {_capability.ActionDescription}",
        _summary.Fields,
        _currentPage,
        _totalPages,
        FormatPageCount(_totalPages),
        _options,
        BuildPrimaryAction());

    public WriterPrintPreviewState SetPageCount(int totalPages)
    {
        _totalPages = Math.Max(1, totalPages);
        _currentPage = Math.Clamp(_currentPage, 1, _totalPages);
        _options = _options with
        {
            PageRange = NormalizeRange(_options.EffectivePageRange, _totalPages),
        };
        return State;
    }

    public WriterPrintPreviewState GoToPage(int page)
    {
        _currentPage = Math.Clamp(page, 1, _totalPages);
        return State;
    }

    public WriterPrintPreviewState ApplyOptions(PrintSelection options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();
        _options = options with
        {
            PageRange = NormalizeRange(options.EffectivePageRange, _totalPages),
        };
        return State;
    }

    private WriterPrintPreviewActionPlan BuildPrimaryAction()
    {
        if (_capability.IsAvailable && _canDirectPrint)
        {
            return new(
                WriterPrintPreviewPrimaryAction.DirectPrint,
                "Print",
                _capability.ActionDescription,
                IsEnabled: true);
        }

        if (_canCreatePdf)
        {
            return new(
                WriterPrintPreviewPrimaryAction.CreatePdf,
                BackstageViewTextResources.CreatePdfLabel,
                _capability.DeferredNote ?? _capability.ActionDescription,
                IsEnabled: true);
        }

        return new(
            WriterPrintPreviewPrimaryAction.None,
            BackstageViewTextResources.CreatePdfLabel,
            _capability.DeferredNote ?? _capability.ActionDescription,
            IsEnabled: false);
    }

    private static PrintPageRange NormalizeRange(PrintPageRange range, int totalPages)
    {
        if (range.Kind == PrintPageRangeKind.All)
            return PrintPageRange.All;

        var (first, last) = WriterPrintRequestPlanner.ResolvePageRange(range, totalPages);
        return first == last ? PrintPageRange.Single(first) : PrintPageRange.Between(first, last);
    }

    private static string FormatPageCount(int pages) => pages == 1 ? "1 page" : $"{pages} pages";
}

public static class WriterDocumentSnapshot
{
    public static TextDocument Clone(TextDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        using var buffer = new MemoryStream();
        DocxWriter.Write(document, buffer);
        buffer.Position = 0;
        return DocxReader.Read(buffer);
    }
}
