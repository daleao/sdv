namespace DaLion.Redux.Framework.Arsenal.Weapons.Patches;

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
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1DrawHUDPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="Game1DrawHUDPatch"/> class.</summary>
    internal Game1DrawHUDPatch()
    {
        this.Target = this.RequireMethod<Game1>("drawHUD");
    }

    #region harmony patches

    /// <summary>Patch draw over-healed health.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? Game1DrawHUDTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: DrawHealthBar(topOfBar);
        // In place of vanilla health draw logic...
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Stsfld, typeof(Game1).RequireField(nameof(Game1.showingHealth))))
                .Advance(2)
                .RemoveInstructionsUntil(
                    new CodeInstruction(OpCodes.Callvirt),
                    new CodeInstruction(OpCodes.Br_S))
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Game1DrawHUDPatch).RequireMethod(nameof(DrawHealthBarSubroutine))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding player over-heal to the HUD.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DrawHealthBarSubroutine(Vector2 topOfBar)
    {
        var player = Game1.player;

        var barFullHeight = 168 + (player.maxHealth - 100);
        if (player.health > player.maxHealth)
        {
            barFullHeight += player.health - player.maxHealth;
        }

        var height = player.health > player.maxHealth
            ? (int)(player.health / (float)player.maxHealth * barFullHeight)
            : barFullHeight;
        topOfBar.X -= 56 + (Game1.hitShakeTimer > 0 ? Game1.random.Next(-3, 4) : 0);
        topOfBar.Y = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Bottom - 224 - 16 -
                     (player.maxHealth - 100);
        if (player.health > player.maxHealth)
        {
            var delta = (int)((player.health / (float)player.maxHealth) - 1) * barFullHeight;
            height += delta;
            topOfBar.Y += delta;
        }

        var color = player.health < 20
            ? Color.Pink * (((float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / (player.health * 50f)) / 4f) + 0.9f)
            : Color.White;
        Game1.spriteBatch.Draw(
            Game1.mouseCursors,
            topOfBar,
            new Rectangle(268, 408, 12, 16),
            color,
            0f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            1f);

        color = player.health < 20
            ? Color.Pink * (((float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / (player.health * 50f)) / 4f) + 0.9f)
            : Color.White;
        Game1.spriteBatch.Draw(
            Game1.mouseCursors,
            new Rectangle(
                (int)topOfBar.X,
                (int)(topOfBar.Y + 64f),
                48,
                Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Bottom - 64 - 16 - (int)(topOfBar.Y + 64f)),
            new Rectangle(268, 424, 12, 16),
            color);

        color = player.health < 20
            ? Color.Pink * (((float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / (player.health * 50f)) / 4f) + 0.9f)
            : Color.White;
        Game1.spriteBatch.Draw(
            Game1.mouseCursors,
            new Vector2(topOfBar.X, topOfBar.Y + 224f + (player.maxHealth - 100) - 64f),
            new Rectangle(268, 448, 12, 16),
            color,
            0f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            1f);

        var healthBarRect = new Rectangle(
            (int)topOfBar.X + 12,
            (int)topOfBar.Y + 16 + 32 + barFullHeight - height,
            24,
            height);
        color = Utility.getRedToGreenLerpColor(player.health / (float)player.maxHealth);
        Game1.spriteBatch.Draw(
            Game1.staminaRect,
            healthBarRect,
            Game1.staminaRect.Bounds,
            color,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            1f);
        color.R = (byte)Math.Max(0, color.R - 50);
        color.G = (byte)Math.Max(0, color.G - 50);
        if (Game1.getOldMouseX() >= topOfBar.X && Game1.getOldMouseY() >= topOfBar.Y &&
            Game1.getOldMouseX() < topOfBar.X + 32f)
        {
            Game1.drawWithBorder(
                Math.Max(0, player.health) + "/" + player.maxHealth,
                Color.Black * 0f,
                Color.Red,
                topOfBar + new Vector2(0f - Game1.dialogueFont.MeasureString("999/999").X - 32f, 64f));
        }

        healthBarRect.Height = 4;
        Game1.spriteBatch.Draw(
            Game1.staminaRect,
            healthBarRect,
            Game1.staminaRect.Bounds,
            color,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            1f);

        if (player.health <= player.maxHealth)
        {
            return;
        }

        var overhealHeight = player.health - player.maxHealth;
        healthBarRect.Height = overhealHeight;
        color = color.ChangeValue(0.2f);
        Game1.spriteBatch.Draw(
            Game1.staminaRect,
            healthBarRect,
            Game1.staminaRect.Bounds,
            color,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            1f);
    }

    #endregion injected subroutines
}
