namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ProfessionGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProfessionGameLaunchedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProfessionGameLaunchedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        // hard dependencies
        SpaceCoreIntegration.Instance!.Register();

        // soft dependencies
        LuckSkillIntegration.Instance?.Register();
        LoveOfCookingIntegration.Instance?.Register();
        AutomateIntegration.Instance?.Register();
        TehsFishingOverhaulIntegration.Instance?.Register();
        CustomOreNodesIntegration.Instance?.Register();
        StardewValleyExpandedIntegration.Instance?.Register();
    }
}
