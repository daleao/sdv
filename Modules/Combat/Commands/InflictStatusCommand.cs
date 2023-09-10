namespace DaLion.Overhaul.Modules.Combat.Commands;

#region using directives

using System.Linq;
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
    public override string[] Triggers { get; } = { "inflict_status", "inflict", "status" };

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

        var all = args.Any(a => a is "-a" or "--all");
        if (all)
        {
            args = args.Except(new[] { "-a", "--all" }).ToArray();
        }

        if (args.Length == 0)
        {
            Log.W("You must specify a valid status condition.");
            return;
        }

        if (args.Length == 1 || !int.TryParse(args[1], out var duration))
        {
            duration = 100000;
        }

        var player = Game1.player;
        if (all)
        {
            for (var i = 0; i < player.currentLocation.characters.Count; i++)
            {
                var character = player.currentLocation.characters[i];
                if (character is not Monster monster)
                {
                    continue;
                }

                switch (args[0].ToLowerInvariant())
                {
                    case "bleed":
                        monster.Bleed(player, duration);
                        break;
                    case "burn":
                        monster.Burn(player, duration);
                        break;
                    case "chill":
                        monster.Chill(duration);
                        break;
                    case "fear":
                        monster.Fear(duration);
                        break;
                    case "freeze":
                        monster.Freeze(duration);
                        break;
                    case "poison":
                        monster.Poison(player, duration);
                        break;
                    case "slow":
                        monster.Slow(duration);
                        break;
                    case "stun":
                        monster.Stun(duration);
                        break;
                }
            }
        }

        var nearest = player.GetClosestCharacter<Monster>(out _);
        if (nearest is null)
        {
            Log.W("There are no enemies nearby.");
            return;
        }

        switch (args[0].ToLowerInvariant())
        {
            case "bleed":
                nearest.Bleed(player, duration);
                break;
            case "burn":
                nearest.Burn(player, duration);
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
                nearest.Poison(player, duration);
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
