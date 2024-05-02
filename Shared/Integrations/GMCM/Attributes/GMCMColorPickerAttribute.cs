namespace DaLion.Shared.Integrations.GMCM.Attributes;

/// <summary>Sets the Color Picker options for a GMCM <see cref="Microsoft.Xna.Framework.Color"/> property.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GMCMColorPickerAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="GMCMColorPickerAttribute"/> class.</summary>
    /// <param name="showAlpha">Whether to display the alpha slider.</param>
    /// <param name="colorPickerStyle">The color picker style to display.</param>
    public GMCMColorPickerAttribute(bool showAlpha, uint colorPickerStyle)
    {
        this.ShowAlpha = showAlpha;
        this.ColorPickerStyle = colorPickerStyle;
    }

    /// <summary>Gets a value indicating whether to display the alpha slider.</summary>
    public bool ShowAlpha { get; }

    /// <summary>Gets a value indicating the color picker style to display.</summary>
    public uint ColorPickerStyle { get; }
}
