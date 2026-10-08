using Avalonia.Controls;
using Writer.Shared.AppServices.Printing;
using Writer.Shared.Shell.Avalonia;

namespace Writer.App.Avalonia.Printing;

internal sealed partial class CupsPrintDialog : WriterDialogWindow
{
    private static readonly AvaloniaPrintDialogOptions Options = new()
    {
        Width = 480,
        ChoiceMinWidth = 220,
        Collation = AvaloniaPrintDialogCollation.Fixed(true),
        ApplyCompactActionButtonChrome = true,
    };

    private CupsPrintDialog()
    {
    }

    public static Task<PrintSelection?> ShowAsync(
        Window owner,
        PrinterDiscoveryResult discovery,
        PrintSelection? requested = null,
        CancellationToken cancellationToken = default) =>
        AvaloniaPrintDialogWorkflow.ShowAsync(
            owner,
            discovery,
            static () => new CupsPrintDialog(),
            Options,
            requested,
            cancellationToken);
}
