namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using Events;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnMonsterSlayPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal RingOnMonsterSlayPatch()
    {
        Target = RequireMethod<Ring>(nameof(Ring.onMonsterSlay));
        Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Jinx up the Savage ring.</summary>
    [HarmonyPrefix]
    private static bool RingOnMonsterSlayPrefix(Ring __instance, Farmer who)
    {
        if (__instance.ParentSheetIndex != 523 || !ModEntry.Config.RebalancedRings) return true; // run original logic

        ModEntry.SavageExcitedness.Value = 9;
        ModEntry.Manager.Enable<SavageUpdateTickedEvent>();

        return false; // don't run original logic
    }

    #endregion harmony patches
}