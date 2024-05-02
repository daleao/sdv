namespace DaLion.Shared.Integrations.GMCM.Attributes;

/// <summary>Assigns a default value to a GMCM <see cref="Microsoft.Xna.Framework.Color"/> property.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GMCMDefaultColorAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="GMCMDefaultColorAttribute"/> class.</summary>
    /// <param name="r">The default red channel value.</param>
    /// <param name="g">The default green channel value.</param>
    /// <param name="b">The default blue channel value.</param>
    public GMCMDefaultColorAttribute(byte r, byte g, byte b)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = 255;
    }

    /// <summary>Initializes a new instance of the <see cref="GMCMDefaultColorAttribute"/> class.</summary>
    /// <param name="r">The default red channel value.</param>
    /// <param name="g">The default green channel value.</param>
    /// <param name="b">The default blue channel value.</param>
    /// <param name="a">The default alpha channel value.</param>
    public GMCMDefaultColorAttribute(byte r, byte g, byte b, byte a)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = a;
    }

    /// <summary>Gets the default red channel value.</summary>
    public byte R { get; }

    /// <summary>Gets the default green channel value.</summary>
    public byte G { get; }

    /// <summary>Gets the default blue channel value.</summary>
    public byte B { get; }

    /// <summary>Gets the default alpha channel value.</summary>
    public byte A { get; }
}
