namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Combat.Resonance;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerShiftToolbarPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerShiftToolbarPatcher"/> class.</summary>
    internal FarmerShiftToolbarPatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.shiftToolbar));
    }

    #region harmony patches

    /// <summary>Trigger playback of gemstone vibration.</summary>
    [HarmonyPostfix]
    private static void FarmerShiftToolbarPostfix(Farmer __instance)
    {
        if (!CombatModule.Config.RingsEnchantments.AudibleGemstones)
        {
            return;
        }

        EventManager.Disable<HoldingGemstoneUpdateTickedEvent>();
        if (__instance.CurrentItem is not { } held || !Gemstone.TryFromObject(held.ParentSheetIndex, out var gemstone))
        {
            return;
        }

        HoldingGemstoneUpdateTickedEvent.Gemstone = gemstone;
        EventManager.Enable<HoldingGemstoneUpdateTickedEvent>();
    }

    #endregion harmony patches
}
