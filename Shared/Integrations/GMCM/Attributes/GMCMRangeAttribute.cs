namespace DaLion.Shared.Integrations.GMCM.Attributes;

/// <summary>Sets the minimum and maximum parameters of a GMCM numeric property.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GMCMRangeAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="GMCMRangeAttribute"/> class.</summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public GMCMRangeAttribute(float min, float max)
    {
        this.Min = min;
        this.Max = max;
    }

    /// <summary>Gets the minimum value.</summary>
    public float Min { get; }

    /// <summary>Gets the maximum value.</summary>
    public float Max { get; }
}
