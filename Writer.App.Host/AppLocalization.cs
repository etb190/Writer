using Writer.App.Localization;
using Writer.Shared.Shell;

namespace Writer.App.Host;

internal static class AppLocalization
{
    public static readonly WpfAppLocalizationBootstrap Bootstrap = new(
        UiText.Get,
        UiText.Format,
        AppLanguageCatalog.ResolveCulture);
}
