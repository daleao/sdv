namespace DaLion.Combat.Framework.Extensions;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Reflection;
using Microsoft.Xna.Framework;
using Shared.Constants;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="MeleeWeapon"/> class.</summary>
internal static class MeleeWeaponExtensions
{
    /// <summary>Gets the total swing speed modifier for the <paramref name="weapon"/>.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <param name="user">The <see cref="Farmer"/> using the weapon.</param>
    /// <returns>The total swing speed modifier, a number between 0 and 1, used for decreasing animation and cooldown times.</returns>
    /// <remarks>Smaller is faster.</remarks>
    internal static float GetTotalSpeedModifier(this MeleeWeapon weapon, Farmer? user = null)
    {
        user ??= weapon.getLastFarmerToUse();

        const double a = 0.2, b = 10.0, k = 2.0, m = 20.0;
        var tanh = Math.Tanh(((a * weapon.speed.Value) + (b * user.buffs.WeaponSpeedMultiplier)) / m);
        return (float)Math.Exp(-k * tanh);
    }

    /// <summary>Gets the name of the sound cue that should play when this weapon is swung.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/>.</param>
    /// <param name="hitStep">The current <see cref="ComboHitStep"/>.</param>
    /// <returns>The name of a sound cue to be played.</returns>
    internal static string GetSwipeSound(this MeleeWeapon weapon, ComboHitStep hitStep)
    {
        if (ModHelper.ModRegistry.IsLoaded("DaLion.Arsenal") &&
            weapon.QualifiedItemId == QualifiedWeaponIds.LavaKatana)
        {
            return "fireball";
        }

        return weapon.type.Value switch
        {
            MeleeWeapon.defenseSword or MeleeWeapon.stabbingSword when hitStep == ComboHitStep.FourthHit => "daggerswipe",
            MeleeWeapon.defenseSword or MeleeWeapon.stabbingSword => "swordswipe",
            MeleeWeapon.club => "clubswipe",
            MeleeWeapon.dagger => "daggerswipe",
        };
    }

    internal static ComboHitStep GetFinalHitStep(this MeleeWeapon weapon)
    {
        return ((WeaponType)weapon.type.Value).GetFinalHitStep();
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
        var swipeSpeed = (400f - (weapon.speed.Value * 40f)) * (1f - farmer.buffs.WeaponSpeedMultiplier);
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
