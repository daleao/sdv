namespace DaLion.Common.Exceptions;

#region using directives

using Extensions.Reflection;
using System;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

/// <summary>Thrown when a <see cref="Label"/> is not found within the expected instructions list.</summary>
public class LabelNotFoundException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="LabelNotFoundException"/> class.</summary>
    /// <param name="label">The <see cref="Label" /> which could not be found.</param>
    /// <param name="target">The target method where the label was searched for.</param>
    public LabelNotFoundException(Label label, MethodBase target)
        : base($"Couldn't find label {label} in target method {target.GetFullName()}.")
    {
    }

    /// <summary>Initializes a new instance of the <see cref="LabelNotFoundException"/> class.</summary>
    /// <param name="label">The <see cref="Label" /> which could not be found.</param>
    /// <param name="target">The target method where the label was searched for.</param>
    /// <param name="snitch">A callback to snitch on applied changes to the target method.</param>
    public LabelNotFoundException(Label label, MethodBase target, Func<string> snitch)
        : base($"Couldn't find label {label} in target method {target.GetFullName()}." + snitch())
    {
    }
}