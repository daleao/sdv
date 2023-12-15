namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodPullFishFromWaterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodPullFishFromWaterPatcher"/> class.</summary>
    internal FishingRodPullFishFromWaterPatcher()
    {
        this.Target = this.RequireMethod<FishingRod>(nameof(FishingRod.pullFishFromWater));
    }

    #region harmony patches

    /// <summary>Count trash fished by rod.</summary>
    [HarmonyPostfix]
    private static void FishingRodPullFishFromWaterPrefix(int whichFish)
    {
        if (whichFish.IsTrashIndex() && Game1.player.HasProfession(Profession.Conservationist))
        {
            Game1.player.Increment(DataKeys.ConservationistTrashCollectedThisSeason);
        }
    }

    #endregion harmony patches
}
