namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Events;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly, UltimateEvent]
internal sealed class UltimateActiveUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal UltimateActiveUpdateTickedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var ultimate = Game1.player.get_Ultimate();
        Game1.player.startGlowing(ultimate!.GlowColor, false, 0.05f);

        if ((Game1.game1.IsActiveNoOverlay || !Game1.options.pauseWhenOutOfFocus) && Game1.shouldTimePass())
            ultimate!.Countdown(Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds);
    }
}