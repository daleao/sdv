namespace DaLion.Stardew.Slingshots.Framework;

#region using directives

using DaLion.Common.Extensions.Reflection;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>A Slingshot <see cref="BasicProjectile"/> that remembers where it came from.</summary>
internal sealed class ImmersiveProjectile : BasicProjectile
{
    private static readonly Lazy<Action<BasicProjectile, GameLocation>> ExplosionAnimation = new(() =>
        typeof(BasicProjectile)
            .RequireMethod("explosionAnimation")
            .CompileUnboundDelegate<Action<BasicProjectile, GameLocation>>());

    public ImmersiveProjectile(
        Slingshot source,
        Farmer firer,
        float damage,
        int parentSheetIndex,
        int tailLength,
        float rotationVelocity,
        float xVelocity,
        float yVelocity,
        Vector2 startingPosition,
        string collisionSound,
        string firingSound,
        GameLocation? location = null,
        bool spriteFromObjectSheet = false,
        onCollisionBehavior? collisionBehavior = null)
        : base(
            (int)damage,
            parentSheetIndex,
            0,
            tailLength,
            rotationVelocity,
            xVelocity,
            yVelocity,
            startingPosition,
            collisionSound,
            firingSound,
            false,
            true,
            location,
            firer,
            spriteFromObjectSheet,
            collisionBehavior)
    {
        this.ParentSheetIndex = parentSheetIndex;
        this.Source = source;

        this.Firer = this.theOneWhoFiredMe.Get(location) as Farmer ?? Game1.player;
        this.Damage = (int)(this.damageToFarmer.Value * (1f + source.GetEnchantmentLevel<RubyEnchantment>()) *
                           (1f + firer.attackIncreaseModifier));
        this.Knockback = this.IsQuincy
            ? 0f
            : (1f + source.GetEnchantmentLevel<AmethystEnchantment>()) * (1f + firer.knockbackModifier);
        this.CritChance = !this.IsQuincy && ModEntry.Config.AllowCrits
            ? (0.05f + (0.046f * source.GetEnchantmentLevel<AquamarineEnchantment>())) *
              (1f + firer.critChanceModifier)
            : 0;
        this.CritPower =
            (1f + ((ModEntry.ArsenalConfig?.Value<bool?>("OverhauledEnchants") == true ? 0.5f : 0.1f) *
                   source.GetEnchantmentLevel<JadeEnchantment>())) * (1f + firer.critPowerModifier);

        switch (spriteFromObjectSheet)
        {
            case true when ModEntry.Config.DisableGracePeriod:
                this.ignoreTravelGracePeriod.Value = true;
                break;
            case false:
                switch (parentSheetIndex)
                {
                    case Constants.QuincyProjectileIndex:
                        this.IsQuincy = true;
                        break;
                    case Constants.SnowballProjectileIndex:
                        this.IsSnowball = true;
                        break;
                }

                break;
        }
    }

    public int ParentSheetIndex { get; }

    public Farmer Firer { get; }

    public Slingshot Source { get; }

    public int Damage { get; }

    public float Knockback { get; }

    public float CritChance { get; }

    public float CritPower { get; }

    public bool IsQuincy { get; }

    public bool IsSnowball { get; }

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

        ExplosionAnimation.Value(this, location);
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
}
