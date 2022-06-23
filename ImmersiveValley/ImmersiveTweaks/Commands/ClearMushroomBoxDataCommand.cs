namespace DaLion.Stardew.Tweex.Commands;

#region using directives

using System.Linq;
using StardewValley;
using StardewValley.Locations;

using Common.Commands;
using Common.Data;
using Extensions;

#endregion using directives

internal class ClearMushroomBoxDataCommand : ICommand
{
    /// <inheritdoc />
    public string Trigger => "deage_shroom_boxes";

    /// <inheritdoc />
    public string Documentation => "Clear the age data of every mushroom box in the farm cave.";

    /// <inheritdoc />
    public void Callback(string[] args)
    {
        foreach (var @object in Game1.locations.OfType<FarmCave>().SelectMany(fc => fc.Objects.Values)
                     .Where(o => o.IsMushroomBox())) ModDataIO.WriteData(@object, "Age", null);
    }
}