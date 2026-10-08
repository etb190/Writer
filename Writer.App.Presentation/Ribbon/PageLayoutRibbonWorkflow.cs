using Writer.Shared.Ribbon;
using Writer.App.Presentation.DocumentView;
using Writer.Core.Model;

namespace Writer.App.Presentation.Ribbon;

public sealed record PageLayoutRibbonPorts(
    Func<PageSettings> GetPageSettings,
    Action<Action<PageSettings>> ApplyPageSettings,
    Func<bool> IsEnabled,
    Action? PrepareExecution = null);

public sealed record PageLayoutRibbonCommand(
    RibbonCommandId Id,
    IRibbonStatefulCommand Command);

public sealed record PageLayoutRibbonCommands(
    IReadOnlyList<PageLayoutRibbonCommand> StatefulCommands);

/// <summary>
/// Owns the renderer-neutral Layout ribbon quick actions. Renderers supply only the current page,
/// the undoable page-settings commit adapter, and their editing-lock state.
/// </summary>
public static class PageLayoutRibbonWorkflow
{
    public static IReadOnlyList<WriterRibbonCommandAction> Actions { get; } =
    [
        WriterRibbonCommandAction.Orientation,
        WriterRibbonCommandAction.Margins,
        WriterRibbonCommandAction.Size,
        WriterRibbonCommandAction.ColumnsOne,
        WriterRibbonCommandAction.ColumnsTwo,
        WriterRibbonCommandAction.ColumnsThree,
        WriterRibbonCommandAction.ColumnsLeft,
        WriterRibbonCommandAction.ColumnsRight,
        WriterRibbonCommandAction.LineNumbers,
        WriterRibbonCommandAction.LineNumbersNone,
        WriterRibbonCommandAction.LineNumbersContinuous,
        WriterRibbonCommandAction.LineNumbersRestartPage,
        WriterRibbonCommandAction.LineNumbersRestartSection,
        WriterRibbonCommandAction.Hyphenation,
        WriterRibbonCommandAction.HyphenationNone,
        WriterRibbonCommandAction.HyphenationAuto,
        WriterRibbonCommandAction.PageValign,
        WriterRibbonCommandAction.DifferentFirstPage,
    ];

    public static PageLayoutRibbonCommands Register(
        IRibbonCommandRegistry registry,
        PageLayoutRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(ports);
        ArgumentNullException.ThrowIfNull(ports.GetPageSettings);
        ArgumentNullException.ThrowIfNull(ports.ApplyPageSettings);
        ArgumentNullException.ThrowIfNull(ports.IsEnabled);

        var commands = new List<PageLayoutRibbonCommand>();

        var orientation = Bind(
            WriterRibbonCommandAction.Orientation,
            PageLayoutCommandPlanner.ToggleOrientation);
        registry.Register("writer.page-orientation", orientation);

        Bind(WriterRibbonCommandAction.Margins, PageLayoutCommandPlanner.ToggleNormalNarrowMargins);
        Register("writer.page-margins-normal", page =>
            PageLayoutCommandPlanner.ApplyMarginPreset(page, PageMarginPreset.Normal));
        Register("writer.page-margins-narrow", page =>
            PageLayoutCommandPlanner.ApplyMarginPreset(page, PageMarginPreset.Narrow));
        Register("writer.page-margins-wide", page =>
            PageLayoutCommandPlanner.ApplyMarginPreset(page, PageMarginPreset.Wide));

        Bind(WriterRibbonCommandAction.Size, PageLayoutCommandPlanner.ToggleLetterA4Paper);
        Register("writer.page-size-letter", page =>
            PageLayoutCommandPlanner.ApplyPaperSize(page, PagePaperSizePreset.Letter));
        Register("writer.page-size-a4", page =>
            PageLayoutCommandPlanner.ApplyPaperSize(page, PagePaperSizePreset.A4));

        BindColumnPreset(WriterRibbonCommandAction.ColumnsOne, PageColumnPreset.One);
        BindColumnPreset(WriterRibbonCommandAction.ColumnsTwo, PageColumnPreset.Two);
        BindColumnPreset(WriterRibbonCommandAction.ColumnsThree, PageColumnPreset.Three);
        BindColumnPreset(WriterRibbonCommandAction.ColumnsLeft, PageColumnPreset.Left);
        BindColumnPreset(WriterRibbonCommandAction.ColumnsRight, PageColumnPreset.Right);

        Bind(WriterRibbonCommandAction.LineNumbers, PageLayoutCommandPlanner.CycleLineNumberMode);
        BindLineNumberMode(WriterRibbonCommandAction.LineNumbersNone, LineNumberMode.None);
        BindLineNumberMode(WriterRibbonCommandAction.LineNumbersContinuous, LineNumberMode.Continuous);
        BindLineNumberMode(WriterRibbonCommandAction.LineNumbersRestartPage, LineNumberMode.RestartEachPage);
        BindLineNumberMode(WriterRibbonCommandAction.LineNumbersRestartSection, LineNumberMode.RestartEachSection);

        Bind(
            WriterRibbonCommandAction.Hyphenation,
            PageLayoutCommandPlanner.ToggleHyphenation,
            page => page.AutoHyphenation);
        Bind(
            WriterRibbonCommandAction.HyphenationNone,
            page => page.AutoHyphenation = false,
            page => !page.AutoHyphenation);
        Bind(
            WriterRibbonCommandAction.HyphenationAuto,
            page => page.AutoHyphenation = true,
            page => page.AutoHyphenation);
        Bind(
            WriterRibbonCommandAction.PageValign,
            page => page.VerticalAlignment = PageVerticalAlignmentPlanner.Next(page.VerticalAlignment));
        Bind(
            WriterRibbonCommandAction.DifferentFirstPage,
            page => page.DifferentFirstPage = !page.DifferentFirstPage,
            page => page.DifferentFirstPage);

        return new PageLayoutRibbonCommands(commands);

        PageLayoutCommand Bind(
            WriterRibbonCommandAction action,
            Action<PageSettings> apply,
            Func<PageSettings, bool>? isChecked = null)
        {
            var id = WriterRibbonCommandWorkflow.GetPrimaryCommandId(action);
            var command = new PageLayoutCommand(ports, apply, isChecked);
            registry.Bind(action, command);
            commands.Add(new PageLayoutRibbonCommand(id, command));
            return command;
        }

        PageLayoutCommand Register(
            RibbonCommandId id,
            Action<PageSettings> apply,
            Func<PageSettings, bool>? isChecked = null)
        {
            var command = new PageLayoutCommand(ports, apply, isChecked);
            registry.Register(id, command);
            commands.Add(new PageLayoutRibbonCommand(id, command));
            return command;
        }

        void BindColumnPreset(WriterRibbonCommandAction action, PageColumnPreset preset) =>
            Bind(
                action,
                page => PageLayoutCommandPlanner.ApplyColumnPreset(page, preset),
                page => PageLayoutCommandPlanner.IsColumnPresetChecked(page, preset));

        void BindLineNumberMode(WriterRibbonCommandAction action, LineNumberMode mode) =>
            Bind(
                action,
                page => page.LineNumberMode = mode,
                page => PageLayoutCommandPlanner.IsLineNumberModeChecked(page, mode));
    }

    private sealed class PageLayoutCommand(
        PageLayoutRibbonPorts ports,
        Action<PageSettings> apply,
        Func<PageSettings, bool>? isChecked) : IRibbonStatefulCommand
    {
        public void Execute(RibbonCommandContext context)
        {
            if (!ports.IsEnabled())
                return;

            ports.PrepareExecution?.Invoke();
            ports.ApplyPageSettings(apply);
        }

        public RibbonCommandState GetState()
        {
            var enabled = ports.IsEnabled();
            return new RibbonCommandState(
                IsEnabled: enabled,
                IsChecked: isChecked?.Invoke(ports.GetPageSettings()) == true);
        }
    }
}
