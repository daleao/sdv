namespace DaLion.Overhaul.Modules.Arsenal.Integrations;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Overhaul.Modules.Arsenal.Enchantments;
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
        ModEntry.Integrations[this.ModName] = this;
    }

    /// <summary>Gets the <see cref="ISpaceCoreApi"/>.</summary>
    internal static ISpaceCoreApi? Api { get; private set; }

    /// <inheritdoc />
    [MemberNotNull(nameof(Api))]
    protected override void RegisterImpl()
    {
        this.AssertLoaded();
        Api = this.ModApi;

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

        this.ModApi.RegisterSerializerType(typeof(LavaEnchantment));
        this.ModApi.RegisterSerializerType(typeof(ObsidianEnchantment));
    }
}
