using Writer.Shared.Ribbon;

namespace Writer.App.Presentation.Ribbon;

public sealed record MailMergeRibbonBindings(
    IRibbonCommand Envelopes,
    IRibbonCommand Labels,
    IRibbonCommand StartLetters,
    IRibbonCommand StartDirectory,
    IRibbonCommand StartNormalDocument,
    IRibbonCommand SelectRecipients,
    IRibbonCommand InsertMergeField,
    IRibbonCommand InsertAddressBlock,
    IRibbonCommand InsertGreetingLine,
    IRibbonCommand MatchFields,
    IRibbonCommand FilterSortRecipients,
    Func<MailMergeRuleKind, IRibbonCommand> CreateRuleCommand,
    IRibbonCommand InsertNextRecordField,
    IRibbonCommand InsertMergeRecordNumberField,
    IRibbonCommand InsertMergeSequenceNumberField,
    IRibbonCommand TogglePreview,
    IRibbonCommand FirstRecord,
    IRibbonCommand PreviousRecord,
    IRibbonCommand NextRecord,
    IRibbonCommand LastRecord,
    IRibbonCommand FinishMerge,
    IRibbonCommand SendEmail,
    IRibbonCommand? FindRecipient = null,
    IRibbonCommand? CheckErrors = null);

/// <summary>
/// Owns Mailings-tab command identity, aliases, rule-kind mapping, and unavailable-route policy.
/// Renderers provide only concrete mail-merge engine, dialog, document, print, and delivery adapters.
/// </summary>
public static class MailMergeRibbonWorkflow
{
    public static IReadOnlyList<WriterRibbonCommandAction> Actions { get; } =
    [
        WriterRibbonCommandAction.MergeEnvelopes,
        WriterRibbonCommandAction.MergeLabels,
        WriterRibbonCommandAction.StartMailMerge,
        WriterRibbonCommandAction.StartMailMergeLetters,
        WriterRibbonCommandAction.StartMailMergeDirectory,
        WriterRibbonCommandAction.StartMailMergeNormal,
        WriterRibbonCommandAction.MergeData,
        WriterRibbonCommandAction.MergeEditRecipients,
        WriterRibbonCommandAction.MergeField,
        WriterRibbonCommandAction.MergeAddressBlock,
        WriterRibbonCommandAction.MergeGreetingLine,
        WriterRibbonCommandAction.MergeMatchFields,
        WriterRibbonCommandAction.MergeFilterSort,
        WriterRibbonCommandAction.MergeRules,
        WriterRibbonCommandAction.MergeRuleIf,
        WriterRibbonCommandAction.MergeRuleSkipRecordIf,
        WriterRibbonCommandAction.MergeRuleNextRecordIf,
        WriterRibbonCommandAction.MergeNextRecord,
        WriterRibbonCommandAction.MergeRecordNumber,
        WriterRibbonCommandAction.MergeSequenceNumber,
        WriterRibbonCommandAction.MergeRuleFillIn,
        WriterRibbonCommandAction.MergeRuleAsk,
        WriterRibbonCommandAction.MergeRuleSet,
        WriterRibbonCommandAction.MergeRuleRef,
        WriterRibbonCommandAction.MergePreview,
        WriterRibbonCommandAction.MergePreviewFirst,
        WriterRibbonCommandAction.MergePreviewPrevious,
        WriterRibbonCommandAction.MergePreviewNext,
        WriterRibbonCommandAction.MergePreviewLast,
        WriterRibbonCommandAction.MergeFindRecipient,
        WriterRibbonCommandAction.MergeCheckErrors,
        WriterRibbonCommandAction.MergeFinish,
        WriterRibbonCommandAction.MergeEmail,
    ];

    public static void Register(
        IRibbonCommandRegistry registry,
        MailMergeRibbonBindings bindings)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(bindings.CreateRuleCommand);

        Bind(WriterRibbonCommandAction.MergeEnvelopes, bindings.Envelopes);
        Bind(WriterRibbonCommandAction.MergeLabels, bindings.Labels);
        Bind(WriterRibbonCommandAction.StartMailMerge, bindings.StartLetters);
        Bind(WriterRibbonCommandAction.StartMailMergeLetters, bindings.StartLetters);
        Bind(WriterRibbonCommandAction.StartMailMergeDirectory, bindings.StartDirectory);
        Bind(WriterRibbonCommandAction.StartMailMergeNormal, bindings.StartNormalDocument);
        BindWithAliases(
            WriterRibbonCommandAction.MergeData,
            bindings.SelectRecipients,
            "writer.select-recipients");
        Bind(WriterRibbonCommandAction.MergeEditRecipients, bindings.SelectRecipients);
        Bind(WriterRibbonCommandAction.MergeField, bindings.InsertMergeField);
        BindWithAliases(
            WriterRibbonCommandAction.MergeAddressBlock,
            bindings.InsertAddressBlock,
            "writer.address-block");
        BindWithAliases(
            WriterRibbonCommandAction.MergeGreetingLine,
            bindings.InsertGreetingLine,
            "writer.greeting-line");
        Bind(WriterRibbonCommandAction.MergeMatchFields, bindings.MatchFields);
        Bind(WriterRibbonCommandAction.MergeFilterSort, bindings.FilterSortRecipients);
        Bind(WriterRibbonCommandAction.MergeRules, EmptyRibbonCommand.Instance);
        BindRule(WriterRibbonCommandAction.MergeRuleIf, MailMergeRuleKind.IfThenElse);
        BindRule(WriterRibbonCommandAction.MergeRuleSkipRecordIf, MailMergeRuleKind.SkipRecordIf);
        BindRule(WriterRibbonCommandAction.MergeRuleNextRecordIf, MailMergeRuleKind.NextRecordIf);
        Bind(WriterRibbonCommandAction.MergeNextRecord, bindings.InsertNextRecordField);
        Bind(WriterRibbonCommandAction.MergeRecordNumber, bindings.InsertMergeRecordNumberField);
        Bind(WriterRibbonCommandAction.MergeSequenceNumber, bindings.InsertMergeSequenceNumberField);
        BindRule(WriterRibbonCommandAction.MergeRuleFillIn, MailMergeRuleKind.FillIn);
        BindRule(WriterRibbonCommandAction.MergeRuleAsk, MailMergeRuleKind.Ask);
        BindRule(WriterRibbonCommandAction.MergeRuleSet, MailMergeRuleKind.Set);
        BindRule(WriterRibbonCommandAction.MergeRuleRef, MailMergeRuleKind.Ref);
        BindWithAliases(
            WriterRibbonCommandAction.MergePreview,
            bindings.TogglePreview,
            "writer.preview-results");
        Bind(WriterRibbonCommandAction.MergePreviewFirst, bindings.FirstRecord);
        BindWithAliases(
            WriterRibbonCommandAction.MergePreviewPrevious,
            bindings.PreviousRecord,
            "writer.prev-record");
        BindWithAliases(
            WriterRibbonCommandAction.MergePreviewNext,
            bindings.NextRecord,
            "writer.next-record");
        Bind(WriterRibbonCommandAction.MergePreviewLast, bindings.LastRecord);
        Bind(
            WriterRibbonCommandAction.MergeFindRecipient,
            bindings.FindRecipient ?? WriterRibbonExecutionProfile.UnavailableCommand);
        Bind(
            WriterRibbonCommandAction.MergeCheckErrors,
            bindings.CheckErrors ?? WriterRibbonExecutionProfile.UnavailableCommand);
        BindWithAliases(
            WriterRibbonCommandAction.MergeFinish,
            bindings.FinishMerge,
            "writer.finish-merge");
        Bind(WriterRibbonCommandAction.MergeEmail, bindings.SendEmail);

        void Bind(WriterRibbonCommandAction action, IRibbonCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);
            registry.Bind(action, command);
        }

        void BindWithAliases(
            WriterRibbonCommandAction action,
            IRibbonCommand command,
            params string[] aliases)
        {
            Bind(action, command);
            foreach (var alias in aliases)
                registry.Register(alias, command);
        }

        void BindRule(WriterRibbonCommandAction action, MailMergeRuleKind kind) =>
            Bind(action, bindings.CreateRuleCommand(kind));
    }
}
