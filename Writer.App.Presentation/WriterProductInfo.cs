using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Writer.Shared.AppServices;

namespace Writer.App.Presentation;

/// <summary>Host-neutral Writer product, version, legal, and diagnostics text.</summary>
public static class WriterProductInfo
{
    public const string ProductName = "Writer";
    public const string HelpUrl = "https://github.com/etb190/Writer";
    public const string FeedbackUrl = "https://github.com/etb190/Writer/issues/new?title=Writer%20feedback";
    public const string LatestReleaseUrl = "https://github.com/etb190/Writer/actions";
    public const string DesktopRendererDescription =
        "Built with .NET 10. Writer provides WPF and Avalonia desktop renderers.";
    public const string TrademarkNotice = FamilyLegalNotices.WriterTrademarkNotice;
    public const string ProjectLicenseNotice = "Writer Source License: Copyright (c) 2026 FreeX contributors. All rights reserved. Tester binaries may be downloaded and run for personal evaluation and testing. Redistribution or commercial distribution requires separate written permission from the copyright holder.";
    public const string PrivacyNotice = "Privacy: Writer is a local desktop app. Documents are opened, edited, and saved on this machine unless the user explicitly chooses an external sharing path. Local tester diagnostics stay on the user's machine unless the user chooses to share them. Writer does not intentionally collect document contents, filenames, or file paths in diagnostics or crash reports.";
    public const string SourceNotice = "Full project license, legal notice, privacy notice, third-party notices, and bundled third-party license texts are available in Help > Legal Notices and are maintained in the Writer repository at https://github.com/etb190/Writer.";

    public static string GetVersionText(Assembly assembly)
    {
        var version = AssemblyVersionMetadata.FromAssembly(assembly);
        return AppVersionFormatter.FormatVersionText(
            version.InformationalVersion);
    }

    public static string GetBuildVersionText(Assembly assembly)
    {
        var version = AssemblyVersionMetadata.FromAssembly(assembly);
        return AppVersionFormatter.FormatBuildVersionText(
            version.InformationalVersion,
            version.AssemblyVersion);
    }

    public static string CreateFeedbackUrl(Assembly assembly) =>
        AppFeedbackReporter.CreateIssueUrl(
            ProductName,
            AppDiagnosticsMetadata.Create(GetBuildVersionText(assembly)));

    public static string CreateAboutText(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        return $"""
               {ProductName}
               {GetVersionText(assembly)}

               A free word processor for DOCX editing and format-fidelity work.

               {DesktopRendererDescription}

               {TrademarkNotice}

               {ProjectLicenseNotice}

               {PrivacyNotice}

               {SourceNotice}
               """;
    }

    public static string CreateDiagnosticsText(
        Assembly assembly,
        string diagnosticsDirectory,
        string optionsPath)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var builder = new StringBuilder();
        builder.AppendLine("Writer Diagnostics");
        builder.AppendLine($"Version: {GetBuildVersionText(assembly)}");
        builder.AppendLine($"Runtime: {RuntimeInformation.FrameworkDescription}");
        builder.AppendLine($"OS: {RuntimeInformation.OSDescription}");
        builder.AppendLine($"Process architecture: {RuntimeInformation.ProcessArchitecture}");
        builder.AppendLine($"Diagnostics directory: {diagnosticsDirectory}");
        builder.AppendLine($"Options path: {optionsPath}");
        builder.AppendLine();
        builder.AppendLine("Review this text before sharing it. Writer does not intentionally include document contents, filenames, or file paths in this diagnostics summary.");
        return builder.ToString();
    }
}
