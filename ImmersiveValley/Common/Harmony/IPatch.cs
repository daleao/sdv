namespace DaLion.Common.Harmony;

#region using directives

using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

#endregion using directives

/// <summary>Interface for Harmony patches.</summary>
internal interface IPatch
{
    /// <summary>The method to be patched.</summary>
    MethodBase Target { get; }

    /// <summary>The <see cref="HarmonyPrefix"/> patch that should be applied, if any.</summary>
    [CanBeNull] HarmonyMethod Prefix { get; }

    /// <summary>The <see cref="HarmonyPostfix"/> patch that should be applied, if any.</summary>
    [CanBeNull] HarmonyMethod Postfix { get; }

    /// <summary>The <see cref="HarmonyTranspiler"/> patch that should be applied, if any.</summary>
    [CanBeNull] HarmonyMethod Transpiler { get; }

    /// <summary>The <see cref="HarmonyReversePatch"/> patch that should be applied, if any.</summary>
    [CanBeNull] HarmonyMethod Reverse { get; }

    /// <summary>Apply internally-defined Harmony patches.</summary>
    /// <param name="harmony">The Harmony instance for this mod.</param>
    void Apply(Harmony harmony);
}