namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Events;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using Ultimates;

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
        if (!Game1.player.isGlowing)
        {
            var glowColor = ModEntry.Player.RegisteredUltimate!.GlowColor;
            if (glowColor != Color.White)
                Game1.player.startGlowing(ModEntry.Player.RegisteredUltimate.GlowColor, false, 0.05f);
        }

        if ((Game1.game1.IsActiveNoOverlay || !Game1.options.pauseWhenOutOfFocus) && Game1.shouldTimePass())
            ModEntry.Player.RegisteredUltimate!.Countdown(Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds);
    }
}