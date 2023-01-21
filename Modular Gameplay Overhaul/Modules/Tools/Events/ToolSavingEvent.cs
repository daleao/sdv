namespace DaLion.Overhaul.Modules.Tools.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolSavingEvent : SavingEvent
{
    /// <summary>Initializes a new instance of the <see cref="ToolSavingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ToolSavingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ToolsModule.Config.EnableAutoSelection;

    /// <inheritdoc />
    protected override void OnSavingImpl(object? sender, SavingEventArgs e)
    {
        Game1.player.Write(
            DataFields.SelectableTools,
            string.Join(
                ',',
                ToolsModule.State.SelectableToolByType.Values.WhereNotNull().Select(selectable => selectable.Index)));
    }
}
