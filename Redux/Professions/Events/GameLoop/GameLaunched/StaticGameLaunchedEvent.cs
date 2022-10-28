namespace DaLion.Redux.Professions.Events.GameLoop;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabled]
internal sealed class StaticGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Initializes a new instance of the <see cref="StaticGameLaunchedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal StaticGameLaunchedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        var registry = ModEntry.ModHelper.ModRegistry;

        // add SpaceCore integration
        if (registry.IsLoaded("spacechase0.SpaceCore"))
        {
            new Integrations.SpaceCoreIntegration(registry).Register();
        }

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
            new Integrations.TehsFishingOverhaulIntegration(registry, ModEntry.ModHelper.Events)
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
            Redux.Integrations.SveConfig = ModEntry.ModHelper.ReadContentPackConfig("FlashShifter.StardewValleyExpandedCP");
        }
    }
}
