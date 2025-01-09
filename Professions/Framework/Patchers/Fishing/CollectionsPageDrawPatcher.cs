namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class CollectionsPageDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CollectionsPageDrawPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CollectionsPageDrawPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<CollectionsPage>(nameof(CollectionsPage.draw), [typeof(SpriteBatch)]);
    }

    #region harmony patches

    /// <summary>Patch to overlay MAX fish size indicator on the Collections page fish tab.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? CollectionsPageDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: DrawMaxIcons(this, b)
        // Before: if (hoverItem != null)
        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(CollectionsPage).RequireField("hoverItem")),
                        new CodeInstruction(OpCodes.Brfalse),
                    ])
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0), // this
                        new CodeInstruction(OpCodes.Ldarg_1), // SpriteBatch b
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(CollectionsPageDrawPatcher).RequireMethod(nameof(DrawMaxIcons))),
                    ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching to draw collections page MAX icons.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static void DrawMaxIcons(CollectionsPage page, SpriteBatch b)
    {
        if (!Config.ShowFishCollectionMaxIcon)
        {
            return;
        }

        var currentTab = page.currentTab;
        if (currentTab != CollectionsPage.fishTab)
        {
            return;
        }

        var currentPage = page.currentPage;
        for (var i = 0; i < page.collections[currentTab][currentPage].Count; i++)
        {
            var component = page.collections[currentTab][currentPage][i];
            var id = component.name.SplitWithoutAllocation(' ')[0].ToString();
            if (!Game1.player.HasCaughtMaxSized(id))
            {
                continue;
            }

            var sourceRect = new Rectangle(
                0,
                id.IsTrapFishId() ? Textures.MaxIcon.Height / 2 : 0,
                Textures.MaxIcon.Width,
                Textures.MaxIcon.Height / 2);
            var destRect = new Rectangle(
                component.bounds.Right - (Textures.MaxIcon.Width * 2),
                component.bounds.Bottom - Textures.MaxIcon.Height,
                Textures.MaxIcon.Width * 2,
                Textures.MaxIcon.Height);
            b.Draw(Textures.MaxIcon, destRect, sourceRect, Color.White);
        }
    }

    #endregion injections
}
