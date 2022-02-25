namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Locations;

using Stardew.Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal class ForestGetFishPatch : BasePatch
{
    private const int GLACIERFISH_INDEX_I = 775;

    /// <summary>Construct an instance.</summary>
    internal ForestGetFishPatch()
    {
        Original = RequireMethod<Forest>(nameof(Forest.getFish));
    }

    #region harmony patches

    /// <summary>Patch for prestiged Angler to recatch Glacierfish.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ForestGetFishTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (!who.fishCaught.ContainsKey(<legendary_fish_id>)) ...
        /// To: if (!who.fishCaught.ContainsKey(<legendary_fish_id>) || !who.HasPrestigedProfession("Angler") ...

        var checkSeason = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4, GLACIERFISH_INDEX_I)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brtrue_S)
                )
                .GetOperand(out var skipLegendary)
                .ReplaceWith(
                    new(OpCodes.Brfalse_S, checkSeason))
                .Advance()
                .AddLabels(checkSeason)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, 4) // arg 4 = Farmer who
                )
                .InsertProfessionCheckForPlayerOnStack((int)Profession.Angler + 100, (Label)skipLegendary);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Angler legendary fish recatch.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}