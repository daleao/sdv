namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CoreLateLoadOneSecondUpdateTickedEvent : OneSecondUpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CoreLateLoadOneSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CoreLateLoadOneSecondUpdateTickedEvent(EventManager manager)
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

        if (EnumerateModules().Skip(1).Where(module => module._ShouldEnable).Any(module => !module.HasFinishedLoading))
        {
            return;
        }

        OverhaulModule.Core.RegisterIntegrations();
        if (GenericModConfigMenu.Instance?.IsRegistered != true || !Config.LaunchInitialSetup)
        {
            this.Manager.Unmanage(this);
            return;
        }

        Log.I("Launching GMCM for initial setup.");
        GenericModConfigMenu.Instance.ModApi!.OpenModMenu(Manifest);
        Config.LaunchInitialSetup = false;
        ModHelper.WriteConfig(Config);
        this.Manager.Unmanage(this);
    }
}
