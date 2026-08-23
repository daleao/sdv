namespace DaLion.Professions.Framework.Integrations;

#region using directives

using DaLion.Professions.Framework.Events.Player.Warped;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="SveIntegration"/> class.</summary>
[ModRequirement(MOD_ID)]
[UsedImplicitly]
internal sealed class SveIntegration()
    : ModIntegration<SveIntegration>(ModHelper.ModRegistry)
{
    internal const string MOD_ID = "FlashShifter.StardewValleyExpandedCP";

    internal const string BIRCH_WATER_QID = MOD_ID + "_Birch_Water";

    internal const string FIR_WAX_QID = MOD_ID + "_Fir_Wax";

    /// <summary>Gets a value indicating whether the <c>DisableGaldoranTheme</c> config setting is enabled.</summary>
    internal bool DisabeGaldoranTheme => this.IsLoaded &&
        (ModHelper.ReadContentPackConfig(MOD_ID)?.Value<bool?>("DisableGaldoranTheme") ?? false);

    /// <summary>Gets a value indicating whether the <c>UseGaldoranThemeAllTimes</c> config setting is enabled.</summary>
    internal bool UseGaldoranThemeAllTimes => this.IsLoaded &&
        (ModHelper.ReadContentPackConfig(MOD_ID)?.Value<bool?>("UseGaldoranThemeAllTimes") ?? false);

    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        EventManager.Enable<SveWarpedEvent>();
        Log.D("Registered the Stardew Valley Expanded integration.");
        return true;
    }
}
