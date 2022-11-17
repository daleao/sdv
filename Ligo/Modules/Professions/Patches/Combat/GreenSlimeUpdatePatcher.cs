namespace DaLion.Ligo.Modules.Professions.Patches.Combat;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Professions.Ultimates;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeUpdatePatcher"/> class.</summary>
    internal GreenSlimeUpdatePatcher()
    {
        this.Target = this.RequireMethod<GreenSlime>(
            nameof(GreenSlime.update), new[] { typeof(GameTime), typeof(GameLocation) });
    }

    #region harmony patches

    /// <summary>Patch for Slimes to damage monsters around Piper.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeUpdatePostfix(GreenSlime __instance, GameTime time)
    {
        var pipeTimer = __instance.Get_PipeTimer();
        if (pipeTimer.Value <= 0)
        {
            return;
        }

        pipeTimer.Value -= time.ElapsedGameTime.Milliseconds;
        foreach (var monster in __instance.currentLocation.characters.OfType<Monster>().Where(m => !m.IsSlime()))
        {
            var monsterBox = monster.GetBoundingBox();
            if (monster.IsInvisible || monster.isInvincible() ||
                (monster.isGlider.Value && !(__instance.Scale > 1.8f || __instance.Get_Jumping())) ||
                !monsterBox.Intersects(__instance.GetBoundingBox()))
            {
                continue;
            }

            if ((monster is Bug bug && bug.isArmoredBug.Value) // skip Armored Bugs
                || (monster is LavaCrab && __instance.Sprite.currentFrame % 4 == 0) // skip shelled Lava Crabs
                || (monster is RockCrab crab && crab.Sprite.currentFrame % 4 == 0 &&
                    !ModEntry.Reflector
                        .GetUnboundFieldGetter<RockCrab, NetBool>(crab, "shellGone")
                        .Invoke(crab).Value) // skip shelled Rock Crabs
                || (monster is LavaLurk lurk &&
                    lurk.currentState.Value == LavaLurk.State.Submerged) // skip submerged Lava Lurks
                || monster is Spiker) // skip Spikers
            {
                continue;
            }

            // damage monster
            var randomizedDamage = __instance.DamageToFarmer +
                                   Game1.random.Next(-__instance.DamageToFarmer / 4, __instance.DamageToFarmer / 4);
            var damageToMonster = (int)Math.Max(1, randomizedDamage * __instance.Scale) - monster.resilience.Value;
            (float xTrajectory, float yTrajectory) = monster.Slipperiness < 0
                ? Vector2.Zero
                : Utility.getAwayFromPositionTrajectory(monsterBox, __instance.getStandingPosition()) / 2f;
            monster.takeDamage(damageToMonster, (int)xTrajectory, (int)yTrajectory, false, 1d, "slime");
            monster.currentLocation.debris.Add(new Debris(
                damageToMonster,
                new Vector2(monsterBox.Center.X + 16, monsterBox.Center.Y),
                new Color(255, 130, 0),
                1f,
                monster));

            var piper = __instance.Get_Piper()!;
            monster.setInvincibleCountdown(piper.Get_Ultimate() is Concerto { IsActive: true } ? 300 : 450);

            // aggro monsters
            if (monster.Get_Taunter().Get(monster.currentLocation) is null)
            {
                monster.Set_Taunter(__instance);
            }

            var fakeFarmer = monster.Get_FakeFarmer();
            if (fakeFarmer is not null)
            {
                fakeFarmer.Position = __instance.Position;
            }

            // get damaged by monster
            randomizedDamage = monster.DamageToFarmer +
                                   Game1.random.Next(-monster.DamageToFarmer / 4, monster.DamageToFarmer / 4);
            var damageToSlime = Math.Max(1, randomizedDamage) - __instance.resilience.Value;
            __instance.takeDamage(damageToSlime, (int)-xTrajectory, (int)-yTrajectory, false, 1d, "slime");
            if (__instance.Health <= 0)
            {
                break;
            }
        }
    }

    #endregion harmony patches
}
