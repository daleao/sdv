namespace DaLion.Overhaul.Modules.Weapons.Events;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class WeaponSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="WeaponSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WeaponSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        var player = Game1.player;

        // temp fix for existing saves
        if (player.hasQuest((int)Quest.CurseIntro))
        {
            player.removeQuest((int)Quest.CurseIntro);
            player.addQuest((int)Quest.CurseIntro);
        }

        if (player.hasQuest((int)Quest.HeroJourney))
        {
            player.removeQuest((int)Quest.HeroJourney);
            player.Write(DataKeys.VirtueQuestState, VirtueQuestState.InProgress.ToString());

            for (var i = 144706; i <= 144710; i++)
            {
                if (player.hasQuest(i))
                {
                    player.removeQuest(i);
                }
            }
        }

        if (player.hasQuest((int)(Quest.ForgeIntro + 1)))
        {
            player.removeQuest((int)(Quest.ForgeIntro + 1));
        }
        // temp fix for existing saves

        WeaponsModule.State.ContainerDropAccumulator = player.Read(DataKeys.ContainerDropAccumulator, 0.05);
        WeaponsModule.State.MonsterDropAccumulator = player.Read<double>(DataKeys.MonsterDropAccumulator);

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

        if (player.hasQuest((int)Quest.ForgeIntro))
        {
            ModEntry.EventManager.Enable<BlueprintDayStartedEvent>();
        }

        // infinity +1 checks
        if (player.Read<VirtueQuestState>(DataKeys.VirtueQuestState) == VirtueQuestState.InProgress)
        {
            if (player.mailReceived.Contains("gotHolyBlade"))
            {
                player.Write(DataKeys.VirtueQuestState, VirtueQuestState.Completed.ToString());
            }
            else
            {
                WeaponsModule.State.VirtuesQuest = new VirtueQuest();
            }
        }

        if (!WeaponsModule.Config.EnableAutoSelection)
        {
            return;
        }

        // load auto-selection
        var index = player.Read(DataKeys.SelectableWeapon, -1);
        if (index < 0 || index >= player.Items.Count)
        {
            return;
        }

        var item = player.Items[index];
        if (item is not MeleeWeapon weapon || weapon.isScythe())
        {
            return;
        }

        WeaponsModule.State.AutoSelectableWeapon = weapon;
    }
}
