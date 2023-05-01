namespace DaLion.Overhaul.Modules.Weapons.Commands;

#region using directives

using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class SpecialMonstersCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="SpecialMonstersCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SpecialMonstersCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "special" };

    /// <inheritdoc />
    public override string Documentation => "Adds special items to all monsters in the current location.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        var monsters = Game1.player.currentLocation.characters.OfType<Monster>().ToArray();
        if (monsters.Length == 0)
        {
            return;
        }

        foreach (var monster in monsters)
        {
            monster.hasSpecialItem.Value = true;
        }

        Log.I($"Added special items to {monsters.Length} monsters.");
    }
}
