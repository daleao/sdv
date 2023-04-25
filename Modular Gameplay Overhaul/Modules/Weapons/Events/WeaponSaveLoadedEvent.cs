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
        if (player.hasQuest((int)Quest.CurseNext) && Virtue.AllProvenBy(player))
        {
            Log.W("[WPNZ]: Congratulations on proving all virtues! Go on to receive your Holy Blade.");
            player.completeQuest((int)Quest.CurseNext);
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
}
