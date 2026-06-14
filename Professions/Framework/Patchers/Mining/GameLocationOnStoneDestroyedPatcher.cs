namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Xna;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationOnStoneDestroyedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationOnStoneDestroyedPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationOnStoneDestroyedPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.OnStoneDestroyed));
    }

    #region harmony patches

    /// <summary>Patch to trigger and manage Prospector hunt + track Spelunker momentum.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void OnStoneDestroyedPostfix(GameLocation __instance, int x, int y, Farmer who)
    {
        State.ProspectorHunt?.OnStoneDestroyed(__instance, new Vector2(x, y));

        if (who is null || !who.IsLocalPlayer || !who.HasProfession(Profession.Spelunker)
            || who.CurrentTool is not Pickaxe)
        {
            return;
        }

        var tile = new Vector2(x, y);
        if (State.SpelunkerLastStoneDestroyedAt is not null)
        {
            if (tile.IsAdjacentTo(State.SpelunkerLastStoneDestroyedAt.Value))
            {
                State.SpelunkerClusterStreak += 1;
            }
            else
            {
                State.SpelunkerClusterStreak = 0;
            }
        }

        State.SpelunkerLastStoneDestroyedAt = new Vector2(x, y);
    }

    /// <summary>Patch to remove Prospector double coal chance.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? GameLocationOnStoneDestroyedTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6])]) // burrowerMultiplier
                .StripLabels(out var labels)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Pop),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                    ],
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing vanilla Prospector double coal chance.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
