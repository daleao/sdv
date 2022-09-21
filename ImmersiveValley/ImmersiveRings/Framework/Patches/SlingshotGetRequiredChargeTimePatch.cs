namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotGetRequiredChargeTimePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotGetRequiredChargeTimePatch"/> class.</summary>
    internal SlingshotGetRequiredChargeTimePatch()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.GetRequiredChargeTime));
        this.Postfix!.after = new[] { "DaLion.ImmersiveArsenal" };
        this.Postfix!.before = new[] { "DaLion.ImmersiveProfessions" };
    }

    #region harmony patches

    /// <summary>Apply Emerald Enchantment to Slingshot.</summary>
    [HarmonyPostfix]
    [HarmonyAfter("DaLion.ImmersiveArsenal")]
    [HarmonyBefore("DaLion.ImmersiveProfessions")]
    private static void SlingshotGetRequiredChargeTimePostfix(Slingshot __instance, ref float __result)
    {
        var firer = __instance.getLastFarmerToUse();
        if (!firer.IsLocalPlayer)
        {
            return;
        }

        __result *= 1f - firer.weaponSpeedModifier;
    }

    #endregion harmony patches
}
