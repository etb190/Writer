using Writer.Shared.Ribbon;
using Writer.App.Presentation.Dialogs;
using Writer.Core.Model;

namespace Writer.App.Presentation.Ribbon;

/// <summary>Renderer adapters consumed by the shared Multilevel List command family.</summary>
public sealed record MultilevelListRibbonPorts(
    Action<MultilevelListDefinition> ApplyDefinition,
    Action<int> ChangeLevel,
    Action? OpenDefineDialog);

/// <summary>
/// Owns the complete Home &gt; Paragraph &gt; Multilevel List command policy for both renderers.
/// Native hosts provide only model adapters and the toolkit-specific definition dialog.
/// </summary>
public static class MultilevelListRibbonWorkflow
{
    public static void Register(
        WriterRibbonCommandBindingPorts bindings,
        MultilevelListRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(ports);
        ArgumentNullException.ThrowIfNull(ports.ApplyDefinition);
        ArgumentNullException.ThrowIfNull(ports.ChangeLevel);

        bindings.Bind(
            WriterRibbonCommandAction.MultilevelList,
            new ActionRibbonCommand(() =>
                ports.ApplyDefinition(MultilevelListDialogPlanner.DefaultDefinition)));
        bindings.Bind(
            WriterRibbonCommandAction.MultilevelDemote,
            new ActionRibbonCommand(() => ports.ChangeLevel(+1)));
        bindings.Bind(
            WriterRibbonCommandAction.MultilevelPromote,
            new ActionRibbonCommand(() => ports.ChangeLevel(-1)));

        foreach (var preset in MultilevelListDialogPlanner.Presets)
        {
            var captured = preset;
            bindings.Register(
                captured.CommandId,
                new ActionRibbonCommand(() => ports.ApplyDefinition(captured.Definition)));
        }

        bindings.Bind(
            WriterRibbonCommandAction.MultilevelDefine,
            ports.OpenDefineDialog is null
                ? WriterRibbonExecutionProfile.UnavailableCommand
                : new ActionRibbonCommand(ports.OpenDefineDialog));
    }
}
