namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Tools;

using SuperMode;

#endregion using directives

[UsedImplicitly]
internal class SlingshotGetRequiredChargeTimePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotGetRequiredChargeTimePatch()
    {
        Original = RequireMethod<Slingshot>(nameof(Slingshot.GetRequiredChargeTime));
    }

    #region harmony patches

    /// <summary>Patch to reduce slingshot charge time for Desperado.</summary>
    [HarmonyPostfix]
    private static void SlingshotGetRequiredChargeTimePostfix(Slingshot __instance, ref float __result)
    {
        var firer = __instance.getLastFarmerToUse();
        if (!firer.IsLocalPlayer || ModEntry.State.Value.SuperMode is not DesperadoTemerity) return;
        
        __result *= 1f - ModEntry.State.Value.SuperMode.Gauge.PercentFill;
    }

    #endregion harmony patches
}