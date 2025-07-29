namespace DaLion.Professions.Framework.Integrations;

#region using directives

using DaLion.Professions.Framework.Events.Player.Warped;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="SveIntegration"/> class.</summary>
[ModRequirement("FlashShifter.StardewValleyExpandedCP")]
[UsedImplicitly]
internal sealed class SveIntegration()
    : ModIntegration<SveIntegration>(ModHelper.ModRegistry)
{
    /// <summary>Gets a value indicating whether the <c>DisableGaldoranTheme</c> config setting is enabled.</summary>
    internal bool DisabeGaldoranTheme => this.IsLoaded &&
        (ModHelper.ReadContentPackConfig("FlashShifter.StardewValleyExpandedCP")?.Value<bool?>("DisableGaldoranTheme") ?? false);

    /// <summary>Gets a value indicating whether the <c>UseGaldoranThemeAllTimes</c> config setting is enabled.</summary>
    internal bool UseGaldoranThemeAllTimes => this.IsLoaded &&
        (ModHelper.ReadContentPackConfig("FlashShifter.StardewValleyExpandedCP")?.Value<bool?>("UseGaldoranThemeAllTimes") ?? false);

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
