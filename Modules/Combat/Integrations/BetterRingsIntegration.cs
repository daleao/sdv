namespace DaLion.Overhaul.Modules.Combat.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations;

#endregion using directives

[ModRequirement("BBR.BetterRings", "Better Rings")]
[ModConflict("Taiyo.VanillaTweaks")]
internal sealed class BetterRingsIntegration : ModIntegration<BetterRingsIntegration>
{
    /// <summary>Initializes a new instance of the <see cref="BetterRingsIntegration"/> class.</summary>
    internal BetterRingsIntegration()
        : base("BBR.BetterRings", "Better Rings", null, ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
        Log.D("[CMBT]: Registered the Better Rings integration.");
        return true;
    }
}
