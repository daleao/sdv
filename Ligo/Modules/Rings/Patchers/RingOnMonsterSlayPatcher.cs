namespace DaLion.Ligo.Modules.Rings.Patchers;

#region using directives

using DaLion.Ligo.Modules.Rings.Events;
using DaLion.Ligo.Modules.Rings.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
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

    /// <summary>Jinx up the Savage and Warrior ring.</summary>
    [HarmonyPrefix]
    private static bool RingOnMonsterSlayPrefix(Ring __instance, Farmer who)
    {
        if (!who.IsLocalPlayer || !ModEntry.Config.Rings.RebalancedRings)
        {
            return true; // run original logic
        }

        switch (__instance.ParentSheetIndex)
        {
            case Constants.WarriorRingIndex:
                who.Increment_WarriorKillCount();
                ModEntry.Events.Enable<WarriorUpdateTickedEvent>();
                break;
            case Constants.SavangeRingIndex:
                who.Set_SavageExcitedness(9);
                ModEntry.Events.Enable<SavageUpdateTickedEvent>();
                break;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
