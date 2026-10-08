using Writer.Shared.Ribbon;

namespace Writer.App.Presentation.Ribbon;

public sealed record DropCapRibbonPorts(
    IRibbonCommand Dropped,
    IRibbonCommand InMargin,
    IRibbonCommand None,
    IRibbonCommand Options);

/// <summary>
/// Owns Insert &gt; Drop Cap command identity for both renderers. The primary split-button route and
/// both historical submenu id families resolve to the same native commands.
/// </summary>
public static class DropCapRibbonWorkflow
{
    public static IReadOnlyList<WriterRibbonCommandAction> Actions { get; } =
    [
        WriterRibbonCommandAction.DropCap,
        WriterRibbonCommandAction.DropCap_Dropped,
        WriterRibbonCommandAction.DropCap_InMargin,
        WriterRibbonCommandAction.DropCap_None,
        WriterRibbonCommandAction.DropCapDropped,
        WriterRibbonCommandAction.DropCapInMargin,
        WriterRibbonCommandAction.DropCapNone,
        WriterRibbonCommandAction.DropCapOptions,
    ];

    public static void Register(
        IRibbonCommandRegistry bindings,
        DropCapRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(ports);

        bindings.Bind(WriterRibbonCommandAction.DropCap, ports.Dropped);
        bindings.Bind(WriterRibbonCommandAction.DropCap_Dropped, ports.Dropped);
        bindings.Bind(WriterRibbonCommandAction.DropCapDropped, ports.Dropped);
        bindings.Bind(WriterRibbonCommandAction.DropCap_InMargin, ports.InMargin);
        bindings.Bind(WriterRibbonCommandAction.DropCapInMargin, ports.InMargin);
        bindings.Bind(WriterRibbonCommandAction.DropCap_None, ports.None);
        bindings.Bind(WriterRibbonCommandAction.DropCapNone, ports.None);
        bindings.Bind(WriterRibbonCommandAction.DropCapOptions, ports.Options);
    }
}
