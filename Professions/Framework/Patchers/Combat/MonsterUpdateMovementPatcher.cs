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
    internal MonsterUpdateMovementPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Monster>(nameof(Monster.updateMovement));
    }

    #region harmony patches

    /// <summary>Patch for improve AI for piped Slimes using D* Lite.</summary>
    [HarmonyPrefix]
    private static bool MonsterUpdateMovementPostfix(Monster __instance, GameLocation location, GameTime time)
    {
        if (__instance is not GreenSlime slime ||
            Reflector.GetUnboundFieldGetter<GreenSlime, NetBool>(slime, "pursuingMate").Invoke(slime).Value)
        {
            return true; // run original logic
        }

        if (slime.Get_Piped() is not { } piped)
        {
            if (!__instance.Player.HasProfession(Profession.Piper) || State.OffendedSlimes.Contains(slime))
            {
                return true; // run original logic
            }

            slime.defaultMovementBehavior(time);
            goto realizeMovement;
        }

        __instance.Speed = piped.Piper.Speed;
        var target = __instance.Player;
        if (piped.FakeFarmer.IsEnemy)
        {
            return true; // run original logic
        }

        var currenTile = __instance.TilePoint;
        var step = slime.Get_CurrentStep();
        if (step is null || step == currenTile)
        {
            var targetTile = target.TilePoint;
            step = slime.Get_IncrementalPathfinder().Step(currenTile, targetTile);
            if (step is null)
            {
                return false; // don't run original logic
            }

            slime.Set_CurrentStep(step);
        }

        slime.SetMovingTowardTile(step.Value);

    realizeMovement:
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
