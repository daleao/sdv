namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.SaveLoaded;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop.DayStarted;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Constants;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
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
            switch (item)
            {
                case MeleeWeapon weapon when weapon.ShouldHaveIntrinsicEnchantment():
                    weapon.AddIntrinsicEnchantments();
                    break;
                case Slingshot slingshot when slingshot.ShouldHaveIntrinsicEnchantment():
                    slingshot.AddIntrinsicEnchantments();
                    break;
            }
        });

        // patch clint event
        if (!string.IsNullOrEmpty(player.Read(DataKeys.BlueprintsFound)) && player.canUnderstandDwarves)
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
        }

        // continue clint translation
        if (player.hasQuest((int)QuestId.ForgeIntro))
        {
            player.questLog.First(q => q.id.Value == (int)QuestId.ForgeIntro).nextQuests.Clear(); // this is a temporary fix for this goddamn error
            ModEntry.EventManager.Enable<BlueprintDayStartedEvent>();
        }

        // record pre-obtained galaxy sword
        if (player.mailReceived.Contains("galaxySword"))
        {
            player.WriteIfNotExists(DataKeys.GalaxyArsenalObtained, WeaponIds.GalaxySword.ToString());
        }

        // load hero quest if necessary
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
        else if (player.Read<HeroQuest.QuestState>(DataKeys.VirtueQuestState) == HeroQuest.QuestState.Completed)
        {
            if (!player.hasQuest((int)QuestId.HeroReward) && !player.mailReceived.Contains("gotHolyBlade"))
            {
                player.addQuest((int)QuestId.HeroReward);
            }
        }

        // block bullseye cursor
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
        if (player.Items.IsIndexInBounds(index))
        {
            var item = player.Items[index];
            if (item is MeleeWeapon weapon && !weapon.isScythe())
            {
                CombatModule.State.AutoSelectableMelee = weapon;
            }
        }

        index = player.Read(DataKeys.SelectableRanged, -1);
        if (player.Items.IsIndexInBounds(index))
        {
            var item = player.Items[index];
            if (item is Slingshot slingshot)
            {
                CombatModule.State.AutoSelectableRanged = slingshot;
            }
        }
    }
}
