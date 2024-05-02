namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotDrawAttachmentsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotDrawAttachmentsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal SlingshotDrawAttachmentsPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.drawAttachments));
    }

    #region harmony patches

    /// <summary>Patch to draw Rascal's additional ammo slot.</summary>
    [HarmonyPostfix]
    private static void SlingshotDrawAttachmentsPostfix(Tool __instance, SpriteBatch b, int x, int y)
    {
        if (__instance is not Slingshot { AttachmentSlotsCount: 2 })
        {
            return;
        }

        b.Draw(
            Game1.menuTexture,
            new Vector2(x, y + 68),
            Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, __instance.attachments[1] is null ? 43 : 10),
            Color.White,
            0f,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            0.86f);

        __instance.attachments[1]?.drawInMenu(
            b,
            new Vector2(x, y + 68),
            1f);
    }

    #endregion harmony patches
}
