namespace DaLion.Overhaul.Modules.Professions.Patchers.Foraging;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class CropHitWithHoePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CropHitWithHoePatcher"/> class.</summary>
    internal CropHitWithHoePatcher()
    {
        this.Target = this.RequireMethod<Crop>(nameof(Crop.hitWithHoe));
    }

    #region harmony patches

    /// <summary>Patch to apply Ecologist perk to wild ginger.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? CropHitWithHoeTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: SetGingerQuality(obj);
        // Between: obj = new SObject(829, 1);
        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Stloc_0) })
                .Move()
                .AddLabels(resumeExecution)
                .InsertProfessionCheck(VanillaProfession.Ecologist)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetEcologistForageQuality))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(SObject).RequirePropertySetter(nameof(SObject.Quality))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed apply Ecologist perk to hoed ginger.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
