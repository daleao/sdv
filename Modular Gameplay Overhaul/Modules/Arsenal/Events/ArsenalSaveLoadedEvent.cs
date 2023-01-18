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
        if (!string.IsNullOrEmpty(player.Read(DataFields.BlueprintsFound)) && player.canUnderstandDwarves)
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
        }

        if (player.hasQuest(Constants.ForgeIntroQuestId))
        {
            this.Manager.Enable<BlueprintDayStartedEvent>();
        }

        if (player.Items.FirstOrDefault(
                item => item is MeleeWeapon { InitialParentTileIndex: Constants.DarkSwordIndex }) is not null &&
            !player.hasOrWillReceiveMail("viegoCurse"))
        {
            Game1.addMailForTomorrow("viegoCurse");
        }

        if (Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
        {
            player.WriteIfNotExists(DataFields.ProvenGenerosity, true.ToString());
        }

        if (player.NumMonsterSlayerQuestsCompleted() >= 5)
        {
            player.WriteIfNotExists(DataFields.ProvenValor, true.ToString());
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

        var slots = Game1.player.Read(DataFields.SelectableSlots).ParseList<int>();
        if (slots.Count == 0)
        {
            return;
        }

        var leftover = slots.ToList();
        for (var i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (slot < 0)
            {
                leftover.Remove(slot);
                continue;
            }

            var item = Game1.player.Items[slot];
            if (item is not (Tool tool and (MeleeWeapon or Slingshot)))
            {
                continue;
            }

            if (tool is MeleeWeapon weapon && weapon.isScythe())
            {
                continue;
            }

            ArsenalModule.State.SelectableArsenal = tool;
            leftover.Remove(slot);
            break;
        }

        Game1.player.Write(DataFields.SelectableSlots, string.Join(',', leftover));
    }
}
