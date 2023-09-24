namespace DaLion.Overhaul.Modules.Combat.Extensions;

#region using directives

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Tools;
using static StardewValley.FarmerSprite;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
internal static class FarmerExtensions
{
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

    /// <summary>Gets the total swing speed modifier for the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="weapon">The <paramref name="farmer"/>'s weapon.</param>
    /// <returns>The total swing speed modifier, a number between 0 and 1.</returns>
    /// <remarks>Smaller is faster.</remarks>
    internal static float GetTotalSwingSpeedModifier(this Farmer farmer, MeleeWeapon? weapon = null)
    {
        weapon ??= farmer.CurrentTool as MeleeWeapon;

        var modifier = 1f;
        if (weapon is not null)
        {
            modifier *= weapon.Get_EffectiveSwingSpeed();
        }

        if (ProfessionsModule.ShouldEnable && farmer.professions.Contains(Farmer.brute))
        {
            modifier *= 1f - (ProfessionsModule.State.BruteRageCounter * 0.005f);
        }

        modifier *= 1f / (1f + farmer.weaponSpeedModifier);
        modifier *= (18f - CombatModule.State.SavageExcitedness) / 18f;
        return modifier;
    }

    /// <summary>Gets the total firing speed modifier for the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="slingshot">The <paramref name="farmer"/>'s slingshot.</param>
    /// <returns>The total firing speed modifier, a number between 0 and 1.</returns>
    /// <remarks>Smaller is faster.</remarks>
    internal static float GetTotalFiringSpeedModifier(this Farmer farmer, Slingshot? slingshot = null)
    {
        slingshot ??= farmer.CurrentTool as Slingshot;

        var modifier = 1f;
        if (slingshot is not null)
        {
            modifier *= slingshot.Get_EffectiveFireSpeed();
        }

        modifier *= 1f / (1f + farmer.weaponSpeedModifier);
        modifier *= (18f - CombatModule.State.SavageExcitedness) / 18f;
        return modifier;
    }

    /// <summary>Checks whether the <paramref name="farmer"/> suffers from Viego's curse.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="farmer"/> has received the Dark Sword but not the Holy Blade, otherwise <see langword="false"/>.</returns>
    internal static bool IsCursed(this Farmer farmer)
    {
        return farmer.mailReceived.Contains("gotDarkSword") && !farmer.mailReceived.Contains("gotHolyBlade");
    }

    /// <summary>Checks whether the <paramref name="farmer"/> suffers from Viego's curse.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="darkSword">The curse's origin.</param>
    /// <returns><see langword="true"/> if the <paramref name="farmer"/> has received the Dark Sword but not the Holy Blade, otherwise <see langword="false"/>.</returns>
    internal static bool IsCursed(this Farmer farmer, [NotNullWhen(true)] out MeleeWeapon? darkSword)
    {
        if (!farmer.IsCursed())
        {
            darkSword = null;
            return false;
        }

        darkSword = (MeleeWeapon?)farmer.Items.FirstOrDefault(item => item is MeleeWeapon { InitialParentTileIndex: WeaponIds.DarkSword });
        return darkSword is not null;
    }

    /// <summary>Determines whether the <paramref name="farmer"/> is stepping on a snowy tile.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the corresponding <see cref="FarmerSprite"/> is using snowy step sounds, otherwise <see langword="false"/>.</returns>
    internal static bool IsSteppingOnSnow(this Farmer farmer)
    {
        return farmer.FarmerSprite.currentStep == "snowyStep";
    }

    [Conditional("RELEASE")]
    internal static void DoStabbingSpecialCooldown(this Farmer user, MeleeWeapon? sword = null)
    {
        sword ??= (MeleeWeapon)user.CurrentTool;

        MeleeWeapon.attackSwordCooldown = MeleeWeapon.attackSwordCooldownTime;
        if (!ProfessionsModule.ShouldEnable && user.professions.Contains(Farmer.acrobat))
        {
            MeleeWeapon.attackSwordCooldown /= 2;
        }

        if (sword.hasEnchantmentOfType<ArtfulEnchantment>())
        {
            MeleeWeapon.attackSwordCooldown /= 2;
        }

        MeleeWeapon.attackSwordCooldown = (int)(MeleeWeapon.attackSwordCooldown *
                                                sword.Get_EffectiveCooldownReduction() *
                                                user.Get_CooldownReduction());
    }

    [Conditional("RELEASE")]
    internal static void DoSlingshotSpecialCooldown(this Farmer user, Slingshot? slingshot = null)
    {
        slingshot ??= (Slingshot)user.CurrentTool;

        if (slingshot.Get_IsOnSpecial())
        {
            const int SlingshotCooldown = 2000;
            CombatModule.State.SlingshotCooldown = SlingshotCooldown;
            if (!ProfessionsModule.ShouldEnable && user.professions.Contains(Farmer.acrobat))
            {
                CombatModule.State.SlingshotCooldown /= 2;
            }

            if (slingshot.hasEnchantmentOfType<ArtfulEnchantment>())
            {
                CombatModule.State.SlingshotCooldown /= 2;
            }

            CombatModule.State.SlingshotCooldown = (int)(CombatModule.State.SlingshotCooldown *
                                                             slingshot.Get_EffectiveCooldownReduction() *
                                                             user.Get_CooldownReduction());
        }
        else
        {
            CombatModule.State.SlingshotCooldown -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            if (CombatModule.State.SlingshotCooldown <= 0)
            {
                Game1.playSound("objectiveComplete");
            }
        }
    }

    #region combo framework

    internal static void QueueForwardSwipe(this Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var modifier = farmer.GetTotalSwingSpeedModifier(weapon);
        var halfModifier = 1f - ((1f - modifier) / 2f);
        var cooldown = weapon.IsClub() ? 250 : 50;
        var sound = weapon.IsClub() ? "clubswipe" : weapon.InitialParentTileIndex == WeaponIds.LavaKatana ? "fireball" : "swordswipe";
        sprite.loopThisAnimation = false;
        var outFrames = sprite.currentAnimation;
        if (CombatModule.State.FarmerAnimating)
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
                    new AnimationFrame(36, (int)(65 * modifier), true, flip: false, who =>
                    {
                        CombatModule.State.ComboHitStep++;
                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(37, (int)(65 * modifier), true, flip: false,  who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(38, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(39, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(40, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(41, (int)(cooldown * modifier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        if (CombatModule.Config.SwipeHold && CombatModule.State.HoldingWeaponSwing)
                        {
                            who.QueueNextSwipeAfterForward(weapon);
                        }
                    }),
                    new AnimationFrame(41, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.right:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(30, (int)(65 * modifier), true, flip: false, who =>
                    {
                        CombatModule.State.ComboHitStep++;
                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(31, (int)(55 * modifier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(32, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(33, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(34, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(35, (int)(cooldown * modifier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        if (CombatModule.Config.SwipeHold && CombatModule.State.HoldingWeaponSwing)
                        {
                            who.QueueNextSwipeAfterForward(weapon);
                        }
                    }),
                    new AnimationFrame(35, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.down:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(24, (int)(65 * modifier), true, flip: false, who =>
                    {
                        CombatModule.State.ComboHitStep++;
                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(25, (int)(55 * modifier), true, flip: false,  who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(26, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(27, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(28, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(29, (int)(cooldown * modifier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        if (CombatModule.Config.SwipeHold && CombatModule.State.HoldingWeaponSwing)
                        {
                            who.QueueNextSwipeAfterForward(weapon);
                        }
                    }),
                    new AnimationFrame(29, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.left:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(30, (int)(65 * modifier), true, flip: true, who =>
                    {
                        CombatModule.State.ComboHitStep++;
                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(31, (int)(55 * modifier), true, flip: true,  who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(32, (int)(30 * halfModifier), true, flip: true, who =>
                    {
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(33, (int)(30 * halfModifier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(34, (int)(30 * halfModifier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(35, (int)(cooldown * modifier), true, flip: true, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        if (CombatModule.Config.SwipeHold && CombatModule.State.HoldingWeaponSwing)
                        {
                            who.QueueNextSwipeAfterForward(weapon);
                        }
                    }),
                    new AnimationFrame(35, 0, true, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;
        }

        Reflector
            .GetUnboundFieldSetter<FarmerSprite, int>(sprite, "currentAnimationFrames")
            .Invoke(sprite, sprite.CurrentAnimation.Count);
        CombatModule.State.ComboHitQueued++;
        CombatModule.State.FarmerAnimating = true;
        Log.D("[Combo]: Queued Forward Slash");
    }

    internal static void QueueReverseSwipe(this Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var modifier = farmer.GetTotalSwingSpeedModifier(weapon);
        var halfModifier = 1f - ((1f - modifier) / 2f);
        const int cooldown = 50;
        var sound = weapon.InitialParentTileIndex == WeaponIds.LavaKatana ? "fireball" : "swordswipe";
        sprite.loopThisAnimation = false;
        var outFrames = sprite.currentAnimation;
        if (CombatModule.State.FarmerAnimating)
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
                    new AnimationFrame(41, (int)(65 * modifier), true, flip: false, who =>
                    {
                        CombatModule.State.ComboHitStep++;
                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(40, (int)(55 * modifier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(39, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(38, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(37, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(36, (int)(cooldown * modifier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        if (CombatModule.Config.SwipeHold && CombatModule.State.HoldingWeaponSwing)
                        {
                            who.QueueNextSwipeAfterReverse(weapon);
                        }
                    }),
                    new AnimationFrame(36, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.right:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(35, (int)(65 * modifier), true, flip: false, who =>
                    {
                        CombatModule.State.ComboHitStep++;
                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(34, (int)(55 * modifier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(33, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(32, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(31, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(30, (int)(cooldown * modifier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        if (CombatModule.Config.SwipeHold && CombatModule.State.HoldingWeaponSwing)
                        {
                            who.QueueNextSwipeAfterReverse(weapon);
                        }
                    }),
                    new AnimationFrame(30, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.down:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(29, (int)(55 * modifier), true, flip: false, who =>
                    {
                        CombatModule.State.ComboHitStep++;
                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(28, (int)(45 * modifier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(27, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(26, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(25, (int)(30 * halfModifier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(24, (int)(cooldown * modifier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        if (CombatModule.Config.SwipeHold && CombatModule.State.HoldingWeaponSwing)
                        {
                            who.QueueNextSwipeAfterReverse(weapon);
                        }
                    }),
                    new AnimationFrame(24, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.left:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(35, (int)(65 * modifier), true, flip: true, who =>
                    {
                        CombatModule.State.ComboHitStep++;
                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(34, (int)(55 * modifier), true, flip: true, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(33, (int)(30 * halfModifier), true, flip: true, who =>
                    {
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        Farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(32, (int)(30 * halfModifier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(31, (int)(30 * halfModifier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(30, (int)(cooldown * modifier), true, flip: true, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        if (CombatModule.Config.SwipeHold && CombatModule.State.HoldingWeaponSwing)
                        {
                            who.QueueNextSwipeAfterReverse(weapon);
                        }
                    }),
                    new AnimationFrame(30, 0, true, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;
        }

        Reflector
            .GetUnboundFieldSetter<FarmerSprite, int>(sprite, "currentAnimationFrames")
            .Invoke(sprite, sprite.CurrentAnimation.Count);
        CombatModule.State.ComboHitQueued++;
        CombatModule.State.FarmerAnimating = true;
        Log.D("[Combo]: Queued Backslash");
    }

    internal static void QueueSmash(this Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var modifier = farmer.GetTotalSwingSpeedModifier(weapon);
        var halfModifier = 1f - ((1f - modifier) / 2f);
        var windup = weapon.IsClub() ? 120 : 65;
        var cooldown = weapon.IsClub() ? 250 : 50;
        var sound = weapon.IsClub() ? "clubswipe" : "swordswipe";
        sprite.loopThisAnimation = false;
        var outFrames = sprite.currentAnimation;
        if (CombatModule.State.FarmerAnimating)
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
                    new AnimationFrame(36, (int)(windup * modifier), false, flip: false, who =>
                    {
                        CombatModule.State.ComboHitStep++;
                        who.DamageDuringSmash();
                    }),
                    new AnimationFrame(37, (int)(50 * halfModifier), false, flip: false, who =>
                    {
                        who.DamageDuringSmash();
                        Farmer.showToolSwipeEffect(who);
                    }),
                    new AnimationFrame(38, (int)(50 * halfModifier), false, flip: false, who =>
                    {
                        who.DamageDuringSmash();
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(63, (int)(50 * halfModifier), false, flip: false, who =>
                    {
                        who.DamageDuringSmash();
                        Farmer.useTool(who);
                    }),
                    new AnimationFrame(62, (int)(cooldown * modifier), false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.right:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(48, (int)(windup * modifier), false, flip: false, who =>
                    {
                        CombatModule.State.ComboHitStep++;
                        who.DamageDuringSmash();
                    }),
                    new AnimationFrame(49, (int)(50 * halfModifier), false, flip: false, who =>
                    {
                        who.DamageDuringSmash();
                        Farmer.showToolSwipeEffect(who);
                    }),
                    new AnimationFrame(50, (int)(50 * halfModifier), false, flip: false, who =>
                    {
                        who.DamageDuringSmash();
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(51, (int)(50 * halfModifier), false, flip: false, who =>
                    {
                        who.DamageDuringSmash();
                        Farmer.useTool(who);
                    }),
                    new AnimationFrame(52, (int)(cooldown * modifier), false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.down:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(66, (int)(windup * modifier), false, flip: false, who =>
                    {
                        CombatModule.State.ComboHitStep++;
                        who.DamageDuringSmash();
                    }),
                    new AnimationFrame(67, (int)(50 * halfModifier), false, flip: false, who =>
                    {
                        who.DamageDuringSmash();
                        Farmer.showToolSwipeEffect(who);
                    }),
                    new AnimationFrame(68, (int)(50 * halfModifier), false, flip: false, who =>
                    {
                        who.DamageDuringSmash();
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(69, (int)(50 * halfModifier), false, flip: false, who =>
                    {
                        who.DamageDuringSmash();
                        Farmer.useTool(who);
                    }),
                    new AnimationFrame(70, (int)(cooldown * modifier), false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.left:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(48, (int)(windup * modifier), false, flip: true, who =>
                    {
                        CombatModule.State.ComboHitStep++;
                        who.DamageDuringSmash();
                    }),
                    new AnimationFrame(49, (int)(50 * halfModifier), false, flip: true, who =>
                    {
                        who.DamageDuringSmash();
                        Farmer.showToolSwipeEffect(who);
                    }),
                    new AnimationFrame(50, (int)(50 * halfModifier), false, flip: true, who =>
                    {
                        who.DamageDuringSmash();
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(51, (int)(50 * halfModifier), false, flip: true, who =>
                    {
                        who.DamageDuringSmash();
                        Farmer.useTool(who);
                    }),
                    new AnimationFrame(52, (int)(cooldown * modifier), false, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;
        }

        Reflector
            .GetUnboundFieldSetter<FarmerSprite, int>(sprite, "currentAnimationFrames")
            .Invoke(sprite, sprite.CurrentAnimation.Count);
        CombatModule.State.ComboHitQueued++;
        CombatModule.State.FarmerAnimating = true;
        Log.D("[Combo]: Queued Smash");
    }

    internal static void QueueThrust(this Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var modifier = farmer.GetTotalSwingSpeedModifier(weapon);
        var halfModifier = 1f - ((1f - modifier) / 2f);
        sprite.loopThisAnimation = false;
        var outFrames = sprite.currentAnimation;
        if (CombatModule.State.FarmerAnimating)
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
                    new AnimationFrame(38, (int)(50 * halfModifier), true, flip: false, who =>
                    {
                        CombatModule.State.ComboHitStep++;
                        who.DamageDuringThrust();
                    }),
                    new AnimationFrame(40, (int)(120 * halfModifier), true, flip: false, who =>
                    {
                        who.DamageDuringThrust();
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        who.currentLocation.localSound("daggerswipe");
                    }),
                    new AnimationFrame(38, (int)(50 * halfModifier), true, flip: false, DamageDuringThrust),
                    new AnimationFrame(38, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.right:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(33, (int)(50 * halfModifier), false, flip: false, who =>
                    {
                        CombatModule.State.ComboHitStep++;
                        who.DamageDuringThrust();
                    }),
                    new AnimationFrame(34, (int)(120 * halfModifier), false, flip: false, who =>
                    {
                        who.DamageDuringThrust();
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        who.currentLocation.localSound("daggerswipe");
                    }),
                    new AnimationFrame(33, (int)(50 * halfModifier), false, flip: false, DamageDuringThrust),
                    new AnimationFrame(33, 0, false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.down:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(25, (int)(50 * halfModifier), true, flip: false, who =>
                    {
                        CombatModule.State.ComboHitStep++;
                        who.DamageDuringThrust();
                    }),
                    new AnimationFrame(27, (int)(120 * halfModifier), true, flip: false, who =>
                    {
                        who.DamageDuringThrust();
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        who.currentLocation.localSound("daggerswipe");
                    }),
                    new AnimationFrame(25, (int)(50 * halfModifier), true, flip: false, DamageDuringThrust),
                    new AnimationFrame(25, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.left:
                outFrames.AddRange(new[]
                {
                    new AnimationFrame(38, (int)(50 * halfModifier), false, flip: true, who =>
                    {
                        CombatModule.State.ComboHitStep++;
                        who.DamageDuringThrust();
                    }),
                    new AnimationFrame(40, (int)(120 * halfModifier), false, flip: true, who =>
                    {
                        who.DamageDuringThrust();
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        who.currentLocation.localSound("daggerswipe");
                    }),
                    new AnimationFrame(38, (int)(50 * halfModifier), false, flip: true, DamageDuringThrust),
                    new AnimationFrame(38, 0, false, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;
        }

        Reflector
            .GetUnboundFieldSetter<FarmerSprite, int>(sprite, "currentAnimationFrames")
            .Invoke(sprite, sprite.CurrentAnimation.Count);
        CombatModule.State.ComboHitQueued++;
        CombatModule.State.FarmerAnimating = true;
    }

    private static void QueueNextSwipeAfterForward(this Farmer farmer, MeleeWeapon weapon)
    {
        var hitStep = CombatModule.State.ComboHitQueued;
        var finalHitStep = weapon.GetFinalHitStep();
        if (hitStep >= finalHitStep)
        {
            return;
        }

        if (weapon.IsClub() && hitStep == finalHitStep - 1)
        {
            farmer.QueueSmash(weapon);
        }
        else if ((int)hitStep % 2 == 1)
        {
            farmer.QueueReverseSwipe(weapon);
        }
    }

    private static void QueueNextSwipeAfterReverse(this Farmer farmer, MeleeWeapon weapon)
    {
        var hitStep = CombatModule.State.ComboHitQueued;
        var finalHitStep = weapon.GetFinalHitStep();
        if (hitStep >= finalHitStep || (int)hitStep % 2 != 0)
        {
            return;
        }

        farmer.QueueForwardSwipe(weapon);
    }

    private static void DamageDuringSmash(this Farmer who)
    {
        var (x, y) = who.GetToolLocation(ignoreClick: true);
        ((MeleeWeapon)who.CurrentTool).DoDamage(who.currentLocation, (int)x, (int)y, who.FacingDirection, 1, who);
    }

    private static void DamageDuringThrust(this Farmer who)
    {
        var (x, y) = who.getUniformPositionAwayFromBox(who.FacingDirection, 48);
        ((MeleeWeapon)who.CurrentTool).DoDamage(who.currentLocation, (int)x, (int)y, who.FacingDirection, 1, who);
    }

    #endregion combo framework
}
