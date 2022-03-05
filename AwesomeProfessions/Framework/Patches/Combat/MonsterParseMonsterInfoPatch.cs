namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal class MonsterParseMonsterInfoPatch : BasePatch
{
    /// <summary>Construct and instance.</summary>
    internal MonsterParseMonsterInfoPatch()
    {
        Original = RequireMethod<Monster>("parseMonsterInfo");
    }

    #region harmony patches

    [HarmonyPostfix]
    private static void MonsterParseMonsterInfoPostfix(Monster __instance)
    {
        __instance.Health = (int) Math.Round(__instance.Health * ModEntry.Config.MonsterHealthMultiplier);
        __instance.DamageToFarmer = (int) Math.Round(__instance.DamageToFarmer * ModEntry.Config.MonsterDamageMultiplier);
        __instance.resilience.Value = (int) Math.Round(__instance.resilience.Value * ModEntry.Config.MonsterDefenseMultiplier);
    }

    #endregion harmony patches
}