namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationBreakStonePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationBreakStonePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationBreakStonePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>("breakStone");
    }

    #region harmony patches

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void GameLocationBreakStonePostfix(GameLocation __instance, string stoneId, int x, int y, Farmer? who, Random r)
    {
        if (who is null || __instance is not MineShaft shaft ||
            !who.HasProfession(Profession.Spelunker) || !shaft.ladderHasSpawned || State.SpelunkerLadderStreak <= 0)
        {
            return;
        }

        if (!stoneId.IsAnyOf("343", "449", "450", "668", "670", "760", "762", "845", "846", "847"))
        {
            return;
        }

        var addedOres = 0;
        if (who.HasProfession(Profession.Miner))
        {
            addedOres += who.HasProfession(Profession.Miner, true) ? 2 : 1;
        }

        if (who.hasBuff("dwarfStatue_0"))
        {
            addedOres++;
        }

        var chance = 0.2 + (State.SpelunkerClusterStreak * 0.01);
        if (who.HasProfession(Profession.Spelunker, true))
        {
            chance *= 2;
        }

        if (!r.NextBool(chance))
        {
            return;
        }

        var farmerId = who.UniqueMultiplayerID;
        var farmerLuckLevel = who.LuckLevel;
        var farmerMiningLevel = who.MiningLevel;
        if (shaft.GetAdditionalDifficulty() > 0 && r.NextBool(0.2))
        {
            Game1.createMultipleObjectDebris(
                QIDs.RadioactiveOre,
                x,
                y,
                addedOres + r.Next(1, 4) + ((r.NextDouble() < (double)(farmerLuckLevel / 100f)) ? 1 : 0) + ((r.NextDouble() < (double)(farmerMiningLevel / 100f)) ? 1 : 0),
                farmerId,
                __instance);
            return;
        }

        if (shaft.mineLevel >= 120 && r.NextBool(0.2))
        {
            Game1.createMultipleObjectDebris(
                QIDs.IridiumOre,
                x,
                y,
                addedOres + r.Next(1, 4) + ((r.NextDouble() < (double)(farmerLuckLevel / 100f)) ? 1 : 0) + ((r.NextDouble() < (double)(farmerMiningLevel / 100f)) ? 1 : 0),
                farmerId,
                __instance);
            return;
        }

        if (shaft.mineLevel >= 80 && r.NextBool(0.2))
        {
            Game1.createMultipleObjectDebris(
                QIDs.GoldOre,
                x,
                y,
                addedOres + r.Next(1, 4) + ((r.NextDouble() < (double)(farmerLuckLevel / 100f)) ? 1 : 0) + ((r.NextDouble() < (double)(farmerMiningLevel / 100f)) ? 1 : 0),
                farmerId,
                __instance);
            return;
        }

        if (shaft.mineLevel >= 40 && r.NextBool(0.2))
        {
            Game1.createMultipleObjectDebris(
                QIDs.IronOre,
                x,
                y,
                addedOres + r.Next(1, 4) + ((r.NextDouble() < (double)(farmerLuckLevel / 100f)) ? 1 : 0) + ((r.NextDouble() < (double)(farmerMiningLevel / 100f)) ? 1 : 0),
                farmerId,
                __instance);
            return;
        }

        Game1.createMultipleObjectDebris(
            QIDs.CopperOre,
            x,
            y,
            addedOres + r.Next(1, 4) + ((r.NextDouble() < (double)(farmerLuckLevel / 100f)) ? 1 : 0) + ((r.NextDouble() < (double)(farmerMiningLevel / 100f)) ? 1 : 0),
            farmerId,
            __instance);
    }

    /// <summary>Patch to remove Geologist extra gem chance + remove Prospector double coal chance.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? GameLocationBreakStoneTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (who.professions.Contains(100 + <miner_id>) addedOres++;
        // After: int addedOres = (who.professions.Contains(<miner_id>) ? 1 : 0);
        try
        {
            var isNotPrestiged = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_I4_1)])
                .StripLabels(out var isMiner)
                .AddLabels(isNotPrestiged)
                .Insert([new CodeInstruction(OpCodes.Ldarg_S, (byte)4)], labels: isMiner) // arg 4 = Farmer who
                .InsertProfessionCheck(Farmer.miner + 100, forLocalPlayer: false)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                        new CodeInstruction(OpCodes.Ldc_I4_2),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    ])
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Miner extra ores.\nHelper returned {ex}");
            return null;
        }

        // From: if (who.professions.Contains(<geologist_id>) ...
        // To: if (who.professions.Contains(<gemologist_id>) ...
        try
        {
            var notPrestiged = generator.DefineLabel();
            helper
                .MatchProfessionCheck(Farmer.geologist)
                .Move()
                .SetOperand(Farmer.gemologist)
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_S, helper.Locals[8])])
                .StripLabels(out var labels)
                .AddLabels(notPrestiged)
                .Insert([new CodeInstruction(OpCodes.Ldarg_S, (byte)4)], labels)
                .InsertProfessionCheck(Farmer.gemologist + 100, forLocalPlayer: false)
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, notPrestiged),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Add),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed replacing vanilla Geologist paired gems with Gemologist.\nHelper returned {ex}");
            return null;
        }

        // Skipped: if (who.professions.Contains(<burrower_id>)) ...
        try
        {
            helper
                .MatchProfessionCheck(Farmer.burrower) // find index of prospector check
                .Move(-1)
                .PatternMatch([new CodeInstruction(OpCodes.Brfalse_S)]) // the false case branch
                .GetOperand(out var isNotProspector) // copy destination
                .Return()
                .Insert(
                    [
                        // insert uncoditional branch to skip this check
                        new CodeInstruction(OpCodes.Br_S, (Label)isNotProspector),
                    ]);
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
