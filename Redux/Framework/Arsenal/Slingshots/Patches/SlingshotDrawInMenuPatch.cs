namespace DaLion.Redux.Framework.Arsenal.Slingshots.Patches;

#region using directives

using DaLion.Shared.Attributes;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Deprecated]
internal sealed class SlingshotDrawInMenuPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotDrawInMenuPatch"/> class.</summary>
    internal SlingshotDrawInMenuPatch()
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
    [HarmonyPostfix]
    private static void SlingshotDrawInMenuPostfix(
        Slingshot __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, StackDrawType drawStackNumber, bool drawShadow)
    {
        if (drawStackNumber != 0 && __instance.numAttachmentSlots.Value > 1 && __instance.attachments[1] is not null)
        {
            Utility.drawTinyDigits(
                __instance.attachments[1].Stack,
                spriteBatch,
                location + new Vector2(64 - Utility.getWidthOfTinyDigitString(__instance.attachments[1].Stack, 3f * scaleSize) + (3f * scaleSize), 64f - (18f * scaleSize) + 2f),
                3f * scaleSize,
                1f,
                Color.White);
        }

        if (ModEntry.State.Arsenal.SlingshotCooldown <= 0)
        {
            return;
        }

        var cooldown = ModEntry.State.Arsenal.SlingshotCooldown / Constants.SlingshotCooldownTime;
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
                (int)y + (Game1.tileSize - (cooldown * Game1.tileSize)),
                Game1.tileSize,
                cooldown * Game1.tileSize),
            Color.Red * 0.66f);
    }

    #endregion harmony patches
}
