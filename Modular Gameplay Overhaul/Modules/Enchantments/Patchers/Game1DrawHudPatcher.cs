namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1DrawHudPatcher : HarmonyPatcher
{
    private const int DefaultBarHeight = 168;

    /// <summary>Initializes a new instance of the <see cref="Game1DrawHudPatcher"/> class.</summary>
    internal Game1DrawHudPatcher()
    {
        this.Target = this.RequireMethod<Game1>("drawHUD");
    }

    #region harmony patches

    /// <summary>Patch draw over-healed health.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? Game1DrawHUDTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: DrawHealthBar(topOfBar);
        // In place of vanilla health draw logic...
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(
                            OpCodes.Stsfld,
                            typeof(Game1).RequireField(nameof(Game1.showingHealth))),
                    })
                .Match(
                    new[] { new CodeInstruction(OpCodes.Bge) },
                    ILHelper.SearchOption.Previous)
                .SetOpCode(OpCodes.Beq) // replace > with ==
                .Return()
                .Move(2)
                .Count(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Callvirt),
                        new CodeInstruction(OpCodes.Br_S),
                    },
                    out var count)
                .Remove(count)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_1),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Game1DrawHudPatcher).RequireMethod(nameof(DrawHealthBarSubroutine))),
                    });
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
        var bonusHeight = player.maxHealth - 100;
        var overhealHeight = 0;
        if (player.health > player.maxHealth)
        {
            overhealHeight = (int)(DefaultBarHeight * (float)player.health / player.maxHealth) - DefaultBarHeight;
            bonusHeight += overhealHeight;
        }

        var barFullHeight = DefaultBarHeight + bonusHeight;
        var height = (int)(Math.Min(player.health, player.maxHealth) / (float)player.maxHealth * barFullHeight);
        topOfBar.X -= 56 + (Game1.hitShakeTimer > 0 ? Game1.random.Next(-3, 4) : 0);
        topOfBar.Y = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 240 - bonusHeight;

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
            new Vector2(topOfBar.X, topOfBar.Y + 224f + bonusHeight - 64f),
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

        if (overhealHeight > 0)
        {
            healthBarRect.Height = overhealHeight;
            Game1.spriteBatch.Draw(
                Game1.staminaRect,
                healthBarRect,
                Game1.staminaRect.Bounds,
                new Color(240, 240, 240),
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                1f);

            healthBarRect.Height = 4;
            Game1.spriteBatch.Draw(
                Game1.staminaRect,
                healthBarRect,
                Game1.staminaRect.Bounds,
                new Color(200, 200, 200),
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                1f);
        }
        else
        {
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
        }

        if (RingsModule.ShouldEnable)
        {
            Rings.Patchers.Game1DrawHudPatcher.DrawShieldHealth(topOfBar, healthBarRect);
        }
    }

    #endregion injected subroutines
}
