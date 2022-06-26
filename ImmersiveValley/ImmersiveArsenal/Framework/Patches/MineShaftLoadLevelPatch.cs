namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewValley;
using StardewValley.Locations;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftLoadLevelPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MineShaftLoadLevelPatch()
    {
        Target = RequireMethod<MineShaft>(nameof(MineShaft.loadLevel));
    }

    #region harmony patches

    /// <summary>Create Qi Challenge reward level.</summary>
    [HarmonyPrefix]
    private static bool MineShaftLoadLevelPrefix(MineShaft __instance, ref NetBool ___netIsTreasureRoom, int level)
    {
        if (level != 170 || !Game1.player.hasQuest(ModEntry.QiChallengeFinalQuestId)) return true; // run original logic

        ___netIsTreasureRoom.Value = true;
        __instance.loadedMapNumber = 120;
        __instance.mapPath.Value = "Maps\\Mines\\10";
        __instance.updateMap();
        __instance.ApplyDiggableTileFixes();
        MineShaft.lowestLevelReached = Math.Max(MineShaft.lowestLevelReached, level);
        return false; // don't run original logic
    }

    #endregion harmony patches
}