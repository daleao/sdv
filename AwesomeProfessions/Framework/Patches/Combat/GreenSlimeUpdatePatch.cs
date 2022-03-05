namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Monsters;

using Stardew.Common.Extensions;
using Extensions;
using SUtility = StardewValley.Utility;

#endregion using directives

[UsedImplicitly]
internal class GreenSlimeUpdatePatch : BasePatch
{
    private const int IMMUNE_TO_DAMAGE_DURATION_I = 450;

    private static readonly FieldInfo _ShellGone = typeof(RockCrab).Field("shellGone");

    /// <summary>Construct an instance.</summary>
    internal GreenSlimeUpdatePatch()
    {
        Original = RequireMethod<GreenSlime>(nameof(GreenSlime.update),
            new[] {typeof(GameTime), typeof(GameLocation)});
    }

    #region harmony patches

    /// <summary>Patch for Slimes to damage monsters around Piper.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeUpdatePostfix(GreenSlime __instance)
    {
        if (!ModEntry.PlayerState.Value.PipedSlimes.Contains(__instance)) return;

        foreach (var monster in __instance.currentLocation.characters.OfType<Monster>().Where(m => !m.IsSlime()))
        {
            var monsterBox = monster.GetBoundingBox();
            if (monster.IsInvisible || monster.isInvincible() ||
                monster.isGlider.Value && !(__instance.Scale > 1.8f || __instance.IsJumping()) ||
                !monsterBox.Intersects(__instance.GetBoundingBox()))
                continue;

            if (monster is Bug bug && bug.isArmoredBug.Value // skip Armored Bugs
                || monster is LavaCrab && __instance.Sprite.currentFrame % 4 == 0 // skip shelled Lava Crabs
                || monster is RockCrab crab && crab.Sprite.currentFrame % 4 == 0 &&
                !((NetBool) _ShellGone.GetValue(crab))!.Value // skip shelled Rock Crabs
                || monster is LavaLurk lurk &&
                lurk.currentState.Value == LavaLurk.State.Submerged // skip submerged Lava Lurks
                || monster is Spiker) // skip Spikers
                continue;

            // damage monster
            var damageToMonster = (int) Math.Max(1,
                (__instance.DamageToFarmer +
                 Game1.random.Next(-__instance.DamageToFarmer / 4, __instance.DamageToFarmer / 4)) * __instance.Scale) - monster.resilience.Value;

            var (xTrajectory, yTrajectory) = monster.Slipperiness < 0
                ? Vector2.Zero
                : SUtility.getAwayFromPositionTrajectory(monsterBox, __instance.getStandingPosition()) / 2f;
            monster.takeDamage(damageToMonster, (int) xTrajectory, (int) yTrajectory, false, 1.0, "slime");
            monster.currentLocation.debris.Add(new(damageToMonster,
                new(monsterBox.Center.X + 16, monsterBox.Center.Y), new(255, 130, 0), 1f, monster));
            monster.setInvincibleCountdown(IMMUNE_TO_DAMAGE_DURATION_I);

            // aggro monsters
            var fakeFarmerId = monster.GetHashCode();
            if (!ModEntry.HostState.FakeFarmers.TryGetValue(fakeFarmerId, out var fakeFarmer))
            {
                fakeFarmer = ModEntry.HostState.FakeFarmers[fakeFarmerId] =
                    new() {UniqueMultiplayerID = fakeFarmerId, currentLocation = __instance.currentLocation};
                Log.D($"Created fake farmer with id {fakeFarmerId}.");
            }
            fakeFarmer.Position = __instance.Position;
            monster.WriteData("Aggroed", true.ToString());
            monster.WriteData("Aggroer", __instance.GetHashCode().ToString());

            // get damaged by monster
            var damageToSlime = Math.Max(1,
                monster.DamageToFarmer + Game1.random.Next(-monster.DamageToFarmer / 4, monster.DamageToFarmer / 4)) - __instance.resilience.Value;
            __instance.takeDamage(damageToSlime, (int) -xTrajectory, (int) -yTrajectory, false, 1.0, "slime");
            if (__instance.Health <= 0) break;
        }
    }

    #endregion harmony patches
}