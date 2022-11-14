namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using DaLion.Ligo.Modules.Rings.Events;
using HarmonyLib;
using Shared.Harmony;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnMonsterSlayPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingOnMonsterSlayPatcher"/> class.</summary>
    internal RingOnMonsterSlayPatcher()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.onMonsterSlay));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Jinx up the Savage ring.</summary>
    [HarmonyPrefix]
    private static bool RingOnMonsterSlayPrefix(Ring __instance)
    {
        if (!ModEntry.Config.Rings.RebalancedRings)
        {
            return true; // run original logic
        }

        switch (__instance.ParentSheetIndex)
        {
            case Constants.WarriorRingIndex:
                ++ModEntry.State.Rings.WarriorKillCount;
                ModEntry.Events.Enable<WarriorUpdateTickedEvent>();
                break;
            case Constants.SavangeRingIndex:
                ModEntry.State.Rings.SavageExcitedness = 9;
                ModEntry.Events.Enable<SavageUpdateTickedEvent>();
                break;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
