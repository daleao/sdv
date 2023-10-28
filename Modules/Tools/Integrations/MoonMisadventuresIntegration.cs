namespace DaLion.Overhaul.Modules.Tools.Integrations;

#region using directives

using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

[ModRequirement("spacechase0.MoonMisadventures", "Moon Misadventures")]
internal sealed class MoonMisadventuresIntegration : ModIntegration<MoonMisadventuresIntegration>
{
    /// <summary>Initializes a new instance of the <see cref="MoonMisadventuresIntegration"/> class.</summary>
    internal MoonMisadventuresIntegration()
        : base(ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        ToolsModule.Config.Axe.RadiusAtEachPowerLevel = new uint[] { 1, 2, 3, 4, 5, 6, 7 };
        ToolsModule.Config.Pick.RadiusAtEachPowerLevel = new uint[] { 1, 2, 3, 4, 5, 6, 7 };
        ToolsModule.Config.Hoe.AffectedTilesAtEachPowerLevel =
            new (uint Length, uint Radius)[] { (3, 0), (5, 0), (3, 1), (6, 1), (7, 2), (8, 3), (9, 4) };
        ToolsModule.Config.Can.AffectedTilesAtEachPowerLevel =
            new (uint Length, uint Radius)[] { (3, 0), (5, 0), (3, 1), (6, 1), (7, 2), (8, 3), (9, 4) };
        GenericModConfigMenu.Instance?.Reload();
        Log.D("[TOLS]: Registered the Moon Misadventures integration.");
        return true;
    }
}
