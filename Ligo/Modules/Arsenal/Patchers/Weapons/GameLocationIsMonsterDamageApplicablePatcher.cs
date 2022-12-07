namespace DaLion.Ligo.Modules.Arsenal.Patchers.Weapons;

#region using directives

using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationIsMonsterDamageApplicablePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationIsMonsterDamageApplicablePatcher"/> class.</summary>
    internal GameLocationIsMonsterDamageApplicablePatcher()
    {
        this.Target = this.RequireMethod<GameLocation>("isMonsterDamageApplicable");
    }

    #region harmony patches

    /// <summary>Club smash aoe ignores gliders.</summary>
    [HarmonyPrefix]
    private static bool GameLocationIsMonsterDamageApplicablePrefix(
        ref bool __result, Farmer who, Monster monster)
    {
        if (!ModEntry.Config.Arsenal.Weapons.GroundedClubSmash || (!monster.isGlider.Value && monster is not Bug) ||
            who.CurrentTool is not MeleeWeapon { type.Value: MeleeWeapon.club, isOnSpecial: true } club)
        {
            return true; // run original logic
        }

        try
        {
            var (x, y) = who.getUniformPositionAwayFromBox(who.FacingDirection, 64);
            var tileLocation1 = Vector2.Zero;
            var tileLocation2 = Vector2.Zero;
            if (monster.TakesDamageFromHitbox(
                    club.getAreaOfEffect(
                        (int)x,
                        (int)y,
                        who.FacingDirection,
                        ref tileLocation1,
                        ref tileLocation2,
                        who.GetBoundingBox(),
                        who.FarmerSprite.currentAnimationIndex)))
            {
                return true; // run original logic
            }

            __result = false;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
