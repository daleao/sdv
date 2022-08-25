namespace DaLion.Common.Attributes;

#region using directives

using System;

#endregion using directives

/// <summary>Specifies that a class should only be available if a certain mod is installed.</summary>
[AttributeUsage(AttributeTargets.Class)]
public class RequiresModAttribute : Attribute
{
    /// <summary>The mod unique ID.</summary>
    public string UniqueID { get; }

    /// <summary>The minimum required version.</summary>
    public string Version { get; }

    /// <summary>Construct an instance.</summary>
    /// <param name="uniqueID">The mod unique ID.</param>
    public RequiresModAttribute(string uniqueID)
    {
        UniqueID = uniqueID;
        Version = string.Empty;
    }

    /// <summary>Construct an instance.</summary>
    /// <param name="uniqueID">The mod unique ID.</param>
    /// <param name="version">The minimum required version.</param>
    public RequiresModAttribute(string uniqueID, string version)
    {
        UniqueID = uniqueID;
        Version = version;
    }
}