namespace DaLion.Stardew.Tweex.Commands;

#region using directives

using System.Linq;
using StardewValley;
using StardewValley.TerrainFeatures;

using Common.Commands;
using Common.Data;

#endregion using directives

internal class ClearTreeDataCommand : ICommand
{
    /// <inheritdoc />
    public string Trigger => "deage_trees";

    /// <inheritdoc />
    public string Documentation => "Clear the age data of every tree in the world.";

    /// <inheritdoc />
    public void Callback(string[] args)
    {
        foreach (var tree in Game1.locations.SelectMany(l => l.terrainFeatures.Values).OfType<Tree>())
            ModDataIO.WriteData(tree, "Age", null);
    }
}