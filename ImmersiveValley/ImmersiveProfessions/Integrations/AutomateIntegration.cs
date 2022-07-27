namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using Common.Integrations;
using Common.Integrations.Automate;
using StardewModdingAPI;

#endregion using directives

internal sealed class AutomateIntegration : BaseIntegration
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public AutomateIntegration(IModRegistry modRegistry)
        : base("Automate", "Pathoschild.Automate", "1.27.3", modRegistry) { }

    /// <summary>Initialize reflected Automate fields.</summary>
    public void Register(IModHelper helper)
    {
        AssertLoaded();
        ExtendedAutomateAPI.Init(helper);
    }
}