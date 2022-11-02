namespace DaLion.Redux.Framework.Tweex.Patches;

#region using directives

using DaLion.Redux.Framework.Professions.Extensions;
using DaLion.Redux.Framework.Tweex.Extensions;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectDayUpdatePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ObjectDayUpdatePatch"/> class.</summary>
    internal ObjectDayUpdatePatch()
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

            __instance.heldObject.Value.Quality = ModEntry.Config.EnableProfessions
                ? Math.Max(
                    Game1.player.GetEcologistForageQuality(),
                    __instance.GetQualityFromAge())
                : Game1.player.professions.Contains(Farmer.botanist)
                    ? SObject.bestQuality
                    : __instance.GetQualityFromAge();
        }
    }

    #endregion harmony patches
}
