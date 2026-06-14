namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Professions.Framework.Events.Input.CursorMoved;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeDrawPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GreenSlimeDrawPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GreenSlime>(nameof(GreenSlime.draw), [typeof(SpriteBatch)]);
    }

    #region harmony patches

    /// <summary>Patch to draw Hat Slime.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool GreenSlimeDrawPrefix(
        NetBool ___avoidingMate,
        NetBool ___pursuingMate,
        NetVector2 ___facePosition,
        GreenSlime __instance,
        SpriteBatch b)
    {
        if (__instance.Get_Piped() is not { } piped)
        {
            return true; // run original logic
        }

        piped.Draw(b, ___avoidingMate.Value, ___pursuingMate.Value, ___facePosition.Value);
        return false; // don't run original logic

    }

    /// <summary>Draw Piped Slime health.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void GreenSlimeDrawPostfix(GreenSlime __instance, SpriteBatch b)
    {
        var location = __instance.currentLocation;
        if (location is null)
        {
            return;
        }

        var inDangerZone = location.IsEnemyArea() || location.Name.ContainsAnyOf("Mine", "SkullCave");

        Vector2 position;
        float width;
        float height;
        if (inDangerZone && __instance.Get_Piped() is { Source: PipedSlime.PipingSource.Summoned or PipedSlime.PipingSource.Charmed })
        {
            const float fullBarWidth = Game1.tileSize * 0.67f;
            position = __instance.getLocalPosition(Game1.viewport);
            position.Y += __instance.Sprite.SpriteHeight * 2.5f;
            var fillPercent = (float)__instance.Health / __instance.MaxHealth;
            width = fullBarWidth * fillPercent;
            position.X += (__instance.Sprite.SpriteWidth * 2) - (width / 2f) + 2;
            height = 4f;
            var color = Utility.getRedToGreenLerpColor(fillPercent);
            b.Draw(Game1.staminaRect, new Rectangle((int)position.X, (int)position.Y, (int)width, (int)height), color);
        }

        if (inDangerZone || (__instance != PiperVisionCursorMovedEvent.SlimeBeingHovered && !Config.ModKey.IsDown()))
        {
            return;
        }

        // implements Slime stat vision

        if (!Game1.player.HasProfession(Profession.Piper, true))
        {
            return;
        }

        // implements Slime color vision

        position = __instance.getLocalPosition(Game1.viewport);
        position.X += Game1.tileSize;
        width = Textures.Dots.Height * 3;
        height = Textures.Dots.Height * 3;

        // R:
        var sourceRect = new Rectangle(0, 0, 5, 5);
        position.Y -= __instance.Sprite.SpriteHeight / 2;
        b.Draw(
            Textures.Dots,
            new Rectangle((int)position.X, (int)position.Y, (int)width, (int)height),
            sourceRect,
            Color.White,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            1.1f);
        Utility.drawTinyDigits(
            __instance.color.R,
            b,
            position + new Vector2(width + 4, -2),
            3f,
            1f,
            Color.White);

        // G:
        sourceRect.X += 5;
        position.Y += height + 6;
        b.Draw(
            Textures.Dots,
            new Rectangle((int)position.X, (int)position.Y, (int)width, (int)height),
            sourceRect,
            Color.White,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            1.1f);
        Utility.drawTinyDigits(
            __instance.color.G,
            b,
            position + new Vector2(width + 4, -2),
            3f,
            1f,
            Color.White);

        // B:
        sourceRect.X += 5;
        position.Y += height + 6;
        b.Draw(
            Textures.Dots,
            new Rectangle((int)position.X, (int)position.Y, (int)width, (int)height),
            sourceRect,
            Color.White,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            1.1f);
        Utility.drawTinyDigits(
            __instance.color.B,
            b,
            position + new Vector2(width + 4, -2),
            3f,
            1f,
            Color.White);
    }

    #endregion harmony patches
}
