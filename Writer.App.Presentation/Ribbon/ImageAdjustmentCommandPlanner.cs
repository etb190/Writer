using Writer.Core.Model;

namespace Writer.App.Presentation.Ribbon;

public enum ImageAdjustmentChannel
{
    Brightness,
    Contrast,
    Saturation,
    Transparency
}

public enum ImageEffectChannel
{
    Shadow,
    Reflection,
    Glow,
    SoftEdge,
    Bevel
}

public sealed record ImageAdjustmentPresetDescriptor(
    WriterRibbonCommandAction Action,
    ImageAdjustmentChannel Channel,
    double Value);

public sealed record ImageRecolorPresetDescriptor(
    WriterRibbonCommandAction Action,
    ImageRecolorMode Mode,
    double? ColorTemperature = null);

public sealed record ImageEffectPresetDescriptor(
    ImageEffectChannel Channel,
    double Value,
    WriterRibbonCommandAction? Action = null,
    string? CommandId = null);

public sealed record ImageArtisticEffectPresetDescriptor(
    string CommandId,
    ImageArtisticEffect Effect);

public static class ImageAdjustmentCommandPlanner
{
    public static IReadOnlyList<ImageAdjustmentPresetDescriptor> AdjustmentPresets { get; } =
    [
        new(WriterRibbonCommandAction.ImageBrightnessPlus20, ImageAdjustmentChannel.Brightness, 20),
        new(WriterRibbonCommandAction.ImageBrightnessPlus40, ImageAdjustmentChannel.Brightness, 40),
        new(WriterRibbonCommandAction.ImageBrightnessMinus20, ImageAdjustmentChannel.Brightness, -20),
        new(WriterRibbonCommandAction.ImageBrightnessMinus40, ImageAdjustmentChannel.Brightness, -40),
        new(WriterRibbonCommandAction.ImageContrastPlus20, ImageAdjustmentChannel.Contrast, 20),
        new(WriterRibbonCommandAction.ImageContrastMinus20, ImageAdjustmentChannel.Contrast, -20),
        new(WriterRibbonCommandAction.ImageSaturation0, ImageAdjustmentChannel.Saturation, 0),
        new(WriterRibbonCommandAction.ImageSaturation50, ImageAdjustmentChannel.Saturation, 50),
        new(WriterRibbonCommandAction.ImageSaturation200, ImageAdjustmentChannel.Saturation, 200),
        new(WriterRibbonCommandAction.ImageTransparency25, ImageAdjustmentChannel.Transparency, 25),
        new(WriterRibbonCommandAction.ImageTransparency50, ImageAdjustmentChannel.Transparency, 50),
        new(WriterRibbonCommandAction.ImageTransparency75, ImageAdjustmentChannel.Transparency, 75),
    ];

    public static IReadOnlyList<ImageRecolorPresetDescriptor> RecolorPresets { get; } =
    [
        new(WriterRibbonCommandAction.ImageRecolorGrayscale, ImageRecolorMode.Grayscale),
        new(WriterRibbonCommandAction.ImageRecolorSepia, ImageRecolorMode.Sepia),
        new(WriterRibbonCommandAction.ImageRecolorWashout, ImageRecolorMode.Washout),
        new(WriterRibbonCommandAction.ImageRecolorBlackwhite, ImageRecolorMode.BlackWhite),
        new(WriterRibbonCommandAction.ImageRecolorNone, ImageRecolorMode.None),
        new(WriterRibbonCommandAction.ImageColortempWarm, ImageRecolorMode.None, 60),
        new(WriterRibbonCommandAction.ImageColortempCool, ImageRecolorMode.None, -60),
        new(WriterRibbonCommandAction.ImageColortempNeutral, ImageRecolorMode.None, 0),
    ];

    public static IReadOnlyList<ImageEffectPresetDescriptor> EffectPresets { get; } =
        BuildEffectPresets();

    public static IReadOnlyList<ImageArtisticEffectPresetDescriptor> ArtisticEffectPresets { get; } =
    [
        new("writer.image-artistic-none", ImageArtisticEffect.None),
        new("writer.image-artistic-blur", ImageArtisticEffect.Blur),
        new("writer.image-artistic-glow-diffused", ImageArtisticEffect.GlowDiffused),
        new("writer.image-artistic-glow-edges", ImageArtisticEffect.GlowEdges),
        new("writer.image-artistic-pencil-gray", ImageArtisticEffect.PencilGrayscale),
        new("writer.image-artistic-pencil-sketch", ImageArtisticEffect.PencilSketch),
        new("writer.image-artistic-line-drawing", ImageArtisticEffect.LineDrawing),
        new("writer.image-artistic-paintbrush", ImageArtisticEffect.Paintbrush),
        new("writer.image-artistic-paint-strokes", ImageArtisticEffect.PaintStrokes),
        new("writer.image-artistic-photocopy", ImageArtisticEffect.Photocopy),
        new("writer.image-artistic-posterize", ImageArtisticEffect.Posterize),
        new("writer.image-artistic-pastels", ImageArtisticEffect.Pastels),
        new("writer.image-artistic-watercolor", ImageArtisticEffect.Watercolor),
        new("writer.image-artistic-film-grain", ImageArtisticEffect.FilmGrain),
        new("writer.image-artistic-mosaic", ImageArtisticEffect.Mosaic),
    ];

    private static IReadOnlyList<ImageEffectPresetDescriptor> BuildEffectPresets()
    {
        var presets = new List<ImageEffectPresetDescriptor>
        {
            new(ImageEffectChannel.Shadow, 0, Action: WriterRibbonCommandAction.ImageShadowNone),
            new(ImageEffectChannel.Reflection, 0, Action: WriterRibbonCommandAction.ImageReflectionNone),
            new(ImageEffectChannel.Bevel, 0, Action: WriterRibbonCommandAction.ImageBevelNone),
        };
        presets.AddRange(Enumerable.Range(1, 5)
            .Select(value => new ImageEffectPresetDescriptor(
                ImageEffectChannel.Shadow, value, CommandId: $"writer.image-shadow-{value}")));
        presets.AddRange(Enumerable.Range(1, 5)
            .Select(value => new ImageEffectPresetDescriptor(
                ImageEffectChannel.Reflection, value, CommandId: $"writer.image-reflection-{value}")));
        presets.AddRange(new[] { 0d, 5d, 8d, 11d, 18d }
            .Select(value => new ImageEffectPresetDescriptor(
                ImageEffectChannel.Glow,
                value,
                CommandId: $"writer.image-glow-{(value == 0 ? "none" : value.ToString("0", System.Globalization.CultureInfo.InvariantCulture))}")));
        presets.AddRange(new[] { 0d, 1d, 2.5d, 5d, 10d }
            .Select(value => new ImageEffectPresetDescriptor(
                ImageEffectChannel.SoftEdge,
                value,
                CommandId: $"writer.image-softedge-{SoftEdgeSuffix(value)}")));
        presets.AddRange(Enumerable.Range(1, 4)
            .Select(value => new ImageEffectPresetDescriptor(
                ImageEffectChannel.Bevel, value, CommandId: $"writer.image-bevel-{value}")));
        return presets;
    }

    private static string SoftEdgeSuffix(double value) => value switch
    {
        0 => "none",
        2.5 => "2pt5",
        _ => value.ToString("0", System.Globalization.CultureInfo.InvariantCulture),
    };
}
