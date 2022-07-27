namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using HarmonyLib;
using System.Collections.Generic;
using Utility;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemPopulateContextTagsPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ItemPopulateContextTagsPatch()
    {
        Target = RequireMethod<Item>("_PopulateContextTags");
    }

    #region harmony patches

    /// <summary>Patch to add pair context tag to extended family fish.</summary>
    [HarmonyPostfix]
    private static void ItemPopulateContextTagsPostfix(Item __instance, HashSet<string> tags)
    {
        if (!ObjectLookups.ExtendedFamilyPairs.TryGetValue(__instance.ParentSheetIndex, out var pairId)) return;

        var pair = new SObject(pairId, 1);
        tags.Add("item_" + __instance.SanitizeContextTag(pair.Name));
    }

    #endregion harmony patches
}