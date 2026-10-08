using Writer.Shared.Ribbon;
using Writer.App.Presentation.ContextMenus;
using Writer.App.Presentation.Ribbon;
using Writer.Core.Model;

namespace Writer.Ribbon.Definitions;

/// <summary>
/// Canonical Writer tab topology shared by both renderers. Capability checks select only the
/// presentation shape that each host already supports; command ownership and ordering live here.
/// </summary>
internal static partial class WriterCanonicalRibbonTabs
{
    internal static RibbonDefinitionBuilder AddLayoutTab(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities) =>
        builder.Tab("layout", "Layout", "L", tab =>
        {
            var topology = new WriterRibbonTabTopology(tab, capabilities);

            topology.Section(
                "layout.page-setup",
                tab => tab.Group("page-setup", "Page Setup", "P", 100, group =>
                    {
                        // Word retains the Page Setup commands as a compact two-row block at narrow
                        // widths instead of spilling them into a generic group flyout.
                        group.Sizing(RibbonGroupSizing.OfficeIconAdaptive with { MaximumRowsPerColumn = 2 });
                        group.Large("writer.margins", "Margins", RibbonCommandIconKind.Margins, "M", menu: menu =>
                        {
                            menu.Item("writer.margins", "Normal / Narrow (toggle)", "N");
                            menu.Item("writer.custom-margins", "Custom Margins\u2026", RibbonCommandIconKind.Margins, "A");
                        });
                        group.Medium("writer.orientation", "Orientation", RibbonCommandIconKind.Orientation, dropdown: true);
                        group.Medium("writer.size", "Size", RibbonCommandIconKind.OnePage, "Z", menu: menu =>
                        {
                            menu.Item("writer.size", "Letter / A4 (toggle)", "L");
                            menu.Item("writer.more-paper-sizes", "More Paper Sizes\u2026", RibbonCommandIconKind.OnePage, "M");
                        });
                        group.Medium("writer.columns", "Columns", RibbonCommandIconKind.TextColumns, menu: menu =>
                        {
                            menu.Item("writer.columns-one", "One", RibbonCommandIconKind.TextColumns, "O");
                            menu.Item("writer.columns-two", "Two", RibbonCommandIconKind.TextColumns, "T");
                            menu.Item("writer.columns-three", "Three", RibbonCommandIconKind.TextColumns, "H");
                            menu.Item("writer.columns-left", "Left", RibbonCommandIconKind.TextColumns, "L");
                            menu.Item("writer.columns-right", "Right", RibbonCommandIconKind.TextColumns, "R");
                            menu.Item("writer.columns-more", "More Columns...", RibbonCommandIconKind.TextColumns, "M");
                        });
                        group.Medium("writer.breaks", "Breaks", RibbonCommandIconKind.PageBreak, "B", menu: menu =>
                        {
                            menu.Item("writer.page-break", "Page Break", "P");
                            menu.Item("writer.column-break", "Column Break", "C");
                            menu.Separator();
                            menu.Item("writer.section-break-next-page", "Next Page", "N");
                            menu.Item("writer.section-break-continuous", "Continuous", "O");
                            menu.Item("writer.section-break-even-page", "Even Page", "E");
                            menu.Item("writer.section-break-odd-page", "Odd Page", "D");
                        });
                        group.DialogLauncher("writer.page-setup", "Page Setup", "Open Page Setup.", "G");
                        group.Icon("writer.line-numbers", "Line Numbers", RibbonCommandIconKind.Number, menu: menu =>
                        {
                            menu.Item("writer.line-numbers-none", "None", "N");
                            menu.Item("writer.line-numbers-continuous", "Continuous", "C");
                            menu.Item("writer.line-numbers-restart-page", "Restart Each Page", "P");
                            menu.Item("writer.line-numbers-restart-section", "Restart Each Section", "S");
                            menu.Item("writer.line-numbers-options", "Line Numbering Options...", "O");
                        });
                        group.Icon("writer.hyphenation", "Hyphenation", RibbonCommandIconKind.Hyphenation, "HY", menu: menu =>
                        {
                            menu.Item("writer.hyphenation-none", "None", RibbonCommandIconKind.Hyphenation, "N");
                            menu.Item("writer.hyphenation-auto", "Automatic", RibbonCommandIconKind.Hyphenation, "A");
                            menu.Item("writer.hyphenation-manual", "Manual", RibbonCommandIconKind.Hyphenation, "M");
                            menu.Item("writer.hyphenation-options", "Hyphenation Options\u2026", RibbonCommandIconKind.Hyphenation, "H");
                        });
                        group.Icon("writer.page-valign", "Vertical Align", RibbonCommandIconKind.AlignJustify);
                        group.Icon("writer.different-first-page", "Different First Page", RibbonCommandIconKind.CoverPage);
                    }),
                tab => tab.Group("page-setup", "Page Setup", null, 100, group =>
                    {
                        // At narrow widths Word keeps Page Setup and Paragraph directly reachable.
                        // Prefer the existing icon-based compact presentation to collapsing Paragraph
                        // while this broad group is still visible at full command widths.
                        group.Sizing(RibbonGroupSizing.OfficeIconAdaptive);
                        group.Dropdown("writer.margins", "Margins", BuildAvaloniaMarginsMenu(), control => control with
                        {
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Margins),
                        });
                        group.Button("writer.orientation", "Orientation");
                        group.Dropdown("writer.size", "Size", BuildAvaloniaPageSizeMenu());
                        group.Dropdown("writer.columns", "Columns", BuildAvaloniaColumnsMenu());
                        group.Dropdown("writer.breaks", "Breaks", BuildAvaloniaBreaksMenu());
                        group.Dropdown("writer.line-numbers", "Line Numbers", BuildAvaloniaLineNumbersMenu());
                        group.Dropdown("writer.hyphenation", "Hyphenation", BuildAvaloniaHyphenationMenu());
                        group.Toggle("writer.different-first-page", "Different First Page");
                        group.Button("writer.page-valign", "Vertical Align");
                        group.Button("writer.page-setup", "Page Setup...");
                    }));

            topology.Section(
                "layout.paragraph",
                tab => tab.Group("paragraph", WriterRibbonText.ParagraphGroup.Label, "A", 95, group =>
                    {
                        // Keep the compact, direct Paragraph block alongside Page Setup at Word's
                        // narrow layout width; Preview, Arrange, and Data yield space first.
                        group.Sizing(RibbonGroupSizing.OfficeIconAdaptive);
                        group.Icon("writer.indent-decrease", "Decrease Indent", RibbonCommandIconKind.IndentDecrease);
                        group.Icon("writer.indent-increase", "Increase Indent", RibbonCommandIconKind.IndentIncrease);
                        group.ComboBox("writer.line-spacing", "Line and Paragraph Spacing", control => control with
                        {
                            Items = new[] { "1.0", "1.15", "1.5", "2.0" },
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.LineSpacing),
                            Width = 52,
                        });
                        group.ComboBox("writer.indent-left", "Indent Left", control => control with
                        {
                            Items = new[] { "0", "18", "36", "54", "72" },
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.IndentIncrease),
                            Width = 52,
                        });
                        group.ComboBox("writer.indent-right", "Indent Right", control => control with
                        {
                            Items = new[] { "0", "18", "36", "54", "72" },
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.IndentDecrease),
                            Width = 52,
                        });
                        group.RowBreak();
                        group.Icon("writer.space-before-toggle", "Add Space Before Paragraph", RibbonCommandIconKind.SpaceBefore);
                        group.Icon("writer.space-after-toggle", "Add Space After Paragraph", RibbonCommandIconKind.SpaceAfter);
                        group.ComboBox("writer.space-before", "Spacing Before", control => control with
                        {
                            Items = new[] { "0", "6", "12", "18", "24" },
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.SpaceBefore),
                            Width = 52,
                        });
                        group.ComboBox("writer.space-after", "Spacing After", control => control with
                        {
                            Items = new[] { "0", "6", "8", "12", "18", "24" },
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.SpaceAfter),
                            Width = 52,
                        });
                        group.DialogLauncher("writer.paragraph-dialog", "Paragraph Settings", "Open Paragraph settings.");
                        group.Icon("writer.tabs-dialog", "Tabs", RibbonCommandIconKind.Ruler);
                    }),
                tab => tab.Group("paragraph", WriterRibbonText.ParagraphGroup.Label, null, 95, group =>
                    {
                        // Pair with Page Setup's compact form so both primary Layout groups remain
                        // actionable before lower-priority Preview, Arrange, and Data groups overflow.
                        group.Sizing(RibbonGroupSizing.OfficeIconAdaptive);
                        group.Button("writer.indent-decrease", "Decrease Indent", control => control with
                        {
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.IndentDecrease),
                        });
                        group.Button("writer.indent-increase", "Increase Indent");
                        group.ComboBox("writer.line-spacing", "Line and Paragraph Spacing", control => control with
                        {
                            Items = new[] { "1.0", "1.15", "1.5", "2.0" },
                            Width = 52,
                        });
                        group.ComboBox("writer.indent-left", "Indent Left", control => control with
                        {
                            Items = new[] { "0", "18", "36", "54", "72" },
                            Width = 52,
                        });
                        group.ComboBox("writer.indent-right", "Indent Right", control => control with
                        {
                            Items = new[] { "0", "18", "36", "54", "72" },
                            Width = 52,
                        });
                        group.Button("writer.space-before-toggle", "Add Space Before Paragraph");
                        group.Button("writer.space-after-toggle", "Add Space After Paragraph");
                        group.ComboBox("writer.space-before", "Spacing Before", control => control with
                        {
                            Items = new[] { "0", "6", "12", "18", "24" },
                            Width = 52,
                        });
                        group.ComboBox("writer.space-after", "Spacing After", control => control with
                        {
                            Items = new[] { "0", "6", "8", "12", "18", "24" },
                            Width = 52,
                        });
                        group.DialogLauncher("writer.paragraph-dialog", "Paragraph Settings", "Open Paragraph settings.");
                        group.Button("writer.tabs-dialog", "Tabs");
                    }));

            topology.Section(
                "layout.preview",
                tab => tab.Group("preview", "Preview", "V", 60, group =>
                    group.Large("writer.print-preview", "Print Preview", RibbonCommandIconKind.Print)),
                tab => tab.Group("preview", "Preview", null, 60, group =>
                    group.Button("writer.print-preview", "Print Preview", control => control with
                    {
                        Icon = new RibbonCommandIcon(RibbonCommandIconKind.Print),
                    })),
                portableOrder: 3);

            topology.Section(
                "layout.arrange",
                tab => tab.Group("arrange", "Arrange", "A", 75, group =>
                    {
                        group.Medium("writer.layout-wrap", "Wrap Text", RibbonCommandIconKind.Wrap, menu: menu =>
                        {
                            menu.Item("writer.layout-wrap-inline", "In Line with Text", "I");
                            menu.Item("writer.layout-wrap-square", "Square", "S");
                            menu.Item("writer.layout-wrap-tight", "Tight", "T");
                            menu.Item("writer.layout-wrap-top-bottom", "Top and Bottom", "B");
                            menu.Item("writer.layout-wrap-behind", "Behind Text", "H");
                            menu.Item("writer.layout-wrap-front", "In Front of Text", "F");
                        });
                        group.Medium("writer.layout-selection-pane", "Selection Pane", RibbonCommandIconKind.NavigationPane);
                        group.Medium("writer.layout-bring-forward", "Bring Forward", RibbonCommandIconKind.BringForward);
                        group.Medium("writer.layout-send-backward", "Send Backward", RibbonCommandIconKind.SendBackward);
                        group.Medium("writer.layout-rotate", "Rotate", RibbonCommandIconKind.Rotate, menu: menu =>
                        {
                            menu.Item("writer.layout-rotate-right90", "Rotate Right 90°", "R");
                            menu.Item("writer.layout-rotate-left90", "Rotate Left 90°", "L");
                            menu.Item("writer.layout-flip-vertical", "Flip Vertical", "V");
                            menu.Item("writer.layout-flip-horizontal", "Flip Horizontal", "H");
                        });
                        group.Medium("writer.object-group", "Group", RibbonCommandIconKind.Group);
                        group.Medium("writer.object-ungroup", "Ungroup", RibbonCommandIconKind.Ungroup);
                    }),
                tab => tab.Group("arrange", "Arrange", null, 75, group =>
                    {
                        group.Dropdown("writer.layout-position", "Position", BuildFloatingPositionMenu("layout"));
                        group.Dropdown("writer.layout-wrap", "Wrap Text", BuildWrapMenu("layout"), control => control with
                        {
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Wrap),
                        });
                        group.Button("writer.layout-selection-pane", "Selection Pane");
                        group.Button("writer.layout-bring-forward", "Bring Forward");
                        group.Button("writer.layout-send-backward", "Send Backward");
                        group.Dropdown("writer.layout-rotate", "Rotate", BuildRotateMenu("layout"));
                        group.Button("writer.object-group", "Group");
                        group.Button("writer.object-ungroup", "Ungroup");
                    }),
                portableOrder: 2);

            topology.Section(
                "layout.data",
                tab => tab.Group("data", "Data", "D", 55, group =>
                    {
                        group.Medium("writer.text-to-table", "Text to Table", RibbonCommandIconKind.Table,
                            accent: RibbonCommandIconAccent.Green);
                        group.Medium("writer.table-to-text", "Table to Text", RibbonCommandIconKind.TextFunction);
                    }),
                tab => tab.Group("data", "Data", null, 55, group =>
                    {
                        group.Button("writer.text-to-table", "Text to Table", control => control with
                        {
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Table),
                        });
                        group.Button("writer.table-to-text", "Table to Text");
                    }),
                portableOrder: 2);

            topology.Build();
        });

    internal static RibbonDefinitionBuilder AddDesignTab(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities) =>
        builder.Tab("design", "Design", "G", tab =>
        {
            var topology = new WriterRibbonTabTopology(tab, capabilities);

            topology.Section(
                "design.formatting",
                tab => tab.Group("themes", "Document Formatting", "T", 100, group =>
                    {
                        // At Word's narrow ribbon width, keep Document Formatting as its compact
                        // menu strip rather than collapsing the entire group into one flyout.
                        group.Sizing(RibbonGroupSizing.OfficeAdaptive with
                        {
                            Hints = new RibbonWidthHints(760, 438, 438, 64),
                        });
                        group.ComboBox("writer.theme", "Themes", control => control with
                        {
                            Items = DocumentTheme.Catalog.Select(theme => theme.Name).ToArray(),
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Theme, RibbonCommandIconAccent.Theme),
                            Width = 140,
                        });
                        group.ComboBox("writer.style-set", "Style Sets", control => control with
                        {
                            Items = DocumentStyleSet.Catalog.Select(style => style.Name).ToArray(),
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Font, RibbonCommandIconAccent.Theme),
                            Width = 140,
                        });
                        group.Icon("writer.reset-style-set", "Reset to Default Style Set", RibbonCommandIconKind.Refresh);
                        group.Medium("writer.theme-colors", "Colors", RibbonCommandIconKind.Color, "C",
                            menu: menu => BuildWpfThemeMenu("writer.theme-colors", menu),
                            accent: RibbonCommandIconAccent.Color);
                        group.Medium("writer.theme-fonts", "Fonts", RibbonCommandIconKind.Font, "F",
                            menu: menu => BuildWpfFontSetMenu("writer.theme-fonts", menu),
                            accent: RibbonCommandIconAccent.Theme);
                        group.Medium("writer.paragraph-spacing", "Paragraph Spacing", RibbonCommandIconKind.LineSpacing, "P",
                            menu: menu => BuildWpfParagraphSpacingMenu("writer.paragraph-spacing", menu),
                            accent: RibbonCommandIconAccent.Theme);
                        group.Medium("writer.theme-effects", "Effects", RibbonCommandIconKind.Effects, "E",
                            menu: menu => BuildWpfEffectsMenu("writer.theme-effects", menu),
                            accent: RibbonCommandIconAccent.Theme);
                    }),
                tab =>
                {
                    tab.Group("themes", "Themes", null, 110, group =>
                    group.Dropdown("writer.theme", "Themes", BuildAvaloniaThemeMenu()));

                    tab.Group("document-formatting", "Document Formatting", null, 100, group =>
                    {
                        group.Dropdown("writer.theme-colors", "Colors", BuildAvaloniaThemeColorsMenu());
                        group.Dropdown("writer.style-set", "Style Sets", BuildAvaloniaStyleSetsMenu());
                        group.Button("writer.reset-style-set", "Reset to Default Style Set", button => button with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Refresh),
                        });
                        group.Dropdown("writer.theme-fonts", "Fonts", BuildAvaloniaThemeFontsMenu());
                        group.Dropdown("writer.para-spacing", "Paragraph Spacing", BuildAvaloniaParagraphSpacingMenu());
                        group.Dropdown("writer.theme-effects", "Effects", WriterContextMenuPlanner.BuildEffects());
                    });
                });

            topology.Section(
                "design.page-background",
                tab => tab.Group("page-background", WriterRibbonText.PageBackgroundGroup.Label,
                        WriterRibbonText.PageBackgroundGroup.KeyTip, 90, group =>
                    {
                        group.Sizing(RibbonGroupSizing.OfficeAdaptive);
                        group.Medium("writer.watermark", WriterRibbonText.WatermarkCommand.Label,
                            RibbonCommandIconKind.Watermark);
                        group.Medium("writer.page-color", WriterRibbonText.PageColorCommand.Label,
                            RibbonCommandIconKind.Fill, accent: RibbonCommandIconAccent.Fill, dropdown: true);
                        group.Medium("writer.page-border", WriterRibbonText.PageBordersCommand.Label,
                            RibbonCommandIconKind.Border, accent: RibbonCommandIconAccent.Border);
                    }),
                tab => tab.Group("page-background", WriterRibbonText.PageBackgroundGroup.Label, null, 90, group =>
                    {
                        group.Dropdown("writer.watermark", WriterRibbonText.WatermarkCommand.Label,
                            BuildAvaloniaWatermarkMenu());
                        group.Dropdown("writer.page-color", WriterRibbonText.PageColorCommand.Label,
                            BuildAvaloniaPageColorMenu());
                        group.Button("writer.page-border", WriterRibbonText.PageBordersCommand.Label);
                    }));

            topology.Build();
        });

    internal static RibbonDefinitionBuilder AddViewTab(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities) =>
        builder.Tab("view", "View", "W", tab =>
        {
            var topology = new WriterRibbonTabTopology(tab, capabilities);

            topology.Section(
                "view.views",
                tab => tab.Group("views", "Views", "V", 100, group =>
                    {
                        group.Medium("writer.read-mode", "Read Mode", RibbonCommandIconKind.ReadMode, menu: menu =>
                        {
                            menu.Item("writer.read-mode-column-narrow", "Narrow Column Width", "N");
                            menu.Item("writer.read-mode-column-default", "Default Column Width", "D");
                            menu.Item("writer.read-mode-column-wide", "Wide Column Width", "W");
                            menu.Separator();
                            menu.Item("writer.read-mode-color-none", WriterRibbonText.PageColorNoColorOption, "O");
                            menu.Item("writer.read-mode-color-sepia", "Sepia", "S");
                            menu.Item("writer.read-mode-color-inverse", "Inverse (Dark Mode)", "I");
                        });
                        group.MediumToggle("writer.print-layout", "Print Layout", RibbonCommandIconKind.PrintLayout);
                        group.MediumToggle("writer.web-layout", "Web Layout", RibbonCommandIconKind.WebLayout);
                        group.MediumToggle("writer.outline-view", "Outline", RibbonCommandIconKind.MultilevelList);
                        group.MediumToggle("writer.draft-view", "Draft", RibbonCommandIconKind.Draft);
                        group.MediumToggle("writer.paged-edit-view", "Page Edit", RibbonCommandIconKind.PrintLayout);
                    }),
                tab => tab.Group("views", "Views", null, 110, group =>
                    {
                        group.Dropdown("writer.read-mode", "Read Mode", BuildAvaloniaReadModeMenu(), dropdown => dropdown with
                        {
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.ReadMode),
                        });
                        group.Toggle("writer.print-layout", "Print Layout");
                        group.Toggle("writer.web-layout", "Web Layout");
                        group.Toggle("writer.outline-view", "Outline");
                        group.Toggle("writer.draft-view", "Draft");
                        group.Toggle("writer.paged-edit-view", "Page Edit");
                    }));

            // Focus is a separate editing surface rather than an alias for Read Mode: it keeps the
            // document editable while temporarily removing the surrounding application chrome.
            topology.Section(
                "view.immersive",
                tab => tab.Group("immersive", "Immersive", "I", 45, group =>
                {
                    group.MediumToggle("writer.focus", "Focus", RibbonCommandIconKind.View);
                }),
                tab => tab.Group("immersive", "Immersive", null, 55, group =>
                {
                    group.Toggle("writer.focus", "Focus", control => control with
                    {
                        Icon = new RibbonCommandIcon(RibbonCommandIconKind.View),
                    });
                }));

            topology.Section(
                "view.show",
                // Word folds these secondary view aids into one affordance at the 900- and
                // 750-DIP reference widths, keeping the Window commands directly available.
                tab => tab.Group("show", "Show", "S", 75, group =>
                    {
                        group.MediumToggle("writer.ruler", "Ruler", RibbonCommandIconKind.Ruler);
                        group.MediumToggle("writer.nav-pane", "Navigation Pane", RibbonCommandIconKind.NavigationPane);
                        group.MediumToggle("writer.gridlines", "Gridlines", RibbonCommandIconKind.Grid);
                    }),
                tab => tab.Group("show", "Show", null, 100, group =>
                    {
                        group.Toggle("writer.ruler", "Ruler", control => control with
                        {
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Ruler),
                        });
                        group.Toggle("writer.gridlines", "Gridlines");
                        group.Toggle("writer.nav-pane", "Navigation Pane");
                        group.Toggle("writer.reviewing-pane", "Reviewing Pane");
                        group.Toggle("writer.reveal-formatting", "Reveal Formatting");
                    }));

            topology.Section(
                "view.zoom",
                tab => tab.Group("zoom", "Zoom", "Z", 80, group =>
                    {
                        group.Large("writer.zoom-dialog", "Zoom", RibbonCommandIconKind.Zoom);
                        group.Medium("writer.zoom-100", "100%", RibbonCommandIconKind.Zoom);
                        group.Medium("writer.zoom-one-page", "One Page", RibbonCommandIconKind.OnePage);
                        group.Medium("writer.zoom-page-width", "Page Width", RibbonCommandIconKind.Scale);
                        group.MediumToggle("writer.zoom-multiple-pages", "Multiple Pages", RibbonCommandIconKind.PreviewResults);
                        group.MediumToggle("writer.zoom-side-to-side", "Side to Side", RibbonCommandIconKind.OnePage);
                    }),
                tab => tab.Group("zoom", "Zoom", null, 90, group =>
                    {
                        group.Button("writer.zoom-dialog", "Zoom", control => control with
                        {
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Zoom),
                        });
                        group.Button("writer.zoom-100", "100%");
                        group.Button("writer.zoom-one-page", "One Page");
                        group.Button("writer.zoom-page-width", "Page Width");
                        group.Toggle("writer.zoom-multiple-pages", "Multiple Pages");
                        group.Toggle("writer.zoom-side-to-side", "Side to Side");
                    }));

            topology.Section(
                "view.window",
                // At Word's 900-DIP View ribbon width, keep the Window command stack direct while
                // Zoom yields its secondary presets first. At 750 DIPs Word folds this entire
                // stack into Window after the Show group has compacted.
                tab => tab.Group("window", "Window", "N", 85, group =>
                    {
                        group.Sizing(RibbonGroupSizing.Default with
                        {
                            // Reserve Word's two-column Window stack while it is direct. This keeps
                            // 900 DIPs direct but makes the existing collapsed flyout win at 750.
                            Hints = new RibbonWidthHints(300, 300, 300, 64),
                        });
                        group.MediumToggle("writer.split-window", "Split", RibbonCommandIconKind.Scale);
                        group.Medium("writer.new-window", "New Window", RibbonCommandIconKind.Page);
                        group.Medium("writer.arrange-all", "Arrange All", RibbonCommandIconKind.Grid);
                        group.Medium("writer.switch-windows", "Switch Windows", RibbonCommandIconKind.Window);
                    }),
                tab => tab.Group("window", "Window", null, 80, group =>
                    {
                        group.Button("writer.new-window", "New Window");
                        group.Button("writer.arrange-all", "Arrange All");
                        group.Button("writer.switch-windows", "Switch Windows");
                        group.Toggle("writer.split-window", "Split", control => control with
                        {
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Scale),
                        });
                    }));

            topology.Build();
        });
    internal static RibbonDefinitionBuilder AddFileTab(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities)
    {
        if (!capabilities.IncludesSection(WriterRibbonTopologySection.File))
            return builder;

        return builder.Tab("file", "File", "F", tab =>
            tab.Group("document", "Document", null, 100, group =>
            {
                group.Button("writer.backstage", "File...");
                group.Button("writer.new", "New");
                group.Button("writer.open", "Open");
                group.Button("writer.import-pdf-text", "Import PDF (text only)");
                group.Button("writer.save", "Save");
            }));
    }

    internal static RibbonDefinitionBuilder AddMailingsTab(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities)
    {
        var avalonia = capabilities.UsesPortableControls;

        return builder.Tab("mailings", "Mailings", "M", tab =>
        {
            tab.Group("create", "Create", "C", avalonia ? 110 : 130, group =>
            {
                AddProfiledButton(group, avalonia, "writer.merge-envelopes", "Envelopes",
                    RibbonCommandIconKind.Envelope, wpfKeyTip: "E");
                AddProfiledButton(group, avalonia, "writer.merge-labels", "Labels",
                    RibbonCommandIconKind.MergeField, wpfKeyTip: "L");
            });
            tab.Group("merge-data", "Start Mail Merge", "D", avalonia ? 120 : 155, group =>
            {
                group.Dropdown("writer.start-mail-merge", "Start Mail Merge", BuildStartMailMergeMenu(), control => control with
                {
                    Icon = new RibbonCommandIcon(RibbonCommandIconKind.Envelope),
                    KeyTip = "S",
                    PreferredLayout = RibbonCommandLayoutKind.Medium,
                });
                AddProfiledButton(group, avalonia, "writer.merge-data", "Select Recipients",
                    RibbonCommandIconKind.Recipients);
                AddProfiledButton(group, avalonia, "writer.merge-edit-recipients", "Edit Recipient List",
                    RibbonCommandIconKind.Recipients);
                AddProfiledButton(group, avalonia, "writer.merge-filter-sort", "Filter & Sort Recipients",
                    RibbonCommandIconKind.Recipients);
            });
            tab.Group("merge-write", "Write & Insert Fields", "W", avalonia ? 100 : 145, group =>
            {
                // Word keeps Write & Insert Fields visible as compact disabled icons at the
                // narrow reference width. Preserve that discoverability before overflow.
                if (!avalonia)
                    group.Sizing(RibbonGroupSizing.OfficeIconAdaptive);
                AddProfiledButton(group, avalonia, "writer.merge-address-block", "Address Block",
                    RibbonCommandIconKind.Recipients, wpfKeyTip: "A");
                AddProfiledButton(group, avalonia, "writer.merge-greeting-line", "Greeting Line",
                    RibbonCommandIconKind.GreetingLine, wpfKeyTip: "G");
                AddProfiledButton(group, avalonia, "writer.merge-field", "Insert Merge Field",
                    RibbonCommandIconKind.MergeField, wpfKeyTip: "F");
                AddProfiledButton(group, avalonia, "writer.merge-match-fields", "Match Fields",
                    RibbonCommandIconKind.MergeField, wpfKeyTip: "H");
                group.Dropdown("writer.merge-rules", "Rules", BuildMergeRulesMenu(), control => control with
                {
                    Icon = new RibbonCommandIcon(RibbonCommandIconKind.Field),
                    KeyTip = "U",
                    PreferredLayout = RibbonCommandLayoutKind.Medium,
                });
            });
            tab.Group("merge-preview", "Preview Results", "P", avalonia ? 80 : 120, group =>
            {
                AddProfiledButton(group, avalonia, "writer.merge-preview", "Preview Results",
                    RibbonCommandIconKind.PreviewResults);
                AddProfiledButton(group, avalonia, "writer.merge-preview-first", "First Record",
                    RibbonCommandIconKind.Previous, wpfLayout: RibbonCommandLayoutKind.Small);
                AddProfiledButton(group, avalonia, "writer.merge-preview-previous", "Previous Record",
                    RibbonCommandIconKind.Previous, wpfLayout: RibbonCommandLayoutKind.Small,
                    avaloniaLabel: "\u25C0 Previous");
                AddProfiledButton(group, avalonia, "writer.merge-preview-next", "Next Record",
                    RibbonCommandIconKind.Next, wpfLayout: RibbonCommandLayoutKind.Small,
                    avaloniaLabel: "Next \u25B6");
                AddProfiledButton(group, avalonia, "writer.merge-preview-last", "Last Record",
                    RibbonCommandIconKind.Next, wpfLayout: RibbonCommandLayoutKind.Small);
                AddProfiledButton(group, avalonia, "writer.merge-find-recipient", "Find Recipient",
                    RibbonCommandIconKind.Search);
                AddProfiledButton(group, avalonia, "writer.merge-check-errors", "Check for Errors",
                    RibbonCommandIconKind.Warning, wpfAccent: RibbonCommandIconAccent.Warning);
            });
            tab.Group("merge-finish", "Finish", "F", avalonia ? 70 : 110, group =>
            {
                AddProfiledButton(group, avalonia, "writer.merge-finish", "Finish & Merge",
                    RibbonCommandIconKind.FinishMerge);
                AddProfiledButton(group, avalonia, "writer.merge-email", "Send E-mail Messages",
                    RibbonCommandIconKind.Envelope, wpfKeyTip: "M");
            });
        });
    }

    internal static RibbonDefinitionBuilder AddHelpTab(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities)
    {
        var avalonia = capabilities.UsesPortableControls;

        return builder.Tab("help", "Help", "Y", tab =>
        {
            tab.Group("help", "Help", "H", 100, group =>
            {
                AddHelpButton(group, avalonia, "writer.help-online", "Help Online",
                    RibbonCommandIconKind.Help, "H");
                AddHelpButton(group, avalonia, "writer.feedback", "Feedback",
                    RibbonCommandIconKind.Feedback, "F");
                AddHelpButton(group, avalonia, "writer.copy-diagnostics", "Copy Diagnostics",
                    RibbonCommandIconKind.Info, "D");
                AddHelpButton(group, avalonia, "writer.test-crash-reporting", "Test Crash Reporting",
                    RibbonCommandIconKind.Info, "T");
            });
            tab.Group("product", "Product", "P", 90, group =>
            {
                AddHelpButton(group, avalonia, "writer.check-updates", "Check for Updates",
                    RibbonCommandIconKind.Refresh, "U");
                AddHelpButton(group, avalonia, "writer.about", "About Writer",
                    RibbonCommandIconKind.Info, "A");
                AddHelpButton(group, avalonia, "writer.legal-notices", "Legal Notices",
                    RibbonCommandIconKind.Book, "L");
            });
        });
    }

    internal static RibbonDefinitionBuilder AddDeveloperTab(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities)
    {
        var avalonia = capabilities.UsesPortableControls;

        return builder.Tab("developer", "Developer", "D", tab =>
            tab.Group("controls", "Controls", "O", 100, group =>
            {
                AddSharedIconButton(group, "writer.cc-text", "Text Control", RibbonCommandIconKind.TextBox);
                AddSharedIconButton(group, "writer.cc-richtext", "Rich Text", RibbonCommandIconKind.QuickParts);
                AddSharedIconButton(group, "writer.cc-checkbox", "Check Box", RibbonCommandIconKind.CheckBox);
                AddSharedIconButton(group, "writer.cc-date", "Date Picker", RibbonCommandIconKind.Date);
                AddSharedIconButton(group, "writer.cc-dropdown", "Drop-Down List", RibbonCommandIconKind.List);
                AddSharedIconButton(group, "writer.cc-combo", "Combo Box", RibbonCommandIconKind.ChevronDown);
            }));
    }

    internal static RibbonDefinitionBuilder AddHeaderFooterDesignTab(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities)
    {
        var avalonia = capabilities.UsesPortableControls;

        return builder.ContextualTab("header-footer-design", "Design",
            new RibbonTabContext("header-footer", "Header & Footer Tools", RibbonContextColor.Purple), tab =>
            {
                tab.Group("hf-header-footer", "Header & Footer", "H", 120, group =>
                {
                    group.Dropdown("writer.hf-edit-header", "Edit Header", BuildHeaderMenu(), control => control with
                    {
                        Icon = ProfiledIcon(RibbonCommandIconKind.Header),
                    });
                    group.Dropdown("writer.hf-edit-footer", "Edit Footer", BuildFooterMenu(), control => control with
                    {
                        Icon = ProfiledIcon(RibbonCommandIconKind.Footer),
                    });
                });
                tab.Group("hf-insert", "Insert", "I", 110, group =>
                {
                    group.Dropdown("writer.hf-insert-page-number", "Page Number", BuildPageNumberMenu(), control => control with
                    {
                        Icon = ProfiledIcon(RibbonCommandIconKind.PageNumber),
                    });
                    AddProfiledButton(group, avalonia, "writer.hf-insert-datetime", "Date && Time",
                        RibbonCommandIconKind.Date);
                    AddProfiledButton(group, avalonia, "writer.hf-insert-field", "Document Info",
                        RibbonCommandIconKind.Field);
                });
                tab.Group("hf-navigation", "Navigation", "N", 100, group =>
                {
                    AddProfiledButton(group, avalonia, "writer.hf-go-to-header", "Go to Header",
                        RibbonCommandIconKind.Header);
                    AddProfiledButton(group, avalonia, "writer.hf-go-to-footer", "Go to Footer",
                        RibbonCommandIconKind.Footer);
                });
                tab.Group("hf-options", "Options", "O", 90, group =>
                {
                    AddHeaderFooterOption(group, avalonia, "writer.hf-different-first-page",
                        "Different First Page", RibbonCommandIconKind.CoverPage);
                    AddHeaderFooterOption(group, avalonia, "writer.hf-different-odd-even",
                        "Different Odd && Even Pages", RibbonCommandIconKind.OnePage);
                });
                tab.Group("hf-position", "Position", "P", 80, group =>
                {
                    AddPositionCombo(group, "writer.hf-header-from-top", "Header from Top");
                    AddPositionCombo(group, "writer.hf-footer-from-bottom", "Footer from Bottom");
                });
                tab.Group("hf-close", "Close", "C", 70, group =>
                AddProfiledButton(group, avalonia, "writer.hf-close", "Close Header and Footer",
                    RibbonCommandIconKind.WindowClose));
            });
    }

    private static void BuildWpfThemeMenu(string commandId, RibbonMenuBuilder menu)
    {
        foreach (var theme in DocumentTheme.Catalog)
            menu.Item(commandId, theme.Name, theme.Name[0].ToString());
        menu.Separator();
        menu.Item("writer.customize-colors", "Customize Colors\u2026", "Z");
    }

    private static void BuildWpfFontSetMenu(string commandId, RibbonMenuBuilder menu)
    {
        foreach (var fontSet in DocumentFontSet.Catalog)
            menu.Item(commandId, fontSet.Name, fontSet.Name[0].ToString());
        menu.Separator();
        menu.Item("writer.customize-fonts", "Customize Fonts\u2026", "Z");
    }

    private static void BuildWpfParagraphSpacingMenu(string commandId, RibbonMenuBuilder menu)
    {
        foreach (var spacingSet in DocumentParagraphSpacingSet.Catalog)
            menu.Item(commandId, spacingSet.Name, spacingSet.Name[0].ToString());
        menu.Separator();
        menu.Item("writer.custom-paragraph-spacing", "Custom Paragraph Spacing\u2026", "U");
    }

    private static void BuildWpfEffectsMenu(string commandId, RibbonMenuBuilder menu)
    {
        foreach (var effectSet in DocumentEffectSet.Catalog)
            menu.Item(commandId, effectSet.Name, effectSet.Name[0].ToString());
    }

    private static RibbonMenu BuildAvaloniaMarginsMenu() => new(
    [
        new("Normal", new RibbonCommandId("writer.page-margins-normal")),
        new("Narrow", new RibbonCommandId("writer.page-margins-narrow")),
        new("Wide", new RibbonCommandId("writer.page-margins-wide")),
        RibbonMenuItem.Separator(),
        new("Custom Margins...", new RibbonCommandId("writer.custom-margins")),
    ]);

    private static RibbonMenu BuildAvaloniaPageSizeMenu() => new(
    [
        new("Letter", new RibbonCommandId("writer.page-size-letter")),
        new("A4", new RibbonCommandId("writer.page-size-a4")),
        RibbonMenuItem.Separator(),
        new("More Paper Sizes...", new RibbonCommandId("writer.more-paper-sizes")),
    ]);

    private static RibbonMenu BuildAvaloniaColumnsMenu() => new(
    [
        new("One", new RibbonCommandId("writer.columns-one")),
        new("Two", new RibbonCommandId("writer.columns-two")),
        new("Three", new RibbonCommandId("writer.columns-three")),
        new("Left", new RibbonCommandId("writer.columns-left")),
        new("Right", new RibbonCommandId("writer.columns-right")),
        RibbonMenuItem.Separator(),
        new("More Columns...", new RibbonCommandId("writer.columns-more")),
    ]);

    private static RibbonMenu BuildAvaloniaBreaksMenu() => new(
    [
        new("Page Break", new RibbonCommandId("writer.page-break")),
        new("Column Break", new RibbonCommandId("writer.column-break")),
        RibbonMenuItem.Separator(),
        new("Next Page", new RibbonCommandId("writer.section-break-next-page")),
        new("Continuous", new RibbonCommandId("writer.section-break-continuous")),
        new("Even Page", new RibbonCommandId("writer.section-break-even-page")),
        new("Odd Page", new RibbonCommandId("writer.section-break-odd-page")),
    ]);

    private static RibbonMenu BuildAvaloniaLineNumbersMenu() => new(
    [
        new("None", new RibbonCommandId("writer.line-numbers-none")),
        new("Continuous", new RibbonCommandId("writer.line-numbers-continuous")),
        new("Restart Each Page", new RibbonCommandId("writer.line-numbers-restart-page")),
        new("Restart Each Section", new RibbonCommandId("writer.line-numbers-restart-section")),
        RibbonMenuItem.Separator(),
        new("Line Numbering Options...", new RibbonCommandId("writer.line-numbers-options")),
    ]);

    private static RibbonMenu BuildAvaloniaHyphenationMenu() => new(
    [
        new("None", new RibbonCommandId("writer.hyphenation-none")),
        new("Automatic", new RibbonCommandId("writer.hyphenation-auto")),
        new("Manual", new RibbonCommandId("writer.hyphenation-manual")),
        RibbonMenuItem.Separator(),
        new("Hyphenation Options...", new RibbonCommandId("writer.hyphenation-options")),
    ]);

    private static RibbonMenu BuildAvaloniaThemeMenu() => new(
        DocumentTheme.Catalog
            .Select(theme => new RibbonMenuItem(theme.Name,
                new RibbonCommandId($"writer.theme.{theme.Name.ToLowerInvariant()}")))
            .ToArray());

    private static RibbonMenu BuildAvaloniaThemeColorsMenu() => new(
        DocumentTheme.Catalog
            .Select(theme => new RibbonMenuItem(theme.Name,
                new RibbonCommandId($"writer.theme-colors.{theme.Name.ToLowerInvariant()}")))
            .Concat([RibbonMenuItem.Separator(),
                new RibbonMenuItem("Customize Colors...", new RibbonCommandId("writer.customize-colors"))])
            .ToArray());

    private static RibbonMenu BuildAvaloniaStyleSetsMenu() => new(
        DocumentStyleSet.Catalog
            .Select(styleSet => new RibbonMenuItem(
                styleSet.Name,
                new RibbonCommandId(DesignRibbonWorkflow.StyleSetCommandId(styleSet.Name))))
            .ToArray());

    private static RibbonMenu BuildAvaloniaThemeFontsMenu() => new(
        DocumentFontSet.Catalog
            .Select(fontSet => new RibbonMenuItem(
                $"{fontSet.Name}  ({fontSet.HeadingFont} / {fontSet.BodyFont})",
                new RibbonCommandId($"writer.theme-fonts.{fontSet.Name.ToLowerInvariant()}")))
            .Concat([RibbonMenuItem.Separator(),
                new RibbonMenuItem("Customize Fonts...", new RibbonCommandId("writer.customize-fonts"))])
            .ToArray());

    private static RibbonMenu BuildAvaloniaParagraphSpacingMenu() => new(
        DocumentParagraphSpacingSet.Catalog
            .Select(spacingSet => new RibbonMenuItem(spacingSet.Name,
                new RibbonCommandId(DesignRibbonWorkflow.ParagraphSpacingCommandId(spacingSet.Name))))
            .Concat([
                RibbonMenuItem.Separator(),
                new RibbonMenuItem("Custom Paragraph Spacing...",
                    new RibbonCommandId("writer.custom-paragraph-spacing")),
            ])
            .ToArray());

    private static RibbonMenu BuildAvaloniaPageColorMenu() => new(
        WriterRibbonDefinitionData.PageColors
            .Select(color => new RibbonMenuItem(color.Label, new RibbonCommandId(color.CommandId)))
            .ToArray());

    private static RibbonMenu BuildAvaloniaWatermarkMenu() => new(
    [
        new("CONFIDENTIAL", new RibbonCommandId("writer.watermark.confidential")),
        new("DO NOT COPY", new RibbonCommandId("writer.watermark.do-not-copy")),
        new("DRAFT", new RibbonCommandId("writer.watermark.draft")),
        new("URGENT", new RibbonCommandId("writer.watermark.urgent")),
        RibbonMenuItem.Separator(),
        new("Custom Watermark\u2026", new RibbonCommandId("writer.watermark.custom")),
        new("Remove Watermark", new RibbonCommandId("writer.watermark.none")),
    ]);

    private static RibbonMenu BuildAvaloniaReadModeMenu() => new(
    [
        new("Narrow Column Width", new RibbonCommandId("writer.read-mode-column-narrow")),
        new("Default Column Width", new RibbonCommandId("writer.read-mode-column-default")),
        new("Wide Column Width", new RibbonCommandId("writer.read-mode-column-wide")),
        RibbonMenuItem.Separator(),
        new("No Color", new RibbonCommandId("writer.read-mode-color-none")),
        new("Sepia", new RibbonCommandId("writer.read-mode-color-sepia")),
        new("Inverse (Dark Mode)", new RibbonCommandId("writer.read-mode-color-inverse")),
    ]);

    private static void AddProfiledButton(
        RibbonGroupBuilder group,
        bool avalonia,
        string commandId,
        string label,
        RibbonCommandIconKind wpfIcon,
        string? wpfKeyTip = null,
        RibbonCommandLayoutKind wpfLayout = RibbonCommandLayoutKind.Medium,
        string? avaloniaLabel = null,
        RibbonCommandIconAccent wpfAccent = RibbonCommandIconAccent.None)
    {
        group.Button(commandId, avalonia ? avaloniaLabel ?? label : label, control => control with
        {
            Icon = ProfiledIcon(wpfIcon, wpfAccent),
            KeyTip = wpfKeyTip,
            PreferredLayout = avalonia ? RibbonCommandLayoutKind.Medium : wpfLayout,
        });
    }

    private static void AddHelpButton(
        RibbonGroupBuilder group,
        bool avalonia,
        string commandId,
        string label,
        RibbonCommandIconKind icon,
        string keyTip)
    {
        group.Button(commandId, label, control => control with
        {
            Icon = new RibbonCommandIcon(icon, RibbonCommandIconAccent.Help),
            KeyTip = keyTip,
            PreferredLayout = avalonia ? RibbonCommandLayoutKind.Medium : RibbonCommandLayoutKind.Large,
        });
    }

    private static void AddSharedIconButton(
        RibbonGroupBuilder group,
        string commandId,
        string label,
        RibbonCommandIconKind icon)
    {
        group.Button(commandId, label, control => control with
        {
            Icon = new RibbonCommandIcon(icon),
            PreferredLayout = RibbonCommandLayoutKind.Medium,
        });
    }

    private static void AddHeaderFooterOption(
        RibbonGroupBuilder group,
        bool avalonia,
        string commandId,
        string label,
        RibbonCommandIconKind wpfIcon)
    {
        if (avalonia)
        {
            group.Toggle(commandId, label);
            return;
        }

        AddProfiledButton(group, avalonia: false, commandId, label, wpfIcon);
    }

    private static void AddPositionCombo(RibbonGroupBuilder group, string commandId, string label)
    {
        group.ComboBox(commandId, label, control => control with
        {
            Items = ["0", "18", "36", "54", "72"],
            Width = 80,
        });
    }

    private static RibbonMenu BuildStartMailMergeMenu() => new(
    [
        Item("Letters", "writer.start-mail-merge-letters", "L"),
        Item("Directory", "writer.start-mail-merge-directory", "D"),
        RibbonMenuItem.Separator(),
        Item("Normal Word Document", "writer.start-mail-merge-normal", "N"),
    ]);

    private static RibbonMenu BuildMergeRulesMenu() => new(
    [
        Item("If\u2026Then\u2026Else", "writer.merge-rule-if", "I"),
        RibbonMenuItem.Separator(),
        Item("Skip Record If", "writer.merge-rule-skip-record-if", "K"),
        Item("Next Record If", "writer.merge-rule-next-record-if", "X"),
        RibbonMenuItem.Separator(),
        Item("Next Record", "writer.merge-next-record", "N"),
        Item("Merge Record #", "writer.merge-record-number", "R"),
        Item("Merge Sequence #", "writer.merge-sequence-number", "Q"),
        RibbonMenuItem.Separator(),
        Item("Fill-in", "writer.merge-rule-fill-in", "L"),
        Item("Ask", "writer.merge-rule-ask", "A"),
        RibbonMenuItem.Separator(),
        Item("Set Bookmark", "writer.merge-rule-set", "B"),
        Item("Ref Bookmark", "writer.merge-rule-ref", "E"),
    ]);

    private static RibbonMenu BuildHeaderMenu() => new(
    [
        Item("Default Header", "writer.hf-edit-header", "H"),
        Item("First-Page Header", "writer.hf-edit-first-header", "F"),
        Item("Even-Page Header", "writer.hf-edit-even-header", "E"),
    ]);

    private static RibbonMenu BuildFooterMenu() => new(
    [
        Item("Default Footer", "writer.hf-edit-footer", "O"),
        Item("First-Page Footer", "writer.hf-edit-first-footer", "I"),
        Item("Even-Page Footer", "writer.hf-edit-even-footer", "V"),
    ]);

    private static RibbonMenu BuildPageNumberMenu() => new(
    [
        Item("In Header", "writer.hf-insert-page-number", "H"),
        Item("In Footer", "writer.hf-insert-page-number-footer", "F"),
    ]);

    private static RibbonMenuItem Item(
        string header,
        string commandId,
        string wpfKeyTip) =>
        new(header, new RibbonCommandId(commandId), wpfKeyTip);

    private static RibbonCommandIcon ProfiledIcon(
        RibbonCommandIconKind kind,
        RibbonCommandIconAccent accent = RibbonCommandIconAccent.None) =>
        new(kind, accent);

}
