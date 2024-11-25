﻿namespace DaLion.Harmonics.Framework.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

[ModRequirement("bcmpinc.WearMoreRings", "Wear More Rings", "5.1")]
internal sealed class WearMoreRingsIntegration : ModIntegration<WearMoreRingsIntegration, IWearMoreRingsApi>
{
    /// <summary>Initializes a new instance of the <see cref="WearMoreRingsIntegration"/> class.</summary>
    internal WearMoreRingsIntegration()
        : base(ModHelper.ModRegistry)
    {
    }
}
