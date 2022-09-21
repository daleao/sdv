namespace DaLion.Stardew.Slingshots.Framework.Patches.Combat;

#region using directives

using DaLion.Common.Integrations.WalkOfLife;
using DaLion.Stardew.Slingshots.Framework.Enchantments;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotGetAutoFireRatePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotGetAutoFireRatePatch"/> class.</summary>
    internal SlingshotGetAutoFireRatePatch()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.GetAutoFireRate));
    }

    #region harmony patches

    /// <summary>Implement <see cref="GatlingEnchantment"/> effect.</summary>
    [HarmonyPostfix]
    private static void SlingshotGetAutoFireRatePostfix(Slingshot __instance, ref float __result)
    {
        var ultimate = ModEntry.ProfessionsApi?.GetRegisteredUltimate();
        if ((ultimate is not null && ultimate.Index == Farmer.desperado &&
             ultimate.IsActive) || !__instance.hasEnchantmentOfType<GatlingEnchantment>())
        {
            return;
        }

        __result *= 1.5f;
    }

    #endregion harmony patches
}
