namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Monsters;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterBuffForAdditionalDifficultyPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct and instance.</summary>
    internal MonsterBuffForAdditionalDifficultyPatch()
    {
        Target = RequireMethod<Monster>("BuffForAdditionalDifficulty");
    }

    #region harmony patches

    /// <summary>Modify combat difficulty.</summary>
    [HarmonyPostfix]
    private static void MonsterBuffForAdditionalDifficultyPostfix(Monster __instance)
    {
        if (ModEntry.Config.VariedMonsterStats)
        {
            var r = new Random(Guid.NewGuid().GetHashCode());
            
            var luckModifier = Game1.player.DailyLuck * 3d + 1d;
            __instance.Health = (int) (__instance.Health * r.Next(80, 121) / 1000d * luckModifier);
            __instance.DamageToFarmer = (int) (__instance.DamageToFarmer * r.Next(10, 41) / 10d * luckModifier);
            __instance.resilience.Value = (int) (__instance.resilience.Value * r.Next(10, 21) / 10d * luckModifier);

            var addedSpeed = r.NextDouble() > 0.5 + Game1.player.DailyLuck * 2d ? 1 :
                r.NextDouble() < 0.5 - Game1.player.DailyLuck * 2d ? -1 : 0;
            __instance.speed = Math.Max(__instance.speed + addedSpeed, 1);

            __instance.durationOfRandomMovements.Value =
                (int)(__instance.durationOfRandomMovements.Value * (r.NextDouble() - 0.5));
            __instance.moveTowardPlayerThreshold.Value =
                Math.Max(__instance.moveTowardPlayerThreshold.Value + r.Next(-1, 2), 1);
        }

        __instance.Health = (int)Math.Round(__instance.Health * ModEntry.Config.MonsterHealthMultiplier);
        __instance.DamageToFarmer =
            (int)Math.Round(__instance.DamageToFarmer * ModEntry.Config.MonsterDamageMultiplier);
        __instance.resilience.Value =
            (int)Math.Round(__instance.resilience.Value * ModEntry.Config.MonsterDefenseMultiplier);
    }

    #endregion harmony patches
}