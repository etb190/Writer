using Writer.Shared.AppServices;
using Writer.Shared.IO;
using Writer.App.Presentation.Dialogs;
using Writer.Core.IO;
using Writer.Core.Model;

namespace Writer.App.Presentation.DocumentFragments;

public enum WriterDocumentFragmentImportKind
{
    TextFromFile,
    EmbeddedObject,
}

public sealed record WriterDocumentFragmentPickerPlan(
    string Title,
    IReadOnlyList<FileDialogPickerTypeDescriptor> FileTypes,
    string DefaultExtensionWithDot = "")
{
    public string BuildWpfFilter() => string.Join(
        '|',
        FileTypes.Select(fileType =>
            $"{fileType.DisplayName}|{string.Join(';', fileType.Patterns)}"));
}

public sealed record WriterDocumentFragmentImportRequest(
    WriterDocumentFragmentImportKind Kind,
    string CommandName,
    WriterDocumentFragmentPickerPlan PickerPlan);

public static class WriterDocumentFragmentImportPlanner
{
    private const string DocxMimeType =
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

    public static WriterDocumentFragmentImportRequest CreateTextFromFileRequest() => new(
        WriterDocumentFragmentImportKind.TextFromFile,
        WriterFileTextResources.InsertTextCommand,
        new WriterDocumentFragmentPickerPlan(
            InsertDialogTextResources.TextFromFilePickerTitle,
            [
                new FileDialogPickerTypeDescriptor(
                    WriterFileTextResources.TextFromFileTypeName,
                    ["*.docx", "*.txt"],
                    [DocxMimeType, "text/plain"]),
            ],
            DefaultExtensionWithDot: ".docx"));

    public static WriterDocumentFragmentImportRequest CreateEmbeddedObjectRequest() => new(
        WriterDocumentFragmentImportKind.EmbeddedObject,
        "Insert object",
        new WriterDocumentFragmentPickerPlan(
            "Insert Object",
            [new FileDialogPickerTypeDescriptor("All files (*.*)", ["*.*"])]));
}

public sealed record WriterDocumentFragmentImportSelection(
    string Name,
    string LocalPath,
    object Source);

public sealed record WriterDocumentFragmentPickerResult
{
    private WriterDocumentFragmentPickerResult(
        PickerOutcome<WriterDocumentFragmentImportSelection> outcome)
    {
        Outcome = outcome;
    }

    public PickerOutcome<WriterDocumentFragmentImportSelection> Outcome { get; }
    public OperationStatus Status => Outcome.Status;
    public WriterDocumentFragmentImportSelection? Selection => Outcome.Selection;
    public string? Message => Outcome.Message;

    public static WriterDocumentFragmentPickerResult Selected(
        string name,
        string localPath,
        object source) =>
        new(PickerOutcome<WriterDocumentFragmentImportSelection>.Selected(
            new WriterDocumentFragmentImportSelection(name, localPath, source)));

    public static WriterDocumentFragmentPickerResult Cancelled { get; } =
        new(PickerOutcome<WriterDocumentFragmentImportSelection>.Cancelled);

    public static WriterDocumentFragmentPickerResult Unavailable(string message) =>
        new(PickerOutcome<WriterDocumentFragmentImportSelection>.Unavailable(message));
}

public interface IWriterDocumentFragmentPickerPort
{
    Task<WriterDocumentFragmentPickerResult> PickAsync(
        WriterDocumentFragmentImportRequest request,
        CancellationToken cancellationToken);
}

public interface IWriterDocumentFragmentSourceReaderPort
{
    Task<byte[]> ReadBytesAsync(
        WriterDocumentFragmentImportSelection selection,
        CancellationToken cancellationToken);

    Task<string> ReadTextAsync(
        WriterDocumentFragmentImportSelection selection,
        CancellationToken cancellationToken);

    void ResolveLinkedImagePreviews(
        WriterDocumentFragmentImportSelection selection,
        TextDocument document);
}

/// <summary>
/// Shared local-file reading for renderer adapters. Native hosts retain linked-image preview
/// realization because that depends on renderer-specific image support.
/// </summary>
public abstract class WriterDocumentFragmentFileSourceReaderPort :
    IWriterDocumentFragmentSourceReaderPort
{
    public Task<byte[]> ReadBytesAsync(
        WriterDocumentFragmentImportSelection selection,
        CancellationToken cancellationToken) =>
        FileByteReadWorkflow.ReadLocalPathBytesAsync(
            (string)selection.Source,
            cancellationToken);

    public Task<string> ReadTextAsync(
        WriterDocumentFragmentImportSelection selection,
        CancellationToken cancellationToken) =>
        File.ReadAllTextAsync((string)selection.Source, cancellationToken);

    public abstract void ResolveLinkedImagePreviews(
        WriterDocumentFragmentImportSelection selection,
        TextDocument document);
}

public enum WriterDocumentFragmentInsertionKind
{
    Document,
    PlainText,
    EmbeddedObject,
}

public sealed record WriterDocumentFragmentInsertionRequest(
    WriterDocumentFragmentInsertionKind Kind,
    TextDocument? Document = null,
    string? PlainText = null,
    EmbeddedObject? EmbeddedObject = null)
{
    public static WriterDocumentFragmentInsertionRequest ForDocument(TextDocument document) =>
        new(WriterDocumentFragmentInsertionKind.Document, Document: document);

    public static WriterDocumentFragmentInsertionRequest ForPlainText(string text) =>
        new(WriterDocumentFragmentInsertionKind.PlainText, PlainText: text);

    public static WriterDocumentFragmentInsertionRequest ForEmbeddedObject(EmbeddedObject embeddedObject) =>
        new(WriterDocumentFragmentInsertionKind.EmbeddedObject, EmbeddedObject: embeddedObject);
}

public sealed record WriterDocumentFragmentInsertionResult(bool Applied, string? Message = null)
{
    public static WriterDocumentFragmentInsertionResult Success { get; } = new(true);

    public static WriterDocumentFragmentInsertionResult NotApplied(string? message = null) =>
        new(false, message);
}

public interface IWriterDocumentFragmentInsertionPort
{
    WriterDocumentFragmentInsertionResult Insert(WriterDocumentFragmentInsertionRequest request);
}

public enum WriterDocumentFragmentImportStatus
{
    Succeeded,
    Cancelled,
    Unavailable,
    UnsupportedFormat,
    NotApplied,
    Failed,
}

public sealed record WriterDocumentFragmentImportResult(
    WriterDocumentFragmentImportRequest Request,
    WriterDocumentFragmentImportStatus Status,
    string? SourceName = null,
    string? SourceExtension = null,
    WriterDocumentFragmentInsertionRequest? Insertion = null,
    string? Message = null,
    Exception? Exception = null);

public enum WriterDocumentFragmentImportFailureSurface
{
    AvaloniaStatus,
    WpfModalError,
    None,
}

public sealed record WriterDocumentFragmentImportOutcomePresentation(
    string? StatusText = null,
    string? ModalTitle = null,
    string? ModalMessage = null)
{
    public static WriterDocumentFragmentImportOutcomePresentation Empty { get; } = new();
}

public static class WriterDocumentFragmentImportOutcomePlanner
{
    public static WriterDocumentFragmentImportOutcomePresentation Plan(
        WriterDocumentFragmentImportResult result,
        SisterAppFileTextSpec fileText,
        WriterDocumentFragmentImportFailureSurface failureSurface)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(fileText);

        if (result.Status is WriterDocumentFragmentImportStatus.Succeeded
            or WriterDocumentFragmentImportStatus.Cancelled
            or WriterDocumentFragmentImportStatus.NotApplied
            || failureSurface == WriterDocumentFragmentImportFailureSurface.None)
        {
            return WriterDocumentFragmentImportOutcomePresentation.Empty;
        }

        var reason = result.Message ?? string.Empty;
        if (failureSurface == WriterDocumentFragmentImportFailureSurface.WpfModalError)
        {
            var subject = result.Request.Kind == WriterDocumentFragmentImportKind.TextFromFile
                ? "file"
                : "object";
            return new WriterDocumentFragmentImportOutcomePresentation(
                ModalTitle: "Writer",
                ModalMessage: $"Could not insert the {subject}:\n{reason}");
        }

        if (failureSurface != WriterDocumentFragmentImportFailureSurface.AvaloniaStatus)
            throw new ArgumentOutOfRangeException(nameof(failureSurface), failureSurface, null);

        if (result.Request.Kind == WriterDocumentFragmentImportKind.EmbeddedObject)
        {
            return new WriterDocumentFragmentImportOutcomePresentation(
                StatusText: $"Could not insert the object: {reason}");
        }

        return new WriterDocumentFragmentImportOutcomePresentation(
            StatusText: result.Status switch
            {
                WriterDocumentFragmentImportStatus.UnsupportedFormat =>
                    SisterAppFileTextPlanner.FormatUnsupportedFileType(
                        fileText,
                        result.Request.CommandName,
                        result.SourceExtension ?? string.Empty),
                WriterDocumentFragmentImportStatus.Unavailable =>
                    SisterAppFileTextPlanner.FormatCommandUnavailable(fileText, result.Request.CommandName),
                _ => SisterAppFileTextPlanner.FormatCommandFailed(
                    fileText,
                    result.Request.CommandName,
                    reason),
            });
    }
}

/// <summary>
/// Owns text/object selection policy, parsing, package creation, insertion requests, and outcomes.
/// Native hosts retain picker, file access, editor, focus, dialog, and status realization.
/// </summary>
public sealed class WriterDocumentFragmentImportWorkflow
{
    private readonly IReadOnlyList<IDocumentFileAdapter> _documentAdapters;
    private readonly IWriterDocumentFragmentPickerPort _picker;
    private readonly IWriterDocumentFragmentSourceReaderPort _reader;
    private readonly IWriterDocumentFragmentInsertionPort _insertion;

    public WriterDocumentFragmentImportWorkflow(
        IEnumerable<IDocumentFileAdapter> documentAdapters,
        IWriterDocumentFragmentPickerPort picker,
        IWriterDocumentFragmentSourceReaderPort reader,
        IWriterDocumentFragmentInsertionPort insertion)
    {
        ArgumentNullException.ThrowIfNull(documentAdapters);
        _documentAdapters = documentAdapters.ToArray();
        _picker = picker ?? throw new ArgumentNullException(nameof(picker));
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _insertion = insertion ?? throw new ArgumentNullException(nameof(insertion));
    }

    public async Task<WriterDocumentFragmentImportResult> ImportAsync(
        WriterDocumentFragmentImportRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var pickerResult = await _picker.PickAsync(request, cancellationToken);
            if (pickerResult.Status == OperationStatus.Cancelled)
                return new WriterDocumentFragmentImportResult(request, WriterDocumentFragmentImportStatus.Cancelled);
            if (pickerResult.Status == OperationStatus.Unavailable)
            {
                return new WriterDocumentFragmentImportResult(
                    request,
                    WriterDocumentFragmentImportStatus.Unavailable,
                    Message: pickerResult.Message);
            }

            var selection = pickerResult.Selection
                ?? throw new InvalidOperationException("The document-fragment picker did not return a selection.");
            var extension = Path.GetExtension(selection.LocalPath);
            var insertionRequest = request.Kind switch
            {
                WriterDocumentFragmentImportKind.TextFromFile =>
                    await BuildTextInsertionAsync(selection, extension, cancellationToken),
                WriterDocumentFragmentImportKind.EmbeddedObject =>
                    await BuildObjectInsertionAsync(selection, cancellationToken),
                _ => throw new ArgumentOutOfRangeException(nameof(request), request.Kind, null),
            };

            if (insertionRequest is null)
            {
                return new WriterDocumentFragmentImportResult(
                    request,
                    WriterDocumentFragmentImportStatus.UnsupportedFormat,
                    selection.Name,
                    extension);
            }

            var insertionResult = _insertion.Insert(insertionRequest);
            return new WriterDocumentFragmentImportResult(
                request,
                insertionResult.Applied
                    ? WriterDocumentFragmentImportStatus.Succeeded
                    : WriterDocumentFragmentImportStatus.NotApplied,
                selection.Name,
                extension,
                insertionRequest,
                insertionResult.Message);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            return new WriterDocumentFragmentImportResult(
                request,
                WriterDocumentFragmentImportStatus.Cancelled,
                Exception: ex);
        }
        catch (Exception ex)
        {
            return new WriterDocumentFragmentImportResult(
                request,
                WriterDocumentFragmentImportStatus.Failed,
                Message: ex.Message,
                Exception: ex);
        }
    }

    private async Task<WriterDocumentFragmentInsertionRequest?> BuildTextInsertionAsync(
        WriterDocumentFragmentImportSelection selection,
        string extension,
        CancellationToken cancellationToken)
    {
        if (string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase))
        {
            var text = await _reader.ReadTextAsync(selection, cancellationToken);
            return WriterDocumentFragmentInsertionRequest.ForPlainText(text);
        }

        var adapter = DocumentFileFormatResolver.FindOpenAdapter(
            _documentAdapters,
            extension,
            out _);
        if (adapter is null)
            return null;

        var bytes = await _reader.ReadBytesAsync(selection, cancellationToken);
        using var stream = new MemoryStream(bytes, writable: false);
        var document = adapter.Load(stream);
        _reader.ResolveLinkedImagePreviews(selection, document);
        return WriterDocumentFragmentInsertionRequest.ForDocument(document);
    }

    /// <summary>Word-typical on-page icon size (points) for a Writer-authored Package object's presentation.</summary>
    private const double ObjectIconSizePt = 96;

    private async Task<WriterDocumentFragmentInsertionRequest> BuildObjectInsertionAsync(
        WriterDocumentFragmentImportSelection selection,
        CancellationToken cancellationToken)
    {
        var bytes = await _reader.ReadBytesAsync(selection, cancellationToken);
        var payload = OlePackagePayloadBuilder.Create(selection.Name, selection.LocalPath, bytes);

        // A Writer-authored object always carries an on-page icon (see EmbeddedObject.cs's design
        // comment): when the picked file's own bytes are a recognised raster image, they become a
        // real thumbnail; otherwise the icon carries no raster but still names the source file, so
        // EmbeddedObjectVisualPlanner's AltText fallback announces "budget.xlsx" instead of the bare
        // ProgID to screen readers (proven by EmbeddedObjectVisualPlannerTests).
        var icon = InlineImage.HasRecognisedSignature(bytes)
            ? new InlineImage(bytes, ObjectIconSizePt, ObjectIconSizePt, InlineImage.DetectFormat(bytes))
            : new InlineImage([], ObjectIconSizePt, ObjectIconSizePt);
        icon.AltText = selection.Name;

        return WriterDocumentFragmentInsertionRequest.ForEmbeddedObject(
            EmbeddedObject.Create(payload, OlePackagePayloadBuilder.ProgId, icon));
    }
}
