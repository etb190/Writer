using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Writer.Shared.IO;
using Writer.Shared.Ribbon.Avalonia;
using Writer.Shared.Shell.Avalonia;
using Writer.App.Avalonia.Editing;
using Writer.App.Presentation.DocumentFragments;

namespace Writer.App.Avalonia;

internal sealed class AvaloniaPictureImportPickerPort(IStorageProvider storageProvider)
    : IWriterPictureImportPickerPort
{
    public async Task<WriterPictureImportPickerResult> PickAsync(
        WriterPictureImportRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!AvaloniaFilePickerService.CanOpen(storageProvider))
        {
            return WriterPictureImportPickerResult.Unavailable(
                $"{request.CommandName} is unavailable because this platform cannot open files.");
        }

        var file = await AvaloniaFilePickerService.PickSingleOpenFileAsync(
            storageProvider,
            AvaloniaFilePickerOpenRequest.FromDescriptors(
                request.PickerPlan.Title,
                request.PickerPlan.FileTypes));
        return file is null
            ? WriterPictureImportPickerResult.Cancelled
            : WriterPictureImportPickerResult.Selected(file.Name, file);
    }
}

internal sealed class AvaloniaPictureImportSourceReaderPort : IWriterPictureImportSourceReaderPort
{
    public async Task<byte[]> ReadAsync(
        WriterPictureImportSelection selection,
        CancellationToken cancellationToken)
    {
        if (selection.Source is not IStorageFile file)
            throw new InvalidOperationException("The selected picture is not an Avalonia storage file.");

        try
        {
            return await FileByteReadWorkflow.ReadStreamBytesAsync(
                file.OpenReadAsync,
                cancellationToken);
        }
        finally
        {
            file.Dispose();
        }
    }
}

internal sealed class AvaloniaPictureDecoderPort : IWriterPictureDecoderPort
{
    public ValueTask<WriterPictureDecoderFacts> DecodeAsync(
        WriterPictureImportSelection selection,
        byte[] bytes,
        CancellationToken cancellationToken) =>
        WriterPictureDecoderPolicy.DecodeOrUnavailable(cancellationToken, () =>
        {
            using var source = new MemoryStream(bytes, writable: false);
            using var bitmap = new Bitmap(source);
            return new WriterPictureDecoderFacts(
                bitmap.PixelSize.Width,
                bitmap.PixelSize.Height,
                bitmap.Dpi.X,
                bitmap.Dpi.Y);
        });
}

internal sealed class AvaloniaPictureRasterizerPort : IWriterPictureRasterizerPort
{
    public ValueTask<WriterPictureRasterizationOutcome> RasterizeAsync(
        WriterPictureRasterizationRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(
            request.SourceKind == WriterPictureImportSourceKind.Svg
                ? RasterizeSvg(request)
                : RasterizeNativeBitmap(request.SourceBytes));
    }

    private static WriterPictureRasterizationOutcome RasterizeSvg(
        WriterPictureRasterizationRequest request)
    {
        using var temporaryFile = TemporaryFileLease.Create("writer_picture_", ".svg");
        using (var output = temporaryFile.OpenWrite())
            output.Write(request.SourceBytes);

        var drawing = SvgIconRasterizer.LoadFileToPaintedBounds(temporaryFile.Path);
        var drawingSize = drawing.Size;
        var sourceWidth = drawingSize.Width > 0 ? drawingSize.Width : request.MaximumPixelEdge;
        var sourceHeight = drawingSize.Height > 0 ? drawingSize.Height : request.MaximumPixelEdge;
        var scale = request.MaximumPixelEdge / Math.Max(sourceWidth, sourceHeight);
        var pixelWidth = Math.Max(1, (int)Math.Round(sourceWidth * scale));
        var pixelHeight = Math.Max(1, (int)Math.Round(sourceHeight * scale));

        var image = new Image
        {
            Source = drawing,
            Width = pixelWidth,
            Height = pixelHeight,
            Stretch = Stretch.Uniform,
        };
        var size = new Size(pixelWidth, pixelHeight);
        image.Measure(size);
        image.Arrange(new Rect(size));

        using var bitmap = new RenderTargetBitmap(
            new PixelSize(pixelWidth, pixelHeight),
            new Vector(96, 96));
        bitmap.Render(image);
        using var stream = new MemoryStream();
        bitmap.Save(stream);
        return new WriterPictureRasterizationOutcome(
            stream.ToArray(),
            new WriterPictureDecoderFacts(pixelWidth, pixelHeight, 96, 96));
    }

    private static WriterPictureRasterizationOutcome RasterizeNativeBitmap(byte[] sourceBytes)
    {
        using var source = new MemoryStream(sourceBytes, writable: false);
        using var bitmap = new Bitmap(source);
        using var output = new MemoryStream();
        bitmap.Save(output);
        return new WriterPictureRasterizationOutcome(
            output.ToArray(),
            new WriterPictureDecoderFacts(
                bitmap.PixelSize.Width,
                bitmap.PixelSize.Height,
                bitmap.Dpi.X,
                bitmap.Dpi.Y));
    }
}

internal sealed class AvaloniaPictureInsertionPort(DocumentView editor) : IWriterPictureInsertionPort
{
    public WriterPictureInsertionResult Insert(WriterPictureInsertionRequest request)
    {
        editor.InsertInlineImage(
            request.Bytes,
            request.WidthPt,
            request.HeightPt,
            request.Format,
            request.OriginalPixelWidth,
            request.OriginalPixelHeight);
        editor.Focus();
        return WriterPictureInsertionResult.Success;
    }
}
