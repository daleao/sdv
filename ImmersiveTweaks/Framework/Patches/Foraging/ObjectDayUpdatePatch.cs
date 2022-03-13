namespace DaLion.Stardew.Tweaks.Framework.Patches.Foraging;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class ObjectDayUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectDayUpdatePatch()
    {
        Original = RequireMethod<SObject>(nameof(SObject.DayUpdate));
        Postfix.priority = Priority.LowerThanNormal;
    }

    #region harmony patches

    [HarmonyPostfix]
    private static void ObjectDayUpdatePostfix(SObject __instance)
    {
        if (__instance.IsBeeHouse() && ModEntry.Config.AgeBeeHouses) __instance.IncrementData<int>("Age");
    }

    #endregion harmony patches
}