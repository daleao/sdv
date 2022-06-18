namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System;
using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class DemolitionistUpdateTickedEvent : UpdateTickedEvent
{
    private const int SHEET_INDEX_I = 41;

    private readonly int _buffId;

    /// <summary>Construct an instance.</summary>
    internal DemolitionistUpdateTickedEvent()
    {
        _buffId = (ModEntry.Manifest.UniqueID + Profession.Demolitionist).GetHashCode();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        if (ModEntry.PlayerState.DemolitionistExcitedness <= 0) Disable();

        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == _buffId);
        if (buff is not null) return;

        Game1.buffsDisplay.addOtherBuff(
            new(0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                ModEntry.PlayerState.DemolitionistExcitedness,
                0,
                0,
                1,
                "Demolitionist",
                ModEntry.i18n.Get(
                    "demolitionist.name" + (Game1.player.IsMale ? ".male" : ".female")))
            {
                which = _buffId,
                sheetIndex = SHEET_INDEX_I,
                millisecondsDuration = 555,
                description = ModEntry.i18n.Get("demolitionist.buffdesc")
            }
        );

        var buffDecay = ModEntry.PlayerState.DemolitionistExcitedness >= 4 ? 2 : 1;
        ModEntry.PlayerState.DemolitionistExcitedness =
            Math.Max(0, ModEntry.PlayerState.DemolitionistExcitedness - buffDecay);
    }
}