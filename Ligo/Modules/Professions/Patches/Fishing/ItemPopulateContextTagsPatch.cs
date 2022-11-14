namespace DaLion.Ligo.Modules.Professions.Patches.Fishing;

#region using directives

using System.Collections.Generic;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemPopulateContextTagsPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ItemPopulateContextTagsPatch"/> class.</summary>
    internal ItemPopulateContextTagsPatch()
    {
        this.Target = this.RequireMethod<Item>("_PopulateContextTags");
    }

    /// <summary>Gets extended family pairs by legendary fish id.</summary>
    private static IReadOnlyDictionary<int, int> ExtendedFamilyPairs { get; } = new Dictionary<int, int>
    {
        { 898, 159 },
        { 899, 160 },
        { 900, 163 },
        { 901, 682 },
        { 902, 775 },
    };

    #region harmony patches

    /// <summary>Patch to add pair context tag to extended family fish.</summary>
    [HarmonyPostfix]
    private static void ItemPopulateContextTagsPostfix(Item __instance, HashSet<string> tags)
    {
        if (!ExtendedFamilyPairs.TryGetValue(__instance.ParentSheetIndex, out var pairId))
        {
            return;
        }

        var pair = new SObject(pairId, 1);
        tags.Add("item_" + __instance.SanitizeContextTag(pair.Name));
    }

    #endregion harmony patches
}
