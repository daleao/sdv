namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodStartMinigameEndFunctionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodStartMinigameEndFunctionPatcher"/> class.</summary>
    internal FishingRodStartMinigameEndFunctionPatcher()
    {
        this.Target = this.RequireMethod<FishingRod>(nameof(FishingRod.startMinigameEndFunction));
    }

    #region harmony patches

    /// <summary>Patch to remove Pirate bonus treasure chance + double Fisher bait effect + Angler rod memory.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FishingRodStartMinigameEndFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Removed: lastUser.professions.Contains(<pirate_id>) ? baseChance ...
        try
        {
            helper // find index of pirate check
                .MatchProfessionCheck(Farmer.pirate)
                .Move(-2)
                .CountUntil(new[] { new CodeInstruction(OpCodes.Add) }, out var count)
                .Remove(count); // remove this check
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing vanilla Pirate bonus treasure chance.\nHelper returned {ex}");
            return null;
        }

        try
        {
            var isNotFisher = generator.DefineLabel();
            var isNotPrestigedFisher = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Ldsfld,
                            typeof(FishingRod).RequireField(nameof(FishingRod.baseChanceForTreasure))),
                    },
                    ILHelper.SearchOption.First,
                    nth: 2)
                .Copy(out var loadBaseChanceForTreasure)
                .Move()
                .AddLabels(isNotFisher, isNotPrestigedFisher)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(Tool).RequireField("lastUser")),
                    })
                .InsertProfessionCheck(Farmer.fisher, forLocalPlayer: false)
                .Insert(new[] { new CodeInstruction(OpCodes.Brfalse_S, isNotFisher) })
                .Insert(loadBaseChanceForTreasure)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Add), new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(Tool).RequireField("lastUser")),
                    })
                .InsertProfessionCheck(Farmer.fisher + 100, forLocalPlayer: false)
                .Insert(new[] { new CodeInstruction(OpCodes.Brfalse_S, isNotPrestigedFisher) })
                .Insert(loadBaseChanceForTreasure)
                .Insert(new[] { new CodeInstruction(OpCodes.Add) });
        }
        catch (Exception ex)
        {
            Log.E($"Failed doubling Magnet effect.\nHelper returned {ex}");
            return null;
        }

        try
        {
            var readMethod = typeof(ItemExtensions).GetMethods()
                .FirstOrDefault(mi =>
                    mi.Name.Contains(nameof(ItemExtensions.Read)) && mi.GetGenericArguments().Length > 0)
                ?.MakeGenericMethod(typeof(int)) ?? ThrowHelper.ThrowMissingMethodException<MethodInfo>("Read method not found.");

            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4, 693) })
                .Move()
                .GetOperand(out var doesHaveTreasureHunter)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldstr, DataKeys.LastTackleUsed),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Ldstr, string.Empty),
                        new CodeInstruction(OpCodes.Call, readMethod),
                        new CodeInstruction(OpCodes.Ldc_I4, 693),
                        new CodeInstruction(OpCodes.Beq_S, doesHaveTreasureHunter),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Treasure Hunter memory.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
