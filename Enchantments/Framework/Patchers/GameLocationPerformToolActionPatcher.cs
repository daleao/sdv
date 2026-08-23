namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using DaLion.Enchantments.Framework.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformToolActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationPerformToolActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationPerformToolActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.performToolAction));
    }

    #region harmony patches

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void UseToolPostfix(GameLocation __instance, Tool t, int tileX, int tileY)
    {
        if (__instance is not MineShaft shaft || t is not Pickaxe ||
            !shaft.IsExcavatable(tileX, tileY))
        {
            return;
        }

        __instance.playSound("hammer");
        var key = (__instance.NameOrUniqueName, tileX, tileY);
        State.MineShaftWallHits[key]++;
        if (State.MineShaftWallHits[key] == 5)
        {
            __instance.playSound("stoneCrack");
        }
        else if (State.MineShaftWallHits[key] == 10)
        {
            shaft.Excavate(tileX, tileY);
        }
    }

    #endregion harmony patches
}
