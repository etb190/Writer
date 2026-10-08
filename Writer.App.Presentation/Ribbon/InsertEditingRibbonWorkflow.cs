using Writer.Shared.Ribbon;

namespace Writer.App.Presentation.Ribbon;

public sealed record InsertEditingRibbonPorts(
    IRibbonCommand Hyperlink,
    IRibbonCommand EditHyperlink,
    IRibbonCommand RemoveHyperlink,
    IRibbonCommand HyperlinkTooltip,
    IRibbonCommand Bookmark,
    IRibbonCommand LinkBookmark,
    IRibbonCommand BookmarkManager,
    Action PrepareContentControlInsertion,
    Action InsertPlainTextControl,
    Action InsertRichTextControl,
    Action InsertCheckBoxControl,
    Action InsertDatePickerControl,
    Action InsertDropDownListControl,
    Action InsertComboBoxControl,
    Action UpdateFields,
    Action ToggleFieldCodes);

/// <summary>
/// Owns the portable command identity and mutation ordering for Insert links/bookmarks,
/// Developer content controls, and field maintenance. Renderers retain only native dialogs
/// and editor-effect adapters.
/// </summary>
public static class InsertEditingRibbonWorkflow
{
    public static void Register(
        IRibbonCommandRegistry registry,
        InsertEditingRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(ports);

        registry.Bind(WriterRibbonCommandAction.Hyperlink, ports.Hyperlink);
        registry.Register("writer.insert-hyperlink", ports.Hyperlink);
        registry.Bind(WriterRibbonCommandAction.EditHyperlink, ports.EditHyperlink);
        registry.Bind(WriterRibbonCommandAction.RemoveHyperlink, ports.RemoveHyperlink);
        registry.Bind(WriterRibbonCommandAction.HyperlinkTooltip, ports.HyperlinkTooltip);

        registry.Bind(WriterRibbonCommandAction.Bookmark, ports.Bookmark);
        registry.Register("writer.insert-bookmark", ports.Bookmark);
        registry.Bind(WriterRibbonCommandAction.LinkBookmark, ports.LinkBookmark);
        registry.Bind(WriterRibbonCommandAction.BookmarkManager, ports.BookmarkManager);

        BindPrepared(WriterRibbonCommandAction.CcText, ports.InsertPlainTextControl);
        BindPrepared(WriterRibbonCommandAction.CcRichtext, ports.InsertRichTextControl);
        BindPrepared(WriterRibbonCommandAction.CcCheckbox, ports.InsertCheckBoxControl);
        BindPrepared(WriterRibbonCommandAction.CcDate, ports.InsertDatePickerControl);
        BindPrepared(WriterRibbonCommandAction.CcDropdown, ports.InsertDropDownListControl);
        BindPrepared(WriterRibbonCommandAction.CcCombo, ports.InsertComboBoxControl);

        registry.Bind(WriterRibbonCommandAction.UpdateFields, new ActionRibbonCommand(ports.UpdateFields));
        registry.Bind(WriterRibbonCommandAction.ToggleFieldCodes, new ActionRibbonCommand(ports.ToggleFieldCodes));

        void BindPrepared(WriterRibbonCommandAction action, Action execute) =>
            registry.Bind(
                action,
                new PreparedActionCommand(ports.PrepareContentControlInsertion, execute));
    }

    private sealed class PreparedActionCommand(Action prepare, Action execute) : IRibbonCommand
    {
        public void Execute(RibbonCommandContext context)
        {
            prepare();
            execute();
        }
    }
}
