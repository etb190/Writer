using Writer.Shared.Shell.Avalonia;

namespace Writer.App.Avalonia;

/// <summary>Writer dialog base that keeps every code-built route on the shared Avalonia chrome.</summary>
public abstract class WriterDialogWindow : AvaloniaDialogWindow
{
    protected WriterDialogWindow()
    {
    }

    protected WriterDialogWindow(AvaloniaCompactDialogChromeStyle style)
        : base(style)
    {
    }
}
