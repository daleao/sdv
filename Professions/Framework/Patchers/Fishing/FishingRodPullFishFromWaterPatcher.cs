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
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FishingRodPullFishFromWaterPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FishingRod>(nameof(FishingRod.pullFishFromWater));
    }

    #region harmony patches

    /// <summary>Count trash fished by rod.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void FishingRodPullFishFromWaterPostfix(FishingRod __instance, string fishId, bool wasPerfect, bool fromFishPond, bool isBossFish)
    {
        if (!fromFishPond && fishId.IsTrashId() && Game1.player.HasProfession(Profession.Conservationist))
        {
            Data.Increment(Game1.player, DataKeys.ConservationistTrashCollectedThisSeason);
        }

        if (!__instance.lastUser.HasProfession(Profession.Angler, true))
        {
            return;
        }

        State.FishingChain += wasPerfect ? 2 : 1;
    }

    #endregion harmony patches
}
