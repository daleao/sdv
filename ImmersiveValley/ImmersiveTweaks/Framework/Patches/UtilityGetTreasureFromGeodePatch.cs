namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using DaLion.Stardew.Tweex.Extensions;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class UtilityGetTreasureFromGeodePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="UtilityGetTreasureFromGeodePatch"/> class.</summary>
    internal UtilityGetTreasureFromGeodePatch()
    {
        this.Target = this.RequireMethod<Utility>(nameof(Utility.getTreasureFromGeode));
    }

    #region harmony patches

    /// <summary>Precious rocks preserve geode quality.</summary>
    [HarmonyPostfix]
    private static void UtilityGetTreasureFromGeodePostfix(Item __result, Item geode)
    {
        if (ModEntry.ProfessionsApi is not null && __result is SObject resultObj && resultObj.IsPreciousRock() &&
            geode is SObject { Quality: > 0 } geodeObj)
        {
            resultObj.Quality = geodeObj.Quality;
        }
    }

    #endregion harmony patches
}
