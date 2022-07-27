// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponGetAreaOfEffectPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponGetAreaOfEffectPatch()
    {
        Target = RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.getAreaOfEffect));
    }

    #region harmony patches

    /// <summary>Fix stabby sword hitbox during lunge.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponGetAreaOfEffectPrefix(MeleeWeapon __instance, ref Rectangle __result, int x, int y,
        int facingDirection, ref Vector2 tileLocation1, ref Vector2 tileLocation2, Rectangle wielderBoundingBox)
    {
        if (__instance.type.Value != MeleeWeapon.stabbingSword || !__instance.isOnSpecial)
            return true; // run original logic

        const int width = 74;
        const int height = 64;
        const int upHeightOffset = 42;
        const int horizontalYOffset = -32;
        switch (facingDirection)
        {
            case Game1.up:
                __result = new(x - width / 2, wielderBoundingBox.Y - height - upHeightOffset, width / 2,
                    height + upHeightOffset);
                tileLocation1 = new((Game1.random.NextDouble() < 0.5 ? __result.Left : __result.Right) / 64,
                    __result.Top / 64);
                tileLocation2 = new(__result.Center.X / 64, __result.Top / 64);
                __result.Offset(20, -16);
                __result.Height += 16;
                __result.Width += 20;
                break;
            case Game1.right:
                __result = new(wielderBoundingBox.Right, y - height / 2 + horizontalYOffset, height, width);
                tileLocation1 = new(__result.Center.X / 64,
                    (Game1.random.NextDouble() < 0.5 ? __result.Top : __result.Bottom) / 64);
                tileLocation2 = new(__result.Center.X / 64, __result.Center.Y / 64);
                __result.Offset(-4, 0);
                __result.Width += 16;
                break;
            case Game1.down:
                __result = new(x - width / 2, wielderBoundingBox.Bottom, width, height);
                tileLocation1 = new((Game1.random.NextDouble() < 0.5 ? __result.Left : __result.Right) / 64,
                    __result.Center.Y / 64);
                tileLocation2 = new(__result.Center.X / 64, __result.Center.Y / 64);
                __result.Offset(12, -8);
                __result.Width -= 21;
                break;
            case Game1.left:
                __result = new(wielderBoundingBox.Left - height, y - height / 2 + horizontalYOffset, height, width);
                tileLocation1 = new(__result.Left / 64,
                    (Game1.random.NextDouble() < 0.5 ? __result.Top : __result.Bottom) / 64);
                tileLocation2 = new(__result.Left / 64, __result.Center.Y / 64);
                __result.Offset(-12, 0);
                __result.Width += 16;
                break;
        }

        __result.Inflate(__instance.addedAreaOfEffect.Value, __instance.addedAreaOfEffect.Value);
        return false; // don't run original logic
    }

    #endregion harmony patches
}