using System.Windows;
using Writer.App.Host.Editing;

namespace Writer.App.Host;

/// <summary>
/// WPF-only ribbon seams that cannot cross the renderer-neutral host execution boundary.
/// </summary>
internal sealed record WriterWpfRibbonNativeExecutionPorts(
    Func<bool, string, string?>? AskHeaderFooterText = null,
    Func<DocumentView>? ResolveFieldEditor = null,
    Func<Window?, string?>? AskFieldInstruction = null)
{
    public static WriterWpfRibbonNativeExecutionPorts Empty { get; } = new();
}
