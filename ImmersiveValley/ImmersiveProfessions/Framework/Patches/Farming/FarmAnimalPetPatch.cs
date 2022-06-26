namespace DaLion.Stardew.Professions.Framework.Patches.Farming;

#region using directives

using DaLion.Common;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmAnimalPetPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmAnimalPetPatch()
    {
        Target = RequireMethod<FarmAnimal>(nameof(FarmAnimal.pet));
    }

    #region harmony patches

    /// <summary>Patch for Rancher to combine Shepherd and Coopmaster friendship bonus.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmAnimalPetTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if ((who.professions.Contains(<shepherd_id>) && !isCoopDweller()) || (who.professions.Contains(<coopmaster_id>) && isCoopDweller()))
        /// To: if (who.professions.Contains(<rancher_id>)

        try
        {
            helper
                .FindProfessionCheck(Farmer.shepherd) // find index of shepherd check
                .Advance()
                .SetOpCode(OpCodes.Ldc_I4_0) // replace with rancher check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brfalse_S) // the get out of here false case branch instruction
                )
                .GetOperand(out var isNotRancher) // copy destination
                .Return(2)
                .SetOperand(isNotRancher) // replace false case branch with true case branch
                .Advance()
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .RemoveLabels();
        }
        catch (Exception ex)
        {
            Log.E(
                $"Failed while moving combined vanilla Coopmaster + Shepherd friendship bonuses to Rancher.\nHelper returned {ex}");
            return null;
        }

        /// From: friendshipTowardFarmer.Value = Math.Min(1000, (int)friendshipTowardFarmer + 15);
        /// To: friendshipTowardFarmer.Value = Math.Min(1000, (int)friendshipTowardFarmer + 15 + (who.professions.Contains(<rancher_id> + 100) ? 15 : 0));

        var isNotPrestiged = generator.DefineLabel();
        try
        {
            helper
                .FindProfessionCheck(Profession.Rancher.Value) // go back and find the inserted rancher check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_S, 15),
                    new CodeInstruction(OpCodes.Add)
                )
                .Advance(2)
                .AddLabels(isNotPrestiged)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1) // arg 1 = Farmer who
                )
                .InsertProfessionCheck(Profession.Rancher.Value + 100, forLocalPlayer: false)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 15),
                    new CodeInstruction(OpCodes.Add)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding prestiged Rancher friendship bonuses.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}