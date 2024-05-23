namespace DaLion.Shared.Integrations.GMCM.Attributes;

/// <summary>Sets the step parameter of a GMCM numeric property.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GMCMStepAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="GMCMStepAttribute"/> class.</summary>
    /// <param name="step">The step value.</param>
    public GMCMStepAttribute(float step)
    {
        this.Step = step;
    }

    /// <summary>Gets the interval value.</summary>
    public float Step { get; }
}
