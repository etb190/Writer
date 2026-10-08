using Writer.Shared.Ribbon;
using Writer.Core.Model;

namespace Writer.App.Presentation.Ribbon;

public sealed record ParagraphEditingRibbonPorts(
    Action PrepareExecution,
    Func<ListKind> CurrentListKind,
    Action ToggleBullets,
    Action ToggleNumbering,
    IRibbonCommand AlignLeft,
    IRibbonCommand AlignCenter,
    IRibbonCommand AlignRight,
    IRibbonCommand AlignJustify,
    Action IncreaseIndent,
    Action DecreaseIndent,
    Action ToggleSpaceBefore,
    Action ToggleSpaceAfter,
    Action ToggleKeepWithNext,
    Action ToggleKeepLinesTogether,
    Action ToggleWidowControl,
    Action ToggleParagraphBorder,
    IRibbonCommand Sort);

public sealed record ParagraphEditingRibbonStatefulCommand(
    RibbonCommandId Id,
    IRibbonStatefulCommand Command);

public sealed record ParagraphEditingRibbonCommands(
    IReadOnlyList<ParagraphEditingRibbonStatefulCommand> StatefulCommands);

/// <summary>
/// Owns Home/Layout paragraph command identity and execution preparation. Renderers retain only
/// native routed-command, dialog, and editor adapters; the semantic mapping is shared.
/// </summary>
public static class ParagraphEditingRibbonWorkflow
{
    public static IReadOnlyList<WriterRibbonCommandAction> Actions { get; } =
    [
        WriterRibbonCommandAction.Bullets,
        WriterRibbonCommandAction.Numbering,
        WriterRibbonCommandAction.AlignLeft,
        WriterRibbonCommandAction.AlignCenter,
        WriterRibbonCommandAction.AlignRight,
        WriterRibbonCommandAction.AlignJustify,
        WriterRibbonCommandAction.IndentIncrease,
        WriterRibbonCommandAction.IndentDecrease,
        WriterRibbonCommandAction.SpaceBeforeToggle,
        WriterRibbonCommandAction.SpaceAfterToggle,
        WriterRibbonCommandAction.KeepWithNext,
        WriterRibbonCommandAction.KeepLines,
        WriterRibbonCommandAction.WidowControl,
        WriterRibbonCommandAction.ParaBorder,
        WriterRibbonCommandAction.Sort,
    ];

    public static ParagraphEditingRibbonCommands Register(
        IRibbonCommandRegistry bindings,
        ParagraphEditingRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(ports);
        ArgumentNullException.ThrowIfNull(ports.PrepareExecution);

        var bullets = BindListToggle(
            bindings,
            ports,
            WriterRibbonCommandAction.Bullets,
            ListKind.Bullet,
            ports.ToggleBullets);
        var numbering = BindListToggle(
            bindings,
            ports,
            WriterRibbonCommandAction.Numbering,
            ListKind.Number,
            ports.ToggleNumbering);
        BindCommand(bindings, ports, WriterRibbonCommandAction.AlignLeft, ports.AlignLeft);
        BindCommand(bindings, ports, WriterRibbonCommandAction.AlignCenter, ports.AlignCenter);
        BindCommand(bindings, ports, WriterRibbonCommandAction.AlignRight, ports.AlignRight);
        BindCommand(bindings, ports, WriterRibbonCommandAction.AlignJustify, ports.AlignJustify);

        var increaseIndent = BindAction(
            bindings,
            ports,
            WriterRibbonCommandAction.IndentIncrease,
            ports.IncreaseIndent);
        bindings.Register("writer.increase-indent", increaseIndent);

        var decreaseIndent = BindAction(
            bindings,
            ports,
            WriterRibbonCommandAction.IndentDecrease,
            ports.DecreaseIndent);
        bindings.Register("writer.decrease-indent", decreaseIndent);

        BindAction(bindings, ports, WriterRibbonCommandAction.SpaceBeforeToggle, ports.ToggleSpaceBefore);
        BindAction(bindings, ports, WriterRibbonCommandAction.SpaceAfterToggle, ports.ToggleSpaceAfter);
        BindAction(bindings, ports, WriterRibbonCommandAction.KeepWithNext, ports.ToggleKeepWithNext);
        BindAction(bindings, ports, WriterRibbonCommandAction.KeepLines, ports.ToggleKeepLinesTogether);
        BindAction(bindings, ports, WriterRibbonCommandAction.WidowControl, ports.ToggleWidowControl);
        BindAction(bindings, ports, WriterRibbonCommandAction.ParaBorder, ports.ToggleParagraphBorder);
        BindCommand(bindings, ports, WriterRibbonCommandAction.Sort, ports.Sort);

        return new ParagraphEditingRibbonCommands(
        [
            new("writer.bullets", bullets),
            new("writer.numbering", numbering),
        ]);
    }

    private static IRibbonCommand BindAction(
        IRibbonCommandRegistry bindings,
        ParagraphEditingRibbonPorts ports,
        WriterRibbonCommandAction action,
        Action execute) =>
        bindings.BindAction(action, execute, prepareExecution: ports.PrepareExecution);

    private static void BindCommand(
        IRibbonCommandRegistry bindings,
        ParagraphEditingRibbonPorts ports,
        WriterRibbonCommandAction action,
        IRibbonCommand command) =>
        bindings.Bind(action, new PreparedCommand(ports.PrepareExecution, command));

    private static IRibbonStatefulCommand BindListToggle(
        IRibbonCommandRegistry bindings,
        ParagraphEditingRibbonPorts ports,
        WriterRibbonCommandAction action,
        ListKind kind,
        Action execute)
    {
        var command = new PreparedCommand(
            ports.PrepareExecution,
            new ParagraphListToggleCommand(execute, ports.CurrentListKind, kind));
        bindings.Bind(action, command);
        return command;
    }

    private sealed class ParagraphListToggleCommand(
        Action execute,
        Func<ListKind> currentListKind,
        ListKind kind) : IRibbonStatefulCommand
    {
        public void Execute(RibbonCommandContext context) => execute();

        public RibbonCommandState GetState() =>
            new(IsChecked: currentListKind() == kind);
    }

    private sealed class PreparedCommand(Action prepareExecution, IRibbonCommand inner) : IRibbonStatefulCommand
    {
        public void Execute(RibbonCommandContext context)
        {
            prepareExecution();
            inner.Execute(context);
        }

        public RibbonCommandState GetState() =>
            inner is IRibbonStatefulCommand stateful
                ? stateful.GetState()
                : RibbonCommandState.Default;
    }
}
