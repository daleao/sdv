namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.GameData.FarmAnimals;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmAnimalPetPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmAnimalPetPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FarmAnimalPetPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FarmAnimal>(nameof(FarmAnimal.pet));
    }

    #region harmony patches

    /// <summary>Patch for Prestiged Rancher tripled friendship bonus.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmAnimalPetTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var isNotPrestiged = generator.DefineLabel();
            helper
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.professions))),
                ])
                .PatternMatch([
                    new CodeInstruction(OpCodes.Brfalse_S)
                ])
                .GetOperand(out var label)
                .Move()
                .AddLabels(isNotPrestiged)
                .CopyUntil((Label)label, out var copy)
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.professions))),
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(
                        OpCodes.Ldfld,
                        typeof(FarmAnimalData).RequireField(nameof(FarmAnimalData.ProfessionForHappinessBoost))),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 100),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(NetIntHashSet).RequireMethod(nameof(NetIntHashSet.Contains))),
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                ])
                .Insert(copy.Take(copy.Length - 1).ToArray());
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding prestiged Rancher friendship bonuses.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
