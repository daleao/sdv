namespace DaLion.Stardew.Arsenal.Integrations;

#region using directives

using Common.Integrations;
using Common.Integrations.SpaceCore;
using Framework.Enchantments;

#endregion using directives

internal sealed class SpaceCoreIntegration : BaseIntegration<ISpaceCoreAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public SpaceCoreIntegration(IModRegistry modRegistry)
        : base("SpaceCore", "spacechase0.SpaceCore", "1.8.3", modRegistry) { }

    /// <summary>Cache the SpaceCore API and initialize reflected SpaceCore fields.</summary>
    public void Register()
    {
        AssertLoaded();
        ModApi.RegisterSerializerType(typeof(BaseSlingshotEnchantment));
        ModApi.RegisterSerializerType(typeof(GatlingEnchantment));
        ModApi.RegisterSerializerType(typeof(QuincyEnchantment));
        ModApi.RegisterSerializerType(typeof(SpreadingEnchantment));
        ModApi.RegisterSerializerType(typeof(CarvingEnchantment));
        ModApi.RegisterSerializerType(typeof(CleavingEnchantment));
        ModApi.RegisterSerializerType(typeof(EnergizedEnchantment));
        ModApi.RegisterSerializerType(typeof(TributeEnchantment));
    }
}