namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondIsLegalFishForPondsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondIsLegalFishForPondsPatcher"/> class.</summary>
    internal FishPondIsLegalFishForPondsPatcher()
    {
        this.Target = this.RequireMethod<FishPond>("isLegalFishForPonds");
    }

    #region harmony patches

    /// <summary>Patch for prestiged Aquarist to raise legendary fish.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FishPondIsLegalFishForPondsTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (fish_item.HasContextTag("fish_legendary")) ...
        // To: if (fish_item.HasContextTag("fish_legendary") && !owner.HasPrestigedProfession("Aquarist"))
        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldstr, "fish_legendary") })
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) })
                .GetOperand(out var resumeExecution)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FishPondIsLegalFishForPondsPatcher).RequireMethod(nameof(CanRaiseLegendaryFish))),
                        new CodeInstruction(OpCodes.Brtrue_S, resumeExecution),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Aquarist permission to raise legendary fish.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool CanRaiseLegendaryFish(FishPond pond)
    {
        return pond.GetOwner().HasProfessionOrLax(VanillaProfession.Aquarist);
    }

    #endregion injected subroutines
}
