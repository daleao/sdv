namespace DaLion.Overhaul.Modules.Professions.Patchers.Foraging;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using DaLion.Shared.ModData;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class BushShakePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BushShakePatcher"/> class.</summary>
    internal BushShakePatcher()
    {
        this.Target = this.RequireMethod<Bush>("shake");
    }

    #region harmony patches

    /// <summary>Patch to nerf Ecologist berry quality and increment forage counter for wild berries.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BushShakeTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: Game1.player.professions.Contains(16) ? 4 : 0
        // To: Game1.player.professions.Contains(16) ? GetEcologistForageQuality() : 0
        try
        {
            helper
                .MatchProfessionCheck(Farmer.botanist) // find index of botanist check
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_4) })
                .GetLabels(out var labels) // backup branch labels
                .ReplaceWith(
                    // replace with custom quality
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetEcologistForageQuality))))
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    },
                    labels); // restore backed-up labels
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching modded Ecologist wild berry quality.\nHelper returned {ex}");
            return null;
        }

        // Injected: if (Game1.player.professions.Contains(<ecologist_id>))
        //     Data.IncrementField<uint>(DataKeys.EcologistItemsForaged)
        try
        {
            var dontIncreaseEcologistCounter = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) })
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) })
                .InsertProfessionCheck(VanillaProfession.Ecologist.Value)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseEcologistCounter),
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(OpCodes.Ldstr, DataKeys.EcologistItemsForaged),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModDataIO)
                                .RequireMethod(nameof(ModDataIO.Increment), new[] { typeof(Farmer), typeof(string) })
                                .MakeGenericMethod(typeof(uint))),
                    })
                .AddLabels(dontIncreaseEcologistCounter);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Ecologist counter increment.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
