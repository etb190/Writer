using Writer.App.Presentation.Documents;
using Writer.Core.Model;

namespace Writer.App.Avalonia;

/// <summary>Supplies the portable starter document to Avalonia startup and packaging smoke tests.</summary>
internal static class SampleDocument
{
    public static TextDocument Create() =>
        WriterSampleDocumentFactory.Create(WriterSampleDocumentProfile.FeatureShowcase);
}
