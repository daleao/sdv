namespace DaLion.Overhaul;

#region using directives

using System.Collections.Generic;
using System.Collections.Immutable;
using DaLion.Shared.Constants;

#endregion using directives

/// <summary>Holds maps which may be referenced by different modules.</summary>
public static class Maps
{
    /// <summary>Gets a map from a legendary fish ID to that of its corresponding extended family pair.</summary>
    public static ImmutableDictionary<int, int> ExtendedFamilyPairs { get; } = new Dictionary<int, int>
    {
        { ObjectIds.Crimsonfish, ObjectIds.SonOfCrimsonfish },
        { ObjectIds.Angler, ObjectIds.MsAngler },
        { ObjectIds.Legend, ObjectIds.LegendII },
        { ObjectIds.MutantCarp, ObjectIds.RadioactiveCarp },
        { ObjectIds.Glacierfish, ObjectIds.GlacierfishJr },
        { ObjectIds.SonOfCrimsonfish, ObjectIds.Crimsonfish },
        { ObjectIds.MsAngler, ObjectIds.Angler },
        { ObjectIds.LegendII, ObjectIds.Legend },
        { ObjectIds.RadioactiveCarp, ObjectIds.MutantCarp },
        { ObjectIds.GlacierfishJr, ObjectIds.Glacierfish },
    }.ToImmutableDictionary();

    /// <summary>Gets a map from stone node ID to its corresponding resource ID.</summary>
    public static ImmutableDictionary<int, int> ResourceFromNode { get; } = new Dictionary<int, int>
    {
        { ObjectIds.Stone_Node_Regular0, ObjectIds.Stone },
        { ObjectIds.Stone_Node_Regular1, ObjectIds.Stone },
        { ObjectIds.Stone_Node_Regular2, ObjectIds.Stone },
        { ObjectIds.Stone_Node_Regular3, ObjectIds.Stone },
        { ObjectIds.Stone_Node_Regular4, ObjectIds.Stone },
        { ObjectIds.Stone_Node_Copper0, ObjectIds.CopperOre },
        { ObjectIds.Stone_Node_Copper1, ObjectIds.CopperOre },
        { ObjectIds.Stone_Node_Iron0, ObjectIds.IronOre },
        { ObjectIds.Stone_Node_Iron1, ObjectIds.IronOre },
        { ObjectIds.Stone_Node_Gold, ObjectIds.GoldOre },
        { ObjectIds.Stone_Node_Iridium, ObjectIds.IridiumOre },
        { ObjectIds.Stone_Node_Radioactive, ObjectIds.RadioactiveOre },
        { ObjectIds.Stone_Node_Geode, ObjectIds.Geode },
        { ObjectIds.Stone_Node_FrozenGeode, ObjectIds.FrozenGeode },
        { ObjectIds.Stone_Node_MagmaGeode, ObjectIds.MagmaGeode },
        { ObjectIds.Stone_Node_OmniGeode, ObjectIds.OmniGeode },
        { ObjectIds.Stone_Node_Amethyst, ObjectIds.Amethyst },
        { ObjectIds.Stone_Node_Topaz, ObjectIds.Topaz },
        { ObjectIds.Stone_Node_Emerald, ObjectIds.Emerald },
        { ObjectIds.Stone_Node_Aquamarine, ObjectIds.Aquamarine },
        { ObjectIds.Stone_Node_Jade, ObjectIds.Jade },
        { ObjectIds.Stone_Node_Ruby, ObjectIds.Ruby },
        { ObjectIds.Stone_Node_Diamond, ObjectIds.Diamond },
        { ObjectIds.Stone_Node_Mussel, ObjectIds.Mussel },
        { ObjectIds.Stone_Node_BoneFragment0, ObjectIds.BoneFragment },
        { ObjectIds.Stone_Node_BoneFragment1, ObjectIds.BoneFragment },
        { ObjectIds.Stone_Node_Clay, ObjectIds.Clay },
        { ObjectIds.Stone_Node_CinderShard0, ObjectIds.CinderShard },
        { ObjectIds.Stone_Node_CinderShard1, ObjectIds.CinderShard },
    }.ToImmutableDictionary();
}
