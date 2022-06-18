namespace DaLion.Stardew.Professions.Framework.Events.GameLoop.DayEnding;

#region using directives

using System;
using JetBrains.Annotations;
using StardewValley;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PiperDayEndingEvent : DayEndingEvent
{
    private static readonly int _piperBuffId = (ModEntry.Manifest.UniqueID + Profession.Piper).GetHashCode();

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object sender, DayEndingEventArgs e)
    {
        Game1.buffsDisplay.removeOtherBuff(_piperBuffId);
        Array.Clear(ModEntry.PlayerState.AppliedPiperBuffs, 0, 12);
        Disable();
    }
}