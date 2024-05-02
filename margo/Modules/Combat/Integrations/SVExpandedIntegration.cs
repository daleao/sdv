namespace DaLion.Overhaul.Modules.Combat.Integrations;

#region using directives

using DaLion.Overhaul.Modules.Combat.Events.Player.Warped;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

[ModRequirement("FlashShifter.StardewValleyExpandedCP", "StardewValleyExpanded")]
internal sealed class SVExpandedIntegration : ModIntegration<SVExpandedIntegration>
{
    /// <summary>Initializes a new instance of the <see cref="SVExpandedIntegration"/> class.</summary>
    internal SVExpandedIntegration()
        : base(ModHelper.ModRegistry)
    {
    }

    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        EventManager.Enable<SveWarpedEvent>();
        Log.D("[CMBT]: Registered the Stardew Valley Expanded integration.");
        return true;
    }
}
