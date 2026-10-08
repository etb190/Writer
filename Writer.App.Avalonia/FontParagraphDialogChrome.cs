using Avalonia;
using Avalonia.Controls;
using Writer.Shared.Shell.Avalonia;

namespace Writer.App.Avalonia;

/// <summary>
/// Route-specific checkbox inset for the Font and Paragraph dialog family.
/// Shared compact chrome owns all control templates and state painting.
/// </summary>
internal static class FontParagraphDialogChrome
{
    private const string CheckBoxClass = "writer-font-paragraph-checkbox";

    public static void ApplyCheckBox(CheckBox checkBox, AvaloniaCompactDialogChromeStyle style)
    {
        ArgumentNullException.ThrowIfNull(checkBox);
        ArgumentNullException.ThrowIfNull(style);

        if (checkBox.Classes.Contains(CheckBoxClass))
            return;

        AvaloniaCompactDialogChrome.ApplyCompactCheckBox(checkBox, style, contentSpacing: 5);
        checkBox.Classes.Add(CheckBoxClass);
        checkBox.Margin = new Thickness(
            checkBox.Margin.Left + 1,
            checkBox.Margin.Top,
            checkBox.Margin.Right,
            checkBox.Margin.Bottom);
    }
}
