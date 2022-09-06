namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Events;
using StardewModdingAPI.Events;
using System;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class DemolitionistUpdateTickedEvent : UpdateTickedEvent
{
    private const int BUFF_SHEET_INDEX_I = 41;

    private readonly int _buffId;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal DemolitionistUpdateTickedEvent(ProfessionEventManager manager)
        : base(manager)
    {
        _buffId = (ModEntry.Manifest.UniqueID + Profession.Demolitionist).GetHashCode();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (ModEntry.State.DemolitionistExcitedness <= 0) Disable();

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
                ModEntry.State.DemolitionistExcitedness,
                0,
                0,
                1,
                "Demolitionist",
                ModEntry.i18n.Get(
                    "demolitionist.name" + (Game1.player.IsMale ? ".male" : ".female")))
            {
                which = _buffId,
                sheetIndex = BUFF_SHEET_INDEX_I,
                millisecondsDuration = 555,
                description = ModEntry.i18n.Get("demolitionist.buff.desc")
            }
        );

        var buffDecay = ModEntry.State.DemolitionistExcitedness >= 4 ? 2 : 1;
        ModEntry.State.DemolitionistExcitedness =
            Math.Max(0, ModEntry.State.DemolitionistExcitedness - buffDecay);
    }
}