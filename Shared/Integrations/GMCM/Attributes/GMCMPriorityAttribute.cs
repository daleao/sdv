namespace DaLion.Shared.Integrations.GMCM.Attributes;

/// <summary>Assigns a priority to GMCM property.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GMCMPriorityAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="GMCMPriorityAttribute"/> class.</summary>
    /// <param name="priority">The priority of the property in the page.</param>
    public GMCMPriorityAttribute(uint priority)
    {
        this.Priority = priority;
    }

    /// <summary>Gets the priority of the property in the page.</summary>
    public uint Priority { get; }
}
