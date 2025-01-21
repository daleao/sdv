namespace DaLion.Core.Commands;

#region using directives

using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="InflictStatusCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
[Debug]
internal sealed class InflictStatusCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["inflict_status", "inflict", "status"];

    /// <inheritdoc />
    public override string Documentation =>
        "Inflicts the specified status condition on the nearest monster, or all monsters if the `--all` flag is used.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        var all = args.Any(a => a is "-a" or "--all");
        if (all)
        {
            args = args.Except(["-a", "--all"]).ToArray();
        }

        if (args.Length == 0)
        {
            Log.W("You must specify a valid status condition.");
            return true;
        }

        var player = Game1.player;
        var q = new Queue<Monster>();
        if (all)
        {
            foreach (var character in player.currentLocation.characters)
            {
                if (character is not Monster { IsMonster: true } monster)
                {
                    continue;
                }

                q.Enqueue(monster);
            }
        }
        else
        {
            var nearest = player.GetClosestCharacter<Monster>();
            if (nearest is null)
            {
                Log.W("There are no enemies nearby.");
                return true;
            }

            q.Enqueue(nearest);
        }

        while (q.TryDequeue(out var monster))
        {
            switch (args[0].ToLowerInvariant())
            {
                case "bleed":
                    monster.Bleed(player);
                    break;
                case "burn":
                    monster.Burn(player);
                    break;
                case "chill":
                    monster.Chill();
                    break;
                case "fear":
                    monster.Fear();
                    break;
                case "freeze":
                    monster.Freeze();
                    break;
                case "poison":
                    monster.Poison(player);
                    break;
                case "slow":
                    monster.Slow();
                    break;
                case "stun":
                    monster.Stun();
                    break;
            }
        }

        return true;
    }
}
