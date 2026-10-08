using Writer.Shared.Ribbon;

namespace Writer.App.Presentation.Ribbon;

/// <summary>
/// Shared stateful command adapter for shell-owned toggles whose checked state comes from live host state.
/// </summary>
public sealed class WriterStatefulToggleCommand(
    Action toggle,
    Func<bool> isChecked,
    Action? beforeToggle = null) : IRibbonStatefulCommand
{
    public void Execute(RibbonCommandContext context)
    {
        beforeToggle?.Invoke();
        toggle();
    }

    public RibbonCommandState GetState() => new(IsChecked: isChecked());
}
