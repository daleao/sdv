namespace DaLion.Stardew.Arsenal.Integrations;

#region using directives

using DaLion.Common.Integrations;
using DaLion.Common.Integrations.SpaceCore;
using DaLion.Stardew.Arsenal.Framework.Enchantments;

#endregion using directives

internal sealed class SpaceCoreIntegration : BaseIntegration<ISpaceCoreApi>
{
    /// <summary>Initializes a new instance of the <see cref="SpaceCoreIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public SpaceCoreIntegration(IModRegistry modRegistry)
        : base("SpaceCore", "spacechase0.SpaceCore", "1.8.3", modRegistry)
    {
    }

    /// <summary>Cache the SpaceCore API and initialize reflected SpaceCore fields.</summary>
    public void Register()
    {
        this.AssertLoaded();
        this.ModApi.RegisterSerializerType(typeof(CarvingEnchantment));
        this.ModApi.RegisterSerializerType(typeof(CleavingEnchantment));
        this.ModApi.RegisterSerializerType(typeof(EnergizedEnchantment));
        this.ModApi.RegisterSerializerType(typeof(TributeEnchantment));
    }
}
