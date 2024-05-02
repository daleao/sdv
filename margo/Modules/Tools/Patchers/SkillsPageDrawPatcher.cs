namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class SkillsPageDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillsPageDrawPatcher"/> class.</summary>
    internal SkillsPageDrawPatcher()
    {
        this.Target = this.RequireMethod<SkillsPage>(nameof(SkillsPage.draw), new[] { typeof(SpriteBatch) });
    }

    /// <summary>Allows new Master Enchantments to draw as green levels in the skills page.</summary>
    private static IEnumerable<CodeInstruction>? SkillsPageDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Tool).RequireMethod(nameof(Tool.hasEnchantmentOfType))
                                .MakeGenericMethod(typeof(MasterEnchantment))),
                    })
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                    },
                    ILHelper.SearchOption.Previous,
                    2)
                .CountUntil(new[] { new CodeInstruction(OpCodes.Br_S) }, out var count)
                .Remove(count);
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing Master enchantment green font.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }
}
