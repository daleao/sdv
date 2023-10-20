namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotDrawInMenuPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotDrawInMenuPatcher"/> class.</summary>
    internal SlingshotDrawInMenuPatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(
            nameof(Slingshot.drawInMenu),
            new[]
            {
                typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
                typeof(StackDrawType), typeof(Color), typeof(bool),
            });
    }

    #region harmony patches

    /// <summary>Draw slingshot cooldown.</summary>
    [HarmonyPrefix]
    private static void SlingshotDrawInMenuPrefix(
        ref float scaleSize)
    {
        scaleSize *= 1f + CombatModule.State.SlingshotAddedScale;
    }

    /// <summary>Draw slingshot cooldown.</summary>
    [HarmonyPostfix]
    private static void SlingshotDrawInMenuPostfix(
        Slingshot __instance,
        SpriteBatch spriteBatch,
        Vector2 location,
        float scaleSize,
        StackDrawType drawStackNumber,
        bool drawShadow)
    {
        if (CombatModule.State.SlingshotGatlingTimer <= 0 && CombatModule.State.SlingshotCooldown <= 0)
        {
            return;
        }

        var blockFillPct = CombatModule.State.SlingshotGatlingTimer > 0
                ? (float)CombatModule.State.SlingshotGatlingTimer / __instance.GetSpecialDuration()
                : (float)CombatModule.State.SlingshotCooldown / __instance.GetSpecialCooldown();
        var drawingAsDebris = drawShadow && drawStackNumber == StackDrawType.Hide;

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (!drawShadow || drawingAsDebris || (Game1.activeClickableMenu is ShopMenu && scaleSize == 1f))
        {
            return;
        }

        var (x, y) = location;
        spriteBatch.Draw(
            Game1.staminaRect,
            new Rectangle(
                (int)x,
                (int)(y + (Game1.tileSize - (blockFillPct * Game1.tileSize))),
                Game1.tileSize,
                (int)(blockFillPct * Game1.tileSize)),
            Color.Red * 0.66f);
    }

    /// <summary>Draw current ammo.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SlingshotPatchDrawInMenuPrefixTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .Match(new[]
                {
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Utility).RequireMethod(nameof(Utility.getWidthOfTinyDigitString))),
                })
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) }, ILHelper.SearchOption.Previous)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0), // the slingshot
                        new CodeInstruction(OpCodes.Ldarg_1), // the sprite batch
                        new CodeInstruction(OpCodes.Ldarg_2), // the location
                        new CodeInstruction(OpCodes.Ldarg_3), // the scale size
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // the transparency
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)5), // the layer depth
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)7), // the color
                        new CodeInstruction(OpCodes.Call, typeof(SlingshotDrawInMenuPatcher).RequireMethod(nameof(DrawAmmo))),
                    });
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
        if (!CombatModule.Config.DrawCurrentAmmo || instance.attachments?[0] is not { } ammo)
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
