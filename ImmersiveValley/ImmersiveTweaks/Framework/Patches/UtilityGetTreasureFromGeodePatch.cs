namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class UtilityGetTreasureFromGeodePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal UtilityGetTreasureFromGeodePatch()
    {
        Target = RequireMethod<Utility>(nameof(Utility.getTreasureFromGeode));
    }

    #region harmony patches

    /// <summary>Precious rocks preserve geode quality.</summary>
    [HarmonyPostfix]
    private static void UtilityGetTreasureFromGeodePostfix(Item __result, Item geode)
    {
        if (ModEntry.ProfessionsApi is not null && __result is SObject resultObj && resultObj.IsPreciousRock() &&
            geode is SObject { Quality: > 0 } geodeObj) resultObj.Quality = geodeObj.Quality;
    }

    #endregion harmony patches
}