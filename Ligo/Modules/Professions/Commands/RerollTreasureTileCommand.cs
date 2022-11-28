namespace DaLion.Ligo.Modules.Professions.Commands;

#region using directives

using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Commands;
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
        var player = Game1.player;
        if (!player.Get_ScavengerHunt().IsActive && !player.Get_ProspectorHunt().IsActive)
        {
            Log.W("There is no Treasure Hunt currently active.");
            return;
        }

        if (player.Get_ScavengerHunt().IsActive)
        {
            var v = ModEntry.ModHelper.Reflection.GetMethod(player.Get_ScavengerHunt(), "ChooseTreasureTile")
                .Invoke<Vector2?>(Game1.currentLocation);
            if (v is null)
            {
                Log.W("Couldn't find a valid treasure tile after 10 tries.");
                return;
            }

            Game1.currentLocation.MakeTileDiggable(v.Value);
            ModEntry.ModHelper.Reflection.GetProperty<Vector2?>(player.Get_ScavengerHunt(), "TreasureTile")
                .SetValue(v);
            ModEntry.ModHelper.Reflection.GetField<uint>(player.Get_ScavengerHunt(), "elapsed").SetValue(0);

            Log.I("The Scavenger Hunt was reset.");
        }
        else if (player.Get_ProspectorHunt().IsActive)
        {
            var v = ModEntry.ModHelper.Reflection.GetMethod(player.Get_ProspectorHunt(), "ChooseTreasureTile")
                .Invoke<Vector2?>(Game1.currentLocation);
            if (v is null)
            {
                Log.W("Couldn't find a valid treasure tile after 10 tries.");
                return;
            }

            ModEntry.ModHelper.Reflection.GetProperty<Vector2?>(player.Get_ProspectorHunt(), "TreasureTile")
                .SetValue(v);
            ModEntry.ModHelper.Reflection.GetField<int>(player.Get_ProspectorHunt(), "Elapsed").SetValue(0);

            Log.I("The Prospector Hunt was reset.");
        }
    }
}
