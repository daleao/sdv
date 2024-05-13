namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Enums;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterUpdateMovementPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterUpdateMovementPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MonsterUpdateMovementPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Monster>(nameof(Monster.updateMovement));
    }

    #region harmony patches

    /// <summary>Patch for improve AI for piped Slimes using A*.</summary>
    [HarmonyPrefix]
    private static void MonsterUpdateMovementWithAStar(Monster __instance, GameLocation location, GameTime time)
    {
        if (__instance is not GreenSlime slime || slime.Get_Piped() is null)
        {
            return;
        }

        var path = slime.Get_Path();
        var current = __instance.TilePoint;
        var target = __instance.Player.TilePoint;
        var priority = Game1.ticks + location.characters.IndexOf(__instance);
        if (priority % 120 == 0)
        {
            if (!location.Get_Pathfinder().ComputeShortestPath(current, target, out path))
            {
                __instance.defaultMovementBehavior(time);
                goto realizeMovement;
            }

            slime.Set_Path(path);
        }

        var next = slime.Get_Step();
        var diff = next - current;
        if (diff == Point.Zero)
        {
            if (path.Count == 0)
            {
                __instance.defaultMovementBehavior(time);
                goto realizeMovement;
            }

            next = path.Pop();
            slime.Set_Step(next);
            diff = next - current;
        }

        var direction = Math.Abs(diff.X) > Math.Abs(diff.Y)
            ? diff.X >= 0 ? FacingDirection.Right : FacingDirection.Left
            : diff.Y >= 0 ? FacingDirection.Down : FacingDirection.Up;
        direction.SetMoving(__instance);

    realizeMovement:
        __instance.MovePosition(time, Game1.viewport, location);
        if (__instance.Position.Equals(__instance.lastPosition) && __instance.IsWalkingTowardPlayer &&
            __instance.withinPlayerThreshold())
        {
            __instance.noMovementProgressNearPlayerBehavior();
        }
    }

    /// <summary>Patch for improve AI for piped Slimes using D* Lite.</summary>
    [HarmonyPrefix]
    private static void MonsterUpdateMovementWithDStarLite(Monster __instance, GameLocation location, GameTime time)
    {
        if (__instance is not GreenSlime slime || slime.Get_Piped() is null)
        {
            return;
        }

        var current = __instance.TilePoint;
        var next = slime.Get_Step();
        var diff = next - current;
        if (diff == Point.Zero)
        {
            var target = __instance.Player.TilePoint;
            if (slime.Get_Pathfinder().Step(current, target) is not { } step)
            {
                __instance.defaultMovementBehavior(time);
                goto realizeMovement;
            }

            next = step;
            slime.Set_Step(next);
            diff = next - current;
        }

        var direction = Math.Abs(diff.X) > Math.Abs(diff.Y)
            ? diff.X >= 0 ? FacingDirection.Right : FacingDirection.Left
            : diff.Y >= 0 ? FacingDirection.Down : FacingDirection.Up;
        direction.SetMoving(__instance);

    realizeMovement:
        __instance.MovePosition(time, Game1.viewport, location);
        if (__instance.Position.Equals(__instance.lastPosition) && __instance.IsWalkingTowardPlayer &&
            __instance.withinPlayerThreshold())
        {
            __instance.noMovementProgressNearPlayerBehavior();
        }
    }

    #endregion harmony patches
}
