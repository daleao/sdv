namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using DaLion.Ligo.Modules.Rings.Extensions;
using DaLion.Ligo.Modules.Rings.VirtualProperties;
using HarmonyLib;
using Shared.Harmony;
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
