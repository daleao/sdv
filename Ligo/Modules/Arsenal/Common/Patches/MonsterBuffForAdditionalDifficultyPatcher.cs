namespace DaLion.Ligo.Modules.Arsenal.Common.Patches;

#region using directives

using HarmonyLib;
using Shared.Harmony;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterBuffForAdditionalDifficultyPatcher : HarmonyPatcher
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MonsterBuffForAdditionalDifficultyPatcher"/> class.Construct and
    ///     instance.
    /// </summary>
    internal MonsterBuffForAdditionalDifficultyPatcher()
    {
        this.Target = this.RequireMethod<Monster>("BuffForAdditionalDifficulty");
    }

    #region harmony patches

    /// <summary>Modify combat difficulty.</summary>
    [HarmonyPostfix]
    private static void MonsterBuffForAdditionalDifficultyPostfix(Monster __instance)
    {
        if (ModEntry.Config.Arsenal.VariedEncounters)
        {
            var r = new Random(Guid.NewGuid().GetHashCode());

            var luckModifier = (Game1.player.DailyLuck * 3d) + 1d;
            __instance.Health = (int)(__instance.Health * r.Next(80, 121) / 1000d * luckModifier);
            __instance.DamageToFarmer = (int)(__instance.DamageToFarmer * r.Next(10, 41) / 10d * luckModifier);
            __instance.resilience.Value = (int)(__instance.resilience.Value * r.Next(10, 21) / 10d * luckModifier);

            var addedSpeed = r.NextDouble() > 0.5 + (Game1.player.DailyLuck * 2d) ? 1 :
                r.NextDouble() < 0.5 - (Game1.player.DailyLuck * 2d) ? -1 : 0;
            __instance.speed = Math.Max(__instance.speed + addedSpeed, 1);

            __instance.durationOfRandomMovements.Value =
                (int)(__instance.durationOfRandomMovements.Value * (r.NextDouble() - 0.5));
            __instance.moveTowardPlayerThreshold.Value =
                Math.Max(__instance.moveTowardPlayerThreshold.Value + r.Next(-1, 2), 1);
        }

        __instance.Health = (int)Math.Round(__instance.Health * ModEntry.Config.Arsenal.MonsterHealthMultiplier);
        __instance.DamageToFarmer =
            (int)Math.Round(__instance.DamageToFarmer * ModEntry.Config.Arsenal.MonsterDamageMultiplier);
        __instance.resilience.Value =
            (int)Math.Round(__instance.resilience.Value * ModEntry.Config.Arsenal.MonsterDefenseMultiplier);
    }

    #endregion harmony patches
}
