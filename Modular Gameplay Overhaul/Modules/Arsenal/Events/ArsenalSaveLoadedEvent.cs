namespace DaLion.Overhaul.Modules.Arsenal.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
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
            ModHelper.GameContent.InvalidateCache("Data/Events/Blacksmith");
        }

        if (player.hasQuest(Constants.ForgeIntroQuestId))
        {
            this.Manager.Enable<BlueprintDayStartedEvent>();
        }

        if (player.Items.FirstOrDefault(
                item => item is MeleeWeapon { InitialParentTileIndex: Constants.DarkSwordIndex }) is not null &&
            !player.hasOrWillReceiveMail("viegoCurse"))
        {
            player.mailForTomorrow.Add("viegoCurse");
        }

        if (Game1.options.useLegacySlingshotFiring)
        {
            ModEntry.Config.Arsenal.Slingshots.BullseyeReplacesCursor = false;
            ModHelper.WriteConfig(ModEntry.Config);
        }
    }
}
