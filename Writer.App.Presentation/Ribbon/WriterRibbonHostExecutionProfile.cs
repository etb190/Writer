using Writer.Shared.Ribbon;

namespace Writer.App.Presentation.Ribbon;

public sealed record WriterRibbonHostExecutionCommands(
    IRibbonStatefulCommand ReviewingPane,
    IRibbonStatefulCommand? ShowMarkupBalloons);

/// <summary>
/// Maps shell-owned operations to canonical Writer actions. Editor-context and native control
/// commands remain renderer adapters and can replace these bindings before the final build.
/// </summary>
public static class WriterRibbonHostExecutionProfile
{
    public static WriterRibbonHostExecutionCommands Register(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonHostExecutionPorts ports,
        bool registerFileAdapterCommands)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(ports);

        if (registerFileAdapterCommands)
        {
            bindings.Register("writer.backstage", new ActionRibbonCommand(ports.Backstage));
            bindings.Register("writer.new", new ActionRibbonCommand(ports.NewDocument));
            bindings.Register("writer.open", new ActionRibbonCommand(ports.Open));
            bindings.Register("writer.import-pdf-text", CommandOrUnavailable(ports.ImportPdfText));
            bindings.Register("writer.save", new ActionRibbonCommand(ports.Save));
        }

        bindings.BindAction(WriterRibbonCommandAction.Cut, ports.Cut);
        bindings.BindAction(WriterRibbonCommandAction.Copy, ports.Copy);
        bindings.BindAction(WriterRibbonCommandAction.Paste, ports.Paste);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.PastePlain, ports.PastePlainText);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.PasteMerge, ports.PasteMergeFormatting);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.PasteSpecial, ports.OpenPasteSpecial);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.CharBorder, ports.OpenCharacterBorderDialog);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.CharShading, ports.OpenCharacterShadingDialog);
        bindings.BindAction(WriterRibbonCommandAction.FontDialog, ports.OpenFontDialog);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.ChangeCase, ports.OpenChangeCaseDialog);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.BordersShading, ports.OpenBordersAndShadingDialog);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.TabsDialog, ports.OpenTabsDialog);
        bindings.BindAction(WriterRibbonCommandAction.ParagraphDialog, ports.OpenParagraphDialog);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.NewStyle, ports.OpenNewStyleDialog);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.ManageStyles, ports.OpenManageStylesDialog);

        var find = bindings.BindAction(
            WriterRibbonCommandAction.Find,
            ports.OpenFindReplaceDialog);
        bindings.Bind(WriterRibbonCommandAction.Replace, find);
        bindings.Register("writer.find-replace-dialog", find);

        bindings.BindAction(WriterRibbonCommandAction.Picture, ports.InsertPicture);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.Symbol, ports.OpenSymbolPickerDialog);
        var screenClip = CommandOrUnavailable(ports.CaptureScreenClip);
        bindings.Bind(WriterRibbonCommandAction.ScreenClipping, screenClip);
        bindings.Register("writer.screenshot", screenClip);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.Field, ports.OpenFieldDialog);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.DrawTable, ports.OpenDrawTableDialog);

        var pageSetup = new ActionRibbonCommand(ports.OpenPageSetupDialog);
        bindings.Bind(WriterRibbonCommandAction.PageSetup, pageSetup);
        bindings.Register("writer.page-setup-dialog", pageSetup);
        bindings.BindAction(
            WriterRibbonCommandAction.CustomMargins,
            ports.OpenCustomMarginsDialog ?? ports.OpenPageSetupDialog);
        bindings.BindAction(
            WriterRibbonCommandAction.MorePaperSizes,
            ports.OpenMorePaperSizesDialog ?? ports.OpenPageSetupDialog);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.LineNumbersOptions, ports.OpenLineNumberOptionsDialog);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.HyphenationManual, ports.OpenManualHyphenationDialog);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.HyphenationOptions, ports.OpenHyphenationOptionsDialog);

        ReviewChangeRibbonWorkflow.Register(
            bindings,
            new ReviewChangeRibbonPorts(
                ports.PreviousChange,
                ports.NextChange,
                ports.AcceptThisChange,
                ports.RejectThisChange));
        var statistics = new ActionRibbonCommand(ports.OpenWordCountDialog);
        bindings.Bind(WriterRibbonCommandAction.Statistics, statistics);
        bindings.Register("writer.word-count", statistics);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.Thesaurus, ports.OpenThesaurus);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.CheckAccessibility, ports.CheckAccessibility);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.InspectDocument, ports.InspectDocument);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.Compare, ports.CompareDocuments);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.Combine, ports.CombineDocuments);
        var reviewingPane = bindings.BindToggle(
            WriterRibbonCommandAction.ReviewingPane,
            ports.ToggleReviewingPane,
            ports.IsReviewingPaneVisible ?? (static () => false));
        bindings.Register("writer.reviewingpane", reviewingPane);
        BindOptionalToggle(
            bindings,
            WriterRibbonCommandAction.ShowNotes,
            ports.ToggleNotesPane,
            ports.IsNotesPaneVisible);
        var showMarkupBalloons = BindOptionalToggle(
            bindings,
            WriterRibbonCommandAction.ShowMarkupBalloons,
            ports.ToggleReviewBalloons,
            ports.IsReviewBalloonsActive);
        RegisterSupportCommands(bindings, ports);
        return new WriterRibbonHostExecutionCommands(reviewingPane, showMarkupBalloons);
    }

    public static void RegisterSupportCommands(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonHostExecutionPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(ports);

        BindOrUnavailable(bindings, WriterRibbonCommandAction.HelpOnline, ports.OpenHelpOnline);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.Feedback, ports.OpenFeedback);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.CopyDiagnostics, ports.CopyDiagnostics);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.TestCrashReporting, ports.TestCrashReporting);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.CheckUpdates, ports.CheckForUpdates);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.About, ports.OpenAbout);
        BindOrUnavailable(bindings, WriterRibbonCommandAction.LegalNotices, ports.OpenLegalNotices);
    }

    private static void BindOrUnavailable(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonCommandAction action,
        Action? callback) =>
        bindings.Bind(action, CommandOrUnavailable(callback));

    private static IRibbonStatefulCommand? BindOptionalToggle(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonCommandAction action,
        Action? toggle,
        Func<bool>? isChecked)
    {
        if (toggle is not null && isChecked is not null)
            return bindings.BindToggle(action, toggle, isChecked);

        return null;
    }

    private static IRibbonCommand CommandOrUnavailable(Action? callback) =>
        callback is null
            ? WriterRibbonExecutionProfile.UnavailableCommand
            : new ActionRibbonCommand(callback);
}
