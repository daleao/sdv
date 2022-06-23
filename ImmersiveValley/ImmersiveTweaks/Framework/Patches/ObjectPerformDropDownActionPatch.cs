namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;

using Common.Data;
using Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPerformDropDownActionPatch : BasePatch
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
            ModDataIO.WriteData(__instance, "Age", null);
    }

    #endregion harmony patches
}