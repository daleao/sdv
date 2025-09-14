namespace DaLion.Core.Framework.Patchers;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Enchantments;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDamageMonsterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationDamageMonsterPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationDamageMonsterPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>(
            nameof(GameLocation.damageMonster),
            [
                typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
                typeof(float), typeof(float), typeof(bool), typeof(Farmer), typeof(bool),
            ]);
    }

    #region harmony patches

    /// <summary>Steadfast enchantment crit to damage conversion.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void GameLocationDamageMonsterPrefix(bool __result, Farmer who)
    {
        if (who.IsLocalPlayer && __result)
        {
            State.SecondsOutOfCombat = 0;
        }
    }

    /// <summary>Record knockback for damage and crit for defense ignore + back attacks.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? GameLocationDamageMonsterTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(BaseEnchantment).RequireMethod(nameof(BaseEnchantment.OnCalculateDamage))),
                    ],
                    ILHelper.SearchOption.First)
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_S, helper.Locals[9])], ILHelper.SearchOption.Previous)
                .Move()
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[8]),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GameLocationDamageMonsterPatcher).RequireMethod(nameof(ApplyBurnIfNecessary))),
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[8]),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Burn damage debuff.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected
    private static int ApplyBurnIfNecessary(Farmer? farmer, int damageAmount)
    {
        return (farmer?.IsBurning() ?? false) ? damageAmount / 2 : damageAmount;
    }

    #endregion injected
}
