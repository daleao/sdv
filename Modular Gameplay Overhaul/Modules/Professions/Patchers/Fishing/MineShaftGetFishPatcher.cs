namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftGetFishPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MineShaftGetFishPatcher"/> class.</summary>
    internal MineShaftGetFishPatcher()
    {
        this.Target = this.RequireMethod<MineShaft>(nameof(MineShaft.getFish));
    }

    #region harmony patches

    /// <summary>Patch for Fisher to reroll reeled fish if first roll resulted in trash.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationGetFishTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (Game1.player.professions.Contains(<fisher_id>)) <baseChance> *= 2
        // Before each of the three fish rolls
        try
        {
            helper
                .Repeat(
                    3,
                    i =>
                    {
                        var isNotFisher = generator.DefineLabel();
                        helper
                            .Match(
                                new[]
                                {
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(Random).RequireMethod(nameof(Random.NextDouble))),
                                })
                            .Match(new[] { new CodeInstruction(OpCodes.Ldc_R8, 0.02 - (i * 0.005)), })
                            .Move()
                            .AddLabels(isNotFisher)
                            .InsertProfessionCheck(Profession.Fisher.Value)
                            .Insert(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Brfalse_S, isNotFisher),
                                    new CodeInstruction(OpCodes.Ldc_R8, 2.0),
                                    new CodeInstruction(OpCodes.Mul),
                                });
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding modded Fisher fish reroll.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
