namespace DaLion.Overhaul.Modules.Tools.Events;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Tools.VirtualProperties;
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
    public override bool IsEnabled => Game1.player.Get_HasShockwave();

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (ToolsModule.Config.TicksBetweenWaves > 1 && !e.IsMultipleOf(ToolsModule.Config.TicksBetweenWaves))
        {
            return;
        }

        var shockwaves = Game1.player.Get_Shockwaves().ToList();
        shockwaves.ForEach(wave => wave.Update(Game1.currentGameTime.TotalGameTime.TotalMilliseconds));
    }
}
