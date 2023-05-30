namespace DaLion.Overhaul.Modules.Slingshots.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Slingshots.Integrations;
using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[RequiresMod("PeacefulEnd.Archery", "Archery", "2.1.0")]
internal sealed class BowPerformFirePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BowPerformFirePatcher"/> class.</summary>
    internal BowPerformFirePatcher()
    {
        this.Target = "Archery.Framework.Objects.Weapons.Bow"
            .ToType()
            .RequireMethod(
                "PerformFire",
                new[]
                {
                    typeof(BasicProjectile),
                    typeof(string),
                    typeof(Slingshot),
                    typeof(GameLocation),
                    typeof(Farmer),
                    typeof(bool),
                });
    }

    #region harmony patches

    /// <summary>Apply projectile stat modifiers.</summary>
    [HarmonyPrefix]
    private static void BowPerformFirePrefix(BasicProjectile projectile, Slingshot slingshot)
    {
        var projectileData = ArcheryIntegration.Instance!.ModApi!.GetProjectileData(Manifest, projectile);
        if (projectileData is null)
        {
            return;
        }

        var collectiveDamage = Reflector.GetUnboundFieldGetter<BasicProjectile, int>(projectile, "_collectiveDamage").Invoke(projectile);
        collectiveDamage = (int)(collectiveDamage * slingshot.Get_RubyDamageModifier());
        Reflector.GetUnboundFieldSetter<BasicProjectile, int>(projectile, "_collectiveDamage").Invoke(projectile, collectiveDamage);

        var criticalChance = Reflector.GetUnboundFieldGetter<BasicProjectile, float>(projectile, "_criticalChance").Invoke(projectile);
        criticalChance *= slingshot.Get_AquamarineCritChanceModifier();
        Reflector.GetUnboundFieldSetter<BasicProjectile, float>(projectile, "_criticalChance").Invoke(projectile, criticalChance);

        var criticalDamageMultiplier = Reflector.GetUnboundFieldGetter<BasicProjectile, float>(projectile, "_criticalDamageMultiplier").Invoke(projectile);
        criticalDamageMultiplier *= slingshot.Get_JadeCritPowerModifier();
        Reflector.GetUnboundFieldSetter<BasicProjectile, float>(projectile, "_criticalDamageMultiplier").Invoke(projectile, criticalDamageMultiplier);

        var knockback = Reflector.GetUnboundFieldGetter<BasicProjectile, float>(projectile, "_knockback").Invoke(projectile);
        knockback *= slingshot.Get_AmethystKnockbackModifer();
        Reflector.GetUnboundFieldSetter<BasicProjectile, float>(projectile, "_knockback").Invoke(projectile, knockback);
    }

    #endregion harmony patches
}
