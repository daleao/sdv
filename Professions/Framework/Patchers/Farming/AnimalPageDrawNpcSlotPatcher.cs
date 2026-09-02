namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using static StardewValley.Menus.AnimalPage;

#endregion using directives

[UsedImplicitly]
internal sealed class AnimalPageDrawNpcSlotPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AnimalPageDrawNpcSlotPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal AnimalPageDrawNpcSlotPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = typeof(AnimalPage).RequireMethod("drawNPCSlot");
    }

    #region harmony patches

    /// <summary>Patch to draw Crop Feed icon in Animal Page menu.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? AnimalPageDrawNpcSlotTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .GoTo(helper.LastIndex)
                .StripLabels(out var labels)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Ldarg_2),
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Ldloc_3),
                        new CodeInstruction(OpCodes.Call, typeof(AnimalPageDrawNpcSlotPatcher).RequireMethod(nameof(DrawCropFeedCheckbox)))
                    ],
                    labels);
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

    private static void DrawCropFeedCheckbox(AnimalPage page, SpriteBatch b, int i, AnimalEntry entry, int yOffset)
    {
        if (entry.AnimalType == "Horse" || !Game1.player.HasProfession(Profession.Rancher))
        {
            return;
        }

        var wasFedCropYet = Data.ReadAs<bool>(entry.Animal, DataKeys.WasSupplementedToday) ? 2 : 0;
        var xOffset = 84;
        if (entry.AnimalType is "Cat")
        {
            b.Draw(
                Game1.mouseCursors,
                new Vector2(page.xPositionOnScreen + 704 - 4 + xOffset, page.sprites[i].bounds.Y + yOffset + 64 - 52),
                new Rectangle(20, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                0.8f);
        }
        else
        {
            b.Draw(
                Game1.mouseCursors,
                new Vector2(page.xPositionOnScreen + 704 - 4 + xOffset, page.sprites[i].bounds.Y + yOffset + 64 - 52),
                new Rectangle(10, 428, 10, 10),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                0.8f);
        }

        b.Draw(
            Game1.mouseCursors_1_6,
            new Vector2(page.xPositionOnScreen + 704 - 4 + xOffset, page.sprites[i].bounds.Y + yOffset + 64 - 8),
            new Rectangle(273 + (wasFedCropYet * 9), 253, 9, 9),
            Color.White,
            0f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            0.8f);
    }

    #endregion injected
}
