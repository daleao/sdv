namespace DaLion.Overhaul.Modules.Weapons.Commands;

#region using directives

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
                if (player.Read<VirtueQuestState>(DataKeys.VirtueQuestState) == VirtueQuestState.NotStarted)
                {
                    if (!player.hasQuest((int)Quest.CurseIntro))
                    {
                        player.addQuest((int)Quest.CurseIntro);
                        return;
                    }

                    player.completeQuest((int)Quest.CurseIntro);
                    player.Write(DataKeys.InspectedHonor, null);
                    player.Write(DataKeys.InspectedCompassion, null);
                    player.Write(DataKeys.InspectedWisdom, null);
                    player.Write(DataKeys.InspectedGenerosity, null);
                    player.Write(DataKeys.InspectedValor, null);
                    WeaponsModule.State.VirtuesQuest ??= new VirtueQuest();
                    player.Write(DataKeys.VirtueQuestState, VirtueQuestState.InProgress.ToString());
                    return;
                }

                if (player.Read<VirtueQuestState>(DataKeys.VirtueQuestState) == VirtueQuestState.InProgress)
                {
                    WeaponsModule.State.VirtuesQuest ??= new VirtueQuest();
                    player.Write(DataKeys.ProvenHonor, int.MaxValue.ToString());
                    player.Write(DataKeys.ProvenCompassion, int.MaxValue.ToString());
                    player.Write(DataKeys.ProvenWisdom, int.MaxValue.ToString());
                    player.Write(DataKeys.ProvenGenerosity, int.MaxValue.ToString());
                    Virtue.List.ForEach(virtue => WeaponsModule.State.VirtuesQuest.UpdateVirtueProgress(virtue));
                }

                break;
            case "honor":
                {
                    if (args.Length == 1 || int.TryParse(args[1], out var amount))
                    {
                        amount = 1;
                    }

                    player.Increment(DataKeys.ProvenHonor, amount);
                    WeaponsModule.State.VirtuesQuest?.UpdateVirtueProgress(Virtue.Honor);
                }

                break;
            case "compassion":
                {
                    if (args.Length == 1 || int.TryParse(args[1], out var amount))
                    {
                        amount = 1;
                    }

                    player.Increment(DataKeys.ProvenCompassion, amount);
                    WeaponsModule.State.VirtuesQuest?.UpdateVirtueProgress(Virtue.Compassion);
                }

                break;
            case "wisdom":
                {
                    if (args.Length == 1 || int.TryParse(args[1], out var amount))
                    {
                        amount = 1;
                    }

                    player.Increment(DataKeys.ProvenWisdom, amount);
                    WeaponsModule.State.VirtuesQuest?.UpdateVirtueProgress(Virtue.Wisdom);
                }

                break;
            case "generosity":
                {
                    if (args.Length == 1 || int.TryParse(args[1], out var amount))
                    {
                        amount = 1;
                    }

                    player.Increment(DataKeys.ProvenGenerosity, amount);
                    WeaponsModule.State.VirtuesQuest?.UpdateVirtueProgress(Virtue.Generosity);
                }

                break;
            case "valor":
                {
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
                }

                break;
        }
    }
}
