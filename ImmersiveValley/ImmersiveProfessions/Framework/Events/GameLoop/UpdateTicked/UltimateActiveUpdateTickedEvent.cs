using Microsoft.Xna.Framework;

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class UltimateActiveUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        if (!Game1.player.isGlowing)
        {
            var glowColor = ModEntry.PlayerState.RegisteredUltimate.GlowColor;
            if (glowColor != Color.White)
                Game1.player.startGlowing(ModEntry.PlayerState.RegisteredUltimate.GlowColor, false, 0.05f);
        }

        if (Game1.game1.IsActive && Game1.shouldTimePass())
            ModEntry.PlayerState.RegisteredUltimate.Countdown(Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds);
    }
}