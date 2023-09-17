namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CombatSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CombatSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CombatSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        var player = Game1.player;

        CombatModule.State.ContainerDropAccumulator = player.Read(DataKeys.ContainerDropAccumulator, 0.05);
        CombatModule.State.MonsterDropAccumulator = player.Read<double>(DataKeys.MonsterDropAccumulator);

        Utility.iterateAllItems(item =>
        {
            if (item is MeleeWeapon weapon && weapon.ShouldHaveIntrinsicEnchantment())
            {
                weapon.AddIntrinsicEnchantments();
            }
        });

        // dwarven legacy checks
        if (!string.IsNullOrEmpty(player.Read(DataKeys.BlueprintsFound)) && player.canUnderstandDwarves)
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
        }

        if (player.hasQuest((int)QuestId.ForgeIntro))
        {
            ModEntry.EventManager.Enable<BlueprintDayStartedEvent>();
        }

        // infinity +1 checks

        // -- temp fixes --

        if (player.Read<int>("ProvenHonor") is var provenHonor and > 0)
        {
            player.WriteIfNotExists("ProvenHonor", provenHonor.ToString());
            player.Write("ProvenHonor", null);
        }

        if (player.Read<int>("ProvenCompassion") is var provenCompassion and > 0)
        {
            player.WriteIfNotExists("ProvenCompassion", provenCompassion.ToString());
            player.Write("ProvenCompassion", null);
        }

        if (player.Read<int>("ProvenWisdom") is var provenWisdom and > 0)
        {
            player.WriteIfNotExists("ProvenWisdom", provenWisdom.ToString());
            player.Write("ProvenWisdom", null);
        }

        if (player.Read<int>("ProvenGenerosity") is var provenGenerosity and > 0)
        {
            player.WriteIfNotExists("ProvenGenerosity", provenGenerosity.ToString());
            player.Write("ProvenGenerosity", null);
        }

        if (player.Read<int>("ProvenValor") is var provenValor and > 0)
        {
            player.WriteIfNotExists("ProvenValor", provenValor.ToString());
            player.Write("ProvenValor", null);
        }

        // -- temp fixes ==

        if (player.Read<HeroQuest.QuestState>(DataKeys.VirtueQuestState) == HeroQuest.QuestState.InProgress)
        {
            if (player.mailReceived.Contains("gotHolyBlade"))
            {
                player.Write(DataKeys.VirtueQuestState, HeroQuest.QuestState.Completed.ToString());
            }
            else if (Virtue.AllProven(player))
            {
                HeroQuest.Complete();
            }
            else
            {
                CombatModule.State.HeroQuest = new HeroQuest();
            }
        }

        if (Game1.options.useLegacySlingshotFiring)
        {
            CombatModule.Config.BullseyeReplacesCursor = false;
            ModHelper.WriteConfig(ModEntry.Config);
            Log.W(
                "[CMBT]: Bullseye cursor settings is not compatible with pull-back firing mode. Switch to hold-and-release to use this option.");
        }

        if (!CombatModule.Config.EnableAutoSelection)
        {
            return;
        }

        // load auto-selections
        var index = player.Read(DataKeys.SelectableMelee, -1);
        if (index > 0 && index < player.Items.Count)
        {
            var item = player.Items[index];
            if (item is MeleeWeapon weapon && !weapon.isScythe())
            {
                CombatModule.State.AutoSelectableMelee = weapon;
            }
        }

        index = player.Read(DataKeys.SelectableRanged, -1);
        if (index > 0 && index < player.Items.Count)
        {
            var item = player.Items[index];
            if (item is Slingshot slingshot)
            {
                CombatModule.State.AutoSelectableRanged = slingshot;
            }
        }
    }
}
