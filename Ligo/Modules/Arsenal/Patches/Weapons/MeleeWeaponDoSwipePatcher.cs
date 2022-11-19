namespace DaLion.Ligo.Modules.Arsenal.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Xna.Framework;
using Shared.Enums;
using Shared.Extensions.Reflection;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoSwipePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDoSwipePatcher"/> class.</summary>
    internal MeleeWeaponDoSwipePatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.doSwipe));
    }

    #region harmony patches

    /// <summary>Allows swiping stabbing sword + removes redundant code.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponDoSwipePrefix(
        MeleeWeapon __instance,
        int type,
        float swipeSpeed,
        Farmer? f)
    {
        if (f is null || f.CurrentTool != __instance)
        {
            return false; // don't run original logic
        }

        if (f.IsLocalPlayer)
        {
            f.TemporaryPassableTiles.Clear();
            f.currentLocation.lastTouchActionLocation = Vector2.Zero;
        }

        switch (type)
        {
            case MeleeWeapon.stabbingSword:
            case MeleeWeapon.defenseSword:
                switch (f.FacingDirection)
                {
                    case Game1.up:
                        f.FarmerSprite.animateOnce(248, swipeSpeed, 6);
                        __instance.Update(0, 0, f);
                        break;
                    case Game1.right:
                        f.FarmerSprite.animateOnce(240, swipeSpeed, 6);
                        __instance.Update(1, 0, f);
                        break;
                    case Game1.down:
                        f.FarmerSprite.animateOnce(232, swipeSpeed, 6);
                        __instance.Update(2, 0, f);
                        break;
                    case Game1.left:
                        f.FarmerSprite.animateOnce(256, swipeSpeed, 6);
                        __instance.Update(3, 0, f);
                        break;
                }

                break;
            case MeleeWeapon.club:
                switch (f.FacingDirection)
                {
                    case Game1.up:
                        f.FarmerSprite.animateOnce(248, swipeSpeed, 8);
                        __instance.Update(0, 0, f);
                        break;
                    case Game1.right:
                        f.FarmerSprite.animateOnce(240, swipeSpeed, 8);
                        __instance.Update(1, 0, f);
                        break;
                    case Game1.down:
                        f.FarmerSprite.animateOnce(232, swipeSpeed, 8);
                        __instance.Update(2, 0, f);
                        break;
                    case Game1.left:
                        f.FarmerSprite.animateOnce(256, swipeSpeed, 8);
                        __instance.Update(3, 0, f);
                        break;
                }

                break;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
