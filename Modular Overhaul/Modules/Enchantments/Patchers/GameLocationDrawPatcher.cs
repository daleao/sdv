namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Enchantments.VirtualProperties;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationDrawPatcher"/> class.</summary>
    internal GameLocationDrawPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.draw));
    }

    #region harmony patches

    /// <summary>Draw Wabbajack animals.</summary>
    [HarmonyPostfix]
    private static void GameLocationDrawPostfix(GameLocation __instance, SpriteBatch b)
    {
        __instance.Get_Animals().ForEach(animal => animal.draw(b));
    }

    #endregion harmony patches
}
