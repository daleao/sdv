namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
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
    internal CollectionsPageDrawPatcher()
    {
        this.Target = this.RequireMethod<CollectionsPage>(nameof(CollectionsPage.draw), new[] { typeof(SpriteBatch) });
    }

    #region harmony patches

    /// <summary>Patch to overlay MAX fish size indicator on the Collections page fish tab.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? CollectionsPageDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: DrawMaxIcons(this, b)
        // Before: b.End()
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(CollectionsPage).RequireField("hoverItem")),
                        new CodeInstruction(OpCodes.Brfalse_S),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0), // this
                        new CodeInstruction(OpCodes.Ldarg_1), // SpriteBatch b
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(CollectionsPageDrawPatcher).RequireMethod(nameof(DrawMaxIcons))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching to draw collections page MAX icons.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DrawMaxIcons(CollectionsPage page, SpriteBatch b)
    {
        if (!ProfessionsModule.Config.ShowFishCollectionMaxIcon)
        {
            return;
        }

        var currentTab = page.currentTab;
        if (currentTab != CollectionsPage.fishTab)
        {
            return;
        }

        var currentPage = page.currentPage;
        foreach (var c in from c in page.collections[currentTab][currentPage]
                 let index = int.Parse(c.name.SplitWithoutAllocation(' ')[0])
                 where Game1.player.HasCaughtMaxSized(index)
                 select c)
        {
            var destRect = new Rectangle(
                c.bounds.Right - (Textures.MaxIconTx.Width * 2),
                c.bounds.Bottom - (Textures.MaxIconTx.Height * 2),
                Textures.MaxIconTx.Width * 2,
                Textures.MaxIconTx.Height * 2);
            b.Draw(Textures.MaxIconTx, destRect, Color.White);
        }
    }

    #endregion injected subroutines
}
