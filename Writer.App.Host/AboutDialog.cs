using Writer.Shared.Shell.Wpf;

namespace Writer.App.Host;

/// <summary>
/// Writer About dialog. Thin wrapper over <see cref="SharedAboutDialog"/> that supplies
/// Writer-specific strings and automation IDs. All structural and interaction logic lives
/// in the shared base so it can be reused across apps without duplication.
/// </summary>
public sealed class AboutDialog : SharedAboutDialog
{
    public AboutDialog()
        : base(WriterAppInfo.AboutPresentation)
    {
    }
}
