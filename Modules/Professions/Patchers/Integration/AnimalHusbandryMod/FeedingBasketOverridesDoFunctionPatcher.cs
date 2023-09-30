namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration.AnimalHusbandryMod;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("DIGUS.ANIMALHUSBANDRYMOD", "Animal Husbandry")]
internal sealed class FeedingBasketOverridesDoFunctionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FeedingBasketOverridesDoFunctionPatcher"/> class.</summary>
    internal FeedingBasketOverridesDoFunctionPatcher()
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
        var helper = new ILHelper(original, instructions);

        // From: if ((!animal.isCoopDweller() && who.professions.Contains(3)) || (animal.isCoopDweller() && who.professions.Contains(2)))
        // To: if (who.professions.Contains(<rancher_id>)
        // -- and also
        // Injected: if (who.professions.Contains(<rancher_id> + 100)) repeat professionAdjust ...
        var isNotPrestiged = generator.DefineLabel();
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_1),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(FarmAnimal).RequireMethod(nameof(FarmAnimal.isCoopDweller))),
                    })
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[7]),
                        new CodeInstruction(OpCodes.Ldsfld),
                        new CodeInstruction(OpCodes.Ldfld),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) }, ILHelper.SearchOption.Previous)
                .GetOperand(out var isNotRancher)
                .Return(2)
                .CountUntil(new[] { new CodeInstruction(OpCodes.Nop) }, out var count)
                .Remove(count)
                .Insert(new[] { new CodeInstruction(OpCodes.Ldarg_S, (byte)5) }) // arg 5 = Farmer who
                .InsertProfessionCheck(Profession.Rancher.Value, forLocalPlayer: false)
                .Insert(new[] { new CodeInstruction(OpCodes.Brfalse_S, isNotRancher) })
                .CountUntil(new[] { new CodeInstruction(OpCodes.Stloc_S, helper.Locals[7]) }, out count)
                .Copy(out var copy, count)
                .Insert(copy)
                .Insert(new[] { new CodeInstruction(OpCodes.Ldarg_S, (byte)5) })
                .InsertProfessionCheck(Profession.Rancher.Value + 100, forLocalPlayer: false)
                .Insert(new[] { new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged) })
                .Match(new[] { new CodeInstruction(OpCodes.Nop) })
                .Remove()
                .AddLabels(isNotPrestiged);
        }
        catch (Exception ex)
        {
            Log.E(
                "Professions module failed moving combined feeding basket Coopmaster + Shepherd friendship bonuses to Rancher." +
                $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
