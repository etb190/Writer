using Writer.Shared.Ribbon;

namespace Writer.App.Presentation.Ribbon;

public sealed record TableOfAuthoritiesRibbonPorts(
    Action? MarkCitation,
    Action? InsertTableOfAuthorities,
    Action? RefreshTableOfAuthorities,
    Action? PrepareRefresh = null);

/// <summary>
/// Owns References &gt; Table of Authorities routing for both renderers. Native dialog actions remain
/// host ports; absent ports are unavailable rather than silently inserting default content.
/// </summary>
public static class TableOfAuthoritiesRibbonWorkflow
{
    public static void Register(
        WriterRibbonEditorCommandFamilyBuilder bindings,
        TableOfAuthoritiesRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        RegisterCore(bindings.Bind, ports);
    }

    public static void Register(IRibbonCommandRegistry bindings, TableOfAuthoritiesRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        RegisterCore((action, command) => bindings.Bind(action, command), ports);
    }

    private static void RegisterCore(
        Func<WriterRibbonCommandAction, IRibbonCommand, IRibbonCommand> bind,
        TableOfAuthoritiesRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(ports);
        bind(WriterRibbonCommandAction.MarkCitation, Command(ports.MarkCitation));
        bind(WriterRibbonCommandAction.TableOfAuthorities, Command(ports.InsertTableOfAuthorities));
        bind(
            WriterRibbonCommandAction.TableOfAuthoritiesRefresh,
            Command(ports.RefreshTableOfAuthorities, ports.PrepareRefresh));
    }

    private static IRibbonCommand Command(Action? execute, Action? prepare = null) =>
        execute is null
            ? WriterRibbonExecutionProfile.UnavailableCommand
            : new ActionRibbonCommand(() =>
            {
                prepare?.Invoke();
                execute();
            });
}
