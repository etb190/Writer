using Writer.App.Localization;

namespace Writer.App.Presentation.Ribbon;

public sealed record WriterRibbonPaletteChoice(
    string CommandId,
    string Label,
    string? Hex,
    bool StartsNewGroup = false,
    string? PickerLabel = null);

/// <summary>
/// Canonical Writer ribbon palette semantics. Renderers choose their native menu, swatch, and brush
/// controls, while command ids, labels, grouping, and color payloads stay identical.
/// </summary>
public static class WriterRibbonPaletteCatalog
{
    public static IReadOnlyList<WriterRibbonPaletteChoice> FontColors =>
    [
        new("writer.font-color.automatic", Loc.Get("Ribbon_Palette_FontColor_Automatic_Label"), null),
        new("writer.font-color.black", Loc.Get("Ribbon_Palette_FontColor_Black_Label"), "#000000"),
        new("writer.font-color.dark-red", Loc.Get("Ribbon_Palette_FontColor_DarkRed_Label"), "#C00000"),
        new("writer.font-color.red", Loc.Get("Ribbon_Palette_FontColor_Red_Label"), "#FF0000"),
        new("writer.font-color.orange", Loc.Get("Ribbon_Palette_FontColor_Orange_Label"), "#FF6600"),
        new("writer.font-color.yellow", Loc.Get("Ribbon_Palette_FontColor_Yellow_Label"), "#FFFF00"),
        new("writer.font-color.green", Loc.Get("Ribbon_Palette_FontColor_Green_Label"), "#00B050"),
        new("writer.font-color.blue", Loc.Get("Ribbon_Palette_FontColor_Blue_Label"), "#0070C0"),
        new("writer.font-color.dark-blue", Loc.Get("Ribbon_Palette_FontColor_DarkBlue_Label"), "#00008B"),
        new("writer.font-color.purple", Loc.Get("Ribbon_Palette_FontColor_Purple_Label"), "#7030A0"),
        new("writer.font-color.white", Loc.Get("Ribbon_Palette_FontColor_White_Label"), "#FFFFFF"),
    ];

    public static readonly IReadOnlyList<WriterRibbonPaletteChoice> ParagraphShading =
    [
        new("writer.para-shading.yellow", "Yellow", "#FFFF00"),
        new("writer.para-shading.green", "Green", "#92D050"),
        new("writer.para-shading.cyan", "Cyan", "#00B0F0"),
        new("writer.para-shading.gold", "Gold", "#FFC000"),
        new("writer.para-shading.red", "Red", "#FF0000"),
        new("writer.para-shading.gray", "Gray", "#D9D9D9"),
        new("writer.para-shading.light-gray", "Light Gray", "#A6A6A6"),
        new("writer.para-shading.light-yellow", "Light Yellow", "#FFF2CC"),
        new("writer.para-shading.light-blue", "Light Blue", "#DEEBF7"),
        new("writer.para-shading.light-green", "Light Green", "#E2EFDA"),
        new("writer.para-shading.light-peach", "Light Peach", "#FCE4D6"),
        new("writer.para-shading.very-light-gray", "Very Light Gray", "#EDEDED"),
        new("writer.para-shading.none", "No Color", null, StartsNewGroup: true),
    ];

    public static readonly IReadOnlyList<WriterRibbonPaletteChoice> CharacterShading =
    [
        new("writer.char-shading.yellow", "Yellow", "#FFFF00"),
        new("writer.char-shading.green", "Green", "#92D050"),
        new("writer.char-shading.cyan", "Cyan", "#00B0F0"),
        new("writer.char-shading.gold", "Gold", "#FFC000"),
        new("writer.char-shading.red", "Red", "#FF0000"),
        new("writer.char-shading.gray", "Gray", "#D9D9D9"),
        new("writer.char-shading.light-gray", "Light Gray", "#A6A6A6", PickerLabel: "Dark Gray"),
        new("writer.char-shading.light-yellow", "Light Yellow", "#FFF2CC"),
        new("writer.char-shading.light-blue", "Light Blue", "#DEEBF7"),
        new("writer.char-shading.light-green", "Light Green", "#E2EFDA"),
        new("writer.char-shading.light-peach", "Light Peach", "#FCE4D6", PickerLabel: "Light Orange"),
        new("writer.char-shading.very-light-gray", "Very Light Gray", "#EDEDED", PickerLabel: "Light Gray"),
        new("writer.char-shading.none", "No Color", null, StartsNewGroup: true),
    ];

    public static readonly IReadOnlyList<WriterRibbonPaletteChoice> CharacterBorders =
    [
        new("writer.char-border.black", "Black", "#000000"),
        new("writer.char-border.red", "Red", "#FF0000"),
        new("writer.char-border.blue", "Blue", "#0070C0"),
        new("writer.char-border.green", "Green", "#00B050"),
        new("writer.char-border.gold", "Gold", "#FFC000"),
        new("writer.char-border.purple", "Purple", "#7030A0"),
        new("writer.char-border.gray", "Gray", "#808080"),
        new("writer.char-border.dark-red", "Dark Red", "#C00000"),
        new("writer.char-border.dark-blue", "Dark Blue", "#002060"),
        new("writer.char-border.dark-green", "Dark Green", "#375623"),
        new("writer.char-border.brown", "Brown", "#974706"),
        new("writer.char-border.dark-gray", "Dark Gray", "#3F3F3F"),
        new("writer.char-border.none", "No Border", null, StartsNewGroup: true),
    ];

    public static readonly IReadOnlyList<WriterRibbonPaletteChoice> Highlights =
    [
        new("writer.highlight.black", "Black", "#000000"),
        new("writer.highlight.dark-gray", "Dark Gray", "#404040"),
        new("writer.highlight.gray", "Gray", "#7F7F7F"),
        new("writer.highlight.dark-red", "Dark Red", "#C00000"),
        new("writer.highlight.red", "Red", "#FF0000"),
        new("writer.highlight.gold", "Gold", "#FFC000"),
        new("writer.highlight.yellow", "Yellow", "#FFFF00"),
        new("writer.highlight.light-green", "Light Green", "#92D050"),
        new("writer.highlight.green", "Green", "#00B050"),
        new("writer.highlight.cyan", "Cyan", "#00B0F0"),
        new("writer.highlight.blue", "Blue", "#0070C0"),
        new("writer.highlight.dark-blue", "Dark Blue", "#2F5496"),
        new("writer.highlight.purple", "Purple", "#7030A0"),
        new("writer.highlight.white", "White", "#FFFFFF"),
        new("writer.highlight.none", "No Color", null, StartsNewGroup: true),
    ];

    public static IReadOnlyList<WriterRibbonPaletteChoice> PageColors =>
    [
        new("writer.page-color.none", Loc.Get("Ribbon_Palette_PageColor_NoColor_Label"), null),
        new("writer.page-color.white", Loc.Get("Ribbon_Palette_PageColor_White_Label"), "#FFFFFF"),
        new("writer.page-color.light-gray", Loc.Get("Ribbon_Palette_PageColor_LightGray_Label"), "#D9D9D9"),
        new("writer.page-color.tan", Loc.Get("Ribbon_Palette_PageColor_Tan_Label"), "#EAD9C0"),
        new("writer.page-color.light-blue", Loc.Get("Ribbon_Palette_PageColor_LightBlue_Label"), "#DDEBF7"),
        new("writer.page-color.light-green", Loc.Get("Ribbon_Palette_PageColor_LightGreen_Label"), "#E2EFDA"),
        new("writer.page-color.light-yellow", Loc.Get("Ribbon_Palette_PageColor_LightYellow_Label"), "#FFF2CC"),
        new("writer.page-color.rose", Loc.Get("Ribbon_Palette_PageColor_Rose_Label"), "#FCE4EC"),
    ];

    public static IReadOnlyList<string> TextAndHighlightPickerSwatches =>
        Highlights.Where(choice => choice.Hex is not null).Select(choice => choice.Hex!).ToArray();

    public static IReadOnlyList<string> ParagraphShadingPickerSwatches =>
        ParagraphShading.Where(choice => choice.Hex is not null).Select(choice => choice.Hex!).ToArray();

    public static readonly IReadOnlyList<string> PageColorPickerSwatches =
    [
        "#FFFFFF", "#F2F2F2", "#DDD9C3", "#C6D9F1", "#DBE5F1", "#F2DCDB",
        "#EBF1DE", "#E5E0EC", "#FDE9D9", "#FFF2CC", "#DEEBF7", "#E2EFDA",
        "#FCE4D6", "#D9E1F2", "#FFFFCC", "#E2F0D9", "#000000", "#1F1F1F",
    ];
}
