namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotDrawInMenuPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotDrawInMenuPatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.drawInMenu),
            new[]
            {
                typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
                typeof(StackDrawType), typeof(Color), typeof(bool)
            });
    }

    #region harmony patches

    /// <summary>Draw slingshot cooldown.</summary>
    [HarmonyPostfix]
    private static void SlingshotDrawInMenuPostfix(Slingshot __instance, SpriteBatch spriteBatch, Vector2 location,
        float scaleSize, StackDrawType drawStackNumber, bool drawShadow)
    {
        if (ModEntry.SlingshotCooldown.Value <= 0) return;

        var cooldownLevel = ModEntry.SlingshotCooldown.Value / Constants.SLINGSHOT_COOLDOWN_TIME_I;
        var drawingAsDebris = drawShadow && drawStackNumber == StackDrawType.Hide;
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (!drawShadow || drawingAsDebris || Game1.activeClickableMenu is ShopMenu && scaleSize == 1f) return;

        var (x, y) = location;
        spriteBatch.Draw(Game1.staminaRect,
            new Rectangle((int)x, (int)y + (Game1.tileSize - (int)(cooldownLevel * Game1.tileSize)),
                Game1.tileSize, (int)(cooldownLevel * Game1.tileSize)), Color.Red * 0.66f);
    }

    #endregion harmony patches
}