namespace DaLion.Shared.Integrations.GMCM.Attributes;

/// <summary>Sets the minimum and maximum parameters of a GMCM numeric property.</summary>
[AttributeUsage(AttributeTargets.Property)]
internal sealed class GMCMRangeAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="GMCMRangeAttribute"/> class.</summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    internal GMCMRangeAttribute(float min, float max)
    {
        this.Min = min;
        this.Max = max;
    }

    /// <summary>Gets the minimum value.</summary>
    internal float Min { get; }

    /// <summary>Gets the maximum value.</summary>
    internal float Max { get; }
}
