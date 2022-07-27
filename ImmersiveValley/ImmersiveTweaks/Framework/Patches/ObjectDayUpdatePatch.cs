namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common.ModData;
using Extensions;
using HarmonyLib;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectDayUpdatePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectDayUpdatePatch()
    {
        Target = RequireMethod<SObject>(nameof(SObject.DayUpdate));
        Postfix!.priority = Priority.LowerThanNormal;
    }

    #region harmony patches

    /// <summary>Age bee houses and mushroom boxes.</summary>
    [HarmonyPostfix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    private static void ObjectDayUpdatePostfix(SObject __instance)
    {
        if (__instance.IsBeeHouse() && ModEntry.Config.AgeImprovesBeeHouses)
        {
            ModDataIO.Increment<int>(__instance, "Age");
        }
        else if (__instance.IsMushroomBox() && ModEntry.Config.AgeImprovesMushroomBoxes)
        {
            ModDataIO.Increment<int>(__instance, "Age");
            if (__instance.heldObject.Value is null) return;

            __instance.heldObject.Value.Quality = ModEntry.ProfessionsAPI is null
                ? Game1.player.professions.Contains(Farmer.botanist)
                    ? SObject.bestQuality
                    : __instance.GetQualityFromAge()
                : Math.Max(ModEntry.ProfessionsAPI.GetEcologistForageQuality(Game1.player),
                    __instance.GetQualityFromAge());
        }
    }

    #endregion harmony patches
}