using System;
using System.Windows;
using Writer.Shared.AppServices;
using Writer.Shared.Shell.Wpf;
using Writer.App.Host.Editing;
using Writer.App.Presentation.Shell;

namespace Writer.App.Host;

/// <summary>
/// WPF scheduling and prompt adapter for the renderer-neutral <see cref="WriterAutosaveSession"/>.
/// </summary>
internal sealed partial class AutosaveCoordinator
{
    private readonly FileCommands _file;
    private readonly WpfAutosaveTimer _timer;
    private readonly WriterAutosaveSession _session;
    private readonly Func<AutosaveRecoveryCandidate, bool>? _recoverInNewWindow;

    public AutosaveCoordinator(
        DocumentView editor,
        FileCommands file,
        Func<WriterAutosavePorts, WriterAutosaveSession>? sessionFactory = null,
        Func<AutosaveRecoveryCandidate, bool>? recoverInNewWindow = null)
    {
        _file = file;
        var ports = new WriterAutosavePorts(
            GetOriginalFilePath: () => file.CurrentPath,
            GetDisplayName: () => file.DisplayName,
            GetIsDirty: () => file.IsDirty,
            GetDirtyGeneration: () => file.DirtyGeneration,
            ExecuteWithDocument: writeDocument =>
            {
                editor.CommitToModel();
                // Same guard as FileCommands' GetDocument port: while Mailings > Preview Results is active,
                // editor.Model is the merged, single-record preview document, not the template. Autosave
                // snapshots (and the crash-recovery snapshot that reuses this same port via
                // AutosaveCoordinator.TryEmergencySnapshot) must persist the real template so an
                // untimely autosave/crash doesn't leave the recovered document with its merge fields baked
                // away.
                writeDocument(editor.MailMergeSession?.Template ?? editor.Model);
            });
        _session = sessionFactory?.Invoke(ports) ?? new WriterAutosaveSession(ports);
        _recoverInNewWindow = recoverInNewWindow;
        _timer = new WpfAutosaveTimer(WriterAutosaveSession.DefaultInterval, _session.Snapshot);
    }

    public void Start() => _timer.Start();

    /// <summary>
    /// Best-effort emergency snapshot for the crash handler (see Program.cs's
    /// TryEmergencySnapshotAllWindows). Must never throw -- delegates to
    /// <see cref="WriterAutosaveSession.TryEmergencySnapshot"/>, which is never-throw by design.
    /// </summary>
    public void TryEmergencySnapshot() => _session.TryEmergencySnapshot();

    public void Stop()
    {
        _timer.Stop();
        _session.CompleteCleanExit();
    }

    /// <summary>
    /// Offers every prior-session snapshot on startup. The first accepted document uses this window;
    /// subsequent documents open through the new-window callback.
    /// </summary>
    /// <remarks>
    /// startup-fileopen F2 (WPF host): mirrors the fix already applied to FreeP's Avalonia
    /// <c>AutosaveAdapter.OfferRecoveryAsync</c>. A command-line/file-association document may
    /// already be loaded into this window before this offer runs, not yet dirty, so routing the
    /// first accepted candidate into "the current window" unconditionally would silently replace it.
    /// We snapshot whether the window already holds an explicitly opened document (<see
    /// cref="_file"/>.<c>CurrentPath</c> non-null) BEFORE any candidate is applied, and if so force
    /// every accepted candidate through the new-window path instead, same as every candidate beyond
    /// the first. A genuinely fresh window (no startup file) keeps the prior unconditional behaviour.
    /// </remarks>
    public bool OfferRecovery(Window owner)
    {
        var text = AutosaveRecoveryTextCatalog.Resolve(UiText.Get);
        return WpfAutosaveRecoveryHost.OfferStartup(
            owner,
            new WpfAutosaveRecoveryMessages(
                text.Title,
                text.NoDocumentsMessage,
                text.FailureMessageFormat),
            () => _file.CurrentPath is not null,
            _session.PlanRecoveries,
            (recovery, remainingCount) =>
                new WriterRecoveryOffer(recovery, remainingCount, WriterRecoveryPromptMode.Startup).Prompt,
            (recovery, useCurrentWindow) => _session.CompleteRecovery(
                recovery,
                accepted: true,
                useCurrentWindow
                    ? _file.OpenSnapshot
                    : (_, _) => _recoverInNewWindow?.Invoke(recovery.Candidate) ?? false,
                WriterRecoveryRestoreExceptionPolicy.QuarantineCandidate));
    }

    public bool RecoverUnsavedDocuments(Window owner)
    {
        var text = AutosaveRecoveryTextCatalog.Resolve(UiText.Get);
        return WpfAutosaveRecoveryHost.RecoverManually(
            owner,
            new WpfAutosaveRecoveryMessages(
                text.Title,
                text.NoDocumentsMessage,
                text.FailureMessageFormat),
            _session.PlanRecoveries,
            (recovery, remainingCount) =>
                new WriterRecoveryOffer(recovery, remainingCount, WriterRecoveryPromptMode.Manual).Prompt,
            (recovery, useCurrentWindow) =>
            {
                // The manual command may target a window with unrelated unsaved edits. Run the
                // product's synchronous dirty gate before accepting the recovery candidate.
                if (useCurrentWindow && !_file.ConfirmCloseAllowed("recovering an unsaved document"))
                {
                    return _session.CompleteRecovery(
                        recovery,
                        accepted: false,
                        _file.OpenSnapshot);
                }

                return _session.CompleteRecovery(
                    recovery,
                    accepted: true,
                    useCurrentWindow
                        ? _file.OpenSnapshot
                        : (_, _) => _recoverInNewWindow?.Invoke(recovery.Candidate) ?? false);
            });
    }
}
