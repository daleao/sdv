namespace DaLion.Redux.Arsenal.Slingshots;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>A Slingshot <see cref="BasicProjectile"/> with extra useful properties.</summary>
internal sealed class ReduxProjectile : BasicProjectile
{
    public ReduxProjectile(
        Slingshot source,
        Farmer firer,
        GameLocation? location,
        int index,
        float damage,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotationVelocity,
        string collisionSound,
        onCollisionBehavior? collisionBehavior,
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
            collisionSound,
            index == Constants.QuincyProjectileIndex ? "debuffSpell" : string.Empty,
            false,
            true,
            location,
            firer,
            spriteFromObjectSheet,
            collisionBehavior)
    {
        this.Source = source;
        this.Firer = this.theOneWhoFiredMe.Get(location) as Farmer ?? Game1.player;
        this.Damage = (int)(this.damageToFarmer.Value * (1f + source.GetEnchantmentLevel<RubyEnchantment>()) *
                            (1f + firer.attackIncreaseModifier));

        this.Knockback = spriteFromObjectSheet
            ? 0f
            : (1f + source.GetEnchantmentLevel<AmethystEnchantment>()) * (1f + firer.knockbackModifier);

        var canCrit = ModEntry.Config.Arsenal.Slingshots.AllowCrits && !spriteFromObjectSheet;
        this.CritChance = canCrit
            ? ((1f / 32f) + (source.GetEnchantmentLevel<AquamarineEnchantment>() * 0.046f)) * (1f + firer.critChanceModifier)
            : 0f;
        this.CritPower = canCrit
            ? (1f + ((ModEntry.Config.EnableArsenal && ModEntry.Config.Arsenal.RebalancedForges ? 0.5f : 0.1f) *
                     source.GetEnchantmentLevel<JadeEnchantment>())) * (1f + firer.critPowerModifier)
            : 0f;

        if (!spriteFromObjectSheet)
        {
            if (index == Constants.QuincyProjectileIndex)
            {
                this.IsQuincyProjectile = true;
            }
            else
            {
                this.IsSnowballProjectile = true;
            }
        }

        if (ModEntry.Config.Arsenal.Slingshots.DisableGracePeriod)
        {
            this.ignoreTravelGracePeriod.Value = true;
        }
    }

    public Farmer Firer { get; }

    public Slingshot Source { get; }

    public int Damage { get; }

    public float Knockback { get; }

    public float CritChance { get; }

    public float CritPower { get; }

    public bool IsQuincyProjectile { get; }

    public bool IsSnowballProjectile { get; }

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
}
