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
internal sealed class SlingshotGetAutoFireRatePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotGetAutoFireRatePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.GetAutoFireRate));
        Prefix!.priority = Priority.High;
        Prefix!.before = new[] { "DaLion.ImmersiveArsenal" };
    }

    #region harmony patches

    /// <summary>Implement <see cref="GatlingEnchantment"/> effect.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyBefore("DaLion.ImmersiveArsenal")]
    private static bool SlingshotCanAutoFirePrefix(Slingshot __instance, ref float __result)
    {
        try
        {
            var who = __instance.getLastFarmerToUse();
            if (!who.IsLocalPlayer || who.get_Ultimate() is not DeathBlossom { IsActive: true })
                return false; // don't run original logic

            __result = 0.3f;
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