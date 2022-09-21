namespace DaLion.Stardew.Slingshots.Framework.Patches;

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
        this.Postfix!.before = new[] { "DaLion.ImmersiveProfessions", "DaLion.ImmersiveRings" };
    }

    #region harmony patches

    /// <summary>Apply Emerald Enchantment to Slingshot.</summary>
    [HarmonyPostfix]
    [HarmonyBefore("DaLion.ImmersiveProfessions", "DaLion.ImmersiveRings")]
    private static void SlingshotGetRequiredChargeTimePostfix(Slingshot __instance, ref float __result)
    {
        __result *= 1f - (__instance.GetEnchantmentLevel<EmeraldEnchantment>() * 0.1f);
    }

    #endregion harmony patches
}
