﻿namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class SlimeHutchUpdateWhenCurrentLocationPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlimeHutchUpdateWhenCurrentLocationPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal SlimeHutchUpdateWhenCurrentLocationPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SlimeHutch>(nameof(SlimeHutch.UpdateWhenCurrentLocation));
    }

    #region harmony patches

    /// <summary>Patch to increase Prestiged Piper Hutch capacity.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool SlimeHutchUpdateWhenCurrentLocationPostfix(SlimeHutch __instance, GameTime time)
    {
        if (!__instance.ParentBuilding.GetOwner().HasProfessionOrLax(Profession.Piper, true))
        {
            return true; // run original logic
        }

        try
        {
            GameLocationUpdateWhenCurrentLocationPatcher.GameLocationUpdateWhenCurrentLocationReverse(
                __instance,
                time);
            for (var i = 0; i < __instance.waterSpots.Length; i++)
            {
                __instance.setMapTile(16, 5 + i, __instance.waterSpots[i] ? 2135 : 2134, "Buildings", "untitled tile sheet");
            }

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
