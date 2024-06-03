namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotDrawInMenuPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotDrawInMenuPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal SlingshotDrawInMenuPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Slingshot>(
            nameof(Slingshot.drawInMenu),
            [
                typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
                typeof(StackDrawType), typeof(Color), typeof(bool),
            ]);
    }

    #region harmony patches

    /// <summary>Draw current ammo.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SlingshotPatchDrawInMenuPrefixTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Utility).RequireMethod(nameof(Utility.getWidthOfTinyDigitString))),
                ])
                .PatternMatch([new CodeInstruction(OpCodes.Brfalse_S)], ILHelper.SearchOption.Previous)
                .Move()
                .Insert(
                [
                    new CodeInstruction(OpCodes.Ldarg_0), // the slingshot
                        new CodeInstruction(OpCodes.Ldarg_1), // the sprite batch
                        new CodeInstruction(OpCodes.Ldarg_2), // the location
                        new CodeInstruction(OpCodes.Ldarg_3), // the scale size
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // the transparency
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)5), // the layer depth
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)7), // the color
                        new CodeInstruction(OpCodes.Call, typeof(SlingshotDrawInMenuPatcher).RequireMethod(nameof(DrawAmmo))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed drawing Bow ammo.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DrawAmmo(
        Slingshot instance,
        SpriteBatch b,
        Vector2 location,
        float scaleSize,
        float transparency,
        float layerDepth,
        Color color)
    {
        if (instance.attachments?[0] is not { } ammo || !Config.ShowEquippedAmmo)
        {
            return;
        }

        b.Draw(
            Game1.objectSpriteSheet,
            location + new Vector2(44f, 43f),
            Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ammo.ParentSheetIndex, 16, 16),
            color * transparency,
            0f,
            new Vector2(8f, 8f),
            scaleSize * 2.5f,
            SpriteEffects.None,
            layerDepth);
    }

    #endregion injected subroutines
}
