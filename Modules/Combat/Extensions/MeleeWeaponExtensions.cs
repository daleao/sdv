namespace DaLion.Overhaul.Modules.Combat.Extensions;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared;
using DaLion.Shared.Constants;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
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
        return weapon.InitialParentTileIndex is WeaponIds.InfinityBlade or WeaponIds.InfinityDagger
            or WeaponIds.InfinityGavel;
    }

    /// <summary>Determines whether the <paramref name="weapon"/> is an Infinity weapon.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/>'s index corresponds to either Dark Sword or Holy Blade, otherwise <see langword="false"/>.</returns>
    internal static bool IsCursedOrBlessed(this MeleeWeapon weapon)
    {
        return weapon.InitialParentTileIndex is WeaponIds.DarkSword or WeaponIds.HolyBlade;
    }

    /// <summary>Determines whether the <paramref name="weapon"/> is unique.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/> is a Galaxy, Infinity or other unique weapon, otherwise <see langword="false"/>.</returns>
    internal static bool IsUnique(this MeleeWeapon weapon)
    {
        return weapon.isGalaxyWeapon() || weapon.IsInfinityWeapon() || weapon.IsCursedOrBlessed() || weapon.specialItem;
    }

    /// <summary>Determines whether the <paramref name="weapon"/> is a mythic relic weapon.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/> is a Galaxy, Infinity or other unique weapon, otherwise <see langword="false"/>.</returns>
    internal static bool IsRelic(this MeleeWeapon weapon)
    {
        return weapon.isGalaxyWeapon() || weapon.IsInfinityWeapon() || weapon.IsCursedOrBlessed() || weapon.specialItem;
    }

    /// <summary>Determines whether the <paramref name="weapon"/> is a legacy Dwarven weapon.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/> if DwarvenLegacy option is enabled and the weapon is a Dwarven, Dragontooth or Elven weapon, otherwise <see langword="false"/>.</returns>
    internal static bool IsLegacyWeapon(this MeleeWeapon weapon)
    {
        if (!CombatModule.Config.DwarvenLegacy)
        {
            return false;
        }

        return weapon.InitialParentTileIndex.IsAnyOf(
            WeaponIds.DwarfDagger,
            WeaponIds.DwarfHammer,
            WeaponIds.DwarfSword,
            WeaponIds.DragontoothClub,
            WeaponIds.DragontoothCutlass,
            WeaponIds.DragontoothShiv,
            WeaponIds.ElfBlade,
            WeaponIds.ForestSword);
    }

    /// <summary>Determines whether the <paramref name="weapon"/> should be converted to stabbing sword.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/> is StabbySwords option is enabled and the weapon should be a stabbing sword, otherwise <see langword="false"/>.</returns>
    internal static bool ShouldBeStabbySword(this MeleeWeapon weapon)
    {
        return CombatModule.Config.EnableWeaponOverhaul &&
               CombatModule.Config.EnableStabbingSwords &&
               (CombatModule.Config.StabbingSwords.Contains(weapon.Name) ||
                (weapon.InitialParentTileIndex is WeaponIds.GalaxySword or WeaponIds.InfinityBlade &&
                 weapon.Read(DataKeys.SwordType, 3) == 0));
    }

    /// <summary>Gets the default crit. chance for this weapon type.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns>The default crit. chance for the weapon type.</returns>
    internal static float DefaultCritChance(this MeleeWeapon weapon)
    {
        return weapon.Name == "Diamond Wand" ? 1f : weapon.type.Value switch
        {
            MeleeWeapon.defenseSword or MeleeWeapon.stabbingSword => 0.05f,
            MeleeWeapon.dagger => 0.1f,
            MeleeWeapon.club => 0.025f,
            _ => 0f,
        };
    }

    /// <summary>Gets the default crit. power for this weapon type.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns>The default crit. power for the weapon type.</returns>
    internal static float DefaultCritPower(this MeleeWeapon weapon)
    {
        return weapon.Name == "Diamond Wand" ? 1f : weapon.type.Value switch
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
        return ((WeaponType)weapon.type.Value).GetFinalHitStep();
    }

    /// <summary>Refreshes the stats of the specified <paramref name="weapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <param name="option">The <see cref="WeaponRefreshOption"/>.</param>
    /// <returns>The modified <paramref name="weapon"/>, for use by transpilers.</returns>
    internal static MeleeWeapon RefreshStats(this MeleeWeapon weapon, WeaponRefreshOption option = WeaponRefreshOption.Initial)
    {
        var data = ModHelper.GameContent.Load<Dictionary<int, string>>("Data/weapons");
        if (!data.ContainsKey(weapon.InitialParentTileIndex))
        {
            return weapon;
        }

        var split = data[weapon.InitialParentTileIndex].SplitWithoutAllocation('/');
        weapon.BaseName = split[0].ToString();
        weapon.knockback.Value = float.Parse(split[4]);
        weapon.speed.Value = int.Parse(split[5]);
        weapon.addedPrecision.Value = int.Parse(split[6]);
        weapon.addedDefense.Value = int.Parse(split[7]);
        weapon.type.Set(int.Parse(split[8]));
        weapon.addedAreaOfEffect.Value = int.Parse(split[11]);
        weapon.critChance.Value = float.Parse(split[12]);
        weapon.critMultiplier.Value = float.Parse(split[13]);
        switch (option)
        {
            case WeaponRefreshOption.Randomized when CombatModule.ShouldEnable && weapon.ShouldRandomizeDamage():
                weapon.RandomizeDamage();
                weapon.Invalidate();
                break;

            case WeaponRefreshOption.Initial when CombatModule.ShouldEnable && weapon.ShouldRandomizeDamage():
                var initialMinDamage = weapon.Read(DataKeys.BaseMinDamage, -1);
                var initialMaxDamage = weapon.Read(DataKeys.BaseMaxDamage, -1);
                if (initialMinDamage >= 0 && initialMaxDamage >= 0)
                {
                    weapon.minDamage.Value = initialMinDamage;
                    weapon.maxDamage.Value = initialMaxDamage;
                    weapon.Invalidate();
                    break;
                }

                weapon.RandomizeDamage();
                weapon.Invalidate();
                break;

            default:
                weapon.minDamage.Value = int.Parse(split[2]);
                weapon.maxDamage.Value = int.Parse(split[3]);
                weapon.Write(DataKeys.BaseMinDamage, null);
                weapon.Write(DataKeys.BaseMaxDamage, null);
                if (weapon.isScythe())
                {
                    weapon.type.Value = 3;
                }

                weapon.Invalidate();
                break;
        }

        weapon.enchantments.Where(e => e.IsForge()).ForEach(e => e.ApplyTo(weapon));
        return weapon;
    }

    /// <summary>Randomizes the damage of the <paramref name="weapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <param name="bias">An optional bias to influence the range of allowed damage values (positive values mean higher stats on average).</param>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Preference for local functions.")]
    [SuppressMessage("ReSharper", "VariableHidesOuterVariable", Justification = "Local function.")]
    internal static void RandomizeDamage(this MeleeWeapon weapon, double bias = 0d)
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
        var mean = ((double)WeaponTier.GetFor(weapon) + bias + (player.DailyLuck * 10d) - 4d) * 2d;
        var randomizer = (0.5 * MathUtils.Sigmoid(Game1.random.NextGaussian(mean, stddev: 2d) / 2d)) - 0.25; // (-0.25, 0.25)
        var randomDamage = baseDamage * (1d + randomizer);

        var minDamage = 1d;
        var maxDamage = 3d;
        switch (weapon.type.Value)
        {
            case MeleeWeapon.stabbingSword:
            case MeleeWeapon.defenseSword:
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
        weapon.Write(DataKeys.BaseMinDamage, weapon.minDamage.Value.ToString());
        weapon.Write(DataKeys.BaseMaxDamage, weapon.maxDamage.Value.ToString());

        int getBaseDamage(int level, bool dangerous)
        {
            return dangerous
                ? (int)(60f * (level / (level + 100f))) + 80
                : (int)(120f * level / (level + 100f));
        }
    }

    /// <summary>Checks whether the <paramref name="weapon"/>'s damage should be randomized.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/> is a Galaxy, Infinity or other unique weapon, otherwise <see langword="false"/>.</returns>
    internal static bool ShouldRandomizeDamage(this MeleeWeapon weapon)
    {
        return CombatModule.Config.EnableWeaponOverhaul && WeaponTier.GetFor(weapon) is var tier &&
               tier > WeaponTier.Untiered && tier <= WeaponTier.Mythic && !weapon.specialItem;
    }

    /// <summary>Adds hidden weapon enchantments related to Rebalance or Infinity +1.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    internal static void AddIntrinsicEnchantments(this MeleeWeapon weapon)
    {
        if (CombatModule.Config.EnableWeaponOverhaul)
        {
            if (ModHelper.ModRegistry.IsLoaded("JA.FishHatchering") && weapon.Name == "Sword Fish" &&
                !weapon.hasEnchantmentOfType<SwordFishEnchantment>())
            {
                weapon.AddEnchantment(new SwordFishEnchantment());
                return;
            }

            if (ModHelper.ModRegistry.IsLoaded("undare.crystalcrops.HCE") && weapon.Name.IsAnyOf(
                    "Blueglazer", "Crystallight", "Grapemaul", "Heartichoker", "Strawblaster", "Sunspark"))
            {
                weapon.AddEnchantment(new UndareCrystallizedWeaponEnchantment());
                return;
            }

            switch (weapon.InitialParentTileIndex)
            {
                case WeaponIds.InsectHead when !weapon.hasEnchantmentOfType<KillerBugEnchantment>():
                    weapon.AddEnchantment(new KillerBugEnchantment());
                    Log.D("[CMBT]: Added KillerBugEnchantment to Insect Head.");
                    break;
                case WeaponIds.IridiumNeedle when !weapon.hasEnchantmentOfType<NeedleEnchantment>():
                    weapon.AddEnchantment(new NeedleEnchantment());
                    Log.D("[CMBT]: Added NeedleEnchantment to Iridium Needle.");
                    break;
                case WeaponIds.LavaKatana when !weapon.hasEnchantmentOfType<LavaEnchantment>():
                    weapon.AddEnchantment(new LavaEnchantment());
                    Log.D("[CMBT]: Added LavaEnchantment to Lava Katana.");
                    break;
                case WeaponIds.NeptuneGlaive when !weapon.hasEnchantmentOfType<NeptuneEnchantment>():
                    weapon.AddEnchantment(new NeptuneEnchantment());
                    Log.D("[CMBT]: Added NeptuneEnchantment to Neptune Glaive.");
                    break;
                case WeaponIds.ObsidianEdge when !weapon.hasEnchantmentOfType<ObsidianEnchantment>():
                    weapon.AddEnchantment(new ObsidianEnchantment());
                    Log.D("[CMBT]: Added ObsidianEnchantment to Obsidian Edge.");
                    break;
                case WeaponIds.YetiTooth when !weapon.hasEnchantmentOfType<YetiEnchantment>():
                    weapon.AddEnchantment(new YetiEnchantment());
                    Log.D("[CMBT]: Added YetiEnchantment to Yeti Tooth.");
                    break;
            }

            if (weapon.IsDagger() && !weapon.hasEnchantmentOfType<KillerBugEnchantment>())
            {
                weapon.AddEnchantment(new DaggerEnchantment());
            }
        }

        if (CombatModule.Config.EnableHeroQuest)
        {
            switch (weapon.InitialParentTileIndex)
            {
                case WeaponIds.DarkSword when !weapon.hasEnchantmentOfType<CursedEnchantment>():
                    weapon.AddEnchantment(new CursedEnchantment());
                    Log.D("[CMBT]: Added CursedEnchantment to Dark Sword.");
                    break;
                case WeaponIds.HolyBlade when !weapon.hasEnchantmentOfType<BlessedEnchantment>():
                    weapon.AddEnchantment(new BlessedEnchantment());
                    Log.D("[CMBT]: Added BlessedEnchantment to Holy Blade.");
                    break;
                default:
                    if (weapon.IsInfinityWeapon() && !weapon.hasEnchantmentOfType<InfinityEnchantment>())
                    {
                        weapon.AddEnchantment(new InfinityEnchantment());
                        Log.D($"[CMBT]: Added InfinityEnchantment to Infinity {(WeaponType)weapon.type.Value}.");
                    }

                    break;
            }
        }
    }

    /// <summary>Removes hidden weapon enchantments related Rebalance or Infinity +1.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    internal static void RemoveIntrinsicEnchantments(this MeleeWeapon weapon)
    {
        BaseEnchantment? enchantment;
        if (weapon.IsDagger())
        {
            enchantment = weapon.GetEnchantmentOfType<DaggerEnchantment>();
            if (enchantment is not null)
            {
                weapon.RemoveEnchantment(enchantment);
                Log.D($"[CMBT]: Removed Dagger enchantment from {weapon.Name}.");
            }
        }

        switch (weapon.InitialParentTileIndex)
        {
            case WeaponIds.DarkSword:
                enchantment = weapon.GetEnchantmentOfType<CursedEnchantment>();
                break;
            case WeaponIds.HolyBlade:
                enchantment = weapon.GetEnchantmentOfType<BlessedEnchantment>();
                break;
            case WeaponIds.InsectHead:
                enchantment = weapon.GetEnchantmentOfType<KillerBugEnchantment>();
                break;
            case WeaponIds.IridiumNeedle:
                enchantment = weapon.GetEnchantmentOfType<NeedleEnchantment>();
                break;
            case WeaponIds.LavaKatana:
                enchantment = weapon.GetEnchantmentOfType<LavaEnchantment>();
                break;
            case WeaponIds.NeptuneGlaive:
                enchantment = weapon.GetEnchantmentOfType<NeptuneEnchantment>();
                break;
            case WeaponIds.ObsidianEdge:
                enchantment = weapon.GetEnchantmentOfType<ObsidianEnchantment>();
                break;
            case WeaponIds.YetiTooth:
                enchantment = weapon.GetEnchantmentOfType<YetiEnchantment>();
                break;
            case WeaponIds.InfinityBlade:
            case WeaponIds.InfinityDagger:
            case WeaponIds.InfinityGavel:
                enchantment = weapon.GetEnchantmentOfType<InfinityEnchantment>();
                break;
            default:
                if (ModHelper.ModRegistry.IsLoaded("JA.FishHatchering") && weapon.Name == "Sword Fish")
                {
                    enchantment = weapon.GetEnchantmentOfType<SwordFishEnchantment>();
                }
                else if (ModHelper.ModRegistry.IsLoaded("undare.crystalcrops.HCE") && weapon.Name.IsAnyOf(
                             "Blueglazer", "Crystallight", "Grapemaul", "Heartichoker", "Strawblaster", "Sunspark"))
                {
                    enchantment = weapon.GetEnchantmentOfType<UndareCrystallizedWeaponEnchantment>();
                }
                else
                {
                    enchantment = null;
                }

                break;
        }

        if (enchantment is null)
        {
            return;
        }

        weapon.RemoveEnchantment(enchantment);
        Log.D($"[CMBT]: Removed {enchantment.GetType().Name} from {weapon.Name}.");
    }

    /// <summary>Checks whether the <paramref name="weapon"/> has one of the special intrinsic enchantments.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="weapon"/> has an intrinsic mythic or legendary enchantment, otherwise <see langword="false"/>.</returns>
    internal static bool HasIntrinsicEnchantment(this MeleeWeapon weapon)
    {
        return weapon.HasAnyEnchantmentOf(
            typeof(DaggerEnchantment),
            typeof(CursedEnchantment),
            typeof(BlessedEnchantment),
            typeof(DaggerEnchantment),
            typeof(InfinityEnchantment),
            typeof(KillerBugEnchantment),
            typeof(LavaEnchantment),
            typeof(NeedleEnchantment),
            typeof(NeptuneEnchantment),
            typeof(ObsidianEnchantment),
            typeof(YetiEnchantment),
            typeof(SwordFishEnchantment),
            typeof(UndareCrystallizedWeaponEnchantment));
    }

    /// <summary>Adds the special item tag to the <paramref name="weapon"/> if it should have it.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    internal static void MakeSpecialIfNecessary(this MeleeWeapon weapon)
    {
        if (CombatModule.Config.EnableWeaponOverhaul)
        {
            switch (weapon.InitialParentTileIndex)
            {
                case WeaponIds.InsectHead:
                    weapon.type.Value = MeleeWeapon.dagger;
                    weapon.specialItem = true;
                    return;
                case WeaponIds.NeptuneGlaive:
                    weapon.specialItem = true;
                    return;
                default:
                {
                    if ((ModHelper.ModRegistry.IsLoaded("JA.FishHatchering") && weapon.Name == "Sword Fish") ||
                        (ModHelper.ModRegistry.IsLoaded("undare.crystalcrops.HCE") && weapon.Name.IsAnyOf(
                            "Blueglazer", "Crystallight", "Grapemaul", "Heartichoker", "Strawblaster", "Sunspark")))
                    {
                        weapon.specialItem = true;
                    }

                    return;
                }
            }
        }

        if (!CombatModule.Config.EnableWeaponOverhaul && !CombatModule.Config.EnableHeroQuest)
        {
            return;
        }

        if (weapon.isGalaxyWeapon() || weapon.IsInfinityWeapon() || weapon.IsCursedOrBlessed())
        {
            weapon.specialItem = true;
        }
    }

    internal static void SetFarmerAnimatingBackwards(this MeleeWeapon weapon, Farmer farmer)
    {
        Reflector
            .GetUnboundFieldSetter<MeleeWeapon, bool>("anotherClick")
            .Invoke(weapon, false);
        farmer.FarmerSprite.PauseForSingleAnimation = false;
        farmer.FarmerSprite.StopAnimation();

        Reflector
            .GetUnboundFieldSetter<MeleeWeapon, bool>("hasBegunWeaponEndPause")
            .Invoke(weapon, false);
        float swipeSpeed = 400 - (weapon.speed.Value * 40);
        swipeSpeed *= farmer.GetTotalSwingSpeedModifier();
        if (farmer.IsLocalPlayer)
        {
            for (var i = 0; i < weapon.enchantments.Count; i++)
            {
                if (weapon.enchantments[i] is BaseWeaponEnchantment weaponEnchantment)
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
