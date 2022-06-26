namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System;
using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

using Common.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class SpelunkerUpdateTickedEvent : UpdateTickedEvent
{
    private const int SHEET_INDEX_I = 40;

    private readonly int _buffId = (ModEntry.Manifest.UniqueID + Profession.Spelunker).GetHashCode();

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal SpelunkerUpdateTickedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (Game1.currentLocation is not MineShaft) return;

        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == _buffId);
        if (buff is not null) return;

        var bonusLadderChance = (ModEntry.PlayerState.SpelunkerLadderStreak * 0.5f).ToString("0.0");
        var bonusSpeed = Math.Min(ModEntry.PlayerState.SpelunkerLadderStreak / 10 + 1,
            (int) ModEntry.Config.SpelunkerSpeedCap);
        Game1.buffsDisplay.addOtherBuff(
            new(0, 0, 0, 0, 0, 0, 0, 0, 0, bonusSpeed, 0, 0,
                1,
                "Spelunker",
                ModEntry.i18n.Get("spelunker.name" + (Game1.player.IsMale ? ".male" : ".female")))
            {
                which = _buffId,
                sheetIndex = SHEET_INDEX_I,
                millisecondsDuration = 0,
                description =
                    ModEntry.i18n.Get("spelunker.buffdesc", new {bonusLadderChance, bonusSpeed})
            }
        );
    }
}