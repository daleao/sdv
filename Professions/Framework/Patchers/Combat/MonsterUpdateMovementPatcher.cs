namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterUpdateMovementPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterUpdateMovementPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MonsterUpdateMovementPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Monster>(nameof(Monster.updateMovement));
    }

    #region harmony patches

    /// <summary>Patch for improve AI for piped Slimes using D* Lite.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool MonsterUpdateMovementPostfix(Monster __instance, GameLocation location, GameTime time)
    {
        if (__instance is not GreenSlime slime ||
            Reflector.GetUnboundFieldGetter<GreenSlime, NetBool>(slime, "pursuingMate").Invoke(slime).Value ||
            __instance.focusedOnFarmers)
        {
            return true; // run original logic
        }

        if (slime.Get_Piped() is not { } piped)
        {
            // // this would make non-piped Slimes neutral / non-aggressive
            // slime.defaultMovementBehavior(time);
            // goto realizeMovement;

            return true; // run original logic
        }

        __instance.Speed = piped.Piper.Speed;
        __instance.addedSpeed = piped.Piper.addedSpeed;
        var target = __instance.Player;
        if (piped.FakeFarmer.IsAttachedToEnemy)
        {
            return true; // run original logic
        }

        var currentTile = __instance.TilePoint;
        var targetTile = target.TilePoint;
        if (currentTile == targetTile)
        {
            return false;
        }

        Point? step;
        if (Config.UseAsyncMinionPathfinder)
        {
            step = PathfinderAsync!.QueryStep(slime);
            if (step is null || step == currentTile)
            {
                PathfinderAsync.QueueRequest(slime, currentTile, targetTile);
                return false; // don't run original logic
            }
        }
        else
        {
            step = Pathfinder!.RequestFor(slime, currentTile, targetTile);
            if (step is null)
            {
                return false; // don't run original logic
            }
        }

        __instance.SetMovingTowardTile(step.Value);
        __instance.MovePosition(time, Game1.viewport, location);
        if (__instance.Position.Equals(__instance.lastPosition) && __instance.IsWalkingTowardPlayer &&
            __instance.withinPlayerThreshold())
        {
            __instance.noMovementProgressNearPlayerBehavior();
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
