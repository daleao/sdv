namespace DaLion.Overhaul.Modules.Weapons.Events;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
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
        if (!player.Read<bool>(DataKeys.Revalidated))
        {
            if (!(Game1.dayOfMonth == 0 && Game1.currentSeason == "spring" && Game1.year == 1))
            {
                Utils.RevalidateAllWeapons();
            }

            player.Write(DataKeys.Revalidated, true.ToString());
        }

        WeaponsModule.State.ContainerDropAccumulator = player.Read(DataKeys.ContainerDropAccumulator, 0.05);
        WeaponsModule.State.MonsterDropAccumulator = player.Read<double>(DataKeys.MonsterDropAccumulator);

        if (!string.IsNullOrEmpty(player.Read(DataKeys.BlueprintsFound)) && player.canUnderstandDwarves)
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
        }

        if (player.mailReceived.Contains("galaxySword"))
        {
            Game1.player.WriteIfNotExists(DataKeys.GalaxyArsenalObtained, ItemIDs.GalaxySword.ToString());
        }

        if (player.hasQuest((int)Quest.ForgeIntro))
        {
            this.Manager.Enable<BlueprintDayStartedEvent>();
        }

        if (player.Items.FirstOrDefault(item =>
                item is MeleeWeapon { InitialParentTileIndex: ItemIDs.DarkSword } &&
                item.Read<int>(DataKeys.CursePoints) >= 50) is not null && !player.hasOrWillReceiveMail("viegoCurse"))
        {
            Game1.addMailForTomorrow("viegoCurse");
        }

        if (Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
        {
            player.WriteIfNotExists(DataKeys.ProvenGenerosity, true.ToString());
        }

        if (player.NumMonsterSlayerQuestsCompleted() >= 5)
        {
            player.WriteIfNotExists(DataKeys.ProvenValor, true.ToString());
        }

        if (!WeaponsModule.Config.EnableAutoSelection)
        {
            return;
        }

        var indices = player.Read(DataKeys.SelectableWeapon).ParseList<int>();
        if (indices.Count == 0)
        {
            return;
        }

        var leftover = indices.ToList();
        for (var i = 0; i < indices.Count; i++)
        {
            var index = indices[i];
            if (index < 0)
            {
                leftover.Remove(index);
                continue;
            }

            var item = player.Items[index];
            if (item is not MeleeWeapon weapon || weapon.isScythe())
            {
                continue;
            }

            WeaponsModule.State.AutoSelectableWeapon = weapon;
            leftover.Remove(index);
            break;
        }

        player.Write(DataKeys.SelectableWeapon, string.Join(',', leftover));
    }
}
