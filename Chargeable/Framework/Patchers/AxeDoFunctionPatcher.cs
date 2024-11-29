namespace DaLion.Chargeable.Framework.Patchers;

#region using directives

using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[HarmonyPatch(typeof(Axe), nameof(Axe.DoFunction))]
internal sealed class AxeDoFunctionPatcher
{
    /// <summary>Charged Axe empower.</summary>
    private static void Prefix(Axe __instance, ref float __state, Farmer who)
    {
        if (State.DoingShockwave)
        {
            __state = who.Stamina;
            return;
        }

        __instance.additionalPower.Value += who.toolPower.Value * 2;
    }

    /// <summary>Charged Axe empower.</summary>
    private static void Postfix(Axe __instance, float __state, Farmer who)
    {
        if (State.DoingShockwave)
        {
            who.Stamina = __state;
            return;
        }

        __instance.additionalPower.Value -= who.toolPower.Value * 2;
        //who.Stamina -= (int)Math.Pow(Math.Max((2 * (toolPower + 1)) - (who.ForagingLevel * 0.1f), 0.1f), 2d) * Config.Pick.StaminaCostMultiplier;
    }
}
