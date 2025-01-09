namespace DaLion.Chargeable.Framework.Patchers;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[HarmonyPatch(typeof(Pickaxe), nameof(Pickaxe.DoFunction))]
internal sealed class PickaxeDoFunctionPatcher
{
    #region harmony patches

    /// <summary>Charged Pick empower.</summary>
    private static void Prefix(Pickaxe __instance, ref float __state, Farmer who)
    {
        var toolPower = who.toolPower.Value;
        if (toolPower > 0)
        {
            __instance.additionalPower.Value += __instance.UpgradeLevel * toolPower;
        }
    }

    /// <summary>Charged Pick empower.</summary>
    private static void Postfix(Pickaxe __instance, float __state, Farmer who)
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
                    new CodeInstruction(OpCodes.Call, typeof(PickaxeDoFunctionPatcher).RequireMethod(nameof(ConsumeStamina)))
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

    private static void ConsumeStamina(Farmer who)
    {
        if (!State.ShockwaveHitting)
        {
            who.Stamina -= (2f * (who.toolPower.Value + 1)) - (who.MiningLevel * 0.1f);
        }
    }
}
