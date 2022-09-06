namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common;
using Common.Extensions;
using Common.Extensions.Stardew;
using Extensions;
using HarmonyLib;
using StardewValley.Buildings;
using System.IO;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondAddFishToPondPatch : Common.Harmony.HarmonyPatch
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
                    .Read("FamilyQualities", $"{__instance.Read<int>("FamilyLivingHere")},0,0,0")
                    .ParseList<int>();
                if (familyQualities.Count != 4 ||
                    familyQualities.Sum() != __instance.Read<int>("FamilyLivingHere"))
                    ThrowHelper.ThrowInvalidDataException("FamilyQualities data had incorrect number of values.");

                ++familyQualities[fish.Quality == 4 ? 3 : fish.Quality];
                __instance.Increment("FamilyLivingHere");
                __instance.Write("FamilyQualities", string.Join(',', familyQualities));
            }
            else if (fish.IsAlgae())
            {
                switch (fish.ParentSheetIndex)
                {
                    case Constants.SEAWEED_INDEX_I:
                        __instance.Increment("SeaweedLivingHere");
                        break;
                    case Constants.GREEN_ALGAE_INDEX_I:
                        __instance.Increment("GreenAlgaeLivingHere");
                        break;
                    case Constants.WHITE_ALGAE_INDEX_I:
                        __instance.Increment("WhiteAlgaeLivingHere");
                        break;
                }
            }
            else
            {
                var fishQualities = __instance.Read("FishQualities",
                        $"{__instance.FishCount - __instance.Read<int>("FamilyLivingHere") - 1},0,0,0") // already added at this point, so consider - 1
                    .ParseList<int>();
                if (fishQualities.Count != 4 || fishQualities.Any(q => 0 > q || q > __instance.FishCount - 1))
                    ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");

                ++fishQualities[fish.Quality == 4 ? 3 : fish.Quality];
                __instance.Write("FishQualities", string.Join(',', fishQualities));
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            __instance.Write("FishQualities", $"{__instance.FishCount},0,0,0");
            __instance.Write("FamilyQualities", null);
            __instance.Write("FamilyLivingHere", null);
        }
    }

    #endregion harmony patches
}