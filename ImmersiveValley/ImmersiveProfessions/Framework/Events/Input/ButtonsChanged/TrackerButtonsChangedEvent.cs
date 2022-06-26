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
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal TrackerButtonsChangedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object? sender, ButtonsChangedEventArgs e)
    {
        if (ModEntry.Config.ModKey.JustPressed())
        {
            Manager.Hook<PointerUpdateTickedEvent>();
            Manager.Hook<TrackerRenderedHudEvent>();
        }
        else if (ModEntry.Config.ModKey.GetState() == SButtonState.Released)
        {
            Manager.Unhook<TrackerRenderedHudEvent>();
            if (!ModEntry.PlayerState.ProspectorHunt.IsActive && !ModEntry.PlayerState.ScavengerHunt.IsActive)
                Manager.Unhook<PointerUpdateTickedEvent>();
        }
    }
}