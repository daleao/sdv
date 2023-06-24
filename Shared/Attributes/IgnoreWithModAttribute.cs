namespace DaLion.Shared.Attributes;

/// <summary>Indicates to a factory that the implicitly-used marked symbol should not be instantiated when a third-party mod is installed.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class IgnoreWithModAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="IgnoreWithModAttribute"/> class.</summary>
    /// <param name="uniqueId">The required mod's unique ID.</param>
    /// <param name="name">A human-readable name for the mod.</param>
    /// <param name="version">The minimum required version.</param>
    public IgnoreWithModAttribute(string uniqueId, string? name = null, string? version = null)
    {
        this.UniqueId = uniqueId;
        this.Name = name ?? uniqueId;
    }

    /// <summary>Gets the required mod's unique ID.</summary>
    public string UniqueId { get; }

    /// <summary>Gets the human-readable name of the mod.</summary>
    public string Name { get; }
}
