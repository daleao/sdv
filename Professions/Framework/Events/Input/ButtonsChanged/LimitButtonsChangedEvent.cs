namespace DaLion.Professions.Framework.Events.Input.ButtonsChanged;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="LimitButtonsChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[LimitEvent]
[UsedImplicitly]
internal sealed class LimitButtonsChangedEvent(EventManager? manager = null)
    : ButtonsChangedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.LimitBreak?.CanActivate == true;

    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object? sender, ButtonsChangedEventArgs e)
    {
        State.LimitBreak!.CheckForActivation();
    }
}
