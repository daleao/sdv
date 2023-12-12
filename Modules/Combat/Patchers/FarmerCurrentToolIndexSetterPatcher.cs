namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Combat.Resonance;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCurrentToolIndexSetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCurrentToolIndexSetterPatcher"/> class.</summary>
    internal FarmerCurrentToolIndexSetterPatcher()
    {
        this.Target = this.RequirePropertySetter<Farmer>(nameof(Farmer.CurrentToolIndex));
    }

    #region harmony patches

    /// <summary>Trigger playback of gemstone vibration.</summary>
    [HarmonyPostfix]
    private static void FarmerCurrentToolIndexPostfix(Farmer __instance, ref int value)
    {
        if (!CombatModule.Config.AudibleGemstones)
        {
            return;
        }

        EventManager.Disable<HoldingGemstoneUpdateTickedEvent>();
        if (value < 0 || value >= __instance.Items.Count || __instance.Items[value] is not Item held ||
            !Gemstone.TryFromObject(held.ParentSheetIndex, out var gemstone))
        {
            return;
        }

        HoldingGemstoneUpdateTickedEvent.Gemstone = gemstone;
        EventManager.Enable<HoldingGemstoneUpdateTickedEvent>();
    }

    #endregion harmony patches
}
