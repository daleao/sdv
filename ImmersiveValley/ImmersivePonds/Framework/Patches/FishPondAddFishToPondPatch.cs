namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common;
using Common.Extensions;
using Common.ModData;
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
                var familyQualities = ModDataIO
                    .Read(__instance, "FamilyQualities", $"{ModDataIO.Read<int>(__instance, "FamilyLivingHere")},0,0,0")
                    .ParseList<int>()!;
                if (familyQualities.Count != 4 ||
                    familyQualities.Sum() != ModDataIO.Read<int>(__instance, "FamilyLivingHere"))
                    throw new InvalidDataException("FamilyQualities data had incorrect number of values.");

                ++familyQualities[fish.Quality == 4 ? 3 : fish.Quality];
                ModDataIO.Increment<int>(__instance, "FamilyLivingHere");
                ModDataIO.Write(__instance, "FamilyQualities", string.Join(',', familyQualities));
            }
            else if (fish.IsAlgae())
            {
                switch (fish.ParentSheetIndex)
                {
                    case Constants.SEAWEED_INDEX_I:
                        ModDataIO.Increment<int>(__instance, "SeaweedLivingHere");
                        break;
                    case Constants.GREEN_ALGAE_INDEX_I:
                        ModDataIO.Increment<int>(__instance, "GreenAlgaeLivingHere");
                        break;
                    case Constants.WHITE_ALGAE_INDEX_I:
                        ModDataIO.Increment<int>(__instance, "WhiteAlgaeLivingHere");
                        break;
                }
            }
            else
            {
                var fishQualities = ModDataIO.Read(__instance, "FishQualities",
                        $"{__instance.FishCount - ModDataIO.Read<int>(__instance, "FamilyLivingHere") - 1},0,0,0") // already added at this point, so consider - 1
                    .ParseList<int>()!;
                if (fishQualities.Count != 4 || fishQualities.Any(q => 0 > q || q > __instance.FishCount - 1))
                    throw new InvalidDataException("FishQualities data had incorrect number of values.");

                ++fishQualities[fish.Quality == 4 ? 3 : fish.Quality];
                ModDataIO.Write(__instance, "FishQualities", string.Join(',', fishQualities));
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            ModDataIO.Write(__instance, "FishQualities", $"{__instance.FishCount},0,0,0");
            ModDataIO.Write(__instance, "FamilyQualities", null);
            ModDataIO.Write(__instance, "FamilyLivingHere", null);
        }
    }

    #endregion harmony patches
}