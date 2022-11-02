namespace DaLion.Redux.Framework.Arsenal.Weapons.Commands;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class OverHealCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="OverHealCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal OverHealCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "overheal" };

    /// <inheritdoc />
    public override string Documentation => "Heals the player, allowing health to go beyond the maximum value.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var amount = 10;
        if (args.Length > 0 && int.TryParse(args[0], out amount))
        {
        }

        Game1.player.health += amount;
    }
}
