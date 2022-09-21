namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using DaLion.Common.Events;
using DaLion.Stardew.Professions.Framework.Events.GameLoop;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class TrackerButtonsChangedEvent : ButtonsChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="TrackerButtonsChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal TrackerButtonsChangedEvent(ProfessionEventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object? sender, ButtonsChangedEventArgs e)
    {
        if (ModEntry.Config.ModKey.JustPressed())
        {
            this.Manager.Enable<PointerUpdateTickedEvent>();
        }
        else if (ModEntry.Config.ModKey.GetState() == SButtonState.Released &&
                 !ModEntry.State.ProspectorHunt.Value.IsActive && !ModEntry.State.ScavengerHunt.Value.IsActive)
        {
            this.Manager.Disable<PointerUpdateTickedEvent>();
        }
    }
}
