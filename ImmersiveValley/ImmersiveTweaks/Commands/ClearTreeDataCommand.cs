namespace DaLion.Stardew.Tweex.Commands;

#region using directives

using Common.Commands;
using Common.Data;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class ClearTreeDataCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ClearTreeDataCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "deage_trees";

    /// <inheritdoc />
    public override string Documentation => "Clear the age data of every tree in the world.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        foreach (var tree in Game1.locations.SelectMany(l => l.terrainFeatures.Values).OfType<Tree>())
            ModDataIO.WriteData(tree, "Age", null);
    }
}