namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemGrabMenuReceiveLeftClickPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ItemGrabMenuReceiveLeftClickPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ItemGrabMenuReceiveLeftClickPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = typeof(ItemGrabMenu).RequireMethod(nameof(ItemGrabMenu.receiveLeftClick));
    }

    #region harmony patches

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ItemGrabMenuReceiveLeftClickPostfix(ItemGrabMenu __instance, int x, int y, bool playSound)
    {
        if (State.MenuWrapper is { } wrapper && ReferenceEquals(__instance, wrapper.Menu))
        {
            wrapper.ReceiveLeftClick(x, y, playSound);
        }
    }

    #endregion harmony patches
}
