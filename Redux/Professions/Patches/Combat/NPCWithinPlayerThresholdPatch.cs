namespace DaLion.Redux.Professions.Patches.Combat;

#region using directives

using System.Reflection;
using DaLion.Redux.Professions.Ultimates;
using DaLion.Redux.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using StardewValley.Monsters;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class NPCWithinPlayerThresholdPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="NPCWithinPlayerThresholdPatch"/> class.</summary>
    internal NPCWithinPlayerThresholdPatch()
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
            if (__instance is not Monster)
            {
                return true; // run original method
            }

            var player = Game1.getFarmer(__instance.Read(DataFields.Target, Game1.player.UniqueMultiplayerID));
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
