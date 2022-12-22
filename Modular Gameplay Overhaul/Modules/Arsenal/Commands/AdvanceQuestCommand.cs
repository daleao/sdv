namespace DaLion.Overhaul.Modules.Arsenal.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;

#endregion using directives

[UsedImplicitly]
internal sealed class AdvanceQuestCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="AdvanceQuestCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal AdvanceQuestCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "advance_quest", "adv" };

    /// <inheritdoc />
    public override string Documentation => "Forcefully advances the specified quest-line (either Clint's Forge or Yoba's Virtues).";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var player = Game1.player;
        if (args.Length == 0)
        {
            Log.W("You must specify a quest-line to advance (either \"Forge\" or \"Ruin\".");
            return;
        }

        switch (args[0].ToLowerInvariant())
        {
            case "legacy":
            case "clint":
            case "forge":
                player.mailReceived.Add("clintForge");
                if (player.hasQuest(Constants.ForgeIntroQuestId))
                {
                    player.completeQuest(Constants.ForgeIntroQuestId);
                }

                break;
            case "ruin":
            case "dawn":
            case "curse":
            case "yoba":
            case "virtues":
            case "chivalry":
                player.Write(DataFields.ProvenHonor, int.MaxValue.ToString());
                player.Write(DataFields.ProvenCompassion, int.MaxValue.ToString());
                player.Write(DataFields.ProvenWisdom, int.MaxValue.ToString());
                player.mailReceived.Add("Gil_Slime Charmer Ring");
                player.mailReceived.Add("Gil_Savage Ring");
                player.mailReceived.Add("Gil_Skeleton Mask");
                player.mailReceived.Add("Gil_Insect Head");
                player.mailReceived.Add("Gil_Vampire Ring");
                player.mailReceived.Add("Gil_Hard Hat");
                player.mailReceived.Add("Gil_Burglar's Ring");
                player.mailReceived.Add("Gil_Crabshell Ring");
                player.mailReceived.Add("Gil_Arcane Hat");
                player.mailReceived.Add("Gil_Knight's Helmet");
                player.mailReceived.Add("Gil_Napalm Ring");
                player.mailReceived.Add("Gil_Telephone");
                player.mailReceived.Add("pamHouseUpgrade");
                if (player.hasQuest(Constants.VirtuesIntroQuestId))
                {
                    player.completeQuest(Constants.ForgeIntroQuestId);
                }

                break;
        }
    }
}
