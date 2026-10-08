using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Writer.Shared.Shell;
using Writer.App.Host.Editing;
using Writer.App.Presentation.DocumentFragments;
using Writer.Core.Model;

namespace Writer.App.Host;

internal sealed class WpfPictureImportPickerPort(Window? owner) : IWriterPictureImportPickerPort
{
    public Task<WriterPictureImportPickerResult> PickAsync(
        WriterPictureImportRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = WpfFileDialogService.ShowOpenDialog(
            owner,
            request.PickerPlan.BuildWpfFilter(),
            title: request.PickerPlan.Title);
        return Task.FromResult(
            result.Chosen && !string.IsNullOrWhiteSpace(result.FileName)
                ? WriterPictureImportPickerResult.Selected(
                    Path.GetFileName(result.FileName),
                    result.FileName)
                : WriterPictureImportPickerResult.Cancelled);
    }
}

internal sealed class WpfPictureImportSourceReaderPort : IWriterPictureImportSourceReaderPort
{
    public Task<byte[]> ReadAsync(
        WriterPictureImportSelection selection,
        CancellationToken cancellationToken) =>
        FileByteReadWorkflow.ReadLocalPathBytesAsync(
            (string)selection.Source,
            cancellationToken);
}

internal sealed class WpfPictureDecoderPort : IWriterPictureDecoderPort
{
    public ValueTask<WriterPictureDecoderFacts> DecodeAsync(
        WriterPictureImportSelection selection,
        byte[] bytes,
        CancellationToken cancellationToken) =>
        WriterPictureDecoderPolicy.DecodeOrUnavailable(cancellationToken, () =>
        {
            using var source = new MemoryStream(bytes, writable: false);

            // Round 172: WIC cannot decode a metafile (BitmapFrame.Create throws NotSupportedException
            // for WMF/EMF), which DecodeOrUnavailable would turn into "no natural size" -- every
            // inserted metafile would land at the 200x150pt fallback. GDI+ reads their real extent, the
            // same route DocumentView.TryDecodeMetafile uses to render them. Any failure here still
            // falls through to DecodeOrUnavailable's fallback.
            if (InlineImage.FormatForExtension(Path.GetExtension(selection.Name))
                is ImageFormat.Wmf or ImageFormat.Emf)
            {
                using var metafile = new System.Drawing.Imaging.Metafile(source);
                return new WriterPictureDecoderFacts(
                    metafile.Width,
                    metafile.Height,
                    metafile.HorizontalResolution,
                    metafile.VerticalResolution);
            }

            var frame = BitmapFrame.Create(
                source,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);
            return new WriterPictureDecoderFacts(
                frame.PixelWidth,
                frame.PixelHeight,
                frame.DpiX,
                frame.DpiY);
        });
}

internal sealed class WpfPictureRasterizerPort : IWriterPictureRasterizerPort
{
    public ValueTask<WriterPictureRasterizationOutcome> RasterizeAsync(
        WriterPictureRasterizationRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (request.SourceKind == WriterPictureImportSourceKind.Svg)
        {
            using var source = new MemoryStream(request.SourceBytes, writable: false);
            var image = SvgRasterizerHelper.RasterizeToInlineImage(
                source,
                request.MaximumPixelEdge);
            return ValueTask.FromResult(new WriterPictureRasterizationOutcome(
                image.Bytes,
                new WriterPictureDecoderFacts(
                    image.OriginalPixelWidth,
                    image.OriginalPixelHeight,
                    96,
                    96)));
        }

        using var input = new MemoryStream(request.SourceBytes, writable: false);
        var frame = BitmapFrame.Create(
            input,
            BitmapCreateOptions.PreservePixelFormat,
            BitmapCacheOption.OnLoad);
        using var output = new MemoryStream();
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(frame);
        encoder.Save(output);
        return ValueTask.FromResult(new WriterPictureRasterizationOutcome(
            output.ToArray(),
            new WriterPictureDecoderFacts(
                frame.PixelWidth,
                frame.PixelHeight,
                frame.DpiX,
                frame.DpiY)));
    }
}

internal sealed class WpfPictureInsertionPort(DocumentView editor) : IWriterPictureInsertionPort
{
    public WriterPictureInsertionResult Insert(WriterPictureInsertionRequest request)
    {
        editor.Focus();
        editor.InsertImage(new InlineImage(
            request.Bytes,
            request.WidthPt,
            request.HeightPt,
            request.Format)
        {
            OriginalPixelWidth = request.OriginalPixelWidth,
            OriginalPixelHeight = request.OriginalPixelHeight,
        });
        return WriterPictureInsertionResult.Success;
    }
}
