namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;
using StardewValley.Menus;

using Common;
using Common.Extensions;
using Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class PondQueryMenuCtorPatch : BasePatch
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
            fish_pond.ReadData("FishQualities", null)?.ParseTuple<int, int, int, int>();
        }
        catch (InvalidOperationException ex)
        {
            Log.W($"FishQualities data is invalid. {ex}\nThe data will be reset");
            fish_pond.WriteData("FishQualities", $"{fish_pond.FishCount},0,0,0");
            fish_pond.WriteData("FamilyQualities", null);
            fish_pond.WriteData("FamilyLivingHere", null);
        }

        try
        {
            fish_pond.ReadData("FamilyQualities", null)?.ParseTuple<int, int, int, int>();
        }
        catch (InvalidOperationException ex)
        {
            Log.W($"FamilyQuality data is invalid. {ex}\nThe data will be reset");
            fish_pond.WriteData("FishQualities", $"{fish_pond.FishCount},0,0,0");
            fish_pond.WriteData("FamilyQualities", null);
            fish_pond.WriteData("FamilyLivingHere", null);
        }
    }

    #endregion harmony patches
}