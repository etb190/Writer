using Avalonia.Platform.Storage;
using Writer.Shared.Shell.Avalonia;
using Writer.App.Avalonia.Editing;
using Writer.App.Presentation.DocumentFragments;
using Writer.Core.Model;

namespace Writer.App.Avalonia;

internal sealed class AvaloniaDocumentFragmentPickerPort(IStorageProvider storageProvider)
    : IWriterDocumentFragmentPickerPort
{
    public async Task<WriterDocumentFragmentPickerResult> PickAsync(
        WriterDocumentFragmentImportRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var file = await AvaloniaFilePickerService.PickSingleOpenFileWithLocalPathAsync(
            storageProvider,
            AvaloniaFilePickerOpenRequest.FromDescriptors(
                request.PickerPlan.Title,
                request.PickerPlan.FileTypes));
        if (file?.LocalPath is not { } localPath)
            return WriterDocumentFragmentPickerResult.Cancelled;

        return WriterDocumentFragmentPickerResult.Selected(
            Path.GetFileName(localPath),
            localPath,
            localPath);
    }
}

internal sealed class AvaloniaDocumentFragmentSourceReaderPort :
    WriterDocumentFragmentFileSourceReaderPort
{
    public override void ResolveLinkedImagePreviews(
        WriterDocumentFragmentImportSelection selection,
        TextDocument document)
    {
    }
}

internal sealed class AvaloniaDocumentFragmentInsertionPort(DocumentView editor)
    : IWriterDocumentFragmentInsertionPort
{
    public WriterDocumentFragmentInsertionResult Insert(WriterDocumentFragmentInsertionRequest request)
    {
        switch (request.Kind)
        {
            case WriterDocumentFragmentInsertionKind.Document when request.Document is not null:
                editor.InsertDocument(request.Document);
                break;
            case WriterDocumentFragmentInsertionKind.PlainText when request.PlainText is not null:
                editor.InsertQuickPartText(request.PlainText);
                break;
            case WriterDocumentFragmentInsertionKind.EmbeddedObject when request.EmbeddedObject is not null:
                editor.InsertEmbeddedObject(request.EmbeddedObject);
                break;
            default:
                return WriterDocumentFragmentInsertionResult.NotApplied();
        }

        editor.Focus();
        return WriterDocumentFragmentInsertionResult.Success;
    }
}
