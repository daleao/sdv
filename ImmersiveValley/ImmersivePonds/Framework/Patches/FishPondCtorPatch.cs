namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;

using Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondCtorPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondCtorPatch()
    {
        Target = RequireConstructor<FishPond>(typeof(BluePrint), typeof(Vector2));
    }

    #region harmony patches

    /// <summary>Compensates for the game calling dayUpdate *twice* immediately upon construction.</summary>
    [HarmonyPostfix]
    private static void FishPondCtorPostfix(FishPond __instance)
    {
        __instance.WriteData("DaysEmpty", (-3).ToString()); // it's -3 for good measure (and also immersion; a fresh pond takes longer to get dirty)
    }

    #endregion harmony patches
}