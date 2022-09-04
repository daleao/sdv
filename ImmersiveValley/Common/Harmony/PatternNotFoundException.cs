namespace DaLion.Common.Exceptions;

#region using directives

using Extensions.Reflection;
using HarmonyLib;
using System;
using System.Reflection;

#endregion using directives

/// <summary>Thrown when a <see cref="CodeInstruction"/> pattern is not found within the expected instructions list.</summary>
public class PatternNotFoundException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="PatternNotFoundException"/> class.</summary>
    /// <param name="pattern">A sequence of <see cref="CodeInstruction"/> that could not be found.</param>
    /// <param name="target">The target method where the pattern was searched for.</param>
    public PatternNotFoundException(CodeInstruction[] pattern, MethodBase target)
        : base($"Couldn't find instruction pattern in target method {target.GetFullName()}." +
               $"\nPattern:\n---- BEGIN ----\n{string.Join<CodeInstruction>("\n", pattern)}\n----- END -----\n")
    {
    }

    /// <summary>Initializes a new instance of the <see cref="PatternNotFoundException"/> class.</summary>
    /// <param name="pattern">A sequence of <see cref="CodeInstruction"/> that could not be found.</param>
    /// <param name="target">The target method where the pattern was searched for.</param>
    /// <param name="snitch">A callback to snitch on applied changes to the target method.</param>
    public PatternNotFoundException(CodeInstruction[] pattern, MethodBase target, Func<string> snitch)
        : base($"Couldn't find instruction pattern in target method {target.GetFullName()}." +
               $"\nPattern:\n---- BEGIN ----\n{string.Join<CodeInstruction>("\n", pattern)}\n----- END -----\n" + snitch())
    {
    }
}