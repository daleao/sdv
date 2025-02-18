﻿namespace DaLion.Shared.Attributes;

/// <summary>Indicates to a factory that the implicitly-used marked symbol should only be instantiated when a third-party mod is installed, or adds third-party mod metadata to an explicitly-instantiated class.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ModRequirementAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="ModRequirementAttribute"/> class.</summary>
    /// <param name="uniqueId">The required mod's unique ID.</param>
    /// <param name="name">A human-readable name for the mod.</param>
    /// <param name="minimumVersion">The minimum required version.</param>
    public ModRequirementAttribute(string uniqueId, string? name = null, string? minimumVersion = null)
    {
        this.UniqueId = uniqueId;
        this.Name = name ?? uniqueId;
        this.Version = minimumVersion ?? string.Empty;
    }

    /// <summary>Gets the required mod's unique ID.</summary>
    public string UniqueId { get; }

    /// <summary>Gets the human-readable name of the mod.</summary>
    public string Name { get; }

    /// <summary>Gets the minimum required version.</summary>
    public string Version { get; }
}
