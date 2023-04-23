namespace DaLion.Overhaul.Modules.Weapons.Commands;

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
    public override string Documentation => "Forcefully advances the specified quest-line (either Clint's Forge or Viego's Curse / Yoba's Virtues).";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        var player = Game1.player;
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("You must specify a quest-line to advance (either \"Forge\" or \"Curse\".");
            return;
        }

        switch (args[0].ToLowerInvariant())
        {
            case "dwarven":
            case "legacy":
            case "clint":
            case "forge":
                if (player.hasQuest((int)Quest.ForgeIntro))
                {
                    player.completeQuest((int)Quest.ForgeIntro);
                }

                player.mailReceived.Add("clintForge");
                break;
            case "viego":
            case "ruin":
            case "dawn":
            case "curse":
            case "yoba":
            case "virtues":
            case "chivalry":
            case "purification":
                if (player.hasQuest((int)Quest.CurseIntro))
                {
                    player.completeQuest((int)Quest.CurseIntro);
                    player.addQuest(Virtue.Honor);
                    player.addQuest(Virtue.Compassion);
                    player.addQuest(Virtue.Wisdom);
                    player.addQuest(Virtue.Generosity);
                    player.addQuest(Virtue.Valor);
                }

                player.Write(DataKeys.ProvenHonor, int.MaxValue.ToString());
                player.Write(DataKeys.ProvenCompassion, int.MaxValue.ToString());
                player.Write(DataKeys.ProvenWisdom, int.MaxValue.ToString());
                player.Write(DataKeys.ProvenGenerosity, true.ToString());
                player.Write(DataKeys.ProvenValor, true.ToString());
                Virtue.List.ForEach(virtue => virtue.CheckForCompletion(Game1.player));
                break;
        }
    }
}
