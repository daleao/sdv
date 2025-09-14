namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectDrawInMenuPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectDrawInMenuPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectDrawInMenuPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(
            nameof(SObject.drawInMenu),
            [
                typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float),
                typeof(StackDrawType), typeof(Color), typeof(bool),
            ]);
    }

    #region harmony patches

    /// <summary>Draw Piped Slime health.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ObjectDrawInMenuPostfix(
        SObject __instance,
        SpriteBatch spriteBatch,
        Vector2 location,
        float scaleSize,
        float transparency,
        float layerDepth)
    {
        if (__instance.ItemId != PrismaticBrushId)
        {
            return;
        }

        var itemData = ItemRegistry.GetDataOrErrorItem(__instance.QualifiedItemId);
        var offset = 1;
        var sourceRect = itemData.GetSourceRect(offset, __instance.ParentSheetIndex);
        var color = Utility.GetPrismaticColor(speedMultiplier: 2f);
        spriteBatch.Draw(
            itemData.GetTexture(),
            location + new Vector2(32f, 32f),
            sourceRect,
            color * transparency,
            0f,
            new Vector2(sourceRect.Width / 2, sourceRect.Height / 2),
            4f * scaleSize,
            SpriteEffects.None,
            layerDepth);
    }

    #endregion harmony patches
}
