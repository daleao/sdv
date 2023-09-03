namespace DaLion.Overhaul.Modules.Tools.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ShockwaveUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ShockwaveUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ShockwaveUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ToolsModule.State.Shockwaves.Count > 0;

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (ToolsModule.Config.TicksBetweenCrests > 1 && !e.IsMultipleOf(ToolsModule.Config.TicksBetweenCrests))
        {
            return;
        }

        var shockwaves = ToolsModule.State.Shockwaves.ToList();
        shockwaves.ForEach(wave => wave.Update(Game1.currentGameTime.TotalGameTime.TotalMilliseconds));
    }
}
