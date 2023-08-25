namespace DaLion.Overhaul.Modules.Rings.Patchers.Integration;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Overhaul.Modules.Rings.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
[ModRequirement("atravita.IdentifiableCombinedRings")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch specifies the mod in file name but not class to avoid breaking pattern.")]
internal sealed class RingPatcherPostfixGetDisplayNamePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingPatcherPostfixGetDisplayNamePatcher"/> class.</summary>
    internal RingPatcherPostfixGetDisplayNamePatcher()
    {
        this.Target = "IdentifiableCombinedRings.HarmonyPatches.RingPatcher"
            .ToType()
            .RequireMethod("PostfixGetDisplayName");
    }

    #region harmony patches

    /// <summary>Remove (Many) tag from Infinity Band.</summary>
    [HarmonyPrefix]
    private static bool RingPatcherPostfixGetDisplayNamePrefix(Ring __0)
    {
        if (__0.IsCombinedInfinityBand(out _))
        {
            return false; // don't run original logic
        }

        return true; // run original logic
    }

    #endregion harmony patches
}
