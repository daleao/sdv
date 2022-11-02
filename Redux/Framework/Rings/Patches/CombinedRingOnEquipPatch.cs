namespace DaLion.Redux.Framework.Rings.Patches;

#region using directives

using DaLion.Redux.Framework.Rings.Extensions;
using DaLion.Redux.Framework.Rings.VirtualProperties;
using HarmonyLib;
using StardewValley.Objects;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class CombinedRingOnEquipPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="CombinedRingOnEquipPatch"/> class.</summary>
    internal CombinedRingOnEquipPatch()
    {
        this.Target = this.RequireMethod<CombinedRing>(nameof(CombinedRing.onEquip));
    }

    #region harmony patches

    /// <summary>Add Infinity Band resonance effects.</summary>
    [HarmonyPostfix]
    private static void CombinedRingOnEquipPostfix(CombinedRing __instance, Farmer who)
    {
        __instance.Get_Chord()?.Apply(who.currentLocation, who);
        if (!ModEntry.Config.EnableArsenal || who.CurrentTool is not { } tool)
        {
            return;
        }

        switch (tool)
        {
            case MeleeWeapon weapon:
                weapon.RecalculateResonances();
                break;
            case Slingshot slingshot:
                slingshot.RecalculateResonances();
                break;
            default:
                return;
        }
    }

    #endregion harmony patches
}
