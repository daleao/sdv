namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class NpcWithinPlayerThresholdPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NpcWithinPlayerThresholdPatcher"/> class.</summary>
    internal NpcWithinPlayerThresholdPatcher()
    {
        this.Target = this.RequireMethod<NPC>(nameof(NPC.withinPlayerThreshold), new[] { typeof(int) });
    }

    #region harmony patch

    /// <summary>Patch to make Poacher invisible in Ultimate.</summary>
    [HarmonyPrefix]
    private static bool NPCWithinPlayerThresholdPrefix(NPC __instance, ref bool __result)
    {
        try
        {
            if (__instance is not Monster monster)
            {
                return true; // run original method
            }

            var player = monster.Get_Target();
            if (!player.IsLocalPlayer || player.Get_Ultimate() is not Ambush { IsActive: true })
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
