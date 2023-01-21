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
internal sealed class InventoryPageDrawPatcher : HarmonyPatcher
{
    private static readonly Lazy<Texture2D> Pixel = new(() =>
    {
        var pixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        pixel.SetData(new[] { Color.White });
        return pixel;
    });

    /// <summary>Initializes a new instance of the <see cref="InventoryPageDrawPatcher"/> class.</summary>
    internal InventoryPageDrawPatcher()
    {
        this.Target = this.RequireMethod<InventoryPage>(nameof(InventoryPage.draw), new[] { typeof(SpriteBatch) });
    }

    #region harmony patches

    /// <summary>Draw selectable indicator.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? InventoryPageDrawTranspiler(
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
                        new CodeInstruction(OpCodes.Ldfld, typeof(InventoryPage).RequireField("hoverText")),
                    })
                .StripLabels(out var labels)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(InventoryPageDrawPatcher).RequireMethod(nameof(DrawSelectors))),
                    },
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed drawing tool selectors in inventory.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DrawSelectors(InventoryPage __instance, SpriteBatch b)
    {
        if (ToolsModule.State.SelectableToolByType.Count == 0)
        {
            return;
        }

        for (var i = 0; i < __instance.inventory.inventory.Count; i++)
        {
            var component = __instance.inventory.inventory[i];
            var slotNumber = Convert.ToInt32(component.name);
            if (slotNumber >= Game1.player.Items.Count)
            {
                continue;
            }

            var item = Game1.player.Items[slotNumber];
            if (item is Tool tool &&
                ToolsModule.State.SelectableToolByType.TryGetValue(tool.GetType(), out var selectable) && selectable.HasValue)
            {
                component.bounds.DrawBorder(Pixel.Value, 3, ToolsModule.Config.SelectionBorderColor, b);
            }
        }
    }

    #endregion injected subroutines
}
