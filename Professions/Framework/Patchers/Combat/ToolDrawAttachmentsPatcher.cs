namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolDrawAttachmentsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolDrawAttachmentsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ToolDrawAttachmentsPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.drawAttachments));
    }

    #region harmony patches

    /// <summary>Patch to draw Rascal's additional ammo slot.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool SlingshotDrawAttachmentsPrefix(Tool __instance, SpriteBatch b, int x, int y)
    {
        if (__instance is not Slingshot)
        {
            return true; // run original logic
        }

        if (__instance.AttachmentSlotsCount != __instance.attachments.Length)
        {
            Log.W("The slingshot instance's attachments slots are corrupted. Please run the console command `prfs fix_slingshots` to resolve.");
            return false; // don't run original logic
        }

        y += __instance.enchantments.Any() ? 8 : 4;
        var pixel = new Vector2(x, y);
        for (var slot = 0; slot < __instance.AttachmentSlotsCount; slot++)
        {
            if (slot == 1 && !Game1.player.HasProfession(Profession.Rascal))
            {
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
            }

            b.Draw(
                Game1.menuTexture,
                pixel,
                Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, __instance.attachments[slot] is null ? 43 : 10),
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.86f);
            __instance.attachments[slot]?.drawInMenu(
                b,
                pixel,
                1f);
            pixel.X += 68;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
