namespace DaLion.Overhaul.Modules.Slingshots.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class SlingshotSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlingshotSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        if (Game1.options.useLegacySlingshotFiring)
        {
            SlingshotsModule.Config.BullseyeReplacesCursor = false;
            ModHelper.WriteConfig(ModEntry.Config);
            Log.W(
                "[Slingshots]: Bullseye cursor settings is not compatible with pull-back firing mode. Please change to hold-and-release to use this option.");
        }

        if (!SlingshotsModule.Config.EnableAutoSelection)
        {
            return;
        }

        var player = Game1.player;
        var index = player.Read(DataKeys.SelectableSlingshot, -1);
        if (index < 0)
        {
            return;
        }

        var item = player.Items[index];
        if (item is not Slingshot slingshot)
        {
            return;
        }

        SlingshotsModule.State.AutoSelectableSlingshot = slingshot;
    }
}
