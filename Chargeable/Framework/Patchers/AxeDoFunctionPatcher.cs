namespace DaLion.Chargeable.Framework.Patchers;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[HarmonyPatch(typeof(Axe), nameof(Axe.DoFunction))]
internal sealed class AxeDoFunctionPatcher
{
    #region harmony patches

    /// <summary>Charged Axe empower.</summary>
    private static void Prefix(Axe __instance, ref float __state, Farmer who)
    {
        var toolPower = who.toolPower.Value;
        if (toolPower > 0)
        {
            __instance.additionalPower.Value += __instance.UpgradeLevel * toolPower;
        }
    }

    /// <summary>Charged Axe empower.</summary>
    private static void Postfix(Axe __instance, float __state, Farmer who)
    {
        var toolPower = who.toolPower.Value;
        if (toolPower > 0)
        {
            __instance.additionalPower.Value -= __instance.UpgradeLevel * toolPower;
        }
    }

    private static IEnumerable<CodeInstruction>? Transpiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: power = who.toolPower.Value;
        try
        {
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Brtrue_S)])
                .Move()
                .RemoveUntil([
                    new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertySetter(nameof(Farmer.Stamina)))
                ])
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)5), // Farmer who
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // int power
                    new CodeInstruction(OpCodes.Call, typeof(AxeDoFunctionPatcher).RequireMethod(nameof(ConsumeStamina)))
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting custom stamina cost.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    private static void ConsumeStamina(Farmer who, int power)
    {
        if (State.ShockwaveHitting)
        {
            return;
        }

        var toolPower = who.toolPower.Value;
        if (toolPower <= 0)
        {
            who.Stamina -= (2f * power) - (who.ForagingLevel * 0.1f);
        }
        else
        {
            who.Stamina -= (2f * (toolPower + 1)) - (who.ForagingLevel * 0.1f);
        }
    }
}
