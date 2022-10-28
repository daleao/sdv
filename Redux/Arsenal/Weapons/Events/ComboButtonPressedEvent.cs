namespace DaLion.Redux.Arsenal.Weapons.Events;

#region using directives

using DaLion.Redux.Arsenal.Weapons.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using static StardewValley.FarmerSprite;

#endregion using directives

[UsedImplicitly]
internal sealed class ComboButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ComboButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ComboButtonPressedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ModEntry.Config.Arsenal.Weapons.ComboHits;

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        var player = Game1.player;
        if (!Context.IsWorldReady || Game1.activeClickableMenu is not null || !e.Button.IsUseToolButton() ||
            player.CurrentTool is not MeleeWeapon weapon || weapon.isScythe())
        {
            return;
        }

        if (ModEntry.State.Arsenal.ComboHitStep == ComboHitStep.Idle)
        {
            ++ModEntry.State.Arsenal.ComboHitStep;
            Log.D($"Doing {ModEntry.State.Arsenal.ComboHitStep}!");
            return;
        }

        if (ModEntry.State.Arsenal.ComboHitStep >= weapon.GetMaxComboHitCount() || ModEntry.State.Arsenal.WeaponSwingCooldown > 0)
        {
            ModEntry.ModHelper.Input.Suppress(e.Button);
            return;
        }

        var type = weapon.type.Value;
        switch (type)
        {
            case MeleeWeapon.club:
            {
                if ((int)ModEntry.State.Arsenal.ComboHitStep % 2 == 0)
                {
                    QueueForwardSwipe(player, weapon);
                }
                else
                {
                    QueueOverheadSwipe(player, weapon);
                }

                break;
            }

            case MeleeWeapon.stabbingSword or MeleeWeapon.defenseSword:
                if ((int)ModEntry.State.Arsenal.ComboHitStep % 2 == 0)
                {
                    QueueForwardSwipe(player, weapon);
                }
                else
                {
                    QueueBackwardSwipe(player, weapon);
                }

                break;
        }

        ++ModEntry.State.Arsenal.ComboHitStep;
        Log.D($"Doing {ModEntry.State.Arsenal.ComboHitStep}!");
    }

    private static void QueueBackwardSwipe(Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        if (sprite.CurrentAnimation is null)
        {
            Log.W("The combo was interrupted by another action.");
            return;
        }

        var multiplier = 10f / (10f + weapon.speed.Value) * (1f - farmer.weaponSpeedModifier);
        var cooldown = 800 / (weapon.type.Value == MeleeWeapon.club ? 5 : 8);
        switch (farmer.FacingDirection)
        {
            case Game1.up:
                sprite.currentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.currentAnimation.AddRange(new[]
                {
                    new AnimationFrame(41, (int)(55 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(40, (int)(45 * multiplier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound("swordswipe");
                    }),
                    new AnimationFrame(39, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(38, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(37, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(36, (int)(cooldown * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(36, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.right:
                sprite.currentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.currentAnimation.AddRange(new[]
                {
                    new AnimationFrame(35, (int)(55 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(34, (int)(45 * multiplier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound("swordswipe");
                    }),
                    new AnimationFrame(33, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(32, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(31, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(30, (int)(cooldown * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(30, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.down:
                sprite.currentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.currentAnimation.AddRange(new[]
                {
                    new AnimationFrame(29, (int)(55 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(28, (int)(45 * multiplier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound("swordswipe");
                    }),
                    new AnimationFrame(27, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(26, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(25, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(24, (int)(cooldown * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(24, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.left:
                sprite.currentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.currentAnimation.AddRange(new[]
                {
                    new AnimationFrame(35, (int)(55 * multiplier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(34, (int)(45 * multiplier), true, flip: true, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound("swordswipe");
                    }),
                    new AnimationFrame(33, (int)(25 * multiplier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(32, (int)(25 * multiplier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(31, (int)(25 * multiplier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(30, (int)(cooldown * multiplier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(30, 0, true, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;
        }

        ModEntry.Reflector
            .GetUnboundFieldSetter<FarmerSprite, int>(sprite, "currentAnimationFrames")
            .Invoke(sprite, sprite.CurrentAnimation.Count);
        Log.D("Queued a backslash!");
    }

    private static void QueueForwardSwipe(Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        if (sprite.CurrentAnimation is null)
        {
            Log.W("The combo was interrupted by another action.");
            return;
        }

        var multiplier = 10f / (10f + weapon.speed.Value) * (1f - farmer.weaponSpeedModifier);
        var cooldown = 800 / (weapon.type.Value == MeleeWeapon.club ? 5 : 8);
        switch (farmer.FacingDirection)
        {
            case Game1.up:
                sprite.currentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.currentAnimation.AddRange(new[]
                {
                    new AnimationFrame(36, (int)(55 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(37, (int)(45 * multiplier), true, flip: false,  who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound("swordswipe");
                    }),
                    new AnimationFrame(38, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(39, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(40, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(41, (int)(cooldown * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(41, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.right:
                sprite.currentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.currentAnimation.AddRange(new[]
                {
                    new AnimationFrame(30, (int)(55 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(31, (int)(45 * multiplier), true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound("swordswipe");
                    }),
                    new AnimationFrame(32, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(33, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(34, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(35, (int)(cooldown * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(35, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.down:
                sprite.currentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.currentAnimation.AddRange(new[]
                {
                    new AnimationFrame(24, (int)(55 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(25, (int)(45 * multiplier), true, flip: false,  who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound("swordswipe");
                    }),
                    new AnimationFrame(26, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(27, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(28, (int)(25 * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(29, (int)(cooldown * multiplier), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(29, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.left:
                sprite.currentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.currentAnimation.AddRange(new[]
                {
                    new AnimationFrame(30, (int)(55 * multiplier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(31, (int)(45 * multiplier), true, flip: true,  who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound("swordswipe");
                    }),
                    new AnimationFrame(32, (int)(25 * multiplier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(33, (int)(25 * multiplier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(34, (int)(25 * multiplier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(35, (int)(cooldown * multiplier), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(35, 0, true, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;
        }

        ModEntry.Reflector
            .GetUnboundFieldSetter<FarmerSprite, int>(sprite, "currentAnimationFrames")
            .Invoke(sprite, sprite.CurrentAnimation.Count);
        Log.D("Queued a forward slash!");
    }

    private static void QueueOverheadSwipe(Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        if (sprite.CurrentAnimation is null)
        {
            Log.W("The combo was interrupted by another action.");
            return;
        }

        var interval = 400 - (weapon.speed.Value * 40);
        interval *= (int)(1f - farmer.weaponSpeedModifier);
        interval /= weapon.type.Value == MeleeWeapon.club ? 5 : 8;
        switch (farmer.FacingDirection)
        {
            case Game1.up:
                sprite.currentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.currentAnimation.AddRange(new[]
                {
                    new AnimationFrame(36, 100, false, flip: false),
                    new AnimationFrame(37, 50, false, flip: false, Farmer.showToolSwipeEffect),
                    new AnimationFrame(38, 50, false, flip: false, Farmer.useTool),
                    new AnimationFrame(63, 50, false, flip: false, who => who.currentLocation.localSound("clubswipe")),
                    new AnimationFrame(62, interval * 3, false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.right:
                sprite.currentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.currentAnimation.AddRange(new[]
                {
                    new AnimationFrame(48, 100, false, flip: false),
                    new AnimationFrame(49, 50, false, flip: false, Farmer.showToolSwipeEffect),
                    new AnimationFrame(50, 50, false, flip: false, Farmer.useTool),
                    new AnimationFrame(51, 50, false, flip: false, who => who.currentLocation.localSound("clubswipe")),
                    new AnimationFrame(52, interval * 3, false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.down:
                sprite.currentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.currentAnimation.AddRange(new[]
                {
                    new AnimationFrame(66, 100, false, flip: false),
                    new AnimationFrame(67, 50, false, flip: false, Farmer.showToolSwipeEffect),
                    new AnimationFrame(68, 50, false, flip: false, Farmer.useTool),
                    new AnimationFrame(69, 50, false, flip: false, who => who.currentLocation.localSound("clubswipe")),
                    new AnimationFrame(70, interval * 3, false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.left:
                sprite.currentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.currentAnimation.AddRange(new[]
                {
                    new AnimationFrame(48, 100, false, flip: true),
                    new AnimationFrame(49, 50, false, flip: true, Farmer.showToolSwipeEffect),
                    new AnimationFrame(50, 50, false, flip: true, Farmer.useTool),
                    new AnimationFrame(51, 50, false, flip: true, who => who.currentLocation.localSound("clubswipe")),
                    new AnimationFrame(52, interval * 3, false, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;
        }

        ModEntry.Reflector
            .GetUnboundFieldSetter<FarmerSprite, int>(sprite, "currentAnimationFrames")
            .Invoke(sprite, sprite.CurrentAnimation.Count);
        Log.D("Queued a smash hit!");
    }
}
