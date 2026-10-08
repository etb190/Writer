namespace Writer.App.Presentation.Ribbon;

public readonly record struct WriterRibbonQuickStyleBinding(
    WriterRibbonCommandAction Action,
    string StyleId);

public readonly record struct WriterRibbonHeaderFooterSlotBinding(
    WriterRibbonCommandAction Action,
    HeaderFooterSlotKind Slot);

/// <summary>Canonical product mappings consumed by both native ribbon command hosts.</summary>
public static class WriterRibbonSemanticCatalog
{
    public static IReadOnlyList<WriterRibbonQuickStyleBinding> QuickStyles { get; } =
    [
        new(WriterRibbonCommandAction.StyleNormal, "Normal"),
        new(WriterRibbonCommandAction.StyleHeading1, "Heading1"),
        new(WriterRibbonCommandAction.StyleHeading2, "Heading2"),
        new(WriterRibbonCommandAction.StyleHeading3, "Heading3"),
        new(WriterRibbonCommandAction.StyleTitle, "Title"),
    ];

    public static IReadOnlyList<WriterRibbonHeaderFooterSlotBinding> HeaderFooterEditSlots { get; } =
    [
        new(WriterRibbonCommandAction.HfEditHeader, HeaderFooterSlotKind.Header),
        new(WriterRibbonCommandAction.HfEditFooter, HeaderFooterSlotKind.Footer),
        new(WriterRibbonCommandAction.HfEditEvenHeader, HeaderFooterSlotKind.EvenHeader),
        new(WriterRibbonCommandAction.HfEditEvenFooter, HeaderFooterSlotKind.EvenFooter),
        new(WriterRibbonCommandAction.HfEditFirstHeader, HeaderFooterSlotKind.FirstHeader),
        new(WriterRibbonCommandAction.HfEditFirstFooter, HeaderFooterSlotKind.FirstFooter),
    ];

    public static IReadOnlyList<WriterRibbonHeaderFooterSlotBinding> HeaderFooterNavigationSlots { get; } =
    [
        new(WriterRibbonCommandAction.HfGoToHeader, HeaderFooterSlotKind.Header),
        new(WriterRibbonCommandAction.HfGoToFooter, HeaderFooterSlotKind.Footer),
    ];
}
