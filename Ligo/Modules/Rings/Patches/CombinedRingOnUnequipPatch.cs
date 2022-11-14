namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using DaLion.Ligo.Modules.Rings.Extensions;
using DaLion.Ligo.Modules.Rings.VirtualProperties;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingOnUnequipPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingOnUnequipPatch"/> class.</summary>
    internal CombinedRingOnUnequipPatch()
    {
        this.Target = this.RequireMethod<CombinedRing>(nameof(CombinedRing.onUnequip));
    }

    #region harmony patches

    /// <summary>Remove Infinity Band resonance effects.</summary>
    [HarmonyPostfix]
    private static void CombinedRingOnUnequipPostfix(CombinedRing __instance, Farmer who)
    {
        __instance.Get_Chord()?.Unapply(who.currentLocation, who);
        if (!ModEntry.Config.EnableArsenal || who.CurrentTool is not { } tool)
        {
            return;
        }

        switch (tool)
        {
            case MeleeWeapon weapon:
                weapon.RemoveResonances();
                break;
            case Slingshot slingshot:
                slingshot.RemoveResonances();
                break;
            default:
                return;
        }
    }

    #endregion harmony patches
}
