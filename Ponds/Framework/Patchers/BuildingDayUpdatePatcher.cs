namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class BuildingDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BuildingDayUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal BuildingDayUpdatePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Building>(nameof(Building.dayUpdate));
    }

    #region harmony patches

#if DEBUG
    /// <summary>Stub for base <see cref="Building.dayUpdate"/>.</summary>
    /// <remarks>Required by DayUpdate prefix.</remarks>
    [HarmonyReversePatch]
    [MethodImpl(MethodImplOptions.NoInlining)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:Element parameters should be documented", Justification = "Reverse patch.")]
    internal static void BuildingDayUpdateReverse(object instance, int dayOfMonth)
    {
        // its a stub so it has no initial content
        ThrowHelperExtensions.ThrowNotImplementedException("It's a stub.");
    }
#endif

    #endregion harmony patches
}
