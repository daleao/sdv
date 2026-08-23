namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Professions.Framework.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class BuildingFinishConstructionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BuildingFinishConstructionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal BuildingFinishConstructionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Building>(nameof(Building.FinishConstruction));
    }

    #region harmony patches

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void BuildingFinishConstructionPostfix(Building __instance)
    {
        bool areThereAnyPrestigedBreeders = false,
            areThereAnyPrestigedProducers = false,
            areThereAnyPipers = false;
        foreach (var farmer in Game1.getAllFarmers())
        {
            if (farmer.HasProfession(Profession.Breeder, true))
            {
                areThereAnyPrestigedBreeders = true;
            }
            else if (farmer.HasProfession(Profession.Producer, true))
            {
                areThereAnyPrestigedProducers = true;
            }

            if (farmer.HasProfession(Profession.Piper))
            {
                areThereAnyPipers = true;
            }
        }

        __instance.ApplyProfessionRules(areThereAnyPrestigedBreeders, areThereAnyPrestigedProducers, areThereAnyPipers);
    }

    #endregion harmony patches
}
