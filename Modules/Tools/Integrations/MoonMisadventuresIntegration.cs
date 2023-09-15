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

        GenericModConfigMenu.Instance?.Reload();
        Log.D("[TOLS]: Registered the Moon Misadventures integration.");
        return true;
    }
}
