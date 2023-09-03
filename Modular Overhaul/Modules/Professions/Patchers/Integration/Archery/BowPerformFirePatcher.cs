namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration.Archery;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
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

    /// <summary>Adds Rascal ammo recovery + apply Desperado overcharge effects + cache projectile properties.</summary>
    [HarmonyPrefix]
    private static void BowPerformFirePrefix(BasicProjectile projectile, Slingshot slingshot, Farmer who)
    {
        var projectileData = ArcheryIntegration.Instance!.ModApi!.GetProjectileData(Manifest, projectile);
        if (projectileData is null || !who.HasProfession(Profession.Rascal))
        {
            return;
        }

        var breakChance = Reflector.GetUnboundFieldGetter<Projectile, float>(projectile, "_breakChance").Invoke(projectile);
        breakChance *= who.HasProfession(Profession.Rascal, true) ? 1.7f : 1.35f;
        if (!who.HasProfession(Profession.Desperado))
        {
            Reflector.GetUnboundFieldSetter<BasicProjectile, float>(projectile, "_breakChance").Invoke(projectile, breakChance);
            return;
        }

        var overcharge = slingshot.GetOvercharge();

        breakChance *= 2f - overcharge;
        Reflector.GetUnboundFieldSetter<BasicProjectile, float>(projectile, "_breakChance").Invoke(projectile, breakChance);

        var collectiveDamage = Reflector.GetUnboundFieldGetter<BasicProjectile, int>(projectile, "_collectiveDamage").Invoke(projectile);
        collectiveDamage = (int)(collectiveDamage * overcharge);
        Reflector.GetUnboundFieldSetter<BasicProjectile, int>(projectile, "_collectiveDamage").Invoke(projectile, collectiveDamage);

        var knockback = Reflector.GetUnboundFieldGetter<BasicProjectile, float>(projectile, "_knockback").Invoke(projectile);
        knockback *= overcharge;
        Reflector.GetUnboundFieldSetter<BasicProjectile, float>(projectile, "_knockback").Invoke(projectile, knockback);

        Reflector.GetUnboundFieldGetter<Projectile, NetFloat>(projectile, "xVelocity")
            .Invoke(projectile).Value *= overcharge;
        Reflector.GetUnboundFieldGetter<Projectile, NetFloat>(projectile, "yVelocity")
            .Invoke(projectile).Value *= overcharge;
        ArrowProjectile_Properties.Create(
            projectile,
            slingshot,
            overcharge,
            projectileData.DoesExplodeOnImpact != true && projectileData.BreakChance is < 1f);
    }

    #endregion harmony patches
}
