namespace DaLion.Overhaul.Modules.Arsenal.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Collections;
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
    public override string[] Triggers { get; } = { "advance_quest", "advance", "adv" };

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
                if (player.hasQuest(Constants.VirtuesIntroQuestId))
                {
                    player.completeQuest(Constants.VirtuesIntroQuestId);
                    player.addQuest(Virtue.Honor);
                    player.addQuest(Virtue.Compassion);
                    player.addQuest(Virtue.Wisdom);
                    player.addQuest(Virtue.Generosity);
                    player.addQuest(Virtue.Valor);
                }

                player.Write(DataFields.ProvenHonor, int.MaxValue.ToString());
                player.Write(DataFields.ProvenCompassion, int.MaxValue.ToString());
                player.Write(DataFields.ProvenWisdom, int.MaxValue.ToString());
                player.Write(DataFields.ProvenGenerosity, true.ToString());
                player.Write(DataFields.ProvenValor, true.ToString());
                Virtue.List.ForEach(virtue => virtue.CheckForCompletion(Game1.player));
                player.completeQuest(Constants.VirtuesNextQuestId);
                break;
        }
    }
}
