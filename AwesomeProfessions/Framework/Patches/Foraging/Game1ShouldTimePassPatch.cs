using System;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using DaLion.Stardew.Common.Harmony;
using DaLion.Stardew.Professions.Framework.Extensions;

namespace DaLion.Stardew.Professions.Framework.Patches.Foraging;

[UsedImplicitly]
internal class Game1ShouldTimePassPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal Game1ShouldTimePassPatch()
    {
        Original = RequireMethod<Game1>(nameof(Game1.shouldTimePass));
        ReversePatch = new(GetType().MethodNamed(nameof(Game1ShouldTimePassOriginal)));
        ++HarmonyPatcher.TotalReversePatchCount;
    }

    #region harmony patches

    /// <summary>Patch to freeze time during prestiged treasure hunts.</summary>
    [HarmonyPrefix]
    private static bool Game1ShouldTimePassPrefix(ref bool __result)
    {
        if ((ModEntry.State.Value.ProspectorHunt is null || !ModEntry.State.Value.ProspectorHunt.IsActive ||
             !Game1.player.HasPrestigedProfession("Prospector")) &&
            (ModEntry.State.Value.ScavengerHunt is null || !ModEntry.State.Value.ScavengerHunt.IsActive ||
             !Game1.player.HasPrestigedProfession("Scavenger"))) return true; // run original logic

        __result = false;
        return false; // don't run original logic
    }

    /// <summary>Reverse patch to store a copy of the original method for exclusive use by Treasure Hunts.</summary>
    [HarmonyReversePatch]
    public static bool Game1ShouldTimePassOriginal(Game1 instance, bool ignore_multiplayer = false)
    {
        // it's a stub so it has no initial content
        throw new NotImplementedException("It's a stub!");
    }

    #endregion harmony patches
}