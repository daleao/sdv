namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System;
using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

#endregion using directives

internal class SpelunkerBuffDisplayUpdateTickedEvent : UpdateTickedEvent
{
    private const int SHEET_INDEX = 40;

    private readonly int _buffId;

    /// <summary>Construct an instance.</summary>
    internal SpelunkerBuffDisplayUpdateTickedEvent()
    {
        _buffId = (ModEntry.Manifest.UniqueID + (int) Profession.Spelunker).GetHashCode();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        if (Game1.currentLocation is not MineShaft) return;

        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == _buffId);
        if (buff is not null) return;

        var bonusLadderChance = (ModEntry.State.Value.SpelunkerLadderStreak * 0.5f).ToString("0.0");
        var bonusSpeed = Math.Min(ModEntry.State.Value.SpelunkerLadderStreak / 10 + 1,
            ModEntry.Config.SpelunkerSpeedCap);
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
                bonusSpeed,
                0,
                0,
                1,
                "Spelunker",
                ModEntry.ModHelper.Translation.Get("spelunker.name." + (Game1.player.IsMale ? "male" : "female")))
            {
                which = _buffId,
                sheetIndex = SHEET_INDEX,
                millisecondsDuration = 0,
                description =
                    ModEntry.ModHelper.Translation.Get("spelunker.buffdesc", new {bonusLadderChance, bonusSpeed})
            }
        );
    }
}