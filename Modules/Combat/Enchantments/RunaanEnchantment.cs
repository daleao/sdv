namespace DaLion.Overhaul.Modules.Combat.Enchantments;

#region using directives

using System.Linq;
using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Combat.Projectiles;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>Fire 2 additional projectiles.</summary>
[XmlType("Mods_DaLion_RunaanEnchantment")]
public sealed class RunaanEnchantment : BaseSlingshotEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Runaan_Name();
    }

    /// <inheritdoc />
    protected override void _OnFire(
        Slingshot slingshot,
        BasicProjectile projectile,
        int damageBase,
        float damageMod,
        float knockback,
        float overcharge,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotationVelocity,
        GameLocation location,
        Farmer firer)
    {
        if (slingshot.Get_IsOnSpecial())
        {
            return;
        }

        var targets = location.characters.OfType<Monster>().ToList();
        if (targets.Count == 0)
        {
            return;
        }

        var velocity = new Vector2(xVelocity, yVelocity);
        var speed = velocity.Length();
        var facingDirectionVector = ((FacingDirection)firer.FacingDirection).ToVector() * 64f;

        // do clockwise projectile
        var runaanStartingPosition = startingPosition + facingDirectionVector.Rotate(30);
        var target = runaanStartingPosition.GetClosest(targets, monster => monster.Position, out _);
        var targetDirection = target.GetBoundingBox().Center.ToVector2() - runaanStartingPosition - new Vector2(32f, 32f);
        targetDirection.Normalize();
        var runaanVelocity = targetDirection * speed;
        this.FireRunaanProjectile(
            projectile,
            slingshot,
            firer,
            damageBase,
            damageMod,
            knockback,
            overcharge,
            runaanStartingPosition,
            runaanVelocity,
            rotationVelocity);

        // do anti-clockwise projectile
        runaanStartingPosition = startingPosition + facingDirectionVector.Rotate(-30);
        target = runaanStartingPosition.GetClosest(targets, monster => monster.Position, out _);
        targetDirection = target.GetBoundingBox().Center.ToVector2() - runaanStartingPosition - new Vector2(32f, 32f);
        targetDirection.Normalize();
        runaanVelocity = targetDirection * speed;
        this.FireRunaanProjectile(
            projectile,
            slingshot,
            firer,
            damageBase,
            damageMod,
            knockback,
            overcharge,
            runaanStartingPosition,
            runaanVelocity,
            rotationVelocity);
    }

    private void FireRunaanProjectile(
        Projectile projectile,
        Slingshot slingshot,
        Farmer firer,
        int damageBase,
        float damageMod,
        float knockback,
        float overcharge,
        Vector2 startingPosition,
        Vector2 velocity,
        float rotationVelocity)
    {
        var damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
        BasicProjectile? runaan = projectile switch
        {
            SnowballProjectile => new SnowballProjectile(
                firer,
                1f,
                startingPosition,
                velocity.X,
                velocity.Y,
                rotationVelocity),
            ObjectProjectile @object => new RunaanProjectile(
                @object.Ammo,
                @object.TileSheetIndex,
                slingshot,
                firer,
                damage,
                knockback,
                overcharge,
                startingPosition,
                velocity.X,
                velocity.Y,
                rotationVelocity),
            _ => null,
        };

        if (runaan is null)
        {
            return;
        }

        firer.currentLocation.projectiles.Add(runaan);
    }
}
