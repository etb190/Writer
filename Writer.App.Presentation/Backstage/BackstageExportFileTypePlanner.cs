using Writer.Shared.Shell;
using Writer.Core.IO;

namespace Writer.App.Presentation.Backstage;

public static class BackstageExportFileTypePlanner
{
    public static BackstageActionGroup BuildChangeFileTypeGroup(
        IEnumerable<FileFormatDescriptor> formats,
        Action<string> saveAsExtension) =>
        BuildChangeFileTypeGroup(formats, (extension, _) => saveAsExtension(extension));

    public static BackstageActionGroup BuildChangeFileTypeGroup(
        IEnumerable<FileFormatDescriptor> formats,
        Action<string, int> saveAsFormat)
    {
        ArgumentNullException.ThrowIfNull(formats);
        ArgumentNullException.ThrowIfNull(saveAsFormat);

        return BackstageFileTypeActionPlanner.BuildGroup(
            "Change File Type",
            BackstageSaveAsFileTypePlanner.BuildRows(formats),
            saveAsFormat);
    }
}
