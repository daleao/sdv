namespace DaLion.Professions.Framework.Events.Display.WindowResized;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PipedMinionHudWindowResizedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[LimitEvent]
[UsedImplicitly]
internal sealed class PipedMinionHudWindowResizedEvent(EventManager? manager = null)
    : WindowResizedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.PipedMinionMenu is not null;

    /// <inheritdoc />
    protected override void OnWindowResizedImpl(object? sender, WindowResizedEventArgs e)
    {
        State.PipedMinionMenu!.populateClickableComponentList();
    }
}
