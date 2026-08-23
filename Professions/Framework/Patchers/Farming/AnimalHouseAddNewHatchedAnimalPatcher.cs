namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using xTile.Layers;

#endregion using directives

[UsedImplicitly]
internal sealed class AnimalHouseAddNewHatchedAnimalPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AnimalHouseAddNewHatchedAnimalPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal AnimalHouseAddNewHatchedAnimalPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<AnimalHouse>(nameof(AnimalHouse.addNewHatchedAnimal));
    }

    #region harmony patches

    /// <summary>Patch for Rancher newborn animals to have random starting friendship.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? AnimalHouseAddNewHatchedAnimalTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                // egg-layers
                .PatternMatch([
                    new CodeInstruction(OpCodes.Newobj, typeof(FarmAnimal).RequireConstructor(3)),
                    new CodeInstruction(OpCodes.Stloc_S),
                ])
                .Move()
                .GetOperand(out var localIndex)
                .Move()
                .Insert([
                    new CodeInstruction(OpCodes.Ldloc_S, (LocalBuilder)localIndex),
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(AnimalHouseAddNewHatchedAnimalPatcher).RequireMethod(
                            nameof(InheritPotential), [typeof(FarmAnimal), typeof(SObject)]))
                ])
                // mammals
                .PatternMatch([
                    new CodeInstruction(OpCodes.Newobj, typeof(FarmAnimal).RequireConstructor(3)),
                    new CodeInstruction(OpCodes.Stloc_S),
                ])
                .Move()
                .GetOperand(out localIndex)
                .Move()
                .Insert([
                    new CodeInstruction(OpCodes.Ldloc_S, (LocalBuilder)localIndex),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(AnimalHouseAddNewHatchedAnimalPatcher).RequireMethod(
                            nameof(InheritPotential), [typeof(FarmAnimal)]))
                ]);
        }
        catch (Exception ex)
        {
            Log.E(
                "Failed injecting inherit potential." + $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static void InheritPotential(FarmAnimal newborn)
    {
        if (!newborn.DoesOwnerHaveProfessionOrLax(Profession.Breeder) ||
            !newborn.type.Value.ContainsAnyOf(Lookups.AnimalReproductiveTypes.Mammals))
        {
            return;
        }

        var parent = Utility.getAnimal(newborn.parentId.Value);
        var parentInheritedPotential = Data.ReadAs<double>(parent, DataKeys.InheritedPotential);
        var parentLongTermNutrition = Data.ReadAs<double>(parent, DataKeys.LongTermNutrition);
        double inheritanceRate;
        int newbornInheritedPotential;
        if (parentInheritedPotential < 5000d || !newborn.DoesOwnerHaveProfessionOrLax(Profession.Breeder, true))
        {
            inheritanceRate = Math.Max(Random.Shared.NextGaussian(1d, 0.1), 0d);
            newbornInheritedPotential = (int)(parentInheritedPotential + (parentLongTermNutrition * inheritanceRate));
        }
        else
        {
            var alpha = 0.1 * Math.Pow(parentLongTermNutrition / 1000d, 2d);
            inheritanceRate = Math.Max(Random.Shared.NextGaussianSkewed(1d, 0.1, alpha), 0d);
            newbornInheritedPotential = (int)(parentInheritedPotential * inheritanceRate);
        }

        Data.Write(newborn, DataKeys.InheritedPotential, newbornInheritedPotential.ToString());
        Data.Increment(parent, DataKeys.Pregnancies);
    }

    private static void InheritPotential(FarmAnimal newborn, SObject egg)
    {
        if (!newborn.DoesOwnerHaveProfessionOrLax(Profession.Breeder) ||
            !newborn.type.Value.ContainsAnyOf(Lookups.AnimalReproductiveTypes.EggLayers))
        {
            return;
        }

        var potentialToInherit = Data.Read(egg, DataKeys.InheritedPotential);
        Data.Write(newborn, DataKeys.InheritedPotential, potentialToInherit);
    }

    #endregion injected
}
