namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged.Integration;

#region using directives

using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[ModRequirement("PeacefulEnd.Archery", "Archery", "2.1.0")]
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
    private static void BowPerformFirePrefix(BasicProjectile projectile, Slingshot slingshot, Farmer who)
    {
        var projectileData = ArcheryIntegration.Instance!.ModApi!.GetProjectileData(Manifest, projectile);
        if (projectileData is null)
        {
            return;
        }

        if (CombatModule.Config.WeaponsSlingshots.EnableOverhaul)
        {
            var weaponData = ArcheryIntegration.Instance.ModApi!.GetWeaponData(Manifest, slingshot);
            if (weaponData is not null && projectileData.BaseDamage is { } baseDamage)
            {
                Reflector.GetUnboundFieldSetter<BasicProjectile, int>(projectile, "_collectiveDamage").Invoke(
                    projectile,
                    (int)(weaponData.DamageRange.Get(Game1.random, baseDamage, baseDamage) * (1f + who.attackIncreaseModifier)));
            }
        }

        var collectiveDamage = Reflector.GetUnboundFieldGetter<BasicProjectile, int>(projectile, "_collectiveDamage").Invoke(projectile);
        collectiveDamage = (int)(collectiveDamage * slingshot.Get_EffectiveDamageModifier());
        Reflector.GetUnboundFieldSetter<BasicProjectile, int>(projectile, "_collectiveDamage").Invoke(projectile, collectiveDamage);

        var criticalChance = Reflector.GetUnboundFieldGetter<BasicProjectile, float>(projectile, "_criticalChance").Invoke(projectile);
        criticalChance += slingshot.Get_EffectiveCritChance();
        Reflector.GetUnboundFieldSetter<BasicProjectile, float>(projectile, "_criticalChance").Invoke(projectile, criticalChance);

        var criticalDamageMultiplier = Reflector.GetUnboundFieldGetter<BasicProjectile, float>(projectile, "_criticalDamageMultiplier").Invoke(projectile);
        criticalDamageMultiplier += slingshot.Get_EffectiveCritPower();
        Reflector.GetUnboundFieldSetter<BasicProjectile, float>(projectile, "_criticalDamageMultiplier").Invoke(projectile, criticalDamageMultiplier);

        var knockback = Reflector.GetUnboundFieldGetter<BasicProjectile, float>(projectile, "_knockback").Invoke(projectile);
        knockback += slingshot.Get_EffectiveKnockback();
        Reflector.GetUnboundFieldSetter<BasicProjectile, float>(projectile, "_knockback").Invoke(projectile, knockback);
    }

    #endregion harmony patches
}
