namespace DaLion.Common.Exceptions;

#region using directives

using System;

#endregion using directives

/// <summary>Thrown when a given type is not found in any executing assembly at runtime.</summary>
public class MissingTypeException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="MissingTypeException"/> class.</summary>
    /// <param name="name">The name of the expected type.</param>
    public MissingTypeException(string name)
        : base($"A type named {name} could not be found in the executing assemblies.")
    {
    }
}
