namespace Writer.Ribbon.Definitions;

/// <summary>Describes the canonical ribbon sections a host can realize.</summary>
public sealed record WriterRibbonCapabilities
{
    private static readonly IReadOnlySet<string> WpfOmittedSections = new HashSet<string>(StringComparer.Ordinal)
    {
        WriterRibbonTopologySection.File,
        WriterRibbonTopologySection.SmartArtSize,
    };

    private static readonly IReadOnlySet<string> AvaloniaOmittedSections = new HashSet<string>(StringComparer.Ordinal)
    {
        WriterRibbonTopologySection.DrawingInsert,
        WriterRibbonTopologySection.DrawingText,
    };

    private WriterRibbonCapabilities(
        string name,
        WriterRibbonControlPresentation controlPresentation,
        IReadOnlySet<string> omittedSections,
        IReadOnlyList<string> tabOrder)
    {
        Name = name;
        ControlPresentation = controlPresentation;
        OmittedSections = omittedSections;
        TabOrder = tabOrder;
    }

    public string Name { get; }
    public string TableContextKey { get; } = "table";
    public string PictureContextKey { get; } = "picture";
    public string DrawingContextKey { get; } = "drawing";
    public string ChartContextKey { get; } = "chart";
    public string SmartArtContextKey { get; } = "smartart";
    public bool UsesPortableControlPresentation => ControlPresentation == WriterRibbonControlPresentation.Portable;

    internal WriterRibbonControlPresentation ControlPresentation { get; }
    internal IReadOnlySet<string> OmittedSections { get; }
    internal IReadOnlyList<string> TabOrder { get; }

    internal bool UsesPortableControls => UsesPortableControlPresentation;

    internal bool IncludesSection(string sectionId) => !OmittedSections.Contains(sectionId);

    public static WriterRibbonCapabilities Wpf { get; } = new(
        "WPF",
        WriterRibbonControlPresentation.Desktop,
        WpfOmittedSections,
        [
            "home", "insert", "design", "layout", "references", "mailings", "review", "view", "help", "developer",
            "drawing-format", "picture-format", "chart-design", "chart-format", "smartart-design",
            "table-design", "table-layout", "header-footer-design",
        ]);

    public static WriterRibbonCapabilities Avalonia { get; } = new(
        "Avalonia",
        WriterRibbonControlPresentation.Portable,
        AvaloniaOmittedSections,
        [
            "file", "home", "insert", "design", "layout", "references", "mailings", "review", "view", "help", "developer",
            "table-design", "table-layout", "header-footer-design", "picture-format", "drawing-format",
            "chart-design", "chart-format", "smartart-design",
        ]);
}

internal enum WriterRibbonControlPresentation
{
    Desktop,
    Portable,
}

internal static class WriterRibbonTopologySection
{
    internal const string File = "file";
    internal const string HomeFormatting = "home.formatting";
    internal const string DrawingInsert = "drawing.insert";
    internal const string DrawingText = "drawing.text";
    internal const string SmartArtSize = "smartart.size";
}
