namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class FishPondJumpFishPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondJumpFishPatch()
    {
        Original = RequireMethod<FishPond>(nameof(FishPond.JumpFish));
    }

    #region harmony patches

    /// <summary>Patch to prevent jumping algae.</summary>
    [HarmonyPrefix]
    private static bool FishPondJumpFishPreix(FishPond __instance, ref bool __result)
    {
        if (!__instance.fishType.Value.IsAlgae()) return true; // run original logic

        __result = false;
        return false; // don't run original logic
    }

    #endregion harmony patches
}