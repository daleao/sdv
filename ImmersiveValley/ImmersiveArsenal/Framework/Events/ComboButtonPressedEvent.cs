namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using DaLion.Common.Events;
using DaLion.Common.Extensions.Reflection;
using DaLion.Stardew.Arsenal.Extensions;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using static StardewValley.FarmerSprite;

#endregion using directives

[UsedImplicitly]
internal sealed class ComboButtonPressedEvent : ButtonPressedEvent
{
    private static readonly Lazy<Action<FarmerSprite, int>> SetCurrentAnimationFrames = new(() =>
        typeof(FarmerSprite)
            .RequireField("currentAnimationFrames")
            .CompileUnboundFieldSetterDelegate<FarmerSprite, int>());

    /// <summary>Initializes a new instance of the <see cref="ComboButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ComboButtonPressedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        var player = Game1.player;
        if (!Context.IsWorldReady || Game1.activeClickableMenu is not null || !e.Button.IsUseToolButton() ||
            player.CurrentTool is not MeleeWeapon weapon || weapon.isScythe())
        {
            return;
        }

        if (ModEntry.State.ComboHitStep == ComboHitStep.Idle)
        {
            ++ModEntry.State.ComboHitStep;
            Log.D($"Doing {ModEntry.State.ComboHitStep}!");
            return;
        }

        if (ModEntry.State.ComboHitStep >= weapon.GetMaxComboHitCount() || ModEntry.State.WeaponSwingCooldown > 0)
        {
            ModEntry.ModHelper.Input.Suppress(e.Button);
            return;
        }

        var type = weapon.type.Value;
        switch (type)
        {
            case MeleeWeapon.club:
            {
                if ((int)ModEntry.State.ComboHitStep % 2 == 0)
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
                if ((int)ModEntry.State.ComboHitStep % 2 == 0)
                {
                    QueueForwardSwipe(player, weapon);
                }
                else
                {
                    QueueBackwardSwipe(player, weapon);
                }

                break;
        }

        ++ModEntry.State.ComboHitStep;
        Log.D($"Doing {ModEntry.State.ComboHitStep}!");
    }

    private static void QueueBackwardSwipe(Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var interval = 400 - (weapon.speed.Value * 40);
        interval *= (int)(1f - farmer.weaponSpeedModifier);
        interval /= weapon.type.Value == MeleeWeapon.club ? 5 : 8;
        switch (farmer.FacingDirection)
        {
            case Game1.up:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(41, 55, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(40, 45, true, flip: false,who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound("swordswipe");
                    }),
                    new AnimationFrame(39, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(38, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(37, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(36, interval * 2, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(36, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.right:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(35, 55, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(34, 45, true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound("swordswipe");
                    }),
                    new AnimationFrame(33, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(32, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(31, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(30, interval * 2, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(30, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.down:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(29, 55, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(28, 45, true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound("swordswipe");
                    }),
                    new AnimationFrame(27, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(26, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(25, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(24, interval * 2, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(24, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.left:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(35, 55, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(34, 45, true, flip: true, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound("swordswipe");
                    }),
                    new AnimationFrame(33, 25, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(32, 25, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(31, 25, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(30, interval * 2, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(30, 0, true, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;
        }

        SetCurrentAnimationFrames.Value(sprite, sprite.CurrentAnimation.Count);
        Log.D("Queued a backslash!");
    }

    private static void QueueForwardSwipe(Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var interval = 400 - (weapon.speed.Value * 40);
        interval *= (int)(1f - farmer.weaponSpeedModifier);
        interval /= weapon.type.Value == MeleeWeapon.club ? 5 : 8;
        switch (farmer.FacingDirection)
        {
            case Game1.up:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(36, 55, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(37, 45, true, flip: false,  who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound("swordswipe");
                    }),
                    new AnimationFrame(38, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(39, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(40, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(41, interval * 2, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(41, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.right:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(30, 55, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(31, 45, true, flip: false, who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound("swordswipe");
                    }),
                    new AnimationFrame(32, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(33, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(34, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(35, interval * 2, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(35, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.down:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(24, 55, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(25, 45, true, flip: false,  who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound("swordswipe");
                    }),
                    new AnimationFrame(26, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(27, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(28, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(29, interval * 2, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(29, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.left:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(30, 55, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(31, 45, true, flip: true,  who =>
                    {
                        Farmer.showSwordSwipe(who);
                        who.currentLocation.localSound("swordswipe");
                    }),
                    new AnimationFrame(32, 25, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(33, 25, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(34, 25, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(35, interval * 2, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(35, 0, true, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;
        }

        SetCurrentAnimationFrames.Value(sprite, sprite.CurrentAnimation.Count);
        Log.D("Queued a forward slash!");
    }

    private static void QueueOverheadSwipe(Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        var interval = 400 - (weapon.speed.Value * 40);
        interval *= (int)(1f - farmer.weaponSpeedModifier);
        interval /= weapon.type.Value == MeleeWeapon.club ? 5 : 8;
        switch (farmer.FacingDirection)
        {
            case Game1.up:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(36, 100, false, flip: false),
                    new AnimationFrame(37, 50, false, flip: false, Farmer.showToolSwipeEffect),
                    new AnimationFrame(38, 50, false, flip: false, Farmer.useTool),
                    new AnimationFrame(63, 50, false, flip: false, who => who.currentLocation.localSound("clubswipe")),
                    new AnimationFrame(62, interval * 3, false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.right:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(48, 100, false, flip: false),
                    new AnimationFrame(49, 50, false, flip: false, Farmer.showToolSwipeEffect),
                    new AnimationFrame(50, 50, false, flip: false, Farmer.useTool),
                    new AnimationFrame(51, 50, false, flip: false, who => who.currentLocation.localSound("clubswipe")),
                    new AnimationFrame(52, interval * 3, false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.down:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(66, 100, false, flip: false),
                    new AnimationFrame(67, 50, false, flip: false, Farmer.showToolSwipeEffect),
                    new AnimationFrame(68, 50, false, flip: false, Farmer.useTool),
                    new AnimationFrame(69, 50, false, flip: false, who => who.currentLocation.localSound("clubswipe")),
                    new AnimationFrame(70, interval * 3, false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.left:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(48, 100, false, flip: true),
                    new AnimationFrame(49, 50, false, flip: true, Farmer.showToolSwipeEffect),
                    new AnimationFrame(50, 50, false, flip: true, Farmer.useTool),
                    new AnimationFrame(51, 50, false, flip: true, who => who.currentLocation.localSound("clubswipe")),
                    new AnimationFrame(52, interval * 3, false, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;
        }

        SetCurrentAnimationFrames.Value(sprite, sprite.CurrentAnimation.Count);
        Log.D("Queued a smash hit!");
    }
}
