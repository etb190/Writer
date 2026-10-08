using Writer.Shared.Localization;

namespace Writer.App.Localization;

/// <summary>
/// Writer UI text facade for shells that need common dialog and shared backstage strings.
/// </summary>
public abstract class LocalizedUiText : LocalizedUiTextCatalog<Loc>
{
    protected LocalizedUiText()
    {
    }
}
