namespace DaLion.Stardew.Professions.Framework.Patches.Farming;

#region using directives

using System;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using DaLion.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class AnimalHouseAddNewHatchedAnimalPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal AnimalHouseAddNewHatchedAnimalPatch()
    {
        Target = RequireMethod<AnimalHouse>(nameof(AnimalHouse.addNewHatchedAnimal));
    }

    #region harmony patches

    /// <summary>Patch for Rancher newborn animals to have random starting friendship.</summary>
    [HarmonyPostfix]
    private static void AnimalHouseAddNewHatchedAnimalPostfix(AnimalHouse __instance)
    {
        var owner = Game1.getFarmer(__instance.getBuilding().owner.Value);
        if (!owner.HasProfession(Profession.Rancher)) return;

        var newborn = __instance.Animals.Values.Last();
        if (newborn is null || newborn.age.Value != 0 || newborn.friendshipTowardFarmer.Value != 0 || newborn.ownerID.Value != owner.UniqueMultiplayerID) return;

        newborn.friendshipTowardFarmer.Value =
            200 + new Random(__instance.GetHashCode() + newborn.GetHashCode()).Next(-50, 51);
    }

    #endregion harmony patches
}