namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common.Extensions.Stardew;
using Extensions;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPerformDropDownActionPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectPerformDropDownActionPatch()
    {
        Target = RequireMethod<SObject>(nameof(SObject.performDropDownAction));
    }

    #region harmony patches

    /// <summary>Clear the age of bee houses and mushroom boxes.</summary>
    [HarmonyPostfix]
    private static void ObjectPerformDropDownActionPostfix(SObject __instance)
    {
        if (__instance.IsBeeHouse() || __instance.IsMushroomBox())
            __instance.Write("Age", null);
    }

    #endregion harmony patches
}