using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class SuperModeBuffDisplayUpdateTickedEvent : UpdateTickedEvent
{
    private const int
        SHEET_INDEX_OFFSET =
            10; // added to profession index to obtain the buff sheet index.</summary>

    /// <inheritdoc />
    public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        if (ModState.SuperModeIndex <= 0)
        {
            ModEntry.Subscriber.Unsubscribe(GetType());
            return;
        }

        if (ModState.SuperModeGaugeValue < 10) return;

        var buffID = ModEntry.Manifest.UniqueID.GetHashCode() + ModState.SuperModeIndex;
        var professionIndex = ModState.SuperModeIndex;
        var professionName = Utility.Professions.NameOf(professionIndex);
        var magnitude = GetSuperModePrimaryBuffMagnitude(professionName);
        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffID);
        if (buff == null)
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
                    0,
                    0,
                    0,
                    1,
                    professionName,
                    ModEntry.ModHelper.Translation.Get(professionName.ToLower() + ".buff"))
                {
                    which = buffID,
                    sheetIndex = professionIndex + SHEET_INDEX_OFFSET,
                    millisecondsDuration = 0,
                    description = ModEntry.ModHelper.Translation.Get(professionName.ToLower() + ".buffdesc",
                        new {magnitude})
                });
    }

    private static string GetSuperModePrimaryBuffMagnitude(string professionName)
    {
#pragma warning disable 8509
        return professionName switch
#pragma warning restore 8509
        {
            "Brute" => ((Utility.Professions.GetBruteBonusDamageMultiplier(Game1.player) - 1.15) * 100f)
                .ToString("0.0"),
            "Poacher" => Utility.Professions.GetPoacherCritDamageMultiplier().ToString("0.0"),
            "Desperado" => ((Utility.Professions.GetDesperadoBulletPower() - 1f) * 100f).ToString("0.0"),
            "Piper" => Utility.Professions.GetPiperSlimeSpawnAttempts().ToString("0")
        };
    }
}