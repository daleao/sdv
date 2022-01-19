namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

using Stardew.Common.Harmony;
using AssetLoaders;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class CollectionsPageDrawPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal CollectionsPageDrawPatch()
    {
        Original = RequireMethod<CollectionsPage>(nameof(CollectionsPage.draw), new[] {typeof(SpriteBatch)});
    }

    #region harmony patches

    /// <summary>Patch to overlay MAX fish size indicator on the Collections page fish tab.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> CollectionsPageDrawTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: DrawMaxIcons(this, b)
        /// Before: b.End()

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(CollectionsPage).Field("hoverItem")),
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0), // this
                    new CodeInstruction(OpCodes.Ldarg_1), // SpriteBatch b
                    new CodeInstruction(OpCodes.Call,
                        typeof(CollectionsPageDrawPatch).MethodNamed(nameof(DrawMaxIcons)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching to draw collections page MAX icons. Helper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DrawMaxIcons(CollectionsPage page, SpriteBatch b)
    {
        var currentTab = page.currentTab;
        if (currentTab != CollectionsPage.fishTab) return;

        var currentPage = page.currentPage;
        foreach (var c in from c in page.collections[currentTab][currentPage]
                 let index = Convert.ToInt32(c.name.Split(' ')[0])
                 where Game1.player.HasCaughtMaxSized(index)
                 select c)
        {
            var destRect = new Rectangle(c.bounds.Right - Textures.MAX_ICON_WIDTH_I,
                c.bounds.Bottom - Textures.MAX_ICON_HEIGHT_I, Textures.MAX_ICON_WIDTH_I,
                Textures.MAX_ICON_HEIGHT_I);
            b.Draw(Textures.MaxIconTx, destRect, Color.White);
        }
    }

    #endregion injected subroutines
}