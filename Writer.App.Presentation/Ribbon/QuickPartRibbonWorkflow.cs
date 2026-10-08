using Writer.Shared.Ribbon;
using Writer.Core.Model;

namespace Writer.App.Presentation.Ribbon;

public sealed record QuickPartRibbonPorts(
    IRibbonCommand InsertSavedPart,
    IRibbonCommand SaveSelection,
    IRibbonCommand OpenOrganizer,
    Action<RunFieldKind> InsertField);

/// <summary>Owns Quick Parts command identity and alias routing for both renderers.</summary>
public static class QuickPartRibbonWorkflow
{
    public static void Register(
        IRibbonCommandRegistry bindings,
        QuickPartRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(ports);
        ArgumentNullException.ThrowIfNull(ports.InsertSavedPart);
        ArgumentNullException.ThrowIfNull(ports.SaveSelection);
        ArgumentNullException.ThrowIfNull(ports.OpenOrganizer);
        ArgumentNullException.ThrowIfNull(ports.InsertField);

        bindings.Register("writer.insert-quickpart", ports.InsertSavedPart);
        bindings.Register("writer.quick-parts", ports.InsertSavedPart);
        bindings.Register("writer.quick-parts.snippet", ports.InsertSavedPart);
        bindings.Bind(WriterRibbonCommandAction.SaveQuickpart, ports.SaveSelection);
        bindings.Bind(WriterRibbonCommandAction.BuildingBlocksOrganizer, ports.OpenOrganizer);

        DocumentPropertyFieldPlanner.RegisterCommands(bindings, ports.InsertField);
        bindings.Register(
            "writer.quick-parts.date",
            new ActionRibbonCommand(() => ports.InsertField(RunFieldKind.Date)));
    }
}
