namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.TimeChanged;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class MineCreateLadderDownPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MineCreateLadderDownPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MineCreateLadderDownPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MineShaft>(nameof(MineShaft.createLadderDown));
    }

    #region harmony patches

    /// <summary>Patch for Spelunker Flag checkpoint.</summary>
    [HarmonyPrefix]
    private static void MineShaftCreateLadderDownPostfix()
    {
        if (State.SpelunkerLadderStreak > 0)
        {
            EventManager.Enable<SpelunkerTimeChangedEvent>();
        }
    }

    #endregion harmony patches
}
