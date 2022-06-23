namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using Common.Events;
using Display;
using GameLoop;

#endregion using directives

[UsedImplicitly]
internal sealed class TrackerButtonsChangedEvent : ButtonsChangedEvent
{
    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object sender, ButtonsChangedEventArgs e)
    {
        if (ModEntry.Config.ModKey.JustPressed())
        {
            ModEntry.EventManager.Hook<PointerUpdateTickedEvent>();
            ModEntry.EventManager.Hook<TrackerRenderedHudEvent>();
        }
        else if (ModEntry.Config.ModKey.GetState() == SButtonState.Released)
        {
            ModEntry.EventManager.Unhook<TrackerRenderedHudEvent>();
            if (!ModEntry.PlayerState.ProspectorHunt.IsActive && !ModEntry.PlayerState.ScavengerHunt.IsActive)
                ModEntry.EventManager.Unhook<PointerUpdateTickedEvent>();
        }
    }
}