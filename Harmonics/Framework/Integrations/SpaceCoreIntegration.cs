namespace DaLion.Harmonics.Framework.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using SpaceShared.APIs;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="SpaceCoreIntegration"/> class.</summary>
[ModRequirement("spacechase0.SpaceCore", "SpaceCore")]
internal sealed class SpaceCoreIntegration()
    : ModIntegration<SpaceCoreIntegration, ISpaceCoreApi>(ModHelper.ModRegistry)
{
    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        this.AssertLoaded();
        this.ModApi.RegisterSerializerType(typeof(GarnetEnchantment));
        Log.D("Registered the SpaceCore integration.");
        return true;
    }
}
