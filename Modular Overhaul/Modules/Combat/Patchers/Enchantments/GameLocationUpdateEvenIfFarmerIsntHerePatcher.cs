namespace DaLion.Overhaul.Modules.Combat.Patchers.Enchantments;

#region using directives

using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationUpdateEvenIfFarmerIsntHerePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationUpdateEvenIfFarmerIsntHerePatcher"/> class.</summary>
    internal GameLocationUpdateEvenIfFarmerIsntHerePatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.updateEvenIfFarmerIsntHere));
    }

    #region harmony patches

    /// <summary>Update Wabbajack animals.</summary>
    [HarmonyPostfix]
    private static void GameLocationUpdateEvenIfFarmerIsntHerePostfix(GameLocation __instance, GameTime time)
    {
        if (Context.IsMainPlayer && Game1.shouldTimePass())
        {
            __instance.Get_Animals().ForEach(animal => animal.updateWhenNotCurrentLocation(null, time, __instance));
        }
    }

    #endregion harmony patches
}
