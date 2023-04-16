namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Enchantments.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationIsTileOccupiedForPlacementPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationIsTileOccupiedForPlacementPatcher"/> class.</summary>
    internal GameLocationIsTileOccupiedForPlacementPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.isTileOccupiedForPlacement));
    }

    #region harmony patches

    /// <summary>Collide with Wabbajack animals.</summary>
    [HarmonyPostfix]
    private static void GameLocationIsTileOccupiedForPlacementPostfix(GameLocation __instance, ref bool __result, Vector2 tileLocation)
    {
        if (__result)
        {
            return;
        }

        foreach (var animal in __instance.Get_Animals())
        {
            if (!animal.getTileLocation().Equals(tileLocation))
            {
                continue;
            }

            __result = true;
            break;
        }
    }

    #endregion harmony patches
}
