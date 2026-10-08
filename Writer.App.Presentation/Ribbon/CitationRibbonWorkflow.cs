using Writer.Shared.Ribbon;
using Writer.Core.Model;

namespace Writer.App.Presentation.Ribbon;

public sealed record CitationRibbonPorts(
    IRibbonCommand InsertCitation,
    IRibbonCommand ManageSources,
    IRibbonCommand InsertBibliography,
    Action<CitationStyle> ApplyStyle,
    Func<CitationStyle> GetStyle,
    Action<RibbonCommandState>? StyleStateChanged = null);

public sealed record CitationRibbonRegistration(IRibbonStatefulCommand CitationStyleCommand);

/// <summary>
/// Owns References citation/bibliography command identity and citation-style value translation for
/// both renderers. Native dialogs and editor mutations remain renderer adapters.
/// </summary>
public static class CitationRibbonWorkflow
{
    public const string InsertCitationCompatibilityId = "writer.insert-citation";

    public static CitationRibbonRegistration Register(
        WriterRibbonEditorCommandFamilyBuilder bindings,
        CitationRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        return RegisterCore(bindings.Bind, bindings.Register, ports);
    }

    public static CitationRibbonRegistration Register(IRibbonCommandRegistry bindings, CitationRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        return RegisterCore(
            (action, command) => bindings.Bind(action, command),
            bindings.Register,
            ports);
    }

    private static CitationRibbonRegistration RegisterCore(
        Func<WriterRibbonCommandAction, IRibbonCommand, IRibbonCommand> bind,
        Action<RibbonCommandId, IRibbonCommand> register,
        CitationRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(ports);
        ArgumentNullException.ThrowIfNull(ports.ApplyStyle);
        ArgumentNullException.ThrowIfNull(ports.GetStyle);

        bind(WriterRibbonCommandAction.Citation, ports.InsertCitation);
        register(InsertCitationCompatibilityId, ports.InsertCitation);
        bind(WriterRibbonCommandAction.ManageSources, ports.ManageSources);
        bind(WriterRibbonCommandAction.Bibliography, ports.InsertBibliography);

        var styleCommand = new WriterRibbonChoiceCommand(
            value => ports.ApplyStyle(Citations.ParseStyle(value, ports.GetStyle())),
            () => Citations.StyleName(ports.GetStyle()),
            ports.StyleStateChanged);
        bind(WriterRibbonCommandAction.CitationStyle, styleCommand);
        return new CitationRibbonRegistration(styleCommand);
    }
}
