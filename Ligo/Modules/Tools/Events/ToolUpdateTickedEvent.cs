namespace DaLion.Ligo.Modules.Tools.Events;

#region using directives

using DaLion.Ligo.Modules.Tools.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ToolUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ToolUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.Get_HasShockwave();

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (ModEntry.Config.Tools.TicksBetweenWaves <= 1 || e.IsMultipleOf(ModEntry.Config.Tools.TicksBetweenWaves))
        {
            Game1.player.Get_Shockwaves().ForEach(wave => wave.Update(Game1.currentGameTime.TotalGameTime.TotalMilliseconds));
        }
    }
}
