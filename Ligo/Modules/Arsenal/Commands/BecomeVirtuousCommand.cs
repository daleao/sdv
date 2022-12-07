namespace DaLion.Ligo.Modules.Arsenal.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;

#endregion using directives

[UsedImplicitly]
internal sealed class BecomeVirtuousCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="BecomeVirtuousCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal BecomeVirtuousCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "chivalrous", "virtuous" };

    /// <inheritdoc />
    public override string Documentation => "Set all the player's virtue targets as complete.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var player = Game1.player;
        player.Write(DataFields.ProvenHonor, int.MaxValue.ToString());
        player.Write(DataFields.ProvenCompassion, int.MaxValue.ToString());
        player.Write(DataFields.ProvenWisdom, int.MaxValue.ToString());
        player.mailForTomorrow.Add("pamHouseUpgrade");
        player.mailForTomorrow.Add("Gil_Slime Charmer Ring");
        player.mailForTomorrow.Add("Gil_Savage Ring");
        player.mailForTomorrow.Add("Gil_Skeleton Mask");
        player.mailForTomorrow.Add("Gil_Insect Head");
        player.mailForTomorrow.Add("Gil_Vampire Ring");
        player.mailForTomorrow.Add("Gil_Hard Hat");
        player.mailForTomorrow.Add("Gil_Burglar's Ring");
        player.mailForTomorrow.Add("Gil_Crabshell Ring");
        player.mailForTomorrow.Add("Gil_Arcane Hat");
        player.mailForTomorrow.Add("Gil_Knight's Helmet");
        player.mailForTomorrow.Add("Gil_Napalm Ring");
        player.mailForTomorrow.Add("Gil_Telephone");
    }
}
