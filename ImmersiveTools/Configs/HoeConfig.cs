namespace DaLion.Stardew.Tools.Configs;

/// <summary>Configs related to the Hoe.</summary>
public class HoeConfig
{
    /// <summary>Whether to apply custom tile area for the Hoe. Keep this at false if using defaults to improve performance.</summary>
    public bool OverrideAffectedTiles { get; set; } = false;

    /// <summary>The area of affected tiles at each power level for the Hoe, in units lengths x units radius.</summary>
    /// <remarks>Note that radius extends to both sides of the farmer.</remarks>
    public int[][] AffectedTiles = {
        new[] {3, 0},
        new[] {5, 0},
        new[] {3, 1},
        new[] {6, 1},
        new[] {5, 2}
    };
}