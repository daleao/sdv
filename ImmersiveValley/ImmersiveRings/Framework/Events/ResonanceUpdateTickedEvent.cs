namespace DaLion.Stardew.Rings.Framework.Events;

#region using directives

using Common.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal class ResonanceUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ResonanceUpdateTickedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        Phase.Update();
    }
}