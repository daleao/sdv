namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemGrabMenuCleanupBeforeExitPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ItemGrabMenuCleanupBeforeExitPatcher "/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ItemGrabMenuCleanupBeforeExitPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = typeof(ItemGrabMenu).RequireMethod("cleanupBeforeExit");
    }

    #region harmony patches

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ItemGrabMenuCleanupBeforeExitPostfix(ItemGrabMenu __instance)
    {
        if (State.MenuWrapper is { } wrapper && ReferenceEquals(__instance, wrapper.Menu))
        {
            State.MenuWrapper = null;
        }
    }

    #endregion harmony patches
}
