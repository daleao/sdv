namespace DaLion.Shared.Classes;

#region using directives

using Microsoft.Xna.Framework;

#endregion using directive

/// <summary>Represents a range of color.</summary>
public class ColorRange
{
    private byte[] _red;
    private byte[] _green;
    private byte[] _blue;

    /// <summary>Initializes a new instance of the <see cref="ColorRange"/> class.</summary>
    /// <param name="r">The red channel range.</param>
    /// <param name="g">The green channel range.</param>
    /// <param name="b">The blue channel range.</param>
    public ColorRange(byte[] r, byte[] g, byte[] b)
    {
        this._red = r;
        this._green = g;
        this._blue = b;
    }

    /// <summary>Gets or sets the red channel range.</summary>
    public byte[] Red
    {
        get => this._red;
        set
        {
            if (value.Length != 2)
            {
                ThrowHelper.ThrowInvalidOperationException("Range value must have length 2.");
                return;
            }

            if (value[1] <= value[0])
            {
                ThrowHelper.ThrowInvalidOperationException("Range max value must be higher than min value.");
                return;
            }

            this._red = value;
        }
    }

    /// <summary>Gets or sets the green channel range.</summary>
    public byte[] Green
    {
        get => this._green;
        set
        {
            if (value.Length != 2)
            {
                ThrowHelper.ThrowInvalidOperationException("Range value must have length 2.");
                return;
            }

            if (value[1] <= value[0])
            {
                ThrowHelper.ThrowInvalidOperationException("Range max value must be higher than min value.");
                return;
            }

            this._green = value;
        }
    }

    /// <summary>Gets or sets the blue channel range.</summary>
    public byte[] Blue
    {
        get => this._blue;
        set
        {
            if (value.Length != 2)
            {
                ThrowHelper.ThrowInvalidOperationException("Range value must have length 2.");
                return;
            }

            if (value[1] <= value[0])
            {
                ThrowHelper.ThrowInvalidOperationException("Range max value must be higher than min value.");
                return;
            }

            this._blue = value;
        }
    }

    /// <summary>Determines whether the specified color is contained within the <see cref="ColorRange"/>.</summary>
    /// <param name="c">A <see cref="Color"/>.</param>
    /// <returns><see langword="true"/> if all channels of <paramref name="c"/> are contained within the respective inclusive ranges in the <see cref="ColorRange"/>, otherwise <see langword="false"/>.</returns>
    public bool Contains(Color c)
    {
        return c.R >= this.Red[0] && c.R <= this.Red[1] && c.G >= this.Green[0] && c.G <= this.Green[1] && c.B >= this.Blue[0] && c.B <= this.Blue[1];
    }
}
