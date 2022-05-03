namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Extensions;
using Framework.Ultimate;

#endregion using directives

[UsedImplicitly]
internal class BruteUpdateTickedEvent : UpdateTickedEvent
{
    private const int SHEET_INDEX_I = 36;

    private readonly int _buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int) Profession.Brute;
    
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        if (ModEntry.PlayerState.BruteRageCounter <= 0) return;

        if (Game1.game1.IsActive && Game1.shouldTimePass() && ModEntry.PlayerState.BruteRageCounter > 0 &&
            e.IsOneSecond)
        {
            ++ModEntry.PlayerState.SecondsSinceLastCombat;
            // decay counter every 5 seconds after 30 seconds out of combat
            if (ModEntry.PlayerState.SecondsSinceLastCombat > 30 && e.IsMultipleOf(300))
                --ModEntry.PlayerState.BruteRageCounter;
        }

        if (Game1.player.hasBuff(_buffId)) return;

        var magnitude = (ModEntry.PlayerState.BruteRageCounter * Frenzy.PCT_INCREMENT_PER_RAGE_F).ToString("P");
        Game1.buffsDisplay.addOtherBuff(
            new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1,
                "Brute",
                ModEntry.ModHelper.Translation.Get("brute.name." + (Game1.player.IsMale ? "male" : "female")) + " " +
                ModEntry.ModHelper.Translation.Get("brute.buff"))
            {
                which = _buffId,
                sheetIndex = SHEET_INDEX_I,
                millisecondsDuration = 0,
                description =
                    ModEntry.ModHelper.Translation.Get(
                        "brute.buffdesc" + (Game1.player.HasProfession(Profession.Brute, true)
                            ? ".prestiged"
                            : string.Empty), new {magnitude})
            }
        );
    }
}