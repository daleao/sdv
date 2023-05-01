namespace DaLion.Overhaul.Modules.Rings.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Rings.Extensions;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingOnEquipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingOnEquipPatcher"/> class.</summary>
    internal CombinedRingOnEquipPatcher()
    {
        this.Target = this.RequireMethod<CombinedRing>(nameof(CombinedRing.onEquip));
    }

    #region harmony patches

    /// <summary>Add Infinity Band resonance effects.</summary>
    [HarmonyPostfix]
    private static void CombinedRingOnEquipPostfix(CombinedRing __instance, Farmer who)
    {
        var chord = __instance.Get_Chord();
        if (chord is null)
        {
            return;
        }

        chord.Apply(who.currentLocation, who);
        if (chord.Root is not null && who.CurrentTool is { } tool &&
            ((tool is MeleeWeapon && WeaponsModule.ShouldEnable) || (tool is Slingshot && SlingshotsModule.ShouldEnable)) &&
            tool.CanResonateWith(chord.Root))
        {
            tool.UpdateResonatingChord(chord);
        }
    }

    #endregion harmony patches
}
