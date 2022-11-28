namespace DaLion.Ligo.Modules.Arsenal.Integrations;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.SpaceCore;

#endregion using directives

internal sealed class SpaceCoreIntegration : BaseIntegration<ISpaceCoreApi>
{
    /// <summary>Initializes a new instance of the <see cref="SpaceCoreIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    internal SpaceCoreIntegration(IModRegistry modRegistry)
        : base("SpaceCore", "spacechase0.SpaceCore", "1.8.3", modRegistry)
    {
    }

    /// <summary>Cache the SpaceCore API and initialize reflected SpaceCore fields.</summary>
    internal void Register()
    {
        this.AssertLoaded();
        this.ModApi.RegisterSerializerType(typeof(BloodthirstyEnchantment));
        this.ModApi.RegisterSerializerType(typeof(CarvingEnchantment));
        this.ModApi.RegisterSerializerType(typeof(CleavingEnchantment));
        this.ModApi.RegisterSerializerType(typeof(EnergizedEnchantment));
        this.ModApi.RegisterSerializerType(typeof(ReduxArtfulEnchantment));
        this.ModApi.RegisterSerializerType(typeof(TributeEnchantment));

        this.ModApi.RegisterSerializerType(typeof(BaseSlingshotEnchantment));
        this.ModApi.RegisterSerializerType(typeof(EngorgingEnchantment));
        this.ModApi.RegisterSerializerType(typeof(GatlingEnchantment));
        this.ModApi.RegisterSerializerType(typeof(PreservingEnchantment));
        this.ModApi.RegisterSerializerType(typeof(QuincyEnchantment));
        this.ModApi.RegisterSerializerType(typeof(SpreadingEnchantment));

        this.ModApi.RegisterSerializerType(typeof(BlessedEnchantment));
        this.ModApi.RegisterSerializerType(typeof(CursedEnchantment));
        this.ModApi.RegisterSerializerType(typeof(GarnetEnchantment));
        this.ModApi.RegisterSerializerType(typeof(InfinityEnchantment));
    }
}
