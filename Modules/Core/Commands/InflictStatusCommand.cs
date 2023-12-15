namespace DaLion.Overhaul.Modules.Core.Commands;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Core.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Monsters;
using Buff = Shared.Enums.Buff;

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
    public override string Documentation => "Inflicts the specified status condition on the nearest monster, or all monsters if the `--all` flag is used.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0)
        {
            Log.W("You must specify a valid status condition.");
            return;
        }

        var self = args.Any(a => a is "-s" or "--self");
        if (self)
        {
            args = args.Except(new[] { "-s", "--self" }).ToArray();
            if (args.Length == 1 || !int.TryParse(args[1], out var duration1))
            {
                duration1 = 100000;
            }

            if (int.TryParse(args[0], out var id) && BuffExtensions.IsDefined((Buff)id))
            {
                Game1.buffsDisplay.addOtherBuff(new StardewValley.Buff(id) { millisecondsDuration = duration1 });
            }
            else
            {
                switch (args[0].ToLowerInvariant())
                {
                    case "burn":
                    case "burnt":
                        Game1.buffsDisplay.addOtherBuff(new StardewValley.Buff((int)Buff.Burnt) { millisecondsDuration = duration1 });
                        break;
                    case "freeze":
                    case "frozen":
                        Game1.buffsDisplay.addOtherBuff(new StardewValley.Buff((int)Buff.Frozen) { millisecondsDuration = duration1 });
                        break;
                    case "jinx":
                    case "jinxed":
                        Game1.buffsDisplay.addOtherBuff(new StardewValley.Buff((int)Buff.Jinxed) { millisecondsDuration = duration1 });
                        break;
                    case "confusion":
                    case "confused":
                    case "dillusion":
                    case "weakness":
                        Game1.buffsDisplay.addOtherBuff(new StardewValley.Buff((int)Buff.Weakness) { millisecondsDuration = duration1 });
                        break;
                }
            }

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

        if (args.Length == 1 || !int.TryParse(args[1], out var duration2))
        {
            duration2 = 100000;
        }

        var player = Game1.player;
        if (all)
        {
            for (var i = 0; i < player.currentLocation.characters.Count; i++)
            {
                var character = player.currentLocation.characters[i];
                if (character is not Monster { IsMonster: true } monster)
                {
                    continue;
                }

                switch (args[0].ToLowerInvariant())
                {
                    case "bleed":
                        monster.Bleed(player, duration2);
                        break;
                    case "burn":
                        monster.Burn(player, duration2);
                        break;
                    case "chill":
                        monster.Chill(duration2);
                        break;
                    case "fear":
                        monster.Fear(duration2);
                        break;
                    case "freeze":
                        monster.Freeze(duration2);
                        break;
                    case "poison":
                        monster.Poison(player, duration2);
                        break;
                    case "slow":
                        monster.Slow(duration2);
                        break;
                    case "stun":
                        monster.Stun(duration2);
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
                nearest.Bleed(player, duration2);
                break;
            case "burn":
                nearest.Burn(player, duration2);
                break;
            case "chill":
                nearest.Chill(duration2);
                break;
            case "fear":
                nearest.Fear(duration2);
                break;
            case "freeze":
                nearest.Freeze(duration2);
                break;
            case "poison":
                nearest.Poison(player, duration2);
                break;
            case "slow":
                nearest.Slow(duration2);
                break;
            case "stun":
                nearest.Stun(duration2);
                break;
        }
    }
}
