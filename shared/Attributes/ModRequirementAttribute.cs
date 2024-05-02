namespace DaLion.Shared.Attributes;

/// <summary>Indicates to a factory that the implicitly-used marked symbol should only be instantiated when a third-party mod is installed, or adds third-party mod metadata to an explicitly-instantiated class.</summary>
[AttributeUsage(AttributeTargets.Class)]
internal sealed class ModRequirementAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="ModRequirementAttribute"/> class.</summary>
    /// <param name="uniqueId">The required mod's unique ID.</param>
    /// <param name="name">A human-readable name for the mod.</param>
    /// <param name="version">The minimum required version.</param>
    internal ModRequirementAttribute(string uniqueId, string? name = null, string? version = null)
    {
        this.UniqueId = uniqueId;
        this.Name = name ?? uniqueId;
        this.Version = version ?? string.Empty;
    }

    /// <summary>Gets the required mod's unique ID.</summary>
    internal string UniqueId { get; }

    /// <summary>Gets the human-readable name of the mod.</summary>
    internal string Name { get; }

    /// <summary>Gets the minimum required version.</summary>
    internal string Version { get; }
}
