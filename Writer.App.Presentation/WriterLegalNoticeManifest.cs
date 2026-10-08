using System.Reflection;
using Writer.Shared.Shell;

namespace Writer.App.Presentation;

/// <summary>Canonical embedded-resource manifest for every Writer renderer.</summary>
public static class WriterLegalNoticeManifest
{
    public static IReadOnlyList<LegalNoticeResource> Resources { get; } =
        Array.AsReadOnly<LegalNoticeResource>([]);
}

/// <summary>Loads Writer's app-owned legal manifest for either desktop renderer.</summary>
public static class WriterLegalNoticeProvider
{
    public static IReadOnlyList<LegalNoticeDocument> GetDocuments(Assembly assembly) =>
        EmbeddedLegalNoticeLoader.GetDocuments(assembly, WriterLegalNoticeManifest.Resources);
}
