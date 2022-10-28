namespace DaLion.Redux.Ponds.Patches;

#region using directives

using System.IO;
using System.Linq;
using DaLion.Redux.Core.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using StardewValley.Buildings;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondAddFishToPondPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FishPondAddFishToPondPatch"/> class.</summary>
    internal FishPondAddFishToPondPatch()
    {
        this.Target = this.RequireMethod<FishPond>("addFishToPond");
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
                    .Read(DataFields.FamilyQualities, $"{__instance.Read<int>(DataFields.FamilyLivingHere)},0,0,0")
                    .ParseList<int>();
                if (familyQualities.Count != 4 ||
                    familyQualities.Sum() != __instance.Read<int>(DataFields.FamilyLivingHere))
                {
                    ThrowHelper.ThrowInvalidDataException("FamilyQualities data had incorrect number of values.");
                }

                ++familyQualities[fish.Quality == 4 ? 3 : fish.Quality];
                __instance.Increment(DataFields.FamilyLivingHere);
                __instance.Write(DataFields.FamilyQualities, string.Join(',', familyQualities));
            }
            else if (fish.IsAlgae())
            {
                switch (fish.ParentSheetIndex)
                {
                    case Constants.SeaweedIndex:
                        __instance.Increment(DataFields.SeaweedLivingHere);
                        break;
                    case Constants.GreenAlgaeIndex:
                        __instance.Increment(DataFields.GreenAlgaeLivingHere);
                        break;
                    case Constants.WhiteAlgaeIndex:
                        __instance.Increment(DataFields.WhiteAlgaeLivingHere);
                        break;
                }
            }
            else
            {
                var fishQualities = __instance.Read(DataFields.FishQualities, $"{__instance.FishCount - __instance.Read<int>(DataFields.FamilyLivingHere) - 1},0,0,0") // already added at this point, so consider - 1
                    .ParseList<int>();
                if (fishQualities.Count != 4 || fishQualities.Any(q => q < 0 || q > __instance.FishCount - 1))
                {
                    ThrowHelper.ThrowInvalidDataException("FishQualities data had incorrect number of values.");
                }

                ++fishQualities[fish.Quality == 4 ? 3 : fish.Quality];
                __instance.Write(DataFields.FishQualities, string.Join(',', fishQualities));
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            __instance.Write(DataFields.FishQualities, $"{__instance.FishCount},0,0,0");
            __instance.Write(DataFields.FamilyQualities, null);
            __instance.Write(DataFields.FamilyLivingHere, null);
        }
    }

    #endregion harmony patches
}
