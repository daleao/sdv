namespace DaLion.Overhaul.Modules.Slingshots.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Slingshots.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Integrations;
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
        this.Postfix!.before = new[] { OverhaulModule.Professions.Namespace };
    }

    #region harmony patches

    /// <summary>Apply Emerald Ring and Enchantment effects to Slingshot.</summary>
    [HarmonyPostfix]
    [HarmonyBefore("DaLion.Overhaul.Modules.Professions")]
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
        if (firer.IsLocalPlayer)
        {
            __result *= firer.GetTotalFiringSpeedModifier(slingshot);
        }
    }

    #endregion harmony patches
}
