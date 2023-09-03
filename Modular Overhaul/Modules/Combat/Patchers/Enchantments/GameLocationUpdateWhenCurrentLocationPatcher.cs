namespace DaLion.Overhaul.Modules.Combat.Patchers.Enchantments;

#region using directives

using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationUpdateWhenCurrentLocationPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationUpdateWhenCurrentLocationPatcher"/> class.</summary>
    internal GameLocationUpdateWhenCurrentLocationPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.UpdateWhenCurrentLocation));
    }

    #region harmony patches

    /// <summary>Update Wabbajack animals.</summary>
    [HarmonyPostfix]
    private static void GameLocationUpdateWhenCurrentLocationPostfix(GameLocation __instance, GameTime time)
    {
        if (Context.IsMainPlayer && Game1.shouldTimePass())
        {
            __instance.Get_Animals().ForEach(animal => animal.updateWhenCurrentLocation(time, __instance));
        }
    }

    #endregion harmony patches
}
