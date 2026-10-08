using Writer.Shared.Ribbon;
using Writer.Core.Model;

namespace Writer.App.Presentation.Ribbon;

public sealed record TableEditingRibbonPorts(
    Action PrepareExecution,
    Func<TableFormatting?> CurrentTableFormatting,
    Func<bool> ViewGridlines,
    Action ToggleHeaderRow,
    Action ToggleBandedRows,
    Action ToggleLastRow,
    Action ToggleFirstColumn,
    Action ToggleLastColumn,
    Action ToggleBandedColumns,
    Action ToggleGridlines,
    Action SelectTable,
    Action SelectRow,
    Action SelectColumn,
    Action SelectCell,
    Action InsertRowAbove,
    Action InsertRowBelow,
    Action InsertColumnLeft,
    Action InsertColumnRight,
    Action MergeCells,
    IRibbonCommand SplitCell,
    IRibbonCommand Shading,
    IRibbonCommand Borders,
    Action DeleteRow,
    Action DeleteColumn,
    Action DeleteTable,
    Action SplitTable,
    Action DistributeRows,
    Action DistributeColumns,
    Action<AutoFitMode> SetAutoFit,
    Action<TableCellVerticalAlignment, TextAlignment> SetCellAlignment,
    Action<CellTextDirection> SetCellTextDirection,
    Action<CellBorderEdges, bool> SetCellBorders,
    Action ToggleRepeatHeaderRow);

public enum TableEditingRibbonToggleKind
{
    HeaderRow,
    BandedRows,
    LastRow,
    FirstColumn,
    LastColumn,
    BandedColumns,
    ViewGridlines,
    RepeatHeaderRow,
}

/// <summary>
/// Owns renderer-neutral Table Design and Table Layout command policy. WPF and Avalonia provide
/// only native editor adapters; command identity, option mapping, and execution preparation remain
/// shared so the two ribbon hosts cannot drift independently.
/// </summary>
public static class TableEditingRibbonWorkflow
{
    public static IReadOnlyList<WriterRibbonCommandAction> Actions { get; } =
    [
        WriterRibbonCommandAction.TableHeaderRow,
        WriterRibbonCommandAction.TableBandedRows,
        WriterRibbonCommandAction.TableLastRow,
        WriterRibbonCommandAction.TableFirstColumn,
        WriterRibbonCommandAction.TableLastColumn,
        WriterRibbonCommandAction.TableBandedCols,
        WriterRibbonCommandAction.TableViewGridlines,
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
        WriterRibbonCommandAction.CellTextDirectionHorizontal,
        WriterRibbonCommandAction.CellTextDirectionRotate90,
        WriterRibbonCommandAction.CellTextDirectionRotate270,
        WriterRibbonCommandAction.TableRepeatHeader,
    ];

    public static void Register(
        WriterRibbonEditorCommandFamilyBuilder bindings,
        TableEditingRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(ports);
        ArgumentNullException.ThrowIfNull(ports.PrepareExecution);

        BindToggle(bindings, ports, WriterRibbonCommandAction.TableHeaderRow, TableEditingRibbonToggleKind.HeaderRow, ports.ToggleHeaderRow);
        BindToggle(bindings, ports, WriterRibbonCommandAction.TableBandedRows, TableEditingRibbonToggleKind.BandedRows, ports.ToggleBandedRows);
        BindToggle(bindings, ports, WriterRibbonCommandAction.TableLastRow, TableEditingRibbonToggleKind.LastRow, ports.ToggleLastRow);
        BindToggle(bindings, ports, WriterRibbonCommandAction.TableFirstColumn, TableEditingRibbonToggleKind.FirstColumn, ports.ToggleFirstColumn);
        BindToggle(bindings, ports, WriterRibbonCommandAction.TableLastColumn, TableEditingRibbonToggleKind.LastColumn, ports.ToggleLastColumn);
        BindToggle(bindings, ports, WriterRibbonCommandAction.TableBandedCols, TableEditingRibbonToggleKind.BandedColumns, ports.ToggleBandedColumns);
        BindToggle(bindings, ports, WriterRibbonCommandAction.TableViewGridlines, TableEditingRibbonToggleKind.ViewGridlines, ports.ToggleGridlines);
        Bind(bindings, ports, WriterRibbonCommandAction.TableSelectTable, ports.SelectTable);
        Bind(bindings, ports, WriterRibbonCommandAction.TableSelectRow, ports.SelectRow);
        Bind(bindings, ports, WriterRibbonCommandAction.TableSelectCol, ports.SelectColumn);
        Bind(bindings, ports, WriterRibbonCommandAction.TableSelectCell, ports.SelectCell);
        Bind(bindings, ports, WriterRibbonCommandAction.TableInsertAbove, ports.InsertRowAbove);
        Bind(bindings, ports, WriterRibbonCommandAction.TableInsertBelow, ports.InsertRowBelow);
        Bind(bindings, ports, WriterRibbonCommandAction.TableInsertColLeft, ports.InsertColumnLeft);
        Bind(bindings, ports, WriterRibbonCommandAction.TableInsertColRight, ports.InsertColumnRight);
        Bind(bindings, ports, WriterRibbonCommandAction.TableMergeCells, ports.MergeCells);
        BindCommand(bindings, ports, WriterRibbonCommandAction.TableSplitCell, ports.SplitCell);
        BindCommand(bindings, ports, WriterRibbonCommandAction.TableShading, ports.Shading);
        BindCommand(bindings, ports, WriterRibbonCommandAction.TableBorders, ports.Borders);
        RegisterBorderPreset(bindings, ports, "writer.table-borders.all", CellBorderEdges.All);
        RegisterBorderPreset(bindings, ports, "writer.table-borders.outside", CellBorderEdges.Outside);
        RegisterBorderPreset(bindings, ports, "writer.table-borders.inside", CellBorderEdges.Inside);
        RegisterBorderPreset(bindings, ports, "writer.table-borders.none", CellBorderEdges.All, clearEdges: true);
        RegisterBorderPreset(bindings, ports, "writer.table-borders.top", CellBorderEdges.Top);
        RegisterBorderPreset(bindings, ports, "writer.table-borders.bottom", CellBorderEdges.Bottom);
        RegisterBorderPreset(bindings, ports, "writer.table-borders.left", CellBorderEdges.Left);
        RegisterBorderPreset(bindings, ports, "writer.table-borders.right", CellBorderEdges.Right);
        Bind(bindings, ports, WriterRibbonCommandAction.TableDeleteRow, ports.DeleteRow);
        Bind(bindings, ports, WriterRibbonCommandAction.TableDeleteCol, ports.DeleteColumn);
        Bind(bindings, ports, WriterRibbonCommandAction.TableDelete, ports.DeleteTable);
        Bind(bindings, ports, WriterRibbonCommandAction.SplitTable, ports.SplitTable);
        Bind(bindings, ports, WriterRibbonCommandAction.TableDistributeRows, ports.DistributeRows);
        Bind(bindings, ports, WriterRibbonCommandAction.TableDistributeCols, ports.DistributeColumns);
        Bind(bindings, ports, WriterRibbonCommandAction.TableAutofitContents, () => ports.SetAutoFit(AutoFitMode.Contents));
        Bind(bindings, ports, WriterRibbonCommandAction.TableAutofitWindow, () => ports.SetAutoFit(AutoFitMode.Window));
        Bind(bindings, ports, WriterRibbonCommandAction.TableAutofitFixed, () => ports.SetAutoFit(AutoFitMode.Fixed));

        BindAlignment(bindings, ports, WriterRibbonCommandAction.CellAlignTopLeft, TableCellVerticalAlignment.Top, TextAlignment.Left);
        BindAlignment(bindings, ports, WriterRibbonCommandAction.CellAlignTopCenter, TableCellVerticalAlignment.Top, TextAlignment.Center);
        BindAlignment(bindings, ports, WriterRibbonCommandAction.CellAlignTopRight, TableCellVerticalAlignment.Top, TextAlignment.Right);
        BindAlignment(bindings, ports, WriterRibbonCommandAction.CellAlignMiddleLeft, TableCellVerticalAlignment.Center, TextAlignment.Left);
        BindAlignment(bindings, ports, WriterRibbonCommandAction.CellAlignMiddleCenter, TableCellVerticalAlignment.Center, TextAlignment.Center);
        BindAlignment(bindings, ports, WriterRibbonCommandAction.CellAlignMiddleRight, TableCellVerticalAlignment.Center, TextAlignment.Right);
        BindAlignment(bindings, ports, WriterRibbonCommandAction.CellAlignBottomLeft, TableCellVerticalAlignment.Bottom, TextAlignment.Left);
        BindAlignment(bindings, ports, WriterRibbonCommandAction.CellAlignBottomCenter, TableCellVerticalAlignment.Bottom, TextAlignment.Center);
        BindAlignment(bindings, ports, WriterRibbonCommandAction.CellAlignBottomRight, TableCellVerticalAlignment.Bottom, TextAlignment.Right);

        Bind(bindings, ports, WriterRibbonCommandAction.CellTextDirectionHorizontal,
            () => ports.SetCellTextDirection(CellTextDirection.Horizontal));
        Bind(bindings, ports, WriterRibbonCommandAction.CellTextDirectionRotate90,
            () => ports.SetCellTextDirection(CellTextDirection.Rotate90));
        Bind(bindings, ports, WriterRibbonCommandAction.CellTextDirectionRotate270,
            () => ports.SetCellTextDirection(CellTextDirection.Rotate270));
        BindToggle(bindings, ports, WriterRibbonCommandAction.TableRepeatHeader, TableEditingRibbonToggleKind.RepeatHeaderRow, ports.ToggleRepeatHeaderRow);
    }

    public static RibbonCommandState BuildToggleState(
        TableFormatting? formatting,
        bool viewGridlines,
        TableEditingRibbonToggleKind kind)
    {
        if (formatting is null)
            return new RibbonCommandState(IsEnabled: false, IsChecked: false);

        var isChecked = kind switch
        {
            TableEditingRibbonToggleKind.HeaderRow => formatting.HeaderRow,
            TableEditingRibbonToggleKind.BandedRows => formatting.BandedRows,
            TableEditingRibbonToggleKind.LastRow => formatting.LastRow,
            TableEditingRibbonToggleKind.FirstColumn => formatting.FirstColumn,
            TableEditingRibbonToggleKind.LastColumn => formatting.LastColumn,
            TableEditingRibbonToggleKind.BandedColumns => formatting.BandedColumns,
            TableEditingRibbonToggleKind.ViewGridlines => viewGridlines,
            TableEditingRibbonToggleKind.RepeatHeaderRow => formatting.RepeatHeaderRow,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };
        return new RibbonCommandState(IsEnabled: true, IsChecked: isChecked);
    }

    private static void Bind(
        WriterRibbonEditorCommandFamilyBuilder bindings,
        TableEditingRibbonPorts ports,
        WriterRibbonCommandAction action,
        Action execute) =>
        bindings.BindAction(action, execute, prepareExecution: ports.PrepareExecution);

    private static void BindAlignment(
        WriterRibbonEditorCommandFamilyBuilder bindings,
        TableEditingRibbonPorts ports,
        WriterRibbonCommandAction action,
        TableCellVerticalAlignment vertical,
        TextAlignment horizontal) =>
        Bind(bindings, ports, action, () => ports.SetCellAlignment(vertical, horizontal));

    private static void BindToggle(
        WriterRibbonEditorCommandFamilyBuilder bindings,
        TableEditingRibbonPorts ports,
        WriterRibbonCommandAction action,
        TableEditingRibbonToggleKind kind,
        Action execute) =>
        BindCommand(
            bindings,
            ports,
            action,
            new TableToggleCommand(
                execute,
                () => BuildToggleState(
                    ports.CurrentTableFormatting(),
                    ports.ViewGridlines(),
                    kind)));

    private static void BindCommand(
        WriterRibbonEditorCommandFamilyBuilder bindings,
        TableEditingRibbonPorts ports,
        WriterRibbonCommandAction action,
        IRibbonCommand command) =>
        bindings.Bind(action, new PreparedCommand(ports.PrepareExecution, command));

    private static void RegisterBorderPreset(
        WriterRibbonEditorCommandFamilyBuilder bindings,
        TableEditingRibbonPorts ports,
        string commandId,
        CellBorderEdges edges,
        bool clearEdges = false) =>
        bindings.Register(
            new RibbonCommandId(commandId),
            new PreparedCommand(
                ports.PrepareExecution,
                new ActionRibbonCommand(() => ports.SetCellBorders(edges, clearEdges))));

    private sealed class PreparedCommand(Action prepareExecution, IRibbonCommand inner) : IRibbonStatefulCommand
    {
        public void Execute(RibbonCommandContext context)
        {
            prepareExecution();
            inner.Execute(context);
        }

        public RibbonCommandState GetState() =>
            inner is IRibbonStatefulCommand stateful
                ? stateful.GetState()
                : RibbonCommandState.Default;
    }

    private sealed class TableToggleCommand(
        Action execute,
        Func<RibbonCommandState> getState) : IRibbonStatefulCommand
    {
        public void Execute(RibbonCommandContext context) => execute();

        public RibbonCommandState GetState() => getState();
    }
}
