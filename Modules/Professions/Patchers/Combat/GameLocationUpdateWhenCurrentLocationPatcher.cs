namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using DaLion.Overhaul.Modules.Professions.VirtualProperties;
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

    /// <summary>Patch to run Musk update.</summary>
    [HarmonyPostfix]
    private static void GameLocationUpdateWhenCurrentLocationPostfix(GameLocation __instance, GameTime time)
    {
        if (time.TotalGameTime.Ticks % 60 == 0)
        {
            __instance.Get_Musks().ForEach(musk => musk.Update());
        }
    }

    #endregion harmony patches
}
