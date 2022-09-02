namespace DaLion.Stardew.Slingshots.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotGetRequiredChargeTimePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotGetRequiredChargeTimePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.GetRequiredChargeTime));
        Postfix!.before = new[] { "DaLion.ImmersiveProfessions", "DaLion.ImmersiveRings" };
    }

    #region harmony patches

    /// <summary>Apply Emerald Enchantment to Slingshot.</summary>
    [HarmonyPostfix, HarmonyBefore("DaLion.ImmersiveProfessions", "DaLion.ImmersiveRings")]
    private static void SlingshotGetRequiredChargeTimePostfix(Slingshot __instance, ref float __result)
    {
        __result *= 1f - __instance.GetEnchantmentLevel<EmeraldEnchantment>() * 0.1f;
    }

    #endregion harmony patches
}