using Writer.Shared.Ribbon;
using Writer.Core.Model;

namespace Writer.App.Presentation.Ribbon;

public sealed record HeaderFooterRibbonBindings(
    IRibbonCommand Header,
    IRibbonCommand Footer,
    IRibbonCommand PageNumber,
    IRibbonCommand PageNumberTop,
    IRibbonCommand PageNumberBottom,
    IRibbonCommand PageNumberCurrent,
    IRibbonCommand PageNumberFormat,
    IRibbonCommand DateTime,
    Func<HeaderFooterSlotKind, IRibbonCommand> CreateEditSlotCommand,
    IRibbonStatefulCommand DifferentFirstPage,
    IRibbonStatefulCommand DifferentOddEvenPages,
    IRibbonStatefulCommand HeaderFromTop,
    IRibbonStatefulCommand FooterFromBottom,
    Func<HeaderFooterSlotKind, IRibbonCommand> CreateNavigationCommand,
    IRibbonCommand Close,
    IRibbonCommand InsertHeaderPageNumber,
    IRibbonCommand InsertFooterPageNumber,
    IRibbonCommand InsertDateTime,
    IRibbonCommand InsertDocumentInfo);

public sealed record HeaderFooterRibbonCommand(
    RibbonCommandId Id,
    IRibbonStatefulCommand Command);

public sealed record HeaderFooterRibbonCommands(
    IReadOnlyList<HeaderFooterRibbonCommand> StatefulCommands);

public sealed record HeaderFooterPageSettingsPorts(
    Func<PageSettings> GetPageSettings,
    Action<Action<PageSettings>> ApplyPageSettings,
    Func<bool> IsEnabled,
    Func<RibbonCommandContext, string?>? ResolveSelectedValue = null);

public sealed record HeaderFooterPageSettingCommands(
    IRibbonStatefulCommand DifferentFirstPage,
    IRibbonStatefulCommand DifferentOddEvenPages,
    IRibbonStatefulCommand HeaderFromTop,
    IRibbonStatefulCommand FooterFromBottom);

/// <summary>
/// Owns Insert and Header &amp; Footer Design command identity over renderer-provided editor, pane,
/// prompt, and dialog adapters. Slot-to-action mapping remains canonical across both renderers.
/// </summary>
public static class HeaderFooterRibbonWorkflow
{
    public static IReadOnlyList<WriterRibbonCommandAction> Actions { get; } =
    [
        WriterRibbonCommandAction.Header,
        WriterRibbonCommandAction.Footer,
        WriterRibbonCommandAction.PageNumber,
        WriterRibbonCommandAction.PageNumberTop,
        WriterRibbonCommandAction.PageNumberBottom,
        WriterRibbonCommandAction.PageNumberCurrent,
        WriterRibbonCommandAction.PageNumberFormat,
        WriterRibbonCommandAction.Datetime,
        .. WriterRibbonSemanticCatalog.HeaderFooterEditSlots.Select(binding => binding.Action),
        WriterRibbonCommandAction.HfDifferentFirstPage,
        WriterRibbonCommandAction.HfDifferentOddEven,
        WriterRibbonCommandAction.HfHeaderFromTop,
        WriterRibbonCommandAction.HfFooterFromBottom,
        .. WriterRibbonSemanticCatalog.HeaderFooterNavigationSlots.Select(binding => binding.Action),
        WriterRibbonCommandAction.HfClose,
        WriterRibbonCommandAction.HfInsertPageNumber,
        WriterRibbonCommandAction.HfInsertPageNumberFooter,
        WriterRibbonCommandAction.HfInsertDatetime,
        WriterRibbonCommandAction.HfInsertField,
    ];

    public static HeaderFooterRibbonCommands Register(
        WriterRibbonEditorCommandFamilyBuilder builder,
        HeaderFooterRibbonBindings bindings)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(bindings.CreateEditSlotCommand);
        ArgumentNullException.ThrowIfNull(bindings.CreateNavigationCommand);

        Bind(WriterRibbonCommandAction.Header, bindings.Header);
        Bind(WriterRibbonCommandAction.Footer, bindings.Footer);
        Bind(WriterRibbonCommandAction.PageNumber, bindings.PageNumber);
        Bind(WriterRibbonCommandAction.PageNumberTop, bindings.PageNumberTop);
        Bind(WriterRibbonCommandAction.PageNumberBottom, bindings.PageNumberBottom);
        Bind(WriterRibbonCommandAction.PageNumberCurrent, bindings.PageNumberCurrent);
        Bind(WriterRibbonCommandAction.PageNumberFormat, bindings.PageNumberFormat);
        Bind(WriterRibbonCommandAction.Datetime, bindings.DateTime);

        foreach (var binding in WriterRibbonSemanticCatalog.HeaderFooterEditSlots)
            Bind(binding.Action, bindings.CreateEditSlotCommand(binding.Slot));

        var stateful = new List<HeaderFooterRibbonCommand>(4);
        BindStateful(WriterRibbonCommandAction.HfDifferentFirstPage, bindings.DifferentFirstPage);
        BindStateful(WriterRibbonCommandAction.HfDifferentOddEven, bindings.DifferentOddEvenPages);
        BindStateful(WriterRibbonCommandAction.HfHeaderFromTop, bindings.HeaderFromTop);
        BindStateful(WriterRibbonCommandAction.HfFooterFromBottom, bindings.FooterFromBottom);

        foreach (var binding in WriterRibbonSemanticCatalog.HeaderFooterNavigationSlots)
            Bind(binding.Action, bindings.CreateNavigationCommand(binding.Slot));

        Bind(WriterRibbonCommandAction.HfClose, bindings.Close);
        Bind(WriterRibbonCommandAction.HfInsertPageNumber, bindings.InsertHeaderPageNumber);
        Bind(WriterRibbonCommandAction.HfInsertPageNumberFooter, bindings.InsertFooterPageNumber);
        Bind(WriterRibbonCommandAction.HfInsertDatetime, bindings.InsertDateTime);
        Bind(WriterRibbonCommandAction.HfInsertField, bindings.InsertDocumentInfo);

        return new HeaderFooterRibbonCommands(stateful);

        void Bind(WriterRibbonCommandAction action, IRibbonCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);
            builder.Bind(action, command);
        }

        void BindStateful(WriterRibbonCommandAction action, IRibbonStatefulCommand command)
        {
            Bind(action, command);
            stateful.Add(new HeaderFooterRibbonCommand(
                WriterRibbonCommandWorkflow.GetPrimaryCommandId(action),
                command));
        }
    }

    public static HeaderFooterPageSettingCommands CreatePageSettingCommands(
        HeaderFooterPageSettingsPorts ports)
    {
        ArgumentNullException.ThrowIfNull(ports);
        ArgumentNullException.ThrowIfNull(ports.GetPageSettings);
        ArgumentNullException.ThrowIfNull(ports.ApplyPageSettings);
        ArgumentNullException.ThrowIfNull(ports.IsEnabled);

        return new HeaderFooterPageSettingCommands(
            DifferentFirstPage: Toggle(
                page => page.DifferentFirstPage = !page.DifferentFirstPage,
                page => page.DifferentFirstPage),
            DifferentOddEvenPages: Toggle(
                page => page.DifferentOddEvenPages = !page.DifferentOddEvenPages,
                page => page.DifferentOddEvenPages),
            HeaderFromTop: Distance(
                page => page.HeaderDistancePt,
                (page, points) => page.HeaderDistancePt = points),
            FooterFromBottom: Distance(
                page => page.FooterDistancePt,
                (page, points) => page.FooterDistancePt = points));

        IRibbonStatefulCommand Toggle(
            Action<PageSettings> apply,
            Func<PageSettings, bool> isChecked) =>
            new PageSettingToggleCommand(ports, apply, isChecked);

        IRibbonStatefulCommand Distance(
            Func<PageSettings, double> getDistance,
            Action<PageSettings, double> setDistance) =>
            new PageSettingDistanceCommand(ports, getDistance, setDistance);
    }

    private sealed class PageSettingToggleCommand(
        HeaderFooterPageSettingsPorts ports,
        Action<PageSettings> apply,
        Func<PageSettings, bool> isChecked) : IRibbonStatefulCommand
    {
        public void Execute(RibbonCommandContext context)
        {
            if (ports.IsEnabled())
                ports.ApplyPageSettings(apply);
        }

        public RibbonCommandState GetState() => new(
            IsEnabled: ports.IsEnabled(),
            IsChecked: isChecked(ports.GetPageSettings()));
    }

    private sealed class PageSettingDistanceCommand(
        HeaderFooterPageSettingsPorts ports,
        Func<PageSettings, double> getDistance,
        Action<PageSettings, double> setDistance) : IRibbonStatefulCommand
    {
        public void Execute(RibbonCommandContext context)
        {
            var selectedValue = ports.ResolveSelectedValue?.Invoke(context) ?? context.SelectedValue;
            if (!ports.IsEnabled()
                || !HeaderFooterDialogPlanner.TryParseDistance(selectedValue, out var points))
            {
                return;
            }

            ports.ApplyPageSettings(page => setDistance(page, points));
        }

        public RibbonCommandState GetState() => new(
            IsEnabled: ports.IsEnabled(),
            Value: HeaderFooterDialogPlanner.FormatDistance(getDistance(ports.GetPageSettings())));
    }
}
