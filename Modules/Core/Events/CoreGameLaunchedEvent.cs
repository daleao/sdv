namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
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
        if (!LocalData.InitialSetupComplete &&
            EnumerateModules().Skip(1).Any(module => module is not (ProfessionsModule or TweexModule)))
        {
            LocalData.InitialSetupComplete = true;
            ModHelper.Data.WriteJsonFile("data.json", LocalData);
        }

        EnumerateModules().Skip(1).ForEach(module => module.RegisterIntegrations());
    }
}
