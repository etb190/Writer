using Writer.Shared.Ribbon;
using Writer.App.Presentation.Dialogs;
using Writer.Core.Model;

namespace Writer.App.Presentation.Ribbon;

public sealed record FontEffectRibbonPorts(
    IRibbonCommand Bold,
    IRibbonCommand Italic,
    IRibbonCommand Underline,
    IRibbonCommand Strikethrough,
    IRibbonCommand SmallCaps,
    IRibbonCommand AllCaps,
    IRibbonCommand Superscript,
    IRibbonCommand Subscript,
    IRibbonCommand GrowFont,
    IRibbonCommand ShrinkFont);

public enum FontEffectRibbonKind
{
    Bold,
    Italic,
    Underline,
    Strikethrough,
    SmallCaps,
    AllCaps,
    Superscript,
    Subscript,
}

/// <summary>Shared checked-state policy for the Home &gt; Font toggle controls.</summary>
public static class FontEffectRibbonStatePlanner
{
    public static RibbonCommandState GetState(
        FontEffectRibbonKind kind,
        FontDialogSelectionState selection,
        bool isEnabled = true)
    {
        ArgumentNullException.ThrowIfNull(selection);
        bool indeterminate = kind switch
        {
            FontEffectRibbonKind.Bold => selection.BoldIndeterminate,
            FontEffectRibbonKind.Italic => selection.ItalicIndeterminate,
            FontEffectRibbonKind.Underline => selection.UnderlineIndeterminate,
            FontEffectRibbonKind.Strikethrough => selection.StrikethroughIndeterminate,
            FontEffectRibbonKind.SmallCaps => selection.SmallCapsIndeterminate,
            FontEffectRibbonKind.AllCaps => selection.AllCapsIndeterminate,
            FontEffectRibbonKind.Superscript => selection.SuperscriptIndeterminate,
            FontEffectRibbonKind.Subscript => selection.SubscriptIndeterminate,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };
        return new RibbonCommandState(
            IsEnabled: isEnabled,
            IsChecked: !indeterminate && IsSet(kind, selection.Run));
    }

    public static IRibbonStatefulCommand CreateCommand(
        FontEffectRibbonKind kind,
        Action execute,
        Func<FontDialogSelectionState> getSelection,
        Func<bool>? isEnabled = null,
        Action? prepareExecution = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        ArgumentNullException.ThrowIfNull(getSelection);
        return new WriterRibbonStatefulPortCommand(
            _ => execute(),
            () => GetState(kind, getSelection(), isEnabled?.Invoke() ?? true),
            prepareExecution);
    }

    private static bool IsSet(FontEffectRibbonKind kind, RunFormatting formatting) => kind switch
    {
        FontEffectRibbonKind.Bold => formatting.Bold,
        FontEffectRibbonKind.Italic => formatting.Italic,
        FontEffectRibbonKind.Underline => formatting.Underline,
        FontEffectRibbonKind.Strikethrough => formatting.Strikethrough,
        FontEffectRibbonKind.SmallCaps => formatting.SmallCaps,
        FontEffectRibbonKind.AllCaps => formatting.AllCaps,
        FontEffectRibbonKind.Superscript => formatting.VerticalAlign == VerticalAlign.Superscript,
        FontEffectRibbonKind.Subscript => formatting.VerticalAlign == VerticalAlign.Subscript,
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
    };
}

/// <summary>
/// Owns Home &gt; Font effect command mapping for both renderers. Native routed/stateful commands
/// remain renderer adapters, while semantic ownership and route completeness stay in Presentation.
/// </summary>
public static class FontEffectRibbonWorkflow
{
    public static IReadOnlyList<WriterRibbonCommandAction> Actions { get; } =
    [
        WriterRibbonCommandAction.Bold,
        WriterRibbonCommandAction.Italic,
        WriterRibbonCommandAction.Underline,
        WriterRibbonCommandAction.Strikethrough,
        WriterRibbonCommandAction.Smallcaps,
        WriterRibbonCommandAction.Allcaps,
        WriterRibbonCommandAction.Superscript,
        WriterRibbonCommandAction.Subscript,
        WriterRibbonCommandAction.GrowFont,
        WriterRibbonCommandAction.ShrinkFont,
    ];

    public static void Register(
        IRibbonCommandRegistry bindings,
        FontEffectRibbonPorts ports)
    {
        ArgumentNullException.ThrowIfNull(bindings);
        ArgumentNullException.ThrowIfNull(ports);

        bindings.Bind(WriterRibbonCommandAction.Bold, ports.Bold);
        bindings.Bind(WriterRibbonCommandAction.Italic, ports.Italic);
        bindings.Bind(WriterRibbonCommandAction.Underline, ports.Underline);
        bindings.Bind(WriterRibbonCommandAction.Strikethrough, ports.Strikethrough);
        bindings.Bind(WriterRibbonCommandAction.Smallcaps, ports.SmallCaps);
        bindings.Bind(WriterRibbonCommandAction.Allcaps, ports.AllCaps);
        bindings.Bind(WriterRibbonCommandAction.Superscript, ports.Superscript);
        bindings.Bind(WriterRibbonCommandAction.Subscript, ports.Subscript);
        bindings.Bind(WriterRibbonCommandAction.GrowFont, ports.GrowFont);
        bindings.Bind(WriterRibbonCommandAction.ShrinkFont, ports.ShrinkFont);
    }
}
