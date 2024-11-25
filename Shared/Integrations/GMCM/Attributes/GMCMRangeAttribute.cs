namespace DaLion.Shared.Integrations.GMCM.Attributes;

/// <summary>Sets the minimum and maximum parameters of a GMCM numeric property.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GMCMRangeAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="GMCMRangeAttribute"/> class.</summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="step">The interval step.</param>
    public GMCMRangeAttribute(float min, float max, float step = 0.1f)
    {
        this.Min = min;
        this.Max = max;
        this.Step = step;
    }

    /// <summary>Initializes a new instance of the <see cref="GMCMRangeAttribute"/> class.</summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="step">The interval step.</param>
    public GMCMRangeAttribute(int min, int max, int step = 1)
    {
        this.Min = min;
        this.Max = max;
        this.Step = step;
    }

    /// <summary>Gets the minimum value.</summary>
    public float Min { get; }

    /// <summary>Gets the maximum value.</summary>
    public float Max { get; }

    /// <summary>Gets the interval value.</summary>
    public float Step { get; }
}
