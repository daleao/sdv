using DaLion.Stardew.Professions.Framework.SuperMode;

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Monsters;

using Extensions;

using SUtility = StardewValley.Utility;

#endregion using directives

[UsedImplicitly]
internal class GreenSlimeUpdatePatch : BasePatch
{
    private const int INITIAL_INVINCIBILITY_TIMER_I = 1200;

    /// <summary>Construct an instance.</summary>
    internal GreenSlimeUpdatePatch()
    {
        Original = RequireMethod<GreenSlime>(nameof(GreenSlime.update),
            new[] {typeof(GameTime), typeof(GameLocation)});
    }

    #region harmony patches

    /// <summary>Patch for Slimes to damage monsters around Piper.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeUpdatePostfix(GreenSlime __instance, GameLocation location)
    {
        if (!location.DoesAnyPlayerHereHaveProfession(Profession.Piper, out var pipers)) return;

        var closestPiper = __instance.GetClosestCharacter(out _, pipers);
        foreach (var monster in __instance.currentLocation.characters.OfType<Monster>().Where(m => !m.IsSlime()))
        {
            var monsterBox = monster.GetBoundingBox();
            if (monster.IsInvisible || monster.isInvincible() ||
                monster.isGlider.Value && !(__instance.Scale > 1.8f || __instance.IsJumping()) ||
                !monsterBox.Intersects(__instance.GetBoundingBox()))
                continue;

            if (monster is Bug bug && bug.isArmoredBug.Value // skip Armored Bugs
                || monster is LavaCrab && __instance.Sprite.currentFrame % 4 == 0 // skip shelled Lava Crabs
                || monster is RockCrab crab && crab.Sprite.currentFrame % 4 == 0 && !ModEntry.ModHelper
                    .Reflection.GetField<NetBool>(crab, "shellGone").GetValue().Value // skip shelled Rock Crabs
                || monster is LavaLurk lurk &&
                lurk.currentState.Value == LavaLurk.State.Submerged // skip submerged Lava Lurks
                || monster is Spiker) // skip Spikers
                continue;

            var damageToMonster = Math.Max(1,
                (__instance.DamageToFarmer +
                 Game1.random.Next(-__instance.DamageToFarmer / 4, __instance.DamageToFarmer / 4)) * __instance.Scale);

            var (xTrajectory, yTrajectory) = monster.Slipperiness < 0
                ? Vector2.Zero
                : SUtility.getAwayFromPositionTrajectory(monsterBox, __instance.getStandingPosition()) / 2f;
            monster.takeDamage((int) damageToMonster, (int) xTrajectory, (int) yTrajectory, false, 1.0, "slime");
            monster.currentLocation.debris.Add(new((int) damageToMonster,
                new(monsterBox.Center.X + 16, monsterBox.Center.Y), new(255, 130, 0), 1f, monster));

            var invincibleCountdown = INITIAL_INVINCIBILITY_TIMER_I;
            if (ModEntry.State.Value.SuperMode is PiperEubstance eubstance)
                invincibleCountdown = (int) (invincibleCountdown * (1f - eubstance.GetBonusSlimeAttackSpeed() / 2f));
            
            monster.setInvincibleCountdown(invincibleCountdown);
        }
    }

    #endregion harmony patches
}