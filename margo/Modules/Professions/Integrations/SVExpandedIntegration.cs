namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.Player.Warped;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations;

#endregion using directives

[ModRequirement("FlashShifter.StardewValleyExpandedCP", "Stardew Valley Expanded")]
internal sealed class SVExpandedIntegration : ModIntegration<SVExpandedIntegration>
{
    /// <summary>Initializes a new instance of the <see cref="SVExpandedIntegration"/> class.</summary>
    internal SVExpandedIntegration()
        : base(ModHelper.ModRegistry)
    {
    }

    /// <summary>Gets a value indicating whether the <c>DisableGaldoranTheme</c> config setting is enabled.</summary>
    internal bool DisabeGaldoranTheme => this.IsLoaded && ModHelper
        .ReadContentPackConfig("FlashShifter.StardewValleyExpandedCP")
        ?.Value<bool?>("DisableGaldoranTheme") == true;

    /// <summary>Gets a value indicating whether the <c>UseGaldoranThemeAllTimes</c> config setting is enabled.</summary>
    internal bool UseGaldoranThemeAllTimes => this.IsLoaded && ModHelper
        .ReadContentPackConfig("FlashShifter.StardewValleyExpandedCP")
        ?.Value<bool?>("UseGaldoranThemeAllTimes") == true;

    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        EventManager.Enable<SveWarpedEvent>();
        Log.D("[PRFS]: Registered the Stardew Valley Expanded integration.");
        return true;
    }
}
