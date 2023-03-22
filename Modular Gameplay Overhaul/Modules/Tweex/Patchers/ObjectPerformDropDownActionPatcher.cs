namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPerformDropDownActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPerformDropDownActionPatcher"/> class.</summary>
    internal ObjectPerformDropDownActionPatcher()
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
            __instance.Write(DataKeys.Age, null);
        }
    }

    #endregion harmony patches
}
