#pragma warning disable CS1591
namespace DaLion.Shared.Integrations.Archery;

#region using directives

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives
public interface IArcheryApi
{
    event EventHandler<IWeaponFiredEventArgs> OnWeaponFired;

    event EventHandler<IWeaponChargeEventArgs> OnWeaponCharging;

    event EventHandler<IWeaponChargeEventArgs> OnWeaponCharged;

    event EventHandler<ICrossbowLoadedEventArgs> OnCrossbowLoaded;

    event EventHandler<IAmmoChangedEventArgs> OnAmmoChanged;

    event EventHandler<IAmmoHitMonsterEventArgs> OnAmmoHitMonster;

    Item CreateWeapon(IManifest callerManifest, string weaponModelId);

    Item CreateAmmo(IManifest callerManifest, string ammoModelId);

    bool PlaySound(IManifest callerManifest, ISound sound, Vector2 position);

    int? GetSpecialAttackCooldown(IManifest callerManifest, Slingshot slingshot);

    IWeaponData? GetWeaponData(IManifest callerManifest, Slingshot slingshot);

    IProjectileData? GetProjectileData(IManifest callerManifest, BasicProjectile projectile);

    bool SetProjectileData(IManifest callerManifest, BasicProjectile projectile, IProjectileData data);

    bool SetChargePercentage(IManifest callerManifest, Slingshot slingshot, float percentage);

    BasicProjectile PerformFire(IManifest callerManifest, BasicProjectile projectile, Slingshot slingshot, GameLocation location, Farmer who, bool suppressFiringSound = false);

    BasicProjectile PerformFire(IManifest callerManifest, string ammoId, Slingshot slingshot, GameLocation location, Farmer who, bool suppressFiringSound = false);

    BasicProjectile PerformFire(IManifest callerManifest, Slingshot slingshot, GameLocation location, Farmer who, bool suppressFiringSound = false);

    bool RegisterSpecialAttack(IManifest callerManifest, string name, WeaponType whichWeaponTypeCanUse, Func<List<object>, string> getDisplayName, Func<List<object>, string> getDescription, Func<List<object>, int> getCooldownMilliseconds, Func<ISpecialAttack, bool> specialAttackHandler);

    bool DeregisterSpecialAttack(IManifest callerManifest, string name);

    bool RegisterEnchantment(IManifest callerManifest, string name, AmmoType whichAmmoTypeCanUse, TriggerType triggerType, Func<List<object>, string> getDisplayName, Func<List<object>, string> getDescription, Func<IEnchantment, bool> enchantmentHandler);

    bool DeregisterEnchantment(IManifest callerManifest, string name);
}

#region Enums

#pragma warning disable SA1602 // Enumeration items should be documented

public enum WeaponType
{
    Any,
    [Obsolete("Not currently used")]
    Slingshot,
    Bow,
    Crossbow,
}

public enum AmmoType
{
    Any,
    [Obsolete("Not currently used")]
    Pellet,
    Arrow,
    Bolt,
}

public enum TriggerType
{
    Unknown,
    OnFire,
    OnImpact,
}

#pragma warning restore SA1602 // Enumeration items should be documented

#endregion Enums

#region Interface objects

public interface ISpecialAttack
{
    public Slingshot Slingshot { get; init; }

    public GameTime Time { get; init; }

    public GameLocation Location { get; init; }

    public Farmer Farmer { get; init; }

    public List<object> Arguments { get; init; }
}

public interface IEnchantment
{
    public BasicProjectile Projectile { get; init; }

    public GameTime Time { get; init; }

    public GameLocation Location { get; init; }

    public Farmer Farmer { get; init; }

    public Monster? Monster { get; init; }

    public int? DamageDone { get; init; }

    public List<object> Arguments { get; init; }
}

public interface IProjectileData
{
    public string AmmoId { get; init; }

    public Vector2? Position { get; set; }

    public Vector2? Velocity { get; set; }

    public float? InitialSpeed { get; init; }

    public float? Rotation { get; set; }

    public int? BaseDamage { get; set; }

    public float? BreakChance { get; set; }

    public float? CriticalChance { get; set; }

    public float? CriticalDamageMultiplier { get; set; }

    public float? Knockback { get; set; }

    public bool? DoesExplodeOnImpact { get; set; }

    public int? ExplosionRadius { get; set; }

    public int? ExplosionDamage { get; set; }
}

public interface IWeaponData
{
    public string WeaponId { get; init; }

    public WeaponType WeaponType { get; init; }

    public int? MagazineSize { get; init; }

    public int? AmmoInMagazine { get; set; }

    public float ChargeTimeRequiredMilliseconds { get; init; }

    public float ProjectileSpeed { get; init; }

    public IRandomRange DamageRange { get; init; }
}

public interface ISound
{
    public string Name { get; set; }

    public int Pitch { get; set; }

    public IRandomRange PitchRandomness { get; set; }

    public float Volume { get; set; }

    public float MaxDistance { get; set; }
}

public interface IRandomRange
{
    public int Min { get; set; }

    public int Max { get; set; }

    public int Get(Random random, int minOffset = 0, int maxOffset = 0);
}

#endregion Interface objects

#region Events

public interface IBaseEventArgs
{
    public Vector2 Origin { get; init; }
}

public interface IWeaponFiredEventArgs : IBaseEventArgs
{
    public string WeaponId { get; init; }

    public string AmmoId { get; init; }

    public BasicProjectile Projectile { get; init; }
}

public interface IWeaponChargeEventArgs : IBaseEventArgs
{
    public string WeaponId { get; init; }

    public float ChargePercentage { get; init; }
}

public interface ICrossbowLoadedEventArgs : IBaseEventArgs
{
    public string WeaponId { get; init; }

    public string AmmoId { get; init; }
}

public interface IAmmoChangedEventArgs : IBaseEventArgs
{
    public string WeaponId { get; init; }

    public string AmmoId { get; init; }
}

public interface IAmmoHitMonsterEventArgs : IWeaponFiredEventArgs
{
    public Monster Monster { get; init; }

    public int DamageDone { get; init; }
}

#endregion Events

#pragma warning restore CS1591
