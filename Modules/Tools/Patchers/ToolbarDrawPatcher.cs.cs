namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Xna;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(Toolbar).RequireField("hoverItem")),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(Toolbar).RequireField("buttons")),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ToolbarDrawPatcher).RequireMethod(nameof(DrawSelectors))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed drawing tool selectors in toolbar.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DrawSelectors(List<ClickableComponent> ___buttons, SpriteBatch b)
    {
        if (Game1.activeClickableMenu is not null || ToolsModule.State.SelectableToolByType.Count == 0)
        {
            return;
        }

        for (var i = 0; i < ___buttons.Count; i++)
        {
            var button = ___buttons[i];
            var slotNumber = Convert.ToInt32(button.name);
            if (slotNumber >= Game1.player.Items.Count)
            {
                continue;
            }

            var item = Game1.player.Items[slotNumber];
            if (item is Tool tool && Game1.player.CurrentTool != tool &&
                ToolsModule.State.SelectableToolByType.TryGetValue(tool.GetType(), out var selectable) &&
                selectable?.Tool == tool)
            {
                button.bounds.DrawBorder(Pixel.Value, 3, ToolsModule.Config.SelectionBorderColor, b);
            }
        }
    }

    #endregion injected subroutines
}
