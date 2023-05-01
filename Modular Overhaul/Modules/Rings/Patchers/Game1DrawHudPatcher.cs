namespace DaLion.Overhaul.Modules.Rings.Patchers;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        if (EnchantmentsModule.ShouldEnable)
        {
            return null;
        }

        var helper = new ILHelper(original, instructions);

        // Injected: DrawShieldBar(topOfBar, height);
        // After vanilla health draw logic...
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(
                            OpCodes.Stsfld,
                            typeof(Game1).RequireField(nameof(Game1.showingHealth))),
                    })
                .Move(-1)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_1),
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[8]),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Game1DrawHudPatcher).RequireMethod(nameof(DrawShieldHealth))),
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

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Cross-module compatibility.")]
    internal static void DrawShieldHealth(Vector2 topOfBar, Rectangle healthBarRect)
    {
        if (!Game1.buffsDisplay.hasBuff(21) || RingsModule.State.YobaShieldHealth <= 0)
        {
            return;
        }

        var player = Game1.player;
        var fullBarHeight = player.maxHealth + 68;
        var shieldBarRect = healthBarRect;
        shieldBarRect.Height = (int)(RingsModule.State.YobaShieldHealth / (player.maxHealth * 0.5) * fullBarHeight * 0.5);
        shieldBarRect.Y -= shieldBarRect.Height - 4;
        Game1.spriteBatch.Draw(
            Game1.staminaRect,
            shieldBarRect,
            Game1.staminaRect.Bounds,
            new Color(240, 240, 240),
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            1f);

        shieldBarRect.Height = 4;
        Game1.spriteBatch.Draw(
            Game1.staminaRect,
            shieldBarRect,
            Game1.staminaRect.Bounds,
            new Color(200, 200, 200),
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            1f);

        if (Game1.getOldMouseX() >= topOfBar.X && Game1.getOldMouseY() >= topOfBar.Y &&
            Game1.getOldMouseX() < topOfBar.X + 32f)
        {
            Game1.drawWithBorder(
                Math.Max(0, RingsModule.State.YobaShieldHealth) + "/" + (int)(player.maxHealth * 0.5),
                Color.Black * 0f,
                Color.Red,
                topOfBar + new Vector2(0f - Game1.dialogueFont.MeasureString("999/999").X - 32f, 64f));
        }
    }

    #endregion injected subroutines
}
