namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using Display;
using GameLoop;

#endregion using directives

[UsedImplicitly]
internal class TrackerButtonsChangedEvent : ButtonsChangedEvent
{
    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object sender, ButtonsChangedEventArgs e)
    {
        if (ModEntry.Config.ModKey.JustPressed())
        {
            EventManager.Enable(typeof(PointerUpdateTickedEvent), typeof(TrackerRenderedHudEvent));
        }
        else if (ModEntry.Config.ModKey.GetState() == SButtonState.Released)
        {
            EventManager.Disable(typeof(TrackerRenderedHudEvent));
            if (!ModEntry.PlayerState.ProspectorHunt.IsActive && !ModEntry.PlayerState.ScavengerHunt.IsActive)
                EventManager.Disable(typeof(PointerUpdateTickedEvent));
        }
    }
}