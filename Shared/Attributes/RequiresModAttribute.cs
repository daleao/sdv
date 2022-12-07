namespace DaLion.Shared.Attributes;

/// <summary>Indicates that an implicitly-used marked symbol should only be instantiated when a third-party mod is installed.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class RequiresModAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="RequiresModAttribute"/> class.</summary>
    /// <param name="uniqueId">The required mod's unique ID.</param>
    public RequiresModAttribute(string uniqueId)
    {
        this.UniqueId = uniqueId;
        this.Version = string.Empty;
    }

    /// <summary>Initializes a new instance of the <see cref="RequiresModAttribute"/> class.</summary>
    /// <param name="uniqueId">The required mod's unique ID.</param>
    /// <param name="version">The minimum required version.</param>
    public RequiresModAttribute(string uniqueId, string version)
    {
        this.UniqueId = uniqueId;
        this.Version = version;
    }

    /// <summary>Gets the required mod's unique ID.</summary>
    public string UniqueId { get; }

    /// <summary>Gets the minimum required version.</summary>
    public string Version { get; }
}
