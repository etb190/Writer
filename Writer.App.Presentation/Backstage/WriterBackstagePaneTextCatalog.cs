using Writer.Shared.AppServices;
using Writer.Shared.Shell;

namespace Writer.App.Presentation.Backstage;

public static class WriterBackstagePaneResourceKeys
{
    public const string RecentEmptyText = "Writer_Backstage_Recent_EmptyText";
    public const string TemplateHeading = "Common_New";
    public const string TemplateTileCaption = "Writer_Backstage_New_BlankDocument";
    public const string TemplateFooterText = "Writer_Backstage_New_FooterText";
    public const string OptionsDescription = "Writer_Backstage_Options_Description";
    public const string OptionsEditText = "Writer_Backstage_Options_EditText";
    public const string ExportHeading = "Writer_Backstage_Export_Heading";
    public const string ExportDescription = "Writer_Backstage_Export_Description";
    public const string ExportFixedLayoutGroupHeading = "Writer_Backstage_Export_FixedLayoutGroupHeading";
    public const string ExportPdfActionLabel = "Writer_Backstage_Export_PdfActionLabel";
    public const string ExportPdfActionDescription = "Writer_Backstage_Export_PdfActionDescription";
    public const string ExportXpsActionLabel = "Writer_Backstage_Export_XpsActionLabel";
    public const string ExportXpsActionDescription = "Writer_Backstage_Export_XpsActionDescription";
    public const string InfoHeading = "Writer_Backstage_Info_Heading";
}

/// <summary>Owns Writer-specific Backstage resource keys and fallback copy.</summary>
public static class WriterBackstagePaneTextCatalog
{
    public static SisterBackstagePaneTextDescriptor Descriptor { get; } = new(
        Text(WriterBackstagePaneResourceKeys.RecentEmptyText, "No recent documents."),
        Text(WriterBackstagePaneResourceKeys.TemplateHeading, "New"),
        Text(WriterBackstagePaneResourceKeys.TemplateTileCaption, "Blank document"),
        Text(WriterBackstagePaneResourceKeys.TemplateFooterText, "More templates are not available in this build."),
        Text(WriterBackstagePaneResourceKeys.OptionsDescription, "Writer application settings. These persist between sessions and apply immediately."),
        new SisterBackstageExportPaneTextDescriptor(
            Text(WriterBackstagePaneResourceKeys.ExportHeading, "Export"),
            Text(WriterBackstagePaneResourceKeys.ExportDescription, "Create a fixed-layout copy or choose an editable document format."),
            Text(WriterBackstagePaneResourceKeys.ExportFixedLayoutGroupHeading, "Create PDF/XPS Document"),
            Text(WriterBackstagePaneResourceKeys.ExportPdfActionLabel, "Create PDF or XPS"),
            Text(WriterBackstagePaneResourceKeys.ExportPdfActionDescription, "Publish a fixed-layout copy for sharing or printing."),
            Text(WriterBackstagePaneResourceKeys.ExportXpsActionLabel, "Export to XPS"),
            Text(WriterBackstagePaneResourceKeys.ExportXpsActionDescription, "Publish an XPS document with selectable, searchable vector text.")),
        Text(WriterBackstagePaneResourceKeys.OptionsEditText, "Edit options\u2026"),
        Info: SisterBackstagePaneTextResources.CreateInfoDescriptor(
            Text(WriterBackstagePaneResourceKeys.InfoHeading, "Document information")),
        OptionsSummary: SisterBackstagePaneTextResources.ApplicationOptionsSummaryDescriptor);

    public static IReadOnlyList<string> RequiredResourceKeys => Descriptor.ResourceKeys;

    public static SisterBackstagePaneTextSpec BuildTextSpec(Func<string, string?>? getText = null) =>
        SisterBackstagePaneTextSpec.FromDescriptor(Descriptor, getText);

    private static ResourceTextDescriptor Text(string key, string fallbackText) => new(key, fallbackText);
}
