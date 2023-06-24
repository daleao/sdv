namespace DaLion.Overhaul.Modules.Slingshots.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Overhaul.Modules.Slingshots.Integrations;
using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[RequiresMod("PeacefulEnd.Archery", "Archery", "2.1.0")]
internal sealed class BowRefreshSpecialAttackCooldownPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BowRefreshSpecialAttackCooldownPatcher"/> class.</summary>
    internal BowRefreshSpecialAttackCooldownPatcher()
    {
        this.Target = "Archery.Framework.Objects.Weapons.Bow"
            .ToType()
            .RequireMethod("RefreshSpecialAttackCooldown");
    }

    #region harmony patches

    /// <summary>Apply Garnet Ring and Enchantment effects to Bows.</summary>
    [HarmonyPrefix]
    private static void BowRefreshSpecialAttackCooldownPrefix(Tool tool, object specialAttack)
    {
        if (tool is not Slingshot slingshot)
        {
            return;
        }

        var firer = slingshot.getLastFarmerToUse();
        if (!firer.IsLocalPlayer)
        {
            return;
        }

        var bowData = ArcheryIntegration.Instance!.ModApi!.GetWeaponData(Manifest, slingshot);
        if (bowData is null)
        {
            return;
        }

        var cooldown = ArcheryIntegration.Instance!.ModApi!.GetSpecialAttackCooldown(Manifest, slingshot);
        if (!cooldown.HasValue)
        {
            return;
        }

        cooldown = (int)(cooldown * slingshot.Get_EffectiveCooldownReduction() * Game1.player.Get_CooldownReduction());
        Reflector.GetStaticFieldSetter<int>("Archery.Framework.Objects.Weapons.Bow", "ActiveCooldown").Invoke(cooldown.Value);
    }

    #endregion harmony patches
}
