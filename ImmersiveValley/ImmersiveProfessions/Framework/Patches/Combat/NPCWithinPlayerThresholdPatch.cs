namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Monsters;

using Extensions;
using Ultimate;

#endregion using directives

[UsedImplicitly]
internal class NPCWithinPlayerThresholdPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal NPCWithinPlayerThresholdPatch()
    {
        Original = RequireMethod<NPC>(nameof(NPC.withinPlayerThreshold), new[] {typeof(int)});
    }

    #region harmony patch

    /// <summary>Patch to make Poacher invisible in Ultimate.</summary>
    [HarmonyPrefix]
    private static bool NPCWithinPlayerThresholdPrefix(NPC __instance, ref bool __result)
    {
        try
        {
            if (__instance is not Monster) return true; // run original method

            var player = Game1.getFarmer(__instance.ReadDataAs("Target", Game1.player.UniqueMultiplayerID));
            if (!player.IsLocalPlayer || ModEntry.PlayerState.RegisteredUltimate is not Ambush {IsActive: true})
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