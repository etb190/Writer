using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Styling;
using Writer.Shared.AppServices;
using Writer.Shared.Theme;
using Writer.Shared.Theme.Avalonia;
using Writer.Shared.Shell.Avalonia;
using Writer.App.Localization;
using Writer.App.Presentation.Options;
using Writer.App.Presentation.Shell;

namespace Writer.App.Avalonia;

public sealed partial class App : Application
{
    internal static Theme ActiveTheme { get; private set; } = WriterApplicationStartup.Theme.DefaultTheme;

    internal static SisterAvaloniaStandardDesktopProfile<App, MainWindow, WriterOptions> DesktopProfile { get; } =
        new(
            WriterApplicationStartup.ProductIdentity,
            new SisterAvaloniaLocalizationStartupDescriptor(
                () => AvaloniaAppLocalizationBootstrap.InstallSharedSeams(
                    UiText.Get,
                    UiText.Format,
                    UiText.CreateAutomationName),
                // r189: this shell offers a UI-language field with a restart notice; without this
                // the restart changed nothing. FreeX had the same bug and fixing it there alone
                // left it here, so the applier is wired through the shared profile for every
                // sister Avalonia app rather than in one App.cs.
                uiLanguage => AvaloniaAppLocalizationBootstrap.ApplyAppLanguage(
                    uiLanguage,
                    name => AppLanguageCatalog.ResolveCulture(name, CultureInfo.CurrentUICulture),
                    CultureInfo.CurrentUICulture)),
            new SisterAvaloniaThemeStartupDescriptor<Theme>(
                WriterApplicationStartup.Theme,
                theme => ActiveTheme = theme,
                (application, theme, resourceKeyPrefix) =>
                    application.Resources.MergedDictionaries.Add(
                        AvaloniaThemeApplier.BuildResources(theme, resourceKeyPrefix))),
            // R169: must resolve to the SAME settings.json path as the WPF host
            // (Writer.App.Host/Program.cs, which never overrides OptionsPathProvider and so falls
            // back to PlatformApplicationDataPathProvider.Instance -- %APPDATA%, not %LOCALAPPDATA%).
            // This used to pass LocalInstance here, so a user who ran both shells on one Windows
            // machine had every preference silently revert to defaults depending on which shell
            // they launched. Passing no provider takes the same Instance default as WPF, and
            // MigrateLegacyLocalSettings (below) recovers a settings.json a pre-fix build already
            // wrote under the old (LocalInstance) path before this store loads.
            new SisterAvaloniaOptionsStartupDescriptor<WriterOptions>(
                () =>
                {
                    MigrateLegacyLocalSettings();
                    return ApplicationOptionsStore<WriterOptions>.Create();
                }),
            new SisterAvaloniaWindowStartupDescriptor<MainWindow, WriterOptions>(
                (startupArguments, options, optionsStore) =>
                    new MainWindow(startupArguments, options, optionsStore)),
            onEmergencySnapshot: AutosaveAdapter.TryEmergencySnapshots);

    public override void OnFrameworkInitializationCompleted()
    {
        SisterAvaloniaStandardDesktopFactory.Initialize(this, DesktopProfile);
        AddMenuChromeStyles();
        AddComboChromeResources();

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Pins every menu/flyout popup to the Office-style light chrome the rest of this shell
    /// paints: black ink on a white surface, gray accelerators/chevrons, light hover tints.
    /// <para>
    /// Fluent resolves all menu ink from the OS color-scheme variant (ThemeVariant.Default), so
    /// under an OS dark scheme the dropdown menus render white item text over the light surfaces
    /// the shell projects behind its popups — the home-tab Font / Paragraph / Styles / Editing
    /// dropdowns then show text that is invisible against its own background, and submenus and
    /// context menus inherit the same mismatch. App-level styles are applied after the
    /// FluentTheme ControlTheme styles, so every setter here wins in the state it targets.
    /// </para>
    /// </summary>
    private static void AddMenuChromeStyles()
    {
        var hoverBrush = new ImmutableSolidColorBrush(Color.FromRgb(0xE8, 0xEC, 0xF1));
        var pressedBrush = new ImmutableSolidColorBrush(Color.FromRgb(0xD9, 0xE2, 0xEC));
        var popupBorderBrush = new ImmutableSolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0));
        var acceleratorBrush = new ImmutableSolidColorBrush(Color.FromRgb(0x76, 0x76, 0x76));
        var acceleratorFocusBrush = new ImmutableSolidColorBrush(Color.FromRgb(0x4A, 0x4A, 0x4A));
        var chevronBrush = new ImmutableSolidColorBrush(Color.FromRgb(0x5A, 0x5A, 0x5A));
        var chevronFocusBrush = new ImmutableSolidColorBrush(Color.FromRgb(0x30, 0x30, 0x30));
        var disabledBrush = new ImmutableSolidColorBrush(Color.FromRgb(0x9A, 0x9A, 0x9A));

        var flyoutPresenter = new Style(s => s.OfType<MenuFlyoutPresenter>())
        {
            Setters =
            {
                new Setter(TemplatedControl.BackgroundProperty, Brushes.White),
                new Setter(TemplatedControl.BorderBrushProperty, popupBorderBrush),
            },
        };

        // Base row: white fill with black ink. Header text, check marks, and radio glyphs all
        // resolve through MenuItem.Foreground.
        var itemBase = new Style(s => s.OfType<MenuItem>())
        {
            Setters =
            {
                new Setter(TemplatedControl.BackgroundProperty, Brushes.White),
                new Setter(TemplatedControl.ForegroundProperty, Brushes.Black),
            },
        };
        // The MenuItem template paints two backgrounds straight from variant resources: the row
        // border (PART_LayoutRoot) and the unnamed submenu popup border. Override both.
        var itemBorders = new Style(s => s.OfType<MenuItem>().Template().OfType<Border>())
        {
            Setters = { new Setter(Border.BackgroundProperty, Brushes.White) },
        };
        var gestureBase = new Style(s => s.OfType<MenuItem>().Template().OfType<TextBlock>().Name("PART_InputGestureText"))
        {
            Setters = { new Setter(TemplatedControl.ForegroundProperty, acceleratorBrush) },
        };
        var chevronBase = new Style(s => s.OfType<MenuItem>().Template().OfType<global::Avalonia.Controls.Shapes.Path>().Name("PART_ChevronPath"))
        {
            Setters = { new Setter(global::Avalonia.Controls.Shapes.Shape.FillProperty, chevronBrush) },
        };

        // Hover / pressed / submenu-open / disabled states — Fluent drives each of these from
        // variant resources, so re-pin them here in the light chrome.
        var itemSelectedRow = new Style(s => s.OfType<MenuItem>().Class(":selected").Template().OfType<Border>().Name("PART_LayoutRoot"))
        {
            Setters = { new Setter(Border.BackgroundProperty, hoverBrush) },
        };
        var itemSelectedHeader = new Style(s => s.OfType<MenuItem>().Class(":selected").Template().OfType<ContentPresenter>().Name("PART_HeaderPresenter"))
        {
            Setters = { new Setter(TemplatedControl.ForegroundProperty, Brushes.Black) },
        };
        var itemSelectedGesture = new Style(s => s.OfType<MenuItem>().Class(":selected").Template().OfType<TextBlock>().Name("PART_InputGestureText"))
        {
            Setters = { new Setter(TemplatedControl.ForegroundProperty, acceleratorFocusBrush) },
        };
        var itemSelectedChevron = new Style(s => s.OfType<MenuItem>().Class(":selected").Template().OfType<global::Avalonia.Controls.Shapes.Path>().Name("PART_ChevronPath"))
        {
            Setters = { new Setter(global::Avalonia.Controls.Shapes.Shape.FillProperty, chevronFocusBrush) },
        };
        var itemPressedRow = new Style(s => s.OfType<MenuItem>().Class(":pressed").Template().OfType<Border>().Name("PART_LayoutRoot"))
        {
            Setters = { new Setter(Border.BackgroundProperty, pressedBrush) },
        };
        var itemPressedHeader = new Style(s => s.OfType<MenuItem>().Class(":pressed").Template().OfType<ContentPresenter>().Name("PART_HeaderPresenter"))
        {
            Setters = { new Setter(TemplatedControl.ForegroundProperty, Brushes.Black) },
        };
        var itemOpenChevron = new Style(s => s.OfType<MenuItem>().Class(":open").Template().OfType<global::Avalonia.Controls.Shapes.Path>().Name("PART_ChevronPath"))
        {
            Setters = { new Setter(global::Avalonia.Controls.Shapes.Shape.FillProperty, chevronFocusBrush) },
        };
        var itemDisabledHeader = new Style(s => s.OfType<MenuItem>().Class(":disabled").Template().OfType<ContentPresenter>().Name("PART_HeaderPresenter"))
        {
            Setters = { new Setter(TemplatedControl.ForegroundProperty, disabledBrush) },
        };

        AppStyles.Add(flyoutPresenter);
        AppStyles.Add(itemBase);
        AppStyles.Add(itemBorders);
        AppStyles.Add(gestureBase);
        AppStyles.Add(chevronBase);
        AppStyles.Add(itemSelectedRow);
        AppStyles.Add(itemSelectedHeader);
        AppStyles.Add(itemSelectedGesture);
        AppStyles.Add(itemSelectedChevron);
        AppStyles.Add(itemPressedRow);
        AppStyles.Add(itemPressedHeader);
        AppStyles.Add(itemOpenChevron);
        AppStyles.Add(itemDisabledHeader);
    }

    /// <summary>
    /// Overrides the Fluent 12 ComboBox resource keys with the light Office chrome values.
    /// <para>
    /// The combo template parts (the popup border, the dropdown chevron glyph, and every
    /// ComboBoxItem state) bind these keys as DIRECT resource references inside the control
    /// template, and template value precedence beats ordinary style setters - so the style
    /// pins in the ribbon renderer silently lose under an OS dark scheme: the popup that
    /// opens under the font dropdown paints a dark charcoal surface, the chevron renders
    /// translucent white on the white combo, and item rows resolve dark ink. Redefining the
    /// keys at application scope fixes every consumer (ribbon combos and dialog combos
    /// alike) no matter which OS color-scheme variant resolves. Light-OS schemes already
    /// resolved these keys to the same light values, so nothing changes for them.
    /// </para>
    /// </summary>
    private static void AddComboChromeResources()
    {
        var divider = new ImmutableSolidColorBrush(Color.FromRgb(0xDA, 0xDC, 0xE0));
        var popupBorder = new ImmutableSolidColorBrush(Color.FromRgb(0xE0, 0xE0, 0xE0));
        var hover = new ImmutableSolidColorBrush(Color.FromRgb(0xE8, 0xEC, 0xF1));
        var pressed = new ImmutableSolidColorBrush(Color.FromRgb(0xD9, 0xE2, 0xEC));
        var selected = new ImmutableSolidColorBrush(Color.FromRgb(0xCC, 0xE4, 0xF7));
        var selectedHover = new ImmutableSolidColorBrush(Color.FromRgb(0xB9, 0xD9, 0xF2));
        var disabled = new ImmutableSolidColorBrush(Color.FromRgb(0x9A, 0x9A, 0x9A));
        var placeholder = new ImmutableSolidColorBrush(Color.FromRgb(0x76, 0x76, 0x76));

        var chrome = new ResourceDictionary
        {
            // Closed combo surface and ink.
            ["ComboBoxForeground"] = Brushes.Black,
            ["ComboBoxBackground"] = Brushes.White,
            ["ComboBoxBorderBrush"] = divider,
            ["ComboBoxForegroundDisabled"] = disabled,
            ["ComboBoxBackgroundDisabled"] = Brushes.White,
            ["ComboBoxBorderBrushDisabled"] = divider,
            ["ComboBoxBackgroundPointerOver"] = Brushes.White,
            ["ComboBoxBorderBrushPointerOver"] = divider,
            ["ComboBoxBackgroundPressed"] = Brushes.White,
            ["ComboBoxBorderBrushPressed"] = divider,
            ["ComboBoxBackgroundUnfocused"] = Brushes.Transparent,
            ["ComboBoxBackgroundBorderBrushUnfocused"] = Brushes.Transparent,
            ["ComboBoxBackgroundBorderBrushFocused"] = Brushes.Transparent,
            ["ComboBoxForegroundFocused"] = Brushes.Black,
            ["ComboBoxForegroundFocusedPressed"] = Brushes.Black,
            ["ComboBoxPlaceHolderForeground"] = placeholder,
            ["ComboBoxPlaceHolderForegroundFocusedPressed"] = placeholder,

            // Dropdown chevron and the popup surface that opens under the combo.
            ["ComboBoxDropDownGlyphForeground"] = Brushes.Black,
            ["ComboBoxDropDownGlyphForegroundFocused"] = Brushes.Black,
            ["ComboBoxDropDownGlyphForegroundFocusedPressed"] = Brushes.Black,
            ["ComboBoxDropDownGlyphForegroundDisabled"] = disabled,
            ["ComboBoxDropDownBackground"] = Brushes.White,
            ["ComboBoxDropDownBorderBrush"] = popupBorder,

            // Dropdown rows: white surface, black ink, light Office hover/selected tints.
            ["ComboBoxItemForeground"] = Brushes.Black,
            ["ComboBoxItemBackground"] = Brushes.Transparent,
            ["ComboBoxItemBorderBrushDisabled"] = Brushes.Transparent,
            ["ComboBoxItemForegroundPointerOver"] = Brushes.Black,
            ["ComboBoxItemBackgroundPointerOver"] = hover,
            ["ComboBoxItemBorderBrushPointerOver"] = Brushes.Transparent,
            ["ComboBoxItemForegroundPressed"] = Brushes.Black,
            ["ComboBoxItemBackgroundPressed"] = pressed,
            ["ComboBoxItemBorderBrushPressed"] = Brushes.Transparent,
            ["ComboBoxItemForegroundSelected"] = Brushes.Black,
            ["ComboBoxItemBackgroundSelected"] = selected,
            ["ComboBoxItemForegroundSelectedPointerOver"] = Brushes.Black,
            ["ComboBoxItemBackgroundSelectedPointerOver"] = selectedHover,
            ["ComboBoxItemBorderBrushSelectedPointerOver"] = Brushes.Transparent,
            ["ComboBoxItemForegroundSelectedPressed"] = Brushes.Black,
            ["ComboBoxItemBackgroundSelectedPressed"] = selectedHover,
            ["ComboBoxItemBorderBrushSelectedPressed"] = Brushes.Transparent,
            ["ComboBoxItemForegroundDisabled"] = disabled,
            ["ComboBoxItemBackgroundDisabled"] = Brushes.Transparent,
            ["ComboBoxItemForegroundSelectedDisabled"] = disabled,
            ["ComboBoxItemBackgroundSelectedDisabled"] = selected,
            ["ComboBoxItemBorderBrushSelectedDisabled"] = Brushes.Transparent,
        };
        Application.Current!.Resources.MergedDictionaries.Add(chrome);
    }

    private static Styles AppStyles => Application.Current!.Styles;

    /// <summary>
    /// R169 one-time recovery for a user who already has two settings.json files because a pre-fix
    /// build of this Avalonia shell wrote to <c>%LOCALAPPDATA%\Writer\settings.json</c> while the WPF
    /// host always wrote to <c>%APPDATA%\Writer\settings.json</c> (the two now-shared, canonical path).
    /// Policy, applied before every load so the two shells stay reconciled even if a stray legacy
    /// write ever recurs:
    /// <list type="bullet">
    /// <item>Only the legacy file exists (an Avalonia-only user) -&gt; it is copied to the canonical
    /// path, so their preferences survive instead of reading back as fresh defaults.</item>
    /// <item>Both exist and the legacy file was written more recently -&gt; the user's latest edits
    /// were made in this shell, so the legacy file is copied over the canonical one (last-write
    /// wins).</item>
    /// <item>Both exist and the canonical file is the same age or newer -&gt; the canonical file
    /// (the WPF host's, or an already-migrated one) is left alone.</item>
    /// </list>
    /// The legacy file itself is never deleted in any of these cases, so no preference set is ever
    /// silently discarded -- the losing file simply becomes an inert leftover on disk. Best-effort
    /// only: any I/O failure here must not block startup, so the shell falls back to loading (or
    /// creating) the canonical file exactly as it would if there were nothing to migrate.
    /// </summary>
    private static void MigrateLegacyLocalSettings()
    {
        try
        {
            var legacyPath = JsonSettingsStore<WriterOptions>.GetProductFilePath(
                ApplicationOptionsStore<WriterOptions>.DefaultFileName,
                PlatformApplicationDataPathProvider.LocalInstance);
            var canonicalPath = JsonSettingsStore<WriterOptions>.GetProductFilePath(
                ApplicationOptionsStore<WriterOptions>.DefaultFileName,
                PlatformApplicationDataPathProvider.Instance);

            ReconcileLegacySettingsFile(legacyPath, canonicalPath);
        }
        catch
        {
            // Best-effort only -- see summary above.
        }
    }

    /// <summary>
    /// The pure file-reconciliation policy described above, isolated from real path resolution so it
    /// can be exercised against temp-directory paths in <c>Writer.App.Avalonia.Tests</c> without ever
    /// touching a real <c>%APPDATA%</c>/<c>%LOCALAPPDATA%</c>. Returns <see langword="true"/> when it
    /// copied <paramref name="legacyPath"/> onto <paramref name="canonicalPath"/>.
    /// </summary>
    internal static bool ReconcileLegacySettingsFile(string legacyPath, string canonicalPath)
    {
        // Same folder on this platform (macOS): there is nothing to reconcile.
        if (string.Equals(legacyPath, canonicalPath, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!System.IO.File.Exists(legacyPath))
            return false;

        if (System.IO.File.Exists(canonicalPath) &&
            System.IO.File.GetLastWriteTimeUtc(legacyPath) <=
                System.IO.File.GetLastWriteTimeUtc(canonicalPath))
        {
            return false;
        }

        var canonicalDirectory = System.IO.Path.GetDirectoryName(canonicalPath);
        if (!string.IsNullOrEmpty(canonicalDirectory))
            System.IO.Directory.CreateDirectory(canonicalDirectory);

        System.IO.File.Copy(legacyPath, canonicalPath, overwrite: true);
        return true;
    }
}
