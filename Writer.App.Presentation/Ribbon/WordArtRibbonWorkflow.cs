using Writer.Shared.Ribbon;
using Writer.Core.Model;

namespace Writer.App.Presentation.Ribbon;

public sealed record WordArtRibbonPreset<T>(
    RibbonCommandId CommandId,
    string Label,
    string? KeyTip,
    T Value) where T : struct, Enum;

public sealed record WordArtRibbonPorts(
    Func<bool> HasSelection,
    Action<WordArtStyle> ApplyStyle,
    Action<WordArtWarp> ApplyWarp,
    Action PrepareExecution);

/// <summary>
/// Owns the WordArt style/warp command catalog and command-state policy. Native editors only resolve
/// their current WordArt selection to a model target and repaint after the shared object edit.
/// </summary>
public static class WordArtRibbonWorkflow
{
    public static RibbonCommandId StyleMenuCommandId { get; } = new("writer.wordart-style");

    public static RibbonCommandId WarpMenuCommandId { get; } = new("writer.wordart-transform");

    public static IReadOnlyList<WordArtRibbonPreset<WordArtStyle>> StylePresets { get; } =
    [
        Style("writer.wordart-style-fill-blue", "Fill: Blue", "B", WordArtStyle.FillBlue),
        Style("writer.wordart-style-gradient", "Gradient Fill", "G", WordArtStyle.GradientFill),
        Style("writer.wordart-style-outline", "Outline", "O", WordArtStyle.Outline),
        Style("writer.wordart-style-shadow", "Shadow", "S", WordArtStyle.Shadow),
        Style("writer.wordart-style-fill-gold", "Fill: Gold", "D", WordArtStyle.FillGold),
        Style("writer.wordart-style-fill-white", "Fill: White", "W", WordArtStyle.FillWhite),
        Style("writer.wordart-style-grad-multi", "Gradient: Multicolour", "M", WordArtStyle.GradFillMulti),
        Style("writer.wordart-style-chrome-one", "Outline Only", "L", WordArtStyle.ChromeOne),
        Style("writer.wordart-style-chrome-two", "White + Outline", "H", WordArtStyle.ChromeTwo),
        Style("writer.wordart-style-shadow-orange", "Shadow: Orange", "A", WordArtStyle.ShadowOrange),
        Style("writer.wordart-style-glow-blue", "Glow: Blue", "U", WordArtStyle.GlowBlue),
        Style("writer.wordart-style-glow-gold", "Glow: Gold", "I", WordArtStyle.GlowGold),
        Style("writer.wordart-style-reflection", "Reflection", "F", WordArtStyle.Reflection),
        Style("writer.wordart-style-bevel", "Bevel", "V", WordArtStyle.Bevel),
        Style("writer.wordart-style-pattern", "Pattern Fill", "P", WordArtStyle.PatternFill),
    ];

    public static IReadOnlyList<WordArtRibbonPreset<WordArtWarp>> WarpPresets { get; } =
    [
        Warp("writer.wordart-warp-none", "No Transform", "N", WordArtWarp.None),
        Warp("writer.wordart-warp-arch-up", "Arch Up", "A", WordArtWarp.ArchUp),
        Warp("writer.wordart-warp-arch-down", "Arch Down", "D", WordArtWarp.ArchDown),
        Warp("writer.wordart-warp-circle", "Circle", "C", WordArtWarp.Circle),
        Warp("writer.wordart-warp-wave1", "Wave 1", "W", WordArtWarp.Wave1),
        Warp("writer.wordart-warp-wave2", "Wave 2", "V", WordArtWarp.Wave2),
        Warp("writer.wordart-warp-inflate", "Inflate", "I", WordArtWarp.Inflate),
        Warp("writer.wordart-warp-deflate", "Deflate", "E", WordArtWarp.Deflate),
        Warp("writer.wordart-warp-chevron-up", "Chevron Up", "U", WordArtWarp.ChevronUp),
        Warp("writer.wordart-warp-chevron-down", "Chevron Down", "H", WordArtWarp.ChevronDown),
        Warp("writer.wordart-warp-fade-right", "Fade Right", "F", WordArtWarp.FadeRight),
        Warp("writer.wordart-warp-fade-left", "Fade Left", "L", WordArtWarp.FadeLeft),
        Warp("writer.wordart-warp-slant-up", "Slant Up", "S", WordArtWarp.SlantUp),
        Warp("writer.wordart-warp-slant-down", "Slant Down", "T", WordArtWarp.SlantDown),
    ];

    public static RibbonCommandId StyleCommandId(WordArtStyle style) =>
        StylePresets.Single(preset => preset.Value == style).CommandId;

    public static RibbonCommandId WarpCommandId(WordArtWarp warp) => warp switch
    {
        WordArtWarp.Button => new("writer.wordart-warp-button"),
        WordArtWarp.InflateBottom => new("writer.wordart-warp-inflate-bottom"),
        _ => WarpPresets.Single(preset => preset.Value == warp).CommandId,
    };

    public static void Register(IRibbonCommandRegistry registry, WordArtRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(ports);

        registry.Register(StyleMenuCommandId, MenuCommand(ports));
        registry.Register(WarpMenuCommandId, MenuCommand(ports));

        foreach (var style in Enum.GetValues<WordArtStyle>())
        {
            var captured = style;
            registry.Register(
                StyleCommandId(captured),
                Command(() => ports.ApplyStyle(captured), ports));
        }

        foreach (var warp in Enum.GetValues<WordArtWarp>())
        {
            var captured = warp;
            registry.Register(
                WarpCommandId(captured),
                Command(() => ports.ApplyWarp(captured), ports));
        }
    }

    private static IRibbonStatefulCommand Command(Action execute, WordArtRibbonPorts ports) =>
        new WriterRibbonStatefulPortCommand(
            _ => execute(),
            () => new RibbonCommandState(IsEnabled: ports.HasSelection()),
            ports.PrepareExecution);

    private static IRibbonStatefulCommand MenuCommand(WordArtRibbonPorts ports) =>
        new WriterRibbonStatefulPortCommand(
            _ => { },
            () => new RibbonCommandState(IsEnabled: ports.HasSelection()));

    private static WordArtRibbonPreset<WordArtStyle> Style(
        string commandId,
        string label,
        string keyTip,
        WordArtStyle style) =>
        new(new RibbonCommandId(commandId), label, keyTip, style);

    private static WordArtRibbonPreset<WordArtWarp> Warp(
        string commandId,
        string label,
        string keyTip,
        WordArtWarp warp) =>
        new(new RibbonCommandId(commandId), label, keyTip, warp);
}
