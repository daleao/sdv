namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectDrawWhenHeldPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectDrawWhenHeldPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectDrawWhenHeldPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.drawWhenHeld), [typeof(SpriteBatch), typeof(Vector2), typeof(Farmer)]);
    }

    #region harmony patches

    /// <summary>Draw Piped Slime health.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ObjectDrawPostfix(
        SObject __instance,
        SpriteBatch spriteBatch,
        Vector2 objectPosition,
        Farmer f)
    {
        if (__instance.ItemId != PrismaticBrushId)
        {
            return;
        }

        var itemData = ItemRegistry.GetDataOrErrorItem(__instance.QualifiedItemId);
        var drawLayer = Math.Max(0f, (f.StandingPixel.Y + 3) / 10000f) + 0.01f;
        var offset = 1;
        var color = Utility.GetPrismaticColor(speedMultiplier: 2f);
        spriteBatch.Draw(
            itemData.GetTexture(),
            objectPosition,
            itemData.GetSourceRect(offset, __instance.ParentSheetIndex),
            color,
            0f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            drawLayer);
    }

    #endregion harmony patches
}
