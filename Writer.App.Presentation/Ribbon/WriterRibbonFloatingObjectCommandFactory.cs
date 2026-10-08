using System.Globalization;
using Writer.Shared.Ribbon;

namespace Writer.App.Presentation.Ribbon;

public sealed record WriterRibbonFloatingObjectCommandPorts(
    Func<bool> HasSelection,
    Action<WriterRibbonObjectPositionInput> ApplyPosition,
    Action<double, double> ApplySize,
    Action? OpenPositionDialog = null,
    Action? OpenSizeDialog = null,
    Action? PrepareExecution = null);

/// <summary>
/// Owns selection gating, value parsing, preset execution, and dialog routing for the
/// image and shape position/size command families. Renderers provide only native ports.
/// </summary>
public static class WriterRibbonFloatingObjectCommandFactory
{
    public static IRibbonStatefulCommand CreatePosition(
        WriterRibbonFloatingObjectCommandPorts ports)
    {
        ArgumentNullException.ThrowIfNull(ports);
        return new WriterRibbonStatefulPortCommand(
            context =>
            {
                ports.PrepareExecution?.Invoke();
                if (!ports.HasSelection())
                    return;

                if (WriterRibbonNumericValueParser.TryParseObjectPosition(
                        context.SelectedValue,
                        CultureInfo.InvariantCulture,
                        out var position))
                {
                    ports.ApplyPosition(position);
                }
                else
                {
                    ports.OpenPositionDialog?.Invoke();
                }
            },
            () => new RibbonCommandState(IsEnabled: ports.HasSelection()));
    }

    public static IRibbonStatefulCommand CreatePositionPreset(
        WriterRibbonFloatingObjectCommandPorts ports,
        WriterRibbonObjectPositionInput position)
    {
        ArgumentNullException.ThrowIfNull(ports);
        return new WriterRibbonStatefulPortCommand(
            _ =>
            {
                ports.PrepareExecution?.Invoke();
                if (ports.HasSelection())
                    ports.ApplyPosition(position);
            },
            () => new RibbonCommandState(IsEnabled: ports.HasSelection()));
    }

    public static IRibbonStatefulCommand CreateSize(
        WriterRibbonFloatingObjectCommandPorts ports)
    {
        ArgumentNullException.ThrowIfNull(ports);
        return new WriterRibbonStatefulPortCommand(
            context =>
            {
                ports.PrepareExecution?.Invoke();
                if (!ports.HasSelection())
                    return;

                if (WriterRibbonNumericValueParser.TryParseObjectSize(
                        context.SelectedValue,
                        CultureInfo.InvariantCulture,
                        out var size))
                {
                    ports.ApplySize(size.WidthPt, size.HeightPt);
                }
                else if (string.IsNullOrWhiteSpace(context.SelectedValue))
                {
                    ports.OpenSizeDialog?.Invoke();
                }
            },
            () => new RibbonCommandState(IsEnabled: ports.HasSelection()));
    }

    public static IRibbonStatefulCommand CreateSizePreset(
        WriterRibbonFloatingObjectCommandPorts ports,
        double widthPt,
        double heightPt)
    {
        ArgumentNullException.ThrowIfNull(ports);
        return new WriterRibbonStatefulPortCommand(
            _ =>
            {
                ports.PrepareExecution?.Invoke();
                if (ports.HasSelection())
                    ports.ApplySize(widthPt, heightPt);
            },
            () => new RibbonCommandState(IsEnabled: ports.HasSelection()));
    }
}
