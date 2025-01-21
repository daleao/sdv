﻿namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftLoadLevelPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MineShaftLoadLevelPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MineShaftLoadLevelPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MineShaft>(nameof(MineShaft.loadLevel));
    }

    #region harmony patches

    /// <summary>Patch for Prestiged Spelunker bonus treasure room chance.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MineShaftCheckStoneForItemsTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_S, helper.Locals[11])], nth: 2)
                .Insert([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MineShaftLoadLevelPatcher).RequireMethod(
                            nameof(GetPrestigedSpelunkerTreasureRoomMultiplier))),
                    new CodeInstruction(OpCodes.Mul),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Prestiged Spelunker bonus treasure room chance.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static double GetPrestigedSpelunkerTreasureRoomMultiplier()
    {
        return !Game1.game1.DoesAnyPlayerHaveProfession(Profession.Spelunker, out var spelunkers, prestiged: true)
            ? 1d
            : 1d + (spelunkers.Count(spelunker => spelunker.currentLocation is MineShaft { mineLevel: > 120 }) * 0.5);
    }

    #endregion injections
}
