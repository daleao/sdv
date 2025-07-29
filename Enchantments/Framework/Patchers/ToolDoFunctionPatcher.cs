namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolDoFunctionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolDoFunctionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ToolDoFunctionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.DoFunction));
    }

    #region harmony patches

    /// <summary>Apply Sharp enchantment to Pickaxe.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? ToolDoFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var doNormalDamage = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(GameLocation).RequireMethod(
                            nameof(GameLocation.damageMonster),
                            [typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(Farmer), typeof(bool)])),
                ])
                .PatternMatch(
                    [new CodeInstruction(OpCodes.Ldarg_1)],
                    ILHelper.SearchOption.Previous)
                .AddLabels(doNormalDamage)
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Tool).RequireMethod(nameof(Tool.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(SharpEnchantment))),
                    new CodeInstruction(OpCodes.Brfalse_S, doNormalDamage),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)5),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ToolDoFunctionPatcher).RequireMethod(nameof(DamageWithSharpPickaxe))),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                ])
                .PatternMatch([new CodeInstruction(OpCodes.Pop)])
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Sharp Pickaxe damage.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static void DamageWithSharpPickaxe(Tool tool, GameLocation location, int x, int y, Farmer who)
    {
        if (tool is Pickaxe && MeleeWeapon.TryGetData(QIDs.GalaxyHammer, out var galaxyHammerData))
        {
            location.damageMonster(
                new Rectangle(x - 32, y - 32, 64, 64),
                galaxyHammerData.MinDamage,
                galaxyHammerData.MaxDamage,
                isBomb: false,
                who);
        }
    }

    #endregion injected
}
