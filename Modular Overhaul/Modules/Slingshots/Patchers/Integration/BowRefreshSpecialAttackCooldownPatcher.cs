namespace DaLion.Overhaul.Modules.Slingshots.Patchers.Integration;

#region using directives

using System.Collections.Generic;
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
[RequiresMod("PeacefulEnd.Archery", "Archery", "1.2.0")]
internal sealed class BowRefreshSpecialAttackCooldownPatcher : HarmonyPatcher
{
    private static readonly Lazy<Func<string, List<object>, int>> SpecialAttackCooldownGetter =
        new(() => Reflector.GetUnboundMethodDelegate<Func<string, List<object>, int>>(
            Reflector.GetStaticFieldGetter<object>("Archery.Archery".ToType(), "internalApi"),
            "GetSpecialAttackCooldown"));

    /// <summary>Initializes a new instance of the <see cref="BowRefreshSpecialAttackCooldownPatcher"/> class.</summary>
    internal BowRefreshSpecialAttackCooldownPatcher()
    {
        this.Target = "Archery.Framework.Objects.Weapons.Bow"
            .ToType()
            .RequireMethod("RefreshSpecialAttackCooldown");
    }

    #region harmony patches

    /// <summary>Apply Emerald Ring and Enchantment effects to Slingshot.</summary>
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

        var model = ArcheryIntegration.Instance!.ModApi!.GetWeaponData(Manifest, slingshot);
        if (!model.Key)
        {
            return;
        }

        var specialAttackId = Reflector.GetUnboundPropertyGetter<object, string>(specialAttack, "Id").Invoke(specialAttack);
        var specialAttackArgs = Reflector.GetUnboundPropertyGetter<object, List<object>>(specialAttack, "Arguments").Invoke(specialAttack);
        var cooldown = SpecialAttackCooldownGetter.Value(specialAttackId, specialAttackArgs);
        cooldown = (int)(cooldown * slingshot.Get_GarnetCooldownReduction() * Game1.player.Get_CooldownReduction());
        Reflector.GetStaticFieldSetter<int>("Archery.Framework.Objects.Weapons.Bow", "ActiveCooldown").Invoke(cooldown);
    }

    #endregion harmony patches
}
