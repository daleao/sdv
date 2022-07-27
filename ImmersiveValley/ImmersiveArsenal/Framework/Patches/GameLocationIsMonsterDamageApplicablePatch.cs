namespace DaLion.Stardew.Arsenal.Framework.Patches.Combat;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationIsMonsterDamageApplicablePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationIsMonsterDamageApplicablePatch()
    {
        Target = RequireMethod<GameLocation>("isMonsterDamageApplicable");
    }

    #region harmony patches

    /// <summary>Club smash aoe ignores gliders.</summary>
    [HarmonyPrefix]
    private static bool GameLocationIsMonsterDamageApplicablePrefix(ref bool __result,
        Farmer who, Monster monster)
    {
        if (!ModEntry.Config.ImmersiveClubSmash || !monster.isGlider.Value && monster is not Bug ||
            who.CurrentTool is not MeleeWeapon { type.Value: MeleeWeapon.club, isOnSpecial: true } club)
            return true; // run original logic

        var (x, y) = who.getUniformPositionAwayFromBox(who.FacingDirection, 64);
        var tileLocation1 = Vector2.Zero;
        var tileLocation2 = Vector2.Zero;
        if (monster.TakesDamageFromHitbox(club.getAreaOfEffect((int)x, (int)y, who.FacingDirection, ref tileLocation1,
                ref tileLocation2, who.GetBoundingBox(), who.FarmerSprite.currentAnimationIndex)))
            return true; // run original logic

        __result = false;
        return false; // don't run original logic
    }

    #endregion harmony patches
}