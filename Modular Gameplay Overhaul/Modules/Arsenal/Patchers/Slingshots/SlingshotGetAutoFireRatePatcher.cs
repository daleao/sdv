namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Slingshots;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Enchantments;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotGetAutoFireRatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotGetAutoFireRatePatcher"/> class.</summary>
    internal SlingshotGetAutoFireRatePatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.GetAutoFireRate));
    }

    #region harmony patches

    /// <summary>Implement <see cref="GatlingEnchantment"/> effect.</summary>
    [HarmonyPostfix]
    private static void SlingshotGetAutoFireRatePostfix(Slingshot __instance, ref float __result)
    {
        var firer = __instance.getLastFarmerToUse();
        var ultimate = ProfessionsModule.IsEnabled ? firer.Get_Ultimate() : null;
        if ((ultimate is not null && ultimate.Index == Farmer.desperado &&
             ultimate.IsActive) || !__instance.hasEnchantmentOfType<GatlingEnchantment>())
        {
            return;
        }

        __result *= 1.5f;
    }

    #endregion harmony patches
}
