using Writer.Shared.Ribbon;

namespace Writer.App.Presentation.Ribbon;

public sealed record ReviewChangeRibbonPorts(
    Action? PreviousChange,
    Action? NextChange,
    Action? AcceptSelectedChange,
    Action? RejectSelectedChange);

/// <summary>
/// Owns Review &gt; Changes navigation and selected-change routing for both renderers. The host
/// supplies Reviewing Pane selection operations; missing native selection endpoints fail closed.
/// </summary>
public static class ReviewChangeRibbonWorkflow
{
    public static void Register(
        WriterRibbonCommandBindingPorts bindings,
        ReviewChangeRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(ports);

        BindOptional(WriterRibbonCommandAction.PreviousChange, ports.PreviousChange);
        BindOptional(WriterRibbonCommandAction.NextChange, ports.NextChange);
        var accept = BindOptional(WriterRibbonCommandAction.AcceptThis, ports.AcceptSelectedChange);
        var reject = BindOptional(WriterRibbonCommandAction.RejectThis, ports.RejectSelectedChange);
        bindings.Register("writer.accept-change", accept);
        bindings.Register("writer.reject-change", reject);

        IRibbonCommand BindOptional(WriterRibbonCommandAction action, Action? execute)
        {
            var command = execute is null
                ? WriterRibbonExecutionProfile.UnavailableCommand
                : new ActionRibbonCommand(execute);
            return bindings.Bind(action, command);
        }
    }
}
