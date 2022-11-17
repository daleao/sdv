namespace DaLion.Ligo.Modules.Arsenal.Slingshots.Patches;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotGetRequiredChargeTimePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotGetRequiredChargeTimePatcher"/> class.</summary>
    internal SlingshotGetRequiredChargeTimePatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.GetRequiredChargeTime));
        this.Postfix!.before = new[] { LigoModule.Professions.Namespace };
    }

    #region harmony patches

    /// <summary>Apply Emerald Ring and Enchantment effects to Slingshot.</summary>
    [HarmonyPostfix]
    [HarmonyBefore("Ligo.Modules.Professions")]
    private static void SlingshotGetRequiredChargeTimePostfix(Slingshot __instance, ref float __result)
    {
        var firer = __instance.getLastFarmerToUse();
        if (!firer.IsLocalPlayer)
        {
            return;
        }

        __result *= 1f - ((__instance.GetEnchantmentLevel<EmeraldEnchantment>() +
                           __instance.Read<float>(DataFields.ResonantSpeed)) * 0.1f);
        __result *= 1f - firer.weaponSpeedModifier;
    }

    #endregion harmony patches
}
