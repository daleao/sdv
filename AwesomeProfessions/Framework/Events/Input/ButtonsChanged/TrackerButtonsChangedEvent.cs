namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using StardewModdingAPI;
using StardewModdingAPI.Events;

using Display;
using GameLoop;

#endregion using directives

internal class TrackerButtonsChangedEvent : ButtonsChangedEvent
{
    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object sender, ButtonsChangedEventArgs e)
    {
        if (ModEntry.Config.Modkey.JustPressed())
        {
            EventManager.Enable(typeof(IndicatorUpdateTickedEvent), typeof(TrackerRenderedHudEvent));
        }
        else if (ModEntry.Config.Modkey.GetState() == SButtonState.Released)
        {
            EventManager.Disable(typeof(TrackerRenderedHudEvent));
            if (!ModEntry.State.Value.ProspectorHunt.IsActive && !ModEntry.State.Value.ScavengerHunt.IsActive)
                EventManager.Disable(typeof(IndicatorUpdateTickedEvent));
        }
    }
}