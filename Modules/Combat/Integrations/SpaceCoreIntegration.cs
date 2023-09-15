namespace DaLion.Overhaul.Modules.Combat.Integrations;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.SpaceCore;

#endregion using directives

[ModRequirement("spacechase0.SpaceCore", "SpaceCore", "1.12.0")]
internal sealed class SpaceCoreIntegration : ModIntegration<SpaceCoreIntegration, ISpaceCoreApi>
{
    /// <summary>Initializes a new instance of the <see cref="SpaceCoreIntegration"/> class.</summary>
    internal SpaceCoreIntegration()
        : base("spacechase0.SpaceCore", "SpaceCore", "1.12.0", ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        this.AssertLoaded();

        // melee
        this.ModApi.RegisterSerializerType(typeof(MeleeArtfulEnchantment));
        this.ModApi.RegisterSerializerType(typeof(CarvingEnchantment));
        this.ModApi.RegisterSerializerType(typeof(CleavingEnchantment));
        this.ModApi.RegisterSerializerType(typeof(EnergizedEnchantment));
        this.ModApi.RegisterSerializerType(typeof(ExplosiveEnchantment));
        this.ModApi.RegisterSerializerType(typeof(MammoniteEnchantment));
        this.ModApi.RegisterSerializerType(typeof(BloodthirstyEnchantment));
        this.ModApi.RegisterSerializerType(typeof(SteadfastEnchantment));
        this.ModApi.RegisterSerializerType(typeof(WabbajackEnchantment));

        // ranged
        this.ModApi.RegisterSerializerType(typeof(BaseSlingshotEnchantment));
        this.ModApi.RegisterSerializerType(typeof(RangedArtfulEnchantment));
        this.ModApi.RegisterSerializerType(typeof(MagnumEnchantment));
        this.ModApi.RegisterSerializerType(typeof(GatlingEnchantment));
        this.ModApi.RegisterSerializerType(typeof(PreservingEnchantment));
        this.ModApi.RegisterSerializerType(typeof(QuincyEnchantment));
        this.ModApi.RegisterSerializerType(typeof(SpreadingEnchantment));

        // gemstone
        this.ModApi.RegisterSerializerType(typeof(GarnetEnchantment));

        // implicit
        this.ModApi.RegisterSerializerType(typeof(BlessedEnchantment));
        this.ModApi.RegisterSerializerType(typeof(CursedEnchantment));
        this.ModApi.RegisterSerializerType(typeof(InfinityEnchantment));
        this.ModApi.RegisterSerializerType(typeof(DaggerEnchantment));
        this.ModApi.RegisterSerializerType(typeof(KillerBugEnchantment));
        this.ModApi.RegisterSerializerType(typeof(LavaEnchantment));
        this.ModApi.RegisterSerializerType(typeof(NeedleEnchantment));
        this.ModApi.RegisterSerializerType(typeof(NeptuneEnchantment));
        this.ModApi.RegisterSerializerType(typeof(ObsidianEnchantment));
        this.ModApi.RegisterSerializerType(typeof(YetiEnchantment));

        Log.D("[CMBT]: Registered the SpaceCore integration.");
        return true;
    }
}
