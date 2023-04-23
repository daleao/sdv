namespace DaLion.Overhaul.Modules.Weapons.Events;

#region using directives

using System.Linq;
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
        WeaponsModule.State.ContainerDropAccumulator = player.Read(DataKeys.ContainerDropAccumulator, 0.05);
        WeaponsModule.State.MonsterDropAccumulator = player.Read<double>(DataKeys.MonsterDropAccumulator);

        Utility.iterateAllItems(item =>
        {
            if (item is MeleeWeapon weapon && weapon.ShouldHaveIntrinsicEnchantment())
            {
                weapon.AddIntrinsicEnchantments();
            }
        });

        PerformInfinityPlusOneValidations(player);
        PerformDwarvenLegacyValidations(player);

        if (!WeaponsModule.Config.EnableAutoSelection)
        {
            return;
        }

        // load auto-selection
        var index = player.Read(DataKeys.SelectableWeapon, -1);
        if (index < 0)
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

    private static void PerformInfinityPlusOneValidations(Farmer player)
    {
        if (player.mailReceived.Contains("galaxySword"))
        {
            Game1.player.WriteIfNotExists(DataKeys.GalaxyArsenalObtained, ItemIDs.GalaxySword.ToString());
        }

        var darkSword = player.Items.FirstOrDefault(item => item is MeleeWeapon
        {
            InitialParentTileIndex: ItemIDs.DarkSword
        });
        if (darkSword is not null)
        {
            if (!player.mailReceived.Contains("gotDarkSword"))
            {
                Log.W($"[WPNZ]: {player.Name} has not officially received the Blade of Ruin, but already carries a copy in their inventory. The appropriate reception flag will be set.");
                player.mailReceived.Add("gotDarkSword");
            }

            if (darkSword.Read<int>(DataKeys.CursePoints) >= 50 && !player.hasOrWillReceiveMail("viegoCurse"))
            {
                Log.W($"[WPNZ]: {player.Name}'s Blade of Ruin has gathered enough kills, but the purification quest-line has not begun. The necessary mail will be added for tomorrow.");
                Game1.addMailForTomorrow("viegoCurse");
            }
        }
        else if (player.mailReceived.Contains("gotDarkSword"))
        {
            Log.W($"[WPNZ]: {player.Name} has received the Blade of Ruin, but does not currently hold it in their inventory. A new copy will be added.");
            darkSword = new MeleeWeapon(ItemIDs.DarkSword);
            if (!player.addItemToInventoryBool(darkSword))
            {
                Log.W($"[WPNZ]: {player.Name} could not receive the Blade of Ruin copy. It will be dropped on ground.");
                Game1.createItemDebris(darkSword, player.getStandingPosition(), -1, player.currentLocation);
            }
        }

        var holyBlade = player.Items.FirstOrDefault(item => item is MeleeWeapon
        {
            InitialParentTileIndex: ItemIDs.HolyBlade
        });
        if (holyBlade is not null)
        {
            if (darkSword is not null)
            {
                Log.W($"[WPNZ]: {player.Name} is carrying both the Blade of Ruin and Blade of Dawn! That should not be possible. No automatic actions will be taken.");
            }

            if (!player.mailReceived.Contains("gotHolyBlade"))
            {
                Log.W($"[WPNZ]: {player.Name} has not officially received the Blade of Ruin, but already carries a copy in their inventory. The appropriate reception flag will be set.");
                player.mailReceived.Add("gotHolyBlade");
                if (!player.hasQuest((int)Quest.VirtuesIntro))
                {
                    Log.W($"[WPNZ]: {player.Name} does not have the Virtues Intro quest. It will be auto-completed.");
                    player.addQuest((int)Quest.VirtuesIntro);
                    player.completeQuest((int)Quest.VirtuesIntro);
                }

                if (!player.hasQuest((int)Quest.VirtuesNext))
                {
                    Log.W($"[WPNZ]: {player.Name} does not have the Virtues Follow-up quest. It will be auto-completed.");
                    player.addQuest((int)Quest.VirtuesNext);
                    player.completeQuest((int)Quest.VirtuesNext);
                }

                if (!player.hasQuest((int)Quest.VirtuesLast))
                {
                    Log.W($"[WPNZ]: {player.Name} does not have the Virtues Final quest. It will be auto-completed.");
                    player.addQuest((int)Quest.VirtuesLast);
                    player.completeQuest((int)Quest.VirtuesLast);
                }
            }
        }

        if (player.NumMonsterSlayerQuestsCompleted() >= 5)
        {
            Log.W($"[WPNZ]: {player.Name} has proven their valor. The corresponding flag will be set.");
            player.WriteIfNotExists(DataKeys.ProvenValor, true.ToString());
        }

        if (Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
        {
            Log.W($"[WPNZ]: {player.Name} has proven their generosity. The corresponding flag will be set.");
            player.WriteIfNotExists(DataKeys.ProvenGenerosity, true.ToString());
        }
    }

    private static void PerformDwarvenLegacyValidations(Farmer player)
    {
        if (!string.IsNullOrEmpty(player.Read(DataKeys.BlueprintsFound)) && player.canUnderstandDwarves)
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
        }

        if (player.hasQuest((int)Quest.ForgeIntro))
        {
            ModEntry.EventManager.Enable<BlueprintDayStartedEvent>();
        }
    }
}
