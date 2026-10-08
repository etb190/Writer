using Writer.Shared.Ribbon;

namespace Writer.App.Presentation.Ribbon;

/// <summary>Renderer adapters consumed by the shared Footnotes command family.</summary>
public sealed record NoteReferenceRibbonPorts(
    Action? InsertFootnote,
    Action? InsertEndnote,
    Action MoveToNextFootnote,
    Action MoveToPreviousFootnote,
    Action MoveToNextEndnote,
    Action MoveToPreviousEndnote,
    Action? OpenNotes,
    Action? ToggleNotesPane,
    Func<bool>? IsNotesPaneVisible,
    Action? OpenFootnoteEndnoteOptions);

/// <summary>
/// Owns the complete References &gt; Footnotes command policy for both renderers. Native hosts
/// provide only editor operations and toolkit-specific dialogs/panes.
/// </summary>
public static class NoteReferenceRibbonWorkflow
{
    public static IRibbonStatefulCommand? Register(
        WriterRibbonEditorCommandFamilyBuilder bindings,
        NoteReferenceRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        return RegisterCore(
            ports,
            (action, execute) => bindings.BindAction(action, execute),
            (action, toggle, isChecked) => bindings.BindToggle(action, toggle, isChecked),
            bindings.Bind,
            bindings.Register);
    }

    public static IRibbonStatefulCommand? Register(
        IRibbonCommandRegistry bindings,
        NoteReferenceRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        return RegisterCore(
            ports,
            (action, execute) => bindings.BindAction(action, execute),
            (action, toggle, isChecked) => bindings.BindToggle(action, toggle, isChecked),
            (action, command) => bindings.Bind(action, command),
            bindings.Register);
    }

    private static IRibbonStatefulCommand? RegisterCore(
        NoteReferenceRibbonPorts ports,
        Func<WriterRibbonCommandAction, Action, IRibbonCommand> bindAction,
        Func<WriterRibbonCommandAction, Action, Func<bool>, IRibbonStatefulCommand> bindToggle,
        Func<WriterRibbonCommandAction, IRibbonCommand, IRibbonCommand> bind,
        Action<RibbonCommandId, IRibbonCommand> register)
    {
        ArgumentNullException.ThrowIfNull(ports);
        ArgumentNullException.ThrowIfNull(ports.MoveToNextFootnote);
        ArgumentNullException.ThrowIfNull(ports.MoveToPreviousFootnote);
        ArgumentNullException.ThrowIfNull(ports.MoveToNextEndnote);
        ArgumentNullException.ThrowIfNull(ports.MoveToPreviousEndnote);

        var footnote = bind(
            WriterRibbonCommandAction.Footnote,
            OptionalCommand(ports.InsertFootnote));
        register("writer.insert-footnote", footnote);

        var endnote = bind(
            WriterRibbonCommandAction.Endnote,
            OptionalCommand(ports.InsertEndnote));
        register("writer.insert-endnote", endnote);

        bindAction(
            WriterRibbonCommandAction.NextFootnote,
            ports.MoveToNextFootnote);
        bindAction(
            WriterRibbonCommandAction.PreviousFootnote,
            ports.MoveToPreviousFootnote);
        bindAction(
            WriterRibbonCommandAction.NextEndnote,
            ports.MoveToNextEndnote);
        bindAction(
            WriterRibbonCommandAction.PreviousEndnote,
            ports.MoveToPreviousEndnote);

        IRibbonStatefulCommand? notesPane = null;
        if (ports.ToggleNotesPane is not null && ports.IsNotesPaneVisible is not null)
        {
            notesPane = bindToggle(
                WriterRibbonCommandAction.ShowNotes,
                ports.ToggleNotesPane,
                ports.IsNotesPaneVisible);
        }
        else if (ports.OpenNotes is not null)
        {
            bindAction(
                WriterRibbonCommandAction.ShowNotes,
                ports.OpenNotes);
        }
        else
        {
            bind(
                WriterRibbonCommandAction.ShowNotes,
                WriterRibbonExecutionProfile.UnavailableCommand);
        }

        bind(
            WriterRibbonCommandAction.FootnoteEndnoteOptions,
            ports.OpenFootnoteEndnoteOptions is null
                ? WriterRibbonExecutionProfile.UnavailableCommand
                : new ActionRibbonCommand(ports.OpenFootnoteEndnoteOptions));

        return notesPane;
    }

    private static IRibbonCommand OptionalCommand(Action? execute) =>
        execute is null
            ? WriterRibbonExecutionProfile.UnavailableCommand
            : new ActionRibbonCommand(execute);
}
