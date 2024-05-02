namespace DaLion.Shared.Integrations.GMCM.Attributes;

/// <summary>Sets the interval parameter of a GMCM numeric property.</summary>
[AttributeUsage(AttributeTargets.Property)]
internal sealed class GMCMIntervalAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="GMCMIntervalAttribute"/> class.</summary>
    /// <param name="interval">The interval value.</param>
    internal GMCMIntervalAttribute(float interval)
    {
        this.Interval = interval;
    }

    /// <summary>Gets the interval value.</summary>
    internal float Interval { get; }
}
