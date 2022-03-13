namespace DaLion.Stardew.Professions.Framework.Patches.Foraging;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.TerrainFeatures;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class BushShakePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal BushShakePatch()
    {
        Original = RequireMethod<Bush>("shake");
    }

    #region harmony patches

    /// <summary>Patch to nerf Ecologist berry quality and increment forage counter for wild berries.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> BushShakeTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: Game1.player.professions.Contains(16) ? 4 : 0
        /// To: Game1.player.professions.Contains(16) ? GetEcologistForageQuality() : 0

        try
        {
            helper
                .FindProfessionCheck(Farmer.botanist) // find index of botanist check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_4)
                )
                .GetLabels(out var labels) // backup branch labels
                .ReplaceWith( // replace with custom quality
                    new(OpCodes.Call,
                        typeof(FarmerExtensions).MethodNamed(
                            nameof(FarmerExtensions.GetEcologistForageQuality)))
                )
                .InsertWithLabels(
                    labels: labels, // restore backed-up labels
                    new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded Ecologist wild berry quality.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        /// Injected: if (Game1.player.professions.Contains(<ecologist_id>))
        ///		Data.IncrementField<uint>("EcologistItemsForaged")

        var dontIncreaseEcologistCounter = generator.DefineLabel();
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldarg_0)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldarg_0)
                )
                .InsertProfessionCheck((int) Profession.Ecologist)
                .Insert(
                    new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseEcologistCounter),
                    new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Ldstr, DataField.EcologistItemsForaged.ToString()),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FarmerExtensions)
                            .MethodNamed(nameof(FarmerExtensions.IncrementData), new[] {typeof(Farmer), typeof(DataField)})
                            .MakeGenericMethod(typeof(uint)))
                )
                .AddLabels(dontIncreaseEcologistCounter);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding Ecologist counter increment.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}