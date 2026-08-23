namespace DaLion.Professions.Framework.Patchers.Farming;

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
internal sealed class AnimalPageDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AnimalPageDrawPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal AnimalPageDrawPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = typeof(AnimalPage).RequireMethod(nameof(AnimalPage.draw), [typeof(SpriteBatch)]);
    }

    #region harmony patches

    /// <summary>Patch to draw another vertical divider in Animal Page menu.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? AnimalPageDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Call, typeof(IClickableMenu).RequireMethod("drawVerticalPartition"))
                    ],
                    ILHelper.SearchOption.Last)
                .Move()
                .Insert([
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Ldloc_3),
                        new CodeInstruction(OpCodes.Call, typeof(AnimalPageDrawPatcher).RequireMethod(nameof(DrawAnotherVerticalPartition)))
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding crop feed to Animal Page menu.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static void DrawAnotherVerticalPartition(AnimalPage page, SpriteBatch b, int heightOverride)
    {
        if (!Game1.player.HasProfession(Profession.Rancher))
        {
            return;
        }

        var drawVerticalPartition = Reflector.GetUnboundMethodDelegate<Action<IClickableMenu, SpriteBatch, int, bool, int, int, int, int>>(page, "drawVerticalPartition");
        drawVerticalPartition(page, b, page.xPositionOnScreen + 644 + 84, true, -1, -1, -1, heightOverride);
    }

    #endregion injected
}
