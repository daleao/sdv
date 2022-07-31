﻿namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common;
using Common.Extensions;
using Common.Extensions.Stardew;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class PondQueryMenuCtorPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal PondQueryMenuCtorPatch()
    {
        Target = RequireConstructor<PondQueryMenu>(typeof(FishPond));
    }

    #region harmony patches

    /// <summary>Handle invalid data on menu open.</summary>
    [HarmonyPrefix]
    private static void PondQueryMenuCtorPrefix(FishPond fish_pond)
    {
        try
        {
            fish_pond.Read("FishQualities").ParseTuple<int, int, int, int>();
        }
        catch (InvalidOperationException ex)
        {
            Log.W($"FishQualities data is invalid. {ex}\nThe data will be reset");
            fish_pond.Write("FishQualities", $"{fish_pond.FishCount},0,0,0");
            fish_pond.Write("FamilyQualities", null);
            fish_pond.Write("FamilyLivingHere", null);
        }

        try
        {
            fish_pond.Read("FamilyQualities").ParseTuple<int, int, int, int>();
        }
        catch (InvalidOperationException ex)
        {
            Log.W($"FamilyQuality data is invalid. {ex}\nThe data will be reset");
            fish_pond.Write("FishQualities", $"{fish_pond.FishCount},0,0,0");
            fish_pond.Write("FamilyQualities", null);
            fish_pond.Write("FamilyLivingHere", null);
        }
    }

    #endregion harmony patches
}