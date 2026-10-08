using Writer.Shared.Ribbon;

namespace Writer.App.Presentation.Ribbon;

public sealed record InsertMediaRibbonPorts(
    IRibbonCommand Chart,
    IRibbonCommand SmartArt,
    IRibbonCommand Icon,
    IRibbonCommand WordArt,
    IRibbonCommand EmbeddedObject);

/// <summary>
/// Owns Insert-tab media command mapping for both renderers. Dialogs, platform services, and editor
/// calls remain native command adapters supplied through the ports.
/// </summary>
public static class InsertMediaRibbonWorkflow
{
    public static IReadOnlyList<WriterRibbonCommandAction> Actions { get; } =
    [
        WriterRibbonCommandAction.Chart,
        WriterRibbonCommandAction.Smartart,
        WriterRibbonCommandAction.InsertIcon,
        WriterRibbonCommandAction.Wordart,
        WriterRibbonCommandAction.Object,
    ];

    public static void Register(IRibbonCommandRegistry bindings, InsertMediaRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(ports);

        bindings.Bind(WriterRibbonCommandAction.Chart, ports.Chart);
        bindings.Bind(WriterRibbonCommandAction.Smartart, ports.SmartArt);
        bindings.Bind(WriterRibbonCommandAction.InsertIcon, ports.Icon);
        bindings.Bind(WriterRibbonCommandAction.Wordart, ports.WordArt);
        bindings.Bind(WriterRibbonCommandAction.Object, ports.EmbeddedObject);
    }
}
