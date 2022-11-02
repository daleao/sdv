namespace DaLion.Redux.Framework.Ponds.Patches;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondCtorPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FishPondCtorPatch"/> class.</summary>
    internal FishPondCtorPatch()
    {
        this.Target = this.RequireConstructor<FishPond>(typeof(BluePrint), typeof(Vector2));
    }

    #region harmony patches

    /// <summary>Compensates for the game calling dayUpdate *twice* immediately upon construction.</summary>
    [HarmonyPostfix]
    private static void FishPondCtorPostfix(FishPond __instance)
    {
        __instance.Write(DataFields.DaysEmpty, (-3).ToString()); // it's -3 for good measure (and also immersion; a fresh pond takes longer to get dirty)
    }

    #endregion harmony patches
}
