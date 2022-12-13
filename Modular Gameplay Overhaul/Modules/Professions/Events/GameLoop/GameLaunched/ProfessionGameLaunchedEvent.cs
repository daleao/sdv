namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
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
        var registry = ModHelper.ModRegistry;

        new Integrations.SpaceCoreIntegration(registry).Register();

        // add Luck Skill integration
        if (registry.IsLoaded("spacechase0.LuckSkill"))
        {
            new Integrations.LuckSkillIntegration(registry).Register();
        }

        // add Love Of Cooking integration
        if (registry.IsLoaded("blueberry.LoveOfCooking"))
        {
            new Integrations.LoveOfCookingIntegration(registry).Register();
        }

        // add Automate integration
        if (registry.IsLoaded("Pathoschild.Automate"))
        {
            new Integrations.AutomateIntegration(registry).Register();
        }

        // add Teh's Fishing Overhaul integration
        if (registry.IsLoaded("TehPers.FishingOverhaul"))
        {
            new Integrations.TehsFishingOverhaulIntegration(registry, ModHelper.Events)
                .Register();
        }

        // add Custom Ore Nodes integration
        if (registry.IsLoaded("aedenthorn.CustomOreNodes"))
        {
            new Integrations.CustomOreNodesIntegration(registry).Register();
        }

        // add SVE integration
        if (registry.IsLoaded("FlashShifter.StardewValleyExpandedCP"))
        {
            new Integrations.StardewValleyExpandedIntegration(registry).Register();
        }
    }
}
