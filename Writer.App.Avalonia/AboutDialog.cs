using Writer.Shared.Shell.Avalonia;
using Writer.App.Presentation;

namespace Writer.App.Avalonia;

internal sealed class AboutDialog : AvaloniaAboutDialog
{
    public AboutDialog()
        : base(WriterAboutDialogPresentation.Create(typeof(AboutDialog).Assembly))
    {
    }
}
