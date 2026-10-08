using Avalonia.Media;
using Writer.Shared.Shell;
using BrandTheme = Writer.Shared.Theme.Theme;

namespace Writer.Shared.Shell.Avalonia;

/// <summary>Native Avalonia materialization of the shared sister-app Backstage palette.</summary>
public sealed record AvaloniaSisterBackstageTheme(
    AvaloniaBackstageAccent Accent,
    Color LinkColor,
    double TileWidth,
    double TileHeight)
{
    public static AvaloniaSisterBackstageTheme Writer { get; } = FromPalette(SisterBackstagePalette.Writer);

    public static AvaloniaSisterBackstageTheme FreeP { get; } = FromPalette(SisterBackstagePalette.FreeP);

    public static AvaloniaSisterBackstageTheme FromTheme(BrandTheme theme, double tileWidth, double tileHeight) =>
        FromPalette(SisterBackstagePalette.FromTheme(theme, tileWidth, tileHeight));

    private static AvaloniaSisterBackstageTheme FromPalette(SisterBackstagePalette palette) => new(
        new AvaloniaBackstageAccent(
            ToColor(palette.Sidebar),
            ToColor(palette.Hover),
            ToColor(palette.Selected),
            ToColor(palette.Separator)),
        ToColor(palette.Link),
        palette.TileWidth,
        palette.TileHeight);

    private static Color ToColor(BackstageRgb color) => Color.FromRgb(color.R, color.G, color.B);
}
