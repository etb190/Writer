# Writer

Writer is a free word processor for local document files. It opens, edits, and saves
Word-compatible DOCX documents with rich text formatting, paragraphs, lists, tables,
fields, and images — while keeping the project, branding, icons, and release artifacts
fully independent from Microsoft.

## Highlights

- **DOCX in, DOCX out** — works directly on standard Word-compatible `.docx` files; no
  proprietary lock-in and no forced conversion step.
- **Rich editing surface** — formatted text, paragraph styles, tables with layout control,
  page layout (margins, orientation, paper sizes), headers/footers, and print output.
- **Two desktop renderers** — a cross-platform Avalonia front end (Windows, Linux, macOS)
  and a WPF front end for Windows, sharing one core model and I/O layer.
- **Ribbon interface** — a familiar tab/ribbon command surface with backstage view,
  built on the shared ribbon component layer.
- **PDF export** — portable PDF output via the bundled PDF writer stack.
- **Local-first** — everything runs against local files. No account, no cloud, no telemetry
  requirement.
- **Localization-ready** — satellite resource infrastructure with per-product culture
  support (French `fr-FR` out of the box).

## Repository layout

```
Writer.slnx                  # solution: app + shared layer
Writer.App.Avalonia/         # cross-platform Avalonia desktop app (Windows/Linux/macOS)
Writer.App.Host/             # WPF desktop host for Windows
Writer.App.Presentation/     # presentation/view-model layer shared by both front ends
Writer.App.Localization/     # app satellite resources
Writer.Ribbon.Definitions/   # ribbon tabs, commands, and icon mappings
Writer.Core.Model/           # document model
Writer.Core.IO/              # DOCX reading/writing (OpenXML/Opc-based)
shared/Writer.Shared.*/      # shared component layer (shell, ribbon, theme, PDF, IO, ...)
```

## Building

Requires the .NET SDK (see `global.json` for the pinned version, currently **10.0.4xx**).

```bash
dotnet build Writer.slnx
```

- The Avalonia front end builds and runs cross-platform.
- The WPF host (`Writer.App.Host`) builds on Windows.

## Portable builds

GitHub Actions builds **one self-contained executable** per platform —
`win-x64`, `linux-x64`, `linux-arm64`, `osx-x64`, `osx-arm64`. The .NET runtime
and native libraries are embedded in the binary (compressed), so the Windows
artifact is a single `Writer-<version>-win-x64.exe` (~50 MB): no installer, no
runtime install, no loose DLLs. Native libraries unpack themselves to a temp
folder on first launch.

- **Manual runs** — start the *Portable build* workflow from the Actions tab;
  the executables appear under that run's **Artifacts**.
- **Tagged releases** — pushing a `writer-v*` tag (e.g. `writer-v0.9.0`) builds
  all five targets and attaches them to a GitHub Release.

Notes: macOS builds are unsigned, so the first launch may require right-click
→ **Open** (or clearing the quarantine attribute).

## Status

Writer is an independently developed, free desktop application. It is not affiliated
with, endorsed by, or derived from Microsoft Word; DOCX compatibility is achieved
through the documented OpenXML file format.
