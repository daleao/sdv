namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodDrawAttachmentsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodDrawAttachmentsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishingRodDrawAttachmentsPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishingRod>(nameof(FishingRod.drawAttachments));
    }

    #region harmony patches

    /// <summary>Draw tackle memory.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void FishingRodDrawAttachmentsPostfix(
        Tool __instance,
        SpriteBatch b,
        ref int x,
        ref int y)
    {
        if (__instance is not FishingRod rod || !rod.CanUseTackle() || !Game1.player.HasProfession(Profession.Angler))
        {
            return;
        }

        Item memorized;
        float transparency;
        var memorizedTackle = Data.Read(rod, DataKeys.FirstMemorizedTackle);
        var pixel = new Vector2(x, y);
        b.Draw(
            Game1.menuTexture,
            pixel,
            Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 57),
            Color.White,
            0f,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            0.86f);
        if (!string.IsNullOrEmpty(memorizedTackle))
        {
            memorized = ItemRegistry.Create<SObject>(memorizedTackle);
            transparency = 2f * Data.ReadAs<float>(rod, DataKeys.FirstMemorizedTackleUses) / FishingRod.maxTackleUses;
            memorized.drawInMenu(b, pixel, 1f, transparency, 0.9f);
        }
        else
        {
            b.Draw(
                Game1.menuTexture,
                pixel,
                Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 37),
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.86f);
        }

        if (rod.AttachmentSlotsCount <= 2 || !Game1.player.HasProfession(Profession.Angler, true))
        {
            return;
        }

        memorizedTackle = Data.Read(rod, DataKeys.SecondMemorizedTackle);
        pixel.X += 68;
        b.Draw(
            Game1.menuTexture,
            pixel,
            Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 57),
            Color.White,
            0f,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            0.86f);
        if (!string.IsNullOrEmpty(memorizedTackle))
        {
            memorized = ItemRegistry.Create<SObject>(memorizedTackle);
            transparency = 2f * Data.ReadAs<float>(rod, DataKeys.SecondMemorizedTackleUses) / FishingRod.maxTackleUses;
            memorized.drawInMenu(b, new Vector2(x + 68, y), 1f, transparency, 0.9f);
        }
        else
        {
            b.Draw(
                Game1.menuTexture,
                pixel,
                Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 37),
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.86f);
        }
    }

    #endregion harmony patches
}
