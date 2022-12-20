namespace DaLion.Shared.Attributes;

/// <summary>Indicates to a factory that an implicitly-used marked symbol should only be instantiated when a third-party mod is installed, or adds third-party mod metadata to an explicitly-instantiated class.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class RequiresModAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="RequiresModAttribute"/> class.</summary>
    /// <param name="uniqueId">The required mod's unique ID.</param>
    /// <param name="name">A human-readable name for the mod.</param>
    /// <param name="version">The minimum required version.</param>
    public RequiresModAttribute(string uniqueId, string? name = null, string? version = null)
    {
        this.UniqueId = uniqueId;
        this.Name = name ?? uniqueId;
        this.Version = version ?? string.Empty;
    }

    /// <summary>Gets the required mod's unique ID.</summary>
    public string UniqueId { get; }

    /// <summary>Gets the human-readable name of the mod.</summary>
    public string Name { get; }

    /// <summary>Gets the minimum required version.</summary>
    public string Version { get; }
}
