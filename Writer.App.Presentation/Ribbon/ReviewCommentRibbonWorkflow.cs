using Writer.Shared.Ribbon;

namespace Writer.App.Presentation.Ribbon;

public sealed record ReviewCommentRibbonCommands(
    IRibbonCommand NewComment,
    IRibbonCommand DeleteComment,
    IRibbonCommand PreviousComment,
    IRibbonCommand NextComment,
    IRibbonCommand ReplyComment,
    IRibbonCommand ResolveComment,
    IRibbonCommand ShowComments);

/// <summary>
/// Owns the canonical Review &gt; Comments command routing for both renderers. Native prompt,
/// focus, pane, and feedback behavior remains behind the supplied renderer commands.
/// </summary>
public static class ReviewCommentRibbonWorkflow
{
    public static void Register(
        IRibbonCommandRegistry registry,
        ReviewCommentRibbonCommands commands)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(commands);

        registry.Bind(WriterRibbonCommandAction.NewComment, commands.NewComment);
        registry.Bind(WriterRibbonCommandAction.DeleteComment, commands.DeleteComment);
        registry.Bind(WriterRibbonCommandAction.PreviousComment, commands.PreviousComment);
        registry.Bind(WriterRibbonCommandAction.NextComment, commands.NextComment);
        registry.Bind(WriterRibbonCommandAction.ReplyComment, commands.ReplyComment);
        registry.Bind(WriterRibbonCommandAction.ResolveComment, commands.ResolveComment);
        registry.Bind(WriterRibbonCommandAction.ShowComments, commands.ShowComments);
    }
}
