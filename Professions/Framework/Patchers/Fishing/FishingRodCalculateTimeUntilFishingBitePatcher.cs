namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodCalculateTimeUntilFishingBitePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodCalculateTimeUntilFishingBitePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FishingRodCalculateTimeUntilFishingBitePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FishingRod>("calculateTimeUntilFishingBite");
    }

    #region harmony patches

    /// <summary>Patch to double Fisher bait effects + Angler rod memory.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? FishingRodCalculateTimeUntilFishingBiteTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var notFisher1 = generator.DefineLabel();
            var notFisher2 = generator.DefineLabel();
            var notFisher3 = generator.DefineLabel();
            var notPrestigedFisher1 = generator.DefineLabel();
            var notPrestigedFisher2 = generator.DefineLabel();
            var notPrestigedFisher3 = generator.DefineLabel();
            helper
                // any standard bait effect
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_3)], nth: 3)
                .Move()
                .AddLabels(notFisher1, notPrestigedFisher1)
                .Insert([new CodeInstruction(OpCodes.Ldarg_3)]) // arg 3 = Farmer who
                .InsertProfessionCheck(Farmer.fisher, forLocalPlayer: false)
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, notFisher1),
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Stloc_3),
                    new CodeInstruction(OpCodes.Ldarg_3),
                ])
                .InsertProfessionCheck(Farmer.fisher + 100, forLocalPlayer: false)
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, notPrestigedFisher1),
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Stloc_3),
                ])
                // wild / challenge bait effects
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_3)])
                .Move()
                .AddLabels(notFisher2, notPrestigedFisher2)
                .Insert([new CodeInstruction(OpCodes.Ldarg_3)]) // arg 3 = Farmer who
                .InsertProfessionCheck(Farmer.fisher, forLocalPlayer: false)
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, notFisher2),
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.75f),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Stloc_3),
                    new CodeInstruction(OpCodes.Ldarg_3),
                ])
                .InsertProfessionCheck(Farmer.fisher + 100, forLocalPlayer: false)
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, notPrestigedFisher2),
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.75f),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Stloc_3),
                ])
                // deluxe bait effect
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_3)])
                .Move()
                .AddLabels(notFisher3, notPrestigedFisher3)
                .Insert([new CodeInstruction(OpCodes.Ldarg_3)]) // arg 3 = Farmer who
                .InsertProfessionCheck(Farmer.fisher, forLocalPlayer: false)
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, notFisher3),
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.66f),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Stloc_3),
                    new CodeInstruction(OpCodes.Ldarg_3),
                ])
                .InsertProfessionCheck(Farmer.fisher + 100, forLocalPlayer: false)
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, notPrestigedFisher3),
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.66f),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Stloc_3),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed doubling bait nibble time effects.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
