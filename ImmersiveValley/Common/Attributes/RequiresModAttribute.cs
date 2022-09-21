namespace DaLion.Common.Attributes;

#region using directives

using System;

#endregion using directives

/// <summary>Specifies that a class should only be available if a certain mod is installed.</summary>
[AttributeUsage(AttributeTargets.Class)]
public class RequiresModAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="RequiresModAttribute"/> class.</summary>
    /// <param name="uniqueId">The mod unique ID.</param>
    public RequiresModAttribute(string uniqueId)
    {
        this.UniqueId = uniqueId;
        this.Version = string.Empty;
    }

    /// <summary>Initializes a new instance of the <see cref="RequiresModAttribute"/> class.</summary>
    /// <param name="uniqueId">The mod unique ID.</param>
    /// <param name="version">The minimum required version.</param>
    public RequiresModAttribute(string uniqueId, string version)
    {
        this.UniqueId = uniqueId;
        this.Version = version;
    }

    /// <summary>Gets the mod unique ID.</summary>
    public string UniqueId { get; }

    /// <summary>Gets the minimum required version.</summary>
    public string Version { get; }
}
