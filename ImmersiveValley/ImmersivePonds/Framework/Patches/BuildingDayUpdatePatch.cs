namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;

using Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class BuildingDayUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal BuildingDayUpdatePatch()
    {
        Target = RequireMethod<Building>(nameof(Building.dayUpdate));
    }

    #region harmony patches

#if DEBUG
    /// <summary>Stub for base FishPond.dayUpdate</summary>
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