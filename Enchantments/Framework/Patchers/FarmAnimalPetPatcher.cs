namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmAnimalPetPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmAnimalPetPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FarmAnimalPetPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FarmAnimal>(nameof(FarmAnimal.pet));
    }

    #region harmony patches

    /// <summary>Prevent selling transmuted animals.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool FarmAnimalPetPrefix(FarmAnimal __instance, bool is_auto_pet)
    {
        return is_auto_pet || !__instance.wasPet.Value || __instance.home is not null;
    }

    #endregion harmony patches
}
