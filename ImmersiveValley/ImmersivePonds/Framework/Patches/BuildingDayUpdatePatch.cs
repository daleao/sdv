namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using DaLion.Common.Exceptions;
using HarmonyLib;
using StardewValley.Buildings;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class BuildingDayUpdatePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="BuildingDayUpdatePatch"/> class.</summary>
    internal BuildingDayUpdatePatch()
    {
        this.Target = this.RequireMethod<Building>(nameof(Building.dayUpdate));
    }

    #region harmony patches

#if DEBUG
    /// <summary>Stub for base <see cref="FishPond.dayUpdate"/>.</summary>
    /// <remarks>Required by DayUpdate prefix.</remarks>
    [HarmonyReversePatch]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:Element parameters should be documented", Justification = "Reverse patch.")]
    internal static void BuildingDayUpdateReverse(object instance, int dayOfMonth)
    {
        // its a stub so it has no initial content
        ThrowHelperExtensions.ThrowNotImplementedException("It's a stub.");
    }
#endif

    #endregion harmony patches
}
