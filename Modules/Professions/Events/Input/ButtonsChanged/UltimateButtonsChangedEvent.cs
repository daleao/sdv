namespace DaLion.Overhaul.Modules.Professions.Events.Input.ButtonsChanged;

#region using directives

using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UltimateEvent]
[UsedImplicitly]
internal sealed class UltimateButtonsChangedEvent : ButtonsChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="UltimateButtonsChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal UltimateButtonsChangedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object? sender, ButtonsChangedEventArgs e)
    {
        Game1.player.Get_Ultimate()!.CheckForActivation();
    }
}
