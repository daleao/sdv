namespace DaLion.Chargeable.Framework.Patchers;

#region using directives

using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[HarmonyPatch(typeof(Pickaxe), nameof(Pickaxe.DoFunction))]
internal sealed class PickaxeDoFunctionPatcher
{
    /// <summary>Charged Pick empower.</summary>
    private static void Prefix(Pickaxe __instance, ref float __state, Farmer who)
    {
        var toolPower = who.toolPower.Value;
        if (toolPower <= 0)
        {
            return;
        }

        if (State.DoingShockwave)
        {
            __state = who.Stamina;
            return;
        }

        __instance.additionalPower.Value += __instance.UpgradeLevel * toolPower;
    }

    /// <summary>Charged Pick empower.</summary>
    private static void Postfix(Pickaxe __instance, float __state, Farmer who)
    {
        var toolPower = who.toolPower.Value;
        if (toolPower <= 0)
        {
            return;
        }

        if (State.DoingShockwave)
        {
            who.Stamina = __state;
            return;
        }

        __instance.additionalPower.Value -= __instance.UpgradeLevel * toolPower;
    }
}
