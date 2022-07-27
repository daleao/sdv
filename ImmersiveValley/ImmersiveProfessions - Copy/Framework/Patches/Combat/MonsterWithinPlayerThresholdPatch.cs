namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common;
using DaLion.Common.ModData;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Reflection;
using Ultimates;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterWithinPlayerThresholdPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MonsterWithinPlayerThresholdPatch()
    {
        Target = RequireMethod<Monster>(nameof(Monster.withinPlayerThreshold), new Type[] { });
    }

    #region harmony patch

    /// <summary>Patch to make Poacher invisible in Ultimate.</summary>
    [HarmonyPrefix]
    private static bool MonsterWithinPlayerThresholdPrefix(Monster __instance, ref bool __result)
    {
        try
        {
            if (__instance is Ghost) return true; // run original logic

            var player = Game1.getFarmer(ModDataIO.Read(__instance, "Target", Game1.player.UniqueMultiplayerID));
            if (!player.IsLocalPlayer || ModEntry.Player.RegisteredUltimate is not Ambush { IsActive: true })
                return true; // run original method

            __result = false;
            return false; // don't run original method
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patch
}