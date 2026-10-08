using Writer.Shared.Ribbon;

namespace Writer.App.Presentation.Ribbon;

/// <summary>
/// Completes renderer-provided native command ports against the canonical Writer action catalog.
/// Presentation owns route coverage and fallback state; renderers own only concrete editor, dialog,
/// focus, and control adapters.
/// </summary>
public static class WriterRibbonExecutionProfile
{
    public static IRibbonCommand UnavailableCommand => UnavailableCommandImpl.Instance;

    public static WriterRibbonCommandBuildResult Build(WriterRibbonCommandBindingPorts ports)
    {
        ArgumentNullException.ThrowIfNull(ports);

        var completed = new WriterRibbonCommandBindingPorts();
        foreach (var action in Enum.GetValues<WriterRibbonCommandAction>())
        {
            completed.Bind(
                action,
                ports.CanonicalBindings.TryGetValue(action, out var command)
                    ? command
                    : UnavailableCommandImpl.Instance);
        }

        foreach (var (commandId, command) in ports.AdapterBindings)
            completed.Register(commandId, command);

        return WriterRibbonCommandWorkflow.Build(completed);
    }

    private sealed class UnavailableCommandImpl : IRibbonStatefulCommand
    {
        public static UnavailableCommandImpl Instance { get; } = new();

        private UnavailableCommandImpl()
        {
        }

        public void Execute(RibbonCommandContext context)
        {
        }

        public RibbonCommandState GetState() => new(IsEnabled: false);
    }
}
