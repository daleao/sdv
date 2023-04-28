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
            case "forge":
            case "clint":
            case "dwarven":
            case "legacy":
                if (player.hasQuest((int)Quest.ForgeIntro))
                {
                    player.completeQuest((int)Quest.ForgeIntro);
                }

                if (!player.mailReceived.Contains("clintForge"))
                {
                    player.mailReceived.Add("clintForge");
                }

                break;
            case "hero":
            case "ruin":
            case "dawn":
            case "curse":
            case "viego":
            case "yoba":
            case "virtues":
            case "chivalry":
            case "purification":
                if (player.hasQuest((int)Quest.CurseIntro))
                {
                    player.completeQuest((int)Quest.CurseIntro);
                    WeaponsModule.State.Quest ??= new VirtuesQuest();
                    return;
                }

                if (WeaponsModule.State.Quest is { } quest)
                {
                    player.Write(DataKeys.ProvenHonor, int.MaxValue.ToString());
                    player.Write(DataKeys.ProvenCompassion, int.MaxValue.ToString());
                    player.Write(DataKeys.ProvenWisdom, int.MaxValue.ToString());
                    player.Write(DataKeys.ProvenGenerosity, int.MaxValue.ToString());
                    Virtue.List.ForEach(virtue => quest.UpdateVirtueProgress(virtue));
                }

                break;
        }
    }
}
