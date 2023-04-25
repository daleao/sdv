namespace DaLion.Overhaul.Modules.Combat.Commands;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class InflictStatusCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="InflictStatusCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal InflictStatusCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "inflict_status", "status", "do" };

    /// <inheritdoc />
    public override string Documentation => "Inflicts the specified status condition on the nearest monster.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0)
        {
            Log.W("You must specify a valid status condition.");
            return;
        }

        if (args.Length == 1 || !int.TryParse(args[1], out var duration))
        {
            duration = 100000;
        }

        var nearest = Game1.player.GetClosestCharacter<Monster>();
        if (nearest is null)
        {
            Log.W("There are no enemies nearby.");
            return;
        }

        switch (args[0].ToLowerInvariant())
        {
            case "bleed":
                nearest.Bleed(Game1.player, duration);
                break;
            case "burn":
                nearest.Burn(Game1.player, duration);
                break;
            case "chill":
                nearest.Chill(duration);
                break;
            case "fear":
                nearest.Fear(duration);
                break;
            case "freeze":
                nearest.Freeze(duration);
                break;
            case "poison":
                nearest.Poison(Game1.player, duration);
                break;
            case "slow":
                nearest.Slow(duration);
                break;
            case "stun":
                nearest.Stun(duration);
                break;
        }
    }
}
