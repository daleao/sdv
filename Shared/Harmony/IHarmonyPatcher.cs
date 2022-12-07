namespace DaLion.Shared.Harmony;

#region using directives

using System.Reflection;
using HarmonyLib;

#endregion using directives

/// <summary>Interface for a <see cref="Harmony"/> patch class targeting a single method.</summary>
internal interface IHarmonyPatcher
{
    /// <summary>Gets the method to be patched.</summary>
    MethodBase Target { get; }

    /// <summary>Gets the <see cref="HarmonyPrefix"/> patch that should be applied, if any.</summary>
    HarmonyMethod? Prefix { get; }

    /// <summary>Gets the <see cref="HarmonyPostfix"/> patch that should be applied, if any.</summary>
    HarmonyMethod? Postfix { get; }

    /// <summary>Gets the <see cref="HarmonyTranspiler"/> patch that should be applied, if any.</summary>
    HarmonyMethod? Transpiler { get; }

    /// <summary>Gets the <see cref="HarmonyFinalizer"/> patch that should be applied, if any.</summary>
    HarmonyMethod? Finalizer { get; }

    /// <summary>Gets the <see cref="HarmonyReversePatch"/> patch that should be applied, if any.</summary>
    HarmonyMethod? Reverse { get; }

    /// <summary>Applies internally-defined Harmony patches.</summary>
    /// <param name="harmony">The <see cref="Harmony"/> instance for this mod.</param>
    /// <returns><see langword="true"/> if the patch was applied successfully, otherwise <see langword="false"/>.</returns>
    bool Apply(Harmony harmony);
}
