namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Tweex.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectDayUpdatePatcher"/> class.</summary>
    internal ObjectDayUpdatePatcher()
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.DayUpdate));
        this.Postfix!.priority = Priority.LowerThanNormal;
    }

    #region harmony patches

    /// <summary>Age bee houses and mushroom boxes.</summary>
    [HarmonyPostfix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    private static void ObjectDayUpdatePostfix(SObject __instance)
    {
        if (__instance.IsBeeHouse())
        {
            __instance.Increment(DataFields.Age);
        }
        else if (__instance.IsMushroomBox())
        {
            __instance.Increment(DataFields.Age);
            if (__instance.heldObject.Value is null)
            {
                return;
            }

            __instance.heldObject.Value.Quality = ProfessionsModule.IsEnabled
                ? Math.Max(
                    Game1.player.GetEcologistForageQuality(),
                    __instance.GetQualityFromAge())
                : __instance.GetQualityFromAge();
        }
    }

    #endregion harmony patches
}
