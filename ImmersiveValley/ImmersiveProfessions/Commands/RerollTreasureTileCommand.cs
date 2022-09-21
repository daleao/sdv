namespace DaLion.Stardew.Professions.Commands;

#region using directives

using DaLion.Common;
using DaLion.Common.Commands;
using DaLion.Stardew.Professions.Extensions;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class RerollTreasureTileCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="RerollTreasureTileCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal RerollTreasureTileCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "reset_the_hunt", "hunt_reset", "reroll_treasure" };

    /// <inheritdoc />
    public override string Documentation =>
        "Forcefully restart the current Treasure Hunt with a new target treasure tile.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (!ModEntry.State.ScavengerHunt.Value.IsActive && !ModEntry.State.ProspectorHunt.Value.IsActive)
        {
            Log.W("There is no Treasure Hunt currently active.");
            return;
        }

        if (ModEntry.State.ScavengerHunt.Value.IsActive)
        {
            var v = ModEntry.ModHelper.Reflection.GetMethod(ModEntry.State.ScavengerHunt, "ChooseTreasureTile")
                .Invoke<Vector2?>(Game1.currentLocation);
            if (v is null)
            {
                Log.W("Couldn't find a valid treasure tile after 10 tries.");
                return;
            }

            Game1.currentLocation.MakeTileDiggable(v.Value);
            ModEntry.ModHelper.Reflection.GetProperty<Vector2?>(ModEntry.State.ScavengerHunt, "TreasureTile")
                .SetValue(v);
            ModEntry.ModHelper.Reflection.GetField<uint>(ModEntry.State.ScavengerHunt, "elapsed").SetValue(0);

            Log.I("The Scavenger Hunt was reset.");
        }
        else if (ModEntry.State.ProspectorHunt.Value.IsActive)
        {
            var v = ModEntry.ModHelper.Reflection.GetMethod(ModEntry.State.ProspectorHunt, "ChooseTreasureTile")
                .Invoke<Vector2?>(Game1.currentLocation);
            if (v is null)
            {
                Log.W("Couldn't find a valid treasure tile after 10 tries.");
                return;
            }

            ModEntry.ModHelper.Reflection.GetProperty<Vector2?>(ModEntry.State.ProspectorHunt, "TreasureTile")
                .SetValue(v);
            ModEntry.ModHelper.Reflection.GetField<int>(ModEntry.State.ProspectorHunt, "Elapsed").SetValue(0);

            Log.I("The Prospector Hunt was reset.");
        }
    }
}
