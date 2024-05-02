namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodPullFishFromWaterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodPullFishFromWaterPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishingRodPullFishFromWaterPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishingRod>(nameof(FishingRod.pullFishFromWater));
    }

    #region harmony patches

    /// <summary>Count trash fished by rod.</summary>
    [HarmonyPostfix]
    private static void FishingRodPullFishFromWaterPrefix(string fishId)
    {
        if (fishId.IsTrashId() && Game1.player.HasProfession(Profession.Conservationist))
        {
            Data.Increment(Game1.player, DataKeys.ConservationistTrashCollectedThisSeason);
        }
    }

    #endregion harmony patches
}
