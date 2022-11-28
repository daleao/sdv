namespace DaLion.Ligo.Modules.Arsenal.Extensions;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="MeleeWeapon"/> class.</summary>
internal static class MeleeWeaponExtensions
{
    /// <summary>Determines whether the <paramref name="weapon"/> is an Infinity weapon.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/>'s index correspond to one of the Infinity weapon, otherwise <see langword="false"/>.</returns>
    internal static bool IsInfinityWeapon(this MeleeWeapon weapon)
    {
        return weapon.InitialParentTileIndex is Constants.InfinityBladeIndex or Constants.InfinityDaggerIndex
            or Constants.InfinityGavelIndex;
    }

    /// <summary>Determines whether the <paramref name="weapon"/> is unique.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/> is a Galaxy, Infinity or other unique weapon, otherwise <see langword="false"/>.</returns>
    internal static bool IsUnique(this MeleeWeapon weapon)
    {
        return weapon.isGalaxyWeapon() || weapon.IsInfinityWeapon() ||
               Collections.UniqueWeapons.Contains(weapon.InitialParentTileIndex);
    }

    /// <summary>Determines whether the <paramref name="weapon"/> is unique.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/> is a Galaxy, Infinity or other unique weapon, otherwise <see langword="false"/>.</returns>
    internal static bool CanBeCrafted(this MeleeWeapon weapon)
    {
        return ModEntry.Config.Arsenal.AncientCrafting &&
               (weapon.Name.StartsWith("Dwarven") || weapon.Name.StartsWith("Dragontooth"));
    }

    /// <summary>Gets the default crit. chance for this weapon type.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns>The default crit. chance for the weapon type.</returns>
    internal static float DefaultCritChance(this MeleeWeapon weapon)
    {
        return weapon.type.Value switch
        {
            MeleeWeapon.defenseSword or MeleeWeapon.stabbingSword => 1f / 16f,
            MeleeWeapon.dagger => 1f / 8f,
            MeleeWeapon.club => 1f / 32f,
            _ => 0f,
        };
    }

    /// <summary>Gets the default crit. power for this weapon type.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns>The default crit. power for the weapon type.</returns>
    internal static float DefaultCritPower(this MeleeWeapon weapon)
    {
        return weapon.type.Value switch
        {
            MeleeWeapon.defenseSword or MeleeWeapon.stabbingSword => 2f,
            MeleeWeapon.dagger => 1.5f,
            MeleeWeapon.club => 3f,
            _ => 0f,
        };
    }

    /// <summary>Gets the maximum number of hits in a combo for this <paramref name="weapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns>The final <see cref="ComboHitStep"/> for <paramref name="weapon"/>.</returns>
    internal static ComboHitStep GetFinalHitStep(this MeleeWeapon weapon)
    {
        return weapon.type.Value switch
        {
            MeleeWeapon.stabbingSword => (ComboHitStep)ModEntry.Config.Arsenal.Weapons.ComboHitsPerWeapon[WeaponType.StabbingSword],
            MeleeWeapon.club => (ComboHitStep)ModEntry.Config.Arsenal.Weapons.ComboHitsPerWeapon[WeaponType.Club],
            MeleeWeapon.dagger => ComboHitStep.FirstHit,
            MeleeWeapon.defenseSword => (ComboHitStep)ModEntry.Config.Arsenal.Weapons.ComboHitsPerWeapon[WeaponType.DefenseSword],
            _ => 0,
        };
    }

    /// <summary>Randomizes the damage of the <paramref name="weapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Preference for local functions.")]
    [SuppressMessage("ReSharper", "VariableHidesOuterVariable", Justification = "Local function.")]
    internal static void RandomizeDamage(this MeleeWeapon weapon)
    {
        var player = Game1.player;
        var level = player.currentLocation is MineShaft shaft
            ? shaft.mineLevel == 77377
                ? 40
                : shaft.mineLevel
            : player.deepestMineLevel;
        var dangerous = (Game1.netWorldState.Value.MinesDifficulty > 0) |
                        (Game1.netWorldState.Value.SkullCavesDifficulty > 0);
        var baseDamage = getBaseDamage(level, dangerous);
        var randomDamage = getRandomDamage(baseDamage);

        double minDamage = 1;
        double maxDamage = 3;
        switch (weapon.type.Value)
        {
            case MeleeWeapon.stabbingSword:
            case MeleeWeapon.defaultSpeed:
                maxDamage = randomDamage;
                minDamage = 0.75 * maxDamage;
                break;
            case MeleeWeapon.dagger:
                maxDamage = 2d / 3d * randomDamage;
                minDamage = 0.85 * maxDamage;
                break;
            case MeleeWeapon.club:
                maxDamage = 5d / 3d * randomDamage;
                minDamage = 1d / 3d * maxDamage;
                break;
        }

        weapon.minDamage.Value = (int)Math.Max(minDamage, 1);
        weapon.maxDamage.Value = (int)Math.Max(maxDamage, 3);

        int getBaseDamage(int level, bool dangerous)
        {
            return dangerous
                ? (60 * (level / (level + 100))) + 80
                : 120 * level / (level + 100);
        }

        double getRandomDamage(int baseDamage)
        {
            var exp = Math.Exp(Game1.random.NextGaussian(stddev: 2d) / 2d);
            return exp / (1d + exp) * baseDamage;
        }
    }

    /// <summary>Refreshes the stats of the specified <paramref name="weapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    internal static void RefreshStats(this MeleeWeapon weapon)
    {
        var data = ModEntry.ModHelper.GameContent.Load<Dictionary<int, string>>("Data/weapons");
        if (!data.ContainsKey(weapon.InitialParentTileIndex))
        {
            return;
        }

        var split = data[weapon.InitialParentTileIndex].Split('/');

        weapon.knockback.Value = (float)Convert.ToDouble(split[4], CultureInfo.InvariantCulture);
        weapon.speed.Value = Convert.ToInt32(split[5]);
        weapon.addedPrecision.Value = Convert.ToInt32(split[6]);
        weapon.addedDefense.Value = Convert.ToInt32(split[7]);
        weapon.type.Set(Convert.ToInt32(split[8]));
        weapon.addedAreaOfEffect.Value = Convert.ToInt32(split[11]);
        weapon.critChance.Value = (float)Convert.ToDouble(split[12], CultureInfo.InvariantCulture);
        weapon.critMultiplier.Value = (float)Convert.ToDouble(split[13], CultureInfo.InvariantCulture);

        var initialMinDamage = weapon.Read(DataFields.InitialMinDamage, -1);
        var initialMaxDamage = weapon.Read(DataFields.InitialMaxDamage, -1);
        if (initialMinDamage >= 0 && initialMaxDamage >= 0)
        {
            weapon.minDamage.Value = initialMinDamage;
            weapon.maxDamage.Value = initialMaxDamage;
        }
        else if (!weapon.IsUnique() && !weapon.CanBeCrafted() && ModEntry.Config.Arsenal.Weapons.RebalancedWeapons)
        {
            weapon.RandomizeDamage();
        }
        else
        {
            weapon.minDamage.Value = Convert.ToInt32(split[2]);
            weapon.maxDamage.Value = Convert.ToInt32(split[3]);
        }

        weapon.Invalidate();
    }

    /// <summary>Adds hidden weapon enchantments related to Infinity +1.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    internal static void AddIntrinsicEnchantments(this MeleeWeapon weapon)
    {
        switch (weapon.InitialParentTileIndex)
        {
            case Constants.DarkSwordIndex when !weapon.hasEnchantmentOfType<CursedEnchantment>():
                weapon.enchantments.Add(new CursedEnchantment());
                weapon.WriteIfNotExists(DataFields.CursePoints, "500");
                break;
            case Constants.HolyBladeIndex when !weapon.hasEnchantmentOfType<BlessedEnchantment>():
                weapon.enchantments.Add(new BlessedEnchantment());
                break;
            case Constants.InfinityBladeIndex:
            case Constants.InfinityDaggerIndex:
            case Constants.InfinityGavelIndex:
                if (!weapon.hasEnchantmentOfType<InfinityEnchantment>())
                {
                    weapon.enchantments.Add(new InfinityEnchantment());
                }

                break;
        }
    }

    internal static void SetFarmerAnimatingBackwards(this MeleeWeapon weapon, Farmer farmer)
    {
        ModEntry.Reflector
            .GetUnboundFieldSetter<MeleeWeapon, bool>(weapon, "anotherClick")
            .Invoke(weapon, false);
        farmer.FarmerSprite.PauseForSingleAnimation = false;
        farmer.FarmerSprite.StopAnimation();

        ModEntry.Reflector
            .GetUnboundFieldSetter<MeleeWeapon, bool>(weapon, "hasBegunWeaponEndPause")
            .Invoke(weapon, false);
        float swipeSpeed = 400 - (weapon.speed.Value * 40);
        swipeSpeed *= farmer.GetTotalSwingSpeedModifier();
        if (farmer.IsLocalPlayer)
        {
            foreach (var enchantment in weapon.enchantments)
            {
                if (enchantment is BaseWeaponEnchantment weaponEnchantment)
                {
                    weaponEnchantment.OnSwing(weapon, farmer);
                }
            }
        }

        weapon.DoBackwardSwipe(farmer.Position, farmer.FacingDirection, swipeSpeed / (weapon.type.Value == 2 ? 5 : 8), farmer);
        farmer.lastClick = Vector2.Zero;
        var actionTile = farmer.GetToolLocation(ignoreClick: true);
        weapon.DoDamage(farmer.currentLocation, (int)actionTile.X, (int)actionTile.Y, farmer.FacingDirection, 1, farmer);
        if (farmer.CurrentTool is not null)
        {
            return;
        }

        farmer.completelyStopAnimatingOrDoingAction();
        farmer.forceCanMove();
    }

    internal static void DoBackwardSwipe(this MeleeWeapon weapon, Vector2 position, int facingDirection, float swipeSpeed, Farmer? farmer)
    {
        if (farmer?.CurrentTool != weapon)
        {
            return;
        }

        if (farmer.IsLocalPlayer)
        {
            farmer.TemporaryPassableTiles.Clear();
            farmer.currentLocation.lastTouchActionLocation = Vector2.Zero;
        }

        swipeSpeed *= 1.3f;
        var sprite = farmer.FarmerSprite;
        switch (farmer.FacingDirection)
        {
            case 0:
                sprite.animateOnce(248, swipeSpeed, 6);
                weapon.Update(0, 0, farmer);
                break;
            case 1:
                sprite.animateOnce(240, swipeSpeed, 6);
                weapon.Update(1, 0, farmer);
                break;
            case 2:
                sprite.animateOnce(232, swipeSpeed, 6);
                weapon.Update(2, 0, farmer);
                break;
            case 3:
                sprite.animateOnce(256, swipeSpeed, 6);
                weapon.Update(3, 0, farmer);
                break;
        }
    }
}
