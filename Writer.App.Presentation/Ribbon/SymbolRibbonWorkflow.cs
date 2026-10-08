using Writer.Shared.Ribbon;
using Writer.App.Localization;

namespace Writer.App.Presentation.Ribbon;

public sealed record WriterRibbonSymbolChoice(
    string CommandId,
    string Glyph,
    string Label);

public sealed record SymbolRibbonPorts(
    Action PrepareExecution,
    Action<string> InsertSymbol);

/// <summary>
/// Owns the stable command identity and exact text payload for the Insert &gt; Symbol palette.
/// Renderers retain only their native root picker and an adapter that inserts ordinary text.
/// </summary>
public static class SymbolRibbonWorkflow
{
    public static IReadOnlyList<WriterRibbonSymbolChoice> Choices =>
    [
        new("writer.symbol.euro", "€", Loc.Get("Ribbon_Palette_Symbol_Euro_Label")),
        new("writer.symbol.pound", "£", Loc.Get("Ribbon_Palette_Symbol_Pound_Label")),
        new("writer.symbol.yen", "¥", Loc.Get("Ribbon_Palette_Symbol_Yen_Label")),
        new("writer.symbol.cent", "¢", Loc.Get("Ribbon_Palette_Symbol_Cent_Label")),
        new("writer.symbol.copyright", "©", Loc.Get("Ribbon_Palette_Symbol_Copyright_Label")),
        new("writer.symbol.registered", "®", Loc.Get("Ribbon_Palette_Symbol_Registered_Label")),
        new("writer.symbol.trademark", "™", Loc.Get("Ribbon_Palette_Symbol_Trademark_Label")),
        new("writer.symbol.degree", "°", Loc.Get("Ribbon_Palette_Symbol_Degree_Label")),
        new("writer.symbol.plusminus", "±", Loc.Get("Ribbon_Palette_Symbol_PlusMinus_Label")),
        new("writer.symbol.multiply", "×", Loc.Get("Ribbon_Palette_Symbol_Multiplication_Label")),
        new("writer.symbol.divide", "÷", Loc.Get("Ribbon_Palette_Symbol_Division_Label")),
        new("writer.symbol.notequal", "≠", Loc.Get("Ribbon_Palette_Symbol_NotEqual_Label")),
        new("writer.symbol.lessequal", "≤", Loc.Get("Ribbon_Palette_Symbol_LessOrEqual_Label")),
        new("writer.symbol.greaterequal", "≥", Loc.Get("Ribbon_Palette_Symbol_GreaterOrEqual_Label")),
        new("writer.symbol.bullet", "•", Loc.Get("Ribbon_Palette_Symbol_Bullet_Label")),
        new("writer.symbol.ellipsis", "…", Loc.Get("Ribbon_Palette_Symbol_Ellipsis_Label")),
        new("writer.symbol.emdash", "—", Loc.Get("Ribbon_Palette_Symbol_EmDash_Label")),
        new("writer.symbol.endash", "–", Loc.Get("Ribbon_Palette_Symbol_EnDash_Label")),
        new("writer.symbol.arrow-right", "→", Loc.Get("Ribbon_Palette_Symbol_RightArrow_Label")),
        new("writer.symbol.arrow-left", "←", Loc.Get("Ribbon_Palette_Symbol_LeftArrow_Label")),
    ];

    public static void Register(IRibbonCommandRegistry registry, SymbolRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(ports);

        foreach (var choice in Choices)
        {
            var captured = choice;
            registry.Register(
                captured.CommandId,
                new PreparedSymbolCommand(
                    ports.PrepareExecution,
                    () => ports.InsertSymbol(captured.Glyph)));
        }
    }

    private sealed class PreparedSymbolCommand(Action prepare, Action insert) : IRibbonCommand
    {
        public void Execute(RibbonCommandContext context)
        {
            prepare();
            insert();
        }
    }
}
