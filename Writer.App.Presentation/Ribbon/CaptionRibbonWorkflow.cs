using Writer.Shared.Ribbon;
using Writer.Core.Model;

namespace Writer.App.Presentation.Ribbon;

public sealed record CaptionRibbonPorts(
    IRibbonCommand InsertCaption,
    Action<CaptionLabel>? InsertCaptionWithLabel,
    IRibbonCommand CrossReference);

/// <summary>
/// Owns References &gt; Captions command routing for both renderers. Native dialogs remain renderer
/// ports, while command identity, fixed-label routing, compatibility aliases, and fail-closed
/// availability stay shared.
/// </summary>
public static class CaptionRibbonWorkflow
{
    public static void Register(
        WriterRibbonEditorCommandFamilyBuilder bindings,
        CaptionRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        RegisterCore(bindings.Bind, bindings.Register, ports);
    }

    public static void Register(IRibbonCommandRegistry bindings, CaptionRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        RegisterCore(
            (action, command) => bindings.Bind(action, command),
            bindings.Register,
            ports);
    }

    private static void RegisterCore(
        Func<WriterRibbonCommandAction, IRibbonCommand, IRibbonCommand> bind,
        Action<RibbonCommandId, IRibbonCommand> register,
        CaptionRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(ports);
        ArgumentNullException.ThrowIfNull(ports.InsertCaption);
        ArgumentNullException.ThrowIfNull(ports.CrossReference);

        bind(WriterRibbonCommandAction.Caption, ports.InsertCaption);
        register("writer.insert-caption", ports.InsertCaption);
        bind(WriterRibbonCommandAction.InsertCaption_Figure, LabelCommand(CaptionLabel.Figure));
        bind(WriterRibbonCommandAction.InsertCaption_Table, LabelCommand(CaptionLabel.Table));
        bind(WriterRibbonCommandAction.InsertCaption_Equation, LabelCommand(CaptionLabel.Equation));
        bind(WriterRibbonCommandAction.CrossReference, ports.CrossReference);

        IRibbonCommand LabelCommand(CaptionLabel label) =>
            ports.InsertCaptionWithLabel is { } insert
                ? new ActionRibbonCommand(() => insert(label))
                : WriterRibbonExecutionProfile.UnavailableCommand;
    }
}
