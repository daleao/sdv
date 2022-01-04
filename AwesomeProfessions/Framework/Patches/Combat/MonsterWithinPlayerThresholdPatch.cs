using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Reflection;

namespace TheLion.Stardew.Professions.Framework.Patches.Combat;

[UsedImplicitly]
internal class MonsterWithinPlayerThresholdPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal MonsterWithinPlayerThresholdPatch()
    {
        Original = RequireMethod<Monster>(nameof(Monster.withinPlayerThreshold), new Type[] { });
    }

    #region harmony patch

    /// <summary>Patch to make Poacher invisible in Super Mode.</summary>
    [HarmonyPrefix]
    private static bool MonsterWithinPlayerThresholdPrefix(Monster __instance, ref bool __result)
    {
        try
        {
            var foundPlayer = ModEntry.ModHelper.Reflection.GetMethod(__instance, "findPlayer").Invoke<Farmer>();
            if (!foundPlayer.IsLocalPlayer || !ModEntry.State.Value.IsSuperModeActive ||
                ModEntry.State.Value.SuperModeIndex != Utility.Professions.IndexOf("Poacher"))
                return true; // run original method

            __result = false;
            return false; // don't run original method
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
            return true; // default to original logic
        }
    }

    #endregion harmony patch
}