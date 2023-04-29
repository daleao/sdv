namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class CoreGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CoreGameLaunchedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CoreGameLaunchedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        OverhaulModule.Core.RegisterIntegrations();
        if (GenericModConfigMenu.Instance?.IsRegistered != true)
        {
            this.Manager.Unmanage(this.Manager.Get<CoreOneSecondUpdateTickedEvent>()!);
            return;
        }

        if (EnumerateModules().Skip(1).Any(module => module is not (ProfessionsModule or TweexModule) && module._ShouldEnable))
        {
            Data.InitialSetupComplete = true;
            ModHelper.Data.WriteJsonFile("data.json", Data);
        }

        if (Data.InitialSetupComplete)
        {
            this.Manager.Unmanage(this.Manager.Get<CoreOneSecondUpdateTickedEvent>()!);
            return;
        }

        this.Manager.Enable<CoreOneSecondUpdateTickedEvent>();
    }
}
