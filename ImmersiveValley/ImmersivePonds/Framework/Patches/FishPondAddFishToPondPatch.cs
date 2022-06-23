namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System.IO;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;

using Common;
using Common.Harmony;
using Common.Extensions;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondAddFishToPondPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondAddFishToPondPatch()
    {
        Target = RequireMethod<FishPond>("addFishToPond");
    }

    #region harmony patches

    /// <summary>Distinguish extended family pairs + increment total Fish Pond quality ratings.</summary>
    [HarmonyPostfix]
    private static void FishPondAddFishToPondPostfix(FishPond __instance, SObject fish)
    {
        try
        {
            if (fish.HasContextTag("fish_legendary") && fish.ParentSheetIndex != __instance.fishType.Value)
            {
                var familyQualities = __instance
                    .ReadData("FamilyQualities", $"{__instance.ReadDataAs<int>("FamilyLivingHere")},0,0,0")
                    .ParseList<int>()!;
                if (familyQualities.Count != 4 ||
                    familyQualities.Sum() != __instance.ReadDataAs<int>("FamilyLivingHere"))
                    throw new InvalidDataException("FamilyQualities data had incorrect number of values.");

                ++familyQualities[fish.Quality == 4 ? 3 : fish.Quality];
                __instance.IncrementData<int>("FamilyLivingHere");
                __instance.WriteData("FamilyQualities", string.Join(',', familyQualities));
            }
            else if (fish.IsAlgae())
            {
                switch (fish.ParentSheetIndex)
                {
                    case Constants.SEAWEED_INDEX_I:
                        __instance.IncrementData<int>("SeaweedLivingHere");
                        break;
                    case Constants.GREEN_ALGAE_INDEX_I:
                        __instance.IncrementData<int>("GreenAlgaeLivingHere");
                        break;
                    case Constants.WHITE_ALGAE_INDEX_I:
                        __instance.IncrementData<int>("WhiteAlgaeLivingHere");
                        break;
                }
            }
            else
            {
                var fishQualities = __instance.ReadData("FishQualities",
                        $"{__instance.FishCount - __instance.ReadDataAs<int>("FamilyLivingHere") - 1},0,0,0") // already added at this point, so consider - 1
                    .ParseList<int>()!;
                if (fishQualities.Count != 4 || fishQualities.Any(q => 0 > q || q > __instance.FishCount - 1))
                    throw new InvalidDataException("FishQualities data had incorrect number of values.");

                ++fishQualities[fish.Quality == 4 ? 3 : fish.Quality];
                __instance.WriteData("FishQualities", string.Join(',', fishQualities));
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            __instance.WriteData("FishQualities", $"{__instance.FishCount},0,0,0");
            __instance.WriteData("FamilyQualities", null);
            __instance.WriteData("FamilyLivingHere", null);
        }
    }

    #endregion harmony patches
}