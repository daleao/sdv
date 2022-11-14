namespace DaLion.Shared.Attributes;

/// <summary>Specifies that an implicitly-used class should only be instantiated when a third-party mod is installed.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class IntegrationAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="IntegrationAttribute"/> class.</summary>
    /// <param name="uniqueId">The required mod's unique ID.</param>
    public IntegrationAttribute(string uniqueId)
    {
        this.UniqueId = uniqueId;
        this.Version = string.Empty;
    }

    /// <summary>Initializes a new instance of the <see cref="IntegrationAttribute"/> class.</summary>
    /// <param name="uniqueId">The required mod's unique ID.</param>
    /// <param name="version">The minimum required version.</param>
    public IntegrationAttribute(string uniqueId, string version)
    {
        this.UniqueId = uniqueId;
        this.Version = version;
    }

    /// <summary>Gets the required mod's unique ID.</summary>
    public string UniqueId { get; }

    /// <summary>Gets the minimum required version.</summary>
    public string Version { get; }
}
