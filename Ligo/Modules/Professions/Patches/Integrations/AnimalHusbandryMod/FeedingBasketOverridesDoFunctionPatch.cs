namespace DaLion.Ligo.Modules.Professions.Patches.Integrations;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Integration("DIGUS.ANIMALHUSBANDRYMOD")]
internal sealed class FeedingBasketOverridesDoFunctionPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FeedingBasketOverridesDoFunctionPatch"/> class.</summary>
    internal FeedingBasketOverridesDoFunctionPatch()
    {
        this.Target = "AnimalHusbandryMod.tools.FeedingBasketOverrides"
            .ToType()
            .RequireMethod("DoFunction");
    }

    #region harmony patches

    /// <summary>Patch for Rancher to combine Shepherd and Coopmaster friendship bonus.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? InseminationSyringeOverridesDoFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // From: if ((!animal.isCoopDweller() && who.professions.Contains(3)) || (animal.isCoopDweller() && who.professions.Contains(2)))
        // To: if (who.professions.Contains(<rancher_id>)
        // -- and also
        // Injected: if (who.professions.Contains(<rancher_id> + 100)) repeat professionAdjust ...
        var isNotPrestiged = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(FarmAnimal).RequireMethod(nameof(FarmAnimal.isCoopDweller))))
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[7]),
                    new CodeInstruction(OpCodes.Ldsfld),
                    new CodeInstruction(OpCodes.Ldfld))
                .RetreatUntil(new CodeInstruction(OpCodes.Brfalse_S))
                .GetOperand(out var isNotRancher)
                .Return(2)
                .RemoveInstructionsUntil(new CodeInstruction(OpCodes.Nop))
                .InsertInstructions(new CodeInstruction(OpCodes.Ldarg_S, (byte)5)) // arg 5 = Farmer who
                .InsertProfessionCheck(Profession.Rancher.Value, forLocalPlayer: false)
                .InsertInstructions(new CodeInstruction(OpCodes.Brfalse_S, isNotRancher))
                .GetInstructionsUntil(
                    out var got,
                    pattern: new CodeInstruction(OpCodes.Stloc_S, $"{typeof(double)} (7)"))
                .InsertInstructions(got)
                .InsertInstructions(new CodeInstruction(OpCodes.Ldarg_S, (byte)5))
                .InsertProfessionCheck(Profession.Rancher.Value + 100, forLocalPlayer: false)
                .InsertInstructions(new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged))
                .AdvanceUntil(new CodeInstruction(OpCodes.Nop))
                .RemoveInstructions()
                .AddLabels(isNotPrestiged);
        }
        catch (Exception ex)
        {
            Log.E(
                "Immersive Professions failed while moving combined feeding basket Coopmaster + Shepherd friendship bonuses to Rancher." +
                "\n—-- Do NOT report this to Animal Husbandry's author. ---" +
                $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
