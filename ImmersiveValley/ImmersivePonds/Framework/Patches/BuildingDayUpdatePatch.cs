namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Buildings;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class BuildingDayUpdatePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal BuildingDayUpdatePatch()
    {
        Target = RequireMethod<Building>(nameof(Building.dayUpdate));
    }

    #region harmony patches

#if DEBUG
    /// <summary>Stub for base <see cref="FishPond.dayUpdate">.</summary>
    /// <remarks>Required by DayUpdate prefix.</remarks>
    [HarmonyReversePatch]
    internal static void BuildingDayUpdateReverse(object instance, int dayOfMonth)
    {
        // its a stub so it has no initial content
        throw new NotImplementedException("It's a stub.");
    }
#endif

    #endregion harmony patches
}