using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;
using TheLion.Stardew.Professions.Framework.Events.Display.RenderingHud;
using TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events.Player.Warped;

internal class SuperModeWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    public override void OnWarped(object sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer || e.NewLocation.GetType() == e.OldLocation.GetType()) return;

        if (e.NewLocation.IsCombatZone())
        {
            ModEntry.Subscriber.Subscribe(new SuperModeBarRenderingHudEvent());
        }
        else
        {
            ModEntry.Subscriber.Unsubscribe(typeof(SuperModeBarFadeOutUpdateTickedEvent),
                typeof(SuperModeBarShakeTimerUpdateTickedEvent), typeof(SuperModeBarRenderingHudEvent));

            ModEntry.State.Value.SuperModeGaugeValue = 0;
            ModEntry.State.Value.SuperModeGaugeAlpha = 1f;
            ModEntry.State.Value.ShouldShakeSuperModeGauge = false;

            var buffID = ModEntry.Manifest.UniqueID.GetHashCode() + ModEntry.State.Value.SuperModeIndex + 4;
            var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffID);
            if (buff is null) return;

            Game1.buffsDisplay.otherBuffs.Remove(buff);
            Game1.player.stopGlowing();
        }
    }
}