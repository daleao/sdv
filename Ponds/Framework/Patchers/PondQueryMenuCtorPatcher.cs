namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class PondQueryMenuCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PondQueryMenuCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal PondQueryMenuCtorPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireConstructor<PondQueryMenu>(typeof(FishPond));
    }

    #region harmony patches

    /// <summary>Handle invalid data on menu open.</summary>
    [HarmonyPrefix]
    private static void PondQueryMenuCtorPrefix(FishPond fish_pond)
    {
        try
        {
            Data.Read(fish_pond, DataKeys.FishQualities).ParseTuple<int, int, int, int>();
        }
        catch (InvalidOperationException ex)
        {
            Log.W($"FishQualities data is invalid. {ex}\nThe data will be reset");
            Data.Write(fish_pond, DataKeys.FishQualities, $"{fish_pond.FishCount},0,0,0");
            Data.Write(fish_pond, DataKeys.FamilyQualities, null);
        }

        try
        {
            Data.Read(fish_pond, DataKeys.FamilyQualities).ParseTuple<int, int, int, int>();
        }
        catch (InvalidOperationException ex)
        {
            Log.W($"FamilyQuality data is invalid. {ex}\nThe data will be reset");
            Data.Write(fish_pond, DataKeys.FishQualities, $"{fish_pond.FishCount},0,0,0");
            Data.Write(fish_pond, DataKeys.FamilyQualities, null);
        }
    }

    #endregion harmony patches
}
