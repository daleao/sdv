using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using TheLion.Stardew.Professions.Framework.Utility;

namespace TheLion.Stardew.Professions.Framework.Patches.Fishing;

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
            ModEntry.Log($"Failed while patching to draw collections page MAX icons. Helper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

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
            var destRect = new Rectangle(c.bounds.Right - Textures.MaxIconWidth,
                c.bounds.Bottom - Textures.MaxIconHeight, Textures.MaxIconWidth,
                Textures.MaxIconHeight);
            b.Draw(Textures.MaxIconTx, destRect, Color.White);
        }
    }

    #endregion private methods
}