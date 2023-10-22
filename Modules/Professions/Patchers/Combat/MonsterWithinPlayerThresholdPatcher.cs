namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Harmony;
using Extensions;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterWithinPlayerThresholdPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterWithinPlayerThresholdPatcher"/> class.</summary>
    internal MonsterWithinPlayerThresholdPatcher()
    {
        this.Target = this.RequireMethod<Monster>(nameof(Monster.withinPlayerThreshold), new Type[] { });
    }

    #region harmony patch

    /// <summary>Patch to make Poacher invisible in Ultimate.</summary>
    [HarmonyPrefix]
    private static bool MonsterWithinPlayerThresholdPrefix(Monster __instance, ref bool __result)
    {
        try
        {
            if (__instance is Ghost)
            {
                return true; // run original logic
            }

            var player = __instance.Get_Target();
            if (!player.IsLocalPlayer || player.IsInAmbush())
            {
                return true; // run original method
            }

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
