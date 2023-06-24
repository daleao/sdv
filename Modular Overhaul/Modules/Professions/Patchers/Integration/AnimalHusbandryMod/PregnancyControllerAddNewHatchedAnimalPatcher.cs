namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
[ModRequirement("DIGUS.ANIMALHUSBANDRYMOD")]
internal sealed class PregnancyControllerAddNewHatchedAnimalPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PregnancyControllerAddNewHatchedAnimalPatcher"/> class.</summary>
    internal PregnancyControllerAddNewHatchedAnimalPatcher()
    {
        this.Target = "AnimalHusbandryMod.animals.PregnancyController"
            .ToType()
            .RequireMethod("addNewHatchedAnimal");
    }

    #region harmony patches

    /// <summary>Patch for Rancher husbanded animals to have random starting friendship.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? PregnancyControllerAddNewHatchedAnimalTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: AddNewHatchedAnimalSubroutine(farmAnimal);
        // Before: AnimalHouse animalHouse = building.indoors.Value as AnimalHouse;
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(Building).RequireField(nameof(Building.indoors))),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Nop) }, ILHelper.SearchOption.Previous)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_1),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(PregnancyControllerAddNewHatchedAnimalPatcher)
                                .RequireMethod(nameof(AddNewHatchedAnimalSubroutine))),
                    });
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching Rancher husbanded newborn friendship." +
                  "\n—-- Do NOT report this to Animal Husbandry's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

    private static void AddNewHatchedAnimalSubroutine(FarmAnimal newborn)
    {
        var owner = Game1.getFarmer(newborn.ownerID.Value);
        if (!owner.HasProfession(Profession.Rancher))
        {
            return;
        }

        newborn.friendshipTowardFarmer.Value =
            200 + new Random(newborn.myID.GetHashCode()).Next(-50, 51);
    }

    #endregion private methods
}
