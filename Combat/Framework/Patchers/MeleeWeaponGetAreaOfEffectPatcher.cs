// ReSharper disable PossibleLossOfFraction
namespace DaLion.Combat.Framework.Patchers;

#region using directives

using System.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponGetAreaOfEffectPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponGetAreaOfEffectPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MeleeWeaponGetAreaOfEffectPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.getAreaOfEffect));
    }

    #region harmony patches

    /// <summary>Fix combo swipe and Stabbing Sword lunge hitboxes.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool MeleeWeaponGetAreaOfEffectPrefix(
        MeleeWeapon __instance,
        ref Rectangle __result,
        int x,
        int y,
        int facingDirection,
        ref Vector2 tileLocation1,
        ref Vector2 tileLocation2,
        Rectangle wielderBoundingBox)
    {
        if (__instance.type.Value == MeleeWeapon.dagger)
        {
            // daggers don't swipe, so they don't need correction
            return true; // run original logic
        }

        try
        {
            var frameOfFarmerAnimation = Game1.player.FarmerSprite.CurrentFrame;
            __result = __instance.type.Value == MeleeWeapon.stabbingSword && __instance.isOnSpecial
                ? GetHitboxDuringLunge(
                    x,
                    y,
                    facingDirection,
                    ref tileLocation1,
                    ref tileLocation2,
                    wielderBoundingBox)
                : GetHitboxDuringSwipe(
                    x,
                    y,
                    facingDirection,
                    ref tileLocation1,
                    ref tileLocation2,
                    wielderBoundingBox,
                    frameOfFarmerAnimation);

            __result.Inflate(__instance.addedAreaOfEffect.Value, __instance.addedAreaOfEffect.Value);
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    /// <summary>More generous club aoe during combo smash.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void MeleeWeaponGetAreaOfEffectPostfix(MeleeWeapon __instance, ref Rectangle __result)
    {
        if (__instance.IsClub() && State.CurrentHitStep == __instance.GetFinalHitStep())
        {
            __result.Inflate(32, 32);
        }
    }

    #endregion harmony patches

    #region subroutines

    private static Rectangle GetHitboxDuringLunge(
        int x,
        int y,
        int facingDirection,
        ref Vector2 tileLocation1,
        ref Vector2 tileLocation2,
        Rectangle wielderBoundingBox)
    {
        const int width = 74;
        const int height = 64;
        const int upHeightOffset = 42;
        const int horizontalYOffset = -32;
        var hitbox = Rectangle.Empty;
        switch (facingDirection)
        {
            case Game1.up:
                hitbox = new Rectangle(
                    x - (width / 2),
                    wielderBoundingBox.Y - height - upHeightOffset,
                    width / 2,
                    height + upHeightOffset);
                tileLocation1 = new Vector2(
                    (Game1.random.NextDouble() < 0.5 ? hitbox.Left : hitbox.Right) / 64,
                    hitbox.Top / 64);
                tileLocation2 = new Vector2(hitbox.Center.X / 64, hitbox.Top / 64);
                hitbox.Offset(20, -16);
                hitbox.Height += 16;
                hitbox.Width += 20;
                break;
            case Game1.right:
                hitbox = new Rectangle(
                    wielderBoundingBox.Right,
                    y - (height / 2) + horizontalYOffset,
                    height,
                    width);
                tileLocation1 = new Vector2(
                    hitbox.Center.X / 64,
                    (Game1.random.NextDouble() < 0.5 ? hitbox.Top : hitbox.Bottom) / 64);
                tileLocation2 = new Vector2(hitbox.Center.X / 64, hitbox.Center.Y / 64);
                hitbox.Offset(-4, 0);
                hitbox.Width += 16;
                break;
            case Game1.down:
                hitbox = new Rectangle(
                    x - (width / 2),
                    wielderBoundingBox.Bottom,
                    width,
                    height);
                tileLocation1 = new Vector2(
                    (Game1.random.NextDouble() < 0.5 ? hitbox.Left : hitbox.Right) / 64,
                    hitbox.Center.Y / 64);
                tileLocation2 = new Vector2(hitbox.Center.X / 64, hitbox.Center.Y / 64);
                hitbox.Offset(12, -8);
                hitbox.Width -= 21;
                break;
            case Game1.left:
                hitbox = new Rectangle(
                    wielderBoundingBox.Left - height,
                    y - (height / 2) + horizontalYOffset,
                    height,
                    width);
                tileLocation1 = new Vector2(
                    hitbox.Left / 64,
                    (Game1.random.NextDouble() < 0.5 ? hitbox.Top : hitbox.Bottom) / 64);
                tileLocation2 = new Vector2(hitbox.Left / 64, hitbox.Center.Y / 64);
                hitbox.Offset(-12, 0);
                hitbox.Width += 16;
                break;
        }

        return hitbox;
    }

    private static Rectangle GetHitboxDuringSwipe(
        int x,
        int y,
        int facingDirection,
        ref Vector2 tileLocation1,
        ref Vector2 tileLocation2,
        Rectangle wielderBoundingBox,
        int frameOfFarmerAnimation)
    {
        const int width = 64;
        const int height = 64;
        const int horizontalYOffset = -32;
        const int upHeightOffset = 0;
        var hitbox = Rectangle.Empty;
        switch (facingDirection)
        {
            case Game1.up:
                hitbox = new Rectangle(
                    x - (width / 2),
                    wielderBoundingBox.Y - height - upHeightOffset,
                    width,
                    height + upHeightOffset);
                tileLocation1 = new Vector2(
                    (Game1.random.NextDouble() < 0.5 ? hitbox.Left : hitbox.Right) / 64,
                    hitbox.Top / 64);
                tileLocation2 = new Vector2(hitbox.Center.X / 64, hitbox.Top / 64);
                switch (frameOfFarmerAnimation)
                {
                    case 41:
                        hitbox.Offset(76, -32);
                        break;
                    case 40:
                        hitbox.Offset(56, -32);
                        hitbox.Height += 32;
                        break;
                    case 39:
                        hitbox.Offset(40, -60);
                        hitbox.Height += 48;
                        break;
                    case 38:
                        hitbox.Offset(-12, -68);
                        hitbox.Height += 48;
                        break;
                    case 37:
                        hitbox.Offset(-48, -56);
                        hitbox.Height += 32;
                        break;
                    case 36:
                        hitbox.Offset(-60, -12);
                        break;
                }

                break;
            case Game1.right:
                hitbox = new Rectangle(
                    wielderBoundingBox.Right,
                    y - (height / 2) + horizontalYOffset,
                    height,
                    width);
                tileLocation1 = new Vector2(
                    hitbox.Center.X / 64,
                    (Game1.random.NextDouble() < 0.5 ? hitbox.Top : hitbox.Bottom) / 64);
                tileLocation2 = new Vector2(hitbox.Center.X / 64, hitbox.Center.Y / 64);
                switch (frameOfFarmerAnimation)
                {
                    case 30:
                        hitbox.Offset(-44, -84);
                        break;
                    case 31:
                        hitbox.Offset(4, -44);
                        break;
                    case 32:
                        hitbox.Offset(12, -4);
                        break;
                    case 33:
                        hitbox.Offset(12, 37);
                        break;
                    case 34:
                        hitbox.Offset(-28, 60);
                        break;
                    case 35:
                        hitbox.Offset(-60, 72);
                        break;
                }

                break;
            case Game1.down:
                hitbox = new Rectangle(x - (width / 2), wielderBoundingBox.Bottom, width, height);
                tileLocation1 = new Vector2(
                    (Game1.random.NextDouble() < 0.5 ? hitbox.Left : hitbox.Right) / 64,
                    hitbox.Center.Y / 64);
                tileLocation2 = new Vector2(hitbox.Center.X / 64, hitbox.Center.Y / 64);
                switch (frameOfFarmerAnimation)
                {
                    case 24:
                        hitbox.Offset(72, -92);
                        break;
                    case 25:
                        hitbox.Offset(56, -32);
                        break;
                    case 26:
                        hitbox.Offset(40, -28);
                        break;
                    case 27:
                        hitbox.Offset(-12, -8);
                        break;
                    case 28:
                        hitbox.Offset(-80, -24);
                        hitbox.Width += 32;
                        break;
                    case 29:
                        hitbox.Offset(-68, -44);
                        break;
                }

                break;
            case Game1.left:
                hitbox = new Rectangle(
                    wielderBoundingBox.Left - height,
                    y - (height / 2) + horizontalYOffset,
                    height,
                    width);
                tileLocation1 = new Vector2(
                    hitbox.Left / 64,
                    (Game1.random.NextDouble() < 0.5 ? hitbox.Top : hitbox.Bottom) / 64);
                tileLocation2 = new Vector2(hitbox.Left / 64, hitbox.Center.Y / 64);
                switch (frameOfFarmerAnimation)
                {
                    case 30:
                        hitbox.Offset(56, -76);
                        break;
                    case 31:
                        hitbox.Offset(-8, -56);
                        break;
                    case 32:
                        hitbox.Offset(-16, -4);
                        break;
                    case 33:
                        hitbox.Offset(0, 37);
                        break;
                    case 34:
                        hitbox.Offset(24, 60);
                        break;
                    case 35:
                        hitbox.Offset(64, 64);
                        break;
                }

                break;
        }

        return hitbox;
    }

    #endregion subroutines
}
