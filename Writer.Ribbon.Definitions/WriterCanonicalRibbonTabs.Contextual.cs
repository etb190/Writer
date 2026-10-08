using Writer.App.Presentation.ContextMenus;
using Writer.App.Presentation.Ribbon;
using Writer.Core.Model;

namespace Writer.Ribbon.Definitions;

/// <summary>
/// Canonical contextual-tab topology shared by the WPF and Avalonia Writer renderers.
/// Portable overrides preserve only the native control representations that genuinely differ.
/// </summary>
internal static partial class WriterCanonicalRibbonTabs
{
    internal static RibbonDefinitionBuilder AddPictureContextualTab(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities) =>
        builder.ContextualTab("picture-format", "Picture Format",
            new RibbonTabContext(capabilities.PictureContextKey, "Picture Tools", RibbonContextColor.Orange), tab =>
            {
                var topology = new WriterRibbonTabTopology(tab, capabilities);

                topology.Section(
                    "picture.arrange",
                    tab => tab.Group("picture-arrange", "Arrange", "A", 100, group =>
                        {
                            group.Medium("writer.image-wrap", "Wrap Text", RibbonCommandIconKind.Wrap, menu: menu =>
                            {
                                menu.Item("writer.image-wrap-inline", "In Line with Text", "I");
                                menu.Item("writer.image-wrap-square", "Square", "S");
                                menu.Item("writer.image-wrap-tight", "Tight", "T");
                                menu.Item("writer.image-wrap-top-bottom", "Top and Bottom", "B");
                                menu.Item("writer.image-wrap-behind", "Behind Text", "H");
                                menu.Item("writer.image-wrap-front", "In Front of Text", "F");
                            });
                            group.Medium("writer.image-position", "Position", RibbonCommandIconKind.Margins);
                            group.Medium("writer.image-rotate", "Rotate", RibbonCommandIconKind.Rotate, menu: menu =>
                            {
                                menu.Item("writer.image-rotate-right90", "Rotate Right 90\u00B0", "R");
                                menu.Item("writer.image-rotate-left90", "Rotate Left 90\u00B0", "L");
                                menu.Item("writer.image-flip-vertical", "Flip Vertical", "V");
                                menu.Item("writer.image-flip-horizontal", "Flip Horizontal", "H");
                            });
                            group.Medium("writer.image-align-left", "Align Left", RibbonCommandIconKind.AlignLeft);
                            group.Medium("writer.image-align-center", "Align Center", RibbonCommandIconKind.AlignCenter);
                            group.Medium("writer.image-align-right", "Align Right", RibbonCommandIconKind.AlignRight);
                            group.Medium("writer.image-align-to-page", "Align to Page", RibbonCommandIconKind.Margins);
                            group.Medium("writer.image-align-to-margin", "Align to Margin", RibbonCommandIconKind.Margins);
                            group.Medium("writer.image-distribute-h", "Distribute Horizontally", RibbonCommandIconKind.AlignCenter);
                            group.Medium("writer.image-distribute-v", "Distribute Vertically", RibbonCommandIconKind.AlignCenter);
                            group.Medium("writer.image-bring-to-front", "Bring to Front", RibbonCommandIconKind.BringToFront);
                            group.Medium("writer.image-send-to-back", "Send to Back", RibbonCommandIconKind.SendToBack);
                            group.Medium("writer.image-bring-forward", "Bring Forward", RibbonCommandIconKind.BringForward);
                            group.Medium("writer.image-send-backward", "Send Backward", RibbonCommandIconKind.SendBackward);
                            group.Medium("writer.object-group", "Group", RibbonCommandIconKind.Generic);
                            group.Medium("writer.object-ungroup", "Ungroup", RibbonCommandIconKind.Generic);
                        }),
                    tab => tab.Group("picture-arrange", "Arrange", null, 100, group =>
                        {
                            group.Dropdown("writer.image-position", "Position", BuildFloatingPositionMenu("image"));
                            group.Dropdown("writer.image-wrap", "Wrap Text", BuildWrapMenu("image"), control => control with
                            {
                                Icon = new RibbonCommandIcon(RibbonCommandIconKind.Wrap),
                            });
                            group.Dropdown("writer.image-rotate", "Rotate", BuildRotateMenu("image"));
                            // r185: these four were writer.shape-* in the PICTURE group and
                            // writer.image-* in the DRAWING group -- a straight swap, and only in
                            // the portable/Avalonia overrides; the WPF canonical closures above
                            // each use the right family. So on Linux/macOS the Picture Format
                            // tab z-order buttons acted on the selected SHAPE and the Drawing
                            // Format ones acted on the selected PICTURE, i.e. they did nothing
                            // visible when only the tab own object type was selected.
                            group.Button("writer.image-bring-to-front", "Bring to Front");
                            group.Button("writer.image-send-to-back", "Send to Back");
                            group.Button("writer.image-bring-forward", "Bring Forward");
                            group.Button("writer.image-send-backward", "Send Backward");
                            group.Button("writer.image-align-left", "Align Left");
                            group.Button("writer.image-align-center", "Align Center");
                            group.Button("writer.image-align-right", "Align Right");
                            group.Button("writer.image-align-to-page", "Align to Page");
                            group.Button("writer.image-align-to-margin", "Align to Margin");
                            group.Button("writer.image-distribute-h", "Distribute Horizontally");
                            group.Button("writer.image-distribute-v", "Distribute Vertically");
                            group.Button("writer.object-group", "Group");
                            group.Button("writer.object-ungroup", "Ungroup");
                        }));

                topology.Section(
                    "picture.styles",
                    tab => tab.Group("picture-styles", "Picture Styles", "Y", 98, group =>
                        {
                            foreach (var preset in PictureStyleCatalog.Catalog)
                                group.Medium($"writer.image-style-{preset.Id}", preset.Name, RibbonCommandIconKind.Border);
                        }),
                    tab => tab.Group("picture-styles", "Picture Styles", null, 98, group =>
                        {
                            foreach (var preset in PictureStyleCatalog.Catalog)
                            {
                                group.Button($"writer.image-style-{preset.Id}", preset.Name, button => button with
                                {
                                    Icon = new RibbonCommandIcon(RibbonCommandIconKind.Border)
                                });
                            }
                        }));

                topology.Section(
                    "picture.adjust",
                    tab => tab.Group("picture-adjust", "Adjust", "J", 95, group =>
                        {
                            group.Medium("writer.image-corrections", "Corrections", RibbonCommandIconKind.Effects, menu: menu =>
                            {
                                menu.Item("writer.image-brightness-plus20", "Brightness: +20%", "1");
                                menu.Item("writer.image-brightness-plus40", "Brightness: +40%", "2");
                                menu.Item("writer.image-brightness-minus20", "Brightness: -20%", "3");
                                menu.Item("writer.image-brightness-minus40", "Brightness: -40%", "4");
                                menu.Item("writer.image-contrast-plus20", "Contrast: +20%", "5");
                                menu.Item("writer.image-contrast-minus20", "Contrast: -20%", "6");
                                menu.Item("writer.image-adjust-dialog", "Picture Corrections\u2026", "D");
                            });
                            group.Medium("writer.image-color", "Color", RibbonCommandIconKind.Color, menu: menu =>
                            {
                                menu.Item("writer.image-saturation-0", "Saturation: 0% (Greyscale)", "G");
                                menu.Item("writer.image-saturation-50", "Saturation: 50%", "H");
                                menu.Item("writer.image-saturation-200", "Saturation: 200%", "J");
                                menu.Item("writer.image-color-dialog", "Color\u2026", "C");
                                menu.Separator();
                                menu.Item("writer.image-recolor-grayscale", "Recolor: Grayscale", "1");
                                menu.Item("writer.image-recolor-sepia", "Recolor: Sepia", "2");
                                menu.Item("writer.image-recolor-washout", "Recolor: Washout", "3");
                                menu.Item("writer.image-recolor-blackwhite", "Recolor: Black and White", "4");
                                menu.Item("writer.image-recolor-none", "Recolor: No Recolor", "N");
                                menu.Separator();
                                menu.Item("writer.image-colortemp-warm", "Color Tone: Warm (3000K)", "W");
                                menu.Item("writer.image-colortemp-cool", "Color Tone: Cool (8000K)", "L");
                                menu.Item("writer.image-colortemp-neutral", "Color Tone: Neutral", "T");
                            });
                            group.Medium("writer.image-transparency", "Transparency", RibbonCommandIconKind.View, menu: menu =>
                            {
                                menu.Item("writer.image-transparency-25", "Transparency: 25%", "A");
                                menu.Item("writer.image-transparency-50", "Transparency: 50%", "B");
                                menu.Item("writer.image-transparency-75", "Transparency: 75%", "C");
                                menu.Item("writer.image-transparency-dialog", "Transparency\u2026", "D");
                            });
                            group.Medium("writer.image-effects", "Picture Effects", RibbonCommandIconKind.Effects, menu: menu =>
                            {
                                menu.Item("writer.image-shadow-none", "Shadow: No Shadow", "N");
                                menu.Item("writer.image-shadow-1", "Shadow: Offset Diagonal", "1");
                                menu.Item("writer.image-shadow-2", "Shadow: Offset Diagonal Medium", "2");
                                menu.Item("writer.image-shadow-3", "Shadow: Perspective", "3");
                                menu.Item("writer.image-shadow-4", "Shadow: Offset Bottom", "4");
                                menu.Item("writer.image-shadow-5", "Shadow: Large", "5");
                                menu.Separator();
                                menu.Item("writer.image-reflection-none", "Reflection: No Reflection", "R");
                                menu.Item("writer.image-reflection-1", "Reflection: Tight, Touching", "A");
                                menu.Item("writer.image-reflection-2", "Reflection: Tight, 4pt", "B");
                                menu.Item("writer.image-reflection-3", "Reflection: Tight, 8pt", "C");
                                menu.Item("writer.image-reflection-4", "Reflection: Half, Touching", "D");
                                menu.Item("writer.image-reflection-5", "Reflection: Half, 4pt", "E");
                                menu.Separator();
                                menu.Item("writer.image-glow-none", "Glow: No Glow", "G");
                                menu.Item("writer.image-glow-5", "Glow: 5 pt", "H");
                                menu.Item("writer.image-glow-8", "Glow: 8 pt", "I");
                                menu.Item("writer.image-glow-11", "Glow: 11 pt", "J");
                                menu.Item("writer.image-glow-18", "Glow: 18 pt", "K");
                                menu.Separator();
                                menu.Item("writer.image-softedge-none", "Soft Edges: None", "S");
                                menu.Item("writer.image-softedge-1", "Soft Edges: 1 pt", "T");
                                menu.Item("writer.image-softedge-2pt5", "Soft Edges: 2.5 pt", "U");
                                menu.Item("writer.image-softedge-5", "Soft Edges: 5 pt", "V");
                                menu.Item("writer.image-softedge-10", "Soft Edges: 10 pt", "X");
                                menu.Separator();
                                menu.Item("writer.image-bevel-none", "Bevel: No Bevel", "O");
                                menu.Item("writer.image-bevel-1", "Bevel: Circle", "P");
                                menu.Item("writer.image-bevel-2", "Bevel: Relaxed Inset", "Q");
                                menu.Item("writer.image-bevel-3", "Bevel: Cross", "F");
                                menu.Item("writer.image-bevel-4", "Bevel: Cool Slant", "M");
                            });
                            group.Medium("writer.image-artistic", "Artistic Effects", RibbonCommandIconKind.Effects, menu: menu =>
                            {
                                menu.Item("writer.image-artistic-none", "No Artistic Effect", "N");
                                menu.Item("writer.image-artistic-blur", "Blur", "B");
                                menu.Item("writer.image-artistic-glow-diffused", "Glow Diffused", "G");
                                menu.Item("writer.image-artistic-glow-edges", "Glow Edges", "E");
                                menu.Item("writer.image-artistic-pencil-gray", "Pencil Grayscale", "A");
                                menu.Item("writer.image-artistic-pencil-sketch", "Pencil Sketch", "K");
                                menu.Item("writer.image-artistic-line-drawing", "Line Drawing", "L");
                                menu.Item("writer.image-artistic-paintbrush", "Paint Brush", "P");
                                menu.Item("writer.image-artistic-paint-strokes", "Paint Strokes", "T");
                                menu.Item("writer.image-artistic-photocopy", "Photocopy", "H");
                                menu.Item("writer.image-artistic-posterize", "Posterize", "O");
                                menu.Item("writer.image-artistic-pastels", "Pastels", "S");
                                menu.Item("writer.image-artistic-watercolor", "Watercolor Sponge", "W");
                                menu.Item("writer.image-artistic-film-grain", "Film Grain", "F");
                                menu.Item("writer.image-artistic-mosaic", "Mosaic Bubbles", "M");
                            });
                            group.Medium("writer.image-crop", "Crop", RibbonCommandIconKind.Scale);
                            group.Medium("writer.image-reset", "Reset Picture", RibbonCommandIconKind.Refresh);
                            group.Medium("writer.image-border", "Picture Border", RibbonCommandIconKind.Border,
                                accent: RibbonCommandIconAccent.Border);
                        }),
                    tab => tab.Group("picture-adjust", "Adjust", null, 90, group =>
                        {
                            group.Dropdown("writer.image-corrections", "Corrections", BuildPictureCorrectionsMenu(), control => control with
                            {
                                Icon = new RibbonCommandIcon(RibbonCommandIconKind.Effects),
                            });
                            group.Dropdown("writer.image-color", "Color", BuildPictureColorMenu());
                            group.Dropdown("writer.image-transparency", "Transparency", BuildPictureTransparencyMenu());
                            group.Dropdown("writer.image-effects", "Picture Effects", BuildPictureEffectsMenu());
                            group.Dropdown("writer.image-artistic", "Artistic Effects", BuildPictureArtisticEffectsMenu());
                            group.Button("writer.image-reset", "Reset Picture", button => button with
                            {
                                Icon = new RibbonCommandIcon(RibbonCommandIconKind.Refresh)
                            });
                            group.Button("writer.image-border", "Picture Border", button => button with
                            {
                                Icon = new RibbonCommandIcon(RibbonCommandIconKind.Border, RibbonCommandIconAccent.Border)
                            });
                            group.Button("writer.image-crop", "Crop", button => button with
                            {
                                Icon = new RibbonCommandIcon(RibbonCommandIconKind.Scale)
                            });
                        }));

                topology.Section(
                    "picture.size",
                    tab => tab.Group("picture-size", "Size", "S", 90, group =>
                        {
                            group.Medium("writer.image-size", "Size", RibbonCommandIconKind.Size);
                            group.Medium("writer.image-alt-text", "Alt Text", RibbonCommandIconKind.Info);
                        }),
                    tab => tab.Group("picture-size", "Size", null, 90, group =>
                        {
                            group.ComboBox("writer.image-width", "Width", control => control with
                            {
                                Items = FloatSizes,
                                Width = 72
                            });
                            group.ComboBox("writer.image-height", "Height", control => control with
                            {
                                Items = FloatSizes,
                                Width = 72
                            });
                            group.Button("writer.image-size", "Size", button => button with
                            {
                                Icon = new RibbonCommandIcon(RibbonCommandIconKind.Size)
                            });
                            group.Button("writer.image-alt-text", "Alt Text", button => button with
                            {
                                Icon = new RibbonCommandIcon(RibbonCommandIconKind.Info)
                            });
                        }));

                topology.Build();
            });

    internal static RibbonDefinitionBuilder AddDrawingContextualTab(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities) =>
        builder.ContextualTab("drawing-format", "Drawing Format",
            new RibbonTabContext(capabilities.DrawingContextKey, "Drawing Tools", RibbonContextColor.Purple), tab =>
            {
                var topology = new WriterRibbonTabTopology(tab, capabilities);

                topology.Section(
                    "drawing.insert",
                    tab => tab.Group("drawing-insert", "Insert Shapes", "I", 110, group =>
                        {
                            group.Medium("writer.shape-change", "Change Shape", RibbonCommandIconKind.Shapes, menu: menu =>
                            {
                                menu.Item("writer.shape-change-rectangle", "Rectangle", "R");
                                menu.Item("writer.shape-change-rounded", "Rounded Rectangle", "U");
                                menu.Item("writer.shape-change-ellipse", "Oval", "O");
                            });
                            group.Medium("writer.shape-edit-shape", "Edit Shape", RibbonCommandIconKind.Generic, menu: menu =>
                            {
                                menu.Item("writer.shape-convert-freeform", "Convert to Freeform", "F");
                                menu.Item("writer.shape-edit-points", "Edit Points", "E");
                            });
                        }));

                topology.Section(
                    "drawing.styles",
                    tab => tab.Group("drawing-styles", "Shape Styles", "H", 100, group =>
                        {
                            group.Medium("writer.shape-styles-gallery", "Shape Styles", RibbonCommandIconKind.Styles);
                            group.Medium("writer.shape-fill", "Shape Fill", RibbonCommandIconKind.Fill,
                                accent: RibbonCommandIconAccent.Fill, menu: menu =>
                            {
                                menu.Item("writer.shape-fill-no-fill", "No Fill", "N");
                                menu.Separator();
                                menu.Item("writer.shape-fill-gradient-blue", "Gradient Blue", "G");
                                menu.Item("writer.shape-fill-gradient-orange", "Gradient Orange", "O");
                                menu.Item("writer.shape-fill-pattern-diag", "Pattern: Diagonal Cross", "D");
                            });
                            group.Medium("writer.shape-outline", "Shape Outline", RibbonCommandIconKind.Border,
                                accent: RibbonCommandIconAccent.Border, menu: menu =>
                            {
                                menu.Item("writer.shape-outline-no-outline", "No Outline", "N");
                                menu.Item("writer.shape-outline-solid", "Solid", "S");
                                menu.Item("writer.shape-outline-dash", "Dash", "D");
                                menu.Item("writer.shape-outline-dot", "Dot", "O");
                            });
                            group.Medium("writer.shape-effects", "Shape Effects", RibbonCommandIconKind.Effects, menu: menu =>
                            {
                                menu.Item("writer.shape-effects-none", "No Effects", "N");
                                menu.Separator();
                                menu.Item("writer.shape-effect-shadow", "Shadow", "S");
                                menu.Item("writer.shape-effect-glow", "Glow", "G");
                                menu.Item("writer.shape-effect-soft-edge", "Soft Edges", "E");
                                menu.Item("writer.shape-effect-reflection", "Reflection", "R");
                                menu.Item("writer.shape-effect-bevel", "Bevel", "B");
                            });
                        }),
                    tab => tab.Group("drawing-styles", "Shape Styles", null, 100, group =>
                        {
                            group.Dropdown("writer.shape-styles-gallery", "Shape Styles", BuildShapeStylesMenu());
                            group.Dropdown("writer.shape-fill", "Shape Fill", BuildShapeFillMenu());
                            group.Dropdown("writer.shape-outline", "Shape Outline", BuildShapeOutlineMenu());
                            group.Dropdown("writer.shape-effects", "Shape Effects", BuildShapeEffectsMenu());
                            group.Dropdown("writer.shape-change", "Change Shape", BuildShapeChangeMenu());
                            group.Dropdown("writer.shape-edit-shape", "Edit Shape", BuildShapeEditMenu());
                            group.Dropdown("writer.shape-text-direction", "Text Direction", BuildShapeTextDirectionMenu());
                        }));

                topology.Section(
                    "drawing.text",
                    tab => tab.Group("drawing-text", "Text", "X", 90, group =>
                        {
                            group.Medium("writer.shape-text-direction", "Text Direction", RibbonCommandIconKind.TextBox, menu: menu =>
                            {
                                menu.Item("writer.shape-text-horizontal", "Horizontal", "H");
                                menu.Item("writer.shape-text-rotate90", "Rotate 90\u00B0", "R");
                                menu.Item("writer.shape-text-rotate270", "Rotate 270\u00B0", "T");
                            });
                        }));

                topology.Section(
                    "drawing.wordart",
                    tab => tab.Group("drawing-wordart", "WordArt Styles", "W", 85, group =>
                        {
                            group.Medium(
                                WordArtRibbonWorkflow.StyleMenuCommandId.Value,
                                "WordArt Style",
                                RibbonCommandIconKind.WordArt,
                                menu: BuildWordArtStyleMenu);
                            group.Medium(
                                WordArtRibbonWorkflow.WarpMenuCommandId.Value,
                                "Text Effects: Transform",
                                RibbonCommandIconKind.WordArt,
                                menu: BuildWordArtWarpMenu);
                        }));

                topology.Section(
                    "drawing.arrange",
                    tab => tab.Group("drawing-arrange", "Arrange", "A", 80, group =>
                        {
                            group.Medium("writer.shape-wrap", "Wrap Text", RibbonCommandIconKind.Wrap, menu: menu =>
                            {
                                menu.Item("writer.shape-wrap-inline", "In Line with Text", "I");
                                menu.Item("writer.shape-wrap-square", "Square", "S");
                                menu.Item("writer.shape-wrap-tight", "Tight", "T");
                                menu.Item("writer.shape-wrap-top-bottom", "Top and Bottom", "B");
                                menu.Item("writer.shape-wrap-behind", "Behind Text", "H");
                                menu.Item("writer.shape-wrap-front", "In Front of Text", "F");
                            });
                            group.Medium("writer.shape-position", "Position", RibbonCommandIconKind.Margins);
                            group.Medium("writer.shape-rotate", "Rotate", RibbonCommandIconKind.Rotate, menu: menu =>
                            {
                                menu.Item("writer.shape-rotate-right90", "Rotate Right 90\u00B0", "R");
                                menu.Item("writer.shape-rotate-left90", "Rotate Left 90\u00B0", "L");
                                menu.Item("writer.shape-flip-vertical", "Flip Vertical", "V");
                                menu.Item("writer.shape-flip-horizontal", "Flip Horizontal", "H");
                            });
                            group.Medium("writer.shape-align-left", "Align Left", RibbonCommandIconKind.AlignLeft);
                            group.Medium("writer.shape-align-center", "Align Center", RibbonCommandIconKind.AlignCenter);
                            group.Medium("writer.shape-align-right", "Align Right", RibbonCommandIconKind.AlignRight);
                            group.Medium("writer.shape-align-to-page", "Align to Page", RibbonCommandIconKind.Margins);
                            group.Medium("writer.shape-align-to-margin", "Align to Margin", RibbonCommandIconKind.Margins);
                            group.Medium("writer.shape-distribute-h", "Distribute Horizontally", RibbonCommandIconKind.AlignCenter);
                            group.Medium("writer.shape-distribute-v", "Distribute Vertically", RibbonCommandIconKind.AlignCenter);
                            group.Medium("writer.shape-bring-to-front", "Bring to Front", RibbonCommandIconKind.BringToFront);
                            group.Medium("writer.shape-send-to-back", "Send to Back", RibbonCommandIconKind.SendToBack);
                            group.Medium("writer.shape-bring-forward", "Bring Forward", RibbonCommandIconKind.BringForward);
                            group.Medium("writer.shape-send-backward", "Send Backward", RibbonCommandIconKind.SendBackward);
                            group.Medium("writer.object-group", "Group", RibbonCommandIconKind.Generic);
                            group.Medium("writer.object-ungroup", "Ungroup", RibbonCommandIconKind.Generic);
                        }),
                    tab => tab.Group("drawing-arrange", "Arrange", null, 90, group =>
                        {
                            group.Dropdown("writer.shape-position", "Position", BuildFloatingPositionMenu("shape"));
                            group.Dropdown("writer.shape-wrap", "Wrap Text", BuildWrapMenu("shape"));
                            group.Dropdown("writer.shape-rotate", "Rotate", BuildRotateMenu("shape"));
                            // r185: see the Picture Format group above -- these two sets were
                            // swapped with each other.
                            group.Button("writer.shape-bring-to-front", "Bring to Front");
                            group.Button("writer.shape-send-to-back", "Send to Back");
                            group.Button("writer.shape-bring-forward", "Bring Forward");
                            group.Button("writer.shape-send-backward", "Send Backward");
                            group.Button("writer.shape-align-left", "Align Left");
                            group.Button("writer.shape-align-center", "Align Center");
                            group.Button("writer.shape-align-right", "Align Right");
                            group.Button("writer.shape-align-to-page", "Align to Page");
                            group.Button("writer.shape-align-to-margin", "Align to Margin");
                            group.Button("writer.shape-distribute-h", "Distribute Horizontally");
                            group.Button("writer.shape-distribute-v", "Distribute Vertically");
                            group.Button("writer.object-group", "Group");
                            group.Button("writer.object-ungroup", "Ungroup");
                        }));

                topology.Section(
                    "drawing.size",
                    tab => tab.Group("drawing-size", "Size", "S", 70, group =>
                        {
                            group.Medium("writer.shape-size", "Size", RibbonCommandIconKind.Size);
                            group.Medium("writer.shape-alt-text", "Alt Text", RibbonCommandIconKind.Info);
                        }),
                    tab => tab.Group("drawing-size", "Size", null, 80, group =>
                        {
                            group.ComboBox("writer.shape-width", "Width", control => control with
                            {
                                Items = FloatSizes,
                                Width = 72
                            });
                            group.ComboBox("writer.shape-height", "Height", control => control with
                            {
                                Items = FloatSizes,
                                Width = 72
                            });
                            group.Dropdown("writer.shape-size", "Size", BuildShapeSizeMenu());
                            group.Dropdown("writer.shape-alt-text", "Alt Text", BuildShapeAltTextMenu());
                        }));

                topology.Build();
            });

    internal static RibbonDefinitionBuilder AddChartContextualTabs(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities)
    {
        AddChartDesignTopology(builder, capabilities);
        return AddChartFormatTopology(builder, capabilities);
    }

    private static RibbonDefinitionBuilder AddChartDesignTopology(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities) =>
        builder.ContextualTab("chart-design", "Chart Design",
            new RibbonTabContext(capabilities.ChartContextKey, "Chart Tools", RibbonContextColor.Orange), tab =>
            {
                var topology = new WriterRibbonTabTopology(tab, capabilities);

                topology.Section(
                    "chart.type",
                    tab => tab.Group("chart-type", "Type", "T", 100, group =>
                        group.Medium("writer.chart-type-column", "Column", RibbonCommandIconKind.ChartColumn, menu: menu =>
                        {
                            menu.Item("writer.chart-type-column", "Column", "C");
                            menu.Item("writer.chart-type-bar", "Bar", "B");
                            menu.Item("writer.chart-type-line", "Line", "L");
                            menu.Item("writer.chart-type-pie", "Pie", "P");
                            menu.Item("writer.chart-type-scatter", "Scatter", "X");
                            menu.Item("writer.chart-type-area", "Area", "A");
                            menu.Item("writer.chart-type-doughnut", "Doughnut", "D");
                        })),
                    tab => tab.Group("chart-type", "Type", null, 100, group =>
                        group.Dropdown("writer.chart-type", "Change Chart Type", BuildChartTypeMenu())));

                topology.Section(
                    "chart.data",
                    tab => tab.Group("chart-data", "Data", "D", 90, group =>
                        group.Medium("writer.chart-edit-data", "Edit Data", RibbonCommandIconKind.Table)),
                    tab => tab.Group("chart-data", "Data", null, 80, group =>
                        {
                            group.ComboBox("writer.chart-edit-data", "Edit Data", control => control with
                            {
                                Items = new[] { "Quarterly Sales", "Monthly Revenue" },
                                Width = 132
                            });
                        }));

                topology.Section(
                    "chart.quick-layout",
                    tab => tab.Group("chart-quick-layout", "Quick Layout", "L", 85, group =>
                        {
                            foreach (var layout in ChartQuickLayout.Catalog)
                                group.Medium($"writer.chart-quick-layout-{layout.Id}", layout.Name, RibbonCommandIconKind.Grid);
                        }),
                    tab => tab.Group("chart-quick-layout", "Quick Layout", null, 85, group =>
                        {
                            foreach (var layout in ChartQuickLayout.Catalog)
                            {
                                group.Button($"writer.chart-quick-layout-{layout.Id}", layout.Name, button => button with
                                {
                                    Icon = new RibbonCommandIcon(RibbonCommandIconKind.Grid)
                                });
                            }
                        }));

                topology.Section(
                    "chart.styles",
                    tab =>
                    {
                        tab.Group("chart-style", "Chart Styles", "S", 80, group =>
                        {
                            foreach (var style in ChartStyle.Catalog)
                                group.Medium($"writer.chart-style-{style.Id}", style.Name, RibbonCommandIconKind.ChartColumn);
                        });

                        tab.Group("chart-colors", "Change Colors", "C", 75, group =>
                        {
                            foreach (var scheme in ChartColorScheme.Catalog)
                                group.Medium(ChartColorRibbonCommandCatalog.CommandId(scheme), scheme.Name, RibbonCommandIconKind.Fill);
                        });
                    },
                    tab => tab.Group("chart-styles", "Chart Styles", null, 90, group =>
                        {
                            group.Dropdown("writer.chart-style", "Chart Styles", BuildChartStyleMenu());
                            group.Dropdown("writer.chart-colors", "Change Colors", BuildChartColorsMenu());
                        }));

                topology.Section(
                    "chart.elements",
                    tab => tab.Group("chart-elements", "Chart Layouts", "E", 70, group =>
                        {
                            group.Medium("writer.chart-title", "Chart Title", RibbonCommandIconKind.Header);
                            group.Medium("writer.chart-axis-titles", "Axis Titles", RibbonCommandIconKind.Ruler);
                            group.Medium("writer.chart-toggle-legend", "Legend", RibbonCommandIconKind.List);
                        }),
                    tab => tab.Group("chart-elements", "Chart Elements", null, 80, group =>
                        {
                            group.Toggle("writer.chart-toggle-legend", "Legend");
                            group.Button("writer.chart-title", "Chart Title");
                            group.Button("writer.chart-axis-titles", "Axis Titles");
                        }));

                topology.Build();
            });

    private static RibbonDefinitionBuilder AddChartFormatTopology(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities) =>
        builder.ContextualTab("chart-format", "Chart Format",
            new RibbonTabContext(
                capabilities.ChartContextKey,
                "Chart Tools",
                RibbonContextColor.Orange), tab =>
            {
                var topology = new WriterRibbonTabTopology(tab, capabilities);

                topology.Section(
                    "chart.arrange",
                    tab => tab.Group("chart-arrange", "Arrange", "A", 100, group =>
                        {
                            group.Medium("writer.shape-rotate", "Rotate", RibbonCommandIconKind.Rotate, menu: menu =>
                            {
                                menu.Item("writer.shape-rotate-right90", "Rotate Right 90\u00B0", "R");
                                menu.Item("writer.shape-rotate-left90", "Rotate Left 90\u00B0", "L");
                                menu.Item("writer.shape-flip-vertical", "Flip Vertical", "V");
                                menu.Item("writer.shape-flip-horizontal", "Flip Horizontal", "H");
                            });
                        }),
                    tab => tab.Group("chart-arrange", "Arrange", null, 100, group =>
                        {
                            group.Dropdown("writer.shape-rotate", "Rotate", BuildRotateMenu("shape"));
                            group.Dropdown("writer.shape-wrap", "Wrap Text", BuildWrapMenu("shape"));
                            group.Button("writer.image-bring-to-front", "Bring to Front");
                            group.Button("writer.image-send-to-back", "Send to Back");
                            group.Button("writer.image-bring-forward", "Bring Forward");
                            group.Button("writer.image-send-backward", "Send Backward");
                        }));

                topology.Section(
                    "chart.size",
                    tab => tab.Group("chart-size", "Size", "S", 90, group =>
                        {
                            group.Medium("writer.chart-size", "Size", RibbonCommandIconKind.Size);
                            group.Medium("writer.chart-size-dialog", "More Size Options...", RibbonCommandIconKind.Size);
                        }),
                    tab => tab.Group("chart-size", "Size", null, 90, group =>
                        {
                            group.ComboBox("writer.chart-size", "Size", control => control with
                            {
                                Items = new[] { "360 x 216", "400 x 300", "468 x 288" },
                                Width = 90
                            });
                            group.ComboBox("writer.shape-width", "Width", control => control with
                            {
                                Items = FloatSizes,
                                Width = 72
                            });
                            group.ComboBox("writer.shape-height", "Height", control => control with
                            {
                                Items = FloatSizes,
                                Width = 72
                            });
                            group.Button("writer.chart-size-dialog", "More Size Options...");
                        }));

                topology.Build();
            });

    internal static RibbonDefinitionBuilder AddSmartArtContextualTab(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities) =>
        builder.ContextualTab("smartart-design", "SmartArt Design",
            new RibbonTabContext(capabilities.SmartArtContextKey, "SmartArt Tools", RibbonContextColor.Orange), tab =>
            {
                var topology = new WriterRibbonTabTopology(tab, capabilities);

                topology.Section(
                    "smartart.create",
                    tab => tab.Group("smartart-create-graphic", "Create Graphic", "G", 100, group =>
                        {
                            group.Medium("writer.smartart-add-shape", "Add Shape", RibbonCommandIconKind.Insert);
                            group.Medium("writer.smartart-remove-shape", "Remove Shape", RibbonCommandIconKind.Delete);
                            group.RowBreak();
                            group.Medium("writer.smartart-promote", "Promote", RibbonCommandIconKind.IndentDecrease);
                            group.Medium("writer.smartart-demote", "Demote", RibbonCommandIconKind.IndentIncrease);
                            group.RowBreak();
                            group.Medium("writer.smartart-move-up", "Move Up", RibbonCommandIconKind.ArrowUp);
                            group.Medium("writer.smartart-move-down", "Move Down", RibbonCommandIconKind.ArrowDown);
                        }),
                    tab => tab.Group("smartart-create-graphic", "Create Graphic", null, 120, group =>
                        {
                            group.Button("writer.smartart-add-shape", "Add Shape");
                            group.Button("writer.smartart-remove-shape", "Remove Shape");
                            group.RowBreak();
                            group.Button("writer.smartart-promote", "Promote");
                            group.Button("writer.smartart-demote", "Demote");
                            group.RowBreak();
                            group.Button("writer.smartart-move-up", "Move Up");
                            group.Button("writer.smartart-move-down", "Move Down");
                        }));

                topology.Section(
                    "smartart.edit",
                    tab => tab.Group("smartart-edit", "Edit", "E", 90, group =>
                        group.Medium("writer.smartart-edit-text", "Edit Text", RibbonCommandIconKind.TextFunction)),
                    tab => tab.Group("smartart-edit", "Edit", null, 90, group =>
                        group.Button("writer.smartart-edit-text", "Edit Text")));

                topology.Section(
                    "smartart.layouts",
                    tab => tab.Group("smartart-layouts", "Layouts", "L", 80, group =>
                        group.Dropdown("writer.smartart-layout", "Layouts", BuildSmartArtLayoutMenu())));

                topology.Section(
                    "smartart.styles",
                    tab => tab.Group("smartart-styles", "SmartArt Styles", "C", 90, group =>
                        {
                            group.Dropdown("writer.smartart-colors", "Change Colors", BuildSmartArtColorsMenu());
                            group.Dropdown("writer.smartart-change-style", "Styles", BuildSmartArtStylesMenu());
                        }));

                topology.Section(
                    "smartart.arrange",
                    tab => tab.Group("smartart-arrange", "Arrange", "A", 60, group =>
                        {
                            group.Medium("writer.shape-rotate", "Rotate", RibbonCommandIconKind.Rotate, menu: menu =>
                            {
                                menu.Item("writer.shape-rotate-right90", "Rotate Right 90\u00B0", "R");
                                menu.Item("writer.shape-rotate-left90", "Rotate Left 90\u00B0", "L");
                                menu.Item("writer.shape-flip-vertical", "Flip Vertical", "V");
                                menu.Item("writer.shape-flip-horizontal", "Flip Horizontal", "H");
                            });
                        }),
                    tab => tab.Group("smartart-arrange", "Arrange", null, 80, group =>
                        {
                            group.Dropdown("writer.shape-rotate", "Rotate", BuildRotateMenu("shape"));
                            group.Dropdown("writer.shape-wrap", "Wrap Text", BuildWrapMenu("shape"));
                            group.Button("writer.image-bring-to-front", "Bring to Front");
                            group.Button("writer.image-send-to-back", "Send to Back");
                        }));

                topology.Section(
                    "smartart.size",
                    tab => tab.Group("smartart-size", "Size", null, 70, group =>
                        {
                            group.ComboBox("writer.shape-width", "Width", control => control with
                            {
                                Items = FloatSizes,
                                Width = 72
                            });
                            group.ComboBox("writer.shape-height", "Height", control => control with
                            {
                                Items = FloatSizes,
                                Width = 72
                            });
                        }));

                topology.Build();
            });

    internal static RibbonDefinitionBuilder AddTableContextualTabs(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities)
    {
        AddTableDesignTopology(builder, capabilities);
        return AddTableLayoutTopology(builder, capabilities);
    }

    private static RibbonDefinitionBuilder AddTableDesignTopology(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities) =>
        builder.ContextualTab("table-design", "Table Design",
            new RibbonTabContext(capabilities.TableContextKey, "Table Tools", RibbonContextColor.Teal), tab =>
            {
                var topology = new WriterRibbonTabTopology(tab, capabilities);

                topology.Section(
                    "table.style-options",
                    tab => tab.Group("table-style-options", "Table Style Options", "O", 100, group =>
                        {
                            group.Medium("writer.table-header-row", "Header Row", RibbonCommandIconKind.Table,
                                accent: RibbonCommandIconAccent.Green);
                            group.Medium("writer.table-last-row", "Last Row", RibbonCommandIconKind.Table,
                                accent: RibbonCommandIconAccent.Green);
                            group.RowBreak();
                            group.Medium("writer.table-first-column", "First Column", RibbonCommandIconKind.Table,
                                accent: RibbonCommandIconAccent.Green);
                            group.Medium("writer.table-last-column", "Last Column", RibbonCommandIconKind.Table,
                                accent: RibbonCommandIconAccent.Green);
                            group.RowBreak();
                            group.Medium("writer.table-banded-rows", "Banded Rows", RibbonCommandIconKind.Table,
                                accent: RibbonCommandIconAccent.Green);
                            group.Medium("writer.table-banded-cols", "Banded Columns", RibbonCommandIconKind.Table,
                                accent: RibbonCommandIconAccent.Green);
                        }),
                    tab => tab.Group("table-style-options", "Table Style Options", null, 100, group =>
                        {
                            group.Toggle("writer.table-header-row", "Header Row");
                            group.Toggle("writer.table-last-row", "Last Row");
                            group.Toggle("writer.table-first-column", "First Column");
                            group.Toggle("writer.table-last-column", "Last Column");
                            group.Toggle("writer.table-banded-rows", "Banded Rows");
                            group.Toggle("writer.table-banded-cols", "Banded Columns");
                        }));

                topology.Section(
                    "table.styles",
                    tab =>
                    {
                        // Keep the injected gallery separate from Shading and Borders. Replacing a
                        // shared group content lane would otherwise hide those table-formatting commands.
                        tab.Group("table-styles", "Table Styles", "Y", 90, group =>
                            group.Dropdown("writer.table-styles", "Table Styles", BuildTableStylesMenu()));
                        tab.Group("table-style", "Table Style", null, 80, group =>
                        {
                            group.Medium("writer.table-shading", "Shading", RibbonCommandIconKind.Fill,
                                accent: RibbonCommandIconAccent.Fill);
                            group.Medium("writer.table-borders", "Borders", RibbonCommandIconKind.Grid);
                        });
                    },
                    tab =>
                    {
                        // Keep the gallery as its own adaptive group. Shading and Borders remain
                        // independently reachable beside the injected thumbnail picker.
                        tab.Group("table-styles", "Table Styles", null, 90, group =>
                            group.Dropdown("writer.table-styles", "Table Styles", BuildTableStylesMenu()));
                        tab.Group("table-style", "Table Style", null, 70, group =>
                        {
                            group.Button("writer.table-shading", "Shading");
                            group.Dropdown("writer.table-borders", "Borders", BuildTableBordersMenu());
                        });
                    });

                topology.Section(
                    "table.borders",
                    tab => tab.Group("draw-borders", "Draw Borders", "D", 60, group =>
                        {
                            group.Medium("writer.draw-table", "Draw Table", RibbonCommandIconKind.Table,
                                accent: RibbonCommandIconAccent.Border);
                            group.Medium("writer.eraser", "Eraser", RibbonCommandIconKind.Clear);
                        }),
                    tab => tab.Group("draw-borders", "Draw Borders", null, 80, group =>
                        {
                            group.Button("writer.draw-table", "Draw Table", button => button with
                            {
                                PreferredLayout = RibbonCommandLayoutKind.Medium,
                                Icon = new RibbonCommandIcon(RibbonCommandIconKind.Table, RibbonCommandIconAccent.Border)
                            });
                            group.Button("writer.eraser", "Eraser", button => button with
                            {
                                PreferredLayout = RibbonCommandLayoutKind.Medium,
                                Icon = new RibbonCommandIcon(RibbonCommandIconKind.Clear)
                            });
                        }));

                topology.Build();
            });

    private static RibbonDefinitionBuilder AddTableLayoutTopology(
        this RibbonDefinitionBuilder builder,
        WriterRibbonCapabilities capabilities) =>
        builder.ContextualTab("table-layout", "Table Layout",
            new RibbonTabContext(capabilities.TableContextKey, "Table Tools", RibbonContextColor.Teal), tab =>
            {
                var topology = new WriterRibbonTabTopology(tab, capabilities);

                topology.Section(
                    "table.select",
                    tab => tab.Group("table-table", "Table", "T", 70, group =>
                        {
                            group.Medium("writer.table-select-table", "Select Table", RibbonCommandIconKind.Table);
                            group.Medium("writer.table-select-row", "Select Row", RibbonCommandIconKind.Table);
                            group.RowBreak();
                            group.Medium("writer.table-select-col", "Select Column", RibbonCommandIconKind.Table);
                            group.Medium("writer.table-select-cell", "Select Cell", RibbonCommandIconKind.Table);
                            group.RowBreak();
                            group.Medium("writer.table-view-gridlines", "View Gridlines", RibbonCommandIconKind.Grid);
                            group.Medium("writer.table-properties", "Properties", RibbonCommandIconKind.Table,
                                accent: RibbonCommandIconAccent.Green);
                        }),
                    tab => tab.Group("table-select", "Table", null, 110, group =>
                        {
                            group.Button("writer.table-select-table", "Select Table", control => control with
                            {
                                Icon = new RibbonCommandIcon(RibbonCommandIconKind.Table),
                            });
                            group.Button("writer.table-select-row", "Select Row");
                            group.Button("writer.table-select-col", "Select Column");
                            group.Button("writer.table-select-cell", "Select Cell");
                            group.Toggle("writer.table-view-gridlines", "View Gridlines");
                            group.Button("writer.table-properties", "Properties");
                        }));

                topology.Section(
                    "table.rows-columns",
                    tab => tab.Group("table-rows-cols", "Rows & Columns", "R", 120, group =>
                        {
                            group.Medium("writer.table-insert-above", "Insert Above", RibbonCommandIconKind.Insert,
                                accent: RibbonCommandIconAccent.Green);
                            group.Medium("writer.table-insert-below", "Insert Below", RibbonCommandIconKind.Insert,
                                accent: RibbonCommandIconAccent.Green);
                            group.RowBreak();
                            group.Medium("writer.table-insert-col-left", "Insert Left", RibbonCommandIconKind.Insert,
                                accent: RibbonCommandIconAccent.Green);
                            group.Medium("writer.table-insert-col-right", "Insert Right", RibbonCommandIconKind.Insert,
                                accent: RibbonCommandIconAccent.Green);
                            group.RowBreak();
                            group.Medium("writer.table-delete-row", "Delete Rows", RibbonCommandIconKind.Delete);
                            group.Medium("writer.table-delete-col", "Delete Columns", RibbonCommandIconKind.Delete);
                            group.RowBreak();
                            group.Medium("writer.table-delete", "Delete Table", RibbonCommandIconKind.Delete);
                        }),
                    tab => tab.Group("table-rows-cols", "Rows & Columns", null, 100, group =>
                        {
                            group.Button("writer.table-insert-above", "Insert Above", control => control with
                            {
                                Icon = new RibbonCommandIcon(RibbonCommandIconKind.Insert, RibbonCommandIconAccent.Green),
                            });
                            group.Button("writer.table-insert-below", "Insert Below");
                            group.Button("writer.table-insert-col-left", "Insert Left");
                            group.Button("writer.table-insert-col-right", "Insert Right");
                            group.Button("writer.table-delete-row", "Delete Row");
                            group.Button("writer.table-delete-col", "Delete Column");
                            group.Button("writer.table-delete", "Delete Table");
                        }));

                topology.Section(
                    "table.merge",
                    tab => tab.Group("table-merge", "Merge", "M", 90, group =>
                        {
                            group.Medium("writer.table-merge-cells", "Merge Cells", RibbonCommandIconKind.Merge);
                            group.Medium("writer.table-split-cell", "Split Cell", RibbonCommandIconKind.Grid);
                            group.RowBreak();
                            group.Medium("writer.split-table", "Split Table", RibbonCommandIconKind.Grid);
                        }),
                    tab => tab.Group("table-merge", "Merge", null, 90, group =>
                        {
                            group.Button("writer.table-merge-cells", "Merge Cells", control => control with
                            {
                                Icon = new RibbonCommandIcon(RibbonCommandIconKind.Merge),
                            });
                            group.Button("writer.table-split-cell", "Split Cell");
                            group.Button("writer.split-table", "Split Table");
                        }));

                topology.Section(
                    "table.cell-size",
                    tab => tab.Group("table-cell-size", "Cell Size", "Z", 100, group =>
                        {
                            group.Medium("writer.table-row-height", "Row Height", RibbonCommandIconKind.Size);
                            group.Medium("writer.table-col-width", "Column Width", RibbonCommandIconKind.Size);
                            group.RowBreak();
                            group.Medium("writer.table-distribute-rows", "Distribute Rows", RibbonCommandIconKind.Grid);
                            group.Medium("writer.table-distribute-cols", "Distribute Columns", RibbonCommandIconKind.Grid);
                            group.RowBreak();
                            group.Medium("writer.table-autofit-contents", "AutoFit Contents", RibbonCommandIconKind.Scale);
                            group.Medium("writer.table-autofit-window", "AutoFit Window", RibbonCommandIconKind.Scale);
                            group.Medium("writer.table-autofit-fixed", "Fixed Column Width", RibbonCommandIconKind.Size);
                        }),
                    tab => tab.Group("table-cell-size", "Cell Size", null, 95, group =>
                        {
                            group.Button("writer.table-row-height", "Row Height", control => control with
                            {
                                Icon = new RibbonCommandIcon(RibbonCommandIconKind.Size),
                            });
                            group.Button("writer.table-col-width", "Column Width");
                            group.Button("writer.table-distribute-rows", "Distribute Rows");
                            group.Button("writer.table-distribute-cols", "Distribute Columns");
                            group.Button("writer.table-autofit-contents", "AutoFit Contents");
                            group.Button("writer.table-autofit-window", "AutoFit Window");
                            group.Button("writer.table-autofit-fixed", "Fixed Column Width");
                        }));

                topology.Section(
                    "table.alignment",
                    tab => tab.Group("table-alignment", "Alignment", "A", 110, group =>
                        {
                            group.Medium("writer.cell-align-top-left", "Top Left", RibbonCommandIconKind.AlignLeft);
                            group.Medium("writer.cell-align-top-center", "Top Center", RibbonCommandIconKind.AlignCenter);
                            group.Medium("writer.cell-align-top-right", "Top Right", RibbonCommandIconKind.AlignRight);
                            group.RowBreak();
                            group.Medium("writer.cell-align-middle-left", "Middle Left", RibbonCommandIconKind.AlignLeft);
                            group.Medium("writer.cell-align-middle-center", "Middle Center", RibbonCommandIconKind.AlignCenter);
                            group.Medium("writer.cell-align-middle-right", "Middle Right", RibbonCommandIconKind.AlignRight);
                            group.RowBreak();
                            group.Medium("writer.cell-align-bottom-left", "Bottom Left", RibbonCommandIconKind.AlignLeft);
                            group.Medium("writer.cell-align-bottom-center", "Bottom Center", RibbonCommandIconKind.AlignCenter);
                            group.Medium("writer.cell-align-bottom-right", "Bottom Right", RibbonCommandIconKind.AlignRight);
                            group.RowBreak();
                            group.Medium("writer.table-cell-margins", "Cell Margins", RibbonCommandIconKind.Margins);
                            group.RowBreak();
                            group.Medium("writer.cell-text-direction-horizontal", "Horizontal", RibbonCommandIconKind.AlignLeft);
                            group.Medium("writer.cell-text-direction-rotate90", "Rotate Text Up", RibbonCommandIconKind.AlignLeft);
                            group.Medium("writer.cell-text-direction-rotate270", "Rotate Text Down", RibbonCommandIconKind.AlignLeft);
                        }),
                    tab => tab.Group("table-alignment", "Alignment", null, 110, group =>
                        {
                            group.Button("writer.cell-align-top-left", "Top Left", control => control with
                            {
                                Icon = new RibbonCommandIcon(RibbonCommandIconKind.AlignLeft),
                            });
                            group.Button("writer.cell-align-top-center", "Top Center");
                            group.Button("writer.cell-align-top-right", "Top Right");
                            group.Button("writer.cell-align-middle-left", "Middle Left");
                            group.Button("writer.cell-align-middle-center", "Middle Center");
                            group.Button("writer.cell-align-middle-right", "Middle Right");
                            group.Button("writer.cell-align-bottom-left", "Bottom Left");
                            group.Button("writer.cell-align-bottom-center", "Bottom Center");
                            group.Button("writer.cell-align-bottom-right", "Bottom Right");
                            group.Button("writer.table-cell-margins", "Cell Margins");
                            group.Button("writer.cell-text-direction-horizontal", "Horizontal");
                            group.Button("writer.cell-text-direction-rotate90", "Rotate Text Up");
                            group.Button("writer.cell-text-direction-rotate270", "Rotate Text Down");
                        }));

                topology.Section(
                    "table.data",
                    tab => tab.Group("table-data", "Data", "D", 70, group =>
                        {
                            group.Medium("writer.table-repeat-header", "Repeat Header Row", RibbonCommandIconKind.Table,
                                accent: RibbonCommandIconAccent.Green);
                            group.Medium("writer.table-formula", "Formula", RibbonCommandIconKind.Sum,
                                accent: RibbonCommandIconAccent.Green);
                            group.RowBreak();
                            group.Medium("writer.sort", "Sort", RibbonCommandIconKind.Sort);
                            group.Medium("writer.table-to-text", "Convert to Text", RibbonCommandIconKind.TextFunction);
                        }),
                    tab => tab.Group("table-data", "Data", null, 80, group =>
                        {
                            group.Toggle("writer.table-repeat-header", "Repeat Header Row", control => control with
                            {
                                Icon = new RibbonCommandIcon(RibbonCommandIconKind.Table, RibbonCommandIconAccent.Green),
                            });
                            group.Button("writer.table-formula", "Formula");
                            group.Button("writer.sort", "Sort");
                            group.Button("writer.table-to-text", "Convert to Text");
                        }));

                topology.Build();
            });

    private static readonly string[] FloatSizes = WriterRibbonDefinitionData.FloatSizes;

    private static void BuildWordArtStyleMenu(RibbonMenuBuilder menu)
    {
        for (var index = 0; index < WordArtRibbonWorkflow.StylePresets.Count; index++)
        {
            if (index == 4)
                menu.Separator();
            var preset = WordArtRibbonWorkflow.StylePresets[index];
            menu.Item(preset.CommandId.Value, preset.Label, preset.KeyTip);
        }
    }

    private static void BuildWordArtWarpMenu(RibbonMenuBuilder menu)
    {
        for (var index = 0; index < WordArtRibbonWorkflow.WarpPresets.Count; index++)
        {
            if (index == 1)
                menu.Separator();
            var preset = WordArtRibbonWorkflow.WarpPresets[index];
            menu.Item(preset.CommandId.Value, preset.Label, preset.KeyTip);
        }
    }

    private static RibbonMenu BuildWrapMenu(string prefix) =>
        new(new RibbonMenuItem[]
        {
            new("In Line with Text", new RibbonCommandId($"writer.{prefix}-wrap-inline")),
            new("Square", new RibbonCommandId($"writer.{prefix}-wrap-square")),
            new("Tight", new RibbonCommandId($"writer.{prefix}-wrap-tight")),
            new("Top and Bottom", new RibbonCommandId($"writer.{prefix}-wrap-top-bottom")),
            new("Behind Text", new RibbonCommandId($"writer.{prefix}-wrap-behind")),
            new("In Front of Text", new RibbonCommandId($"writer.{prefix}-wrap-front")),
        });

    private static RibbonMenu BuildRotateMenu(string prefix) =>
        new(new RibbonMenuItem[]
        {
            new("Rotate Right 90\u00B0", new RibbonCommandId($"writer.{prefix}-rotate-right90")),
            new("Rotate Left 90\u00B0", new RibbonCommandId($"writer.{prefix}-rotate-left90")),
            RibbonMenuItem.Separator(),
            new("Flip Vertical", new RibbonCommandId($"writer.{prefix}-flip-vertical")),
            new("Flip Horizontal", new RibbonCommandId($"writer.{prefix}-flip-horizontal")),
        });

    private static RibbonMenu BuildFloatingPositionMenu(string prefix) =>
        new(WriterRibbonDefinitionData.FloatingPositionPresets
            .Select(preset => new RibbonMenuItem(
                preset.Label,
                new RibbonCommandId($"writer.{prefix}-position-{preset.Suffix}")))
            .Concat(prefix == "image"
                ? [RibbonMenuItem.Separator(), new RibbonMenuItem("More Layout Options...", new RibbonCommandId($"writer.{prefix}-position"))]
                : [])
            .ToArray());

    private static RibbonMenu BuildPictureCorrectionsMenu() =>
        new(new RibbonMenuItem[]
        {
            new("Brightness: +20%", new RibbonCommandId("writer.image-brightness-plus20"), "1"),
            new("Brightness: +40%", new RibbonCommandId("writer.image-brightness-plus40"), "2"),
            new("Brightness: -20%", new RibbonCommandId("writer.image-brightness-minus20"), "3"),
            new("Brightness: -40%", new RibbonCommandId("writer.image-brightness-minus40"), "4"),
            new("Contrast: +20%", new RibbonCommandId("writer.image-contrast-plus20"), "5"),
            new("Contrast: -20%", new RibbonCommandId("writer.image-contrast-minus20"), "6"),
            new("Picture Corrections\u2026", new RibbonCommandId("writer.image-adjust-dialog"), "D"),
        });

    private static RibbonMenu BuildPictureColorMenu() =>
        new(new RibbonMenuItem[]
        {
            new("Saturation: 0% (Greyscale)", new RibbonCommandId("writer.image-saturation-0"), "G"),
            new("Saturation: 50%", new RibbonCommandId("writer.image-saturation-50"), "H"),
            new("Saturation: 200%", new RibbonCommandId("writer.image-saturation-200"), "J"),
            new("Color\u2026", new RibbonCommandId("writer.image-color-dialog"), "C"),
            RibbonMenuItem.Separator(),
            new("Recolor: Grayscale", new RibbonCommandId("writer.image-recolor-grayscale"), "1"),
            new("Recolor: Sepia", new RibbonCommandId("writer.image-recolor-sepia"), "2"),
            new("Recolor: Washout", new RibbonCommandId("writer.image-recolor-washout"), "3"),
            new("Recolor: Black and White", new RibbonCommandId("writer.image-recolor-blackwhite"), "4"),
            new("Recolor: No Recolor", new RibbonCommandId("writer.image-recolor-none"), "N"),
            RibbonMenuItem.Separator(),
            new("Color Tone: Warm (3000K)", new RibbonCommandId("writer.image-colortemp-warm"), "W"),
            new("Color Tone: Cool (8000K)", new RibbonCommandId("writer.image-colortemp-cool"), "L"),
            new("Color Tone: Neutral", new RibbonCommandId("writer.image-colortemp-neutral"), "T"),
        });

    private static RibbonMenu BuildPictureTransparencyMenu() =>
        new(new RibbonMenuItem[]
        {
            new("Transparency: 25%", new RibbonCommandId("writer.image-transparency-25"), "A"),
            new("Transparency: 50%", new RibbonCommandId("writer.image-transparency-50"), "B"),
            new("Transparency: 75%", new RibbonCommandId("writer.image-transparency-75"), "C"),
            new("Transparency\u2026", new RibbonCommandId("writer.image-transparency-dialog"), "D"),
        });

    private static RibbonMenu BuildPictureEffectsMenu() =>
        new(new RibbonMenuItem[]
        {
            new("Shadow: No Shadow", new RibbonCommandId("writer.image-shadow-none"), "N"),
            new("Shadow: Offset Diagonal", new RibbonCommandId("writer.image-shadow-1"), "1"),
            new("Shadow: Offset Diagonal Medium", new RibbonCommandId("writer.image-shadow-2"), "2"),
            new("Shadow: Perspective", new RibbonCommandId("writer.image-shadow-3"), "3"),
            new("Shadow: Offset Bottom", new RibbonCommandId("writer.image-shadow-4"), "4"),
            new("Shadow: Large", new RibbonCommandId("writer.image-shadow-5"), "5"),
            RibbonMenuItem.Separator(),
            new("Reflection: No Reflection", new RibbonCommandId("writer.image-reflection-none"), "R"),
            new("Reflection: Tight, Touching", new RibbonCommandId("writer.image-reflection-1"), "A"),
            new("Reflection: Tight, 4pt", new RibbonCommandId("writer.image-reflection-2"), "B"),
            new("Reflection: Tight, 8pt", new RibbonCommandId("writer.image-reflection-3"), "C"),
            new("Reflection: Half, Touching", new RibbonCommandId("writer.image-reflection-4"), "D"),
            new("Reflection: Half, 4pt", new RibbonCommandId("writer.image-reflection-5"), "E"),
            RibbonMenuItem.Separator(),
            new("Glow: No Glow", new RibbonCommandId("writer.image-glow-none"), "G"),
            new("Glow: 5 pt", new RibbonCommandId("writer.image-glow-5"), "H"),
            new("Glow: 8 pt", new RibbonCommandId("writer.image-glow-8"), "I"),
            new("Glow: 11 pt", new RibbonCommandId("writer.image-glow-11"), "J"),
            new("Glow: 18 pt", new RibbonCommandId("writer.image-glow-18"), "K"),
            RibbonMenuItem.Separator(),
            new("Soft Edges: None", new RibbonCommandId("writer.image-softedge-none"), "S"),
            new("Soft Edges: 1 pt", new RibbonCommandId("writer.image-softedge-1"), "T"),
            new("Soft Edges: 2.5 pt", new RibbonCommandId("writer.image-softedge-2pt5"), "U"),
            new("Soft Edges: 5 pt", new RibbonCommandId("writer.image-softedge-5"), "V"),
            new("Soft Edges: 10 pt", new RibbonCommandId("writer.image-softedge-10"), "X"),
            RibbonMenuItem.Separator(),
            new("Bevel: No Bevel", new RibbonCommandId("writer.image-bevel-none"), "O"),
            new("Bevel: Circle", new RibbonCommandId("writer.image-bevel-1"), "P"),
            new("Bevel: Relaxed Inset", new RibbonCommandId("writer.image-bevel-2"), "Q"),
            new("Bevel: Cross", new RibbonCommandId("writer.image-bevel-3"), "F"),
            new("Bevel: Cool Slant", new RibbonCommandId("writer.image-bevel-4"), "M"),
        });

    private static RibbonMenu BuildPictureArtisticEffectsMenu() =>
        new(new RibbonMenuItem[]
        {
            new("No Artistic Effect", new RibbonCommandId("writer.image-artistic-none"), "N"),
            new("Blur", new RibbonCommandId("writer.image-artistic-blur"), "B"),
            new("Glow Diffused", new RibbonCommandId("writer.image-artistic-glow-diffused"), "G"),
            new("Glow Edges", new RibbonCommandId("writer.image-artistic-glow-edges"), "E"),
            new("Pencil Grayscale", new RibbonCommandId("writer.image-artistic-pencil-gray"), "A"),
            new("Pencil Sketch", new RibbonCommandId("writer.image-artistic-pencil-sketch"), "K"),
            new("Line Drawing", new RibbonCommandId("writer.image-artistic-line-drawing"), "L"),
            new("Paint Brush", new RibbonCommandId("writer.image-artistic-paintbrush"), "P"),
            new("Paint Strokes", new RibbonCommandId("writer.image-artistic-paint-strokes"), "T"),
            new("Photocopy", new RibbonCommandId("writer.image-artistic-photocopy"), "H"),
            new("Posterize", new RibbonCommandId("writer.image-artistic-posterize"), "O"),
            new("Pastels", new RibbonCommandId("writer.image-artistic-pastels"), "S"),
            new("Watercolor Sponge", new RibbonCommandId("writer.image-artistic-watercolor"), "W"),
            new("Film Grain", new RibbonCommandId("writer.image-artistic-film-grain"), "F"),
            new("Mosaic Bubbles", new RibbonCommandId("writer.image-artistic-mosaic"), "M"),
        });

    private static RibbonMenu BuildShapeSizeMenu() =>
        new(WriterRibbonDefinitionData.FloatingSizePresets
            .Select(preset => new RibbonMenuItem(
                preset.Label,
                new RibbonCommandId($"writer.shape-size-{preset.Suffix}")))
            .ToArray());

    private static RibbonMenu BuildShapeAltTextMenu() =>
        new(WriterRibbonDefinitionData.ShapeAltTextPresets
            .Select(preset => new RibbonMenuItem(
                preset.Label,
                new RibbonCommandId($"writer.shape-alt-text-{preset.Suffix}")))
            .ToArray());

    private static RibbonMenu BuildShapeStylesMenu() =>
        new(ShapeStylePreset.Catalog
            .Select(preset => new RibbonMenuItem(
                preset.Name,
                new RibbonCommandId($"writer.{preset.Id}")))
            .ToArray());

    private static RibbonMenu BuildShapeChangeMenu() =>
        new(new RibbonMenuItem[]
        {
            new("Rectangle", new RibbonCommandId("writer.shape-change-rectangle")),
            new("Rounded Rectangle", new RibbonCommandId("writer.shape-change-rounded")),
            new("Ellipse", new RibbonCommandId("writer.shape-change-ellipse")),
        });

    private static RibbonMenu BuildShapeEditMenu() =>
        new(new RibbonMenuItem[]
        {
            new("Convert to Freeform", new RibbonCommandId("writer.shape-convert-freeform")),
            new("Edit Points", new RibbonCommandId("writer.shape-edit-points")),
        });

    private static RibbonMenu BuildShapeFillMenu() =>
        new(new RibbonMenuItem[]
        {
            new("No Fill", new RibbonCommandId("writer.shape-fill-no-fill")),
            RibbonMenuItem.Separator(),
            new("Gradient Blue", new RibbonCommandId("writer.shape-fill-gradient-blue")),
            new("Gradient Orange", new RibbonCommandId("writer.shape-fill-gradient-orange")),
            new("Pattern Diagonal", new RibbonCommandId("writer.shape-fill-pattern-diag")),
        });

    private static RibbonMenu BuildShapeOutlineMenu() =>
        new(new RibbonMenuItem[]
        {
            new("No Outline", new RibbonCommandId("writer.shape-outline-no-outline")),
            new("Solid", new RibbonCommandId("writer.shape-outline-solid")),
            new("Dash", new RibbonCommandId("writer.shape-outline-dash")),
            new("Dot", new RibbonCommandId("writer.shape-outline-dot")),
        });

    private static RibbonMenu BuildShapeEffectsMenu() =>
        new(new RibbonMenuItem[]
        {
            new("None", new RibbonCommandId("writer.shape-effects-none")),
            RibbonMenuItem.Separator(),
            new("Shadow", new RibbonCommandId("writer.shape-effect-shadow")),
            new("Glow", new RibbonCommandId("writer.shape-effect-glow")),
            new("Soft Edges", new RibbonCommandId("writer.shape-effect-soft-edge")),
            new("Reflection", new RibbonCommandId("writer.shape-effect-reflection")),
            new("Bevel", new RibbonCommandId("writer.shape-effect-bevel")),
        });

    private static RibbonMenu BuildShapeTextDirectionMenu() =>
        new(new RibbonMenuItem[]
        {
            new("Horizontal", new RibbonCommandId("writer.shape-text-horizontal")),
            new("Rotate 90\u00B0", new RibbonCommandId("writer.shape-text-rotate90")),
            new("Rotate 270\u00B0", new RibbonCommandId("writer.shape-text-rotate270")),
        });

    private static RibbonMenu BuildChartTypeMenu() =>
        new(Enum.GetValues<ChartKind>()
            .Select(kind => new RibbonMenuItem(
                kind.ToString(),
                new RibbonCommandId($"writer.chart-type-{kind.ToString().ToLowerInvariant()}")))
            .ToArray());

    private static RibbonMenu BuildChartStyleMenu() =>
        new(ChartStyle.Catalog
            .Select(style => new RibbonMenuItem(
                style.Name,
                new RibbonCommandId($"writer.chart-style-{style.Id}")))
            .ToArray());

    private static RibbonMenu BuildChartColorsMenu() =>
        new(ChartColorScheme.Catalog
            .Select(scheme => new RibbonMenuItem(
                scheme.Name,
                new RibbonCommandId(ChartColorRibbonCommandCatalog.CommandId(scheme))))
            .ToArray());

    private static RibbonMenu BuildSmartArtLayoutMenu() =>
        new(SmartArtLayoutPreset.Catalog
            .Select(preset => new RibbonMenuItem(
                preset.Name,
                new RibbonCommandId($"writer.smartart-layout-{preset.Id}")))
            .ToArray());

    private static RibbonMenu BuildSmartArtColorsMenu() =>
        new(SmartArtColorScheme.Catalog
            .Select(scheme => new RibbonMenuItem(
                scheme.Name,
                new RibbonCommandId($"writer.smartart-colors-{scheme.Id}")))
            .ToArray());

    private static RibbonMenu BuildSmartArtStylesMenu() =>
        new(SmartArtStyle.Catalog
            .Select(style => new RibbonMenuItem(
                style.Name,
                SmartArtCommandPlanner.StyleCommandId(style)))
            .ToArray());

    private static RibbonMenu BuildTableBordersMenu() =>
        new(new RibbonMenuItem[]
        {
            new("All Borders", new RibbonCommandId("writer.table-borders.all")),
            new("Outside Borders", new RibbonCommandId("writer.table-borders.outside")),
            new("Inside Borders", new RibbonCommandId("writer.table-borders.inside")),
            new("No Border", new RibbonCommandId("writer.table-borders.none")),
            RibbonMenuItem.Separator(),
            new("Top Border", new RibbonCommandId("writer.table-borders.top")),
            new("Bottom Border", new RibbonCommandId("writer.table-borders.bottom")),
            new("Left Border", new RibbonCommandId("writer.table-borders.left")),
            new("Right Border", new RibbonCommandId("writer.table-borders.right")),
        });

    private static RibbonMenu BuildTableStylesMenu() => WriterContextMenuPlanner.BuildTableStyles();
}
