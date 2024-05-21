namespace DaLion.Core.Framework.Events.Debug;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="DebugUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[Debug]
internal sealed class DebugUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.DebugMode;

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (State.FpsCounter is null)
        {
            State.FpsCounter = new FrameRateCounter(GameRunner.instance);
            ModHelper.Reflection.GetMethod(State.FpsCounter, "LoadContent").Invoke();
        }

        State.FpsCounter.Update(Game1.currentGameTime);
    }
}
