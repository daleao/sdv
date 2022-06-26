namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Commands;
using Extensions;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class RerollTreasureTileCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal RerollTreasureTileCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "hunt_reset";

    /// <inheritdoc />
    public override string Documentation => "Forcefully restart the current Treasure Hunt with a new target treasure tile.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (!ModEntry.PlayerState.ScavengerHunt.IsActive && !ModEntry.PlayerState.ProspectorHunt.IsActive)
        {
            Log.W("There is no Treasure Hunt currently active.");
            return;
        }

        if (ModEntry.PlayerState.ScavengerHunt.IsActive)
        {
            var v = ModEntry.ModHelper.Reflection.GetMethod(ModEntry.PlayerState.ScavengerHunt, "ChooseTreasureTile")
                .Invoke<Vector2?>(Game1.currentLocation);
            if (v is null)
            {
                Log.W("Couldn't find a valid treasure tile after 10 tries.");
                return;
            }

            Game1.currentLocation.MakeTileDiggable(v.Value);
            ModEntry.ModHelper.Reflection.GetProperty<Vector2?>(ModEntry.PlayerState.ScavengerHunt, "TreasureTile")
                .SetValue(v);
            ModEntry.ModHelper.Reflection.GetField<uint>(ModEntry.PlayerState.ScavengerHunt, "elapsed").SetValue(0);

            Log.I("The Scavenger Hunt was reset.");
        }
        else if (ModEntry.PlayerState.ProspectorHunt.IsActive)
        {
            var v = ModEntry.ModHelper.Reflection.GetMethod(ModEntry.PlayerState.ProspectorHunt, "ChooseTreasureTile")
                .Invoke<Vector2?>(Game1.currentLocation);
            if (v is null)
            {
                Log.W("Couldn't find a valid treasure tile after 10 tries.");
                return;
            }

            ModEntry.ModHelper.Reflection.GetProperty<Vector2?>(ModEntry.PlayerState.ProspectorHunt, "TreasureTile")
                .SetValue(v);
            ModEntry.ModHelper.Reflection.GetField<int>(ModEntry.PlayerState.ProspectorHunt, "Elapsed").SetValue(0);

            Log.I("The Prospector Hunt was reset.");
        }
    }
}