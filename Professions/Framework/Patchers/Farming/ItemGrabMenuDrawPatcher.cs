namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemGrabMenuDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ItemGrabMenuDrawPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ItemGrabMenuDrawPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = typeof(ItemGrabMenu).RequireMethod(nameof(ItemGrabMenu.draw), [typeof(SpriteBatch)]);
    }

    #region harmony patches

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ItemGrabMenuDrawPostfix(ItemGrabMenu __instance, SpriteBatch b)
    {
        if (State.MenuWrapper is { } wrapper && ReferenceEquals(__instance, wrapper.Menu))
        {
            wrapper.Draw(b);
        }
    }

    #endregion harmony patches
}
