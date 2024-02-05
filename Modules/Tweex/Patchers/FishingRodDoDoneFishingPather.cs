namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodDoDoneFishingPather : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodDoDoneFishingPather"/> class.</summary>
    internal FishingRodDoDoneFishingPather()
    {
        this.Target = this.RequireMethod<FishingRod>("doDoneFishing");
    }

    #region harmony patches

    [HarmonyPrefix]
    private static void FishingRodDoDoneFishingPrefix(bool ___lastCatchWasJunk, ref bool consumeBaitAndTackle)
    {
        if (TweexModule.Config.TrashDoesNotConsumeBait && ___lastCatchWasJunk)
        {
            consumeBaitAndTackle = false;
        }
    }

    #endregion harmony patches
}
