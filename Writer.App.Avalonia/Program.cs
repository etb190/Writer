using Writer.Shared.Shell.Avalonia;

namespace Writer.App.Avalonia;

/// <summary>
/// Writer (the Word-like sibling of FreeX) cross-platform entry point. Installs Writer's product
/// identity into the shared tier (so storage/diagnostics land under the Writer folder, not FreeX),
/// then runs the Avalonia shell. Validation commands live in the external Writer validation host;
/// the WPF Writer.App.Host stays Windows-only.
/// </summary>
internal static partial class Program
{
    [STAThread]
    public static int Main(string[] args) =>
        SisterAvaloniaStandardDesktopFactory.Run(args, App.DesktopProfile);
}
