namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xTile.Tiles;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectDrawPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectDrawPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(
            nameof(SObject.draw),
            [typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)]);
    }

    #region harmony patches

    /// <summary>Patch to draw machine calibration tooltips.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ObjectDrawPostfix(SObject __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
    {
        var calibrations = __instance.Get_Calibrations();
        if (!calibrations.Any())
        {
            return;
        }

        var (machineTileX, machineTileY) = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize, (y - 1) * Game1.tileSize));
        bool isCursorHovering = false, isGamepadFacingMachine = false;
        if (Game1.options.gamepadControls)
        {
            var isNextToMachine = __instance.TileDistanceToPlayer(Game1.player) <= 1;
            var isFacingMachine = (__instance.TileLocation - Game1.player.Tile).ToFacingDirection() == (Direction)Game1.player.FacingDirection;
            isGamepadFacingMachine = isNextToMachine && isFacingMachine;
        }
        else
        {
            var bb = new Rectangle((int)machineTileX, (int)machineTileY, 64, 128);
            var (mouseX, mouseY) = Game1.getMousePosition();
            isCursorHovering = bb.Contains(mouseX, mouseY);
        }

        if (!isCursorHovering && !isGamepadFacingMachine)
        {
            return;
        }

        var tileCenterX = machineTileX + 32f;
        var totalWidth = 72f * calibrations.Count;
        var firstIconX = tileCenterX - (totalWidth / 2f) + 8f;
        var iconY = machineTileY - 16f;
        var i = 0;
        var drawLayer = Math.Max(0f, (((y + 1) * Game1.tileSize) - 24) / 10000f) + (x * 2e-5f);
        var calibrationChanged = Data.ReadAs<bool>(__instance, DataKeys.CalibrationChanged);
        var calibrationLocked = Data.ReadAs<bool>(__instance, DataKeys.CalibrationLocked);
        foreach (var (key, value) in calibrations)
        {
            var itemData = ItemRegistry.GetDataOrErrorItem(key);
            var color = Color.White;
            if (value == 100)
            {
                color = Color.Cyan;
            }
            else if (calibrationChanged)
            {
                color = key == __instance.lastInputItem.Value.QualifiedItemId ? Color.Lime : Color.DarkOrange;
            }
            else if (calibrationLocked)
            {
                color = Color.Yellow;
            }

            var iconX = firstIconX + (i * 72f);
            var destinationRect = new Rectangle(
                (int)iconX,
                (int)iconY,
                32,
                32);

            spriteBatch.Draw(
                itemData.GetTexture(),
                destinationRect,
                itemData.GetSourceRect(spriteIndex: itemData.SpriteIndex),
                Color.White * alpha,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                drawLayer);

            Utility.drawTinyDigits(
                value,
                spriteBatch,
                new Vector2(iconX + 32f, iconY + 8f),
                3f,
                drawLayer,
                color);

            i++;
        }
    }

    #endregion harmony patches
}
