namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondCreateFishInstancePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondCreateFishInstancePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishPondCreateFishInstancePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishPond>("CreateFishInstance");
    }

    #region harmony patches

    /// <summary>Spawn extended family when the pond is cleared.</summary>
    [HarmonyPrefix]
    private static bool FishPondCreateFishInstancePrefix(FishPond __instance, ref Item __result)
    {
        if (Data.ReadAs<int>(__instance, DataKeys.FamilyLivingHere) <= 0)
        {
            return true; // run original logic
        }

        __result = ItemRegistry.Create<SObject>(Lookups.FamilyPairs[$"(O){__instance.fishType.Value}"]);
        Data.Increment(__instance, DataKeys.FamilyLivingHere, -1);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
