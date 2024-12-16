namespace DaLion.Core.Framework.Extensions;

#region using directives

using StardewValley.Buffs;

#endregion using directives

/// <summary>Extensions for the <see cref="BuffManager"/> class.</summary>
public static class BuffManagerExtensions
{
    /// <summary>Gets the combined buff to the player's defense as <see cref="float"/>.</summary>
    /// <param name="manager">The <see cref="BuffManager"/>.</param>
    /// <returns>The combined buff to the player's defense as <see cref="float"/>.</returns>
    public static float FloatingDefense(this BuffManager manager)
    {
        return manager.GetValues().Defense.Value;
    }
}
