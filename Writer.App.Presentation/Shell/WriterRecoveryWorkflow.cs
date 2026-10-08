using Writer.Shared.AppServices;

namespace Writer.App.Presentation.Shell;

public enum WriterRecoveryPromptMode
{
    Startup,
    StartupQuotedDisplayName,
    Manual,
}

public sealed record WriterRecoveryOffer(
    AutosaveRecoveryPlan Recovery,
    int RemainingCount,
    WriterRecoveryPromptMode PromptMode)
{
    private static readonly AutosaveRecoveryPromptText PromptText =
        new("Writer", "documents");

    public string Prompt => AutosaveRecoveryPromptFormatter.Format(
        Recovery.DisplayName,
        RemainingCount,
        WriterRecoveryWorkflow.MapPromptMode(PromptMode),
        PromptText);
}

public readonly record struct WriterRecoveryWorkflowResult(
    bool AnyAccepted,
    bool AnyRecovered);

/// <summary>Writer compatibility facade over the shared recovery sequencer.</summary>
public static class WriterRecoveryWorkflow
{
    public static async ValueTask<WriterRecoveryWorkflowResult> RunAsync(
        IReadOnlyList<AutosaveRecoveryPlan> recoveries,
        WriterRecoveryPromptMode promptMode,
        Func<WriterRecoveryOffer, ValueTask<bool>> promptAsync,
        Func<AutosaveRecoveryPlan, bool, ValueTask<bool>> completeRecoveryAsync)
    {
        var result = await AutosaveRecoveryWorkflow.RunAsync(
            recoveries,
            MapPromptMode(promptMode),
            (recovery, remainingCount) =>
                new WriterRecoveryOffer(recovery, remainingCount, promptMode),
            promptAsync,
            completeRecoveryAsync);

        return new WriterRecoveryWorkflowResult(result.AnyAccepted, result.AnyRecovered);
    }

    internal static AutosaveRecoveryPromptMode MapPromptMode(
        WriterRecoveryPromptMode promptMode) =>
        (AutosaveRecoveryPromptMode)(int)promptMode;
}
