namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotGetRequiredChargeTimePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotGetRequiredChargeTimePatcher"/> class.</summary>
    internal SlingshotGetRequiredChargeTimePatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.GetRequiredChargeTime));
        this.Postfix!.after = new[] { OverhaulModule.Arsenal.Namespace };
    }

    #region harmony patches

    /// <summary>Patch to reduce Slingshot charge time for Desperado.</summary>
    [HarmonyPostfix]
    [HarmonyAfter("Overhaul.Modules.Arsenal")]
    private static void SlingshotGetRequiredChargeTimePostfix(Slingshot __instance, ref float __result)
    {
        var firer = __instance.getLastFarmerToUse();
        if (!firer.IsLocalPlayer || !firer.HasProfession(Profession.Desperado))
        {
            return;
        }

        __result *= 1f - MathHelper.Lerp(0f, 0.5f, (float)firer.health / firer.maxHealth);
    }

    #endregion harmony patches
}
