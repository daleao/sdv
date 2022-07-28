namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common.Extensions.Reflection;
using Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Monsters;
using System;
using System.Linq;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeUpdatePatch : DaLion.Common.Harmony.HarmonyPatch
{
    private const int IMMUNE_TO_DAMAGE_DURATION_I = 450;

    private static readonly Lazy<Func<RockCrab, NetBool>> _GetShellGone = new(() =>
        typeof(RockCrab).RequireField("shellGone").CompileUnboundFieldGetterDelegate<RockCrab, NetBool>());

    /// <summary>Construct an instance.</summary>
    internal GreenSlimeUpdatePatch()
    {
        Target = RequireMethod<GreenSlime>(nameof(GreenSlime.update),
            new[] { typeof(GameTime), typeof(GameLocation) });
    }

    #region harmony patches

    /// <summary>Patch for Slimes to damage monsters around Piper.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeUpdatePostfix(GreenSlime __instance, GameTime time)
    {
        var pipeTimer = __instance.get_PipeTimer();
        if (pipeTimer.Value <= 0) return;

        pipeTimer.Value -= time.ElapsedGameTime.Milliseconds;
        foreach (var monster in __instance.currentLocation.characters.OfType<Monster>().Where(m => !m.IsSlime()))
        {
            var monsterBox = monster.GetBoundingBox();
            if (monster.IsInvisible || monster.isInvincible() ||
                monster.isGlider.Value && !(__instance.Scale > 1.8f || __instance.IsJumping()) ||
                !monsterBox.Intersects(__instance.GetBoundingBox()))
                continue;

            if (monster is Bug bug && bug.isArmoredBug.Value // skip Armored Bugs
                || monster is LavaCrab && __instance.Sprite.currentFrame % 4 == 0 // skip shelled Lava Crabs
                || monster is RockCrab crab && crab.Sprite.currentFrame % 4 == 0 && !_GetShellGone.Value(crab).Value // skip shelled Rock Crabs
                || monster is LavaLurk lurk &&
                lurk.currentState.Value == LavaLurk.State.Submerged // skip submerged Lava Lurks
                || monster is Spiker) // skip Spikers
                continue;

            // damage monster
            var damageToMonster = (int)Math.Max(1,
                                      (__instance.DamageToFarmer +
                                       Game1.random.Next(-__instance.DamageToFarmer / 4,
                                           __instance.DamageToFarmer / 4)) * __instance.Scale) -
                                  monster.resilience.Value;

            var (xTrajectory, yTrajectory) = monster.Slipperiness < 0
                ? Vector2.Zero
                : StardewValley.Utility.getAwayFromPositionTrajectory(monsterBox, __instance.getStandingPosition()) / 2f;
            monster.takeDamage(damageToMonster, (int)xTrajectory, (int)yTrajectory, false, 1.0, "slime");
            monster.currentLocation.debris.Add(new(damageToMonster,
                new(monsterBox.Center.X + 16, monsterBox.Center.Y), new(255, 130, 0), 1f, monster));
            monster.setInvincibleCountdown(IMMUNE_TO_DAMAGE_DURATION_I);

            // aggro monsters
            if (monster.get_Taunter() is null) monster.set_Taunter(__instance);

            var fakeFarmer = monster.get_FakeFarmer();
            if (fakeFarmer is not null) fakeFarmer.Position = __instance.Position;

            // get damaged by monster
            var damageToSlime = Math.Max(1,
                                    monster.DamageToFarmer + Game1.random.Next(-monster.DamageToFarmer / 4,
                                        monster.DamageToFarmer / 4)) -
                                __instance.resilience.Value;
            __instance.takeDamage(damageToSlime, (int)-xTrajectory, (int)-yTrajectory, false, 1.0, "slime");
            if (__instance.Health <= 0) break;
        }
    }

    #endregion harmony patches
}