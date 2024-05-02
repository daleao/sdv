namespace DaLion.Shared.Integrations.GMCM.Attributes;

/// <summary>Assigns a default value to a GMCM <see cref="Microsoft.Xna.Framework.Vector2"/> property.</summary>
[AttributeUsage(AttributeTargets.Property)]
internal sealed class GMCMDefaultVector2Attribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="GMCMDefaultVector2Attribute"/> class.</summary>
    /// <param name="x">The default X coordinate.</param>
    /// <param name="y">The default Y coordinate.</param>
    internal GMCMDefaultVector2Attribute(float x, float y)
    {
        this.X = x;
        this.Y = y;
    }

    /// <summary>Gets the default X coordinate.</summary>
    internal float X { get; }

    /// <summary>Gets the default Y coordinate.</summary>
    internal float Y { get; }
}
