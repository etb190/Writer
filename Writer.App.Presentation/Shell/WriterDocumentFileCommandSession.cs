using Writer.Shared.AppServices;
using Writer.Core.Model;

namespace Writer.App.Presentation.Shell;

public sealed record WriterFileCommandLifecyclePorts(
    Func<string?> CurrentPath,
    Func<string?> CurrentFileName,
    Func<string, Func<Task>, Task<bool>> NewAsync,
    Func<string, Func<Task<string?>>, Func<string, Task<bool>>, Task<bool>> OpenAsync,
    Func<Func<string, Task<bool>>, Func<Task<bool>>, Task<bool>> SaveAsync);

public sealed record WriterDocumentOpenPickerRequest(string? InitialDirectory = null);

public sealed record WriterDocumentSavePickerRequest(
    string Title,
    string? CurrentPath,
    string? CurrentFileName,
    string? SuggestedFileName = null,
    string? PreferredExtension = null);

public sealed record WriterDocumentSavePickerResult(string Path, int FilterIndex = 0);

public sealed record WriterDocumentFileCommandPorts(
    Func<Task> LoadNewDocumentAsync,
    Func<WriterDocumentOpenPickerRequest, Task<string?>> PickOpenPathAsync,
    Func<Task<string?>> PickPdfImportPathAsync,
    Func<WriterDocumentSavePickerRequest, Task<WriterDocumentSavePickerResult?>> PickSaveTargetAsync,
    Action<WriterDocumentFileFeedback> PresentFeedback);

/// <summary>
/// Owns Writer's renderer-neutral file-command sequencing. Renderers retain their native dirty gate,
/// pickers, messages, and editor projection behind the supplied ports.
/// </summary>
public sealed class WriterDocumentFileCommandSession
{
    private readonly WriterDocumentFileWorkflow _workflow;
    private readonly WriterFileCommandLifecyclePorts _lifecycle;
    private readonly WriterDocumentFileCommandPorts _ports;
    private readonly SisterAppFileTextSpec _text;

    public WriterDocumentFileCommandSession(
        WriterDocumentFileWorkflow workflow,
        WriterFileCommandLifecyclePorts lifecycle,
        WriterDocumentFileCommandPorts ports,
        SisterAppFileTextSpec text)
    {
        _workflow = workflow ?? throw new ArgumentNullException(nameof(workflow));
        _lifecycle = lifecycle ?? throw new ArgumentNullException(nameof(lifecycle));
        _ports = ports ?? throw new ArgumentNullException(nameof(ports));
        _text = text ?? throw new ArgumentNullException(nameof(text));
    }

    public Task<bool> NewAsync() =>
        _lifecycle.NewAsync(_text.NewAction, _ports.LoadNewDocumentAsync);

    public Task<bool> OpenAsync(string? initialDirectory = null) =>
        _lifecycle.OpenAsync(
            _text.OpenAction,
            () => _ports.PickOpenPathAsync(new WriterDocumentOpenPickerRequest(initialDirectory)),
            path => OpenPathAsync(path));

    public Task<bool> OpenSelectedPathAsync(string path) =>
        _lifecycle.OpenAsync(
            _text.OpenAction,
            () => Task.FromResult<string?>(path),
            selectedPath => OpenPathAsync(selectedPath));

    public async Task<bool> OpenPathAsync(string path, bool suppressRecentFiles = false)
    {
        var execution = await _workflow.OpenPathAsync(path, suppressRecentFiles);
        return Present(WriterDocumentFileFeedbackPlanner.PlanOpen(execution, path));
    }

    public Task<bool> ImportPdfTextAsync() =>
        _lifecycle.OpenAsync(
            WriterDocumentFileFeedbackPlanner.ImportPdfAction,
            _ports.PickPdfImportPathAsync,
            ImportPdfTextPathAsync);

    public async Task<bool> ImportPdfTextPathAsync(string path)
    {
        var execution = await _workflow.ImportPdfTextPathAsync(path);
        return Present(WriterDocumentFileFeedbackPlanner.PlanImport(execution, path));
    }

    public Task<bool> SaveAsync() =>
        _lifecycle.SaveAsync(SaveToCurrentPathAsync, SaveAsAsync);

    public Task<bool> SaveAsAsync() => SaveAsAsync(null, null);

    public Task<bool> SaveAsFormatAsync(string preferredExtension) =>
        SaveAsAsync(suggestedFileName: null, preferredExtension);

    public async Task<bool> SaveAsAsync(
        string? suggestedFileName,
        string? preferredExtension)
    {
        var selection = await _ports.PickSaveTargetAsync(new WriterDocumentSavePickerRequest(
            _text.SavePickerTitle,
            _lifecycle.CurrentPath(),
            _lifecycle.CurrentFileName(),
            suggestedFileName,
            preferredExtension));
        return selection is not null
            && await SavePathAsync(
                selection.Path,
                selection.FilterIndex,
                DocumentSaveExecutionKind.Save);
    }

    public async Task<bool> SaveCopyAsync()
    {
        var selection = await _ports.PickSaveTargetAsync(new WriterDocumentSavePickerRequest(
            WriterDocumentFileFeedbackPlanner.SaveCopyCommand,
            _lifecycle.CurrentPath(),
            _lifecycle.CurrentFileName()));
        return selection is not null
            && await SavePathAsync(
                selection.Path,
                selection.FilterIndex,
                DocumentSaveExecutionKind.SaveCopy);
    }

    public async Task<bool> SaveToCurrentPathAsync(string path)
    {
        var execution = await _workflow.SaveCurrentPathAsync(path);
        var feedback = WriterDocumentFileFeedbackPlanner.PlanSave(
            execution,
            DocumentSaveExecutionKind.Save,
            path);
        return feedback.RequiresSaveAs
            ? await SaveAsAsync()
            : Present(feedback);
    }

    public async Task<bool> SavePathAsync(
        string path,
        int filterIndex = 0,
        DocumentSaveExecutionKind kind = DocumentSaveExecutionKind.Save)
    {
        var execution = await _workflow.SavePathAsync(path, filterIndex, kind);
        return Present(WriterDocumentFileFeedbackPlanner.PlanSave(execution, kind, path));
    }

    private bool Present(WriterDocumentFileFeedback feedback)
    {
        _ports.PresentFeedback(feedback);
        return feedback.Succeeded;
    }
}
