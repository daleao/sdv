using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches.Mining;

[UsedImplicitly]
internal class ObjectGetMinutesForCrystalariumPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectGetMinutesForCrystalariumPatch()
    {
        Original = RequireMethod<SObject>("getMinutesForCrystalarium");
    }

    #region harmony patches

    /// <summary>Patch to speed up crystalarium processing time for each Gemologist.</summary>
    [HarmonyPostfix]
    private static void ObjectGetMinutesForCrystalariumPostfix(SObject __instance, ref int __result)
    {
        var owner = Game1.getFarmerMaybeOffline(__instance.owner.Value) ?? Game1.MasterPlayer;
        if (owner.HasProfession("Gemologist"))
            __result = (int) (__result * (owner.HasPrestigedProfession("Gemologist") ? 0.5 : 0.75));
    }

    #endregion harmony patches
}