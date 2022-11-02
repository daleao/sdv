namespace DaLion.Redux.Framework.Arsenal.Slingshots;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>A Slingshot <see cref="BasicProjectile"/> with extra useful properties.</summary>
internal sealed class ReduxProjectile : BasicProjectile
{
    public ReduxProjectile(
        Item? ammo,
        int index,
        Slingshot source,
        Farmer firer,
        GameLocation? location,
        float damage,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotationVelocity,
        bool spriteFromObjectSheet)
        : base(
            (int)damage,
            index,
            0,
            index == Constants.QuincyProjectileIndex ? 5 : 0,
            rotationVelocity,
            xVelocity,
            yVelocity,
            startingPosition,
            index == Constants.ExplosiveAmmoIndex ? "explosion" : "hammer",
            index == Constants.QuincyProjectileIndex ? "debuffSpell" : string.Empty,
            false,
            true,
            location,
            firer,
            spriteFromObjectSheet,
            index == Constants.ExplosiveAmmoIndex ? explodeOnImpact : null)
    {
        this.Ammo = ammo;
        this.Source = source;
        this.Firer = this.theOneWhoFiredMe.Get(location) as Farmer ?? Game1.player;
        this.Damage = (int)(this.damageToFarmer.Value *
                            (1f + (source.GetEnchantmentLevel<RubyEnchantment>() * 0.1f) +
                             source.Read<float>(DataFields.ResonantSlingshotDamage)) *
                            (1f + firer.attackIncreaseModifier));
        this.Knockback = spriteFromObjectSheet
            ? 0f
            : (0.75f + (source.GetEnchantmentLevel<AmethystEnchantment>() * 0.1f) +
               source.Read<float>(DataFields.ResonantSlingshotKnockback)) * (1f + firer.knockbackModifier);

        var canCrit = ModEntry.Config.Arsenal.Slingshots.AllowCrits && !spriteFromObjectSheet;
        this.CritChance = canCrit
            ? ((1f / 32f) + (source.GetEnchantmentLevel<AquamarineEnchantment>() * 0.046f) +
               source.Read<float>(DataFields.ResonantSlingshotCritChance)) * (1f + firer.critChanceModifier)
            : 0f;
        this.CritPower = canCrit
            ? (1f + ((ModEntry.Config.EnableArsenal && ModEntry.Config.Arsenal.RebalancedForges ? 0.5f : 0.1f) *
                     source.GetEnchantmentLevel<JadeEnchantment>()) +
               source.Read<float>(DataFields.ResonantSlingshotCritPower)) * (1f + firer.critPowerModifier)
            : 0f;

        if (ammo is null)
        {
            ModEntry.Reflector
                .GetUnboundFieldGetter<BasicProjectile, NetString>(this, "collisionSound")
                .Invoke(this).Value = index == Constants.QuincyProjectileIndex ? "debuffHit" : "snowyStep";
        }
        else if (this.IsSquishy())
        {
            ModEntry.Reflector
                .GetUnboundFieldGetter<BasicProjectile, NetString>(this, "collisionSound")
                .Invoke(this).Value = "slimedead";
        }

        if (ModEntry.Config.Arsenal.Slingshots.DisableGracePeriod)
        {
            this.ignoreTravelGracePeriod.Value = true;
        }
    }

    public Item? Ammo { get; }

    public Farmer Firer { get; }

    public Slingshot Source { get; }

    public int Damage { get; }

    public float Knockback { get; }

    public float CritChance { get; }

    public float CritPower { get; }

    public override void behaviorOnCollisionWithMonster(NPC n, GameLocation location)
    {
        if (!this.damagesMonsters.Value)
        {
            return;
        }

        if (n is not Monster { IsMonster: true } monster)
        {
            base.behaviorOnCollisionWithMonster(n, location);
            return;
        }

        ModEntry.Reflector
            .GetUnboundMethodDelegate<Action<BasicProjectile, GameLocation>>(this, "explosionAnimation")
            .Invoke(this, location);
        location.damageMonster(
            monster.GetBoundingBox(),
            this.Damage,
            this.Damage + 1,
            false,
            this.Knockback,
            0,
            this.CritChance,
            this.CritPower,
            true,
            this.Firer);
    }

    /// <summary>Determines whether the projectile should pierce and bounce or make squishy noises upon collision.</summary>
    /// <returns><see langword="true"/> if the projectile is an egg, fruit, vegetable or slime, otherwise <see langword="false"/>.</returns>
    public bool IsSquishy()
    {
        return this.Ammo?.Category is SObject.EggCategory or SObject.FruitsCategory or SObject.VegetableCategory ||
               this.Ammo?.ParentSheetIndex == Constants.SlimeIndex;
    }
}
