namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class FishPondDoFishSpecificWaterColoring : BasePatch
{
    private const int MUTANT_CARP_I = 682, RADIOACTIVE_CARP_I = 901, WHITE_ALGAE_I = 157;

    /// <summary>Construct an instance.</summary>
    internal FishPondDoFishSpecificWaterColoring()
    {
        Original = RequireMethod<FishPond>("doFishSpecificWaterColoring");
    }

    #region harmony patches

    /// <summary>Patch for recolor Mutant and Radioactive Carp ponds.</summary>
    [HarmonyPostfix]
    private static void FishPondDoFishSpecificWaterColoringPostfix(FishPond __instance)
    {
        if (__instance.fishType.Value is MUTANT_CARP_I or RADIOACTIVE_CARP_I)
            __instance.overrideWaterColor.Value = new(40, 255, 40);
        else if (__instance.fishType.Value.IsAlgae())
            __instance.overrideWaterColor.Value =
                __instance.fishType.Value == WHITE_ALGAE_I ? new(246, 244, 118) : new(102, 168, 28);
    }

    #endregion harmony patches
}