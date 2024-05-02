namespace DaLion.Shared.Attributes;

/// <summary>Indicates to a factory that the implicitly-used marked symbol should not be instantiated when a third-party mod is installed.</summary>
[AttributeUsage(AttributeTargets.Class)]
internal sealed class ModConflictAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="ModConflictAttribute"/> class.</summary>
    /// <param name="uniqueId">The required mod's unique ID.</param>
    /// <param name="name">A human-readable name for the mod.</param>
    internal ModConflictAttribute(string uniqueId, string? name = null)
    {
        this.UniqueId = uniqueId;
        this.Name = name ?? uniqueId;
    }

    /// <summary>Gets the required mod's unique ID.</summary>
    internal string UniqueId { get; }

    /// <summary>Gets the human-readable name of the mod.</summary>
    internal string Name { get; }
}
