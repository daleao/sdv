namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using StardewValley.Locations;

#endregion using directives

/// <summary>Extensions for the <see cref="VolcanoDungeon"/> class.</summary>
internal static class VolcanoDungeonExtensions
{
    /// <summary>Determines whether the current mine level is a safe level; i.e. shouldn't spawn any monsters.</summary>
    /// <param name="volcano">The <see cref="VolcanoDungeon"/> instance.</param>
    /// <returns><see langword="true"/> if the <paramref name="volcano"/>'s level is 5 (Dwarf shop) or 10 (Forge summit), otherwise <see langword="false"/>.</returns>
    internal static bool IsTreasureOrSafeRoom(this VolcanoDungeon volcano)
    {
        return volcano.level.Value % 5 == 0;
    }
}
