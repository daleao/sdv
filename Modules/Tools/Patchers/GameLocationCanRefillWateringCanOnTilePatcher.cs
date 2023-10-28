namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationCanRefillWateringCanOnTilePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationCanRefillWateringCanOnTilePatcher"/> class.</summary>
    internal GameLocationCanRefillWateringCanOnTilePatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.CanRefillWateringCanOnTile));
    }

    #region harmony patches

    private static void GameLocationCanRefillWateringCanOnTilePostfix(
        GameLocation __instance,
        ref bool __result,
        int tileX,
        int tileY)
    {
        if (__result && (__instance is Beach || __instance.catchOceanCrabPotFishFromThisSpot(tileX, tileY)) &&
            ToolsModule.Config.Can.PreventSaltWaterRefill)
        {
            __result = false;
        }
    }

    #endregion harmony patches
}
