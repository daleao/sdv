namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common;
using HarmonyLib;
using StardewValley.Tools;
using System;
using System.Reflection;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotCanAutoFirePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotCanAutoFirePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.CanAutoFire));
    }

    #region harmony patches

    /// <summary>Patch to allow auto-fire during Desperado Ultimate.</summary>
    [HarmonyPrefix]
    private static bool SlingshotCanAutoFirePrefix(Slingshot __instance, ref bool __result)
    {
        try
        {
            var who = __instance.getLastFarmerToUse();
            __result = who.IsLocalPlayer && who.get_Ultimate() is DeathBlossom { IsActive: true };
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}