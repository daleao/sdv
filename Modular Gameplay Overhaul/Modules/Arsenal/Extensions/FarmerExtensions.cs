namespace DaLion.Overhaul.Modules.Arsenal.Extensions;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Arsenal.VirtualProperties;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Tools;
using static StardewValley.FarmerSprite;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
internal static class FarmerExtensions
{
    /// <summary>Gets the total swing speed modifier for the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="weapon">The <paramref name="farmer"/>'s weapon.</param>
    /// <returns>The total swing speed modifier, a number between 0 and 1.</returns>
    internal static float GetTotalSwingSpeedModifier(this Farmer farmer, MeleeWeapon? weapon = null)
    {
        var modifier = 1f;
        weapon ??= farmer.CurrentTool as MeleeWeapon;
        if (weapon is not null)
        {
            modifier *= weapon.Get_EffectiveSwingSpeed();
        }

        if (ModEntry.Config.EnableProfessions && farmer.professions.Contains(Farmer.brute))
        {
            modifier *= 1f - (ProfessionsModule.State.BruteRageCounter * 0.005f);
        }

        modifier *= 1f / (1f + farmer.weaponSpeedModifier);
        return modifier;
    }

    /// <summary>Gets the total firing speed modifier for the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="slingshot">The <paramref name="farmer"/>'s slingshot.</param>
    /// <returns>The total firing speed modifier, a number between 0 and 1.</returns>
    internal static float GetTotalFiringSpeedModifier(this Farmer farmer, Slingshot? slingshot = null)
    {
        var modifier = 10f / (10f + farmer.weaponSpeedModifier);
        slingshot ??= farmer.CurrentTool as Slingshot;
        if (slingshot is not null)
        {
            modifier *= slingshot.Get_EffectiveFireSpeed();
        }

        return modifier;
    }

    /// <summary>Gets the overhauled total resilience of the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The total resilience, a number between 0 and 1.</returns>
    internal static float GetOverhauledResilience(this Farmer farmer)
    {
        var weaponResilience = farmer.CurrentTool switch
        {
            MeleeWeapon weapon => weapon.Get_EffectiveResilience(),
            Slingshot slingshot => slingshot.Get_EffectiveResilience(),
            _ => 0f,
        };

        var playerResilience = farmer.resilience + farmer.Get_ResonantResilience();
        return weaponResilience * (10f / (10f + playerResilience));
    }

    /// <summary>Determines whether the <paramref name="farmer"/> is stepping on a snowy tile.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the corresponding <see cref="FarmerSprite"/> is using snowy step sounds, otherwise <see langword="false"/>.</returns>
    internal static bool IsSteppingOnSnow(this Farmer farmer)
    {
        return farmer.FarmerSprite.currentStep == "snowyStep";
    }

    /// <summary>Counts the number of completed Monster Eradication goals.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The number of completed Monster Eradication goals.</returns>
    internal static int NumMonsterSlayerQuestsCompleted(this Farmer farmer)
    {
        var count = 0;

        if (farmer.mailReceived.Contains("Gil_Slime Charmer Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Savage Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Skeleton Mask"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Insect Head"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Vampire Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Hard Hat"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Burglar's Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Crabshell Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Arcane Hat"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Knight's Helmet"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Napalm Ring"))
        {
            count++;
        }

        if (farmer.mailReceived.Contains("Gil_Telephone"))
        {
            count++;
        }

        return count;
    }

    /// <summary>Determines whether the <paramref name="farmer"/> has proven the 5 chivalric virtues.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="farmer"/> has proved all 5 chivalric virtues, otherwise <see langword="false"/>.</returns>
    internal static bool HasProvenChivalricVirtues(this Farmer farmer)
    {
        return farmer.Read<int>(DataFields.ProvenHonor) >= 3 &&
               farmer.Read<int>(DataFields.ProvenCompassion) >= 3 &&
               farmer.Read<int>(DataFields.ProvenWisdom) >= 3 &&
               farmer.mailReceived.Contains("pamHouseUpgrade") &&
               farmer.NumMonsterSlayerQuestsCompleted() >= 5;
    }

    #region combo framework

    internal static void QueueBackwardSwipe(this Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var modifier = farmer.GetTotalSwingSpeedModifier(weapon);
        var halfModifier = 1f - ((1f - modifier) / 2f);
        var cooldown = weapon.IsClub() ? 250 : 50;
        var sound = weapon.InitialParentTileIndex == Constants.LavaKatanaIndex ? "fireball" : "swordswipe";
        sprite.loopThisAnimation = false;
        var outFrames = sprite.currentAnimation;
        if (farmer.Get_IsAnimating())
        {
            outFrames.RemoveAt(sprite.CurrentAnimation.Count - 1);
        }
        else
        {
            outFrames.Clear();
        }

        switch (farmer.FacingDirection)
        {
            case Game1.up:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(41, (int)(65 * modifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(40, (int)(55 * modifier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(39, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
                        {
                            enchantment.OnSwing(weapon, who);
                        }

                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(38, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(37, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(36, (int)(cooldown * modifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(36, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.right:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(35, (int)(65 * modifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(34, (int)(55 * modifier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(33, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
                        {
                            enchantment.OnSwing(weapon, who);
                        }

                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(32, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(31, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(30, (int)(cooldown * modifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(30, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.down:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(29, (int)(55 * modifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(28, (int)(45 * modifier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(27, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
                        {
                            enchantment.OnSwing(weapon, who);
                        }

                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(26, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(25, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(24, (int)(cooldown * modifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(24, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.left:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(35, (int)(65 * modifier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(34, (int)(55 * modifier), true, flip: true, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(33, (int)(30 * halfModifier), true, flip: true, who =>
                    {
                        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
                        {
                            enchantment.OnSwing(weapon, who);
                        }

                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(32, (int)(30 * halfModifier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(31, (int)(30 * halfModifier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(30, (int)(cooldown * modifier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(30, 0, true, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;
        }

        Reflector
            .GetUnboundFieldSetter<FarmerSprite, int>(sprite, "currentAnimationFrames")
            .Invoke(sprite, sprite.CurrentAnimation.Count);
        farmer.Increment_CurrentHitStep();
        farmer.Set_IsAnimating(true);
    }

    internal static void QueueForwardSwipe(this Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var modifier = farmer.GetTotalSwingSpeedModifier(weapon);
        var halfModifier = 1f - ((1f - modifier) / 2f);
        var cooldown = weapon.IsClub() ? 250 : 50;
        var sound = weapon.IsClub() ? "clubswipe" : weapon.InitialParentTileIndex == Constants.LavaKatanaIndex ? "fireball" : "swordswipe";
        sprite.loopThisAnimation = false;
        var outFrames = sprite.currentAnimation;
        if (farmer.Get_IsAnimating())
        {
            outFrames.RemoveAt(sprite.CurrentAnimation.Count - 1);
        }
        else
        {
            outFrames.Clear();
        }

        switch (farmer.FacingDirection)
        {
            case Game1.up:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(36, (int)(65 * modifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(37, (int)(65 * modifier), true, flip: false,  who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(38, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
                        {
                            enchantment.OnSwing(weapon, who);
                        }

                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(39, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(40, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(41, (int)(cooldown * modifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(41, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.right:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(30, (int)(65 * modifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(31, (int)(55 * modifier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(32, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
                        {
                            enchantment.OnSwing(weapon, who);
                        }

                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(33, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(34, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(35, (int)(cooldown * modifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(35, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.down:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(24, (int)(65 * modifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(25, (int)(55 * modifier), true, flip: false,  who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(26, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
                        {
                            enchantment.OnSwing(weapon, who);
                        }

                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(27, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(28, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(29, (int)(cooldown * modifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(29, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.left:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(30, (int)(65 * modifier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(31, (int)(55 * modifier), true, flip: true,  who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(32, (int)(30 * halfModifier), true, flip: true, who =>
                    {
                        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
                        {
                            enchantment.OnSwing(weapon, who);
                        }

                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(33, (int)(30 * halfModifier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(34, (int)(30 * halfModifier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(35, (int)(cooldown * modifier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(35, 0, true, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;
        }

        Reflector
            .GetUnboundFieldSetter<FarmerSprite, int>(sprite, "currentAnimationFrames")
            .Invoke(sprite, sprite.CurrentAnimation.Count);
        farmer.Increment_CurrentHitStep();
        farmer.Set_IsAnimating(true);
    }

    internal static void QueueOverheadSwipe(this Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var modifier = farmer.GetTotalSwingSpeedModifier(weapon);
        var halfModifier = 1f - ((1f - modifier) / 2f);
        var windup = weapon.IsClub() ? 120 : 65;
        var cooldown = weapon.IsClub() ? 250 : 50;
        var sound = weapon.IsClub() ? "clubswipe" : "swordswipe";
        sprite.loopThisAnimation = false;
        var outFrames = sprite.currentAnimation;
        if (farmer.Get_IsAnimating())
        {
            outFrames.RemoveAt(sprite.CurrentAnimation.Count - 1);
        }
        else
        {
            outFrames.Clear();
        }

        switch (farmer.FacingDirection)
        {
            case Game1.up:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(36, (int)(windup * modifier), false, flip: false),
                    new AnimationFrame(37, (int)(50 * halfModifier), false, flip: false, Farmer.showToolSwipeEffect),
                    new AnimationFrame(38, (int)(50 * halfModifier), false, flip: false, who =>
                    {
                        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
                        {
                            enchantment.OnSwing(weapon, who);
                        }

                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(63, (int)(50 * halfModifier), false, flip: false, Farmer.useTool),
                    new AnimationFrame(62, (int)(cooldown * modifier), false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.right:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(48, (int)(windup * modifier), false, flip: false),
                    new AnimationFrame(49, (int)(50 * halfModifier), false, flip: false, Farmer.showToolSwipeEffect),
                    new AnimationFrame(50, (int)(50 * halfModifier), false, flip: false, who =>
                    {
                        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
                        {
                            enchantment.OnSwing(weapon, who);
                        }

                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(51, (int)(50 * halfModifier), false, flip: false, Farmer.useTool),
                    new AnimationFrame(52, (int)(cooldown * modifier), false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.down:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(66, (int)(windup * modifier), false, flip: false),
                    new AnimationFrame(67, (int)(50 * halfModifier), false, flip: false, Farmer.showToolSwipeEffect),
                    new AnimationFrame(68, (int)(50 * halfModifier), false, flip: false, who =>
                    {
                        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
                        {
                            enchantment.OnSwing(weapon, who);
                        }

                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(69, (int)(50 * halfModifier), false, flip: false, Farmer.useTool),
                    new AnimationFrame(70, (int)(cooldown * modifier), false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.left:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(48, (int)(windup * modifier), false, flip: true),
                    new AnimationFrame(49, (int)(50 * halfModifier), false, flip: true, Farmer.showToolSwipeEffect),
                    new AnimationFrame(50, (int)(50 * halfModifier), false, flip: true, who =>
                    {
                        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
                        {
                            enchantment.OnSwing(weapon, who);
                        }

                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(51, (int)(50 * halfModifier), false, flip: true, Farmer.useTool),
                    new AnimationFrame(52, (int)(cooldown * modifier), false, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;
        }

        Reflector
            .GetUnboundFieldSetter<FarmerSprite, int>(sprite, "currentAnimationFrames")
            .Invoke(sprite, sprite.CurrentAnimation.Count);
        farmer.Increment_CurrentHitStep();
        farmer.Set_IsAnimating(true);
    }

    internal static void QueueThrust(this Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var modifier = farmer.GetTotalSwingSpeedModifier(weapon);
        var halfModifier = 1f - ((1f - modifier) / 2f);
        sprite.loopThisAnimation = false;
        var outFrames = sprite.currentAnimation;
        if (farmer.Get_IsAnimating())
        {
            outFrames.RemoveAt(sprite.CurrentAnimation.Count - 1);
        }
        else
        {
            outFrames.Clear();
        }

        switch (farmer.FacingDirection)
        {
            case Game1.up:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(38, (int)(50 * halfModifier), true, flip: false),
                    new AnimationFrame(40, (int)(120 * halfModifier), true, flip: false, who =>
                    {
                        Farmer.useTool(who);
                        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
                        {
                            enchantment.OnSwing(weapon, who);
                        }

                        who.currentLocation.localSound("daggerswipe");
                    }),
                    new AnimationFrame(38, (int)(50 * halfModifier), true, flip: false),
                    new AnimationFrame(38, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.right:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(33, (int)(50 * halfModifier), false, flip: false),
                    new AnimationFrame(34, (int)(120 * halfModifier), false, flip: false, who =>
                    {
                        Farmer.useTool(who);
                        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
                        {
                            enchantment.OnSwing(weapon, who);
                        }

                        who.currentLocation.localSound("daggerswipe");
                    }),
                    new AnimationFrame(33, (int)(50 * halfModifier), false, flip: false),
                    new AnimationFrame(33, 0, false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.down:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(25, (int)(50 * halfModifier), true, flip: false),
                    new AnimationFrame(27, (int)(120 * halfModifier), true, flip: false, who =>
                    {
                        Farmer.useTool(who);
                        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
                        {
                            enchantment.OnSwing(weapon, who);
                        }

                        who.currentLocation.localSound("daggerswipe");
                    }),
                    new AnimationFrame(25, (int)(50 * halfModifier), true, flip: false),
                    new AnimationFrame(25, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.left:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(38, (int)(50 * halfModifier), false, flip: true),
                    new AnimationFrame(40, (int)(120 * halfModifier), false, flip: true, who =>
                    {
                        Farmer.useTool(who);
                        foreach (var enchantment in weapon.enchantments.OfType<BaseWeaponEnchantment>())
                        {
                            enchantment.OnSwing(weapon, who);
                        }

                        who.currentLocation.localSound("daggerswipe");
                    }),
                    new AnimationFrame(38, (int)(50 * halfModifier), false, flip: true),
                    new AnimationFrame(38, 0, false, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;
        }

        Reflector
            .GetUnboundFieldSetter<FarmerSprite, int>(sprite, "currentAnimationFrames")
            .Invoke(sprite, sprite.CurrentAnimation.Count);
        farmer.Increment_CurrentHitStep();
        farmer.Set_IsAnimating(true);
    }

    #endregion combo framework
}
