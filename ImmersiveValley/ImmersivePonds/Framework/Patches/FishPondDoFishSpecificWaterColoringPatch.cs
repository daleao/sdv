namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;

using Common.Harmony;
using Common.Extensions;
using Common.Extensions.Xna;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondDoFishSpecificWaterColoringPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondDoFishSpecificWaterColoringPatch()
    {
        Target = RequireMethod<FishPond>("doFishSpecificWaterColoring");
    }

    #region harmony patches

    /// <summary>Recolor for algae/seaweed.</summary>
    [HarmonyPostfix]
    private static void FishPondDoFishSpecificWaterColoringPostfix(FishPond __instance)
    {
        if (__instance.fishType.Value.IsAlgae())
        {
            var shift = -5 - 3 * __instance.FishCount;
            __instance.overrideWaterColor.Value = new Color(60, 126, 150).ShiftHue(shift);
        }
        else if (__instance.GetFishObject().Name.ContainsAnyOf("Mutant", "Radioactive"))
        {
            __instance.overrideWaterColor.Value = new(40, 255, 40);
        }
    }

    #endregion harmony patches
}