namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using System.IO;
using System.Linq;
using System.Reflection;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using DaLion.Shared.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
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

    /// <summary>Decrement total Fish Pond quality ratings.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void FishingRodPullFishFromWaterPrefix(
        FishingRod __instance, ref string fishId, ref int fishQuality, bool fromFishPond)
    {
        if (!fromFishPond || fishId.IsTrashId())
        {
            return;
        }

        try
        {
            var (x, y) = Reflector
                .GetUnboundMethodDelegate<Func<FishingRod, Vector2>>(__instance, "calculateBobberTile")
                .Invoke(__instance);
            var pond = Game1.currentLocation.buildings.OfType<FishPond>().FirstOrDefault(p =>
                x > p.tileX.Value && x < p.tileX.Value + p.tilesWide.Value - 1 &&
                y > p.tileY.Value && y < p.tileY.Value + p.tilesHigh.Value - 1);
            if (pond is null || pond.FishCount < 0)
            {
                return;
            }

            if (pond.HasAlgae())
            {
                PullAlgae(pond, ref fishId, ref fishQuality);
            }
            else
            {
                PullFish(pond, ref fishId, ref fishQuality);
            }
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches

    #region handlers

    private static void PullAlgae(FishPond pond, ref string id, ref int quality)
    {
        quality = SObject.lowQuality;
        try
        {
            var algae = pond.ParsePondFishes();
            var pulled = algae.Choose();
            id = pulled.Id;
            algae.Remove(pulled);
            if (algae.Count != pond.FishCount)
            {
                ThrowHelper.ThrowInvalidDataException(
                    $"Mismatch between algae population data and actual population:" +
                    $"\n\t- Population: {pond.FishCount}\n\t- Data: {Data.Read(pond, DataKeys.PondFish)}");
            }

            Data.Write(pond, DataKeys.PondFish, string.Join(';', algae));
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            pond.ResetPondFishData();
        }
    }

    private static void PullFish(FishPond pond, ref string id, ref int quality)
    {
        try
        {
            var fishes = pond.ParsePondFishes();
            fishes.SortDescending();
            PondFish pulled;
            if (pond.HasBossFish())
            {
                if (Data.ReadAs<int>(pond, "FamilyLivingHere") > 0 && Game1.random.NextBool())
                {
                    pulled = fishes.Last(f => $"(O){f.Id}" == Lookups.FamilyPairs[$"(O){pond.fishType.Value}"]);
                    Data.Increment(pond, "FamilyLivingHere", -1);
                }
                else
                {
                    pulled = fishes.Last(f => f.Id == pond.fishType.Value);
                }
            }
            else
            {
                pulled = fishes.Last();
            }

            (id, quality) = pulled;
            fishes.Remove(pulled);
            if (fishes.Count != pond.FishCount)
            {
                ThrowHelper.ThrowInvalidDataException(
                    $"Mismatch between fish population data and actual population:" +
                    $"\n\t- Population: {pond.FishCount}\n\t- Data: {Data.Read(pond, DataKeys.PondFish)}");
            }

            Data.Write(pond, DataKeys.PondFish, string.Join(';', fishes));
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            pond.ResetPondFishData();
        }
    }

    #endregion handlers
}
