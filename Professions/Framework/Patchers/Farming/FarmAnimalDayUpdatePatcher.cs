namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.GameData.FarmAnimals;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmAnimalDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmAnimalDayUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FarmAnimalDayUpdatePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FarmAnimal>(nameof(FarmAnimal.dayUpdate));
    }

    #region harmony patches

    /// <summary>Patch to apply Producer production frequency bonus at max happiness.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? FarmAnimalDayUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Stloc_2),
                    ],
                    nth: 2)
                .Move(-2)
                .Remove(2)
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FarmAnimalDayUpdatePatcher).RequireMethod(nameof(ApplyProducerProductionSpeedBonus)))
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching animal production speed bonus for Producer.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static int ApplyProducerProductionSpeedBonus(int produceSpeedBonus, FarmAnimal animal, FarmAnimalData data)
    {
        return animal.happiness.Value < 200
            ? produceSpeedBonus
            : animal.GetOwner().HasProfessionOrLax(Profession.FromValue(data.ProfessionForFasterProduce), true)
                ? produceSpeedBonus + (int)Math.Ceiling(data.DaysToProduce * (2f / 3f))
                : produceSpeedBonus + (int)Math.Ceiling(data.DaysToProduce / 2f);
    }

    #endregion injected
}
