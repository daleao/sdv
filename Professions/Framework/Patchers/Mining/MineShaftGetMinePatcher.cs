namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftGetMinePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MineShaftGetMinePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MineShaftGetMinePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MineShaft>(nameof(MineShaft.GetMine));
    }

    #region harmony patches

    /// <summary>Patch for Spelunker Flag checkpoint.</summary>
    [HarmonyPrefix]
    private static bool MineShaftGetMinePrefix(ref MineShaft __result, string name)
    {
        if (State.SpelunkerFlag?.Location is MineShaft shaft && name == shaft.Name)
        {
            if (!MineShaft.activeMines.Contains(shaft))
            {
                MineShaft.activeMines.Add(shaft);
            }

            __result = shaft;
            return false; // don't run original logic
        }

        return true; // run original logic
    }

    #endregion harmony patches
}
