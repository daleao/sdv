namespace DaLion.Redux.Framework.Professions.Events.GameLoop.DayEnding;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PiperDayEndingEvent : DayEndingEvent
{
    private static readonly int PiperBuffId = (ModEntry.Manifest.UniqueID + Profession.Piper).GetHashCode();

    /// <summary>Initializes a new instance of the <see cref="PiperDayEndingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PiperDayEndingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        Game1.buffsDisplay.removeOtherBuff(PiperBuffId);
        Array.Clear(ModEntry.State.Professions.AppliedPiperBuffs, 0, 12);
        this.Disable();
    }
}
