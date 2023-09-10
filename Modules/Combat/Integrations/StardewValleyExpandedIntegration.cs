namespace DaLion.Overhaul.Modules.Combat.Integrations;

#region using directives

using DaLion.Overhaul.Modules.Combat.Events.Player;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

[ModRequirement("FlashShifter.StardewValleyExpandedCP", "StardewValleyExpanded")]
internal sealed class StardewValleyExpandedIntegration : ModIntegration<StardewValleyExpandedIntegration>
{
    /// <summary>Initializes a new instance of the <see cref="StardewValleyExpandedIntegration"/> class.</summary>
    internal StardewValleyExpandedIntegration()
        : base(
            "FlashShifter.StardewValleyExpandedCP",
            "StardewValleyExpanded",
            null,
            ModHelper.ModRegistry)
    {
    }

    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        EventManager.Enable<SveWarpedEvent>();
        return true;
    }
}
