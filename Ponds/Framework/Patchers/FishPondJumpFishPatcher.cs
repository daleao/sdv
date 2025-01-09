namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondJumpFishPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondJumpFishPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishPondJumpFishPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.JumpFish));
    }

    #region harmony patches

    /// <summary>Prevent un-immersive jumping algae.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool FishPondJumpFishPrefix(FishPond __instance, ref bool __result)
    {
        if (!__instance.fishType.Value.IsAlgaeId())
        {
            return true; // run original logic
        }

        __result = false;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
