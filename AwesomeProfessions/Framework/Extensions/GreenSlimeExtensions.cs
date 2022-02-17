namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using StardewValley.Monsters;

#endregion using directives

internal static class GreenSlimeExtensions
{
    /// <summary>Whether the Slime instance is currently jumping.</summary>
    public static bool IsJumping(this GreenSlime slime)
    {
        return !string.IsNullOrEmpty(slime.ReadData("Jumping"));
    }
}