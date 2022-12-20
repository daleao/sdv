namespace DaLion.Overhaul.Modules.Arsenal.Integrations;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Enchantments;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.SpaceCore;

#endregion using directives

[RequiresMod("spacechase0.SpaceCore", "SpaceCore", "1.8.3")]
internal sealed class SpaceCoreIntegration : ModIntegration<SpaceCoreIntegration, ISpaceCoreApi>
{
    private SpaceCoreIntegration()
        : base("spacechase0.SpaceCore", "SpaceCore", "1.8.3", ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
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

        this.ModApi.RegisterSerializerType(typeof(LavaEnchantment));
        this.ModApi.RegisterSerializerType(typeof(ObsidianEnchantment));

        return true;
    }
}
