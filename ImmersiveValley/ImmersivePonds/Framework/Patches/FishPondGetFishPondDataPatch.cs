#if DEBUG
namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common.Extensions.Reflection;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondGetFishPondDataPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondGetFishPondDataPatch()
    {
        Target = RequireMethod<FishPond>(nameof(FishPond.GetFishPondData));
    }

    #region harmony patches

    /// <summary>Replace single production with multi-yield production.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static bool FishPondGetFishPondDataPrefix(FishPond __instance, ref FishPondData? __result)
    {
        if (__instance.fishType.Value <= 0)
        {
            __result = null;
            return false;
        }

        var list = Game1.content.Load<List<FishPondData>>("Data\\FishPondData");
        var fish_item = __instance.GetFishObject();
        foreach (var data_entry in list)
        {
            if (data_entry.RequiredTags.Any(required_tag => !fish_item.HasContextTag(required_tag))) continue;

            if (data_entry.SpawnTime == -1)
            {
                data_entry.SpawnTime = fish_item.Price switch
                {
                    <= 30 => 1,
                    <= 80 => 2,
                    <= 120 => 3,
                    <= 250 => 4,
                    _ => 5
                };
            }

            __instance.GetType().RequireField("_fishPondData").SetValue(__instance, data_entry);
            __result = data_entry;
            return false;
        }

        __result = null;
        return false;
    }

    #endregion harmony patches
}

#endif