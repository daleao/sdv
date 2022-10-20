namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Tweex.Extensions;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPerformDropDownActionPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPerformDropDownActionPatch"/> class.</summary>
    internal ObjectPerformDropDownActionPatch()
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.performDropDownAction));
    }

    #region harmony patches

    /// <summary>Clear the age of bee houses and mushroom boxes.</summary>
    [HarmonyPostfix]
    private static void ObjectPerformDropDownActionPostfix(SObject __instance)
    {
        if (__instance.IsBeeHouse() || __instance.IsMushroomBox())
        {
            __instance.Write(DataFields.Age, null);
        }
    }

    #endregion harmony patches
}
