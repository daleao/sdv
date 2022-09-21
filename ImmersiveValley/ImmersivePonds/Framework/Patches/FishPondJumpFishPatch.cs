namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using DaLion.Stardew.Ponds.Extensions;
using HarmonyLib;
using StardewValley.Buildings;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondJumpFishPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FishPondJumpFishPatch"/> class.</summary>
    internal FishPondJumpFishPatch()
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.JumpFish));
    }

    #region harmony patches

    /// <summary>Prevent un-immersive jumping algae.</summary>
    [HarmonyPrefix]
    private static bool FishPondJumpFishPrefix(FishPond __instance, ref bool __result)
    {
        if (!__instance.fishType.Value.IsAlgaeIndex())
        {
            return true; // run original logic
        }

        __result = false;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
