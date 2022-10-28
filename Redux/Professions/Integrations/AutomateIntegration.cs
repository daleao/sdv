namespace DaLion.Redux.Professions.Integrations;

#region using directives

using DaLion.Shared.Integrations;

#endregion using directives

internal sealed class AutomateIntegration : BaseIntegration
{
    /// <summary>Initializes a new instance of the <see cref="AutomateIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public AutomateIntegration(IModRegistry modRegistry)
        : base("Automate", "Pathoschild.Automate", "1.27.3", modRegistry)
    {
    }

    /// <summary>Initialize reflected Automate fields.</summary>
    public void Register()
    {
        this.AssertLoaded();
        ExtendedAutomateApi.Init();
    }
}
