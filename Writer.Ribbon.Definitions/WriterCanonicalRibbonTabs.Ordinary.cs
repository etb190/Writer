using Writer.Shared.Ribbon;
using Writer.App.Presentation.Ribbon;
using Writer.Core.Model;

namespace Writer.Ribbon.Definitions;

internal static partial class WriterCanonicalRibbonTabs
{
    internal static RibbonDefinitionBuilder AddHomeTab(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities)
    {
        var homeTab = WriterRibbonText.HomeTab;
        var clipboardGroup = WriterRibbonText.ClipboardGroup;
        var pasteCommand = WriterRibbonText.PasteCommand;
        var cutCommand = WriterRibbonText.CutCommand;
        var copyCommand = WriterRibbonText.CopyCommand;
        var formatPainterCommand = WriterRibbonText.FormatPainterCommand;
        var pasteTextOnlyCommand = WriterRibbonText.PasteTextOnlyCommand;
        var pasteMergeFormattingCommand = WriterRibbonText.PasteMergeFormattingCommand;
        var pasteSpecialCommand = WriterRibbonText.PasteSpecialCommand;
        var fontGroup = WriterRibbonText.FontGroup;
        var fontFamilyCommand = WriterRibbonText.FontFamilyCommand;
        var fontSizeCommand = WriterRibbonText.FontSizeCommand;
        var boldCommand = WriterRibbonText.BoldCommand;
        var italicCommand = WriterRibbonText.ItalicCommand;
        var underlineCommand = WriterRibbonText.UnderlineCommand;
        var strikethroughCommand = WriterRibbonText.StrikethroughCommand;
        var growFontCommand = WriterRibbonText.GrowFontCommand;
        var shrinkFontCommand = WriterRibbonText.ShrinkFontCommand;
        var subscriptCommand = WriterRibbonText.SubscriptCommand;
        var superscriptCommand = WriterRibbonText.SuperscriptCommand;
        var changeCaseCommand = WriterRibbonText.ChangeCaseCommand;
        var smallCapsCommand = WriterRibbonText.SmallCapsCommand;
        var allCapsCommand = WriterRibbonText.AllCapsCommand;
        var textHighlightColorCommand = WriterRibbonText.TextHighlightColorCommand;
        var fontColorCommand = WriterRibbonText.FontColorCommand;
        var characterBorderCommand = WriterRibbonText.CharacterBorderCommand;
        var characterShadingCommand = WriterRibbonText.CharacterShadingCommand;
        var clearAllFormattingCommand = WriterRibbonText.ClearAllFormattingCommand;
        var fontDialogCommand = WriterRibbonText.FontDialogCommand;
        var paragraphGroup = WriterRibbonText.ParagraphGroup;
        var bulletsCommand = WriterRibbonText.BulletsCommand;
        var numberingCommand = WriterRibbonText.NumberingCommand;
        var multilevelListCommand = WriterRibbonText.MultilevelListCommand;
        var multilevelPromoteCommand = WriterRibbonText.MultilevelPromoteCommand;
        var multilevelDemoteCommand = WriterRibbonText.MultilevelDemoteCommand;
        var multilevelDefineCommand = WriterRibbonText.MultilevelDefineCommand;
        var symbolsGroup = WriterRibbonText.SymbolsGroup;
        var symbolCommand = WriterRibbonText.SymbolCommand;
        return builder.Tab("home", homeTab.Label, homeTab.KeyTip, tab =>
        {
            var topology = new WriterRibbonTabTopology(tab, capabilities);

            topology.Section(
                "home.clipboard",
                tab => tab.Group("clipboard", clipboardGroup.Label, clipboardGroup.KeyTip, 100, g =>
                    {
                        // Paste is the hero (Large); the rest stack as labelled medium buttons, like Word.
                        g.Large("writer.paste", pasteCommand.Label, RibbonCommandIconKind.Paste, pasteCommand.KeyTip, menu: m =>
                        {
                            m.Item("writer.paste-plain", pasteTextOnlyCommand.Label, pasteTextOnlyCommand.KeyTip);
                            m.Item("writer.paste-merge", pasteMergeFormattingCommand.Label, pasteMergeFormattingCommand.KeyTip);
                            m.Item("writer.paste-special", pasteSpecialCommand.Label, pasteSpecialCommand.KeyTip);
                        });
                        // Keep the secondary clipboard actions in Word's narrow icon lane. Their command
                        // IDs, keytips, and tooltips remain unchanged, while the compact lane leaves room
                        // for the Home Styles gallery at a 1280-DIP window.
                        g.Icon("writer.cut", cutCommand.Label, RibbonCommandIconKind.Cut, cutCommand.KeyTip);
                        g.Icon("writer.copy", copyCommand.Label, RibbonCommandIconKind.Copy, copyCommand.KeyTip);
                        g.Icon("writer.format-painter", formatPainterCommand.Label, RibbonCommandIconKind.FormatPainter, formatPainterCommand.KeyTip);
                        // Paste variants live in the Paste split-button menu, as they do in Word, rather
                        // than consuming a second compact column beside the primary Paste action.
                    }),
                tab => tab.Group("clipboard", WriterRibbonText.ClipboardGroup.Label, WriterRibbonText.ClipboardGroup.KeyTip, 100, g =>
                    {
                        g.Button("writer.cut", WriterRibbonText.CutCommand.Label, b => b with
                        {
                            KeyTip = WriterRibbonText.CutCommand.KeyTip
                        });
                        g.Button("writer.copy", WriterRibbonText.CopyCommand.Label, b => b with
                        {
                            KeyTip = WriterRibbonText.CopyCommand.KeyTip
                        });
                        g.Button("writer.paste", WriterRibbonText.PasteCommand.Label, b => b with
                        {
                            KeyTip = WriterRibbonText.PasteCommand.KeyTip
                        });
                        g.Button("writer.format-painter", WriterRibbonText.FormatPainterCommand.Label, b => b with
                        {
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.FormatPainter),
                            KeyTip = WriterRibbonText.FormatPainterCommand.KeyTip
                        });
                        g.Icon("writer.paste-plain", WriterRibbonText.PasteTextOnlyCommand.Label, RibbonCommandIconKind.Paste);
                        g.Icon("writer.paste-merge", WriterRibbonText.PasteMergeFormattingCommand.Label, RibbonCommandIconKind.Paste);
                        g.Icon("writer.paste-special", WriterRibbonText.PasteSpecialCommand.Label, RibbonCommandIconKind.Paste);
                    }));

            topology.Section(
                "home.font",
                tab => tab.Group("font", fontGroup.Label, fontGroup.KeyTip, 90, g =>
                    {
                        g.Sizing(RibbonGroupSizing.OfficeIconAdaptive);
                        // Row 1: the font name + size combos. Row 2+: compact icon-only buttons, exactly like Word.
                        g.ComboBox("writer.font-family", fontFamilyCommand.Label, c => c with
                        {
                            Items = new[] { "Calibri", "Arial", "Times New Roman", "Georgia", "Consolas", "Verdana", "Cambria" },
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Font),
                            Width = 140
                        });
                        g.ComboBox("writer.font-size", fontSizeCommand.Label, c => c with
                        {
                            Items = new[] { "8", "9", "10", "11", "12", "14", "16", "18", "24", "28", "36", "48", "72" },
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Font),
                            Width = 56
                        });
                        g.Icon("writer.grow-font", growFontCommand.Label, RibbonCommandIconKind.ArrowUp);
                        g.Icon("writer.shrink-font", shrinkFontCommand.Label, RibbonCommandIconKind.ArrowDown);
                        g.RowBreak();
                        g.IconToggle("writer.bold", boldCommand.Label, RibbonCommandIconKind.Bold, boldCommand.KeyTip);
                        g.IconToggle("writer.italic", italicCommand.Label, RibbonCommandIconKind.Italic, italicCommand.KeyTip);
                        g.IconToggle("writer.underline", underlineCommand.Label, RibbonCommandIconKind.Underline, underlineCommand.KeyTip);
                        g.Icon("writer.strikethrough", strikethroughCommand.Label, RibbonCommandIconKind.Strikethrough);
                        g.Icon("writer.subscript", subscriptCommand.Label, RibbonCommandIconKind.Subscript);
                        g.Icon("writer.superscript", superscriptCommand.Label, RibbonCommandIconKind.Superscript);
                        g.Icon("writer.change-case", changeCaseCommand.Label, RibbonCommandIconKind.ChangeCase);
                        g.RowBreak();
                        g.Icon("writer.smallcaps", smallCapsCommand.Label, RibbonCommandIconKind.Font);
                        g.Icon("writer.allcaps", allCapsCommand.Label, RibbonCommandIconKind.Font);
                        g.Icon("writer.highlight", textHighlightColorCommand.Label, RibbonCommandIconKind.Highlight);
                        g.Icon("writer.font-color", fontColorCommand.Label, RibbonCommandIconKind.FontColor);
                        g.Icon("writer.char-border", characterBorderCommand.Label, RibbonCommandIconKind.Border);
                        g.Icon("writer.char-shading", characterShadingCommand.Label, RibbonCommandIconKind.Fill);
                        g.Icon("writer.clear-formatting", clearAllFormattingCommand.Label, RibbonCommandIconKind.Clear);
                        // Font dialog-launcher: opens the two-tab Font dialog (Font + Advanced tab with
                        // character spacing, kerning, position, ligatures, stylistic sets, number form/spacing).
                        g.DialogLauncher("writer.font-dialog", fontDialogCommand.Label, "Open Font settings.");
                    }),
                tab => tab.Group("font", WriterRibbonText.FontGroup.Label, WriterRibbonText.FontGroup.KeyTip, 90, g =>
                    {
                        g.Sizing(RibbonGroupSizing.OfficeIconAdaptive);
                        g.ComboBox("writer.font-family", WriterRibbonText.FontFamilyCommand.Label, c => c with { Items = FontFamilies, Width = 128 });
                        g.ComboBox("writer.font-size", WriterRibbonText.FontSizeCommand.Label, c => c with { Items = FontSizes, Width = 64 });
                        g.Toggle("writer.bold", WriterRibbonText.BoldCommand.Label, b => b with { KeyTip = WriterRibbonText.BoldCommand.KeyTip });
                        g.Toggle("writer.italic", WriterRibbonText.ItalicCommand.Label, b => b with { KeyTip = WriterRibbonText.ItalicCommand.KeyTip });
                        g.Toggle("writer.underline", WriterRibbonText.UnderlineCommand.Label, b => b with { KeyTip = WriterRibbonText.UnderlineCommand.KeyTip });
                        g.Toggle("writer.strikethrough", WriterRibbonText.StrikethroughCommand.Label);
                        g.Toggle("writer.superscript", WriterRibbonText.SuperscriptCompactCommand.Label);
                        g.Toggle("writer.subscript", WriterRibbonText.SubscriptCompactCommand.Label);
                        g.Toggle("writer.smallcaps", WriterRibbonText.SmallCapsCommand.Label);
                        g.Toggle("writer.allcaps", WriterRibbonText.AllCapsCommand.Label);
                        g.Dropdown("writer.highlight", WriterRibbonText.HighlightCompactCommand.Label, BuildHighlightMenu(), d => d with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Highlight)
                        });
                        g.Dropdown("writer.char-border", WriterRibbonText.CharacterBorderCommand.Label, BuildCharacterBorderMenu(), d => d with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Border)
                        });
                        g.Dropdown("writer.char-shading", WriterRibbonText.CharacterShadingCommand.Label, BuildCharacterShadingMenu(), d => d with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Fill)
                        });
                        g.Button("writer.grow-font", WriterRibbonText.GrowFontCompactCommand.Label);
                        g.Button("writer.shrink-font", WriterRibbonText.ShrinkFontCompactCommand.Label);
                        g.Button("writer.clear-formatting", WriterRibbonText.ClearFormattingCompactCommand.Label);
                        g.Dropdown("writer.font-color", WriterRibbonText.FontColorDropdownCommand.Label, BuildFontColorMenu());
                        g.Button("writer.change-case", WriterRibbonText.ChangeCaseCompactCommand.Label);
                        g.DialogLauncher("writer.font-dialog", WriterRibbonText.FontDialogCommand.Label, "Open Font settings.");
                    }));

            topology.Section(
                "home.paragraph",
                tab => tab.Group("paragraph", paragraphGroup.Label, paragraphGroup.KeyTip, 80, g =>
                    {
                        g.Sizing(RibbonGroupSizing.OfficeIconAdaptive);
                        // Row 1: list + indent + spacing. Row 2: alignment + shading/borders. Compact icon-only, Word-style.
                        g.Icon("writer.bullets", bulletsCommand.Label, RibbonCommandIconKind.Bullets, dropdown: true);
                        g.Icon("writer.numbering", numberingCommand.Label, RibbonCommandIconKind.NumberedList, dropdown: true);
                        g.Icon("writer.multilevel-list", multilevelListCommand.Label, RibbonCommandIconKind.MultilevelList, dropdown: true, menu: m =>
                        {
                            m.Item("writer.multilevel-promote", multilevelPromoteCommand.Label, multilevelPromoteCommand.KeyTip);
                            m.Item("writer.multilevel-demote", multilevelDemoteCommand.Label, multilevelDemoteCommand.KeyTip);
                            // Predefined multilevel list presets (mirrors Word's gallery of 3 presets).
                            foreach (var (preset, idx) in WriterRibbonDefinitionData.MultilevelListPresetNames.Select((p, i) => (p, i)))
                                m.Item($"writer.multilevel-preset-{idx}", preset, (idx + 1).ToString());
                            // Define New Multilevel List: opens a dialog to configure levels and start-at.
                            m.Item("writer.multilevel-define", multilevelDefineCommand.Label, multilevelDefineCommand.KeyTip);
                        });
                        g.Icon("writer.indent-decrease", "Decrease Indent", RibbonCommandIconKind.IndentDecrease);
                        g.Icon("writer.indent-increase", "Increase Indent", RibbonCommandIconKind.IndentIncrease);
                        g.RowBreak();
                        g.Icon("writer.align-left", "Align Left", RibbonCommandIconKind.AlignLeft);
                        g.Icon("writer.align-center", "Center", RibbonCommandIconKind.AlignCenter);
                        g.Icon("writer.align-right", "Align Right", RibbonCommandIconKind.AlignRight);
                        g.Icon("writer.align-justify", "Justify", RibbonCommandIconKind.AlignJustify);
                        g.Icon("writer.sort", "Sort", RibbonCommandIconKind.Sort);
                        g.IconToggle("writer.formatting-marks", "Show ¶", RibbonCommandIconKind.FormattingMarks);
                        g.ComboBox("writer.line-spacing", "Line and Paragraph Spacing", c => c with
                        {
                            Items = new[] { "1.0", "1.15", "1.5", "2.0" },
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.LineSpacing),
                            Width = 52
                        });
                        g.Icon("writer.para-shading", "Shading", RibbonCommandIconKind.Fill);
                        g.Icon("writer.para-border", "Borders", RibbonCommandIconKind.Border);
                        g.Icon("writer.borders-shading", "Borders and Shading…", RibbonCommandIconKind.Border, accent: RibbonCommandIconAccent.Border);
                        g.Icon("writer.space-before-toggle", "Add Space Before Paragraph", RibbonCommandIconKind.SpaceBefore);
                        g.Icon("writer.space-after-toggle", "Add Space After Paragraph", RibbonCommandIconKind.SpaceAfter);
                        g.DialogLauncher("writer.paragraph-dialog", "Paragraph Settings", "Open Paragraph settings.");
                        g.Icon("writer.tabs-dialog", "Tabs", RibbonCommandIconKind.Ruler);
                        g.Icon("writer.keep-with-next", "Keep with Next", RibbonCommandIconKind.TextFunction);
                        g.Icon("writer.keep-lines", "Keep Lines Together", RibbonCommandIconKind.TextFunction);
                        g.Icon("writer.widow-control", "Widow/Orphan Control", RibbonCommandIconKind.TextFunction);
                    }),
                tab => tab.Group("paragraph", WriterRibbonText.ParagraphGroup.Label, null, 80, g =>
                    {
                        g.Sizing(RibbonGroupSizing.OfficeIconAdaptive);
                        g.Toggle("writer.bullets", WriterRibbonText.BulletsCommand.Label);
                        g.Toggle("writer.numbering", WriterRibbonText.NumberingCommand.Label);
                        g.Dropdown("writer.multilevel-list", WriterRibbonText.MultilevelListCommand.Label, BuildMultilevelListMenu(), d => d with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.MultilevelList)
                        });
                        g.Button("writer.indent-decrease", "Decrease Indent", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.IndentDecrease)
                        });
                        g.Button("writer.indent-increase", "Increase Indent", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.IndentIncrease)
                        });
                        g.Button("writer.align-left", "Left");
                        g.Button("writer.align-center", "Center");
                        g.Button("writer.align-right", "Right");
                        g.Button("writer.align-justify", "Justify");
                        g.Button("writer.sort", "Sort", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Sort)
                        });
                        g.ComboBox("writer.line-spacing", "Line and Paragraph Spacing", c => c with
                        {
                            Items = new[] { "1.0", "1.15", "1.5", "2.0" },
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.LineSpacing),
                            Width = 52
                        });
                        g.Dropdown("writer.para-shading", "Shading", BuildParagraphShadingMenu(), d => d with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Fill)
                        });
                        g.Button("writer.para-border", "Borders", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Border)
                        });
                        g.Button("writer.borders-shading", "Borders and Shading...", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Border, RibbonCommandIconAccent.Border)
                        });
                        g.Button("writer.space-before-toggle", "Add Space Before Paragraph", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.SpaceBefore)
                        });
                        g.Button("writer.space-after-toggle", "Add Space After Paragraph", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.SpaceAfter)
                        });
                        g.Button("writer.keep-with-next", "Keep with Next", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.TextFunction)
                        });
                        g.Button("writer.keep-lines", "Keep Lines Together", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.TextFunction)
                        });
                        g.Button("writer.widow-control", "Widow/Orphan Control", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.TextFunction)
                        });
                        g.Button("writer.tabs-dialog", "Tabs", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Ruler)
                        });
                        g.Toggle("writer.formatting-marks", "Show Formatting Marks", t => t with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.FormattingMarks)
                        });
                        g.DialogLauncher("writer.paragraph-dialog", "Paragraph Settings", "Open Paragraph settings.");
                    }));

            topology.Section(
                "home.styles",
                // Keep the visible quick-style strip ahead of the Editing overflow at desktop widths.
                // This matches Word's Home topology: the gallery remains in the ribbon while Find/Replace
                // are still available from the compact Editing group.
                tab => tab.Group("styles", "Styles", "S", 76, g =>
                    {
                        g.ComboBox("writer.style", "Style", c => c with
                        {
                            Items = new[] { "Normal", "Heading 1", "Heading 2", "Heading 3", "Title", "Subtitle", "Quote" },
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.TextBox),
                            Width = 130
                        });
                        g.Button("writer.style-normal", "Normal", b => Icon(b, RibbonCommandIconKind.TextBox));
                        g.Button("writer.style-heading1", "Heading 1", b => Icon(b, RibbonCommandIconKind.TextBox));
                        g.Button("writer.style-heading2", "Heading 2", b => Icon(b, RibbonCommandIconKind.TextBox));
                        g.Button("writer.style-heading3", "Heading 3", b => Icon(b, RibbonCommandIconKind.TextBox));
                        g.Button("writer.style-title", "Title", b => Icon(b, RibbonCommandIconKind.TextBox));
                        g.Button("writer.style-clear", "Clear Style", b => Icon(b, RibbonCommandIconKind.Clear));
                        g.Button("writer.new-style", "New Style", b => Icon(b, RibbonCommandIconKind.TextBox));
                        g.Button("writer.manage-styles", "Manage Styles", b => Icon(b, RibbonCommandIconKind.TextBox));
                    }),
                tab => tab.Group("styles", "Styles", null, 82, g =>
                    {
                        // Quick-style buttons (kept from the A1 wave; now model-backed via ApplyNamedStyle).
                        g.Button("writer.style-normal", "Normal");
                        g.Button("writer.style-heading1", "Heading 1");
                        g.Button("writer.style-heading2", "Heading 2");
                        g.Button("writer.style-heading3", "Heading 3");
                        g.Button("writer.style-title", "Title");
                        // AV-STYLES: full built-in style gallery dropdown + clear-style.
                        g.Dropdown("writer.styles-gallery", "Styles", BuildStylesMenu());
                        g.Button("writer.style-clear", "Clear Style");
                        g.Button("writer.new-style", "New Style", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Insert)
                        });
                        g.Button("writer.manage-styles", "Manage Styles", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Effects)
                        });
                    }));

            topology.Section(
                "home.formatting",
                tab => tab.Group("formatting", "Formatting", "M", 70, g =>
                    {
                        g.MediumToggle("writer.reveal-formatting", "Reveal Formatting", RibbonCommandIconKind.Info);
                    }));

            topology.Section(
                "home.editing",
                tab => tab.Group("editing", "Editing", "E", 75, g =>
                    {
                        g.Medium("writer.undo", "Undo", RibbonCommandIconKind.Undo);
                        g.Medium("writer.redo", "Redo", RibbonCommandIconKind.Redo);
                        g.Medium("writer.find", "Find", RibbonCommandIconKind.Search, "F");
                        g.Medium("writer.replace", "Replace", RibbonCommandIconKind.Search, "R");
                        g.Medium("writer.select", "Select", RibbonCommandIconKind.Search, "SL");
                    }),
                tab => tab.Group("editing", "Editing", null, 70, g =>
                    {
                        g.Button("writer.undo", "Undo", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Undo)
                        });
                        g.Button("writer.redo", "Redo", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Redo)
                        });
                        g.Button("writer.find", "Find", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Search)
                        });
                        g.Button("writer.replace", "Replace", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Search)
                        });
                        g.Button("writer.select", "Select", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Search)
                        });
                    }));

            topology.Build();
        });
    }

    internal static RibbonDefinitionBuilder AddInsertTab(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities)
    {
        var symbolsGroup = WriterRibbonText.SymbolsGroup;
        var symbolCommand = WriterRibbonText.SymbolCommand;
        return builder.Tab("insert", "Insert", "N", tab =>
        {
            var topology = new WriterRibbonTabTopology(tab, capabilities);

            topology.Section(
                "insert.pages",
                tab => tab.Group("pages", "Pages", "P", 100, g =>
                    {
                        // Word shows the Pages group as labelled icon+label rows — use Medium so the labels read.
                        g.Medium("writer.cover-page", "Cover Page", RibbonCommandIconKind.CoverPage, menu: m =>
                        {
                            m.Item("writer.cover-page-default", "Default", "D");
                            m.Item("writer.cover-page-banded", "Banded", "B");
                            m.Item("writer.cover-page-motion", "Motion", "M");
                        });
                        g.Medium("writer.blank-page", "Blank Page", RibbonCommandIconKind.OnePage);
                        g.Medium("writer.page-break", "Page Break", RibbonCommandIconKind.PageBreak);
                        g.RowBreak();
                        g.Medium("writer.horizontal-rule", "Horizontal Rule", RibbonCommandIconKind.HorizontalRule);
                    }),
                tab => tab.Group("pages", "Pages", null, 100, g =>
                    {
                        // AV-INSERT2: Cover Page (gallery of presets) + Page Break.
                        g.Dropdown("writer.cover-page", "Cover Page", BuildCoverPageMenu());
                        g.Button("writer.blank-page", "Blank Page");
                        g.Button("writer.page-break", "Page Break");
                        g.Button("writer.horizontal-rule", "Horizontal Rule");
                    }));

            topology.Section(
                "insert.tables",
                tab => tab.Group("tables", "Tables", "T", 90, g => g.Large("writer.table", "Table", RibbonCommandIconKind.Table, dropdown: true)),
                tab => tab.Group("tables", "Tables", null, 98, g =>
                    {
                        g.Dropdown("writer.table", "Table…", BuildTableSizeMenu());
                    }));

            topology.Section(
                "insert.illustrations",
                tab => tab.Group("illustrations", "Illustrations", "I", 88, g =>
                    {
                        g.Medium("writer.picture", "Pictures", RibbonCommandIconKind.Picture);
                        // Shapes gallery: a dropdown of the preset shape kinds, each inserting the matching
                        // Shape via DocumentView.InsertShape (the items dispatch their own writer.shape-* ids).
                        g.Medium("writer.shapes", "Shapes", RibbonCommandIconKind.Shapes, "SH", menu: m =>
                        {
                            m.Item("writer.shape-rectangle", "Rectangle", "R");
                            m.Item("writer.shape-rounded", "Rounded Rectangle", "O");
                            m.Item("writer.shape-ellipse", "Ellipse", "E");
                            m.Item("writer.shape-textbox", "Text Box", "T");
                        });
                        g.Medium("writer.smartart", "SmartArt", RibbonCommandIconKind.SmartArt);
                        g.RowBreak();
                        g.Medium("writer.chart", "Chart", RibbonCommandIconKind.ChartColumn, accent: RibbonCommandIconAccent.Chart);
                        // Screenshot gallery: "Screen Clipping" drag-selects a screen region and inserts the
                        // capture as an inline image (same path as Insert Picture). The top-level id only opens
                        // the menu (no direct insert), mirroring the Shapes dropdown above.
                        g.Medium("writer.screenshot", "Screenshot", RibbonCommandIconKind.Picture, "SC", menu: m =>
                        {
                            m.Item("writer.screen-clipping", "Screen Clipping", "C");
                        });
                        // Icons picker: opens a searchable icon library and inserts the chosen icon as a
                        // rasterised InlineImage (same PNG path as Insert Picture / Screen Clipping).
                        g.Medium("writer.insert-icon", "Icons", RibbonCommandIconKind.Icons);
                    }),
                tab => tab.Group("illustrations", "Illustrations", null, 96, g =>
                    {
                        g.Button("writer.picture", "Picture", b => Icon(b, RibbonCommandIconKind.Picture));
                        g.Dropdown("writer.shapes", "Shapes", BuildInsertShapesMenu());
                        g.Button("writer.smartart", "SmartArt");
                        g.Button("writer.chart", "Chart");
                        g.Dropdown("writer.screenshot", "Screenshot", new RibbonMenu(new[]
                        {
                            new RibbonMenuItem("Screen Clipping", new RibbonCommandId("writer.screen-clipping")),
                        }));
                        g.Button("writer.insert-icon", "Icons");
                    }));

            topology.Section(
                "insert.links",
                tab => tab.Group("links", "Links", "K", 70, g =>
                    {
                        g.Medium("writer.hyperlink", "Link", RibbonCommandIconKind.Link);
                        g.Icon("writer.bookmark", "Bookmark", RibbonCommandIconKind.Bookmark);
                        g.Icon("writer.cross-reference", "Cross-reference", RibbonCommandIconKind.CrossReference);
                        g.Icon("writer.edit-hyperlink", "Edit Hyperlink", RibbonCommandIconKind.Link);
                        g.RowBreak();
                        g.Icon("writer.remove-hyperlink", "Remove Hyperlink", RibbonCommandIconKind.Link);
                        g.Icon("writer.hyperlink-tooltip", "ScreenTip", RibbonCommandIconKind.Info);
                        g.Icon("writer.link-bookmark", "Link to Bookmark", RibbonCommandIconKind.Bookmark);
                        g.Icon("writer.bookmark-manager", "Bookmark Manager", RibbonCommandIconKind.Bookmark);
                    }),
                tab => tab.Group("links", "Links", null, 95, g =>
                    {
                        g.Button("writer.hyperlink", "Hyperlink", b => Icon(b, RibbonCommandIconKind.Link));
                        g.Button("writer.edit-hyperlink", "Edit Hyperlink");
                        g.Button("writer.remove-hyperlink", "Remove Hyperlink");
                        g.Button("writer.hyperlink-tooltip", "ScreenTip");
                        g.Button("writer.bookmark", "Bookmark");
                        g.Button("writer.link-bookmark", "Link to Bookmark");
                        g.Button("writer.bookmark-manager", "Bookmark Manager");
                    }));

            topology.Section(
                "insert.comments",
                tab => tab.Group("comments", "Comments", "C", 68, g =>
                    {
                        // Word exposes the existing New Comment action directly on Insert as well
                        // as in Review. Keep that primary entry point at the narrow reference width.
                        g.Large("writer.new-comment", "Comment", RibbonCommandIconKind.Comment);
                    }),
                tab => tab.Group("comments", "Comments", null, 94, g =>
                    {
                        g.Button("writer.new-comment", "Comment", b => Icon(b, RibbonCommandIconKind.Comment));
                    }));

            topology.Section(
                "insert.header-footer",
                tab => tab.Group("header-footer", "Header & Footer", "H", 60, g =>
                    {
                        // Small group -> labelled Medium buttons, Word-style.
                        g.Medium("writer.header", "Header", RibbonCommandIconKind.Header);
                        g.Medium("writer.footer", "Footer", RibbonCommandIconKind.Footer);
                        g.Medium("writer.page-number", "Page Number", RibbonCommandIconKind.PageNumber, menu: m =>
                        {
                            m.Item("writer.page-number-top", "Top of Page", "T");
                            m.Item("writer.page-number-bottom", "Bottom of Page", "B");
                            m.Item("writer.page-number-current", "Current Position", "C");
                            m.Separator();
                            m.Item("writer.page-number-format", "Format Page Numbers…", "F");
                        });
                    }),
                tab => tab.Group("header-footer", "Header & Footer", null, 94, g =>
                    {
                        g.Button("writer.header", "Header");
                        g.Button("writer.footer", "Footer");
                        g.Dropdown("writer.page-number", "Page Number", new RibbonMenu(new[]
                        {
                            new RibbonMenuItem("Top of Page", new RibbonCommandId("writer.page-number-top")),
                            new RibbonMenuItem("Bottom of Page", new RibbonCommandId("writer.page-number-bottom")),
                            new RibbonMenuItem("Current Position", new RibbonCommandId("writer.page-number-current")),
                            RibbonMenuItem.Separator(),
                            new RibbonMenuItem("Format Page Numbers...", new RibbonCommandId("writer.page-number-format")),
                        }));
                    }));

            topology.Section(
                "insert.text",
                tab => tab.Group("text", "Text", "X", 74, g =>
                    {
                        // Text Box gallery: Simple (plain), Sidebar/Banded (accent fill), and Quote (indented
                        // italic) presets — each inserts a pre-styled Shape.TextBox at the caret. The top-level
                        // id falls through to Simple (same as the existing plain text-box insert).
                        g.Large("writer.shape-textbox", "Text Box", RibbonCommandIconKind.TextBox, menu: m =>
                        {
                            m.Item("writer.textbox-simple", "Simple Text Box", "S");
                            m.Item("writer.textbox-sidebar", "Sidebar (Banded)", "B");
                            m.Item("writer.textbox-quote", "Quote", "Q");
                        });
                        // Quick Parts: a dropdown with Document Property sub-items + the existing AutoText entry.
                        g.Icon("writer.insert-quickpart", "Quick Parts", RibbonCommandIconKind.QuickParts, menu: m =>
                        {
                            foreach (var plan in DocumentPropertyFieldPlanner.CommandPlans)
                                m.Item(plan.CommandId, plan.Label, plan.KeyTip);
                            m.Separator();
                            m.Item("writer.field", "Field…", "F");
                            m.Separator();
                            m.Item("writer.save-quickpart", "Save Selection to Quick Part Gallery…", "V");
                            m.Item("writer.building-blocks-organizer", "Building Blocks Organizer…", "B");
                            m.Separator();
                            m.Item("writer.update-fields", "Update Fields", "U");
                            m.Item("writer.toggle-field-codes", "Toggle Field Codes", "G");
                        });
                        g.Icon("writer.insert-file", "Text from File", RibbonCommandIconKind.TextFromFile);
                        g.Icon("writer.wordart", "WordArt", RibbonCommandIconKind.WordArt);
                        g.RowBreak();
                        // Drop Cap: top-level applies the default drop cap; dropdown opens the options dialog.
                        g.Icon("writer.drop-cap", "Drop Cap", RibbonCommandIconKind.DropCap, menu: m =>
                        {
                            m.Item("writer.drop-cap-dropped", "Dropped", "D");
                            m.Item("writer.drop-cap-in-margin", "In Margin", "M");
                            m.Item("writer.drop-cap-none", "None (Remove)", "N");
                            m.Separator();
                            m.Item("writer.drop-cap-options", "Drop Cap Options…", "O");
                        });
                        g.Icon("writer.datetime", "Date & Time", RibbonCommandIconKind.Date);
                        g.Icon("writer.object", "Object", RibbonCommandIconKind.Object);
                    }),
                tab => tab.Group("text", "Text", null, 93, g =>
                    {
                        g.Dropdown("writer.shape-textbox", "Text Box", BuildTextBoxMenu());
                        g.Dropdown("writer.insert-quickpart", "Quick Parts", BuildQuickPartsMenu());
                        g.Dropdown("writer.drop-cap", "Drop Cap", BuildDropCapMenu());
                        g.Button("writer.insert-file", "Text from File");
                        g.Button("writer.wordart", "WordArt");
                        g.Button("writer.datetime", "Date & Time");
                        g.Button("writer.field", "Field", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Field)
                        });
                        g.Button("writer.update-fields", "Update Fields");
                        g.Button("writer.toggle-field-codes", "Toggle Field Codes");
                        g.Button("writer.object", "Object");
                        g.Button("writer.save-quickpart", "Save Selection", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.QuickParts)
                        });
                        g.Button("writer.building-blocks-organizer", "Building Blocks Organizer", b => b with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Small,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.QuickParts)
                        });
                    }));

            topology.Section(
                "insert.symbols",
                tab => tab.Group("symbols", symbolsGroup.Label, symbolsGroup.KeyTip, 50, g =>
                    {
                        // Equation gallery: the top-level id inserts the default sample equation (E = mc^2),
                        // and the dropdown offers Word's common structure presets.
                        g.Medium("writer.equation", "Equation", RibbonCommandIconKind.Equation, menu: m =>
                        {
                            m.Item(EquationPresetCatalog.Get(EquationPresetKind.Fraction).CommandId, "Fraction", "F");
                            m.Item(EquationPresetCatalog.Get(EquationPresetKind.Script).CommandId, "Subscript / Superscript", "S");
                            m.Item(EquationPresetCatalog.Get(EquationPresetKind.Radical).CommandId, "Radical (Square Root)", "R");
                            m.Item(EquationPresetCatalog.Get(EquationPresetKind.NthRoot).CommandId, "Radical (nth Root)", "N");
                            m.Item(EquationPresetCatalog.Get(EquationPresetKind.Integral).CommandId, "Integral", "I");
                            m.Item(EquationPresetCatalog.Get(EquationPresetKind.Summation).CommandId, "Summation", "U");
                            m.Item(EquationPresetCatalog.Get(EquationPresetKind.Product).CommandId, "Product", "P");
                            m.Item(EquationPresetCatalog.Get(EquationPresetKind.Accent).CommandId, "Accent (Hat)", "A");
                            m.Item(EquationPresetCatalog.Get(EquationPresetKind.Bar).CommandId, "Overbar", "O");
                            m.Item(EquationPresetCatalog.Get(EquationPresetKind.Bracket).CommandId, "Bracket", "B");
                            m.Item(EquationPresetCatalog.Get(EquationPresetKind.Matrix).CommandId, "Matrix (2x2)", "M");
                            m.Item(EquationPresetCatalog.Get(EquationPresetKind.Function).CommandId, "Function (sin)", "C");
                            m.Item(EquationPresetCatalog.Get(EquationPresetKind.GroupCharacter).CommandId, "Group (brace)", "G");
                        });
                        g.SplitButton("writer.symbol", symbolCommand.Label, BuildSymbolMenu(), split => split with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Medium,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Symbol),
                        });
                    }),
                tab => tab.Group("symbols", WriterRibbonText.SymbolsGroup.Label, null, 92, g =>
                    {
                        g.SplitButton("writer.symbol", WriterRibbonText.SymbolCommand.Label, BuildSymbolMenu(), split => split with
                        {
                            PreferredLayout = RibbonCommandLayoutKind.Medium,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Symbol),
                        });
                        // AV-INSERT2: Equation — default (E=mc²) opener + a few common OMML presets.
                        g.Dropdown("writer.equation", "Equation", BuildEquationMenu());
                    }));

            topology.Build();
        });
    }

    internal static RibbonDefinitionBuilder AddReferencesTab(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities)
    {
        return builder.Tab("references", "References", "R", tab =>
        {
            var topology = new WriterRibbonTabTopology(tab, capabilities);

            topology.Section(
                "references.contents",
                tab => tab.Group("table-of-contents", "Table of Contents", "T", 100, g =>
                    {
                        g.Medium("writer.toc", "Table of Contents", RibbonCommandIconKind.TableOfContents);
                        g.Medium("writer.toc-add-text", "Add Text", RibbonCommandIconKind.TableOfContents, dropdown: true, menu: m =>
                        {
                            m.Item("writer.toc-addtext-none", "Do Not Show in Table of Contents", "N");
                            m.Separator();
                            m.Item("writer.toc-addtext-level1", "Level 1", "1");
                            m.Item("writer.toc-addtext-level2", "Level 2", "2");
                            m.Item("writer.toc-addtext-level3", "Level 3", "3");
                        });
                        g.Medium("writer.toc-refresh", "Update Table", RibbonCommandIconKind.Refresh);
                    }),
                tab => tab.Group("toc", "Table of Contents", null, 110, g =>
                    {
                        g.Button("writer.toc", "Table of Contents");
                        g.Dropdown("writer.toc-add-text", "Add Text", BuildTableOfContentsAddTextMenu());
                        g.Button("writer.toc-refresh", "Update Table");
                    }));

            topology.Section(
                "references.footnotes",
                tab => tab.Group("footnotes", "Footnotes", "F", 92, g =>
                    {
                        g.Medium("writer.footnote", "Insert Footnote", RibbonCommandIconKind.Footnote);
                        g.Medium("writer.endnote", "Insert Endnote", RibbonCommandIconKind.Endnote);
                        g.Medium("writer.next-footnote", "Next Footnote", RibbonCommandIconKind.Footnote, dropdown: true, menu: m =>
                        {
                            m.Item("writer.next-footnote", "Next Footnote", "N");
                            m.Item("writer.previous-footnote", "Previous Footnote", "P");
                            m.Separator();
                            m.Item("writer.next-endnote", "Next Endnote", "E");
                            m.Item("writer.previous-endnote", "Previous Endnote", "V");
                        });
                        g.Medium("writer.show-notes", "Show Notes", RibbonCommandIconKind.Footnote);
                        g.Medium("writer.footnote-endnote-options", "Footnote/Endnote Options…", RibbonCommandIconKind.Footnote);
                    }),
                tab => tab.Group("footnotes", "Footnotes", null, 100, g =>
                    {
                        g.Button("writer.footnote", "Insert Footnote");
                        g.Button("writer.endnote", "Insert Endnote");
                        g.Button("writer.next-footnote", "Next Footnote");
                        g.Button("writer.previous-footnote", "Previous Footnote");
                        g.Button("writer.next-endnote", "Next Endnote");
                        g.Button("writer.previous-endnote", "Previous Endnote");
                        g.Button("writer.show-notes", "Show Notes");
                        g.Button("writer.footnote-endnote-options", "Footnote/Endnote Options...");
                    }));

            topology.Section(
                "references.citations",
                tab => tab.Group("citations", "Citations & Bibliography", "C", 84, g =>
                    {
                        g.Medium("writer.citation", "Insert Citation", RibbonCommandIconKind.Citation);
                        g.Medium("writer.manage-sources", "Manage Sources", RibbonCommandIconKind.Citation);
                        g.ComboBox("writer.citation-style", "Style", c => c with
                        {
                            Items = WriterRibbonDefinitionData.CitationStyleNames,
                            Icon = new RibbonCommandIcon(RibbonCommandIconKind.Citation),
                            Width = 90
                        });
                        g.Medium("writer.bibliography", "Bibliography", RibbonCommandIconKind.Bibliography);
                    }),
                tab => tab.Group("citations", "Citations & Bibliography", null, 90, g =>
                    {
                        g.Button("writer.citation", "Insert Citation", b => Icon(b, RibbonCommandIconKind.Citation));
                        g.Button("writer.manage-sources", "Manage Sources");
                        g.ComboBox("writer.citation-style", "Style", c => c with
                        {
                            Items = WriterRibbonDefinitionData.CitationStyleNames,
                            Width = 90
                        });
                        g.Button("writer.bibliography", "Bibliography");
                    }));

            topology.Section(
                "references.captions",
                tab => tab.Group("captions", "Captions", "P", 78, g =>
                    {
                        g.Medium("writer.caption", "Insert Caption", RibbonCommandIconKind.Caption, menu: m =>
                        {
                            m.Item("writer.insert-caption.figure", "Figure", "F");
                            m.Item("writer.insert-caption.table", "Table", "T");
                            m.Item("writer.insert-caption.equation", "Equation", "E");
                        });
                        g.Medium("writer.tof", "Insert Table of Figures", RibbonCommandIconKind.TableOfContents, menu: m =>
                        {
                            m.Item("writer.tof.figure", "Figure", "F");
                            m.Item("writer.tof.table", "Table", "T");
                            m.Item("writer.tof.equation", "Equation", "E");
                        });
                        g.Medium("writer.tof-refresh", "Update Table", RibbonCommandIconKind.Refresh);
                        g.Medium("writer.cross-reference", "Cross-reference", RibbonCommandIconKind.CrossReference);
                    }),
                tab => tab.Group("captions", "Captions", null, 80, g =>
                    {
                        g.Dropdown("writer.caption", "Insert Caption", BuildCaptionMenu(), b => b with { Icon = new RibbonCommandIcon(RibbonCommandIconKind.Caption) });
                        g.Dropdown("writer.tof", "Insert Table of Figures", BuildTableOfFiguresMenu());
                        g.Button("writer.tof-refresh", "Update Table");
                        g.Button("writer.cross-reference", "Cross-reference");
                    }));

            topology.Section(
                "references.index",
                tab => tab.Group("index", "Index", "I", 72, g =>
                    {
                        g.Medium("writer.index-mark", "Mark Entry", RibbonCommandIconKind.Index);
                        g.Medium("writer.index-insert", "Insert Index", RibbonCommandIconKind.Index);
                        g.Medium("writer.index-refresh", "Update Index", RibbonCommandIconKind.Refresh);
                    }),
                tab => tab.Group("index", "Index", null, 70, g =>
                    {
                        g.Button("writer.index-mark", "Mark Entry");
                        g.Button("writer.index-insert", "Insert Index");
                        g.Button("writer.index-refresh", "Update Index");
                    }));

            topology.Section(
                "references.authorities",
                tab => tab.Group("authorities", "Table of Authorities", "A", 66, g =>
                    {
                        g.Medium("writer.mark-citation", "Mark Citation", RibbonCommandIconKind.Citation);
                        g.Medium("writer.table-of-authorities", "Insert Table of Authorities", RibbonCommandIconKind.Bibliography);
                        g.Medium("writer.table-of-authorities-refresh", "Update Table", RibbonCommandIconKind.Refresh);
                    }),
                tab => tab.Group("authorities", "Table of Authorities", null, 60, g =>
                    {
                        g.Button("writer.mark-citation", "Mark Citation");
                        g.Button("writer.table-of-authorities", "Insert Table of Authorities");
                        g.Button("writer.table-of-authorities-refresh", "Update Table");
                    }));

            topology.Build();
        });
    }

    internal static RibbonDefinitionBuilder AddReviewTab(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities)
    {
        return builder.Tab("review", "Review", "R", tab =>
        {
            var topology = new WriterRibbonTabTopology(tab, capabilities);

            topology.Section(
                "review.proofing",
                tab => tab.Group("proofing", "Proofing", "P", 100, g =>
                    {
                        // Word Count hero, then the two proofing toggles/commands as labelled Medium rows.
                        g.Large("writer.statistics", "Word Count", RibbonCommandIconKind.WordCount);
                        g.MediumToggle("writer.spellcheck-toggle", "Spelling & Grammar", RibbonCommandIconKind.Spelling);
                        g.Medium("writer.add-to-dictionary", "Add to Dictionary", RibbonCommandIconKind.Book);
                        // Thesaurus (Shift+F7): looks up synonyms for the selected/caret word in the bundled
                        // compact English synonym dictionary (Moby II derivative, ~3 000 headwords, public domain).
                        // Shows senses + synonyms in a docked pane with Insert (replace word) and Copy actions.
                        g.Medium("writer.thesaurus", "Thesaurus", RibbonCommandIconKind.Book, "T");
                        // Set Proofing Language lives in the Proofing group (matching Word's Review tab layout).
                        // It applies a BCP-47 language tag to the selected runs (rPr/w:lang) so the built-in
                        // spell checker uses the correct dictionary per run.
                        g.Medium("writer.set-proofing-language", "Set Proofing Language", RibbonCommandIconKind.Language);
                        g.Sizing(RibbonGroupSizing.OfficeAdaptive);
                    }),
                tab => tab.Group("proofing", "Proofing", null, 110, g =>
                    {
                        g.Button("writer.statistics", "Word Count", b => Icon(b, RibbonCommandIconKind.WordCount));
                        g.Toggle("writer.spellcheck-toggle", "Spelling & Grammar");
                        g.Button("writer.add-to-dictionary", "Add to Dictionary");
                        g.Button("writer.thesaurus", "Thesaurus");
                        g.Button("writer.set-proofing-language", "Set Proofing Language");
                    }));

            topology.Section(
                "review.speech",
                tab => tab.Group("speech", "Speech", "S", 97, g =>
                    {
                        g.MediumToggle("writer.read-aloud", "Read Aloud", RibbonCommandIconKind.ReadAloud);
                        g.Sizing(RibbonGroupSizing.OfficeAdaptive);
                    }),
                tab => tab.Group("speech", "Speech", null, 105, g =>
                    {
                        g.Toggle("writer.read-aloud", "Read Aloud", b => Icon(b, RibbonCommandIconKind.ReadAloud));
                    }));

            topology.Section(
                "review.accessibility",
                tab => tab.Group("accessibility", "Accessibility", "A", 92, g =>
                    {
                        g.Medium("writer.check-accessibility", "Check Accessibility", RibbonCommandIconKind.Accessibility);
                        g.Sizing(RibbonGroupSizing.OfficeAdaptive);
                    }),
                tab => tab.Group("accessibility", "Accessibility", null, 92, g =>
                    {
                        g.Button("writer.check-accessibility", "Check Accessibility", b => Icon(b, RibbonCommandIconKind.Accessibility));
                    }));

            topology.Section(
                "review.comments",
                tab => tab.Group("comments", "Comments", "C", 95, g =>
                    {
                        // Thread actions mirror Word's Review > Comments group and stay labelled at narrow widths.
                        g.Medium("writer.new-comment", "New Comment", RibbonCommandIconKind.Comment);
                        g.Medium("writer.delete-comment", "Delete", RibbonCommandIconKind.Delete);
                        g.Medium("writer.previous-comment", "Previous", RibbonCommandIconKind.Previous);
                        g.Medium("writer.next-comment", "Next", RibbonCommandIconKind.Next);
                        g.RowBreak();
                        g.Medium("writer.reply-comment", "Reply", RibbonCommandIconKind.Comment);
                        g.Medium("writer.resolve-comment", "Resolve", RibbonCommandIconKind.AcceptChange);
                        g.Medium("writer.show-comments", "Show Comments", RibbonCommandIconKind.Comment);
                        g.Sizing(RibbonGroupSizing.OfficeIconAdaptive);
                    }),
                tab => tab.Group("comments", "Comments", null, 100, g =>
                    {
                        g.Button("writer.new-comment", "New Comment", b => Icon(b, RibbonCommandIconKind.Comment));
                        g.Button("writer.delete-comment", "Delete");
                        g.Button("writer.previous-comment", "Previous");
                        g.Button("writer.next-comment", "Next");
                        g.Button("writer.reply-comment", "Reply");
                        g.Button("writer.resolve-comment", "Resolve");
                        g.Button("writer.show-comments", "Show Comments");
                    }));

            topology.Section(
                "review.tracking",
                tab => tab.Group("tracking", "Tracking", "G", 90, g =>
                    {
                        // Track Changes is the big toggle; the Reviewing Pane toggle opens the dockable revisions
                        // list. Accept/Reject live in Changes, mirroring Word's group geography.
                        g.MediumToggle("writer.track-changes", "Track Changes", RibbonCommandIconKind.History);
                        g.MediumToggle("writer.track-formatting", "Track Formatting", RibbonCommandIconKind.History);
                        g.MediumToggle("writer.reviewing-pane", "Reviewing Pane", RibbonCommandIconKind.History);
                        g.RowBreak();
                        // Display for Review: dropdown with All Markup (default), Simple Markup, No Markup,
                        // and Original — Word's order. Simple Markup shows the final form (No Markup inline
                        // path) plus a left-margin change bar beside each changed paragraph.
                        g.Medium("writer.display-for-review", "All Markup", RibbonCommandIconKind.History, "D", menu: m =>
                        {
                            m.Item("writer.display-for-review-all-markup", "All Markup", "A");
                            m.Item("writer.display-for-review-simple-markup", "Simple Markup", "S");
                            m.Item("writer.display-for-review-no-markup", "No Markup", "N");
                            m.Item("writer.display-for-review-original", "Original", "O");
                        });
                        // Show Markup: per-category visibility toggles. Balloons mode renders comments and
                        // tracked-change revisions as right-margin callouts with leader lines, instead of
                        // inline highlights. The BalloonOverlay adorner/panel hosts the balloon strip.
                        g.Medium("writer.show-markup", "Show Markup", RibbonCommandIconKind.History, "M", menu: m =>
                        {
                            m.Item("writer.show-markup-insertions-deletions", "Insertions and Deletions", "I");
                            m.Item("writer.show-markup-comments", "Comments", "C");
                            m.Item("writer.show-markup-formatting", "Formatting", "F");
                            m.Separator();
                            // Balloons: toggle right-margin balloon display mode for comments and revisions.
                            m.Item("writer.show-markup-balloons", "Show Revisions in Balloons", "B");
                        });
                        g.Sizing(RibbonGroupSizing.OfficeIconAdaptive);
                    }),
                tab => tab.Group("tracking", "Tracking", null, 90, g =>
                    {
                        g.Toggle("writer.track-changes", "Track Changes", b => Icon(b, RibbonCommandIconKind.History));
                        g.Toggle("writer.track-formatting", "Track Formatting");
                        g.Toggle("writer.reviewing-pane", "Reviewing Pane");
                        g.Dropdown("writer.display-for-review", "All Markup", BuildDisplayForReviewMenu());
                        g.Dropdown("writer.show-markup", "Show Markup", BuildShowMarkupMenu());
                    }));

            topology.Section(
                "review.changes",
                tab => tab.Group("changes", "Changes", "H", 88, g =>
                    {
                        g.Medium("writer.accept-this", "Accept", RibbonCommandIconKind.AcceptChange, "A", menu: m =>
                        {
                            m.Item("writer.accept-this", "Accept This Change", "A");
                            m.Item("writer.accept-all", "Accept All Changes", "L");
                        });
                        g.Medium("writer.reject-this", "Reject", RibbonCommandIconKind.RejectChange, "J", menu: m =>
                        {
                            m.Item("writer.reject-this", "Reject This Change", "R");
                            m.Item("writer.reject-all", "Reject All Changes", "L");
                        });
                        g.RowBreak();
                        g.Medium("writer.previous-change", "Previous", RibbonCommandIconKind.History);
                        g.Medium("writer.next-change", "Next", RibbonCommandIconKind.History);
                        g.Sizing(RibbonGroupSizing.OfficeIconAdaptive);
                    }),
                tab => tab.Group("changes", "Changes", null, 80, g =>
                    {
                        g.Button("writer.accept-this", "Accept", b => Icon(b, RibbonCommandIconKind.AcceptChange));
                        g.Button("writer.accept-all", "Accept All");
                        g.Button("writer.reject-this", "Reject");
                        g.Button("writer.reject-all", "Reject All");
                        g.Button("writer.previous-change", "Previous");
                        g.Button("writer.next-change", "Next");
                    }));

            topology.Section(
                "review.protect",
                tab => tab.Group("protect", "Protect", "T", 85, g =>
                    {
                        g.MediumToggle("writer.mark-as-final", "Mark as Final", RibbonCommandIconKind.Protect);
                        g.MediumToggle("writer.restrict-editing", "Restrict Editing", RibbonCommandIconKind.Protect);
                        g.Sizing(RibbonGroupSizing.OfficeIconAdaptive);
                    }),
                tab => tab.Group("compare", "Compare", null, 78, g =>
                    {
                        g.Button("writer.compare", "Compare", b => Icon(b, RibbonCommandIconKind.Compare));
                        g.Button("writer.combine", "Combine");
                    }));

            topology.Section(
                "review.compare",
                tab => tab.Group("compare", "Compare", "M", 80, g =>
                    {
                        g.Medium("writer.compare", "Compare", RibbonCommandIconKind.Compare);
                        g.Medium("writer.combine", "Combine", RibbonCommandIconKind.Compare);
                        g.Sizing(RibbonGroupSizing.OfficeIconAdaptive);
                    }),
                tab => tab.Group("protect", "Protect", null, 85, g =>
                    {
                        g.Toggle("writer.mark-as-final", "Mark as Final", b => Icon(b, RibbonCommandIconKind.Protect));
                        g.Toggle("writer.restrict-editing", "Restrict Editing");
                    }));

            topology.Section(
                "review.inspect",
                tab => tab.Group("inspect", "Inspect", "I", 75, g =>
                    {
                        g.Medium("writer.inspect-document", "Inspect Document", RibbonCommandIconKind.Search);
                        g.Sizing(RibbonGroupSizing.OfficeIconAdaptive);
                    }),
                tab => tab.Group("inspect", "Inspect", null, 75, g =>
                    {
                        g.Button("writer.inspect-document", "Inspect Document", b => Icon(b, RibbonCommandIconKind.Search));
                    }));

            topology.Build();
        });
    }

    private static readonly string[] FontSizes = WriterRibbonDefinitionData.FontSizes;
    private static readonly string[] FontFamilies = WriterRibbonDefinitionData.FontFamilies;

    private static RibbonButton Icon(
        RibbonButton button,
        RibbonCommandIconKind kind,
        RibbonCommandIconAccent accent = RibbonCommandIconAccent.None) =>
        button with { Icon = new RibbonCommandIcon(kind, accent) };

    private static RibbonToggleButton Icon(
        RibbonToggleButton button,
        RibbonCommandIconKind kind,
        RibbonCommandIconAccent accent = RibbonCommandIconAccent.None) =>
        button with { Icon = new RibbonCommandIcon(kind, accent) };

    private static RibbonMenu BuildFontColorMenu() =>
        new(WriterRibbonDefinitionData.FontColors
            .Select(fc => new RibbonMenuItem(fc.Label, new RibbonCommandId(fc.CommandId)))
            .ToArray());

    private static RibbonMenu BuildParagraphShadingMenu() =>
        BuildPaletteMenu(WriterRibbonPaletteCatalog.ParagraphShading);

    private static RibbonMenu BuildCharacterShadingMenu() =>
        BuildPaletteMenu(WriterRibbonPaletteCatalog.CharacterShading);

    private static RibbonMenu BuildCharacterBorderMenu() =>
        BuildPaletteMenu(WriterRibbonPaletteCatalog.CharacterBorders);

    private static RibbonMenu BuildHighlightMenu() =>
        BuildPaletteMenu(WriterRibbonPaletteCatalog.Highlights);

    private static RibbonMenu BuildPaletteMenu(IReadOnlyList<WriterRibbonPaletteChoice> choices)
    {
        var items = new List<RibbonMenuItem>(choices.Count + 1);
        foreach (var choice in choices)
        {
            if (choice.StartsNewGroup)
                items.Add(RibbonMenuItem.Separator());
            items.Add(new RibbonMenuItem(choice.Label, new RibbonCommandId(choice.CommandId)));
        }

        return new RibbonMenu(items);
    }

    private static RibbonMenu BuildDisplayForReviewMenu() =>
        new(new RibbonMenuItem[]
        {
            new("All Markup", new RibbonCommandId("writer.display-for-review-all-markup")),
            new("Simple Markup", new RibbonCommandId("writer.display-for-review-simple-markup")),
            new("No Markup", new RibbonCommandId("writer.display-for-review-no-markup")),
            new("Original", new RibbonCommandId("writer.display-for-review-original")),
        });

    private static RibbonMenu BuildShowMarkupMenu() =>
        new(new RibbonMenuItem[]
        {
            new("Insertions and Deletions", new RibbonCommandId("writer.show-markup-insertions-deletions")),
            new("Comments", new RibbonCommandId("writer.show-markup-comments")),
            new("Formatting", new RibbonCommandId("writer.show-markup-formatting")),
            RibbonMenuItem.Separator(),
            new("Show Revisions in Balloons", new RibbonCommandId("writer.show-markup-balloons")),
        });

    /// <summary>AV-INSERT: Insert &gt; Table dropdown — common row×column size presets.</summary>
    private static RibbonMenu BuildTableSizeMenu() =>
        new(new RibbonMenuItem[]
        {
            new("2 × 2 Table",       new RibbonCommandId("writer.table-2x2")),
            new("3 × 3 Table",       new RibbonCommandId("writer.table-3x3")),
            new("4 × 4 Table",       new RibbonCommandId("writer.table-4x4")),
            new("5 × 2 Table",       new RibbonCommandId("writer.table-5x2")),
        });

    /// <summary>AV-REF: References &gt; Insert Caption dropdown — Figure / Table caption labels.</summary>
    private static RibbonMenu BuildCaptionMenu() =>
        new(new RibbonMenuItem[]
        {
            new("Figure", new RibbonCommandId("writer.insert-caption.figure")),
            new("Table",  new RibbonCommandId("writer.insert-caption.table")),
            new("Equation", new RibbonCommandId("writer.insert-caption.equation")),
        });

    private static RibbonMenu BuildTableOfFiguresMenu() =>
        new(new RibbonMenuItem[]
        {
            new("Figure", new RibbonCommandId("writer.tof.figure")),
            new("Table", new RibbonCommandId("writer.tof.table")),
            new("Equation", new RibbonCommandId("writer.tof.equation")),
        });

    private static RibbonMenu BuildTableOfContentsAddTextMenu() =>
        new(new RibbonMenuItem[]
        {
            new("Do Not Show in Table of Contents", new RibbonCommandId("writer.toc-addtext-none")),
            RibbonMenuItem.Separator(),
            new("Level 1", new RibbonCommandId("writer.toc-addtext-level1")),
            new("Level 2", new RibbonCommandId("writer.toc-addtext-level2")),
            new("Level 3", new RibbonCommandId("writer.toc-addtext-level3")),
        });

    private static RibbonMenu BuildMultilevelListMenu() =>
        new(new RibbonMenuItem[]
        {
            new(WriterRibbonText.MultilevelPromoteCommand.Label, new RibbonCommandId("writer.multilevel-promote")),
            new(WriterRibbonText.MultilevelDemoteCommand.Label, new RibbonCommandId("writer.multilevel-demote")),
            new(WriterRibbonDefinitionData.MultilevelListPresetNames[0], new RibbonCommandId("writer.multilevel-preset-0")),
            new(WriterRibbonDefinitionData.MultilevelListPresetNames[1], new RibbonCommandId("writer.multilevel-preset-1")),
            new(WriterRibbonDefinitionData.MultilevelListPresetNames[2], new RibbonCommandId("writer.multilevel-preset-2")),
            new(WriterRibbonText.MultilevelDefineCommand.Label, new RibbonCommandId("writer.multilevel-define")),
        });

    /// <summary>
    /// AV-STYLES: Home &gt; Styles gallery dropdown — the full built-in style set (paragraph and character
    /// styles), one item per <see cref="BuiltInStyles.Gallery"/> entry. Each item's command id is
    /// <c>writer.style.&lt;id&gt;</c> (matching <see cref="FormattingGalleryRibbonWorkflow.StyleCommandId"/>).
    /// </summary>
    private static RibbonMenu BuildStylesMenu() =>
        new(BuiltInStyles.Gallery
            .Select(d => new RibbonMenuItem(
                d.Type == StyleType.Character ? $"{d.Name}  (a)" : d.Name,
                new RibbonCommandId(FormattingGalleryRibbonWorkflow.StyleCommandId(d.Id))))
            .ToArray());
    /// <summary>AV-INSERT2: Insert &gt; Cover Page gallery — the three built-in cover-page presets.</summary>
    private static RibbonMenu BuildCoverPageMenu() =>
        new(new RibbonMenuItem[]
        {
            new("Default", new RibbonCommandId("writer.cover-page-default")),
            new("Banded",  new RibbonCommandId("writer.cover-page-banded")),
            new("Motion",  new RibbonCommandId("writer.cover-page-motion")),
        });

    private static RibbonMenu BuildInsertShapesMenu() =>
        new(new RibbonMenuItem[]
        {
            new("Rectangle", new RibbonCommandId("writer.shape-rectangle")),
            new("Rounded Rectangle", new RibbonCommandId("writer.shape-rounded")),
            new("Ellipse", new RibbonCommandId("writer.shape-ellipse")),
            new("Text Box", new RibbonCommandId("writer.shape-textbox")),
        });

    private static RibbonMenu BuildTextBoxMenu() =>
        new(new RibbonMenuItem[]
        {
            new("Simple Text Box", new RibbonCommandId("writer.textbox-simple")),
            new("Sidebar (Banded)", new RibbonCommandId("writer.textbox-sidebar")),
            new("Quote", new RibbonCommandId("writer.textbox-quote")),
        });

    /// <summary>AV-INSERT2: Insert &gt; Drop Cap menu matching the WPF host routes.</summary>
    private static RibbonMenu BuildDropCapMenu() =>
        new(new RibbonMenuItem[]
        {
            new("Dropped",   new RibbonCommandId("writer.drop-cap-dropped")),
            new("In Margin", new RibbonCommandId("writer.drop-cap-in-margin")),
            new("None (Remove)", new RibbonCommandId("writer.drop-cap-none")),
            RibbonMenuItem.Separator(),
            new("Drop Cap Options...", new RibbonCommandId("writer.drop-cap-options")),
        });

    /// <summary>
    /// Insert &gt; Quick Parts menu shared with the WPF projection. Legacy Avalonia ids remain registry
    /// aliases, but the visible command identity is canonical across both renderers.
    /// </summary>
    private static RibbonMenu BuildQuickPartsMenu() =>
        new(DocumentPropertyFieldPlanner.CommandPlans
            .Select(plan => new RibbonMenuItem(plan.Label, new RibbonCommandId(plan.CommandId)))
            .Concat(new RibbonMenuItem[]
            {
                RibbonMenuItem.Separator(),
                new("Field…", new RibbonCommandId("writer.field")),
                RibbonMenuItem.Separator(),
                new("Save Selection to Quick Part Gallery…", new RibbonCommandId("writer.save-quickpart")),
                new("Building Blocks Organizer…", new RibbonCommandId("writer.building-blocks-organizer")),
            })
            .ToArray());

    /// <summary>
    /// AV-INSERT2: Insert &gt; Equation menu — a default sample (E=mc²) plus a few common OMML structures.
    /// Each preset maps to the canonical command id owned by <see cref="EquationPresetCatalog"/>.
    /// </summary>
    private static RibbonMenu BuildEquationMenu() =>
        new(new RibbonMenuItem[]
        {
            new("Insert New Equation", new RibbonCommandId(EquationPresetCatalog.DefaultCommandId)),
            RibbonMenuItem.Separator(),
            new("Fraction  a/b",       new RibbonCommandId(EquationPresetCatalog.Get(EquationPresetKind.Fraction).CommandId)),
            new("Script  xⁿ",          new RibbonCommandId(EquationPresetCatalog.Get(EquationPresetKind.Script).CommandId)),
            new("Radical  √x",         new RibbonCommandId(EquationPresetCatalog.Get(EquationPresetKind.Radical).CommandId)),
            new("Nth Root",            new RibbonCommandId(EquationPresetCatalog.Get(EquationPresetKind.NthRoot).CommandId)),
            new("Integral  ∫",         new RibbonCommandId(EquationPresetCatalog.Get(EquationPresetKind.Integral).CommandId)),
            new("Summation  ∑",        new RibbonCommandId(EquationPresetCatalog.Get(EquationPresetKind.Summation).CommandId)),
            new("Product",             new RibbonCommandId(EquationPresetCatalog.Get(EquationPresetKind.Product).CommandId)),
            RibbonMenuItem.Separator(),
            new("Accent",              new RibbonCommandId(EquationPresetCatalog.Get(EquationPresetKind.Accent).CommandId)),
            new("Bar",                 new RibbonCommandId(EquationPresetCatalog.Get(EquationPresetKind.Bar).CommandId)),
            new("Bracket",             new RibbonCommandId(EquationPresetCatalog.Get(EquationPresetKind.Bracket).CommandId)),
            new("Matrix",              new RibbonCommandId(EquationPresetCatalog.Get(EquationPresetKind.Matrix).CommandId)),
            new("Function",            new RibbonCommandId(EquationPresetCatalog.Get(EquationPresetKind.Function).CommandId)),
            new("Group Character",     new RibbonCommandId(EquationPresetCatalog.Get(EquationPresetKind.GroupCharacter).CommandId)),
        });

    private static RibbonMenu BuildSymbolMenu() =>
        new(SymbolRibbonWorkflow.Choices
            .Select(choice => new RibbonMenuItem(
                $"{choice.Glyph}   {choice.Label}",
                new RibbonCommandId(choice.CommandId)))
            .ToArray());
}
