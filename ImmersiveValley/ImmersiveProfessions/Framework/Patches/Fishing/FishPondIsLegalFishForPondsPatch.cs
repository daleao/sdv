namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Stardew;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondIsLegalFishForPondsPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondIsLegalFishForPondsPatch()
    {
        Target = RequireMethod<FishPond>("isLegalFishForPonds");
    }

    #region harmony patches

    /// <summary>Patch for prestiged Aquarist to raise legendary fish.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FishPondIsLegalFishForPondsTranspiler(
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
                        typeof(FishPondIsLegalFishForPondsPatch).RequireMethod(nameof(CanRaiseLegendaryFish))),
                    new CodeInstruction(OpCodes.Brtrue_S, resumeExecution)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Aquarist permission to raise legendary fish.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool CanRaiseLegendaryFish(FishPond pond) =>
        pond.GetOwner().HasProfession(Profession.Aquarist, true) || ModEntry.Config.LaxOwnershipRequirements &&
        Game1.game1.DoesAnyPlayerHaveProfession(Profession.Aquarist, out _);

    #endregion injected subroutines
}