namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.LoveOfCooking;

#endregion using directives

internal sealed class LoveOfCookingIntegration : BaseIntegration<ICookingSkillApi>
{
    /// <summary>Initializes a new instance of the <see cref="LoveOfCookingIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    internal LoveOfCookingIntegration(IModRegistry modRegistry)
        : base("LoveOfCooking", "blueberry.LoveOfCooking", "1.0.27", modRegistry)
    {
        ModEntry.Integrations[this.ModName] = this;
    }

    /// <summary>Gets the <see cref="ICookingSkillApi"/>.</summary>
    internal static ICookingSkillApi? Api { get; private set; }

    /// <inheritdoc />
    [MemberNotNull(nameof(Api))]
    protected override void RegisterImpl()
    {
        this.AssertLoaded();
        Api = this.ModApi;
    }
}
