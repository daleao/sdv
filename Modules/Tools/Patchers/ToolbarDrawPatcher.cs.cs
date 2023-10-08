namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Extensions.Collections;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolbarDrawPatcher : HarmonyPatcher
{
    private static readonly Lazy<Texture2D> Pixel = new(() =>
    {
        var pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        pixel.SetData(new[] { Color.White });
        return pixel;
    });

    /// <summary>Initializes a new instance of the <see cref="ToolbarDrawPatcher"/> class.</summary>
    internal ToolbarDrawPatcher()
    {
        this.Target = this.RequireMethod<Toolbar>(nameof(Toolbar.draw), new[] { typeof(SpriteBatch) });
    }

    #region harmony patches

    /// <summary>Draw selectable indicator.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ToolbarDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Options).RequireField(nameof(Options.gamepadControls))),
                    })
                .Move(-1)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[7]), new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(Toolbar).RequireField("buttons")),
                        new CodeInstruction(OpCodes.Ldarg_1), new CodeInstruction(
                            OpCodes.Call,
                            typeof(ToolbarDrawPatcher).RequireMethod(nameof(DrawSelector))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed drawing tool selector in toolbar.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DrawSelector(int j, List<ClickableComponent> buttons, SpriteBatch b)
    {
        if (ToolsModule.State.SelectableToolByType.Count == 0)
        {
            return;
        }

        if (Game1.player.Items[j] is Tool tool && Game1.player.CurrentTool != tool &&
            ToolsModule.State.SelectableToolByType.TryGetValue(tool.GetType(), out var selectable) &&
            selectable?.Tool == tool)
        {
            buttons[j].bounds.DrawBorder(Pixel.Value, ToolsModule.Config.SelectionBorderColor, b);
        }
    }

    #endregion injected subroutines
}
