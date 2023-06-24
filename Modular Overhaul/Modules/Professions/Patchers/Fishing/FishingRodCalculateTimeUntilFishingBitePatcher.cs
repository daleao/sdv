namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodCalculateTimeUntilFishingBitePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodCalculateTimeUntilFishingBitePatcher"/> class.</summary>
    internal FishingRodCalculateTimeUntilFishingBitePatcher()
    {
        this.Target = this.RequireMethod<FishingRod>("calculateTimeUntilFishingBite");
    }

    #region harmony patches

    /// <summary>Patch to double Fisher bait effects + Angler rod memory.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FishingRodCalculateTimeUntilFishingBiteTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var notFisher1 = generator.DefineLabel();
            var notPrestigedFisher1 = generator.DefineLabel();
            var notFisher2 = generator.DefineLabel();
            var notPrestigedFisher2 = generator.DefineLabel();
            helper
                // any bait effect
                .Match(new[] { new CodeInstruction(OpCodes.Stloc_0) }, nth: 3)
                .Move()
                .AddLabels(notFisher1, notPrestigedFisher1)
                .Insert(new[] { new CodeInstruction(OpCodes.Ldarg_3) }) // arg 3 = Farmer who
                .InsertProfessionCheck(Farmer.fisher, forLocalPlayer: false)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, notFisher1),
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(OpCodes.Stloc_0),
                        new CodeInstruction(OpCodes.Ldarg_3),
                    })
                .InsertProfessionCheck(Farmer.fisher + 100, forLocalPlayer: false)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, notPrestigedFisher1),
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(OpCodes.Stloc_0),
                    })
                // wild bait effect
                .Match(new[] { new CodeInstruction(OpCodes.Stloc_0) })
                .Move()
                .AddLabels(notFisher2, notPrestigedFisher2)
                .Insert(new[] { new CodeInstruction(OpCodes.Ldarg_3) }) // arg 3 = Farmer who
                .InsertProfessionCheck(Farmer.fisher, forLocalPlayer: false)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, notFisher2),
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Ldc_R4, 0.75f),
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(OpCodes.Stloc_0),
                        new CodeInstruction(OpCodes.Ldarg_3),
                    })
                .InsertProfessionCheck(Farmer.fisher + 100, forLocalPlayer: false)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, notPrestigedFisher2),
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Ldc_R4, 0.75f),
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(OpCodes.Stloc_0),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed doubling bait nibble time effects.\nHelper returned {ex}");
            return null;
        }

        helper.GoTo(0);
        var readMethod = typeof(ItemExtensions).GetMethods()
            .FirstOrDefault(mi =>
                mi.Name.Contains(nameof(ItemExtensions.Read)) && mi.GetGenericArguments().Length > 0)
            ?.MakeGenericMethod(typeof(int)) ?? ThrowHelper.ThrowMissingMethodException<MethodInfo>("Read method not found.");

        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4, 686) })
                .Move()
                .GetOperand(out var doesHaveSpinner)
                .Move()
                .StripLabels(out var labels)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldstr, DataKeys.LastTackleUsed),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ldstr, string.Empty),
                        new CodeInstruction(OpCodes.Call, readMethod),
                        new CodeInstruction(OpCodes.Ldc_I4, 686),
                        new CodeInstruction(OpCodes.Beq_S, doesHaveSpinner),
                    },
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Spinner memory.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4, 687) })
                .Move()
                .GetOperand(out var doesHaveDressedSpinner)
                .Move()
                .StripLabels(out var labels)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldstr, DataKeys.LastTackleUsed),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ldstr, string.Empty),
                        new CodeInstruction(OpCodes.Call, readMethod),
                        new CodeInstruction(OpCodes.Ldc_I4, 687),
                        new CodeInstruction(OpCodes.Beq_S, doesHaveDressedSpinner),
                    },
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Dressed Spinner memory.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
