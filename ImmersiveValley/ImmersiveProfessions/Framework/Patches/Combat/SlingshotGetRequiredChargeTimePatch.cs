namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotGetRequiredChargeTimePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotGetRequiredChargeTimePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.GetRequiredChargeTime));
        Postfix!.after = new[] { "DaLion.ImmersiveRings", "DaLion.ImmersiveSlingshots" };
    }

    #region harmony patches

    /// <summary>Patch to reduce Slingshot charge time for Desperado.</summary>
    [HarmonyPostfix]
    [HarmonyBefore("DaLion.ImmersiveRings", "DaLion.ImmersiveSlingshots")]
    private static void SlingshotGetRequiredChargeTimePostfix(Slingshot __instance, ref float __result)
    {
        var firer = __instance.getLastFarmerToUse();
        if (!firer.IsLocalPlayer || !firer.HasProfession(Profession.Desperado)) return;

        __result *= 1f - MathHelper.Lerp(0f, 0.5f, (float)firer.health / firer.maxHealth);
    }

    #endregion harmony patches
}