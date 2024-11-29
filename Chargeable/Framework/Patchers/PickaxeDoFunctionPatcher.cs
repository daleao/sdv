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
        if (State.DoingShockwave)
        {
            __state = who.Stamina;
            return;
        }

        __instance.additionalPower.Value += who.toolPower.Value * 2;
    }

    /// <summary>Charged Pick empower.</summary>
    private static void Postfix(Pickaxe __instance, float __state, Farmer who)
    {
        if (State.DoingShockwave)
        {
            who.Stamina = __state;
            return;
        }

        __instance.additionalPower.Value -= who.toolPower.Value * 2;
        //who.Stamina -= (int)Math.Pow(Math.Max((2 * (toolPower + 1)) - (who.MiningLevel * 0.1f), 0.1f), 2d) * Config.Pick.StaminaCostMultiplier;
    }
}
