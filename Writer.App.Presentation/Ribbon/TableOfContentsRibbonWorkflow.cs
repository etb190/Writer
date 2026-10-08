using Writer.Shared.Ribbon;

namespace Writer.App.Presentation.Ribbon;

public sealed record TableOfContentsRibbonPorts(
    Action Insert,
    Action Refresh,
    Action<string> ApplyParagraphStyle);

public sealed record TableOfContentsStyleChoice(RibbonCommandId CommandId, string StyleId);

/// <summary>
/// Owns References &gt; Table of Contents command identity and Add Text policy for both renderers.
/// Hosts adapt editor focus/layout details, while Presentation owns the identical style choices.
/// </summary>
public static class TableOfContentsRibbonWorkflow
{
    private static readonly TableOfContentsStyleChoice[] StyleChoiceItems =
    [
        new("writer.toc-add-text", "Heading1"),
        new("writer.toc-addtext-none", "Normal"),
        new("writer.toc-addtext-level1", "Heading1"),
        new("writer.toc-addtext-level2", "Heading2"),
        new("writer.toc-addtext-level3", "Heading3"),
    ];

    public static IReadOnlyList<TableOfContentsStyleChoice> StyleChoices => StyleChoiceItems;

    public static void Register(
        WriterRibbonEditorCommandFamilyBuilder bindings,
        TableOfContentsRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        RegisterCore(
            ports,
            (action, execute) => bindings.BindAction(action, execute),
            bindings.Register);
    }

    public static void Register(
        IRibbonCommandRegistry bindings,
        TableOfContentsRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        RegisterCore(
            ports,
            (action, execute) => bindings.BindAction(action, execute),
            bindings.Register);
    }

    private static void RegisterCore(
        TableOfContentsRibbonPorts ports,
        Func<WriterRibbonCommandAction, Action, IRibbonCommand> bindAction,
        Action<RibbonCommandId, IRibbonCommand> register)
    {
        ArgumentNullException.ThrowIfNull(ports);
        ArgumentNullException.ThrowIfNull(ports.Insert);
        ArgumentNullException.ThrowIfNull(ports.Refresh);
        ArgumentNullException.ThrowIfNull(ports.ApplyParagraphStyle);

        var insert = bindAction(WriterRibbonCommandAction.Toc, ports.Insert);
        var refresh = bindAction(WriterRibbonCommandAction.TocRefresh, ports.Refresh);
        register("writer.insert-toc", insert);
        register("writer.update-toc", refresh);

        foreach (var choice in StyleChoiceItems)
        {
            var captured = choice;
            register(
                captured.CommandId,
                new ActionRibbonCommand(() => ports.ApplyParagraphStyle(captured.StyleId)));
        }
    }
}
