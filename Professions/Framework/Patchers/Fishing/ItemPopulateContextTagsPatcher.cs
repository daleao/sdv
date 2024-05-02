namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemPopulateContextTagsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ItemPopulateContextTagsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ItemPopulateContextTagsPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Item>("_PopulateContextTags");
    }

    /// <summary>Gets extended family pairs by legendary fish id.</summary>
    private static Dictionary<string, string> ExtendedFamilyPairs { get; } = new()
    {
        { "(O)898", "(O)159" },
        { "(O)899", "(O)160" },
        { "(O)900", "(O)163" },
        { "(O)901", "(O)682" },
        { "(O)902", "(O)775" },
    };

    #region harmony patches

    /// <summary>Patch to add pair context tag to extended family fish.</summary>
    [HarmonyPostfix]
    private static void ItemPopulateContextTagsPostfix(Item __instance, HashSet<string> tags)
    {
        if (!ExtendedFamilyPairs.TryGetValue(__instance.QualifiedItemId, out var pairId))
        {
            return;
        }

        var pair = ItemRegistry.Create<SObject>(pairId);
        tags.Add("item_" + ItemContextTagManager.SanitizeContextTag(pair.Name));
    }

    #endregion harmony patches
}
