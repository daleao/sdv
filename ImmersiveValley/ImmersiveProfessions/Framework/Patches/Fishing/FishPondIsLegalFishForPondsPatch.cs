namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Buildings;

using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class FishPondIsLegalFishForPondsPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondIsLegalFishForPondsPatch()
    {
        Original = RequireMethod<FishPond>("isLegalFishForPonds");
    }

    #region harmony patches

    /// <summary>Patch for prestiged Aquarist to raise legendary fish.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> FishPondIsLegalFishForPondsTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (fish_item.HasContextTag("fish_legendary")) ...
        /// To: if (fish_item.HasContextTag("fish_legendary") && !owner.HasPrestigedProfession("Aquarist"))

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldstr, "fish_legendary")
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .GetOperand(out var resumeExecution)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FishPondIsLegalFishForPondsPatch).RequireMethod(nameof(IsLegalFishForPondsSubroutine))),
                    new CodeInstruction(OpCodes.Brtrue_S, resumeExecution)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Aquarist permission to raise legendary fish.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool IsLegalFishForPondsSubroutine(FishPond pond)
    {
        var owner = Game1.getFarmerMaybeOffline(pond.owner.Value) ?? Game1.MasterPlayer;
        return owner.HasProfession(Profession.Aquarist, true);
    }

    #endregion injected subroutines
}