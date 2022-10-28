namespace DaLion.Shared.Attributes;

/// <summary>Specifies that a class should only be available when a certain mod is installed.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class IntegrationAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="IntegrationAttribute"/> class.</summary>
    /// <param name="uniqueId">The mod unique ID.</param>
    public IntegrationAttribute(string uniqueId)
    {
        this.UniqueId = uniqueId;
        this.Version = string.Empty;
    }

    /// <summary>Initializes a new instance of the <see cref="IntegrationAttribute"/> class.</summary>
    /// <param name="uniqueId">The mod unique ID.</param>
    /// <param name="version">The minimum required version.</param>
    public IntegrationAttribute(string uniqueId, string version)
    {
        this.UniqueId = uniqueId;
        this.Version = version;
    }

    /// <summary>Gets the mod unique ID.</summary>
    public string UniqueId { get; }

    /// <summary>Gets the minimum required version.</summary>
    public string Version { get; }
}
