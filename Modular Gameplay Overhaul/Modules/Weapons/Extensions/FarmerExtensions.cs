namespace DaLion.Overhaul.Modules.Weapons.Extensions;

#region using directives

using System.Diagnostics;
using System.Linq;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Overhaul.Modules.Weapons.VirtualProperties;
using DaLion.Shared.Extensions.Collections;
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

        if (ProfessionsModule.ShouldEnable && farmer.professions.Contains(Farmer.brute))
        {
            modifier *= 1f - (ProfessionsModule.State.BruteRageCounter * 0.005f);
        }

        modifier *= 1f / (1f + farmer.weaponSpeedModifier);
        return modifier;
    }

    /// <summary>Checks whether the <paramref name="farmer"/> suffers from Viego's curse.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="farmer"/> has received the Dark Sword but not the Holy Blade, otherwise <see langword="false"/>.</returns>
    internal static bool IsCursed(this Farmer farmer)
    {
        return farmer.mailReceived.Contains("gotDarkSword") && !farmer.mailReceived.Contains("gotHolyBlade");
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

    [Conditional("RELEASE")]
    internal static void DoStabbingSpecielCooldown(this Farmer user, MeleeWeapon? sword = null)
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

    #region combo framework

    internal static void QueueForwardSwipe(this Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var modifier = farmer.GetTotalSwingSpeedModifier(weapon);
        var halfModifier = 1f - ((1f - modifier) / 2f);
        var cooldown = weapon.IsClub() ? 250 : 50;
        var sound = weapon.IsClub() ? "clubswipe" : weapon.InitialParentTileIndex == ItemIDs.LavaKatana ? "fireball" : "swordswipe";
        sprite.loopThisAnimation = false;
        var outFrames = sprite.currentAnimation;
        if (WeaponsModule.State.FarmerAnimating)
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
                        WeaponsModule.State.ComboHitStep++;
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
                        if (WeaponsModule.Config.SwipeHold && WeaponsModule.State.HoldingWeaponSwing)
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
                        WeaponsModule.State.ComboHitStep++;
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
                        if (WeaponsModule.Config.SwipeHold && WeaponsModule.State.HoldingWeaponSwing)
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
                        WeaponsModule.State.ComboHitStep++;
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
                        if (WeaponsModule.Config.SwipeHold && WeaponsModule.State.HoldingWeaponSwing)
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
                        WeaponsModule.State.ComboHitStep++;
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
                        if (WeaponsModule.Config.SwipeHold && WeaponsModule.State.HoldingWeaponSwing)
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
        WeaponsModule.State.ComboHitQueued++;
        WeaponsModule.State.FarmerAnimating = true;
        Log.D("[Combo]: Queued Forward Slash");
    }

    internal static void QueueReverseSwipe(this Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var modifier = farmer.GetTotalSwingSpeedModifier(weapon);
        var halfModifier = 1f - ((1f - modifier) / 2f);
        const int cooldown = 50;
        var sound = weapon.InitialParentTileIndex == ItemIDs.LavaKatana ? "fireball" : "swordswipe";
        sprite.loopThisAnimation = false;
        var outFrames = sprite.currentAnimation;
        if (WeaponsModule.State.FarmerAnimating)
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
                        WeaponsModule.State.ComboHitStep++;
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
                        if (WeaponsModule.Config.SwipeHold && WeaponsModule.State.HoldingWeaponSwing)
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
                        WeaponsModule.State.ComboHitStep++;
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
                        if (WeaponsModule.Config.SwipeHold && WeaponsModule.State.HoldingWeaponSwing)
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
                        WeaponsModule.State.ComboHitStep++;
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
                        if (WeaponsModule.Config.SwipeHold && WeaponsModule.State.HoldingWeaponSwing)
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
                        WeaponsModule.State.ComboHitStep++;
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
                        if (WeaponsModule.Config.SwipeHold && WeaponsModule.State.HoldingWeaponSwing)
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
        WeaponsModule.State.ComboHitQueued++;
        WeaponsModule.State.FarmerAnimating = true;
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
        if (WeaponsModule.State.FarmerAnimating)
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
                        WeaponsModule.State.ComboHitStep++;
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
                        WeaponsModule.State.ComboHitStep++;
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
                        WeaponsModule.State.ComboHitStep++;
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
                        WeaponsModule.State.ComboHitStep++;
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
        WeaponsModule.State.ComboHitQueued++;
        WeaponsModule.State.FarmerAnimating = true;
        Log.D("[Combo]: Queued Smash");
    }

    internal static void QueueThrust(this Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var modifier = farmer.GetTotalSwingSpeedModifier(weapon);
        var halfModifier = 1f - ((1f - modifier) / 2f);
        sprite.loopThisAnimation = false;
        var outFrames = sprite.currentAnimation;
        if (WeaponsModule.State.FarmerAnimating)
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
                        WeaponsModule.State.ComboHitStep++;
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
                        WeaponsModule.State.ComboHitStep++;
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
                        WeaponsModule.State.ComboHitStep++;
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
                        WeaponsModule.State.ComboHitStep++;
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
        WeaponsModule.State.ComboHitQueued++;
        WeaponsModule.State.FarmerAnimating = true;
    }

    private static void QueueNextSwipeAfterForward(this Farmer farmer, MeleeWeapon weapon)
    {
        var hitStep = WeaponsModule.State.ComboHitQueued;
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
        var hitStep = WeaponsModule.State.ComboHitQueued;
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
