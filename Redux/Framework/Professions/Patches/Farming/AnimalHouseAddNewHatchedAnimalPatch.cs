namespace DaLion.Redux.Framework.Professions.Patches.Farming;

#region using directives

using System.Linq;
using DaLion.Redux.Framework.Professions.Extensions;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class AnimalHouseAddNewHatchedAnimalPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="AnimalHouseAddNewHatchedAnimalPatch"/> class.</summary>
    internal AnimalHouseAddNewHatchedAnimalPatch()
    {
        this.Target = this.RequireMethod<AnimalHouse>(nameof(AnimalHouse.addNewHatchedAnimal));
    }

    #region harmony patches

    /// <summary>Patch for Rancher newborn animals to have random starting friendship.</summary>
    [HarmonyPostfix]
    private static void AnimalHouseAddNewHatchedAnimalPostfix(AnimalHouse __instance)
    {
        var owner = Game1.getFarmer(__instance.getBuilding().owner.Value);
        if (!owner.HasProfession(Profession.Rancher))
        {
            return;
        }

        var newborn = __instance.Animals.Values.Last();
        if (newborn is null || newborn.age.Value != 0 || newborn.friendshipTowardFarmer.Value != 0 ||
            newborn.ownerID.Value != owner.UniqueMultiplayerID)
        {
            return;
        }

        newborn.friendshipTowardFarmer.Value =
            200 + new Random(__instance.GetHashCode() + newborn.GetHashCode()).Next(-50, 51);
    }

    #endregion harmony patches
}
