namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using DaLion.Common.Enums;
using DaLion.Common.Events;
using DaLion.Common.Extensions.Reflection;
using DaLion.Stardew.Arsenal.Extensions;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using StardewValley.Tools;
using static StardewValley.FarmerSprite;

#endregion using directives

[UsedImplicitly]
internal sealed class ComboButtonPressedEvent : ButtonPressedEvent
{
    private static readonly Lazy<Func<MeleeWeapon, float>> GetSwipeSpeed = new(() =>
        typeof(MeleeWeapon)
            .RequireField("swipeSpeed")
            .CompileUnboundFieldGetterDelegate<MeleeWeapon, float>());

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
        if (!Context.IsWorldReady || !e.Button.IsUseToolButton() || player.CurrentTool is not MeleeWeapon weapon ||
            weapon.isScythe() || ModEntry.State.ComboHitStep == ComboHitStep.Idle)
        {
            return;
        }

        if (ModEntry.State.QueuedHits >= weapon.GetMaxComboHitCount() - 1 || ModEntry.State.WeaponSwingCooldown > 0)
        {
            ModEntry.ModHelper.Input.Suppress(e.Button);
            return;
        }

        var type = weapon.type.Value;
        switch (type)
        {
            case MeleeWeapon.club:
            {
                if (ModEntry.State.ComboHitStep == ComboHitStep.FirstHit)
                {
                    QueueOverheadSwipe();
                }

                break;
            }

            case MeleeWeapon.stabbingSword or MeleeWeapon.defenseSword:
                switch (ModEntry.State.ComboHitStep)
                {
                    case ComboHitStep.FirstHit or ComboHitStep.ThirdHit:
                        QueueBackwardSwipe(player, weapon);
                        break;

                    case ComboHitStep.SecondHit:
                        QueueForwardSwipe(player, weapon);
                        break;
                }

                break;
        }

        ++ModEntry.State.QueuedHits;
    }

    private static void QueueBackwardSwipe(Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        switch (farmer.FacingDirection)
        {
            case Game1.up:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(41, 55, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(40, 45, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(39, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(38, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(37, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(36, (short)(GetSwipeSpeed.Value(weapon) * 2.6), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(36, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.right:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(35, 55, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(34, 45, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(33, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(32, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(31, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(30, (short)(GetSwipeSpeed.Value(weapon) * 2.6), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(30, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.down:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(29, 55, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(28, 45, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(27, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(26, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(25, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(24, (short)(GetSwipeSpeed.Value(weapon) * 2.6), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(24, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.left:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(35, 55, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(34, 45, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(33, 25, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(32, 25, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(31, 25, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(30, (short)(GetSwipeSpeed.Value(weapon) * 2.6), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(30, 0, true, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;
        }

        SetCurrentAnimationFrames.Value(sprite, sprite.CurrentAnimation.Count);
    }

    private static void QueueForwardSwipe(Farmer farmer, MeleeWeapon weapon)
    {
        var sprite = farmer.FarmerSprite;
        switch (farmer.FacingDirection)
        {
            case Game1.up:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(36, 55, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(37, 45, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(38, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(39, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(40, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(41, (short)(GetSwipeSpeed.Value(weapon) * 2.6), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(41, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.right:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(30, 55, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(31, 45, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(32, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(33, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(34, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(35, (short)(GetSwipeSpeed.Value(weapon) * 2.6), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(35, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.down:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(24, 55, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(25, 45, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(26, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(27, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(28, 25, true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(29, (short)(GetSwipeSpeed.Value(weapon) * 2.6), true, flip: false, Farmer.showSwordSwipe),
                    new AnimationFrame(29, 0, true, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;

            case Game1.left:
                sprite.CurrentAnimation.RemoveAt(sprite.CurrentAnimation.Count - 1);
                sprite.CurrentAnimation.AddRange(new[]
                {
                    new AnimationFrame(30, 55, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(31, 45, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(32, 25, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(33, 25, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(34, 25, true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(35, (short)(GetSwipeSpeed.Value(weapon) * 2.6), true, flip: true, Farmer.showSwordSwipe),
                    new AnimationFrame(35, 0, true, flip: true, Farmer.canMoveNow, behaviorAtEndOfFrame: true),
                });

                break;
        }

        SetCurrentAnimationFrames.Value(sprite, sprite.CurrentAnimation.Count);
    }

    private static void QueueOverheadSwipe()
    {

    }
}
