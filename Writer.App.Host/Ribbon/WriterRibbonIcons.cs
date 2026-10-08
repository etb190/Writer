using System.Collections.Generic;
using Writer.Shared.Ribbon;
using Writer.App.Presentation.Ribbon;

namespace Writer.App.Host;

/// <summary>
/// Installs Writer's app-local SVG loader and supplies geometry fallbacks for menu or legacy command ids
/// that are not controls in the current WPF ribbon definition. Control icons come directly from definition
/// metadata and must not be repeated here.
/// </summary>
internal static class WriterRibbonIcons
{
    /// <summary>Installs Writer's SVG loader and legacy command-id to glyph resolver.</summary>
    public static void Install()
    {
        Writer.Shared.Ribbon.Wpf.RibbonIconFactory.CommandIconElementResolver = RibbonIconFactory.TryCreateCommandIcon;
        Writer.Shared.Ribbon.Wpf.RibbonIconFactory.CommandIconKindResolver = Resolve;
    }

    public static RibbonCommandIconKind? Resolve(string commandId) =>
        FallbackMap.TryGetValue(commandId, out var kind) ? kind : null;

    internal static IReadOnlyDictionary<string, RibbonCommandIconKind> Fallbacks => FallbackMap;

    private static readonly IReadOnlyDictionary<string, RibbonCommandIconKind> FallbackMap =
        new Dictionary<string, RibbonCommandIconKind>(System.StringComparer.OrdinalIgnoreCase)
        {
            ["writer.multilevel-demote"] = RibbonCommandIconKind.IndentIncrease,
            ["writer.multilevel-promote"] = RibbonCommandIconKind.IndentDecrease,

            ["writer.image-wrap-inline"] = RibbonCommandIconKind.Wrap,
            ["writer.image-wrap-square"] = RibbonCommandIconKind.Wrap,
            ["writer.image-wrap-tight"] = RibbonCommandIconKind.Wrap,
            ["writer.image-wrap-top-bottom"] = RibbonCommandIconKind.Wrap,
            ["writer.image-wrap-behind"] = RibbonCommandIconKind.Wrap,
            ["writer.image-wrap-front"] = RibbonCommandIconKind.Wrap,
            ["writer.shape-rectangle"] = RibbonCommandIconKind.Rectangle,
            ["writer.shape-rounded"] = RibbonCommandIconKind.Rectangle,
            ["writer.shape-ellipse"] = RibbonCommandIconKind.Ellipse,
            ["writer.screen-clipping"] = RibbonCommandIconKind.Picture,

            ["writer.shape-change-rectangle"] = RibbonCommandIconKind.Rectangle,
            ["writer.shape-change-rounded"] = RibbonCommandIconKind.Rectangle,
            ["writer.shape-change-ellipse"] = RibbonCommandIconKind.Ellipse,
            ["writer.shape-fill-no-fill"] = RibbonCommandIconKind.Fill,
            ["writer.shape-outline-no-outline"] = RibbonCommandIconKind.Border,
            ["writer.shape-outline-solid"] = RibbonCommandIconKind.Border,
            ["writer.shape-outline-dash"] = RibbonCommandIconKind.Border,
            ["writer.shape-outline-dot"] = RibbonCommandIconKind.Border,
            ["writer.shape-text-horizontal"] = RibbonCommandIconKind.TextBox,
            ["writer.shape-text-rotate90"] = RibbonCommandIconKind.Rotate,
            ["writer.shape-text-rotate270"] = RibbonCommandIconKind.Rotate,
            ["writer.shape-fill-gradient-blue"] = RibbonCommandIconKind.Fill,
            ["writer.shape-fill-gradient-orange"] = RibbonCommandIconKind.Fill,
            ["writer.shape-fill-pattern-diag"] = RibbonCommandIconKind.Fill,
            ["writer.shape-effects-none"] = RibbonCommandIconKind.Effects,
            ["writer.shape-effect-shadow"] = RibbonCommandIconKind.Effects,
            ["writer.shape-effect-glow"] = RibbonCommandIconKind.Effects,
            ["writer.shape-effect-soft-edge"] = RibbonCommandIconKind.Effects,
            ["writer.shape-effect-reflection"] = RibbonCommandIconKind.Effects,
            ["writer.shape-effect-bevel"] = RibbonCommandIconKind.Effects,
            ["writer.wordart-style-fill-blue"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-style-gradient"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-style-outline"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-style-shadow"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-style-fill-gold"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-style-fill-white"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-style-grad-multi"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-style-chrome-one"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-style-chrome-two"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-style-shadow-orange"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-style-glow-blue"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-style-glow-gold"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-style-reflection"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-style-bevel"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-style-pattern"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-warp-none"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-warp-arch-up"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-warp-arch-down"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-warp-circle"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-warp-button"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-warp-wave1"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-warp-wave2"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-warp-inflate"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-warp-deflate"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-warp-inflate-bottom"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-warp-chevron-up"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-warp-chevron-down"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-warp-fade-right"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-warp-fade-left"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-warp-slant-up"] = RibbonCommandIconKind.WordArt,
            ["writer.wordart-warp-slant-down"] = RibbonCommandIconKind.WordArt,

            ["writer.chart-type-bar"] = RibbonCommandIconKind.ChartColumn,
            ["writer.chart-type-line"] = RibbonCommandIconKind.ChartColumn,
            ["writer.chart-type-pie"] = RibbonCommandIconKind.ChartColumn,
            ["writer.chart-type-scatter"] = RibbonCommandIconKind.ChartColumn,
            ["writer.chart-type-area"] = RibbonCommandIconKind.ChartColumn,
            ["writer.chart-type-doughnut"] = RibbonCommandIconKind.ChartColumn,

            [EquationPresetCatalog.Get(EquationPresetKind.Fraction).CommandId] = RibbonCommandIconKind.Equation,
            [EquationPresetCatalog.Get(EquationPresetKind.Script).CommandId] = RibbonCommandIconKind.Equation,
            [EquationPresetCatalog.Get(EquationPresetKind.Radical).CommandId] = RibbonCommandIconKind.Equation,
            [EquationPresetCatalog.Get(EquationPresetKind.NthRoot).CommandId] = RibbonCommandIconKind.Equation,
            [EquationPresetCatalog.Get(EquationPresetKind.Integral).CommandId] = RibbonCommandIconKind.Equation,
            [EquationPresetCatalog.Get(EquationPresetKind.Summation).CommandId] = RibbonCommandIconKind.Equation,
            [EquationPresetCatalog.Get(EquationPresetKind.Product).CommandId] = RibbonCommandIconKind.Equation,
            [EquationPresetCatalog.Get(EquationPresetKind.Accent).CommandId] = RibbonCommandIconKind.Equation,
            [EquationPresetCatalog.Get(EquationPresetKind.Bar).CommandId] = RibbonCommandIconKind.Equation,
            [EquationPresetCatalog.Get(EquationPresetKind.Bracket).CommandId] = RibbonCommandIconKind.Equation,
            [EquationPresetCatalog.Get(EquationPresetKind.Matrix).CommandId] = RibbonCommandIconKind.Equation,
            [EquationPresetCatalog.Get(EquationPresetKind.Function).CommandId] = RibbonCommandIconKind.Equation,
            [EquationPresetCatalog.Get(EquationPresetKind.GroupCharacter).CommandId] = RibbonCommandIconKind.Equation,

            ["writer.previous-footnote"] = RibbonCommandIconKind.Footnote,
            ["writer.next-endnote"] = RibbonCommandIconKind.Endnote,
            ["writer.previous-endnote"] = RibbonCommandIconKind.Endnote,

            ["writer.hf-edit-even-header"]       = RibbonCommandIconKind.Header,
            ["writer.hf-edit-even-footer"]       = RibbonCommandIconKind.Footer,
            ["writer.hf-edit-first-header"]      = RibbonCommandIconKind.Header,
            ["writer.hf-edit-first-footer"]      = RibbonCommandIconKind.Footer,
            ["writer.hf-header-from-top"]        = RibbonCommandIconKind.Margins,
            ["writer.hf-footer-from-bottom"]     = RibbonCommandIconKind.Margins,
            ["writer.hf-insert-page-number-footer"] = RibbonCommandIconKind.PageNumber,


            ["writer.read-mode-column-narrow"]  = RibbonCommandIconKind.ReadMode,
            ["writer.read-mode-column-default"] = RibbonCommandIconKind.ReadMode,
            ["writer.read-mode-column-wide"]    = RibbonCommandIconKind.ReadMode,
            ["writer.read-mode-color-none"]    = RibbonCommandIconKind.ReadMode,
            ["writer.read-mode-color-sepia"]   = RibbonCommandIconKind.ReadMode,
            ["writer.read-mode-color-inverse"] = RibbonCommandIconKind.ReadMode,

            ["writer.start-mail-merge-letters"] = RibbonCommandIconKind.Envelope,
            ["writer.start-mail-merge-directory"] = RibbonCommandIconKind.Labels,
            ["writer.start-mail-merge-normal"] = RibbonCommandIconKind.Page,
            ["writer.merge-next-record"] = RibbonCommandIconKind.Next,
            ["writer.merge-record-number"] = RibbonCommandIconKind.Field,
            ["writer.merge-sequence-number"] = RibbonCommandIconKind.Field,
            ["writer.merge-rule-if"] = RibbonCommandIconKind.Field,
            ["writer.merge-rule-skip-record-if"] = RibbonCommandIconKind.Field,
            ["writer.merge-rule-next-record-if"] = RibbonCommandIconKind.Next,
            ["writer.merge-rule-fill-in"] = RibbonCommandIconKind.Field,
            ["writer.merge-rule-ask"] = RibbonCommandIconKind.Field,
            ["writer.merge-rule-set"] = RibbonCommandIconKind.Field,
            ["writer.merge-rule-ref"] = RibbonCommandIconKind.Field,

            ["writer.accept-all"] = RibbonCommandIconKind.AcceptChange,
            ["writer.reject-all"] = RibbonCommandIconKind.RejectChange,
            ["writer.display-for-review-all-markup"] = RibbonCommandIconKind.History,
            ["writer.show-markup-insertions-deletions"] = RibbonCommandIconKind.History,
            ["writer.show-markup-comments"] = RibbonCommandIconKind.Comment,

            ["writer.show-markup-balloons"] = RibbonCommandIconKind.Comment,
        };
}
