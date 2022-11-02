namespace DaLion.Redux.Framework.Professions.Events.Input;

#region using directives

using DaLion.Redux.Framework.Professions.Events.GameLoop;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class TrackerButtonsChangedEvent : ButtonsChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="TrackerButtonsChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TrackerButtonsChangedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object? sender, ButtonsChangedEventArgs e)
    {
        if (ModEntry.Config.Professions.ModKey.JustPressed())
        {
            this.Manager.Enable<PointerUpdateTickedEvent>();
        }
        else if (ModEntry.Config.Professions.ModKey.GetState() == SButtonState.Released &&
                 !ModEntry.State.Professions.ProspectorHunt.Value.IsActive && !ModEntry.State.Professions.ScavengerHunt.Value.IsActive)
        {
            this.Manager.Disable<PointerUpdateTickedEvent>();
        }
    }
}
