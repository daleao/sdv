namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using DaLion.Stardew.Rings.Framework.Events;
using HarmonyLib;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnMonsterSlayPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="RingOnMonsterSlayPatch"/> class.</summary>
    internal RingOnMonsterSlayPatch()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.onMonsterSlay));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Jinx up the Savage ring.</summary>
    [HarmonyPrefix]
    private static bool RingOnMonsterSlayPrefix(Ring __instance, Farmer who)
    {
        if (__instance.ParentSheetIndex != 523 || !ModEntry.Config.RebalancedRings)
        {
            return true; // run original logic
        }

        ModEntry.State.SavageExcitedness = 9;
        ModEntry.Events.Enable<SavageUpdateTickedEvent>();

        return false; // don't run original logic
    }

    #endregion harmony patches
}
