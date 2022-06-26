namespace DaLion.Stardew.Professions.Framework.Events.GameLoop.DayEnding;

#region using directives

using System;
using JetBrains.Annotations;
using StardewValley;
using StardewModdingAPI.Events;

using Common.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PiperDayEndingEvent : DayEndingEvent
{
    private static readonly int _piperBuffId = (ModEntry.Manifest.UniqueID + Profession.Piper).GetHashCode();

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal PiperDayEndingEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        Game1.buffsDisplay.removeOtherBuff(_piperBuffId);
        Array.Clear(ModEntry.PlayerState.AppliedPiperBuffs, 0, 12);
        Unhook();
    }
}