using StardewModdingAPI;
using StardewModdingAPI.Events;
using DaLion.Stardew.Professions.Framework.Events.Display;
using DaLion.Stardew.Professions.Framework.Events.GameLoop;

namespace DaLion.Stardew.Professions.Framework.Events.Input;

internal class TrackerButtonsChangedEvent : ButtonsChangedEvent
{
    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object sender, ButtonsChangedEventArgs e)
    {
        if (ModEntry.Config.Modkey.JustPressed())
        {
            ModEntry.EventManager.Enable(typeof(IndicatorUpdateTickedEvent), typeof(TrackerRenderedHudEvent));
        }
        else if (ModEntry.Config.Modkey.GetState() == SButtonState.Released)
        {
            ModEntry.EventManager.Disable(typeof(TrackerRenderedHudEvent));
            if (!ModEntry.State.Value.ProspectorHunt.IsActive && !ModEntry.State.Value.ScavengerHunt.IsActive)
                ModEntry.EventManager.Disable(typeof(IndicatorUpdateTickedEvent));
        }
    }
}