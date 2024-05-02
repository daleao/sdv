namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class AnimalHouseAddNewHatchedAnimalPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AnimalHouseAddNewHatchedAnimalPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal AnimalHouseAddNewHatchedAnimalPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<AnimalHouse>(nameof(AnimalHouse.addNewHatchedAnimal));
    }

    #region harmony patches

    /// <summary>Patch for Rancher newborn animals to have random starting friendship.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? AnimalHouseAddNewHatchedAnimalTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .ForEach(
                    [
                        new CodeInstruction(OpCodes.Newobj, typeof(FarmAnimal).RequireConstructor(3)),
                        new CodeInstruction(OpCodes.Stloc_S),
                    ],
                    _ => helper
                        .Move()
                        .GetOperand(out var localIndex)
                        .Move()
                        .Insert([
                            new CodeInstruction(OpCodes.Ldloc_S, (LocalBuilder)localIndex),
                            new CodeInstruction(
                                OpCodes.Call,
                                typeof(AnimalHouseAddNewHatchedAnimalPatcher).RequireMethod(
                                    nameof(AddRancherStartingFriendship)))
                        ]));
        }
        catch (Exception ex)
        {
            Log.E(
                "Failed injecting Rancher starting friendship." + $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static void AddRancherStartingFriendship(FarmAnimal newborn)
    {
        var owner = newborn.GetOwner();
        if (!owner.HasProfessionOrLax(Profession.Rancher))
        {
            return;
        }

        newborn.friendshipTowardFarmer.Value = 200 + Game1.random.Next(-50, 51);
        if (owner.HasProfession(Profession.Breeder, true))
        {
            Data.Write(newborn, DataKeys.BredByPrestigedBreeder, true.ToString());
        }
    }

    #endregion injections
}
