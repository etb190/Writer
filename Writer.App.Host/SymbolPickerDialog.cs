using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Writer.App.Presentation.Dialogs;

namespace Writer.App.Host;

/// <summary>
/// A tiny modal picker showing a grid of common glyphs (symbols, punctuation, currency, Greek/math).
/// Clicking a glyph closes the dialog and returns it; the caller inserts it at the caret as plain text.
/// Returns the chosen glyph, or null if the user cancels.
/// </summary>
internal sealed class SymbolPickerDialog : Writer.Shared.Ribbon.Wpf.DialogWindow
{
    private string? _result;

    internal SymbolPickerDialog(Window? owner)
    {
        Owner = owner;
        Title = WriterSymbolPickerDialogPlanner.Title;
        SizeToContent = SizeToContent.WidthAndHeight;
        WindowStartupLocation = owner is null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        AutomationProperties.SetAutomationId(this, WriterSymbolPickerDialogPlanner.DialogAutomationId);

        var panel = new StackPanel { Margin = new Thickness(WriterSymbolPickerDialogPlanner.OuterMargin) };
        var grid = new UniformGrid { Columns = WriterSymbolPickerDialogPlanner.Columns };
        foreach (var glyph in WriterSymbolPickerDialogPlanner.Glyphs)
        {
            var semantic = WriterSymbolPickerDialogPlanner.BuildSemantic(glyph);
            var button = new Button
            {
                Content = glyph,
                Width = WriterSymbolPickerDialogPlanner.ButtonSize,
                Height = WriterSymbolPickerDialogPlanner.ButtonSize,
                FontSize = WriterSymbolPickerDialogPlanner.ButtonFontSize,
                Margin = new Thickness(WriterSymbolPickerDialogPlanner.ButtonMargin),
                ToolTip = semantic.CodePointLabel,
            };
            AutomationProperties.SetName(button, semantic.AutomationName);
            AutomationProperties.SetAutomationId(button, semantic.AutomationId);
            button.Click += (_, _) => { _result = glyph; DialogResult = true; };
            grid.Children.Add(button);
        }
        panel.Children.Add(grid);

        var cancel = new Button
        {
            Content = WriterSymbolPickerDialogPlanner.CancelText,
            IsCancel = true,
            MinWidth = WriterSymbolPickerDialogPlanner.FooterButtonMinWidth,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(
                WriterSymbolPickerDialogPlanner.ButtonMargin,
                WriterSymbolPickerDialogPlanner.FooterTopMargin,
                WriterSymbolPickerDialogPlanner.ButtonMargin,
                0),
            Padding = new Thickness(8, 2, 8, 2),
        };
        AutomationProperties.SetAutomationId(cancel, WriterSymbolPickerDialogPlanner.CancelAutomationId);
        panel.Children.Add(cancel);

        Content = panel;
    }

    /// <summary>Show the picker; returns the chosen glyph, or null if cancelled.</summary>
    public static string? Prompt(Window? owner)
    {
        var dialog = new SymbolPickerDialog(owner);
        return dialog.ShowDialog() == true ? dialog._result : null;
    }
}
