namespace Writer.Shared.Ribbon.Wpf;

/// <summary>
/// Compatibility wrapper for callers that still import the historical ribbon namespace.
/// Dialog chrome and resources live in <see cref="Writer.Shared.Shell.Wpf.DialogWindow"/>.
/// </summary>
public abstract class DialogWindow : Writer.Shared.Shell.Wpf.DialogWindow
{
}
