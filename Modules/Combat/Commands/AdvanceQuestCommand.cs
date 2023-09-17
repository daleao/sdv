namespace DaLion.Overhaul.Modules.Combat.Commands;

#region using directives

using DaLion.Overhaul.Modules.Combat;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewValley;

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
                if (player.mailReceived.Contains("clintForge"))
                {
                    Log.W("This quest was already completed.");
                    return;
                }

                if (!player.hasQuest((int)QuestId.ForgeIntro))
                {
                    player.addQuest((int)QuestId.ForgeIntro);
                }
                else
                {
                    player.completeQuest((int)QuestId.ForgeIntro);
                    if (!player.mailReceived.Contains("clintForge"))
                    {
                        player.mailReceived.Add("clintForge");
                    }
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
                if (player.mailReceived.Contains("gotHolyBlade"))
                {
                    Log.W("This quest was already completed.");
                    return;
                }

                if (player.Read<HeroQuest.QuestState>(DataKeys.VirtueQuestState) == HeroQuest.QuestState.NotStarted)
                {
                    if (!player.hasQuest((int)QuestId.CurseIntro))
                    {
                        player.addQuest((int)QuestId.CurseIntro);
                        return;
                    }

                    player.completeQuest((int)QuestId.CurseIntro);
                    player.Write(DataKeys.InspectedHonor, null);
                    player.Write(DataKeys.InspectedCompassion, null);
                    player.Write(DataKeys.InspectedWisdom, null);
                    player.Write(DataKeys.InspectedGenerosity, null);
                    player.Write(DataKeys.InspectedValor, null);
                    CombatModule.State.HeroQuest ??= new HeroQuest();
                    player.Write(DataKeys.VirtueQuestState, HeroQuest.QuestState.InProgress.ToString());
                    return;
                }

                if (player.Read<HeroQuest.QuestState>(DataKeys.VirtueQuestState) == HeroQuest.QuestState.InProgress)
                {
                    CombatModule.State.HeroQuest ??= new HeroQuest();
                    Virtue.List.ForEach(virtue =>
                    {
                        player.Write(virtue.Name, int.MaxValue.ToString());
                        CombatModule.State.HeroQuest.UpdateTrialProgress(virtue);
                    });
                }

                break;
            case "honor":
                {
                    if (args.Length == 1 || !int.TryParse(args[1], out var amount))
                    {
                        amount = 1;
                    }

                    player.Increment(Virtue.Honor.Name, amount);
                    CombatModule.State.HeroQuest?.UpdateTrialProgress(Virtue.Honor);
                }

                break;
            case "compassion":
                {
                    if (args.Length == 1 || !int.TryParse(args[1], out var amount))
                    {
                        amount = 1;
                    }

                    player.Increment(Virtue.Compassion.Name, amount);
                    CombatModule.State.HeroQuest?.UpdateTrialProgress(Virtue.Compassion);
                }

                break;
            case "wisdom":
                {
                    if (args.Length == 1 || !int.TryParse(args[1], out var amount))
                    {
                        amount = 1;
                    }

                    player.Increment(Virtue.Wisdom.Name, amount);
                    CombatModule.State.HeroQuest?.UpdateTrialProgress(Virtue.Wisdom);
                }

                break;
            case "generosity":
                {
                    if (args.Length == 1 || !int.TryParse(args[1], out var amount))
                    {
                        amount = 1;
                    }

                    player.Increment(Virtue.Generosity.Name, amount);
                    CombatModule.State.HeroQuest?.UpdateTrialProgress(Virtue.Generosity);
                }

                break;
            case "valor":
                {
                    if (args.Length == 1 || !int.TryParse(args[1], out var amount))
                    {
                        amount = 1;
                    }

                    player.Increment(Virtue.Valor.Name, amount);
                    CombatModule.State.HeroQuest?.UpdateTrialProgress(Virtue.Generosity);
                }

                break;
        }
    }
}
