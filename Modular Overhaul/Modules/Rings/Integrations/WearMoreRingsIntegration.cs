namespace DaLion.Overhaul.Modules.Rings.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.WearMoreRings;

#endregion using directives

[ModRequirement("bcmpinc.WearMoreRings", "Wear More Rings", "5.1")]
internal sealed class WearMoreRingsIntegration : ModIntegration<WearMoreRingsIntegration, IWearMoreRingsApi>
{
    /// <summary>Initializes a new instance of the <see cref="WearMoreRingsIntegration"/> class.</summary>
    internal WearMoreRingsIntegration()
        : base("bcmpinc.WearMoreRings", "Wear More Rings", "5.1", ModHelper.ModRegistry)
    {
    }
}
