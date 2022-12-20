namespace DaLion.Overhaul.Modules.Tools.Integrations;

#region using directives

using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

[RequiresMod("spacechase0.MoonMisadventures", "Moon Misadventures")]
internal sealed class MoonMisadventuresIntegration : ModIntegration<MoonMisadventuresIntegration>
{
    private MoonMisadventuresIntegration()
        : base("spacechase0.MoonMisadventures", "Moon Misadventures", null, ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        if (ModEntry.Config.Tools.Validate())
        {
            return true;
        }

        GenericModConfigMenuForOverhaul.Instance!.Reload();
        return true;
    }
}
