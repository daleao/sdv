namespace DaLion.Ligo.Modules.Professions.Patches.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondIsLegalFishForPondsPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FishPondIsLegalFishForPondsPatch"/> class.</summary>
    internal FishPondIsLegalFishForPondsPatch()
    {
        this.Target = this.RequireMethod<FishPond>("isLegalFishForPonds");
    }

    #region harmony patches

    /// <summary>Patch for prestiged Aquarist to raise legendary fish.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FishPondIsLegalFishForPondsTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // From: if (fish_item.HasContextTag("fish_legendary")) ...
        // To: if (fish_item.HasContextTag("fish_legendary") && !owner.HasPrestigedProfession("Aquarist"))
        try
        {
            helper
                .FindFirst(new CodeInstruction(OpCodes.Ldstr, "fish_legendary"))
                .AdvanceUntil(new CodeInstruction(OpCodes.Brfalse_S))
                .GetOperand(out var resumeExecution)
                .Advance()
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FishPondIsLegalFishForPondsPatch).RequireMethod(nameof(CanRaiseLegendaryFish))),
                    new CodeInstruction(OpCodes.Brtrue_S, resumeExecution));
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

    private static bool CanRaiseLegendaryFish(FishPond pond)
    {
        return pond.GetOwner().HasProfession(Profession.Aquarist, true) ||
               (ModEntry.Config.Professions.LaxOwnershipRequirements &&
                Game1.game1.DoesAnyPlayerHaveProfession(Profession.Aquarist, out _));
    }

    #endregion injected subroutines
}
