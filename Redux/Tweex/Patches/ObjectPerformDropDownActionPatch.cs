namespace DaLion.Redux.Tweex.Patches;

#region using directives

using DaLion.Redux.Core.Extensions;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

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
