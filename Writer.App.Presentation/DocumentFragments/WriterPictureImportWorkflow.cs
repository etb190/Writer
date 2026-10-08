using Writer.Shared.AppServices;
using Writer.Shared.IO;
using Writer.App.Presentation.Dialogs;
using Writer.Core.Model;

namespace Writer.App.Presentation.DocumentFragments;

public enum WriterPictureImportSourceKind
{
    PreservedRaster,
    Svg,
    NativeRasterization,
}

public sealed record WriterPictureImportPickerPlan(
    string Title,
    FileDialogPickerTypeDescriptor PictureFiles,
    bool IncludeAllFiles)
{
    public IReadOnlyList<FileDialogPickerTypeDescriptor> FileTypes => IncludeAllFiles
        ? [PictureFiles, new FileDialogPickerTypeDescriptor("All files", ["*.*"])]
        : [PictureFiles];

    public string BuildWpfFilter()
    {
        var patterns = string.Join(';', PictureFiles.Patterns);
        var pictureFilter = $"{PictureFiles.DisplayName}|{patterns}";
        return IncludeAllFiles
            ? $"{pictureFilter}|{FileDialogFilterBuilder.AllFilesFilterEntry}"
            : pictureFilter;
    }
}

public sealed record WriterPictureImportSizingPolicy(
    double ReferenceDpi = 96,
    double FallbackWidthPt = 200,
    double FallbackHeightPt = 150,
    double MaximumLongEdgePt = 400,
    int VectorRasterMaximumPixelEdge = 400);

public sealed record WriterPictureImportRequest(
    string CommandName,
    WriterPictureImportPickerPlan PickerPlan,
    WriterPictureImportSizingPolicy SizingPolicy);

public sealed record WriterPictureImportSelection(string Name, object Source);

public sealed record WriterPictureImportPickerResult
{
    private WriterPictureImportPickerResult(PickerOutcome<WriterPictureImportSelection> outcome)
    {
        Outcome = outcome;
    }

    public PickerOutcome<WriterPictureImportSelection> Outcome { get; }
    public OperationStatus Status => Outcome.Status;
    public WriterPictureImportSelection? Selection => Outcome.Selection;
    public string? Message => Outcome.Message;

    public static WriterPictureImportPickerResult Selected(string name, object source) =>
        new(PickerOutcome<WriterPictureImportSelection>.Selected(
            new WriterPictureImportSelection(name, source)));

    public static WriterPictureImportPickerResult Cancelled { get; } =
        new(PickerOutcome<WriterPictureImportSelection>.Cancelled);

    public static WriterPictureImportPickerResult Unavailable(string message) =>
        new(PickerOutcome<WriterPictureImportSelection>.Unavailable(message));
}

public interface IWriterPictureImportPickerPort
{
    Task<WriterPictureImportPickerResult> PickAsync(
        WriterPictureImportRequest request,
        CancellationToken cancellationToken);
}

public interface IWriterPictureImportSourceReaderPort
{
    Task<byte[]> ReadAsync(
        WriterPictureImportSelection selection,
        CancellationToken cancellationToken);
}

public sealed record WriterPictureDecoderFacts(
    int PixelWidth,
    int PixelHeight,
    double SourceDpiX = 96,
    double SourceDpiY = 96)
{
    public static WriterPictureDecoderFacts Unavailable { get; } = new(0, 0, 0, 0);

    public bool HasNaturalSize => PixelWidth > 0 && PixelHeight > 0;
}

public static class WriterPictureDecoderPolicy
{
    public static ValueTask<WriterPictureDecoderFacts> DecodeOrUnavailable(
        CancellationToken cancellationToken,
        Func<WriterPictureDecoderFacts> decode)
    {
        ArgumentNullException.ThrowIfNull(decode);
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            return ValueTask.FromResult(decode());
        }
        catch
        {
            return ValueTask.FromResult(WriterPictureDecoderFacts.Unavailable);
        }
    }
}

public interface IWriterPictureDecoderPort
{
    ValueTask<WriterPictureDecoderFacts> DecodeAsync(
        WriterPictureImportSelection selection,
        byte[] bytes,
        CancellationToken cancellationToken);
}

public sealed record WriterPictureRasterizationRequest(
    WriterPictureImportSelection Selection,
    byte[] SourceBytes,
    WriterPictureImportSourceKind SourceKind,
    int MaximumPixelEdge);

public sealed record WriterPictureRasterizationOutcome(
    byte[] Bytes,
    WriterPictureDecoderFacts DecoderFacts,
    ImageFormat Format = ImageFormat.Png);

public interface IWriterPictureRasterizerPort
{
    ValueTask<WriterPictureRasterizationOutcome> RasterizeAsync(
        WriterPictureRasterizationRequest request,
        CancellationToken cancellationToken);
}

public sealed record WriterPictureImportSizingPlan(
    double WidthPt,
    double HeightPt,
    double EffectiveDpiX,
    double EffectiveDpiY,
    bool UsedFallbackSize,
    bool WasScaled);

public sealed record WriterPictureInsertionRequest(
    byte[] Bytes,
    ImageFormat Format,
    double WidthPt,
    double HeightPt,
    int OriginalPixelWidth,
    int OriginalPixelHeight);

public sealed record WriterPictureInsertionResult(bool Applied, string? Message = null)
{
    public static WriterPictureInsertionResult Success { get; } = new(true);

    public static WriterPictureInsertionResult NotApplied(string? message = null) =>
        new(false, message);
}

public interface IWriterPictureInsertionPort
{
    WriterPictureInsertionResult Insert(WriterPictureInsertionRequest request);
}

public enum WriterPictureImportStatus
{
    Succeeded,
    Cancelled,
    Unavailable,
    NotApplied,
    Failed,
}

public sealed record WriterPictureImportResult(
    WriterPictureImportRequest Request,
    WriterPictureImportStatus Status,
    string? SourceName = null,
    WriterPictureInsertionRequest? Insertion = null,
    string? Message = null,
    Exception? Exception = null);

public enum WriterPictureImportFailureSurface
{
    Status,
    ModalError,
    None,
}

public sealed record WriterPictureImportOutcomePresentation(
    string? StatusText = null,
    string? ModalTitle = null,
    string? ModalMessage = null)
{
    public static WriterPictureImportOutcomePresentation Empty { get; } = new();
}

public static class WriterPictureImportPlanner
{
    // Round 172 (freep-media F1 follow-up): wmf/emf belong here. Unlike FreeX -- which has no metafile
    // decoder and therefore rejects them -- Writer carries metafiles end to end: ImageFormat.Emf/Wmf,
    // InlineImage.FormatForExtension/ExtensionFor, the DocxWriter media part + [Content_Types] Default
    // (OoxmlWordprocessing.ImageContentTypeForExtension -> image/x-emf / image/x-wmf), and the WPF
    // renderer's GDI+ metafile path. Omitting them from the picker meant a user could only reach that
    // working path through the picker's "All files" entry, which made a first-class supported format
    // look unsupported. Mirrors FreeP's picture profile (PresentationAssetPickerProfileCatalog).
    private static readonly IReadOnlyList<string> PicturePatterns =
        ["*.png", "*.jpg", "*.jpeg", "*.gif", "*.bmp", "*.tif", "*.tiff", "*.svg", "*.wmf", "*.emf"];

    private static readonly IReadOnlyList<string> PictureMimeTypes =
    [
        "image/png",
        "image/jpeg",
        "image/gif",
        "image/bmp",
        "image/tiff",
        "image/svg+xml",
        "image/x-wmf",
        "image/x-emf",
    ];

    public static WriterPictureImportRequest CreateRequest() =>
        new(
            WriterFileTextResources.Document.InsertPictureCommand,
            new WriterPictureImportPickerPlan(
                WriterFileTextResources.Document.InsertPicturePickerTitle,
                new FileDialogPickerTypeDescriptor(
                    WriterFileTextResources.PictureFileTypeName,
                    PicturePatterns,
                    PictureMimeTypes),
                IncludeAllFiles: true),
            new WriterPictureImportSizingPolicy());

    public static WriterPictureImportSourceKind ClassifySource(string sourceName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceName);

        var extension = Path.GetExtension(sourceName);
        if (string.Equals(extension, ".svg", StringComparison.OrdinalIgnoreCase))
            return WriterPictureImportSourceKind.Svg;

        return InlineImage.FormatForExtension(extension) is not null
            ? WriterPictureImportSourceKind.PreservedRaster
            : WriterPictureImportSourceKind.NativeRasterization;
    }

    public static ImageFormat ResolvePreservedFormat(string sourceName) =>
        InlineImage.FormatForExtension(Path.GetExtension(sourceName))
        ?? throw new ArgumentException("The selected picture does not have a preservable image format.", nameof(sourceName));

    public static WriterPictureImportSizingPlan PlanSize(
        WriterPictureDecoderFacts facts,
        WriterPictureImportSizingPolicy? policy = null)
    {
        ArgumentNullException.ThrowIfNull(facts);
        policy ??= new WriterPictureImportSizingPolicy();
        Validate(policy);

        if (!facts.HasNaturalSize)
        {
            return new WriterPictureImportSizingPlan(
                policy.FallbackWidthPt,
                policy.FallbackHeightPt,
                policy.ReferenceDpi,
                policy.ReferenceDpi,
                UsedFallbackSize: true,
                WasScaled: false);
        }

        // Desktop image insertion has historically interpreted pixels at 96 DPI in both renderers.
        // Source DPI remains in the decoder facts for diagnostics, while one reference DPI keeps parity.
        var widthPt = facts.PixelWidth * 72.0 / policy.ReferenceDpi;
        var heightPt = facts.PixelHeight * 72.0 / policy.ReferenceDpi;
        var longestEdge = Math.Max(widthPt, heightPt);
        var wasScaled = longestEdge > policy.MaximumLongEdgePt;
        if (wasScaled)
        {
            var scale = policy.MaximumLongEdgePt / longestEdge;
            widthPt *= scale;
            heightPt *= scale;
        }

        return new WriterPictureImportSizingPlan(
            widthPt,
            heightPt,
            policy.ReferenceDpi,
            policy.ReferenceDpi,
            UsedFallbackSize: false,
            WasScaled: wasScaled);
    }

    private static void Validate(WriterPictureImportSizingPolicy policy)
    {
        if (!IsPositiveFinite(policy.ReferenceDpi))
            throw new ArgumentOutOfRangeException(nameof(policy), "Reference DPI must be positive and finite.");
        if (!IsPositiveFinite(policy.FallbackWidthPt) || !IsPositiveFinite(policy.FallbackHeightPt))
            throw new ArgumentOutOfRangeException(nameof(policy), "Fallback dimensions must be positive and finite.");
        if (!IsPositiveFinite(policy.MaximumLongEdgePt))
            throw new ArgumentOutOfRangeException(nameof(policy), "Maximum size must be positive and finite.");
        if (policy.VectorRasterMaximumPixelEdge <= 0)
            throw new ArgumentOutOfRangeException(nameof(policy), "Vector raster size must be positive.");
    }

    private static bool IsPositiveFinite(double value) => value > 0 && double.IsFinite(value);
}

public static class WriterPictureImportOutcomePlanner
{
    public static WriterPictureImportOutcomePresentation Plan(
        WriterPictureImportResult result,
        SisterAppFileTextSpec fileText,
        WriterPictureImportFailureSurface failureSurface)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(fileText);

        if (result.Status is WriterPictureImportStatus.Succeeded
            or WriterPictureImportStatus.Cancelled
            or WriterPictureImportStatus.NotApplied
            || failureSurface == WriterPictureImportFailureSurface.None)
        {
            return WriterPictureImportOutcomePresentation.Empty;
        }

        var reason = result.Message ?? string.Empty;
        return failureSurface switch
        {
            WriterPictureImportFailureSurface.Status =>
                new WriterPictureImportOutcomePresentation(
                    StatusText: result.Status == WriterPictureImportStatus.Unavailable
                        ? SisterAppFileTextPlanner.FormatCommandUnavailable(fileText, result.Request.CommandName)
                        : SisterAppFileTextPlanner.FormatCommandFailed(fileText, result.Request.CommandName, reason)),
            WriterPictureImportFailureSurface.ModalError =>
                new WriterPictureImportOutcomePresentation(
                    ModalTitle: "Writer",
                    ModalMessage: $"Could not insert the image:\n{reason}"),
            _ => throw new ArgumentOutOfRangeException(nameof(failureSurface), failureSurface, null),
        };
    }
}

/// <summary>
/// Owns picture selection, format policy, sizing, rasterization decisions, and insertion outcomes.
/// Native hosts only implement file, codec, raster, editor, focus, and feedback realization.
/// </summary>
public sealed class WriterPictureImportWorkflow
{
    private readonly IWriterPictureImportPickerPort _picker;
    private readonly IWriterPictureImportSourceReaderPort _reader;
    private readonly IWriterPictureDecoderPort _decoder;
    private readonly IWriterPictureRasterizerPort _rasterizer;
    private readonly IWriterPictureInsertionPort _insertion;

    public WriterPictureImportWorkflow(
        IWriterPictureImportPickerPort picker,
        IWriterPictureImportSourceReaderPort reader,
        IWriterPictureDecoderPort decoder,
        IWriterPictureRasterizerPort rasterizer,
        IWriterPictureInsertionPort insertion)
    {
        _picker = picker ?? throw new ArgumentNullException(nameof(picker));
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
        _rasterizer = rasterizer ?? throw new ArgumentNullException(nameof(rasterizer));
        _insertion = insertion ?? throw new ArgumentNullException(nameof(insertion));
    }

    public async Task<WriterPictureImportResult> ImportAsync(
        CancellationToken cancellationToken = default)
    {
        var request = WriterPictureImportPlanner.CreateRequest();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var pickerResult = await _picker.PickAsync(request, cancellationToken);
            if (pickerResult.Status == OperationStatus.Cancelled)
                return new WriterPictureImportResult(request, WriterPictureImportStatus.Cancelled);
            if (pickerResult.Status == OperationStatus.Unavailable)
            {
                return new WriterPictureImportResult(
                    request,
                    WriterPictureImportStatus.Unavailable,
                    Message: pickerResult.Message);
            }

            var selection = pickerResult.Selection
                ?? throw new InvalidOperationException("The picture picker did not return a selection.");
            var sourceBytes = await _reader.ReadAsync(selection, cancellationToken);
            var sourceKind = WriterPictureImportPlanner.ClassifySource(selection.Name);

            byte[] insertionBytes;
            ImageFormat insertionFormat;
            WriterPictureDecoderFacts decoderFacts;
            if (sourceKind == WriterPictureImportSourceKind.PreservedRaster)
            {
                insertionBytes = sourceBytes;
                insertionFormat = WriterPictureImportPlanner.ResolvePreservedFormat(selection.Name);
                decoderFacts = await _decoder.DecodeAsync(selection, sourceBytes, cancellationToken);
            }
            else
            {
                var rasterized = await _rasterizer.RasterizeAsync(
                    new WriterPictureRasterizationRequest(
                        selection,
                        sourceBytes,
                        sourceKind,
                        request.SizingPolicy.VectorRasterMaximumPixelEdge),
                    cancellationToken);
                insertionBytes = rasterized.Bytes;
                insertionFormat = rasterized.Format;
                decoderFacts = rasterized.DecoderFacts;
            }

            if (insertionBytes.Length == 0)
                throw new InvalidDataException("The selected picture produced no image data.");

            var size = WriterPictureImportPlanner.PlanSize(decoderFacts, request.SizingPolicy);
            var insertionRequest = new WriterPictureInsertionRequest(
                insertionBytes,
                insertionFormat,
                size.WidthPt,
                size.HeightPt,
                Math.Max(0, decoderFacts.PixelWidth),
                Math.Max(0, decoderFacts.PixelHeight));
            var insertionResult = _insertion.Insert(insertionRequest);
            return new WriterPictureImportResult(
                request,
                insertionResult.Applied
                    ? WriterPictureImportStatus.Succeeded
                    : WriterPictureImportStatus.NotApplied,
                selection.Name,
                insertionRequest,
                insertionResult.Message);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            return new WriterPictureImportResult(
                request,
                WriterPictureImportStatus.Cancelled,
                Exception: ex);
        }
        catch (Exception ex)
        {
            return new WriterPictureImportResult(
                request,
                WriterPictureImportStatus.Failed,
                Message: ex.Message,
                Exception: ex);
        }
    }
}
