namespace Writer.App.Presentation.Shell;

public sealed record WriterApplicationCommandActions(
    Action NewDocument,
    Action OpenDocument,
    Action SaveDocument,
    Action SaveDocumentAs,
    Action PrintDocument,
    Action Find,
    Action Replace,
    Action Cut,
    Action Copy,
    Action Paste,
    Action PasteTextOnly,
    Action SelectAll,
    Action Undo,
    Action Redo,
    Action RevealFormatting,
    Action Thesaurus,
    Action LockCurrentField,
    Action UnlockCurrentField,
    Action UnlinkCurrentField,
    Action ToggleCurrentFieldCode,
    Action ToggleFieldCodes,
    Action UpdateCurrentField);

/// <summary>
/// Owns the application decision that maps Writer commands to host-provided actions.
/// Key conversion and native editing/clipboard execution remain platform responsibilities.
/// </summary>
public sealed class WriterApplicationCommandRouter
{
    private readonly WriterApplicationCommandActions _actions;

    public WriterApplicationCommandRouter(WriterApplicationCommandActions actions)
    {
        _actions = actions ?? throw new ArgumentNullException(nameof(actions));
    }

    public IReadOnlyList<WriterKeyboardShortcut> Shortcuts =>
        WriterKeyboardShortcutCatalog.All;

    public bool TryExecute(
        WriterKeyboardKey key,
        WriterKeyboardModifiers modifiers) =>
        WriterKeyboardShortcutCatalog.TryDispatch(key, modifiers, Execute);

    public void Execute(WriterKeyboardCommand command)
    {
        Action action = command switch
        {
            WriterKeyboardCommand.NewDocument => _actions.NewDocument,
            WriterKeyboardCommand.OpenDocument => _actions.OpenDocument,
            WriterKeyboardCommand.SaveDocument => _actions.SaveDocument,
            WriterKeyboardCommand.SaveDocumentAs => _actions.SaveDocumentAs,
            WriterKeyboardCommand.PrintDocument => _actions.PrintDocument,
            WriterKeyboardCommand.Find => _actions.Find,
            WriterKeyboardCommand.Replace => _actions.Replace,
            WriterKeyboardCommand.Cut => _actions.Cut,
            WriterKeyboardCommand.Copy => _actions.Copy,
            WriterKeyboardCommand.Paste => _actions.Paste,
            WriterKeyboardCommand.PasteTextOnly => _actions.PasteTextOnly,
            WriterKeyboardCommand.SelectAll => _actions.SelectAll,
            WriterKeyboardCommand.Undo => _actions.Undo,
            WriterKeyboardCommand.Redo => _actions.Redo,
            WriterKeyboardCommand.RevealFormatting => _actions.RevealFormatting,
            WriterKeyboardCommand.Thesaurus => _actions.Thesaurus,
            WriterKeyboardCommand.LockCurrentField => _actions.LockCurrentField,
            WriterKeyboardCommand.UnlockCurrentField => _actions.UnlockCurrentField,
            WriterKeyboardCommand.UnlinkCurrentField => _actions.UnlinkCurrentField,
            WriterKeyboardCommand.ToggleCurrentFieldCode => _actions.ToggleCurrentFieldCode,
            WriterKeyboardCommand.ToggleFieldCodes => _actions.ToggleFieldCodes,
            WriterKeyboardCommand.UpdateCurrentField => _actions.UpdateCurrentField,
            _ => throw new ArgumentOutOfRangeException(nameof(command), command, null),
        };

        action();
    }
}
