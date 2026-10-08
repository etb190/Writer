using Writer.Shared.Shell;

namespace Writer.App.Presentation.Shell;

[Flags]
public enum WriterKeyboardModifiers
{
    None = 0,
    Control = 1,
    Shift = 2,
    Alt = 4,
}

public enum WriterKeyboardKey
{
    A,
    C,
    F,
    H,
    N,
    O,
    P,
    S,
    V,
    X,
    Y,
    Z,
    F1,
    F7,
    F9,
    F11,
}

public enum WriterKeyboardCommand
{
    NewDocument,
    OpenDocument,
    SaveDocument,
    SaveDocumentAs,
    PrintDocument,
    Find,
    Replace,
    Cut,
    Copy,
    Paste,
    PasteTextOnly,
    SelectAll,
    Undo,
    Redo,
    RevealFormatting,
    Thesaurus,
    LockCurrentField,
    UnlockCurrentField,
    UnlinkCurrentField,
    ToggleCurrentFieldCode,
    ToggleFieldCodes,
    UpdateCurrentField,
}

public readonly record struct WriterKeyboardShortcut(
    WriterKeyboardCommand Command,
    WriterKeyboardKey Key,
    WriterKeyboardModifiers Modifiers);

/// <summary>
/// Host-neutral keyboard contract shared by the WPF and Avalonia Writer shells.
/// Platform-specific key types and command implementations stay in their host adapters.
/// </summary>
public static class WriterKeyboardShortcutCatalog
{
    private static readonly WriterKeyboardShortcut[] Shortcuts =
    [
        new(WriterKeyboardCommand.NewDocument, WriterKeyboardKey.N, WriterKeyboardModifiers.Control),
        new(WriterKeyboardCommand.OpenDocument, WriterKeyboardKey.O, WriterKeyboardModifiers.Control),
        new(WriterKeyboardCommand.SaveDocument, WriterKeyboardKey.S, WriterKeyboardModifiers.Control),
        new(WriterKeyboardCommand.SaveDocumentAs, WriterKeyboardKey.S, WriterKeyboardModifiers.Control | WriterKeyboardModifiers.Shift),
        new(WriterKeyboardCommand.PrintDocument, WriterKeyboardKey.P, WriterKeyboardModifiers.Control),
        new(WriterKeyboardCommand.Find, WriterKeyboardKey.F, WriterKeyboardModifiers.Control),
        new(WriterKeyboardCommand.Replace, WriterKeyboardKey.H, WriterKeyboardModifiers.Control),
        new(WriterKeyboardCommand.Cut, WriterKeyboardKey.X, WriterKeyboardModifiers.Control),
        new(WriterKeyboardCommand.Copy, WriterKeyboardKey.C, WriterKeyboardModifiers.Control),
        new(WriterKeyboardCommand.Paste, WriterKeyboardKey.V, WriterKeyboardModifiers.Control),
        new(WriterKeyboardCommand.PasteTextOnly, WriterKeyboardKey.V, WriterKeyboardModifiers.Control | WriterKeyboardModifiers.Shift),
        new(WriterKeyboardCommand.SelectAll, WriterKeyboardKey.A, WriterKeyboardModifiers.Control),
        new(WriterKeyboardCommand.Undo, WriterKeyboardKey.Z, WriterKeyboardModifiers.Control),
        new(WriterKeyboardCommand.Redo, WriterKeyboardKey.Y, WriterKeyboardModifiers.Control),
        new(WriterKeyboardCommand.RevealFormatting, WriterKeyboardKey.F1, WriterKeyboardModifiers.Shift),
        new(WriterKeyboardCommand.Thesaurus, WriterKeyboardKey.F7, WriterKeyboardModifiers.Shift),
        new(WriterKeyboardCommand.LockCurrentField, WriterKeyboardKey.F11, WriterKeyboardModifiers.Control),
        new(WriterKeyboardCommand.UnlockCurrentField, WriterKeyboardKey.F11,
            WriterKeyboardModifiers.Control | WriterKeyboardModifiers.Shift),
        new(WriterKeyboardCommand.UnlinkCurrentField, WriterKeyboardKey.F9,
            WriterKeyboardModifiers.Control | WriterKeyboardModifiers.Shift),
        new(WriterKeyboardCommand.ToggleCurrentFieldCode, WriterKeyboardKey.F9, WriterKeyboardModifiers.Shift),
        new(WriterKeyboardCommand.ToggleFieldCodes, WriterKeyboardKey.F9, WriterKeyboardModifiers.Alt),
        new(WriterKeyboardCommand.UpdateCurrentField, WriterKeyboardKey.F9, WriterKeyboardModifiers.None),
    ];

    private static readonly ApplicationKeyboardShortcutCatalog<
        WriterKeyboardCommand,
        WriterKeyboardKey,
        WriterKeyboardModifiers> Resolver = new(
            Shortcuts.Select(shortcut => new ApplicationKeyboardShortcut<
                WriterKeyboardCommand,
                WriterKeyboardKey,
                WriterKeyboardModifiers>(
                    shortcut.Command,
                    shortcut.Key,
                    shortcut.Modifiers)));

    public static IReadOnlyList<WriterKeyboardShortcut> All => Shortcuts;

    public static WriterKeyboardCommand? Resolve(
        WriterKeyboardKey key,
        WriterKeyboardModifiers modifiers) =>
        Resolver.Resolve(key, modifiers);

    public static bool TryDispatch(
        WriterKeyboardKey key,
        WriterKeyboardModifiers modifiers,
        Action<WriterKeyboardCommand> dispatch) =>
        Resolver.TryDispatch(key, modifiers, dispatch);
}
