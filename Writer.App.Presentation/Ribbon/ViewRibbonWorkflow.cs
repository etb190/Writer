using Writer.Shared.Ribbon;
using Writer.App.Presentation.Shell;

namespace Writer.App.Presentation.Ribbon;

public enum ViewRibbonBindingAvailability
{
    Omitted,
    Disabled,
}

public sealed record ViewRibbonActionBinding(
    Action? Execute = null,
    ViewRibbonBindingAvailability AvailabilityWhenUnbound = ViewRibbonBindingAvailability.Omitted);

public sealed record ViewRibbonToggleBinding(
    Action? Toggle = null,
    Func<bool>? IsChecked = null,
    ViewRibbonBindingAvailability AvailabilityWhenUnbound = ViewRibbonBindingAvailability.Omitted,
    Action? PrepareExecution = null);

public sealed record ViewRibbonChoiceBinding(
    Action<string>? Apply = null,
    ViewRibbonBindingAvailability AvailabilityWhenUnbound = ViewRibbonBindingAvailability.Omitted);

public sealed record ViewRibbonReadModeBindings(
    ViewRibbonToggleBinding? Toggle = null,
    ViewRibbonChoiceBinding? ColumnWidth = null,
    ViewRibbonChoiceBinding? PageColor = null);

public sealed record ViewRibbonModeBindings(
    ViewRibbonToggleBinding? Focus = null,
    ViewRibbonToggleBinding? PrintLayout = null,
    ViewRibbonToggleBinding? WebLayout = null,
    ViewRibbonToggleBinding? Draft = null,
    ViewRibbonToggleBinding? Outline = null,
    ViewRibbonToggleBinding? PagedEdit = null);

public sealed record ViewRibbonShowBindings(
    ViewRibbonToggleBinding? NavigationPane = null,
    ViewRibbonToggleBinding? RevealFormatting = null,
    ViewRibbonToggleBinding? Gridlines = null,
    ViewRibbonToggleBinding? Ruler = null);

public sealed record ViewRibbonZoomBindings(
    ViewRibbonActionBinding? Dialog = null,
    ViewRibbonActionBinding? ZoomIn = null,
    ViewRibbonActionBinding? ZoomOut = null,
    ViewRibbonActionBinding? Reset100 = null,
    ViewRibbonActionBinding? OnePage = null,
    ViewRibbonActionBinding? PageWidth = null,
    ViewRibbonToggleBinding? MultiplePages = null,
    ViewRibbonToggleBinding? SideToSide = null);

public sealed record ViewRibbonWindowBindings(
    ViewRibbonActionBinding? NewWindow = null,
    ViewRibbonActionBinding? ArrangeAll = null,
    ViewRibbonActionBinding? SwitchWindows = null,
    ViewRibbonToggleBinding? Split = null);

public sealed record ViewRibbonCommandBindings(
    ViewRibbonActionBinding? PrintPreview = null,
    ViewRibbonReadModeBindings? ReadMode = null,
    ViewRibbonModeBindings? Modes = null,
    ViewRibbonShowBindings? Show = null,
    ViewRibbonZoomBindings? Zoom = null,
    ViewRibbonWindowBindings? Window = null,
    bool RegisterCompatibilityAliases = false);

public sealed record ViewRibbonCommands(IRibbonStatefulCommand? Gridlines);

/// <summary>
/// Registers Writer's renderer-neutral View ribbon commands over host-supplied UI operations.
/// Native surfaces, focus, viewport measurement, and window lifecycle remain in renderer bindings.
/// </summary>
public static class ViewRibbonWorkflow
{
    public static ViewRibbonCommands Register(
        IRibbonCommandRegistry registry,
        ViewRibbonCommandBindings bindings)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(bindings);

        RegisterAction(registry, "writer.print-preview", bindings.PrintPreview);

        var readMode = bindings.ReadMode;
        RegisterToggle(registry, "writer.read-mode", readMode?.Toggle);
        RegisterChoice(
            registry,
            "writer.read-mode-column-narrow",
            WriterReadModePlanner.NarrowColumn,
            readMode?.ColumnWidth);
        RegisterChoice(
            registry,
            "writer.read-mode-column-default",
            WriterReadModePlanner.DefaultColumn,
            readMode?.ColumnWidth);
        RegisterChoice(
            registry,
            "writer.read-mode-column-wide",
            WriterReadModePlanner.WideColumn,
            readMode?.ColumnWidth);
        RegisterChoice(
            registry,
            "writer.read-mode-color-none",
            WriterReadModePlanner.NoColor,
            readMode?.PageColor);
        RegisterChoice(
            registry,
            "writer.read-mode-color-sepia",
            WriterReadModePlanner.SepiaColor,
            readMode?.PageColor);
        RegisterChoice(
            registry,
            "writer.read-mode-color-inverse",
            WriterReadModePlanner.InverseColor,
            readMode?.PageColor);

        var modes = bindings.Modes;
        RegisterToggle(registry, "writer.focus", modes?.Focus);
        var printLayout = RegisterToggle(registry, "writer.print-layout", modes?.PrintLayout);
        var webLayout = RegisterToggle(registry, "writer.web-layout", modes?.WebLayout);
        var draft = RegisterToggle(registry, "writer.draft-view", modes?.Draft);
        RegisterToggle(registry, "writer.outline-view", modes?.Outline);
        RegisterToggle(registry, "writer.paged-edit-view", modes?.PagedEdit);

        var show = bindings.Show;
        var navigationPane = RegisterToggle(registry, "writer.nav-pane", show?.NavigationPane);
        RegisterToggle(registry, "writer.reveal-formatting", show?.RevealFormatting);
        var gridlines = RegisterToggle(registry, "writer.gridlines", show?.Gridlines);
        var ruler = RegisterToggle(registry, "writer.ruler", show?.Ruler);

        var zoom = bindings.Zoom;
        RegisterAction(registry, "writer.zoom-dialog", zoom?.Dialog);
        RegisterAction(registry, "writer.zoom-in", zoom?.ZoomIn);
        RegisterAction(registry, "writer.zoom-out", zoom?.ZoomOut);
        RegisterAction(registry, "writer.zoom-100", zoom?.Reset100);
        RegisterAction(registry, "writer.zoom-one-page", zoom?.OnePage);
        RegisterAction(registry, "writer.zoom-page-width", zoom?.PageWidth);
        RegisterToggle(registry, "writer.zoom-multiple-pages", zoom?.MultiplePages);
        RegisterToggle(registry, "writer.zoom-side-to-side", zoom?.SideToSide);

        var window = bindings.Window;
        RegisterAction(registry, "writer.new-window", window?.NewWindow);
        RegisterAction(registry, "writer.arrange-all", window?.ArrangeAll);
        RegisterAction(registry, "writer.switch-windows", window?.SwitchWindows);
        var split = RegisterToggle(registry, "writer.split-window", window?.Split);

        if (bindings.RegisterCompatibilityAliases)
        {
            RegisterAlias(registry, "writer.printlayout", printLayout);
            RegisterAlias(registry, "writer.weblayout", webLayout);
            RegisterAlias(registry, "writer.draftview", draft);
            RegisterAlias(registry, "writer.navigationpane", navigationPane);
            RegisterAlias(registry, "writer.view-gridlines", gridlines);
            RegisterAlias(registry, "writer.view-ruler", ruler);
            RegisterAlias(registry, "writer.split", split);
        }

        return new ViewRibbonCommands(gridlines);
    }

    private static IRibbonCommand? RegisterAction(
        IRibbonCommandRegistry registry,
        string commandId,
        ViewRibbonActionBinding? binding)
    {
        var command = binding?.Execute is { } execute
            ? new ActionRibbonCommand(execute)
            : CommandFor(binding?.AvailabilityWhenUnbound);
        if (command is not null)
            registry.Register(commandId, command);
        return command;
    }

    private static IRibbonStatefulCommand? RegisterToggle(
        IRibbonCommandRegistry registry,
        string commandId,
        ViewRibbonToggleBinding? binding)
    {
        IRibbonCommand? command = binding?.Toggle is { } toggle && binding.IsChecked is { } isChecked
            ? new WriterStatefulToggleCommand(toggle, isChecked, binding.PrepareExecution)
            : CommandFor(binding?.AvailabilityWhenUnbound);
        if (command is not null)
            registry.Register(commandId, command);
        return command as IRibbonStatefulCommand;
    }

    private static void RegisterChoice(
        IRibbonCommandRegistry registry,
        string commandId,
        string token,
        ViewRibbonChoiceBinding? binding)
    {
        var command = binding?.Apply is { } apply
            ? new ActionRibbonCommand(() => apply(token))
            : CommandFor(binding?.AvailabilityWhenUnbound);
        if (command is not null)
            registry.Register(commandId, command);
    }

    private static void RegisterAlias(
        IRibbonCommandRegistry registry,
        string alias,
        IRibbonCommand? command)
    {
        if (command is not null)
            registry.Register(alias, command);
    }

    private static IRibbonCommand? CommandFor(ViewRibbonBindingAvailability? availability) =>
        availability switch
        {
            ViewRibbonBindingAvailability.Disabled => WriterRibbonExecutionProfile.UnavailableCommand,
            _ => null,
        };
}
