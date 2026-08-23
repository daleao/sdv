namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using System.Reflection;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmAnimalGetProduceIdPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmAnimalGetProduceIdPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FarmAnimalGetProduceIdPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FarmAnimal>(nameof(FarmAnimal.GetProduceID));
    }

    #region harmony patches

    /// <summary>Patch to randomly breed Blue Chickens.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void FarmAnimalGetProduceIdPostfix(FarmAnimal __instance, ref string? __result, Random r)
    {
        if (string.IsNullOrEmpty(__result) || !__instance.DoesOwnerHaveProfessionOrLax(Profession.Breeder))
        {
            return;
        }

        try
        {
            var parentInheritedPotential = Data.ReadAs<int>(__instance, DataKeys.InheritedPotential);
            var parentLongTermNutrition = Data.ReadAs<int>(__instance, DataKeys.LongTermNutrition);
            if (parentLongTermNutrition < 1000 || parentInheritedPotential < 1000)
            {
                return;
            }

            if (r.NextBool(0.01))
            {
                switch (__result)
                {
                    case QIDs.Egg_Brown or QIDs.Egg_White:
                        __result = BlueEggId;
                        break;
                    case QIDs.LargeEgg_Brown or QIDs.LargeEgg_White:
                        __result = LargeBlueEggId;
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches
}
