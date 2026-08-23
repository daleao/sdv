namespace DaLion.Enchantments;

#region using directives

using StardewValley.Monsters;

#endregion using directives

/// <summary>The ephemeral runtime state for Tools.</summary>
internal sealed class EnchantmentsState
{
    internal Monster? HoveredEnemy { get; set; }

    internal Dictionary<(string Level, int X, int Y), int> MineShaftWallHits { get; } = [];
}
