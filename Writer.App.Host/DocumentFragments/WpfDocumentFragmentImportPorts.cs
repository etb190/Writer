using System.IO;
using System.Windows;
using Writer.Shared.Shell;
using Writer.App.Host.Editing;
using Writer.App.Presentation.DocumentFragments;
using Writer.Core.IO;
using Writer.Core.Model;

namespace Writer.App.Host;

internal sealed class WpfDocumentFragmentPickerPort(Window? owner) : IWriterDocumentFragmentPickerPort
{
    public Task<WriterDocumentFragmentPickerResult> PickAsync(
        WriterDocumentFragmentImportRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = WpfFileDialogService.ShowOpenDialog(
            owner,
            request.PickerPlan.BuildWpfFilter(),
            request.PickerPlan.DefaultExtensionWithDot,
            title: request.PickerPlan.Title);
        if (!result.Chosen || string.IsNullOrWhiteSpace(result.FileName))
            return Task.FromResult(WriterDocumentFragmentPickerResult.Cancelled);

        return Task.FromResult(WriterDocumentFragmentPickerResult.Selected(
            Path.GetFileName(result.FileName),
            result.FileName,
            result.FileName));
    }
}

internal sealed class WpfDocumentFragmentSourceReaderPort :
    WriterDocumentFragmentFileSourceReaderPort
{
    public override void ResolveLinkedImagePreviews(
        WriterDocumentFragmentImportSelection selection,
        TextDocument document) =>
        LinkedImagePreviewResolver.ResolveLocalPreviews(document, selection.LocalPath);
}

internal sealed class WpfDocumentFragmentInsertionPort(DocumentView editor) : IWriterDocumentFragmentInsertionPort
{
    public WriterDocumentFragmentInsertionResult Insert(WriterDocumentFragmentInsertionRequest request)
    {
        editor.Focus();
        switch (request.Kind)
        {
            case WriterDocumentFragmentInsertionKind.Document when request.Document is not null:
                editor.InsertDocument(request.Document);
                break;
            case WriterDocumentFragmentInsertionKind.EmbeddedObject when request.EmbeddedObject is not null:
                editor.InsertEmbeddedObject(request.EmbeddedObject);
                break;
            default:
                return WriterDocumentFragmentInsertionResult.NotApplied();
        }

        return WriterDocumentFragmentInsertionResult.Success;
    }
}
