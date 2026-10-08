namespace Writer.Shared.IO;

public sealed record FileDialogFormatDescriptor(
    string Extension,
    string FormatName,
    bool CanOpen = true,
    bool CanSave = true);
