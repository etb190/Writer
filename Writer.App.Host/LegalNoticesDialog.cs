using Writer.Shared.Shell.Wpf;
using Writer.Shared.Shell;
using Writer.App.Presentation;

namespace Writer.App.Host;

/// <summary>
/// Writer Legal Notices dialog. Thin wrapper over <see cref="SharedLegalNoticesDialog"/> that
/// composes the Writer presentation with the WPF renderer.
/// </summary>
public sealed partial class LegalNoticesDialog : SharedLegalNoticesDialog
{
    public LegalNoticesDialog()
        : this(WriterLegalNoticeProvider.GetDocuments(typeof(LegalNoticesDialog).Assembly))
    {
    }

    internal LegalNoticesDialog(IReadOnlyList<LegalNoticeDocument> notices)
        : base(WriterLegalNoticesPresentation.Create(notices))
    {
    }

    internal LegalNoticesDialog(IReadOnlyList<(string Title, string Text)> notices)
        : base(WriterLegalNoticesPresentation.Create(notices))
    {
    }
}
