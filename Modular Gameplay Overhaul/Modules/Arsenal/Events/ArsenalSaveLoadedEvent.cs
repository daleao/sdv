namespace DaLion.Overhaul.Modules.Arsenal.Events;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ArsenalSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ArsenalSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalSaveLoadedEvent(EventManager manager)
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

        ArsenalModule.State.ContainerDropAccumulator = player.Read(DataKeys.ContainerDropAccumulator, 0.05);
        ArsenalModule.State.MonsterDropAccumulator = player.Read<double>(DataKeys.MonsterDropAccumulator);

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

        if (Game1.options.useLegacySlingshotFiring)
        {
            ModEntry.Config.Arsenal.Slingshots.BullseyeReplacesCursor = false;
            ModHelper.WriteConfig(ModEntry.Config);
        }

        if (!ArsenalModule.Config.EnableAutoSelection)
        {
            return;
        }

        var indices = Game1.player.Read(DataKeys.SelectableArsenal).ParseList<int>();
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

            var item = Game1.player.Items[index];
            if (item is not (Tool tool and (MeleeWeapon or Slingshot)))
            {
                continue;
            }

            if (tool is MeleeWeapon weapon && weapon.isScythe())
            {
                continue;
            }

            ArsenalModule.State.SelectableArsenal = tool;
            leftover.Remove(index);
            break;
        }

        Game1.player.Write(DataKeys.SelectableArsenal, string.Join(',', leftover));
    }
}
