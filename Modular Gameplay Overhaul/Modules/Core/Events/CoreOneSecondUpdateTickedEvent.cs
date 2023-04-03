namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class CoreOneSecondUpdateTickedEvent : OneSecondUpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CoreOneSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CoreOneSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (Game1.ticks <= 1 || Game1.currentGameTime == null || Game1.activeClickableMenu is not TitleMenu ||
            TitleMenu.subMenu is ConfirmationDialog)
        {
            return;
        }

        Log.I("Opening GMCM for initial setup.");
        GenericModConfigMenu.Instance!.ModApi!.OpenModMenu(Manifest);
        Data.InitialSetupComplete = true;
        ModHelper.Data.WriteJsonFile("data.json", Data);
        GenericModConfigMenu.Instance.Reload();
        this.Disable();
        this.Dispose();
    }
}
