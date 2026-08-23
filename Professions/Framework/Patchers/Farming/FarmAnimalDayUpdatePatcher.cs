namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

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

    /// <summary>Patch to apply Producer production frequency bonus at max nutrition.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void FarmAnimalDayUpdatePrefix(FarmAnimal __instance)
    {
        if (!__instance.GetOwner().HasProfessionOrLax(Profession.Producer))
        {
            return;
        }

        var nutrition = Data.ReadAs<int>(__instance, DataKeys.ShortTermNutrition);
        if (nutrition < 100)
        {
            return;
        }

        __instance.daysSinceLastLay.Value++;
        if (__instance.GetOwner().HasProfession(Profession.Producer, true) && nutrition >= 200)
        {
            __instance.daysSinceLastLay.Value += 2;
        }
    }

    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? FarmAnimalDayUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch([
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[18])
                ])
                .Move()
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[18]),
                    new CodeInstruction(OpCodes.Call, typeof(FarmAnimalDayUpdatePatcher).RequireMethod(nameof(InheritPotential))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting dodge chance.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static void InheritPotential(FarmAnimal chicken, SObject egg)
    {
        if (!chicken.DoesOwnerHaveProfessionOrLax(Profession.Breeder) ||
            !chicken.type.Value.ContainsAnyOf(Lookups.AnimalReproductiveTypes.EggLayers))
        {
            return;
        }

        var parentInheritedPotential = Data.ReadAs<double>(chicken, DataKeys.InheritedPotential);
        var parentLongTermNutrition = Data.ReadAs<double>(chicken, DataKeys.LongTermNutrition);
        double inheritanceRate;
        int eggInheritedPotential;
        if (parentInheritedPotential < 5000d || !chicken.DoesOwnerHaveProfessionOrLax(Profession.Breeder, true))
        {
            inheritanceRate = Math.Max(Random.Shared.NextGaussian(1d, 0.1), 0d);
            eggInheritedPotential = (int)(parentInheritedPotential + (parentLongTermNutrition * inheritanceRate));
        }
        else
        {
            var alpha = 0.1 * Math.Pow(parentLongTermNutrition / 1000d, 2d);
            inheritanceRate = Math.Max(Random.Shared.NextGaussianSkewed(1d, 0.1, alpha), 0d);
            eggInheritedPotential = (int)(parentInheritedPotential * inheritanceRate);
        }

        Data.Write(egg, DataKeys.InheritedPotential, eggInheritedPotential.ToString());
        Data.Increment(chicken, DataKeys.EggsLaid);
    }

    #endregion injected
}
