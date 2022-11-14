#if DEBUG
namespace DaLion.Ligo.Modules.Ponds.Patches;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;
using Shared.Harmony;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondGetFishPondDataPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondGetFishPondDataPatcher"/> class.</summary>
    internal FishPondGetFishPondDataPatcher()
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.GetFishPondData));
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
        var fish = __instance.GetFishObject();
        foreach (var entry in list)
        {
            if (entry.RequiredTags.Any(required => !fish.HasContextTag(required)))
            {
                continue;
            }

            if (entry.SpawnTime == -1)
            {
                entry.SpawnTime = fish.Price switch
                {
                    <= 30 => 1,
                    <= 80 => 2,
                    <= 120 => 3,
                    <= 250 => 4,
                    _ => 5,
                };
            }

            __instance.GetType().RequireField("_fishPondData").SetValue(__instance, entry);
            __result = entry;
            return false;
        }

        __result = null;
        return false;
    }

    #endregion harmony patches
}

#endif
