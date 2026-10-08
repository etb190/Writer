using Writer.Shared.Shell;
using Writer.Shared.Shell.Avalonia;
using Writer.App.Presentation;

namespace Writer.App.Avalonia;

internal sealed class LegalNoticesDialog : AvaloniaLegalNoticesDialog
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
