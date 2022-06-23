namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Monsters;

using DaLion.Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterBuffForAdditionalDifficultyPatch : BasePatch
{
    /// <summary>Construct and instance.</summary>
    internal MonsterBuffForAdditionalDifficultyPatch()
    {
        Target = RequireMethod<Monster>("BuffForAdditionalDifficulty");
    }

    #region harmony patches

    /// <summary>Patch to modify combat difficulty.</summary>
    [HarmonyPostfix]
    private static void MonsterBuffForAdditionalDifficultyPostfix(Monster __instance)
    {
        __instance.Health = (int) Math.Round(__instance.Health * ModEntry.Config.MonsterHealthMultiplier);
        __instance.DamageToFarmer = (int) Math.Round(__instance.DamageToFarmer * ModEntry.Config.MonsterDamageMultiplier);
        __instance.resilience.Value = (int) Math.Round(__instance.resilience.Value * ModEntry.Config.MonsterDefenseMultiplier);
    }

    #endregion harmony patches
}