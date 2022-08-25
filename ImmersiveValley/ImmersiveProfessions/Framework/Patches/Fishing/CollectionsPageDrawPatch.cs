namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Textures;

#endregion using directives

[UsedImplicitly]
internal sealed class CollectionsPageDrawPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal CollectionsPageDrawPatch()
    {
        Target = RequireMethod<CollectionsPage>(nameof(CollectionsPage.draw), new[] { typeof(SpriteBatch) });
    }

    #region harmony patches

    /// <summary>Patch to overlay MAX fish size indicator on the Collections page fish tab.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? CollectionsPageDrawTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: DrawMaxIcons(this, b)
        /// Before: b.End()

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(CollectionsPage).RequireField("hoverItem")),
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0), // this
                    new CodeInstruction(OpCodes.Ldarg_1), // SpriteBatch b
                    new CodeInstruction(OpCodes.Call,
                        typeof(CollectionsPageDrawPatch).RequireMethod(nameof(DrawMaxIcons)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching to draw collections page MAX icons.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DrawMaxIcons(CollectionsPage page, SpriteBatch b)
    {
        if (!ModEntry.Config.ShowFishCollectionMaxIcon) return;

        var currentTab = page.currentTab;
        if (currentTab != CollectionsPage.fishTab) return;

        var currentPage = page.currentPage;
        foreach (var c in from c in page.collections[currentTab][currentPage]
                          let index = Convert.ToInt32(c.name.Split(' ')[0])
                          where Game1.player.HasCaughtMaxSized(index)
                          select c)
        {
            var destRect = new Rectangle(c.bounds.Right - Textures.MaxIconTx.Width * 2,
                c.bounds.Bottom - Textures.MaxIconTx.Height * 2, Textures.MaxIconTx.Width * 2,
                Textures.MaxIconTx.Height * 2);
            b.Draw(Textures.MaxIconTx, destRect, Color.White);
        }
    }

    #endregion injected subroutines
}