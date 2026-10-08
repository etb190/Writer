using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Styling;
using Writer.Shared.Shell.Avalonia;
using Writer.App.Presentation.Dialogs;

namespace Writer.App.Avalonia;

/// <summary>Compact modal glyph picker matching Writer's WPF Symbol dialog.</summary>
internal sealed partial class SymbolPickerDialog : WriterDialogWindow
{
    private static readonly IBrush GlyphBackground = new ImmutableSolidColorBrush(Colors.White);
    private static readonly IBrush GlyphBorder = new ImmutableSolidColorBrush(Color.FromRgb(200, 200, 200));
    private static readonly IBrush GlyphHoverBackground = new ImmutableSolidColorBrush(Color.FromRgb(229, 243, 255));
    private static readonly IBrush GlyphHoverBorder = new ImmutableSolidColorBrush(Color.FromRgb(0, 120, 215));
    private static readonly IBrush GlyphPressedBackground = new ImmutableSolidColorBrush(Color.FromRgb(204, 232, 255));
    private static readonly FuncControlTemplate<Button> GlyphButtonTemplate = new((button, _) =>
    {
        var presenter = new ContentPresenter();
        presenter.Bind(ContentPresenter.ContentProperty, new Binding(nameof(ContentControl.Content)) { Source = button });
        presenter.Bind(ContentPresenter.ContentTemplateProperty, new Binding(nameof(ContentControl.ContentTemplate)) { Source = button });
        presenter.Bind(Layoutable.HorizontalAlignmentProperty, new Binding(nameof(ContentControl.HorizontalContentAlignment)) { Source = button });
        presenter.Bind(Layoutable.VerticalAlignmentProperty, new Binding(nameof(ContentControl.VerticalContentAlignment)) { Source = button });

        var border = new Border { CornerRadius = new CornerRadius(1), Child = presenter };
        border.Bind(Border.BackgroundProperty, new Binding(nameof(TemplatedControl.Background)) { Source = button });
        border.Bind(Border.BorderBrushProperty, new Binding(nameof(TemplatedControl.BorderBrush)) { Source = button });
        border.Bind(Border.BorderThicknessProperty, new Binding(nameof(TemplatedControl.BorderThickness)) { Source = button });
        border.Bind(Border.PaddingProperty, new Binding(nameof(TemplatedControl.Padding)) { Source = button });
        return border;
    });

    private readonly List<Button> _glyphButtons = [];

    public string? Result { get; private set; }

    public SymbolPickerDialog()
    {
        Title = WriterSymbolPickerDialogPlanner.Title;
        SizeToContent = SizeToContent.WidthAndHeight;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        CanResize = false;
        ShowInTaskbar = false;
        Focusable = true;
        AutomationProperties.SetAutomationId(this, WriterSymbolPickerDialogPlanner.DialogAutomationId);

        var panel = new StackPanel { Margin = new Thickness(WriterSymbolPickerDialogPlanner.OuterMargin) };
        var grid = new UniformGrid { Columns = WriterSymbolPickerDialogPlanner.Columns };
        foreach (var glyph in WriterSymbolPickerDialogPlanner.Glyphs)
        {
            var semantic = WriterSymbolPickerDialogPlanner.BuildSemantic(glyph);
            var button = new Button
            {
                Content = glyph,
                MinWidth = WriterSymbolPickerDialogPlanner.ButtonSize,
                Height = WriterSymbolPickerDialogPlanner.ButtonSize,
                FontSize = WriterSymbolPickerDialogPlanner.ButtonFontSize,
                Margin = new Thickness(WriterSymbolPickerDialogPlanner.ButtonMargin),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            ToolTip.SetTip(button, semantic.CodePointLabel);
            AutomationProperties.SetName(button, semantic.AutomationName);
            AutomationProperties.SetAutomationId(button, semantic.AutomationId);
            button.Click += (_, _) => SelectGlyph(glyph, close: true);
            _glyphButtons.Add(button);
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
            Padding = new Thickness(8, 2),
        };
        AutomationProperties.SetAutomationId(cancel, WriterSymbolPickerDialogPlanner.CancelAutomationId);
        cancel.Click += (_, _) => Close();
        panel.Children.Add(cancel);

        Content = panel;
        Opened += (_, _) =>
        {
            ApplyGlyphButtonChrome(grid);
            Focus();
        };
    }

    private void SelectGlyph(string glyph, bool close)
    {
        if (!WriterSymbolPickerDialogPlanner.Glyphs.Contains(glyph, StringComparer.Ordinal))
            throw new ArgumentOutOfRangeException(nameof(glyph));

        Result = glyph;
        if (close)
            Close();
    }

    private static void ApplyGlyphButtonChrome(UniformGrid grid)
    {
        foreach (var button in grid.Children.OfType<Button>())
        {
            // Shared dialog chrome normalizes generic buttons after Opened; restore the WPF tile
            // metrics here while retaining that chrome for the dialog and footer.
            button.MinWidth = WriterSymbolPickerDialogPlanner.ButtonSize;
            button.Height = WriterSymbolPickerDialogPlanner.ButtonSize;
            button.MinHeight = WriterSymbolPickerDialogPlanner.ButtonSize;
            button.MaxHeight = WriterSymbolPickerDialogPlanner.ButtonSize;
            button.Padding = new Thickness(0);
            button.FontSize = WriterSymbolPickerDialogPlanner.ButtonFontSize;
            button.HorizontalAlignment = HorizontalAlignment.Stretch;
            button.VerticalAlignment = VerticalAlignment.Stretch;
            button.Background = GlyphBackground;
            button.BorderBrush = GlyphBorder;
            button.BorderThickness = new Thickness(1);
            button.Template = GlyphButtonTemplate;
        }

        grid.Styles.Add(new Style(x => x.OfType<Button>().Class(":pointerover"))
        {
            Setters =
            {
                new Setter(TemplatedControl.BackgroundProperty, GlyphHoverBackground),
                new Setter(TemplatedControl.BorderBrushProperty, GlyphHoverBorder),
            },
        });
        grid.Styles.Add(new Style(x => x.OfType<Button>().Class(":focus"))
        {
            Setters =
            {
                new Setter(TemplatedControl.BorderBrushProperty, GlyphHoverBorder),
                new Setter(TemplatedControl.BorderThicknessProperty, new Thickness(2)),
            },
        });
        grid.Styles.Add(new Style(x => x.OfType<Button>().Class(":pressed"))
        {
            Setters =
            {
                new Setter(TemplatedControl.BackgroundProperty, GlyphPressedBackground),
                new Setter(TemplatedControl.BorderBrushProperty, GlyphHoverBorder),
            },
        });
    }
}
