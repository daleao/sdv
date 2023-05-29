namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[RequiresMod("PeacefulEnd.Archery", "Archery", "1.2.0")]
internal sealed class BowGetSlingshotChargeTimePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BowGetSlingshotChargeTimePatcher"/> class.</summary>
    internal BowGetSlingshotChargeTimePatcher()
    {
        this.Target = "Archery.Framework.Objects.Weapons.Bow"
            .ToType()
            .RequireMethod("GetSlingshotChargeTime");
        this.Postfix!.after = new[] { OverhaulModule.Slingshots.Namespace };
    }

    #region harmony patches

    /// <summary>Patch to reduce Bow charge time for Desperado.</summary>
    [HarmonyPostfix]
    [HarmonyAfter("DaLion.Overhaul.Modules.Slingshots")]
    private static void BowGetSlingshotChargeTimePostfix(Tool tool, ref float __result)
    {
        if (tool is not Slingshot slingshot)
        {
            return;
        }

        var model = ArcheryIntegration.Instance!.ModApi!.GetWeaponData(Manifest, slingshot);
        if (!model.Key)
        {
            return;
        }

        var firer = slingshot.getLastFarmerToUse();
        if (firer.IsLocalPlayer && firer.HasProfession(Profession.Desperado))
        {
            __result *= 1f - MathHelper.Lerp(0f, 0.5f, (float)firer.health / firer.maxHealth);
        }
    }

    #endregion harmony patches
}
