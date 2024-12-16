namespace DaLion.Combat.Framework.Extensions;

#region using directives

using System.Linq;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Reflection;
using StardewValley.Enchantments;
using StardewValley.Tools;
using static StardewValley.FarmerSprite;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
internal static class FarmerExtensions
{
    /// <summary>Gets the total defense for the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="weapon">The farmer's currently held <see cref="MeleeWeapon"/>.</param>
    /// <returns>The total defense modifier.</returns>
    internal static float GetTotalDefenseModifier(this Farmer farmer, MeleeWeapon? weapon = null)
    {
        weapon ??= farmer.CurrentTool as MeleeWeapon;
        return (float)Math.Pow(0.9, farmer.buffs.Defense + (weapon?.addedDefense.Value ?? 0));
    }

    #region combo framework

    internal static void QueueForwardSwipe(this Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var modifier = weapon.GetTotalSpeedModifier(farmer);
        var halfModifier = modifier / 2f;
        var cooldown = weapon.IsClub() ? 150 : 50;
        var sound = weapon.GetSwipeSound(State.CurrentHitStep);
        sprite.loopThisAnimation = false;
        var outFrames = sprite.currentAnimation;
        if (State.FarmerAnimating)
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
                outFrames.AddRange([
                    new AnimationFrame(36, (int)(65 * modifier), true, flip: false, who =>
                    {
                        State.CurrentHitStep++;
                        farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(37, (int)(65 * modifier), true, flip: false,  who =>
                    {
                        farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(38, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(39, (int)(30 * halfModifier), true, flip: false, farmer.showSwordSwipe),
                    new AnimationFrame(40, (int)(30 * halfModifier), true, flip: false, farmer.showSwordSwipe),
                    new AnimationFrame(41, (int)(cooldown * modifier), true, flip: false, who =>
                    {
                        farmer.showSwordSwipe(who);
                        if (Config.SwipeHold && State.HoldingWeaponSwing)
                        {
                            who.QueueNextSwipeAfterForward(weapon);
                        }
                    }),
                    new AnimationFrame(41, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                ]);

                break;

            case Game1.right:
                outFrames.AddRange([
                    new AnimationFrame(30, (int)(65 * modifier), true, flip: false, who =>
                    {
                        State.CurrentHitStep++;
                        farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(31, (int)(55 * modifier), true, flip: false, who =>
                    {
                        farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(32, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(33, (int)(30 * halfModifier), true, flip: false, farmer.showSwordSwipe),
                    new AnimationFrame(34, (int)(30 * halfModifier), true, flip: false, farmer.showSwordSwipe),
                    new AnimationFrame(35, (int)(cooldown * modifier), true, flip: false, who =>
                    {
                        farmer.showSwordSwipe(who);
                        if (Config.SwipeHold && State.HoldingWeaponSwing)
                        {
                            who.QueueNextSwipeAfterForward(weapon);
                        }
                    }),
                    new AnimationFrame(35, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                ]);

                break;

            case Game1.down:
                outFrames.AddRange([
                    new AnimationFrame(24, (int)(65 * modifier), true, flip: false, who =>
                    {
                        State.CurrentHitStep++;
                        farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(25, (int)(55 * modifier), true, flip: false,  who =>
                    {
                        farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(26, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(27, (int)(30 * halfModifier), true, flip: false, farmer.showSwordSwipe),
                    new AnimationFrame(28, (int)(30 * halfModifier), true, flip: false, farmer.showSwordSwipe),
                    new AnimationFrame(29, (int)(cooldown * modifier), true, flip: false, who =>
                    {
                        farmer.showSwordSwipe(who);
                        if (Config.SwipeHold && State.HoldingWeaponSwing)
                        {
                            who.QueueNextSwipeAfterForward(weapon);
                        }
                    }),
                    new AnimationFrame(29, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                ]);

                break;

            case Game1.left:
                outFrames.AddRange([
                    new AnimationFrame(30, (int)(65 * modifier), true, flip: true, who =>
                    {
                        State.CurrentHitStep++;
                        farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(31, (int)(55 * modifier), true, flip: true,  who =>
                    {
                        farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(32, (int)(30 * halfModifier), true, flip: true, who =>
                    {
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(33, (int)(30 * halfModifier), true, flip: true, farmer.showSwordSwipe),
                    new AnimationFrame(34, (int)(30 * halfModifier), true, flip: true, farmer.showSwordSwipe),
                    new AnimationFrame(35, (int)(cooldown * modifier), true, flip: true, who =>
                    {
                        farmer.showSwordSwipe(who);
                        if (Config.SwipeHold && State.HoldingWeaponSwing)
                        {
                            who.QueueNextSwipeAfterForward(weapon);
                        }
                    }),
                    new AnimationFrame(35, 0, true, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                ]);

                break;
        }

        Reflector
            .GetUnboundFieldSetter<FarmerSprite, int>("currentAnimationFrames")
            .Invoke(sprite, sprite.CurrentAnimation.Count);
        State.QueuedHitStep++;
        State.FarmerAnimating = true;
        Log.D("[Combo]: Queued Forward Slash");
    }

    internal static void QueueReverseSwipe(this Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var modifier = weapon.GetTotalSpeedModifier(farmer);
        var halfModifier = modifier / 2f;
        const int cooldown = 50;
        var sound = weapon.GetSwipeSound(State.CurrentHitStep);
        sprite.loopThisAnimation = false;
        var outFrames = sprite.currentAnimation;
        if (State.FarmerAnimating)
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
                outFrames.AddRange([
                    new AnimationFrame(41, (int)(65 * modifier), true, flip: false, who =>
                    {
                        State.CurrentHitStep++;
                        farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(40, (int)(55 * modifier), true, flip: false, who =>
                    {
                        farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(39, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(38, (int)(30 * halfModifier), true, flip: false, farmer.showSwordSwipe),
                    new AnimationFrame(37, (int)(30 * halfModifier), true, flip: false, farmer.showSwordSwipe),
                    new AnimationFrame(36, (int)(cooldown * modifier), true, flip: false, who =>
                    {
                        farmer.showSwordSwipe(who);
                        if (Config.SwipeHold && State.HoldingWeaponSwing)
                        {
                            who.QueueNextSwipeAfterReverse(weapon);
                        }
                    }),
                    new AnimationFrame(36, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                ]);

                break;

            case Game1.right:
                outFrames.AddRange([
                    new AnimationFrame(35, (int)(65 * modifier), true, flip: false, who =>
                    {
                        State.CurrentHitStep++;
                        farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(34, (int)(55 * modifier), true, flip: false, who =>
                    {
                        farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(33, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(32, (int)(30 * halfModifier), true, flip: false, farmer.showSwordSwipe),
                    new AnimationFrame(31, (int)(30 * halfModifier), true, flip: false, farmer.showSwordSwipe),
                    new AnimationFrame(30, (int)(cooldown * modifier), true, flip: false, who =>
                    {
                        farmer.showSwordSwipe(who);
                        if (Config.SwipeHold && State.HoldingWeaponSwing)
                        {
                            who.QueueNextSwipeAfterReverse(weapon);
                        }
                    }),
                    new AnimationFrame(30, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                ]);

                break;

            case Game1.down:
                outFrames.AddRange([
                    new AnimationFrame(29, (int)(55 * modifier), true, flip: false, who =>
                    {
                        State.CurrentHitStep++;
                        farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(28, (int)(45 * modifier), true, flip: false, who =>
                    {
                        farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(27, (int)(30 * halfModifier), true, flip: false, who =>
                    {
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(26, (int)(30 * halfModifier), true, flip: false, farmer.showSwordSwipe),
                    new AnimationFrame(25, (int)(30 * halfModifier), true, flip: false, farmer.showSwordSwipe),
                    new AnimationFrame(24, (int)(cooldown * modifier), true, flip: false, who =>
                    {
                        farmer.showSwordSwipe(who);
                        if (Config.SwipeHold && State.HoldingWeaponSwing)
                        {
                            who.QueueNextSwipeAfterReverse(weapon);
                        }
                    }),
                    new AnimationFrame(24, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                ]);

                break;

            case Game1.left:
                outFrames.AddRange([
                    new AnimationFrame(35, (int)(65 * modifier), true, flip: true, who =>
                    {
                        State.CurrentHitStep++;
                        farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(34, (int)(55 * modifier), true, flip: true, who =>
                    {
                        farmer.showSwordSwipe(who);
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(33, (int)(30 * halfModifier), true, flip: true, who =>
                    {
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        farmer.showSwordSwipe(who);
                    }),
                    new AnimationFrame(32, (int)(30 * halfModifier), true, flip: true, farmer.showSwordSwipe),
                    new AnimationFrame(31, (int)(30 * halfModifier), true, flip: true, farmer.showSwordSwipe),
                    new AnimationFrame(30, (int)(cooldown * modifier), true, flip: true, who =>
                    {
                        farmer.showSwordSwipe(who);
                        if (Config.SwipeHold && State.HoldingWeaponSwing)
                        {
                            who.QueueNextSwipeAfterReverse(weapon);
                        }
                    }),
                    new AnimationFrame(30, 0, true, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                ]);

                break;
        }

        Reflector
            .GetUnboundFieldSetter<FarmerSprite, int>("currentAnimationFrames")
            .Invoke(sprite, sprite.CurrentAnimation.Count);
        State.QueuedHitStep++;
        State.FarmerAnimating = true;
        Log.D("[Combo]: Queued Backslash");
    }

    internal static void QueueSmash(this Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var modifier = weapon.GetTotalSpeedModifier(farmer);
        var halfModifier = modifier / 2f;
        var windup = 120;
        var cooldown = 150;
        var sound = weapon.GetSwipeSound(State.CurrentHitStep);
        sprite.loopThisAnimation = false;
        var outFrames = sprite.currentAnimation;
        if (State.FarmerAnimating)
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
                outFrames.AddRange([
                    new AnimationFrame(36, (int)(windup * modifier), false, flip: false, who =>
                    {
                        State.CurrentHitStep++;
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
                ]);

                break;

            case Game1.right:
                outFrames.AddRange([
                    new AnimationFrame(48, (int)(windup * modifier), false, flip: false, who =>
                    {
                        State.CurrentHitStep++;
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
                ]);

                break;

            case Game1.down:
                outFrames.AddRange([
                    new AnimationFrame(66, (int)(windup * modifier), false, flip: false, who =>
                    {
                        State.CurrentHitStep++;
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
                ]);

                break;

            case Game1.left:
                outFrames.AddRange([
                    new AnimationFrame(48, (int)(windup * modifier), false, flip: true, who =>
                    {
                        State.CurrentHitStep++;
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
                ]);

                break;
        }

        Reflector
            .GetUnboundFieldSetter<FarmerSprite, int>("currentAnimationFrames")
            .Invoke(sprite, sprite.CurrentAnimation.Count);
        State.QueuedHitStep++;
        State.FarmerAnimating = true;
        Log.D("[Combo]: Queued Smash");
    }

    internal static void QueueThrust(this Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var modifier = weapon.GetTotalSpeedModifier(farmer);
        var halfModifier = modifier / 2f;
        var sound = weapon.GetSwipeSound(State.CurrentHitStep);
        sprite.loopThisAnimation = false;
        var outFrames = sprite.currentAnimation;
        if (State.FarmerAnimating)
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
                outFrames.AddRange([
                    new AnimationFrame(38, (int)(50 * halfModifier), true, flip: false, who =>
                    {
                        State.QueuedHitStep++;
                        who.DamageDuringThrust();
                    }),
                    new AnimationFrame(40, (int)(120 * halfModifier), true, flip: false, who =>
                    {
                        who.DamageDuringThrust();
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(38, (int)(50 * halfModifier), true, flip: false, DamageDuringThrust),
                    new AnimationFrame(38, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                ]);

                break;

            case Game1.right:
                outFrames.AddRange([
                    new AnimationFrame(33, (int)(50 * halfModifier), false, flip: false, who =>
                    {
                        State.QueuedHitStep++;
                        who.DamageDuringThrust();
                    }),
                    new AnimationFrame(34, (int)(120 * halfModifier), false, flip: false, who =>
                    {
                        who.DamageDuringThrust();
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(33, (int)(50 * halfModifier), false, flip: false, DamageDuringThrust),
                    new AnimationFrame(33, 0, false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                ]);

                break;

            case Game1.down:
                outFrames.AddRange([
                    new AnimationFrame(25, (int)(50 * halfModifier), true, flip: false, who =>
                    {
                        State.QueuedHitStep++;
                        who.DamageDuringThrust();
                    }),
                    new AnimationFrame(27, (int)(120 * halfModifier), true, flip: false, who =>
                    {
                        who.DamageDuringThrust();
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(25, (int)(50 * halfModifier), true, flip: false, DamageDuringThrust),
                    new AnimationFrame(25, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                ]);

                break;

            case Game1.left:
                outFrames.AddRange([
                    new AnimationFrame(33, (int)(50 * halfModifier), false, flip: true, who =>
                    {
                        State.QueuedHitStep++;
                        who.DamageDuringThrust();
                    }),
                    new AnimationFrame(34, (int)(120 * halfModifier), false, flip: true, who =>
                    {
                        who.DamageDuringThrust();
                        weapon.enchantments.OfType<BaseWeaponEnchantment>().ForEach(e => e.OnSwing(weapon, who));
                        who.currentLocation.localSound(sound);
                    }),
                    new AnimationFrame(33, (int)(50 * halfModifier), false, flip: true, DamageDuringThrust),
                    new AnimationFrame(33, 0, false, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                ]);

                break;
        }

        Reflector
            .GetUnboundFieldSetter<FarmerSprite, int>("currentAnimationFrames")
            .Invoke(sprite, sprite.CurrentAnimation.Count);
        State.QueuedHitStep++;
        State.FarmerAnimating = true;
        Log.D("[Combo]: Queued Thrust");
    }

    private static void QueueNextSwipeAfterForward(this Farmer farmer, MeleeWeapon weapon)
    {
        var hitStep = State.QueuedHitStep;
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
        var hitStep = State.QueuedHitStep;
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
