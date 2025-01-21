namespace DaLion.Shared.Extensions.SMAPI;

#region using directives

using Microsoft.Xna.Framework.Input;

#endregion using directives

/// <summary>Extensions for the <see cref="SButton"/> enum.</summary>
public static class SButtonExtensions
{
    /// <summary>Get the <see cref="Buttons" /> equivalent for the given <see cref="SButton"/>.</summary>
    /// <param name="button">The controller button to convert.</param>
    /// <returns>The <see cref="Buttons"/> equivalent.</returns>
    public static Buttons ToButton(this SButton button)
    {
        return (Buttons)(button - 2000);
    }
}
