namespace DaLion.Overhaul.Modules.Weapons.Integrations;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Enchantments;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.SpaceCore;

#endregion using directives

[RequiresMod("spacechase0.SpaceCore", "SpaceCore", "1.12.0")]
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

        return true;
    }
}
