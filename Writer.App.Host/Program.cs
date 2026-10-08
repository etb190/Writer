using Writer.Shared.Theme;
using Writer.Shared.Theme.Wpf;
using Writer.App.Presentation.Options;
using Writer.App.Presentation.Shell;

namespace Writer.App.Host;

/// <summary>
/// Writer entry point. Installs Writer identity/seams, then delegates the common WPF options,
/// diagnostics, theme/language startup, and application-run lifecycle to the shared startup runner.
/// </summary>
public static class Program
{
    /// <summary>
    /// The active brand theme selected at startup (default: <see cref="BrandThemes.Writer"/>).
    /// Stored so tests and future windows can read the active palette.
    /// </summary>
    internal static Theme ActiveTheme { get; private set; } = WriterApplicationStartup.Theme.DefaultTheme;

    [STAThread]
    public static void Main(string[] args)
    {
        // Velopack must service install/update/uninstall hooks before WPF creates an Application.
        // Keep Run() at the real entry point so Velopack recognizes the lifecycle invocation.
        VelopackBootstrap.Configure().Run();

        WpfApplicationStartupRunner.Run(new WpfApplicationStartupSpec<WriterOptions>(
            WriterApplicationStartup.ProductIdentity,
            (options, optionsStore, startupFilePaths) =>
                new MainWindow(options, optionsStore, startupFilePaths: startupFilePaths))
        {
            InstallSharedSeams = AppLocalization.Bootstrap.InstallSharedSeams,
            Theme = new WpfApplicationThemeStartupSpec<Theme>(
                Plan: WriterApplicationStartup.Theme,
                ApplyTheme: WpfThemeApplier.Apply)
            {
                SetActiveTheme = theme => ActiveTheme = theme
            },
            Localization = new WpfApplicationLocalizationStartupSpec<WriterOptions>(
                SelectUiLanguage: options => options.UiLanguage,
                ApplyUiLanguage: AppLocalization.Bootstrap.ApplyAppLanguage,
                ApplyCurrentCultureToWpf: AppLocalization.Bootstrap.ApplyCurrentCultureToWpf),
            OnEmergencySnapshot = EmergencySnapshotCrashHandler.TryEmergencySnapshotAllWindows
        }, args);
    }
}
