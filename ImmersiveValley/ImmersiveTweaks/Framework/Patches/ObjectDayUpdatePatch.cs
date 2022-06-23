namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using System;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Common.Data;
using Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectDayUpdatePatch : BasePatch
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
        if (__instance.IsBeeHouse() && ModEntry.Config.AgeBeeHouses)
        {
            ModDataIO.IncrementData<int>(__instance, "Age");
        }
        else if (__instance.IsMushroomBox())
        {
            if (ModEntry.Config.AgeMushroomBoxes) ModDataIO.IncrementData<int>(__instance, "Age");

            if (__instance.heldObject.Value is null) return;

            __instance.heldObject.Value.Quality = ModEntry.ProfessionsAPI is null
                ? __instance.GetQualityFromAge()
                : Math.Max(ModEntry.ProfessionsAPI.GetEcologistForageQuality(Game1.player),
                    __instance.heldObject.Value.Quality);
        }
    }

    #endregion harmony patches
}