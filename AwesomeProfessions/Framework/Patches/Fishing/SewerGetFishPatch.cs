using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley.Locations;
using DaLion.Stardew.Common.Harmony;
using DaLion.Stardew.Professions.Framework.Extensions;

namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

[UsedImplicitly]
internal class SewerGetFishPatch : BasePatch
{
    private const int MUTANT_CARP_INDEX_I = 682;

    /// <summary>Construct an instance.</summary>
    internal SewerGetFishPatch()
    {
        Original = RequireMethod<Sewer>(nameof(Sewer.getFish));
    }

    #region harmony patches

    /// <summary>Patch for prestiged Angler to recatch Mutant Carp.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> SewerGetFishTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (!who.fishCaught.ContainsKey(<legendary_fish_id>)) ...
        /// To: if (!who.fishCaught.ContainsKey(<legendary_fish_id>) || !who.HasPrestigedProfession("Angler") ...

        var chooseLegendary = iLGenerator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4, MUTANT_CARP_INDEX_I)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brtrue_S)
                )
                .GetOperand(out var skipLegendary)
                .ReplaceWith(
                    new(OpCodes.Brfalse_S, chooseLegendary))
                .Advance()
                .AddLabels(chooseLegendary)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, 4), // arg 4 = Farmer who
                    new CodeInstruction(OpCodes.Ldstr, "Angler"),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FarmerExtensions).MethodNamed(nameof(FarmerExtensions.HasPrestigedProfession))),
                    new CodeInstruction(OpCodes.Brfalse_S, skipLegendary)
                );
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while adding prestiged Angler legendary fish recatch.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}