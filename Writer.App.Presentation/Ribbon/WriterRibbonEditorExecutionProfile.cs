using Writer.Shared.Ribbon;
using Writer.App.Presentation.Dialogs;
using Writer.App.Presentation.DocumentView;
using Writer.App.Presentation.Editing;
using Writer.Core.Model;

namespace Writer.App.Presentation.Ribbon;

/// <summary>
/// Typed renderer boundary for editor-owned ribbon behavior. Presentation owns command construction,
/// state gating, command-value parsing, and planner/catalog expansion; renderers only adapt their
/// editor surface and native dialogs to these ports.
/// </summary>
public sealed record WriterRibbonEditorCommandFamilyPorts(
    IReadOnlyDictionary<WriterRibbonCommandAction, IRibbonCommand> Commands,
    IReadOnlyDictionary<RibbonCommandId, IRibbonCommand>? AdapterCommands = null);

/// <summary>
/// Collects renderer-native commands without mutating the application registry. The editor profile
/// remains the sole owner of canonical command-id registration for these families.
/// </summary>
public sealed class WriterRibbonEditorCommandFamilyBuilder
{
    private readonly WriterRibbonCommandBindingPorts _bindings = new();

    public IRibbonCommand Bind(WriterRibbonCommandAction action, IRibbonCommand command) =>
        _bindings.Bind(action, command);

    public IRibbonCommand BindAction(
        WriterRibbonCommandAction action,
        Action execute,
        Func<bool>? isEnabled = null,
        Action? prepareExecution = null) =>
        _bindings.BindAction(action, execute, isEnabled, prepareExecution);

    public IRibbonStatefulCommand BindToggle(
        WriterRibbonCommandAction action,
        Action toggle,
        Func<bool> isChecked,
        Func<bool>? isEnabled = null,
        Action? prepareExecution = null) =>
        _bindings.BindToggle(action, toggle, isChecked, isEnabled, prepareExecution);

    public void Register(RibbonCommandId commandId, IRibbonCommand command) =>
        _bindings.Register(commandId, command);

    public WriterRibbonEditorCommandFamilyPorts Build() =>
        new(
            _bindings.CanonicalBindings.ToDictionary(static pair => pair.Key, static pair => pair.Value),
            _bindings.AdapterBindings.ToDictionary(static pair => pair.Key, static pair => pair.Value));
}

public sealed record WriterRibbonFloatingFeedback(string Title, string Message);

public interface IWriterRibbonFloatingPositionPreset
{
    string Suffix { get; }
    double HorizontalOffsetPt { get; }
    double VerticalOffsetPt { get; }
    HorizontalAnchor HorizontalAnchor { get; }
    VerticalAnchor VerticalAnchor { get; }
}

public static class WriterRibbonFloatingFeedbackCatalog
{
    public static readonly WriterRibbonFloatingFeedback EditShape = new(
        "Edit Shape",
        "Choose 'Convert to Freeform' or 'Edit Points' from the menu.");

    public static readonly WriterRibbonFloatingFeedback TextDirection = new(
        "Text Direction",
        "Choose a text direction from the dropdown.");

    public static readonly WriterRibbonFloatingFeedback ShapeEffects = new(
        "Shape Effects",
        "Choose an effect from the dropdown.");

    public static readonly WriterRibbonFloatingFeedback ShapeStyles = new(
        "Shape Styles",
        "Choose a shape style from the gallery.");

    public static readonly WriterRibbonFloatingFeedback GroupSelectionRequired = new(
        "Group",
        "Select two or more floating objects first (Shift-click or Ctrl-click).");

    public static readonly WriterRibbonFloatingFeedback UngroupSelectionRequired = new(
        "Ungroup",
        "Select a group first.");
}

public sealed record WriterRibbonFloatingExecutionPorts(
    Action PrepareExecution,
    Func<ObjectFormatTarget, bool> HasSelection,
    Func<bool> HasTransformSelection,
    Action<ObjectFormatTarget, ImageWrapping> ApplyWrap,
    Func<ObjectFormatTarget, ObjectFormatTransformCommand, bool> ApplyTransform,
    Func<ObjectFormatTarget, ZOrderOperation, bool> ApplyZOrder,
    Action<ObjectFormatTarget, ObjectFormatSizeDimension, double> ApplySize,
    Action<ObjectFormatTarget, TextAlignment> ApplyParagraphAlignment,
    Func<FloatingObjectArrangeKind, bool> CanArrange,
    Action<FloatingObjectArrangeKind> Arrange,
    Func<Shape?> SelectedShape,
    Action<ShapeKind> SetShapeKind,
    Action ConvertShapeToFreeform,
    Action BeginShapeEditPoints,
    Action<ShapeTextDirection> SetShapeTextDirection,
    Action<ShapeFill?> SetShapeExtendedFill,
    Action<string?> SetShapeFill,
    Action<string?, double, string?> SetShapeOutline,
    Action<ShapeEffectLst?> SetShapeEffects,
    Action<ShapeStylePreset> ApplyShapeStyle,
    Func<bool> CanGroup,
    Action Group,
    Func<bool> CanUngroup,
    Action Ungroup,
    Action<WriterRibbonFloatingFeedback>? ShowFeedback = null,
    Action<ObjectFormatTarget, WriterRibbonObjectPositionInput>? ApplyPosition = null,
    Action? ToggleSelectionPane = null);

public sealed record WriterRibbonChartSmartArtExecutionPorts(
    Action PrepareExecution,
    Action CompleteExecution,
    Func<Chart?> SelectedChart,
    Action<ChartKind> SetChartKind,
    Action<ChartStyle> ApplyChartStyle,
    Action<ChartColorScheme> ApplyChartColorScheme,
    Action<ChartQuickLayout> ApplyChartQuickLayout,
    Action ToggleChartLegend,
    Func<Chart, ValueTask<ChartTitleDialogResult?>>? ShowChartTitleDialogAsync,
    Action<ChartTitleDialogResult> ApplyChartTitleOutcome,
    Action? ToggleChartTitleFallback,
    Func<Chart, ValueTask<ChartAxisTitlesDialogResult?>>? ShowChartAxisTitlesDialogAsync,
    Action<ChartAxisTitlesDialogResult> ApplyChartAxisTitlesOutcome,
    Action? ToggleChartAxisTitlesFallback,
    Func<Chart, ValueTask<Chart?>>? ShowChartDataDialogAsync,
    Action<Chart> ApplyChartDataOutcome,
    Func<Chart, ValueTask<ChartSizeDialogResult?>>? ShowChartSizeDialogAsync,
    Action<ChartSizeDialogResult> ApplyChartSizeOutcome,
    Func<SmartArt?> SelectedSmartArt,
    Action<SmartArtStructureOperation> MutateSmartArt,
    Action<SmartArtLayoutPreset> ApplySmartArtLayout,
    Action<SmartArtColorScheme> ApplySmartArtColorScheme,
    Action<SmartArtStyle> ApplySmartArtStyle,
    Func<SmartArt, ValueTask<SmartArt?>>? ShowSmartArtEditDialogAsync,
    Action<SmartArt> ApplySmartArtEditOutcome,
    Action<ChartStyle>? PreviewChartStyle = null,
    Action<ChartColorScheme>? PreviewChartColorScheme = null,
    Action<ChartQuickLayout>? PreviewChartQuickLayout = null,
    Action? CancelChartDesignPreview = null,
    Action<ChartStyle>? CommitChartStyle = null,
    Action<ChartColorScheme>? CommitChartColorScheme = null,
    Action<ChartQuickLayout>? CommitChartQuickLayout = null,
    Action<SmartArtLayoutPreset>? PreviewSmartArtLayout = null,
    Action<SmartArtColorScheme>? PreviewSmartArtColorScheme = null,
    Action<SmartArtStyle>? PreviewSmartArtStyle = null,
    Action? CancelSmartArtDesignPreview = null,
    Action<SmartArtLayoutPreset>? CommitSmartArtLayout = null,
    Action<SmartArtColorScheme>? CommitSmartArtColorScheme = null,
    Action<SmartArtStyle>? CommitSmartArtStyle = null);

public sealed record WriterRibbonChartSmartArtCommands(
    IRibbonStatefulCommand ChartLegend);

public sealed record WriterRibbonImageExecutionPorts(
    Action PrepareExecution,
    Action CompleteExecution,
    Func<InlineImage?> SelectedImage,
    Func<InlineImage, ValueTask<ImageCropDialogResult?>>? ShowCropDialogAsync,
    Action<ImageCropDialogResult> ApplyCropOutcome,
    Action ResetImage);

public sealed record WriterRibbonTableCellSelection(Table Table, int RowIndex, int ColumnIndex);

public sealed record WriterRibbonTableExecutionPorts(
    Action PrepareExecution,
    Action CompleteExecution,
    Func<WriterRibbonTableCellSelection?> SelectedCell,
    Func<ModelTableContext?> SelectedContext,
    Func<bool> CanConvertToText,
    Func<TableFormulaDialogInitialState, ValueTask<TableFormulaField?>>? ShowFormulaDialogAsync,
    Action<TableFormulaField> ApplyFormulaOutcome,
    Func<ModelTableContext, ValueTask<TablePropertiesValues?>>? ShowPropertiesDialogAsync,
    Action<TablePropertiesValues> ApplyPropertiesOutcome,
    Func<ValueTask<char?>>? ShowTableToTextDialogAsync,
    Action<char> ApplyTableToTextOutcome);

public static class WriterRibbonEditorExecutionProfile
{
    public static IReadOnlyList<WriterRibbonCommandAction> TableActions { get; } =
    [
        WriterRibbonCommandAction.Table,
        WriterRibbonCommandAction.TableHeaderRow,
        WriterRibbonCommandAction.TableBandedRows,
        WriterRibbonCommandAction.TableLastRow,
        WriterRibbonCommandAction.TableFirstColumn,
        WriterRibbonCommandAction.TableLastColumn,
        WriterRibbonCommandAction.TableBandedCols,
        WriterRibbonCommandAction.DrawTable,
        WriterRibbonCommandAction.Eraser,
        WriterRibbonCommandAction.TableViewGridlines,
        WriterRibbonCommandAction.TableProperties,
        WriterRibbonCommandAction.TableSelectTable,
        WriterRibbonCommandAction.TableSelectRow,
        WriterRibbonCommandAction.TableSelectCol,
        WriterRibbonCommandAction.TableSelectCell,
        WriterRibbonCommandAction.TableInsertAbove,
        WriterRibbonCommandAction.TableInsertBelow,
        WriterRibbonCommandAction.TableInsertColLeft,
        WriterRibbonCommandAction.TableInsertColRight,
        WriterRibbonCommandAction.TableMergeCells,
        WriterRibbonCommandAction.TableSplitCell,
        WriterRibbonCommandAction.TableShading,
        WriterRibbonCommandAction.TableBorders,
        WriterRibbonCommandAction.TableDeleteRow,
        WriterRibbonCommandAction.TableDeleteCol,
        WriterRibbonCommandAction.TableDelete,
        WriterRibbonCommandAction.SplitTable,
        WriterRibbonCommandAction.TableRowHeight,
        WriterRibbonCommandAction.TableColWidth,
        WriterRibbonCommandAction.TableDistributeRows,
        WriterRibbonCommandAction.TableDistributeCols,
        WriterRibbonCommandAction.TableAutofitContents,
        WriterRibbonCommandAction.TableAutofitWindow,
        WriterRibbonCommandAction.TableAutofitFixed,
        WriterRibbonCommandAction.CellAlignTopLeft,
        WriterRibbonCommandAction.CellAlignTopCenter,
        WriterRibbonCommandAction.CellAlignTopRight,
        WriterRibbonCommandAction.CellAlignMiddleLeft,
        WriterRibbonCommandAction.CellAlignMiddleCenter,
        WriterRibbonCommandAction.CellAlignMiddleRight,
        WriterRibbonCommandAction.CellAlignBottomLeft,
        WriterRibbonCommandAction.CellAlignBottomCenter,
        WriterRibbonCommandAction.CellAlignBottomRight,
        WriterRibbonCommandAction.TableCellMargins,
        WriterRibbonCommandAction.CellTextDirectionHorizontal,
        WriterRibbonCommandAction.CellTextDirectionRotate90,
        WriterRibbonCommandAction.CellTextDirectionRotate270,
        WriterRibbonCommandAction.TableRepeatHeader,
        WriterRibbonCommandAction.TableFormula,
        WriterRibbonCommandAction.TableToText,
    ];

    public static IReadOnlyList<WriterRibbonCommandAction> ReferenceActions { get; } =
    [
        WriterRibbonCommandAction.Footnote,
        WriterRibbonCommandAction.Endnote,
        WriterRibbonCommandAction.NextFootnote,
        WriterRibbonCommandAction.PreviousFootnote,
        WriterRibbonCommandAction.NextEndnote,
        WriterRibbonCommandAction.PreviousEndnote,
        WriterRibbonCommandAction.ShowNotes,
        WriterRibbonCommandAction.FootnoteEndnoteOptions,
        WriterRibbonCommandAction.Toc,
        WriterRibbonCommandAction.TocRefresh,
        WriterRibbonCommandAction.Caption,
        WriterRibbonCommandAction.InsertCaption_Figure,
        WriterRibbonCommandAction.InsertCaption_Table,
        WriterRibbonCommandAction.InsertCaption_Equation,
        WriterRibbonCommandAction.CrossReference,
        WriterRibbonCommandAction.Citation,
        WriterRibbonCommandAction.ManageSources,
        WriterRibbonCommandAction.CitationStyle,
        WriterRibbonCommandAction.Bibliography,
        WriterRibbonCommandAction.Tof,
        WriterRibbonCommandAction.Tof_Figure,
        WriterRibbonCommandAction.Tof_Table,
        WriterRibbonCommandAction.Tof_Equation,
        WriterRibbonCommandAction.TofRefresh,
        WriterRibbonCommandAction.TofRefresh_Figure,
        WriterRibbonCommandAction.TofRefresh_Table,
        WriterRibbonCommandAction.TofRefresh_Equation,
        WriterRibbonCommandAction.IndexMark,
        WriterRibbonCommandAction.IndexInsert,
        WriterRibbonCommandAction.IndexRefresh,
        WriterRibbonCommandAction.MarkCitation,
        WriterRibbonCommandAction.TableOfAuthorities,
        WriterRibbonCommandAction.TableOfAuthoritiesRefresh,
    ];

    public static IReadOnlyList<WriterRibbonCommandAction> HeaderFooterActions { get; } =
    [
        WriterRibbonCommandAction.Header,
        WriterRibbonCommandAction.Footer,
        WriterRibbonCommandAction.PageNumber,
        WriterRibbonCommandAction.PageNumberTop,
        WriterRibbonCommandAction.PageNumberBottom,
        WriterRibbonCommandAction.PageNumberCurrent,
        WriterRibbonCommandAction.PageNumberFormat,
        WriterRibbonCommandAction.Datetime,
        WriterRibbonCommandAction.HfEditHeader,
        WriterRibbonCommandAction.HfEditFooter,
        WriterRibbonCommandAction.HfEditFirstHeader,
        WriterRibbonCommandAction.HfEditFirstFooter,
        WriterRibbonCommandAction.HfEditEvenHeader,
        WriterRibbonCommandAction.HfEditEvenFooter,
        WriterRibbonCommandAction.HfGoToHeader,
        WriterRibbonCommandAction.HfGoToFooter,
        WriterRibbonCommandAction.HfClose,
        WriterRibbonCommandAction.HfDifferentFirstPage,
        WriterRibbonCommandAction.HfDifferentOddEven,
        WriterRibbonCommandAction.HfHeaderFromTop,
        WriterRibbonCommandAction.HfFooterFromBottom,
        WriterRibbonCommandAction.HfInsertPageNumber,
        WriterRibbonCommandAction.HfInsertPageNumberFooter,
        WriterRibbonCommandAction.HfInsertDatetime,
        WriterRibbonCommandAction.HfInsertField,
    ];

    public static void RegisterFamilies(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonEditorCommandFamilyPorts tables,
        WriterRibbonEditorCommandFamilyPorts references,
        WriterRibbonEditorCommandFamilyPorts headerFooter)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        RegisterFamily(bindings, tables);
        RegisterFamily(bindings, references);
        RegisterFamily(bindings, headerFooter);
    }

    public static void RegisterFamily(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonEditorCommandFamilyPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(ports);

        foreach (var (action, command) in ports.Commands)
            bindings.Bind(action, command);

        if (ports.AdapterCommands is null)
            return;

        foreach (var (commandId, command) in ports.AdapterCommands)
            bindings.Register(commandId, command);
    }

    public static void RegisterFloatingPositionCommands(
        IRibbonCommandRegistry registry,
        string prefix,
        WriterRibbonFloatingObjectCommandPorts ports,
        IEnumerable<IWriterRibbonFloatingPositionPreset> presets)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
        ArgumentNullException.ThrowIfNull(ports);
        ArgumentNullException.ThrowIfNull(presets);

        registry.Register(
            $"writer.{prefix}-position",
            WriterRibbonFloatingObjectCommandFactory.CreatePosition(ports));
        foreach (var preset in presets)
        {
            var captured = preset;
            registry.Register(
                $"writer.{prefix}-position-{captured.Suffix}",
                WriterRibbonFloatingObjectCommandFactory.CreatePositionPreset(
                    ports,
                    new WriterRibbonObjectPositionInput(
                        captured.HorizontalOffsetPt,
                        captured.VerticalOffsetPt,
                        captured.HorizontalAnchor,
                        captured.VerticalAnchor)));
        }
    }

    public static void RegisterFloating(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonFloatingExecutionPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(ports);

        foreach (var target in ObjectFormatCommandPlanner.Targets)
        {
            bindings.Register(
                ObjectFormatCommandPlanner.WrapDropdownCommandId(target),
                EmptyRibbonCommand.Instance);
            foreach (var command in ObjectFormatCommandPlanner.WrapCommands(target))
            {
                var captured = command;
                bindings.Register(captured.CommandId, Stateful(
                    () => ports.ApplyWrap(target, captured.Wrapping),
                    () => ports.HasSelection(target),
                    ports.PrepareExecution));
            }

            bindings.Register(
                ObjectFormatCommandPlanner.TransformDropdownCommandId(target),
                EmptyRibbonCommand.Instance);
            foreach (var command in ObjectFormatCommandPlanner.TransformCommands(target))
            {
                var captured = command;
                bindings.Register(captured.CommandId, Stateful(
                    () => ports.ApplyTransform(target, captured),
                    ports.HasTransformSelection,
                    ports.PrepareExecution));
            }

            foreach (var command in ObjectFormatCommandPlanner.ZOrderCommands(target))
            {
                var captured = command;
                bindings.Register(captured.CommandId, Stateful(
                    () => ports.ApplyZOrder(target, captured.Operation),
                    () => ports.HasSelection(target),
                    ports.PrepareExecution));
            }

            foreach (var command in ObjectFormatCommandPlanner.SizeCommands(target))
            {
                var captured = command;
                bindings.Register(captured.CommandId, new WriterRibbonStatefulPortCommand(
                    context =>
                    {
                        if (ObjectFormatCommandPlanner.TryParseSizePoints(context.SelectedValue, out var points))
                            ports.ApplySize(target, captured.Dimension, points);
                    },
                    () => new RibbonCommandState(IsEnabled: ports.HasSelection(target)),
                    ports.PrepareExecution));
            }
        }

        BindAlignmentCommands(bindings, ports, ObjectFormatTarget.Picture);
        BindAlignmentCommands(bindings, ports, ObjectFormatTarget.Shape);
        BindArrangeCommands(bindings, ports, ObjectFormatTarget.Picture);
        BindArrangeCommands(bindings, ports, ObjectFormatTarget.Shape);
        RegisterLayoutArrangeCommands(bindings, ports);

        bindings.Bind(WriterRibbonCommandAction.ShapeEditShape, Stateful(
            () => ports.ShowFeedback?.Invoke(WriterRibbonFloatingFeedbackCatalog.EditShape),
            () => ports.ShowFeedback is not null || ports.SelectedShape() is not null,
            ports.PrepareExecution));
        bindings.Bind(WriterRibbonCommandAction.ShapeConvertFreeform, Stateful(
            ports.ConvertShapeToFreeform,
            () => ports.SelectedShape() is not null,
            ports.PrepareExecution));
        bindings.Bind(WriterRibbonCommandAction.ShapeEditPoints, Stateful(
            () =>
            {
                if (ports.SelectedShape() is { HasCustomGeometry: false })
                    ports.ConvertShapeToFreeform();
                ports.BeginShapeEditPoints();
            },
            () => ports.SelectedShape() is not null,
            ports.PrepareExecution));

        BindShapeKind(bindings, ports, WriterRibbonCommandAction.ShapeChangeRectangle, ShapeKind.Rectangle);
        BindShapeKind(bindings, ports, WriterRibbonCommandAction.ShapeChangeRounded, ShapeKind.RoundedRectangle);
        BindShapeKind(bindings, ports, WriterRibbonCommandAction.ShapeChangeEllipse, ShapeKind.Ellipse);
        bindings.Register("writer.shape-change", Stateful(
            static () => { },
            () => ports.SelectedShape() is not null,
            ports.PrepareExecution));

        bindings.Bind(WriterRibbonCommandAction.ShapeTextDirection, Stateful(
            () => ports.ShowFeedback?.Invoke(WriterRibbonFloatingFeedbackCatalog.TextDirection),
            () => ports.ShowFeedback is not null || ports.SelectedShape() is not null,
            ports.PrepareExecution));
        BindShapeTextDirection(bindings, ports, WriterRibbonCommandAction.ShapeTextHorizontal, ShapeTextDirection.Horizontal);
        BindShapeTextDirection(bindings, ports, WriterRibbonCommandAction.ShapeTextRotate90, ShapeTextDirection.Rotate90);
        BindShapeTextDirection(bindings, ports, WriterRibbonCommandAction.ShapeTextRotate270, ShapeTextDirection.Rotate270);

        RegisterShapeFillOutline(bindings, ports);
        BindShapeEffects(bindings, ports);

        bindings.Bind(WriterRibbonCommandAction.ShapeStylesGallery, new WriterRibbonStatefulPortCommand(
            context =>
            {
                if (ports.ShowFeedback is not null)
                {
                    ports.ShowFeedback(WriterRibbonFloatingFeedbackCatalog.ShapeStyles);
                    return;
                }

                var preset = ShapeStylePreset.Catalog.FirstOrDefault(item =>
                    string.Equals(item.Id, context.SelectedValue, StringComparison.OrdinalIgnoreCase));
                if (preset is not null)
                    ports.ApplyShapeStyle(preset);
            },
            () => new RibbonCommandState(IsEnabled: ports.ShowFeedback is not null || CanFormatShape(ports)),
            ports.PrepareExecution));
        foreach (var preset in ShapeStylePreset.Catalog)
        {
            var captured = preset;
            bindings.Register($"writer.{captured.Id}", Stateful(
                () => ports.ApplyShapeStyle(captured),
                () => CanFormatShape(ports),
                ports.PrepareExecution));
        }

        bindings.Bind(WriterRibbonCommandAction.ObjectGroup, Stateful(
            () => ExecuteOrShowFeedback(
                ports.CanGroup,
                ports.Group,
                ports.ShowFeedback,
                WriterRibbonFloatingFeedbackCatalog.GroupSelectionRequired),
            () => ports.ShowFeedback is not null || ports.CanGroup(),
            ports.PrepareExecution));
        bindings.Bind(WriterRibbonCommandAction.ObjectUngroup, Stateful(
            () => ExecuteOrShowFeedback(
                ports.CanUngroup,
                ports.Ungroup,
                ports.ShowFeedback,
                WriterRibbonFloatingFeedbackCatalog.UngroupSelectionRequired),
            () => ports.ShowFeedback is not null || ports.CanUngroup(),
            ports.PrepareExecution));

    }

    public static WriterRibbonChartSmartArtCommands RegisterChartSmartArt(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonChartSmartArtExecutionPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(ports);

        bindings.Register("writer.chart-type", EmptyRibbonCommand.Instance);
        foreach (var kind in Enum.GetValues<ChartKind>())
        {
            var captured = kind;
            bindings.Register($"writer.chart-type-{captured.ToString().ToLowerInvariant()}", Stateful(
                () => ports.SetChartKind(captured),
                () => ports.SelectedChart() is not null,
                ports.PrepareExecution));
        }

        bindings.Register("writer.chart-style", EmptyRibbonCommand.Instance);
        foreach (var style in ChartStyle.Catalog)
        {
            var captured = style;
            bindings.Register(
                $"writer.chart-style-{captured.Id}",
                ChartGalleryCommand(
                    captured,
                    ports.ApplyChartStyle,
                    ports.PreviewChartStyle,
                    ports.CancelChartDesignPreview,
                    ports.CommitChartStyle,
                    ports));
        }

        foreach (var layout in ChartQuickLayout.Catalog)
        {
            var captured = layout;
            bindings.Register(
                $"writer.chart-quick-layout-{captured.Id}",
                ChartGalleryCommand(
                    captured,
                    ports.ApplyChartQuickLayout,
                    ports.PreviewChartQuickLayout,
                    ports.CancelChartDesignPreview,
                    ports.CommitChartQuickLayout,
                    ports));
        }

        bindings.Register(ChartColorRibbonCommandCatalog.ParentCommandId, EmptyRibbonCommand.Instance);
        foreach (var scheme in ChartColorScheme.Catalog)
        {
            var captured = scheme;
            bindings.Register(
                ChartColorRibbonCommandCatalog.CommandId(captured),
                ChartGalleryCommand(
                    captured,
                    ports.ApplyChartColorScheme,
                    ports.PreviewChartColorScheme,
                    ports.CancelChartDesignPreview,
                    ports.CommitChartColorScheme,
                    ports));
        }

        var chartLegend = new WriterRibbonStatefulPortCommand(
            _ => ports.ToggleChartLegend(),
            () => BuildChartLegendState(ports.SelectedChart()),
            ports.PrepareExecution);
        bindings.Bind(WriterRibbonCommandAction.ChartToggleLegend, chartLegend);
        bindings.Bind(WriterRibbonCommandAction.ChartTitle, AsyncStateful(
            _ => ExecuteSelectedDialogAsync(
                ports.SelectedChart,
                ports.ShowChartTitleDialogAsync,
                ports.ApplyChartTitleOutcome,
                ports.CompleteExecution,
                ports.ToggleChartTitleFallback),
            () => ports.SelectedChart() is not null
                && (ports.ShowChartTitleDialogAsync is not null || ports.ToggleChartTitleFallback is not null),
            ports.PrepareExecution));
        bindings.Bind(WriterRibbonCommandAction.ChartAxisTitles, AsyncStateful(
            _ => ExecuteSelectedDialogAsync(
                ports.SelectedChart,
                ports.ShowChartAxisTitlesDialogAsync,
                ports.ApplyChartAxisTitlesOutcome,
                ports.CompleteExecution,
                ports.ToggleChartAxisTitlesFallback),
            () => ports.SelectedChart() is not null
                && (ports.ShowChartAxisTitlesDialogAsync is not null || ports.ToggleChartAxisTitlesFallback is not null),
            ports.PrepareExecution));
        bindings.Bind(WriterRibbonCommandAction.ChartEditData, AsyncStateful(
            context => ExecuteChartDataAsync(ports, context.SelectedValue),
            () => ports.SelectedChart() is not null,
            ports.PrepareExecution));
        var chartSizeCommand = AsyncStateful(
            context => ExecuteChartSizeAsync(ports, context.SelectedValue),
            () => ports.SelectedChart() is not null,
            ports.PrepareExecution);
        bindings.Bind(WriterRibbonCommandAction.ChartSize, chartSizeCommand);
        bindings.Bind(WriterRibbonCommandAction.ChartSizeDialog, chartSizeCommand);

        bindings.Register("writer.smartart-layout", EmptyRibbonCommand.Instance);
        foreach (var preset in SmartArtLayoutPreset.Catalog)
        {
            var captured = preset;
            bindings.Register(
                $"writer.smartart-layout-{captured.Id}",
                CatalogGalleryCommand(
                    captured,
                    ports.ApplySmartArtLayout,
                    ports.PreviewSmartArtLayout,
                    ports.CancelSmartArtDesignPreview,
                    ports.CommitSmartArtLayout,
                    () => SmartArtCommandPlanner.CanEdit(ports.SelectedSmartArt()),
                    ports.PrepareExecution));
        }
        RegisterSmartArtLayoutAlias(bindings, ports, "writer.smartart-layout-list", SmartArtKind.List);
        RegisterSmartArtLayoutAlias(bindings, ports, "writer.smartart-layout-process", SmartArtKind.Process);
        RegisterSmartArtLayoutAlias(bindings, ports, "writer.smartart-layout-cycle", SmartArtKind.Process);
        RegisterSmartArtLayoutAlias(bindings, ports, "writer.smartart-layout-hierarchy", SmartArtKind.Hierarchy);

        bindings.Register("writer.smartart-colors", EmptyRibbonCommand.Instance);
        foreach (var scheme in SmartArtColorScheme.Catalog)
        {
            var captured = scheme;
            bindings.Register(
                $"writer.smartart-colors-{captured.Id}",
                CatalogGalleryCommand(
                    captured,
                    ports.ApplySmartArtColorScheme,
                    ports.PreviewSmartArtColorScheme,
                    ports.CancelSmartArtDesignPreview,
                    ports.CommitSmartArtColorScheme,
                    () => SmartArtCommandPlanner.CanEdit(ports.SelectedSmartArt()),
                    ports.PrepareExecution));
        }

        foreach (var style in SmartArtStyle.Catalog)
        {
            var captured = style;
            bindings.Register(
                SmartArtCommandPlanner.StyleCommandId(captured),
                CatalogGalleryCommand(
                    captured,
                    ports.ApplySmartArtStyle,
                    ports.PreviewSmartArtStyle,
                    ports.CancelSmartArtDesignPreview,
                    ports.CommitSmartArtStyle,
                    () => SmartArtCommandPlanner.CanEdit(ports.SelectedSmartArt()),
                    ports.PrepareExecution));
        }

        BindSmartArtStructure(bindings, ports, WriterRibbonCommandAction.SmartartAddShape, SmartArtStructureOperation.AddShape);
        BindSmartArtStructure(bindings, ports, WriterRibbonCommandAction.SmartartRemoveShape, SmartArtStructureOperation.RemoveShape);
        BindSmartArtStructure(bindings, ports, WriterRibbonCommandAction.SmartartPromote, SmartArtStructureOperation.Promote);
        BindSmartArtStructure(bindings, ports, WriterRibbonCommandAction.SmartartDemote, SmartArtStructureOperation.Demote);
        BindSmartArtStructure(bindings, ports, WriterRibbonCommandAction.SmartartMoveUp, SmartArtStructureOperation.MoveUp);
        BindSmartArtStructure(bindings, ports, WriterRibbonCommandAction.SmartartMoveDown, SmartArtStructureOperation.MoveDown);
        bindings.Bind(WriterRibbonCommandAction.SmartartEditText, AsyncStateful(
            context => ExecuteSmartArtEditAsync(ports, context.SelectedValue),
            () => SmartArtCommandPlanner.CanEdit(ports.SelectedSmartArt()),
            ports.PrepareExecution));
        bindings.Bind(WriterRibbonCommandAction.SmartartChangeStyle, new WriterRibbonStatefulPortCommand(
            context =>
            {
                if (SmartArtCommandPlanner.ResolveStyle(context.SelectedValue) is { } style)
                    (ports.CommitSmartArtStyle ?? ports.ApplySmartArtStyle)(style);
            },
            () => new RibbonCommandState(IsEnabled: SmartArtCommandPlanner.CanEdit(ports.SelectedSmartArt())),
            ports.PrepareExecution));

        return new WriterRibbonChartSmartArtCommands(chartLegend);
    }

    public static RibbonCommandState BuildChartLegendState(Chart? chart)
    {
        if (chart is null)
            return new RibbonCommandState(IsEnabled: false, IsChecked: false);

        var state = ChartSmartArtVisualPlanner.BuildChartElementCommandState(chart);
        return new RibbonCommandState(
            IsEnabled: state.CanToggleLegend,
            IsChecked: state.IsLegendVisible);
    }

    public static void RegisterImageTableWorkflows(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonImageExecutionPorts imagePorts,
        WriterRibbonTableExecutionPorts tablePorts)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(imagePorts);
        ArgumentNullException.ThrowIfNull(tablePorts);

        bindings.Bind(WriterRibbonCommandAction.ImageCrop, AsyncStateful(
            _ => ExecuteSelectedDialogAsync(
                imagePorts.SelectedImage,
                imagePorts.ShowCropDialogAsync,
                imagePorts.ApplyCropOutcome,
                imagePorts.CompleteExecution),
            () => imagePorts.SelectedImage() is not null && imagePorts.ShowCropDialogAsync is not null,
            imagePorts.PrepareExecution));
        bindings.Bind(WriterRibbonCommandAction.ImageReset, Stateful(
            imagePorts.ResetImage,
            () => imagePorts.SelectedImage() is not null,
            imagePorts.PrepareExecution));

        bindings.Bind(WriterRibbonCommandAction.TableFormula, AsyncStateful(
            _ => ExecuteTableFormulaAsync(tablePorts),
            () => tablePorts.SelectedCell() is not null && tablePorts.ShowFormulaDialogAsync is not null,
            tablePorts.PrepareExecution));
        var propertiesCommand = AsyncStateful(
            _ => ExecuteSelectedDialogAsync(
                tablePorts.SelectedContext,
                tablePorts.ShowPropertiesDialogAsync,
                tablePorts.ApplyPropertiesOutcome,
                tablePorts.CompleteExecution),
            () => tablePorts.SelectedContext() is not null && tablePorts.ShowPropertiesDialogAsync is not null,
            tablePorts.PrepareExecution);
        bindings.Bind(WriterRibbonCommandAction.TableProperties, propertiesCommand);
        bindings.Bind(WriterRibbonCommandAction.TableRowHeight, propertiesCommand);
        bindings.Bind(WriterRibbonCommandAction.TableColWidth, propertiesCommand);
        bindings.Bind(WriterRibbonCommandAction.TableCellMargins, propertiesCommand);
        bindings.Bind(WriterRibbonCommandAction.TableToText, AsyncStateful(
            _ => ExecuteTableToTextAsync(tablePorts),
            () => tablePorts.CanConvertToText() && tablePorts.ShowTableToTextDialogAsync is not null,
            tablePorts.PrepareExecution));
    }

    private static ValueTask ExecuteChartDataAsync(
        WriterRibbonChartSmartArtExecutionPorts ports,
        string? selectedValue)
    {
        if (ports.SelectedChart() is not { } chart)
            return Complete(ports.CompleteExecution);
        if (ChartDataPresetCatalog.TryCreateNamedReplacement(selectedValue, out var preset))
        {
            ports.ApplyChartDataOutcome(preset);
            return Complete(ports.CompleteExecution);
        }
        return !string.IsNullOrWhiteSpace(selectedValue) || ports.ShowChartDataDialogAsync is null
            ? Complete(ports.CompleteExecution)
            : ApplyDialogOutcomeAsync(
                ports.ShowChartDataDialogAsync(chart),
                ports.ApplyChartDataOutcome,
                ports.CompleteExecution);
    }

    private static ValueTask ExecuteChartSizeAsync(
        WriterRibbonChartSmartArtExecutionPorts ports,
        string? selectedValue)
    {
        if (ports.SelectedChart() is not { } chart)
            return Complete(ports.CompleteExecution);
        if (WriterRibbonNumericValueParser.TryParseChartSize(
                selectedValue,
                System.Globalization.CultureInfo.InvariantCulture,
                out var parsed))
        {
            ports.ApplyChartSizeOutcome(new ChartSizeDialogResult(parsed.WidthPt, parsed.HeightPt));
            return Complete(ports.CompleteExecution);
        }
        return !string.IsNullOrWhiteSpace(selectedValue) || ports.ShowChartSizeDialogAsync is null
            ? Complete(ports.CompleteExecution)
            : ApplyDialogOutcomeAsync(
                ports.ShowChartSizeDialogAsync(chart),
                ports.ApplyChartSizeOutcome,
                ports.CompleteExecution);
    }

    private static ValueTask ExecuteSmartArtEditAsync(
        WriterRibbonChartSmartArtExecutionPorts ports,
        string? selectedValue)
    {
        if (ports.SelectedSmartArt() is not { } smartArt)
            return Complete(ports.CompleteExecution);
        if (selectedValue is not null)
        {
            if (SmartArtCommandPlanner.BuildEditedContent(smartArt.Kind, selectedValue) is { } replacement)
                ports.ApplySmartArtEditOutcome(replacement);
            return Complete(ports.CompleteExecution);
        }
        return ports.ShowSmartArtEditDialogAsync is null
            ? Complete(ports.CompleteExecution)
            : ApplyDialogOutcomeAsync(
                ports.ShowSmartArtEditDialogAsync(smartArt),
                ports.ApplySmartArtEditOutcome,
                ports.CompleteExecution);
    }

    private static ValueTask ExecuteTableFormulaAsync(WriterRibbonTableExecutionPorts ports)
    {
        if (ports.SelectedCell() is not { } cell || ports.ShowFormulaDialogAsync is null)
            return Complete(ports.CompleteExecution);
        var initialState = TableFormulaDialogPlanner.BuildInitialState(
            cell.Table,
            cell.RowIndex,
            cell.ColumnIndex);
        return ApplyDialogOutcomeAsync(
            ports.ShowFormulaDialogAsync(initialState),
            ports.ApplyFormulaOutcome,
            ports.CompleteExecution);
    }

    private static async ValueTask ExecuteTableToTextAsync(WriterRibbonTableExecutionPorts ports)
    {
        try
        {
            if (ports.ShowTableToTextDialogAsync is null)
                return;
            if (await ports.ShowTableToTextDialogAsync() is { } outcome)
                ports.ApplyTableToTextOutcome(outcome);
        }
        finally
        {
            ports.CompleteExecution();
        }
    }

    private static ValueTask ExecuteSelectedDialogAsync<TSelection, TOutcome>(
        Func<TSelection?> selected,
        Func<TSelection, ValueTask<TOutcome?>>? showDialogAsync,
        Action<TOutcome> applyOutcome,
        Action completeExecution,
        Action? fallback = null)
        where TSelection : class
        where TOutcome : class
    {
        var selection = selected();
        if (selection is not null && showDialogAsync is not null)
            return ApplyDialogOutcomeAsync(showDialogAsync(selection), applyOutcome, completeExecution);

        if (selection is not null)
            fallback?.Invoke();
        completeExecution();
        return ValueTask.CompletedTask;
    }

    private static async ValueTask ApplyDialogOutcomeAsync<TOutcome>(
        ValueTask<TOutcome?> pendingOutcome,
        Action<TOutcome> applyOutcome,
        Action completeExecution)
        where TOutcome : class
    {
        try
        {
            if (await pendingOutcome is { } outcome)
                applyOutcome(outcome);
        }
        finally
        {
            completeExecution();
        }
    }

    private static ValueTask Complete(Action completeExecution)
    {
        completeExecution();
        return ValueTask.CompletedTask;
    }

    private static void RegisterShapeFillOutline(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonFloatingExecutionPorts ports)
    {
        bindings.Register(ObjectFormatCommandPlanner.ShapeFillCommandId, Stateful(
            static () => { },
            () => CanFormatShape(ports),
            ports.PrepareExecution));
        foreach (var command in ObjectFormatCommandPlanner.ShapeFillCommands())
        {
            var captured = command;
            bindings.Register(captured.CommandId, Stateful(
                () =>
                {
                    if (captured.Kind == ObjectFormatShapeFillKind.NoFill)
                    {
                        ports.SetShapeExtendedFill(null);
                        ports.SetShapeFill(null);
                    }
                    else if (ObjectFormatCommandPlanner.UsesExtendedShapeFill(captured.Kind))
                    {
                        ports.SetShapeExtendedFill(ObjectFormatCommandPlanner.BuildShapeExtendedFill(captured.Kind));
                    }
                },
                () => CanFormatShape(ports),
                ports.PrepareExecution));
        }

        bindings.Register(ObjectFormatCommandPlanner.ShapeOutlineCommandId, Stateful(
            static () => { },
            () => CanFormatShape(ports),
            ports.PrepareExecution));
        foreach (var command in ObjectFormatCommandPlanner.ShapeOutlineCommands())
        {
            var captured = command;
            bindings.Register(captured.CommandId, Stateful(
                () =>
                {
                    var shape = ports.SelectedShape();
                    if (shape is null)
                        return;
                    var plan = ObjectFormatCommandPlanner.PlanShapeOutline(
                        captured.Kind,
                        shape.OutlineColorHex,
                        shape.OutlineWidthPt);
                    ports.SetShapeOutline(plan.ColorHex, plan.WidthPt, plan.Dash);
                },
                () => CanFormatShape(ports),
                ports.PrepareExecution));
        }
    }

    private static void BindShapeEffects(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonFloatingExecutionPorts ports)
    {
        bindings.Bind(WriterRibbonCommandAction.ShapeEffects, Stateful(
            () =>
            {
                if (ports.ShowFeedback is not null)
                    ports.ShowFeedback(WriterRibbonFloatingFeedbackCatalog.ShapeEffects);
                else
                    ports.SetShapeEffects(null);
            },
            () => ports.ShowFeedback is not null || ports.SelectedShape() is not null,
            ports.PrepareExecution));
        BindShapeEffect(bindings, ports, WriterRibbonCommandAction.ShapeEffectsNone, null);
        BindShapeEffect(bindings, ports, WriterRibbonCommandAction.ShapeEffectShadow, new ShapeEffectLst { HasShadow = true });
        BindShapeEffect(bindings, ports, WriterRibbonCommandAction.ShapeEffectGlow, new ShapeEffectLst { HasGlow = true });
        BindShapeEffect(bindings, ports, WriterRibbonCommandAction.ShapeEffectSoftEdge, new ShapeEffectLst { HasSoftEdge = true });
        BindShapeEffect(bindings, ports, WriterRibbonCommandAction.ShapeEffectReflection, new ShapeEffectLst { HasReflection = true });
        BindShapeEffect(bindings, ports, WriterRibbonCommandAction.ShapeEffectBevel, new ShapeEffectLst { HasBevel = true });
    }

    private static void BindShapeEffect(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonFloatingExecutionPorts ports,
        WriterRibbonCommandAction action,
        ShapeEffectLst? effect) =>
        bindings.Bind(action, Stateful(
            () => ports.SetShapeEffects(effect?.Clone()),
            () => ports.SelectedShape() is not null,
            ports.PrepareExecution));

    private static void BindAlignmentCommands(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonFloatingExecutionPorts ports,
        ObjectFormatTarget target)
    {
        var left = target == ObjectFormatTarget.Picture
            ? WriterRibbonCommandAction.ImageAlignLeft
            : WriterRibbonCommandAction.ShapeAlignLeft;
        var center = target == ObjectFormatTarget.Picture
            ? WriterRibbonCommandAction.ImageAlignCenter
            : WriterRibbonCommandAction.ShapeAlignCenter;
        var right = target == ObjectFormatTarget.Picture
            ? WriterRibbonCommandAction.ImageAlignRight
            : WriterRibbonCommandAction.ShapeAlignRight;

        BindAlignment(bindings, ports, target, left, TextAlignment.Left);
        BindAlignment(bindings, ports, target, center, TextAlignment.Center);
        BindAlignment(bindings, ports, target, right, TextAlignment.Right);
    }

    private static void BindAlignment(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonFloatingExecutionPorts ports,
        ObjectFormatTarget target,
        WriterRibbonCommandAction action,
        TextAlignment alignment) =>
        bindings.Bind(action, Stateful(
            () => ports.ApplyParagraphAlignment(target, alignment),
            () => ports.HasSelection(target),
            ports.PrepareExecution));

    private static void BindArrangeCommands(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonFloatingExecutionPorts ports,
        ObjectFormatTarget target)
    {
        var alignPage = target == ObjectFormatTarget.Picture
            ? WriterRibbonCommandAction.ImageAlignToPage
            : WriterRibbonCommandAction.ShapeAlignToPage;
        var alignMargin = target == ObjectFormatTarget.Picture
            ? WriterRibbonCommandAction.ImageAlignToMargin
            : WriterRibbonCommandAction.ShapeAlignToMargin;
        var distributeH = target == ObjectFormatTarget.Picture
            ? WriterRibbonCommandAction.ImageDistributeH
            : WriterRibbonCommandAction.ShapeDistributeH;
        var distributeV = target == ObjectFormatTarget.Picture
            ? WriterRibbonCommandAction.ImageDistributeV
            : WriterRibbonCommandAction.ShapeDistributeV;

        BindArrange(bindings, ports, alignPage, FloatingObjectArrangeKind.AlignToPage);
        BindArrange(bindings, ports, alignMargin, FloatingObjectArrangeKind.AlignToMargin);
        BindArrange(bindings, ports, distributeH, FloatingObjectArrangeKind.DistributeHorizontal);
        BindArrange(bindings, ports, distributeV, FloatingObjectArrangeKind.DistributeVertical);
    }

    private static void BindArrange(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonFloatingExecutionPorts ports,
        WriterRibbonCommandAction action,
        FloatingObjectArrangeKind kind) =>
        bindings.Bind(action, Stateful(
            () => ports.Arrange(kind),
            () => ports.CanArrange(kind),
            ports.PrepareExecution));

    private static void RegisterLayoutArrangeCommands(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonFloatingExecutionPorts ports)
    {
        bindings.Register("writer.layout-wrap", EmptyRibbonCommand.Instance);
        bindings.Register("writer.layout-rotate", EmptyRibbonCommand.Instance);
        bindings.Register("writer.layout-position", EmptyRibbonCommand.Instance);
        bindings.Register(
            "writer.layout-selection-pane",
            Stateful(
                () => ports.ToggleSelectionPane?.Invoke(),
                () => ports.ToggleSelectionPane is not null,
                ports.PrepareExecution));

        foreach (var preset in LayoutPositionPresets)
        {
            var captured = preset;
            bindings.Register(
                $"writer.layout-position-{captured.Suffix}",
                Stateful(
                    () => TryWithSelectedLayoutTarget(ports, target => ports.ApplyPosition?.Invoke(target, captured.Input)),
                    () => ports.ApplyPosition is not null && HasLayoutSelection(ports),
                    ports.PrepareExecution));
        }

        foreach (var command in ObjectFormatCommandPlanner.WrapCommands(ObjectFormatTarget.Picture))
        {
            var captured = command;
            bindings.Register(
                LayoutCommandId(captured.CommandId),
                Stateful(
                    () => TryWithSelectedLayoutTarget(ports, target => ports.ApplyWrap(target, captured.Wrapping)),
                    () => HasLayoutSelection(ports),
                    ports.PrepareExecution));
        }

        foreach (var command in ObjectFormatCommandPlanner.ZOrderCommands(ObjectFormatTarget.Picture))
        {
            var captured = command;
            bindings.Register(
                LayoutCommandId(captured.CommandId),
                Stateful(
                    () => TryWithSelectedLayoutTarget(ports, target => ports.ApplyZOrder(target, captured.Operation)),
                    () => HasLayoutSelection(ports),
                    ports.PrepareExecution));
        }

        foreach (var command in ObjectFormatCommandPlanner.TransformCommands(ObjectFormatTarget.Picture))
        {
            var captured = command;
            bindings.Register(
                LayoutCommandId(captured.CommandId),
                Stateful(
                    () => TryWithSelectedLayoutTarget(ports, target => ports.ApplyTransform(target, captured)),
                    () => HasLayoutSelection(ports),
                    ports.PrepareExecution));
        }
    }

    private static bool HasLayoutSelection(WriterRibbonFloatingExecutionPorts ports) =>
        ports.HasSelection(ObjectFormatTarget.Picture) || ports.HasSelection(ObjectFormatTarget.Shape);

    private static void TryWithSelectedLayoutTarget(
        WriterRibbonFloatingExecutionPorts ports,
        Action<ObjectFormatTarget> apply)
    {
        if (ports.HasSelection(ObjectFormatTarget.Picture))
            apply(ObjectFormatTarget.Picture);
        else if (ports.HasSelection(ObjectFormatTarget.Shape))
            apply(ObjectFormatTarget.Shape);
    }

    private static string LayoutCommandId(string targetCommandId) =>
        targetCommandId.Replace("writer.image-", "writer.layout-", StringComparison.Ordinal);

    private static IReadOnlyList<(string Suffix, WriterRibbonObjectPositionInput Input)> LayoutPositionPresets { get; } =
    [
        ("column-paragraph", new(0, 0, HorizontalAnchor.Column, VerticalAnchor.Paragraph)),
        ("margin-paragraph", new(0, 0, HorizontalAnchor.Margin, VerticalAnchor.Paragraph)),
        ("page-paragraph", new(0, 0, HorizontalAnchor.Page, VerticalAnchor.Paragraph)),
        ("page-top", new(0, 0, HorizontalAnchor.Page, VerticalAnchor.Page)),
    ];

    private static void BindShapeKind(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonFloatingExecutionPorts ports,
        WriterRibbonCommandAction action,
        ShapeKind kind) =>
        bindings.Bind(action, Stateful(
            () => ports.SetShapeKind(kind),
            () => ports.SelectedShape() is not null,
            ports.PrepareExecution));

    private static void BindShapeTextDirection(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonFloatingExecutionPorts ports,
        WriterRibbonCommandAction action,
        ShapeTextDirection direction) =>
        bindings.Bind(action, Stateful(
            () => ports.SetShapeTextDirection(direction),
            () => ports.SelectedShape() is not null,
            ports.PrepareExecution));

    private static void BindSmartArtStructure(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonChartSmartArtExecutionPorts ports,
        WriterRibbonCommandAction action,
        SmartArtStructureOperation operation) =>
        bindings.Bind(action, Stateful(
            () => ports.MutateSmartArt(operation),
            () => SmartArtCommandPlanner.IsEnabled(ports.SelectedSmartArt(), operation),
            ports.PrepareExecution));

    private static void RegisterSmartArtLayoutAlias(
        WriterRibbonCommandBindingPorts bindings,
        WriterRibbonChartSmartArtExecutionPorts ports,
        RibbonCommandId commandId,
        SmartArtKind kind)
    {
        var preset = SmartArtLayoutPreset.Catalog.First(item => item.Kind == kind);
        bindings.Register(commandId, Stateful(
            () => ports.ApplySmartArtLayout(preset),
            () => SmartArtCommandPlanner.CanEdit(ports.SelectedSmartArt()),
            ports.PrepareExecution));
    }

    private static bool CanFormatShape(WriterRibbonFloatingExecutionPorts ports) =>
        ObjectFormatCommandPlanner.CanFormatShapeFillOutline(ports.SelectedShape()?.Kind);

    private static IRibbonStatefulCommand Stateful(
        Action execute,
        Func<bool> isEnabled,
        Action? prepareExecution = null) =>
        new WriterRibbonStatefulPortCommand(
            _ => execute(),
            () => new RibbonCommandState(IsEnabled: isEnabled()),
            prepareExecution);

    private static IRibbonStatefulCommand ChartGalleryCommand<T>(
        T value,
        Action<T> apply,
        Action<T>? preview,
        Action? cancelPreview,
        Action<T>? commit,
        WriterRibbonChartSmartArtExecutionPorts ports)
        where T : class =>
        CatalogGalleryCommand(
            value,
            apply,
            preview,
            cancelPreview,
            commit,
            () => ports.SelectedChart() is not null,
            ports.PrepareExecution);

    private static IRibbonStatefulCommand CatalogGalleryCommand<T>(
        T value,
        Action<T> apply,
        Action<T>? preview,
        Action? cancelPreview,
        Action<T>? commit,
        Func<bool> isEnabled,
        Action prepareExecution)
        where T : class =>
        preview is not null && cancelPreview is not null && commit is not null
            ? new PreviewableCatalogCommand<T>(
                value,
                preview,
                cancelPreview,
                commit,
                isEnabled,
                prepareExecution)
            : Stateful(
                () => apply(value),
                isEnabled,
                prepareExecution);

    private sealed class PreviewableCatalogCommand<T>(
        T value,
        Action<T> preview,
        Action cancelPreview,
        Action<T> commit,
        Func<bool> isEnabled,
        Action prepareExecution) : IRibbonPreviewCommand, IRibbonStatefulCommand
        where T : class
    {
        public void BeginPreview(RibbonCommandContext context) => preview(value);

        public void CancelPreview() => cancelPreview();

        public void Execute(RibbonCommandContext context)
        {
            prepareExecution();
            commit(value);
        }

        public RibbonCommandState GetState() => new(IsEnabled: isEnabled());
    }

    private static void ExecuteOrShowFeedback(
        Func<bool> canExecute,
        Action execute,
        Action<WriterRibbonFloatingFeedback>? showFeedback,
        WriterRibbonFloatingFeedback feedback)
    {
        if (canExecute())
            execute();
        else
            showFeedback?.Invoke(feedback);
    }

    private static IRibbonStatefulCommand AsyncStateful(
        Func<RibbonCommandContext, ValueTask> executeAsync,
        Func<bool> isEnabled,
        Action? prepareExecution = null) =>
        new WriterRibbonAsyncStatefulPortCommand(
            executeAsync,
            () => new RibbonCommandState(IsEnabled: isEnabled()),
            prepareExecution);
}
