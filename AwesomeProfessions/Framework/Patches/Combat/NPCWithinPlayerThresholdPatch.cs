namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Monsters;

using SuperMode;

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

    /// <summary>Patch to make Poacher invisible in Super Mode.</summary>
    [HarmonyPrefix]
    private static bool NPCWithinPlayerThresholdPrefix(NPC __instance, ref bool __result)
    {
        try
        {
            if (__instance is not Monster) return true; // run original method

            var foundPlayer = ModEntry.ModHelper.Reflection.GetMethod(__instance, "findPlayer").Invoke<Farmer>();
            if (!foundPlayer.IsLocalPlayer || ModEntry.State.Value.SuperMode is not
                    {Index: SuperModeIndex.Poacher, IsActive: true})
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